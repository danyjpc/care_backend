using System;
using System.Collections.Generic;
using System.Linq;
using care_core.dto.AdmCase;
using care_core.dto.AdmOrganization;
using care_core.dto.AdmOrganizationMember;
using care_core.dto.AdmPerson;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Serilog;

namespace care_core.repository
{
    public class AdmCaseRepository : IAdmCase
    {
        private readonly EntityDbContext _dbContext;

        public AdmCaseRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IEnumerable<AdmCaseDto> getBaseResults(int estado, int caseId)
        {
            IEnumerable<AdmCaseDto> resultado = (from caso in _dbContext.admCases
                join victima in _dbContext.admPersons on caso.victim.person_id equals victima.person_id
                //todo join victima typologies
                join aggressorState in _dbContext.admTypologies on caso.aggressor_state.typology_id equals
                    aggressorState.typology_id
                join aggressorCity in _dbContext.admTypologies on caso.aggressor_city.typology_id equals aggressorCity
                    .typology_id
                join aggressorOccupation in _dbContext.admTypologies on caso.aggressor_occupation.typology_id equals
                    aggressorOccupation.typology_id
                join aggressorMaritalStatus in _dbContext.admTypologies on caso.aggressor_marital_status.typology_id
                    equals aggressorMaritalStatus.typology_id
                join victimRelationship in _dbContext.admTypologies on caso.victim_relationship.typology_id equals
                    victimRelationship.typology_id
                join violenceType in _dbContext.admTypologies on caso.identified_violence_type.typology_id equals
                    violenceType.typology_id
                join accompanimentType in _dbContext.admTypologies on caso.accompaniment_type.typology_id equals
                    accompanimentType.typology_id
                join institution in _dbContext.admTypologies on caso.institution.typology_id equals institution
                    .typology_id
                join accompanimentRoute in _dbContext.admTypologies on caso.accompaniment_route.typology_id equals
                    accompanimentRoute.typology_id
                join recordingOrganization in _dbContext.admOrganizations on caso.recording_organization.organization_id
                    equals recordingOrganization.organization_id
                join recordedByMember in _dbContext.admOrganizationMembers on caso.recorded_by_member
                        .organization_member_id
                    equals recordedByMember.organization_member_id
                join caseCategory in _dbContext.admTypologies on caso.case_category.typology_id equals caseCategory
                    .typology_id
                join caseStatus in _dbContext.admTypologies on caso.status.typology_id equals caseStatus.typology_id
                join createdBy in _dbContext.admUsers on caso.created_by_user.user_id equals createdBy.user_id
                where (
                    caseStatus.typology_id.Equals(estado > 0 ? estado : caseStatus.typology_id)
                    &&
                    caso.case_id.Equals(caseId > 0 ? caseId : caso.case_id)
                )
                select new AdmCaseDto
                {
                    victim = new AdmPersonDto(
                        victima.person_id,
                        victima.first_name,
                        victima.last_name,
                        victima.cui
                    ),
                    case_id = caso.case_id,
                    aggressor_first_name = caso.aggressor_first_name,
                    aggressor_last_name = caso.aggressor_last_name,
                    aggressor_phone_number = caso.aggressor_phone_number,
                    aggressor_cui = caso.aggressor_cui,
                    aggressor_address_line = caso.aggressor_address_line,
                    aggressor_work_place = caso.aggressor_work_place,
                    act_place = caso.act_place,
                    act_description = caso.act_description,
                    lives_with_aggressor = caso.lives_with_aggressor,
                    annotation = caso.annotation,
                    attached_victim_dpi = caso.attached_victim_dpi,
                    attached_victim_birth_certificate = caso.attached_victim_birth_certificate,
                    attached_children_birth_certificate = caso.attached_children_birth_certificate,
                    attached_marriage_certificate = caso.attached_marriage_certificate,
                    attached_aggressor_dpi = caso.attached_aggressor_dpi,

                    aggressor_state = new AdmTypologyDto
                    {
                        typology_id = aggressorState.typology_id,
                        description = aggressorState.description
                    },
                    aggressor_city = new AdmTypologyDto
                    {
                        typology_id = aggressorCity.typology_id,
                        description = aggressorCity.description
                    },
                    aggressor_occupation = new AdmTypologyDto
                    {
                        typology_id = aggressorOccupation.typology_id,
                        description = aggressorOccupation.description
                    },
                    aggressor_marital_status = new AdmTypologyDto
                    {
                        typology_id = aggressorMaritalStatus.typology_id,
                        description = aggressorMaritalStatus.description
                    },
                    victim_relationship = new AdmTypologyDto
                    {
                        typology_id = victimRelationship.typology_id,
                        description = victimRelationship.description
                    },
                    identified_violence_type = new AdmTypologyDto
                    {
                        typology_id = violenceType.typology_id,
                        description = violenceType.description
                    },
                    accompaniment_type = new AdmTypologyDto
                    {
                        typology_id = accompanimentType.typology_id,
                        description = accompanimentType.description
                    },
                    institution = new AdmTypologyDto
                    {
                        typology_id = institution.typology_id,
                        description = institution.description
                    },
                    accompaniment_route = new AdmTypologyDto
                    {
                        typology_id = accompanimentRoute.typology_id,
                        description = accompanimentRoute.description
                    },
                    case_category = new AdmTypologyDto(caseCategory.typology_id, caseCategory.description),
                    status = new AdmTypologyDto
                    {
                        typology_id = caseStatus.typology_id,
                        description = caseStatus.description
                    },
                    recording_organization = new AdmOrganizationDto(recordingOrganization.organization_id, recordingOrganization.name_organization),
                    recorded_by = new AdmOrganizationMemberDto(recordedByMember.organization_member_id, recordedByMember.name_organization_member),
                    created_by = new AdmUserDto
                    {
                        user_id = caso.created_by_user.user_id,
                        email = caso.created_by_user.person.email
                    },
                    date_created = caso.date_created,
                    aggressor_birthday = caso.aggressor_birthday,
                    act_date = caso.act_date
                }).OrderBy(x => x.date_created).ToList();

            return resultado;
        }

        public IEnumerable<AdmCaseDto> getAll(int estado)
        {
            return getBaseResults(estado, 0);
        }

        public AdmCaseDto getCaseById(int caseId)
        {
            return getBaseResults(0, caseId).SingleOrDefault();
        }

        public long persist(AdmCase admCase)
        {
            try
            {
                _dbContext.Add(admCase);
            }
            catch (Exception ex)
            {
                Log.Error("Error: " + ex.Message);
            }

            save();

            return admCase.case_id;
        }

        public void upd(AdmCase admCase)
        {
            throw new NotImplementedException();
        }

        public void save()
        {
            _dbContext.SaveChanges();
        }
    }
}