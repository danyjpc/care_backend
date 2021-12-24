using System;
using System.Collections.Generic;
using System.Transactions;
using care_core.Controllers.util;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;

namespace care_core.Controllers
{
    [Route("rest/typologies")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmTypologyController : ControllerBase
    {
        private readonly IAdmTypology _admTypology;
        private readonly ILogger<AdmTypologyController> _logger;
        private JsonResponse response;

        public AdmTypologyController(IAdmTypology admTypology, ILogger<AdmTypologyController> logger)
        {
            _admTypology = admTypology;
            _logger = logger;
            response = new JsonResponse();
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] bool showInSurvey)
        {
        
            IEnumerable<AdmTypology> admTypologies = _admTypology.getAll(100,showInSurvey);
            return new OkObjectResult(admTypologies);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetAll([FromRoute] int id)
        {
            AdmTypology admTypology = _admTypology.getById(id);
            return new OkObjectResult(admTypology);
        }


        [HttpPost]
        public IActionResult Post([FromBody] AdmTypology admTypology)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    long id = _admTypology.persist(admTypology);
                    response.msg = "Success";
                    response.code = "Ok";
                    response.id = id;
                    scope.Complete();

                    return StatusCode(201, response);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Object: " + admTypology.ToString());

                Log.Error("Error" + ex.Message);
                Log.Error("Error: " + ex.StackTrace);

                return new NoContentResult();
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] AdmTypology admTypology, [FromRoute] long id)
        {
            if (id != admTypology.typology_id)
            {
                response.msg = "Incorrect ID";
                response.code = "Bad Request";
                response.id = id;

                return StatusCode(400, response);
            }

            using (var scope = new TransactionScope())
            {
                _admTypology.upd(admTypology);
                scope.Complete();
                return new OkResult();
            }
        }
    }
}