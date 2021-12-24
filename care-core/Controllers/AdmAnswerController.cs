using System;
using System.Collections.Generic;
using System.Transactions;
using care_core.Controllers.util;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using care_core.dto.AdmAnswerDto;
using care_core.dto.AdmForm;
using care_core.model;
using Microsoft.Extensions.Logging;
using care_core.dto.AdmQuestionGroup;
using care_core.dto.AdmTypology;
using Serilog;

namespace care_core.controllers
{
    [Route("rest/answers")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmAnswerController : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmSurvey _admSurvey;
        private readonly IAdmAnswer _admAnswer;
        private readonly IAdmUser _admUser;

        private JsonResponse response;

        public AdmAnswerController(EntityDbContext dbContext, IAdmSurvey admSurvey, IAdmAnswer admAnswer, IAdmUser admUser)
        {
            _dbContext = dbContext;
            _admSurvey = admSurvey;
            _admAnswer = admAnswer;
            _admUser = admUser;
            response = new JsonResponse();
        }

        [HttpPost("v1/form/{form_id}")]
        [AllowAnonymous]
        public IActionResult Post([FromBody] AdmAnswerDto[] admAnswer, [FromRoute] int form_id)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        AdmForm form = _dbContext.admForms.Find(form_id);
                        //New survey
                        AdmSurvey survey = new AdmSurvey();
                        survey.form = form;
                        survey.date_create = CsnFunctions.now();
                        survey.survey_id = CareConstants.ZERO_DEFAULT;
                        survey.status = _dbContext.admTypologies.Find(CareConstants.ESTADO_ACTIVO);
                        
                        foreach (AdmAnswerDto e in admAnswer)
                        {
                            AdmQuestion question = _dbContext.admQuestions.Find(e.question_id);
                            //If user is set to null, we use public user, else, we search the user
                            //when null C# gives 0 to user id on dto
                            
                            AdmUser user;
                            //we use the public user
                            user = e.created_by_user.user_id == 0 
                                ? _admUser.findByEmail(CareConstants.GUEST_USER_EMAIL) 
                                : _dbContext.admUsers.Find(e.created_by_user.user_id);

                            if (survey.survey_id == 0)
                            {
                                survey.created_by_user = user;
                                //persist survey
                                survey.survey_id = _admSurvey.persist(survey);
                            }

                            //Create a new instance of AdmAnswer
                            AdmAnswer answer = new AdmAnswer();

                            answer.answer_id = 0;
                            answer.survey = survey;
                            answer.answer = e.answer;
                            answer.question = question;
                            answer.created_by_user = user;
                            answer.date_created = CsnFunctions.now();
                            //setting default status to ACTIVE
                            answer.status = survey.status;

                            _dbContext.Add(answer);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error" + ex.Message);
                        return StatusCode(400);
                    }

                    _dbContext.SaveChanges();

                    scope.Complete();
                    response.msg = "Success";
                    response.code = "Ok";
                    response.id = 0;

                    return StatusCode(201, response);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                return StatusCode(400);
            }
        }

        [HttpGet("v1")]
        public IActionResult GetAllAnswers()
        {
            //IEnumerable<AdmSurveyDto> boletas = _admSurvey.getAll(0, true);
            IEnumerable<AdmAnswerDto> boletas = _admAnswer.getAll();
            return Ok(boletas);
        }
        
        [HttpPut("v1")]
        public IActionResult UpdateSurvey([FromBody] AdmAnswerDto[] answerDtos)
        {
            foreach (var answerDto in answerDtos)
            {
                //check if every answer is valid
                AdmAnswer answer = _dbContext.admAnswers.Find(answerDto.answer_id);
                if (answer != null)
                {
                    //check if status is valid for every answer
                    AdmTypology status = _dbContext.admTypologies.Find(answerDto.status.typology_id);
                    if (status != null)
                    {
                        //update answer status
                        answer.status = status;
                        _admAnswer.update(answer);
                    }
                    
                }
            }
            return Ok();
        }
    }
}