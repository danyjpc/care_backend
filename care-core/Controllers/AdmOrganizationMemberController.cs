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
    [Route("rest/organizationmembers")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmOrganizationMemberController : ControllerBase
    {
        private readonly IAdmOrganizationMember _admOrganizationM;
        private readonly ILogger<AdmOrganizationMemberController> _logger;
        private JsonResponse response;
        
        public AdmOrganizationMemberController(IAdmOrganizationMember admOrganizationMember, ILogger<AdmOrganizationMemberController> logger)
        {
            _admOrganizationM = admOrganizationMember;
            _logger = logger;
            response = new JsonResponse();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Object> organizationmembers = _admOrganizationM.getAll();
            return new OkObjectResult(organizationmembers);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] long id)
        {
            Object organizationmember = _admOrganizationM.getById(id);
            return new OkObjectResult(organizationmember);
        }

        [HttpPost]
        public IActionResult Post([FromBody] AdmOrganizationMember organizationMember)
        {           
            organizationMember.organization_member_id = 0;
            try
            {
                using (var scope = new TransactionScope())
                {
                    long id = _admOrganizationM.persist(organizationMember);
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

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] AdmOrganizationMember admOrganizationMember, [FromRoute] int id)
        {
            if(id != admOrganizationMember.organization_member_id){
                response.msg = "Incorrect ID";
                response.code = "Bad Request";
                response.id = id;

                return StatusCode(400, response);
            }
            using(var scope =new TransactionScope()){
                _admOrganizationM.upd(admOrganizationMember);
                scope.Complete();
                return new OkResult();
            }
        }
    }
}