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
    [Route("rest/game")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmGameController : ControllerBase
    {
        private readonly IAdmGame _admGame;
        private readonly ILogger<AdmGameController> _logger;
        private JsonResponse response;

        public AdmGameController(IAdmGame admGame, ILogger<AdmGameController> logger)
        {
            _admGame = admGame;
            _logger = logger;
            response = new JsonResponse();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<AdmGame> games = _admGame.getAll();
            return new OkObjectResult(games);
        }

        [HttpGet("{id}")]
        public IActionResult GetAll([FromRoute] long id)
        {
            AdmGame game = _admGame.getById(id);
            return new OkObjectResult(game);
        }

        [HttpPost]
        public IActionResult Post([FromBody] AdmGame game)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    long id = _admGame.persist(game);
                    scope.Complete();
                    return StatusCode(201, id);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);

                return new NoContentResult();
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] AdmGame admGame, [FromRoute] int id)
        {
            if (id != admGame.id)
            {
                return StatusCode(204);
            }

            using (var scope = new TransactionScope())
            {
                _admGame.upd(admGame);
                scope.Complete();
                return new OkResult();
            }
        }
    }
}