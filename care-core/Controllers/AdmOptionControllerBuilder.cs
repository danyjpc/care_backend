using System;
using System.Transactions;
using care_core.Controllers.util;
using care_core.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using care_core.model;
using care_core.dto.AdmQuestionGroup;
using care_core.dto.AdmTypology;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using care_core.repository.interfaces;

namespace care_core.controllers
{
    [Route("rest/builder/v1/")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmOptionControllerBuilder : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmOption _admOption;
        private JsonResponse response;

        public AdmOptionControllerBuilder(EntityDbContext dbContext, IAdmOption admOption)
        {
            _dbContext = dbContext;
            _admOption = admOption;
            response = new JsonResponse();
        }

        //method for getting options from a group
        [HttpGet("form/{form_id}/group/{group_id}/questions/{question_id}")]
        public IActionResult getOptionsByQuestion(int form_id, int group_id,
            int question_id, int estado)
        {
            try
            {
                AdmForm form = _dbContext.admForms.Find(form_id);
                if (form == null)
                {
                    response.code = "400";
                    response.msg = "Form not found";
                    return new BadRequestObjectResult(response);
                }

                AdmGroup group = _dbContext.admGroups.Find(group_id);
                if (group == null)
                {
                    response.code = "400";
                    response.msg = "Group not found";
                    return new BadRequestObjectResult(response);
                }
                
                AdmQuestion question = _dbContext.admQuestions.Find(question_id);
                if (question == null)
                {
                    response.code = "400";
                    response.msg = "Question not found";
                    return new BadRequestObjectResult(response);
                }

                IEnumerable<AdmQuestionOptionDto> options = _admOption.getAllByQuestion(question_id, estado);
                return new OkObjectResult(options);
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                return StatusCode(400);
            }
        }
        /*
        [HttpPost("form/{form_id}/group/{group_id}/questions")]
        public IActionResult persistSingleQuestionInGroup(int form_id, int group_id, AdmQuestionDto questionDto)
        {
            try
            {
                
                //CHECKING IF FORM ID IS VALID
                AdmForm form = _dbContext.admForms.Find(form_id);
                if (form == null)
                {
                    response.code = "400";
                    response.msg = "Form not found";
                    return new BadRequestObjectResult(response);
                }

                //CHECKING IF GROUP ID IS VALID
                AdmGroup group = _dbContext.admGroups.Find(group_id);
                if (group == null)
                {
                    response.code = "400";
                    response.msg = "Group not found";
                    return new BadRequestObjectResult(response);
                }

                //CHECKING IF TYPOLOGY VALUE IS VALID
                AdmTypology typology = _dbContext.admTypologies.Find(questionDto.typology_id) ?? new AdmTypology
                {
                    typology_id = CareConstants.EMPTY_TYPOLOGY
                };

                //CHECKING IF STATUS VALUE IS VALID
                AdmTypology status = _dbContext.admTypologies.Find(questionDto.status.typology_id) ??
                                     _dbContext.admTypologies.Find(CareConstants.ESTADO_ACTIVO);

                //CHECKING IF USER IS VALID
                AdmUser user = _dbContext.admUsers.Find(questionDto.created_by_user.user_id);
                if (user == null)
                {
                    response.code = "400";
                    response.msg = "User not found";
                    return new BadRequestObjectResult(response);
                }

                int questionId = 0;
                //IF 0, THEN PERSIST
                if (questionDto.question_id == 0)
                {
                    questionId = this.persistQuestion(questionDto, group, typology, status, user);
                }
                //ELSE UPDATE
                else if (questionDto.question_id > 0)
                {
                    AdmQuestion currentQuestion = _dbContext.admQuestions.Find(questionDto.question_id);
                    if (currentQuestion == null)
                    {
                        response.code = "400";
                        response.msg = "Question not found";
                        return new BadRequestObjectResult(response);
                    }
                    questionId = this.updateQuestion(questionDto, group, typology, status);
                }
                //ELSE INCORRECT ID
                else
                {
                    response.code = "400";
                    response.msg = "Question id is incorrect";
                    return new BadRequestObjectResult(response);
                }
                
                response.msg = "Success";
                response.code = "Ok";
                response.id = questionId;
                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                return StatusCode(400);
            }
        }

        [HttpPut("form/{form_id}/group/{group_id}/questions/{question_id}")]
        public IActionResult updateSingleQuestionInGroup(int form_id, int group_id, int question_id, AdmQuestionDto questionDto)
        {
            try
            {
                if (question_id != questionDto.question_id)
                {
                    response.code = "400";
                    response.msg = "Incorrect ID";
                    return new BadRequestObjectResult(response);
                }
                
                //CHECKING IF FORM ID IS VALID
                AdmForm form = _dbContext.admForms.Find(form_id);
                if (form == null)
                {
                    response.code = "400";
                    response.msg = "Form not found";
                    return new BadRequestObjectResult(response);
                }

                //CHECKING IF GROUP ID IS VALID
                AdmGroup group = _dbContext.admGroups.Find(group_id);
                if (group == null)
                {
                    response.code = "400";
                    response.msg = "Group not found";
                    return new BadRequestObjectResult(response);
                }

                //CHECKING IF TYPOLOGY VALUE IS VALID
                AdmTypology typology = _dbContext.admTypologies.Find(questionDto.typology_id) ?? new AdmTypology
                {
                    typology_id = CareConstants.EMPTY_TYPOLOGY
                };

                //CHECKING IF STATUS VALUE IS VALID
                AdmTypology status = _dbContext.admTypologies.Find(questionDto.status.typology_id) ??
                                     _dbContext.admTypologies.Find(CareConstants.ESTADO_ACTIVO);

                //CHECKING IF USER IS VALID
                AdmUser user = _dbContext.admUsers.Find(questionDto.created_by_user.user_id);
                if (user == null)
                {
                    response.code = "400";
                    response.msg = "User not found";
                    return new BadRequestObjectResult(response);
                }

                int questionId = 0;
                //IF 0, THEN PERSIST
                if (questionDto.question_id == 0)
                {
                    questionId = this.persistQuestion(questionDto, group, typology, status, user);
                }
                //ELSE UPDATE
                else if (questionDto.question_id > 0)
                {
                    AdmQuestion currentQuestion = _dbContext.admQuestions.Find(questionDto.question_id);
                    if (currentQuestion == null)
                    {
                        response.code = "400";
                        response.msg = "Question not found";
                        return new BadRequestObjectResult(response);
                    }
                    questionId = this.updateQuestion(questionDto, group, typology, status);
                }
                //ELSE INCORRECT ID
                else
                {
                    response.code = "400";
                    response.msg = "Question id is incorrect";
                    return new BadRequestObjectResult(response);
                }
                
                response.msg = "Success";
                response.code = "Ok";
                response.id = questionId;
                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                return StatusCode(400);
            }
        }

        private int updateQuestion(AdmQuestionDto questionDto, AdmGroup group, AdmTypology typology, AdmTypology status)
        {
            using (var scope = new TransactionScope())
            {
                AdmQuestion question = new AdmQuestion();
                question.question_id = questionDto.question_id;
                question.name_question = questionDto.name_question;
                question.type = questionDto.type;
                question.use_custom_option = questionDto.use_custom_option ?? false;
                question.group = group;
                question.typology = typology;
                question.status = status;
                
                question.question_id =_admQuestion.update(question);
                scope.Complete();
                return question.question_id;
            }
        }

        private int persistQuestion(AdmQuestionDto questionDto, AdmGroup group, AdmTypology typology,
            AdmTypology status, AdmUser user)
        {
            using (var scope = new TransactionScope())
            {
                AdmQuestion question = new AdmQuestion();
                question.name_question = questionDto.name_question;
                question.type = questionDto.type;
                question.use_custom_option = questionDto.use_custom_option ?? false;
                question.group = group;
                question.typology = typology;
                question.status = status;
                question.created_by_user = user;
                question.date_create = CsnFunctions.now();
                question.question_id = _admQuestion.persist(question);

                scope.Complete();
                return question.question_id;
            }
        }
        */
    }
}