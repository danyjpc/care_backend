using System;
using System.Collections.Generic;
using System.Transactions;
using care_core.Controllers.util;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using care_core.model;
using care_core.dto.AdmForm;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using Serilog;
using Microsoft.EntityFrameworkCore;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Security;

namespace care_core.controllers
{
    [Route("rest/surveys")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmFormController : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private JsonResponse response;
        public AdmFormController(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
            response = new JsonResponse();
        }

        //Metodo que devuelve los fomularios filtrado por modulo y categoria 
        [HttpGet("v1")]
        public IActionResult GetAllForms([FromQuery(Name = "module")] int? module_id = null, [FromQuery(Name = "category")] int? category_id = null)
        {

            var item = _dbContext.admForms.AsQueryable();

            if (module_id != null && category_id != null)
            {
                item = item.Where(x => x.module_category.module.module_id == module_id
                && x.module_category.category.category_id == category_id);
            }
            else if (module_id != null && category_id == null)
            {
                item = item.Where(x => x.module_category.module.module_id == module_id);
            }
            else if (module_id == null && category_id != null)
            {
                item = item.Where(x => x.module_category.category.category_id == category_id);
            }

            var list = item.Where(x => x.status.typology_id == CareConstants.ESTADO_ACTIVO).Select(
                form => new
                {
                    form_id = form.form_id,
                    name_form = form.name_form,
                    is_public = form.is_public,
                    module = new AdmModulDto
                    {
                        module_id = form.module_category.module.module_id,
                        name_module = form.module_category.module.name_module
                    },
                    category = new AdmCategDto
                    {
                        category_id = form.module_category.category.category_id,
                        name_category = form.module_category.category.name_category
                    },
                    status = new AdmTypologyDto
                    {
                        typology_id = form.status.typology_id,
                        description = form.status.description
                    },
                    created_by_user = new AdmUserDto
                    {
                        user_id = form.created_by_user.user_id,
                        email = form.created_by_user.person.email
                    },
                    date_created = form.date_create

                }
                ).OrderByDescending(id => id.form_id).ToArray();

            //lista que se devolvera con el formato esperado    
            List<AdmFormDto> listform = new List<AdmFormDto>();

            foreach (var x in list)
            {
                listform.Add(new AdmFormDto()
                {
                    form_id = x.form_id,
                    name_form = x.name_form,
                    is_public = x.is_public,
                    module = x.module,
                    category = x.category,
                    status = x.status,
                    nrecorded = GetNRecorded(x.form_id),
                    lastrecorded = GetLastDate(x.form_id),
                    created_by_user = x.created_by_user,
                    date_created = x.date_created

                });
            }

            return Ok(listform);
        }

        //Metod que obtiene el numero de registros de un formulario
        private int GetNRecorded(int form_id)
        {
            var item = _dbContext.admSurveys.Where(x => x.form.form_id == form_id 
                        && x.status.typology_id == CareConstants.STATUS_ACTIVE).ToArray();
            int nRecorded = item.Count();
            return nRecorded;
        }

        //Obiene la fecha del ultimo registro de un formulario
        private string GetLastDate(int form_id)
        {
            var item = _dbContext.admSurveys.Where(x => x.form.form_id == form_id
                        && x.status.typology_id == CareConstants.STATUS_ACTIVE).ToArray();
            if (item == null || item.Count() == 0)
            {
                return "---";
            }
            else
            {
                var survey = item.Last();
                string lastdate = survey.date_create.ToString("dd/MM/yyyy");
                return lastdate;
            }
        }

        [HttpGet("v1/{form_id}")]
        [AllowAnonymous]
        public IActionResult GetFormById([FromRoute] int form_id)
        {
            var form = _dbContext.admForms.Where(x => x.form_id == form_id).Select(
               form => new
               {
                   form_id = form.form_id,
                   name_form = form.name_form,
                   is_public = form.is_public,
                   module = new
                   {
                       module_id = form.module_category.module.module_id,
                       name_module = form.module_category.module.name_module
                   },
                   category = new
                   {
                       category_id = form.module_category.category.category_id,
                       name_category = form.module_category.category.name_category
                   },
                   status = new
                   {
                       typology_id = form.status.typology_id,
                       description = form.status.description
                   },
                   created_by_user = new
                   {
                       user_id = form.created_by_user.user_id,
                       email = form.created_by_user.person.email
                   },
                   date_created = form.date_create
               }

            ).SingleOrDefault();
            return Ok(form);
        }

        [HttpPost("v1")]
        public IActionResult Post([FromBody] AdmFormDto admFormDto)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    AdmForm admform = new AdmForm();

                    admform.form_id = 0;

                    AdmModule module = _dbContext.admModules.Find(admFormDto.module.module_id);
                    AdmCategory category = _dbContext.admCategories.Find(admFormDto.category.category_id);
                    AdmTypology status = _dbContext.admTypologies.Find(admFormDto.status.typology_id);
                    AdmUser user = _dbContext.admUsers.Find(admFormDto.created_by_user.user_id);

                    AdmModuleCategory mc = _dbContext.admModuleCategories
                        .Where(x => x.module.module_id == module.module_id &&
                                    x.category.category_id == category.category_id)
                        .SingleOrDefault();


                    admform.name_form = admFormDto.name_form;
                    admform.is_public = admFormDto.is_public;
                    admform.module_category = mc;
                    admform.status = status;
                    admform.created_by_user = user;
                    admform.date_create = CsnFunctions.now();

                    _dbContext.Add(admform);
                    _dbContext.SaveChanges();

                    CrearGrupo(admform.form_id, user);

                    scope.Complete();
                    response.msg = "Success";
                    response.code = "Ok";
                    response.id = admform.form_id;

                    return StatusCode(201, response);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                return StatusCode(400);
            }
        }

        [HttpPut("v1/{id}")]
        public IActionResult Put([FromBody] AdmFormDto admForm, [FromRoute] int id)
        {
            try
            {
                if (id != admForm.form_id)
                {
                    response.msg = "Incorrect ID";
                    response.code = "Bad Request";
                    response.id = id;

                    return StatusCode(400, response);
                }
                using (var scope = new TransactionScope())
                {
                    AdmForm updForm = _dbContext.admForms.Find(admForm.form_id);

                    AdmModule module = _dbContext.admModules.Find(admForm.module.module_id);
                    AdmCategory category = _dbContext.admCategories.Find(admForm.category.category_id);
                    AdmTypology status = _dbContext.admTypologies.Find(admForm.status.typology_id);
                    AdmUser user = _dbContext.admUsers.Find(admForm.created_by_user.user_id);

                    AdmModuleCategory mc = _dbContext.admModuleCategories
                        .Where(x => x.module.module_id == module.module_id &&
                                    x.category.category_id == category.category_id)
                        .SingleOrDefault();

                    updForm.name_form = admForm.name_form;
                    updForm.is_public = admForm.is_public;
                    updForm.module_category = mc;
                    updForm.status = status;
                    updForm.created_by_user = user;

                    _dbContext.Entry(updForm).State = EntityState.Modified;
                    _dbContext.SaveChanges();
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

        //Crea un grupo por default al formulario
        private int CrearGrupo(int form_id, AdmUser user)
        {
            AdmGroup admGroup = new AdmGroup();

            AdmForm form = _dbContext.admForms.Find(form_id);
            AdmTypology status = _dbContext.admTypologies.Find(160445);

            admGroup.name_group = "Grupo predeterminado";
            admGroup.form = form;
            admGroup.status = status;
            admGroup.created_by_user = user;
            admGroup.date_created = CsnFunctions.now();

            _dbContext.Add(admGroup);
            _dbContext.SaveChanges();

            return admGroup.group_id;

        }

        [HttpPost("v1/sendemail")]
        public IActionResult SendEmail([FromQuery] string toEmail, [FromQuery] string publicURL)
        {
            try
            {
                if(toEmail == null || publicURL == null){
                    return StatusCode(400);
                }
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(CareConstants.SMTP_NAME_FROM));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = "URL de formulario público";

                message.Body = new TextPart("plain")
                {
                    Text = @"Hola,

                Te han enviado una URL de un formulario público,
                puedes revisar el formulario público en el siguiente link: "

                    + "\n\nLink del formulario público: " + publicURL + " \n\n"

                    + "--Saludos"
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(CareConstants.SMTP_HOST, CareConstants.SMTP_PORT, SecureSocketOptions.Auto);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(CareConstants.SMTP_USER, CareConstants.SMTP_PASS);

                    client.Send(message);
                    client.Disconnect(true);
                }

                return new OkResult();
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                return StatusCode(500, "Internal Server Error");
            }

        }
    }
}