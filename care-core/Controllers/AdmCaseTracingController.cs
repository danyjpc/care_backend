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

namespace care_core.controllers
{
    [Route("rest/cases")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmCaseTracingController : ControllerBase
    {
        private readonly IAdmCaseTracing _admCaseTracing;
        private readonly ILogger<AdmCaseTracingController> _logger;
        private JsonResponse response;

        public AdmCaseTracingController(IAdmCaseTracing admCaseTracing, ILogger<AdmCaseTracingController> logger)
        {
            _admCaseTracing = admCaseTracing;
            _logger = logger;
            response = new JsonResponse();
        }

        [HttpGet("{id}/tracings")]
        public IActionResult GetAll(int id)
        {
            IEnumerable<Object> tracings = _admCaseTracing.getAll(id);
            return new OkObjectResult(tracings);
        }

        [HttpGet("{case_id}/tracing/{tracing_id}")]
        public IActionResult GetByIdCaseTracing([FromRoute] int case_id, [FromRoute] int tracing_id)
        {
            Object tracing = _admCaseTracing.getCaseTracingById(case_id, tracing_id);
            return new OkObjectResult(tracing);
        }

        [HttpPost("tracing")]
        public IActionResult CreateCaseTracing([FromBody] AdmCaseTracing admCaseTracing)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    long id = _admCaseTracing.persist(admCaseTracing);
                    response.msg = "Success";
                    response.code = "Ok";
                    response.id = id;
                    scope.Complete();

                    return StatusCode(201, response);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);

                return StatusCode(400);
            }
        }

        [HttpPut("tracing/{tracing_id}")]
        public IActionResult PutProyectActivity([FromBody] AdmCaseTracing admCaseTracing, [FromRoute] int tracing_id)
        {
            try
            {
                if (tracing_id != admCaseTracing.tracing_id)
                {
                    response.msg = "Incorrect ID";
                    response.code = "Bad Request";
                    response.id = tracing_id;

                    return StatusCode(400, response);
                }
                using (var scope = new TransactionScope())
                {
                    _admCaseTracing.upd(admCaseTracing);
                    scope.Complete();
                    return new OkResult();
                }

            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                return StatusCode(400, "Record no found");
            }

        }
    }
}
