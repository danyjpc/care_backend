using System;
using System.Collections.Generic;
using System.Transactions;
using care_core.Controllers.util;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using care_core.dto.AdmForm;
using Microsoft.Extensions.Logging;
using Serilog;

namespace care_core.controllers
{
    [Route("rest/modules/v1/")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmModuleController : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmModule _admModule;
        private readonly IAdmCategory _admCategory;
        private readonly IAdmModuleCategory _admModuleCategory;
        private JsonResponse response;

        public AdmModuleController(EntityDbContext dbContext, IAdmModule admModule, IAdmModuleCategory admModuleCategory, IAdmCategory admCategory)
        {
            _dbContext = dbContext;
            _admModule = admModule;
            _admModuleCategory = admModuleCategory;
            _admCategory = admCategory;
            response = new JsonResponse();
        }

        [HttpGet]
        public IActionResult GetAllModules()
        {
            var item = _dbContext.admModules.Where(x => x.status.typology_id == CareConstants.DEFAULT_STATUS)
            .Select(
                modules => new
                {
                    module_id = modules.module_id,
                    name_module = modules.name_module,
                    icon = modules.icon,
                    picture = modules.picture,
                    date_create = modules.date_create,
                    status = new
                    {
                        typology_id = modules.status.typology_id,
                        description = modules.status.description
                    },
                    created_by_user = new
                    {
                        modules.created_by_user.user_id,
                        modules.created_by_user.person.email
                    }
                }).ToArray();
            return Ok(item);
        }

        [HttpGet("{module_id}/categories")]
        public IActionResult GetCategoriesById([FromRoute] int module_id)
        {
            var item = _dbContext.admModuleCategories.Where(x => x.module.module_id == module_id
                        && x.category.status.typology_id == CareConstants.DEFAULT_STATUS).Select(
                    module_categories => new
                    {
                        category_id = module_categories.category.category_id,
                        name_category = module_categories.category.name_category,
                        color = module_categories.category.color,
                        icon = module_categories.module.icon,
                        status = new
                        {
                            typology_id = module_categories.category.status.typology_id,
                            description = module_categories.category.status.description
                        },
                        created_by_user = new
                        {
                            module_categories.category.created_by_user.user_id,
                            module_categories.category.created_by_user.person.email
                        }
                    }
                )
                .ToArray();
            return Ok(item);
        }


        //Get module by ID
        [HttpGet("{module_id}")]
        public IActionResult getModuleById([FromRoute] int module_id)
        {
            var modulo = _admModule.getModuleById(module_id);

            return new OkObjectResult(modulo);
        }


        //Persist module
        [HttpPost]
        public IActionResult Post([FromBody] AdmModuleDto moduleDto)
        {
            try
            {
                //CHECKING IF STATUS VALUE IS VALID
                AdmTypology status = _dbContext.admTypologies.Find(moduleDto.status.typology_id) ??
                                     _dbContext.admTypologies.Find(CareConstants.ESTADO_ACTIVO);

                //CHECKING IF USER IS VALID
                AdmUser user = _dbContext.admUsers.Find(moduleDto.created_by_user.user_id);
                if (user == null)
                {
                    response.code = "400";
                    response.msg = "User not found";
                    return new BadRequestObjectResult(response);
                }

                AdmModule module = new AdmModule();
                using (var scope = new TransactionScope())
                {
                    module.created_by_user = user;
                    module.status = status;
                    module.icon = moduleDto.icon;
                    module.name_module = moduleDto.name_module;
                    module.picture = moduleDto.picture;
                    module.date_create = CsnFunctions.now();

                    //persisting module
                    module.module_id = _admModule.persist(module);

                    //adding default category Task 1556
                    //Updated task I1658 category must me a new record
                    AdmCategory category = new AdmCategory();
                    category.category_id = 0;
                    category.name_category = "Categoria default";
                    category.status = module.status;
                    category.created_by_user = module.created_by_user;
                    category.date_create = CsnFunctions.now();
                    //category default color must be different thna white
                    category.color = "#000000";
                    category.icon = "icon";
                    //persisting new category
                    //method also persist module_category record
                    category.category_id = _admCategory.persist(module.module_id, category);

                    response.msg = "Success";
                    response.code = "Ok";
                    response.id = module.module_id;
                    scope.Complete();

                    return StatusCode(201, response);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);

                return new NoContentResult();
            }
        }

        [HttpPut("{module_id}")]
        public IActionResult updateModule([FromBody] AdmModuleDto moduleDto, [FromRoute] int module_id)
        {
            try
            {
                if (module_id != moduleDto.module_id)
                {
                    response.code = "400";
                    response.msg = "Incorrect ID";
                    return new BadRequestObjectResult(response);
                }

                //CHECKING IF STATUS VALUE IS VALID
                AdmTypology status = _dbContext.admTypologies.Find(moduleDto.status.typology_id) ??
                                     _dbContext.admTypologies.Find(CareConstants.ESTADO_ACTIVO);

                //CHECKING IF USER IS VALID
                AdmUser user = _dbContext.admUsers.Find(moduleDto.created_by_user.user_id);
                if (user == null)
                {
                    response.code = "400";
                    response.msg = "User not found";
                    return new BadRequestObjectResult(response);
                }


                AdmModule module = new AdmModule();
                using (var scope = new TransactionScope())
                {

                    module.module_id = _admModule.update(moduleDto);
                    scope.Complete();
                }

                response.msg = "Success";
                response.code = "Ok";
                response.id = module.module_id;
                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                response.msg = "Error";
                response.code = "Fail";
                return StatusCode(400, response);
            }
        }
    }
}