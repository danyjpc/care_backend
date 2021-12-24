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
    public class AdmQuestionControllerBuilder : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmQuestion _admQuestion;
        private readonly IAdmOption _admOption;
        private readonly IAdmAnswer _admAnswer;
        private JsonResponse response;

        public AdmQuestionControllerBuilder(EntityDbContext dbContext, IAdmQuestion admQuestion, IAdmOption admOption, IAdmAnswer admAnswer)
        {
            _dbContext = dbContext;
            _admQuestion = admQuestion;
            _admOption = admOption;
            _admAnswer = admAnswer;
            response = new JsonResponse();
        }

        //method for getting questions from a group
        [HttpGet("form/{form_id}/group/{group_id}/questions")]
        public IActionResult getQuestionsByGroup(int form_id, int group_id, int estado_id)
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

                IEnumerable<AdmQuestionDto> questions = _admQuestion.getAllByGroup(form_id, group_id, estado_id);
                return new OkObjectResult(questions);
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                return StatusCode(400);
            }
        }

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

                    questionId = this.updateQuestion(questionDto, currentQuestion, group, typology, status, user);
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
                Log.Error("Error " + ex.Message);
                return StatusCode(400);
            }
        }

        [HttpPut("form/{form_id}/group/{group_id}/questions/{question_id}")]
        public IActionResult updateSingleQuestionInGroup(int form_id, int group_id, int question_id,
            AdmQuestionDto questionDto)
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

                //setting default typology for state (Departamento)
                if (questionDto.type != null && questionDto.type.Equals("state"))
                {
                    questionDto.typology_id = CareConstants.DEFAULT_STATE_ID;
                }

                //setting default typology for city (Municipio)
                if (questionDto.type != null && questionDto.type.Equals("city"))
                {
                    questionDto.typology_id = CareConstants.DEFAULT_CITY_ID;
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

                //Id Id > 0 update
                if (questionDto.question_id > 0)
                {
                    AdmQuestion currentQuestion = _dbContext.admQuestions.Find(questionDto.question_id);
                    if (currentQuestion == null)
                    {
                        response.code = "400";
                        response.msg = "Question not found";
                        return new BadRequestObjectResult(response);
                    }

                    questionId = this.updateQuestion(questionDto, currentQuestion, group, typology, status, user);
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

        //method that updates question order_index
        [HttpPut("form/")]
        public IActionResult updateQuestionOrderIndex(AdmQuestionDto[] questionDtos)
        {
            try
            {
                if (!questionDtos.Any())
                {
                    response.code = "400";
                    response.msg = "Incorrect questions";
                    return new BadRequestObjectResult(response);
                }

                //updating for every question on dto
                foreach (AdmQuestionDto questionDto in questionDtos)
                {
                    if (questionDto.question_id > 0)
                    {
                        AdmQuestion currentQuestion = _dbContext.admQuestions.Find(questionDto.question_id);
                        if (currentQuestion == null)
                        {
                            //go to next element
                            continue;
                        }

                        updateQuestionOrder(questionDto, currentQuestion);
                    }
                    //ELSE INCORRECT ID
                    else
                    {
                        response.code = "400";
                        response.msg = "Question id is incorrect";
                        return new BadRequestObjectResult(response);
                    }
                }


                response.msg = "Success";
                response.code = "Ok";
                response.id = 0;
                return StatusCode(200, response);
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                return StatusCode(400);
            }
        }

        private void updateQuestionOrder(AdmQuestionDto questionDto, AdmQuestion currentQuestion)
        {
            using (var scope = new TransactionScope())
            {
                currentQuestion.orderIndex = questionDto.order_index ?? currentQuestion.orderIndex;

                currentQuestion.question_id = _admQuestion.update(currentQuestion);

                scope.Complete();
            }
        }

        //end of method

        private int updateQuestion(AdmQuestionDto questionDto, AdmQuestion currentQuestion, AdmGroup group,
            AdmTypology typology, AdmTypology status,
            AdmUser user)
        {
            AdmQuestion question = new AdmQuestion();
            using (var scope = new TransactionScope())
            {
                question.question_id = questionDto.question_id;
                question.name_question = questionDto.name_question;
                question.type = questionDto.type;
                question.use_custom_option = questionDto.use_custom_option ?? false;
                question.use_for_counter = questionDto.use_for_counter ?? false;
                question.group = group;
                question.typology = typology;
                question.status = status;

                //updates question order
                question.orderIndex = questionDto.order_index ?? question.orderIndex;

                question.question_id = _admQuestion.update(question);
                
                //updating answers
                var answers = _admAnswer.getAnswersByQuestion(question.question_id).ToList();
                if (answers.Any())
                {
                    //disabling answers
                    if (question.status.typology_id == CareConstants.ESTADO_INACTIVO)
                    {
                        foreach (var answer in answers)
                        {
                            answer.status = question.status;
                            _dbContext.Entry(answer).State = EntityState.Modified;
                        }
                        _dbContext.SaveChanges();
                    }
                }

                //updating options
                if (questionDto.options != null)
                {
                    if (question.use_custom_option && question.typology.typology_id == CareConstants.EMPTY_TYPOLOGY)
                    {
                        foreach (AdmQuestionOptionDto optionDto in questionDto.options)
                        {
                            //updating only if id is different to 0
                            if (optionDto.option_id != 0)
                            {
                                AdmOption option = new AdmOption();
                                option.option_id = optionDto.option_id;
                                option.value = optionDto.value;
                                option.status = _dbContext.admTypologies.Find(optionDto.status_id);
                                option.option_id = _admOption.update(option);
                            }

                            else if (optionDto.option_id == 0)
                            {
                                //Detaching question from dbContext
                                _dbContext.Entry(question).State = EntityState.Detached;

                                AdmOption option = new AdmOption();
                                option.value = optionDto.value;
                                option.question = _dbContext.admQuestions.Find(question.question_id);
                                option.created_by_user = user;
                                option.status = _dbContext.admTypologies.Find(CareConstants.ESTADO_ACTIVO);
                                ;
                                option.date_create = CsnFunctions.now();

                                option.option_id = _admOption.persist(option);
                            }
                        }
                    }
                }

                scope.Complete();
            }


            return question.question_id;
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
                question.use_for_counter = questionDto.use_for_counter ?? false;
                question.group = group;
                question.typology = typology;
                question.status = status;
                question.created_by_user = user;
                question.date_create = CsnFunctions.now();
                //order index, get last index by group, if null, return 0 + 1
                question.orderIndex = questionDto.order_index ?? 0;

                var questionList = _dbContext.admQuestions.Where(x => x.group.group_id == group.group_id)
                    .ToList();

                //get max value, if group has no questions, default to 0
                int maxIndex = questionList.Any() ? questionList.Max(x => x.orderIndex) : 0;
                
                //add 1 to max value and assign it
                question.orderIndex = maxIndex + 1;
                
                question.question_id = _admQuestion.persist(question);

                //Persisting options
                if (questionDto.options != null)
                {
                    foreach (AdmQuestionOptionDto optionDto in questionDto.options)
                    {
                        //persisting only if id equals 0
                        if (optionDto.option_id == 0)
                        {
                            AdmOption option = new AdmOption();
                            option.value = optionDto.value;
                            option.question = question;
                            option.created_by_user = user;
                            option.status = status;
                            option.date_create = CsnFunctions.now();
                            option.option_id = _admOption.persist(option);
                        }
                    }
                }

                scope.Complete();
                return question.question_id;
            }
        }
    }
}