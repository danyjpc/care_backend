using care_core.dto.auth;
using care_core.repository.interfaces;
using care_core.security.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using care_core.Controllers.util;
using System;
using Serilog;
using care_core.model;

namespace care_core.Controllers
{
    [Route("rest/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAdmUser _admUser;
        private readonly IAdmPerson _admPerson;
        private ITokenService _tokenService;
        private JsonResponse response;

        public AuthController(ITokenService tokenService, IAdmUser admUser, IAdmPerson admPerson)
        {
            _admUser = admUser;
            _admPerson = admPerson;
            _tokenService = tokenService;
            response = new JsonResponse();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authenticate([FromBody] CredentialsDto credentials)
        {
            var user = _tokenService.Authenticate(credentials.email, credentials.password);

            if (user == null)
                return BadRequest(new {message = "Username or password is incorrect"});

            return Ok(user);
        }

        [HttpPost("forgot-password")]
        public IActionResult forgotPass([FromQuery] string email)
        {
            try
            {
                AdmPerson existingPerson = _admPerson.findByEmail(email, false);
                if (existingPerson != null)
                {
                    _admUser.passwordReset(existingPerson);
                    //sendMail();
                    return new OkResult();
                }
                else
                {
                    response.code = "404";
                    response.msg = "Email no encontrado";
                    return new BadRequestObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);

                return StatusCode(500);
            }

        }
    }
}