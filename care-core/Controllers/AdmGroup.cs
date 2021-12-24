using System;
using care_core.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using care_core.Controllers.util;
using System.Linq;
using care_core.model;
using Serilog;
using System.Collections.Generic;
using System.Transactions;
using care_core.repository.interfaces;
using Microsoft.EntityFrameworkCore;

namespace care_core.controllers
{
    [Route("rest/builder")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmGroupController : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private JsonResponse response;
        private readonly IAdmQuestion _admQuestion;
        private readonly IAdmOption _admOption;

        public AdmGroupController(EntityDbContext dbContext, IAdmQuestion admQuestion, IAdmOption admOption)
        {
            _dbContext = dbContext;
            _admQuestion = admQuestion;
            _admOption = admOption;
            response = new JsonResponse();
        }

        [HttpGet("v1/{form_id}/group")]
        public IActionResult GetGroupByFromId([FromRoute] int form_id)
        {
            var item = _dbContext.admGroups.Where(x=>x.form.form_id == form_id &&
                x.status.typology_id == CareConstants.STATUS_ACTIVE)
            .Select(
                 group => new{
                     group_id = group.group_id,
                     name_group = group.name_group,
                     status = new{
                         typology_id = group.status.typology_id,
                         description = group.status.description
                     },
                     created_by_user = new {
                         user_id = group.created_by_user.user_id,
                         email = group.created_by_user.person.email
                     }
                 }
                ).OrderBy(x=> x.group_id).ToArray();
            return Ok(item);
        }

        [HttpGet("v1/{form_id}/group/{group_id}")]
        public IActionResult GetGroupById([FromRoute] int form_id, [FromRoute] int group_id)
        {
            var item = _dbContext.admGroups.Where(x=>x.form.form_id == form_id && x.group_id == group_id)
            .Select(
                 group => new{
                     group_id = group.group_id,
                     name_group = group.name_group,
                     status = new{
                         typology_id = group.status.typology_id,
                         description = group.status.description
                     },
                     created_by_user = new {
                         user_id = group.created_by_user.user_id,
                         email = group.created_by_user.person.email
                     }
                 }
                ).SingleOrDefault();
            return Ok(item);
        }

        [HttpPost("v1/{form_id}/group")]
        public IActionResult Post([FromRoute] int form_id, [FromBody] AdmGroup admGroup)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    AdmForm form = _dbContext.admForms.Find(form_id);
                    AdmTypology status = _dbContext.admTypologies.Find(admGroup.status.typology_id);
                    AdmUser user = _dbContext.admUsers.Find(admGroup.created_by_user.user_id);
                    
                    admGroup.form = form;
                    admGroup.status = status;
                    admGroup.created_by_user = user;
                    admGroup.date_created = CsnFunctions.now();

                    //checking for null value
                    admGroup.name_group ??= "Nueva Sección";

                    _dbContext.Add(admGroup);
                    save();

                    scope.Complete();
                    response.msg = "Success";
                    response.code = "Ok";
                    response.id = admGroup.group_id;

                    return StatusCode(201, response);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                return StatusCode(400);
            }
        }

        [HttpPut("v1/{form_id}/group/{group_id}")]
        public IActionResult Put([FromRoute] int form_id, [FromBody] AdmGroup admGroup, [FromRoute] int group_id)
        {
            try
            {
                if (group_id != admGroup.group_id)
                {
                    response.msg = "Incorrect ID";
                    response.code = "Bad Request";
                    response.id = group_id;

                    return StatusCode(400, response);
                }

                AdmGroup updgroup = _dbContext.admGroups.Find(admGroup.group_id);
                using (var scope = new TransactionScope())
                {
                    

                    AdmForm form = _dbContext.admForms.Find(form_id);
                    AdmTypology status = _dbContext.admTypologies.Find(admGroup.status.typology_id);
                    AdmUser user = _dbContext.admUsers.Find(admGroup.created_by_user.user_id);

                    updgroup.name_group = admGroup.name_group;
                    updgroup.status = status;
                    updgroup.created_by_user = user;

                    _dbContext.Entry(updgroup).State = EntityState.Modified;
                    save();
                    scope.Complete();
                }

                
                using (var scope = new TransactionScope())
                {
                    //update status for questions that belong to the group
                    if (updgroup.status.typology_id == CareConstants.ESTADO_INACTIVO)
                    {
                        List<AdmQuestion> questions = _dbContext.admQuestions
                            .Where(x => x.group.group_id.Equals(updgroup.group_id)).ToList();
                        if (questions.Any())
                        {
                            foreach (var question in questions)
                            {
                                //change status for each question
                                question.status = updgroup.status;
                                _dbContext.Entry(question).State = EntityState.Modified;
                                
                                //get question options
                                List<AdmOption> options = _dbContext.admOptions
                                    .Where(x => x.question.question_id.Equals(question.question_id)).ToList();

                                if (options.Any())
                                {
                                    //update options
                                    foreach (var option in options)
                                    {
                                        option.status = question.status;
                                        _dbContext.Entry(option).State = EntityState.Modified;
                                    }
                                }
                            }
                            //calling save changes once
                            _dbContext.SaveChanges();
                            
                        }
                    }
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

        private void save()
        {
            _dbContext.SaveChanges();
        }
    }
}