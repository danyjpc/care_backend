using System;
using System.Collections.Generic;
using System.Transactions;
using care_core.Controllers.util;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.repository.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using care_core.util;
using Microsoft.EntityFrameworkCore;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Security;

namespace care_core.controllers
{
    [Route("rest/users/v1")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmUserController : ControllerBase
    {
        private readonly IAdmUser _admUser;
        private readonly IAdmPerson _admPerson;
        private readonly ILogger<AdmUserController> _logger;
        private JsonResponse response;
        private readonly EntityDbContext _dbContext;

        public AdmUserController(IAdmUser updPass, ILogger<AdmUserController> logger, EntityDbContext dbContext,
            IAdmPerson admPerson)
        {
            _admUser = updPass;
            _logger = logger;
            _dbContext = dbContext;
            _admPerson = admPerson;
            response = new JsonResponse();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Object> users = _admUser.getAll();
            return new OkObjectResult(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] long id)
        {
            Object user = _admUser.getById(id);
            return new OkObjectResult(user);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] AdmUser updPass, [FromRoute] int id)
        {
            try
            {
                if (id != updPass.user_id)
                {
                    response.msg = "Incorrect ID";
                    response.code = "Bad Request";
                    response.id = id;

                    return StatusCode(400, response);
                }

                using (var scope = new TransactionScope())
                {
                    _admUser.upd(updPass);
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


        [HttpPost]
        public IActionResult Post([FromBody] AdmUserDto userDto)
        {
            /*when persisting the DB will assign an auto-generated id
            if it is not 0 the DB will try to persist that id number
            and may cause an integrity error*/
            userDto.user_id = 0;
            try
            {
                using (var scope = new TransactionScope())
                {
                    //CHECKING IF Organization ID IS VALID
                    AdmOrganization organization =
                        _dbContext.admOrganizations.Find(userDto.organization.organization_id);
                    if (organization == null)
                    {
                        response.code = "400";
                        response.msg = "organization not found";
                        return new BadRequestObjectResult(response);
                    }

                    AdmTypology role = _dbContext.admTypologies.Find(userDto.role.typology_id);
                    if (role == null)
                    {
                        response.code = "400";
                        response.msg = "role not found";
                        return new BadRequestObjectResult(response);
                    }

                    Object existingPerson = _admPerson.findByDpi(userDto.person.cui);
                    if (existingPerson != null)
                    {
                        response.code = "400";
                        response.msg = "CUI already registered";
                        return new BadRequestObjectResult(response);
                    }

                    existingPerson = _admPerson.findByEmail(userDto.person.email, true);
                    if (existingPerson != null)
                    {
                        response.code = "400";
                        response.msg = "Email already registered";
                        return new BadRequestObjectResult(response);
                    }

                    AdmTypology state = _dbContext.admTypologies.Find(userDto.person.state.typology_id);
                    if (state == null)
                    {
                        //setting state to default Guatemala departamento
                        state = _dbContext.admTypologies.Find(CareConstants.DEFAULT_STATE_ID);
                    }

                    AdmTypology city = _dbContext.admTypologies.Find(userDto.person.city.typology_id);
                    if (city == null)
                    {
                        //setting city to default Guatemala municipio
                        city = _dbContext.admTypologies.Find(CareConstants.DEFAULT_CITY_ID);
                    }

                    AdmTypology status = _dbContext.admTypologies.Find(userDto.status.typology_id);
                    if (status == null)
                    {
                        //setting city to default Guatemala municipio
                        status = _dbContext.admTypologies.Find(CareConstants.DEFAULT_STATE_ID);
                    }


                    AdmUser user = new AdmUser();
                    AdmPerson person = new AdmPerson();

                    //setting person values
                    person.first_name = userDto.person.first_name;
                    person.last_name = userDto.person.last_name;
                    person.cui = userDto.person.cui;
                    person.email = userDto.person.email;

                    if (userDto.person.birthday != null) person.birthday = (DateTime)userDto.person.birthday;


                    person.state = state;
                    person.city = city;

                    //persisting person
                    person = _admPerson.persistDefaultValues(person);

                    //setting user values
                    user.user_id = CareConstants.ZERO_DEFAULT;
                    user.person = person;
                    user.organization = organization;
                    user.password = userDto.password;
                    user.date_create = CsnFunctions.now();
                    user.role = role;
                    user.status = status;
                    user.user_id = _admUser.persist(user);

                    response.msg = "Success";
                    response.code = "Ok";
                    response.id = user.user_id;
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

        [HttpPut("changepassword/{id}")]
        public IActionResult PutPassword([FromBody] AdmUpdatePass updPass, [FromRoute] int id)
        {
            if (id != updPass.user_id)
            {
                response.msg = "Incorrect ID";
                response.code = "Bad Request";
                response.id = id;

                return StatusCode(400, response);
            }

            using (var scope = new TransactionScope())
            {
                AdmUser updUserPass = _dbContext.admUsers.Find(updPass.user_id);

                if (updUserPass == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
                else if (!updPass.new_pass.Equals(updPass.confirm_pass))
                {
                    return BadRequest(new { message = "Contraseñas no coinciden" });
                }
                else if (!updPass.old_pass.Equals(updUserPass.password))
                {
                    return BadRequest(new { message = "La contraseña actual no coincide" });
                }

                updUserPass.password = updPass.new_pass;
                _dbContext.Entry(updUserPass).State = EntityState.Modified;
                _dbContext.SaveChanges();
                scope.Complete();

                return new OkResult();
            }
        }

    }
}