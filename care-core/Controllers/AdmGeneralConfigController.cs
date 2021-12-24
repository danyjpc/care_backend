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
using care_core.dto.AdmGeneralConfigDto;
using Microsoft.Extensions.Logging;
using Serilog;

namespace care_core.controllers
{
    [Route("rest/configs/v1")]
    [ApiController]
    [Authorize(Roles = CareConstants.ADMINISTRADOR_ROLE)]
    public class AdmGeneralConfigController : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmGeneralConfig _admGeneralConfig;
        private JsonResponse response;

        public AdmGeneralConfigController(EntityDbContext dbContext, IAdmGeneralConfig admGeneralConfig)
        {
            _dbContext = dbContext;
            _admGeneralConfig = admGeneralConfig;
            response = new JsonResponse();
        }

        [HttpGet()]
        [AllowAnonymous]
        public IActionResult GetAllConfigs([FromQuery] int status_id)
        {
            IEnumerable<AdmGeneralConfigDto> configs = _admGeneralConfig.getAll(status_id);
            return new OkObjectResult(configs);
        }

        [HttpGet("{config_id}")]
        [AllowAnonymous]
        public IActionResult GetConfigById([FromRoute] int config_id)
        {
            var config = _admGeneralConfig.getConfigById(config_id);
            if (config == null)
            {
                response.code = "404";
                response.msg = "Configuration not found";
                return new NotFoundObjectResult(response);
            }

            return new OkObjectResult(config);
        }

        //Persist config
        [HttpPost]
        public IActionResult persistConfig(AdmGeneralConfigDto admGeneralConfigDto)
        {
            try
            {
                //CHECKING IF STATUS VALUE IS VALID
                AdmTypology status = _dbContext.admTypologies.Find(admGeneralConfigDto.status.typology_id) ??
                                     _dbContext.admTypologies.Find(CareConstants.ESTADO_ACTIVO);

                //CHECKING IF USER IS VALID
                AdmUser user = _dbContext.admUsers.Find(admGeneralConfigDto.created_by_user.user_id);
                if (user == null)
                {
                    response.code = "400";
                    response.msg = "User not found";
                    return new BadRequestObjectResult(response);
                }

                AdmGeneralConfig config = new AdmGeneralConfig();
                config.config_name = admGeneralConfigDto.config_name;
                config.config_value = admGeneralConfigDto.config_value;
                config.status = status;
                config.created_by_user = user;
                config.date_created = CsnFunctions.now();
                config.config_id = CareConstants.ZERO_DEFAULT;
                config.description = admGeneralConfigDto.description;

                config.config_id = _admGeneralConfig.persist(config);
                response.msg = "Success";
                response.code = "Ok";
                response.id = config.config_id;

                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);

                return new NoContentResult();
            }
        }

        [HttpPut]
        public IActionResult updateConfig([FromBody] AdmGeneralConfigDto[] configDtos)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach (var configDto in configDtos)
                    {
                        configDto.config_id = _admGeneralConfig.update(configDto);
                    }

                    scope.Complete();

                    response.msg = "Success";
                    response.code = "Ok";
                    response.id = 0;
                    return StatusCode(200, response);
                }
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