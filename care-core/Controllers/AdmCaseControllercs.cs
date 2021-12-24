using System;
using System.Collections.Generic;
using System.Transactions;
using care_core.Controllers.util;
using care_core.dto.AdmCase;
using care_core.dto.AdmTypology;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;

namespace care_core.Controllers
{
    [Route("rest/cases")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmCaseController : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmCase _admCase;
        private readonly IAdmPerson _admPerson;
        private readonly IAdmOrganization _admOrganization;

        private readonly ILogger<AdmCaseController> _logger;
        private JsonResponse response;

        public AdmCaseController(
            EntityDbContext dbContext,
            IAdmCase admCase,
            IAdmPerson admPerson,
            IAdmTypology admTypology,
            IAdmOrganization admOrganization,
            ILogger<AdmCaseController> logger)
        {
            _dbContext = dbContext;
            _admCase = admCase;
            _admPerson = admPerson;
            _admOrganization = admOrganization;
            _logger = logger;
            response = new JsonResponse();
        }

        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] int estado)
        {
            IEnumerable<AdmCaseDto> admCaseDtos = _admCase.getAll(estado);
            return new OkObjectResult(admCaseDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetByCaseId([FromRoute] int id)
        {
            AdmCaseDto caseDto = _admCase.getCaseById(id);
            return new OkObjectResult(caseDto);
        }

        [HttpPost]
        public IActionResult Post([FromBody] AdmCaseDto caseDto)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    /*verifying dto*/

                    //created by
                    if (caseDto.created_by == null || caseDto.created_by.user_id == 0)
                    {
                        response.msg = "Created by incorrecto";
                        response.id = 0;
                        response.code = "0";
                        return new BadRequestObjectResult(response);
                    }

                    AdmUser createdByUser = _dbContext.admUsers.Find(caseDto.created_by.user_id);
                    if (createdByUser == null)
                    {
                        response.msg = "Usuario no encontrado";
                        response.id = 0;
                        response.code = "0";
                        return new BadRequestObjectResult(response);
                    }

                    //recording organization
                    if (caseDto.recording_organization == null || caseDto.recording_organization.organization_id == 0)
                    {
                        response.msg = "Organizacion incorrecta";
                        response.id = 0;
                        response.code = "0";
                        return new BadRequestObjectResult(response);
                    }

                    AdmOrganization recordedByOrganization =
                        _dbContext.admOrganizations.Find(caseDto.recording_organization.organization_id);
                    if (recordedByOrganization == null)
                    {
                        response.msg = "Organizacion no encontrada";
                        response.id = 0;
                        response.code = "0";
                        return new BadRequestObjectResult(response);
                    }

                    //recorded by
                    if (caseDto.recorded_by == null || caseDto.recorded_by.organization_member_id == 0)
                    {
                        response.msg = "Miembro de Organizacion incorrecto";
                        response.id = 0;
                        response.code = "0";
                        return new BadRequestObjectResult(response);
                    }

                    AdmOrganizationMember recordedByMember =
                        _dbContext.admOrganizationMembers.Find(caseDto.recorded_by.organization_member_id);
                    if (recordedByMember == null)
                    {
                        response.msg = "Miembro de Organizacion no encontrado";
                        response.id = 0;
                        response.code = "0";
                        return new BadRequestObjectResult(response);
                    }

                    //victim name
                    if (caseDto.victim == null || caseDto.victim.first_name == null || caseDto.victim.last_name == null)
                    {
                        response.msg = "El nombre / apellido de la victima es obligatorio";
                        response.id = 0;
                        response.code = "0";
                        return new BadRequestObjectResult(response);
                    }

                    //victim dpi
                    if (caseDto.victim.cui == 0)
                    {
                        response.msg = "El CUI de la victima es incorrecto";
                        response.id = 0;
                        response.code = "0";
                        return new BadRequestObjectResult(response);
                    }
                    
                    AdmPerson foundPerson = _admPerson.findByDpi( caseDto.victim.cui);
                    if (foundPerson != null)
                    {
                        response.msg = "DPI de la victima ya esta en uso";
                        response.id = 0;
                        response.code = "0";
                        return new BadRequestObjectResult(response);
                    }

                    //victim email
                    if (string.IsNullOrEmpty(caseDto.victim.email) || caseDto.victim.email.Equals(""))
                    {
                        response.msg = "El Email de la victima es incorrecto";
                        response.id = 0;
                        response.code = "0";
                        return new BadRequestObjectResult(response);
                    }

                    foundPerson = _admPerson.findByEmail(caseDto.victim.email, true);
                    if (foundPerson != null)
                    {
                        response.msg = "Email ya esta en uso";
                        response.id = 0;
                        response.code = "0";
                        return new BadRequestObjectResult(response);
                    }


                    //checking typologies
                    AdmTypology emptyTypology = _dbContext.admTypologies.Find(CareConstants.EMPTY_TYPOLOGY);

                    AdmTypology culturalIdentityTypo = checkTypology(caseDto.victim.cultural_identity, emptyTypology);
                    AdmTypology stateTypo = checkTypology(caseDto.victim.state, emptyTypology);
                    AdmTypology cityTypo = checkTypology(caseDto.victim.city, emptyTypology);
                    AdmTypology occupationTypo = checkTypology(caseDto.victim.occupation, emptyTypology);
                    AdmTypology maritalStatusTypo = checkTypology(caseDto.victim.marital_status, emptyTypology);
                    AdmTypology educationTypo = checkTypology(caseDto.victim.education, emptyTypology);
                    AdmTypology spokenLanguageTypo = checkTypology(caseDto.victim.spoken_language, emptyTypology);
                    AdmTypology genreTypo = checkTypology(caseDto.victim.genre, emptyTypology);
                    AdmTypology statusTypo = checkTypology(caseDto.victim.status, emptyTypology);


                    AdmPerson victimPerson = new AdmPerson()
                    {
                        person_id = CareConstants.ZERO_DEFAULT,
                        first_name = caseDto.victim.first_name,
                        last_name = caseDto.victim.last_name,
                        birthday = caseDto.victim.birthday ?? CareConstants.DATE_TIME_NO_TIMEZONE,
                        phone_number = caseDto.victim.phone_number ?? CareConstants.ZERO_DEFAULT,
                        cui = caseDto.victim.cui,
                        cultural_identity = culturalIdentityTypo,
                        state = stateTypo,
                        city = cityTypo,
                        occupation = occupationTypo,
                        marital_status = maritalStatusTypo,
                        education = educationTypo,
                        spoken_language = spokenLanguageTypo,
                        address_line = caseDto.victim.address_line ?? CareConstants.NO_DESCRIPTION,
                        email = caseDto.victim.email,
                        daughters_no = caseDto.victim.daughters_no ?? CareConstants.ZERO_DEFAULT,
                        sons_no = caseDto.victim.sons_no ?? CareConstants.ZERO_DEFAULT,
                        genre = genreTypo,
                        status = statusTypo,
                        date_created = CsnFunctions.now()
                    };

                    //veryfing case
                    //case typologies
                    AdmTypology aggressorStateTypo = checkTypology(caseDto.aggressor_state, emptyTypology);
                    AdmTypology aggressorCityTypo = checkTypology(caseDto.aggressor_city, emptyTypology);
                    AdmTypology aggressorOccupationTypo = checkTypology(caseDto.aggressor_occupation, emptyTypology);
                    AdmTypology aggressorMaritalStatusTypo =
                        checkTypology(caseDto.aggressor_marital_status, emptyTypology);
                    AdmTypology victimRelationshipTypo = checkTypology(caseDto.victim_relationship, emptyTypology);
                    AdmTypology identifiedViolenceTypeTypo =
                        checkTypology(caseDto.identified_violence_type, emptyTypology);
                    AdmTypology accompanimentTypeTypo = checkTypology(caseDto.accompaniment_type, emptyTypology);
                    AdmTypology institutionTypo = checkTypology(caseDto.institution, emptyTypology);
                    AdmTypology accompanimentRouteTypo = checkTypology(caseDto.accompaniment_route, emptyTypology);
                    AdmTypology caseCategoryTypo = checkTypology(caseDto.case_category, emptyTypology);
                    AdmTypology caseStatusTypo = checkTypology(caseDto.status, emptyTypology);

                    AdmCase caseObject = new AdmCase()
                    {
                        case_id = CareConstants.ZERO_DEFAULT,
                        aggressor_first_name = caseDto.aggressor_first_name ?? CareConstants.NO_DESCRIPTION,
                        aggressor_last_name = caseDto.aggressor_last_name ?? CareConstants.NO_DESCRIPTION,
                        aggressor_birthday = caseDto.aggressor_birthday,
                        aggressor_phone_number = caseDto.aggressor_phone_number,
                        aggressor_cui = caseDto.aggressor_cui,
                        aggressor_state = aggressorStateTypo,
                        aggressor_address_line = caseDto.aggressor_address_line ?? CareConstants.NO_DESCRIPTION,
                        aggressor_city = aggressorCityTypo,
                        aggressor_occupation = aggressorOccupationTypo,
                        aggressor_marital_status = aggressorMaritalStatusTypo,
                        aggressor_work_place = caseDto.aggressor_work_place ?? CareConstants.NO_DESCRIPTION,
                        act_date = caseDto.act_date,
                        act_place = caseDto.act_place,
                        victim_relationship = victimRelationshipTypo,
                        act_description = caseDto.act_description ?? CareConstants.NO_DESCRIPTION,
                        identified_violence_type = identifiedViolenceTypeTypo,
                        lives_with_aggressor = caseDto.lives_with_aggressor,
                        accompaniment_type = accompanimentTypeTypo,
                        institution = institutionTypo,
                        accompaniment_route = accompanimentRouteTypo,
                        annotation = caseDto.annotation ?? CareConstants.NO_DESCRIPTION,
                        recording_organization = recordedByOrganization,
                        recorded_by_member = recordedByMember,
                        victim = victimPerson,
                        case_category = caseCategoryTypo,
                        attached_victim_dpi = caseDto.attached_victim_dpi,
                        attached_victim_birth_certificate = caseDto.attached_victim_birth_certificate,
                        attached_children_birth_certificate = caseDto.attached_children_birth_certificate,
                        attached_marriage_certificate = caseDto.attached_marriage_certificate,
                        attached_aggressor_dpi = caseDto.attached_aggressor_dpi,
                        status = caseStatusTypo,
                        created_by_user = createdByUser,
                        date_created = CsnFunctions.now()

                    };


                    //persisting objects
                    //persisting person
                    _admPerson.persist(victimPerson);
                    _admCase.persist(caseObject);

                    scope.Complete();
                    response.msg = "Caso creado";
                    response.id = caseObject.case_id;
                    response.code = "200";
                    return new OkObjectResult(caseObject);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);

                return new NoContentResult();
            }
        }

        //Method that checks if dto values are correct for a typology, if not, returns an empty typology
        private AdmTypology checkTypology(AdmTypologyDto typologyDto, AdmTypology emptyTypology)
        {
            AdmTypology responseTypology;
            if (typologyDto == null || typologyDto.typology_id == 0)
            {
                responseTypology = emptyTypology;
            }
            else
            {
                responseTypology = _dbContext.admTypologies.Find(typologyDto.typology_id) ?? emptyTypology;
            }

            return responseTypology;
        }
    }
}