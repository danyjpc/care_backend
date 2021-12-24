using System;
using System.Collections.Generic;
using care_core.Controllers.util;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace care_core.Controllers
{
    [Route("rest/persons")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmPersonController : ControllerBase
    {
        private readonly IAdmPerson _admPerson;
        
        private readonly ILogger<AdmPersonController> _logger;
        private JsonResponse response;

        public AdmPersonController(IAdmPerson admPerson, ILogger<AdmPersonController> logger)
        {
            _admPerson = admPerson;
            _logger = logger;
            response = new JsonResponse();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Object> admTypologies = _admPerson.getAll(0);
            return new OkObjectResult(admTypologies);
        }
    }
}