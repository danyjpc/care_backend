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
    [Route("rest/organizations")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmOrganizationController : ControllerBase
    {
        private readonly IAdmOrganization _organization;
        private readonly ILogger<AdmOrganizationController> _logger;
        private JsonResponse response;

        public AdmOrganizationController(IAdmOrganization admOrganization, ILogger<AdmOrganizationController> logger)
        {
            _organization = admOrganization;
            _logger = logger;
            response = new JsonResponse();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Object> organizations = _organization.getAll();
            return new OkObjectResult(organizations);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] long id)
        {
            Object organization = _organization.getById(id);
            return new OkObjectResult(organization);
        }

        [HttpPost]
        public IActionResult Post([FromBody] AdmOrganization organization)
        {
            /*when persisting the DB will assign an auto-generated id
            if it is not 0 the DB will try to persist that id number
            and may cause an integrity error*/
            organization.organization_id = 0;
            try
            {
                using (var scope = new TransactionScope())
                {
                    long id = _organization.persist(organization);
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

                return new NoContentResult();
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] AdmOrganization admOrganization, [FromRoute] int id)
        {
            try
            {
                if (id != admOrganization.organization_id)
                {
                    response.msg = "Incorrect ID";
                    response.code = "Bad Request";
                    response.id = id;

                    return StatusCode(400, response);
                }
                using (var scope = new TransactionScope())
                {
                    _organization.upd(admOrganization);
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

        //*********** ORGANIZATION MEMBERS ****************

        [HttpGet("{id}/members")]
        public IActionResult GetAllMembers([FromRoute] int id)
        {
            IEnumerable<Object> organizationMembers = _organization.getAllMembers(id);
            return new OkObjectResult(organizationMembers);
        }

        [HttpGet("{id}/member/{member_id}")]
        public IActionResult GetByIdMember([FromRoute] int id, int member_id)
        {
            Object organizationMember = _organization.getByIdMember(id, member_id);
            return new OkObjectResult(organizationMember);
        }

        [HttpPost("member")]
        public IActionResult PostMember([FromBody] AdmOrganizationMember organizationMember)
        {

            try
            {
                using (var scope = new TransactionScope())
                {
                    long id = _organization.persistMember(organizationMember);
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

                return new NoContentResult();
            }
        }

        [HttpPut("member/{member_id}")]
        public IActionResult PutMember([FromBody] AdmOrganizationMember admOrganizationMember, [FromRoute] int member_id)
        {
            try
            {
                if (member_id != admOrganizationMember.organization_member_id)
                {
                    response.msg = "Incorrect ID";
                    response.code = "Bad Request";
                    response.id = member_id;

                    return StatusCode(400, response);
                }
                using (var scope = new TransactionScope())
                {
                    _organization.updMember(admOrganizationMember);
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