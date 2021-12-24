using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using care_core.dto.AdmSurvey;
using care_core.dto.AdmTypology;
using Newtonsoft.Json.Linq;
using NPOI.OpenXmlFormats.Dml.Diagram;
using Serilog;

namespace care_core.controllers
{
    [Route("rest/surveys")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmSurveyController : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmSurvey _admSurvey;
        private readonly IAdmOption _admOption;
        private readonly IAdmQuestion _admQuestion;
        private readonly IAdmAnswer _admAnswer;


        private JsonResponse response;

        public AdmSurveyController(EntityDbContext dbContext, IAdmSurvey admSurvey, IAdmOption admOption,
            IAdmQuestion admQuestion, IAdmAnswer admAnswer)
        {
            _dbContext = dbContext;
            _admSurvey = admSurvey;
            _admOption = admOption;
            _admQuestion = admQuestion;
            _admAnswer = admAnswer;
            response = new JsonResponse();
        }

        [HttpGet("v1/{form_id}/info")]
        public IActionResult GetAllSurveysUserInfo([FromQuery] int status_id, int form_id)
        {
            IEnumerable<object> surveys = _admSurvey.getAllSurveysUserInfo(status_id, form_id);
            return Ok(surveys);
        }

        [HttpPut("v1/{survey_id}/update")]
        public IActionResult UpdateSurvey([FromBody] AdmSurveyDto surveyDto, [FromRoute] int survey_id)
        {
            if (survey_id != surveyDto.survey_id)
            {
                response.msg = "Incorrect ID";
                response.code = "Bad Request";
                response.id = survey_id;

                return StatusCode(400, response);
            }

            AdmSurvey survey = _dbContext.admSurveys.Find(surveyDto.survey_id);
            if (survey == null)
            {
                response.msg = "Survey Not found";
                response.code = "Not Found";
                response.id = survey_id;

                return StatusCode(404, response);
            }

            AdmTypology status = _dbContext.admTypologies.Find(surveyDto.status.typology_id);
            if (status == null)
            {
                response.msg = "Incorrect status";
                response.code = "Bad Request";
                response.id = (long) surveyDto.status.typology_id;

                return StatusCode(404, response);
            }

            survey.status = status;

            int respuesta = _admSurvey.update(survey);

            //on cascade set survey's answers to inactive
            if (survey.status.typology_id == CareConstants.ESTADO_INACTIVO)
            {
                List<AdmAnswer> respuestas = _dbContext.admAnswers
                    .Where(answer => answer.survey.survey_id == survey.survey_id).ToList();

                foreach (var answer in respuestas)
                {
                    answer.status = status;
                    int answerId = _admAnswer.update(answer);
                }
            }

            return Ok(respuesta);
        }

        //updates an array of surveys
        [HttpPut("v1/")]
        public IActionResult UpdateSurvey([FromBody] AdmSurveyDto[] surveyDtos)
        {
            foreach (var surveyDto in surveyDtos)
            {
                AdmSurvey survey = _dbContext.admSurveys.Find(surveyDto.survey_id);
                if (survey != null)
                {
                    AdmTypology status = _dbContext.admTypologies.Find(surveyDto.status.typology_id);
                    if (status != null)
                    {
                        survey.status = status;
                        _admSurvey.update(survey);
                    }

                    //on cascade set survey's answers to inactive
                    if (survey.status.typology_id == CareConstants.ESTADO_INACTIVO)
                    {
                        List<AdmAnswer> respuestas = _dbContext.admAnswers
                            .Where(answer => answer.survey.survey_id == survey.survey_id).ToList();

                        foreach (var answer in respuestas)
                        {
                            answer.status = status;
                            int answerId = _admAnswer.update(answer);
                        }
                    }
                }
            }

            return Ok();
        }

        //method that returns all answers by form
        //https://dev.azure.com/People-Apps/CARE/_workitems/edit/1762
        [HttpGet("v1/{form_id}/data")]
        public IActionResult getSurveysByFormId(int form_id)
        {
            List<AdmSurveyDto> surveys = _admSurvey.getAllByForm(form_id, true, false).ToList();
            if (!surveys.Any())
            {
                return NoContent();
            }

            var questions = _admQuestion.getAllByForm(form_id).ToList();

            AdmSurveyAnswersDto responseObject = new AdmSurveyAnswersDto();
            responseObject.form_id = form_id;
            responseObject.name_form = surveys[0].form.name_form;
            responseObject.question_labels = new List<string>();
            //adding constant values
            responseObject.question_labels.Add("Id");
            responseObject.question_labels.Add("Fecha");
            responseObject.question_labels.Add("Usuario");
            //adding questions
            foreach (var question in questions)
            {
                responseObject.question_labels.Add(question.name_question);
            }

            //adding answers
            responseObject.answers = new List<List<string>>();

            //Lista que contendra las respuestas a cada pregunta para utilizar en la creacion del archivo excel 
            List<AdmReportAnswerDto> pivote = new List<AdmReportAnswerDto>();

            foreach (var boleta in surveys)
            {
                AdmReportAnswerDto reporte = new AdmReportAnswerDto();
                //Adding headers
                reporte.surveyId = boleta.survey_id;
                reporte.userName = boleta.created_by_user.email;
                reporte.dateCreated = boleta.date_created;

                reporte.elementos = new List<AdmReportAnswerDto.AdmQuestionAnswerDto>();
                foreach (var respuesta in boleta.answers)
                {
                    AdmReportAnswerDto.AdmQuestionAnswerDto elemento = new AdmReportAnswerDto.AdmQuestionAnswerDto();
                    elemento.preguntaId = respuesta.question_id;
                    elemento.respuesta = respuesta.answer;
                    reporte.elementos.Add(elemento);
                }

                pivote.Add(reporte);
            }

            //from pivote to responseObject
            foreach (var survey in pivote)
            {
                List<string> boletaRespuestas = new List<string>();
                boletaRespuestas.Add(survey.surveyId.ToString());
                boletaRespuestas.Add(survey.dateCreated.ToString());
                boletaRespuestas.Add(survey.userName);


                for (int i = 0; i < questions.Count(); i++)
                {
                    try
                    {
                        //trying to access to every element using preguntas index
                        if (survey.elementos[i].preguntaId == questions[i].question_id)
                        {
                            //adds answer to output result
                            boletaRespuestas.Add(survey.elementos[i].respuesta);
                        }
                    }
                    catch (Exception ex)
                    {
                        boletaRespuestas.Add("");
                    }
                }

                //if survey has no answers
                if (survey.elementos.Count == 0)
                {
                    for (int a = 0; a < (questions.Count() - 1); a++)
                    {
                        boletaRespuestas.Add("");
                    }
                }

                responseObject.answers.Add(boletaRespuestas);
            }

            //return Ok(pivote);
            return Ok(responseObject);
        }


        [HttpGet("v1/stats/{formId}")]
        public IActionResult GetSurveyCounterByForm([FromRoute] int formId)
        {
            var form = _dbContext.admForms.Find(formId);
            if (form == null)
            {
                response.code = "404";
                response.msg = "Form not found";
                return new BadRequestObjectResult(response);
            }

            List<AdmSurveyDto> surveys = _admSurvey.getSurveyCounterByForm(form.form_id).ToList();
            if (surveys.Count == 0)
            {
                response.code = "404";
                response.msg = "No available surveys not found";
                return new BadRequestObjectResult(surveys);
            }


            //return Ok(surveys);
            AdmSurveyStatsDto statsDtos = new AdmSurveyStatsDto();
            statsDtos.total_surveys = surveys.Count();
            statsDtos.questions = new List<AdmSurveyStatsDto.AdmQuestionStatDto>();
/////////////////

            //dynamic counters
            //getting options

            if (surveys.Count <= 0) return Ok(statsDtos);

            var questions = _admQuestion.getAllByForm(formId);

            //create a dictionary for every question
            foreach (var question in questions)
            {
                if (question.use_for_counter != null && (bool) question.use_for_counter)
                {
                    //Contains question summary (name, type, stats)
                    AdmSurveyStatsDto.AdmQuestionStatDto questionStatDto =
                        new AdmSurveyStatsDto.AdmQuestionStatDto();

                    questionStatDto.question_id = question.question_id;
                    questionStatDto.question_name = question.name_question;
                    questionStatDto.question_type = question.type;
                    Dictionary<string, int> diccionario = new Dictionary<string, int>();
                    List<AdmQuestionOptionDto> grupoOpciones =
                        _admOption.getAllByQuestion(question.question_id, CareConstants.ESTADO_ACTIVO).ToList();

                    //Add options to dictionary
                    foreach (AdmQuestionOptionDto grupo in grupoOpciones)
                    {
                        diccionario.Add(grupo.value.ToLower(), 0);
                        //Log.Error(grupo.value.ToLower());
                    }

                    //adding values for typology (state), yes / no questions, initial/final
                    if (diccionario.Count == 0)
                    {
                        //for state question type
                        if (question.type != null && question.type.Equals("state"))
                        {
                            //Log.Error("---------> " + question.name_question);
                            //we add all states to the dictionary, later we will change it to the  state name
                            diccionario = getStateIdDictionary();
                        }

                        //for initial-last question type
                        if (question.type != null && question.type.Equals("inital-last"))
                        {
                            //Log.Error("---------> " + question.name_question);
                            diccionario = getInitialLastDictionary();
                        }

                        //for initial-last question type
                        if (question.type != null && question.type.Equals("boolean"))
                        {
                            //Log.Error("---------> " + question.name_question);
                            diccionario = getYesNoDictionary();
                        }
                    }

                    //for other types of questions, select
                    if (diccionario.Count > 0)
                    {
                        //calculate counters
                        diccionario = calculateCounters(diccionario, surveys, question.question_id);
                        //Log.Error(answerDto.question_name);
                        //changes from typology id to iso code (typology.value_1)
                        if (question.type != null && question.type.Equals("state"))
                        {
                            diccionario = setStateCode(diccionario);
                        }

                        questionStatDto.stats = diccionario;
                        statsDtos.questions.Add(questionStatDto);
                    }
                }
            }

            return Ok(statsDtos);
        }

        private Dictionary<string, int> setStateCode(Dictionary<string, int> diccionario)
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();

            foreach (var elemento in diccionario)
            {
                //only add elements with counter > 1
                if (elemento.Value > 0)
                {
                    int typologyId;
                    //parsing string to int
                    bool isValidNumber = Int32.TryParse(elemento.Key, out typologyId);
                    if (isValidNumber)
                    {
                        var typology = _dbContext.admTypologies.Find(typologyId);
                        //if typology exists, get description
                        if (typology != null)
                        {
                            resultado.Add(typology.value_1, elemento.Value);
                        }
                    }
                }
            }

            return resultado;
        }

        private Dictionary<string, int> calculateCounters(Dictionary<string, int> diccionario,
            List<AdmSurveyDto> surveys, int questionId)
        {
            foreach (var survey in surveys)
            {
                foreach (var answer in survey.answers)
                {
                    foreach (var opcion in answer.answer.Split(','))
                    {
                        try
                        {
                            //Log.Error("Opcion: " + opcion.ToLower());
                            //we need to check in order to count only for the correct answer
                            //or it will add for any answer that matches the option (example: SI/NO)
                            if (questionId == answer.question_id)
                            {
                                if (diccionario.ContainsKey(opcion.ToLower()))
                                {
                                    diccionario[opcion.ToLower()] += 1;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //Log.Error(ex.Message);
                        }
                    }
                }
            }

            return diccionario;
        }

        private Dictionary<string, int> getStateIdDictionary()
        {
            Dictionary<string, int> contadores = new Dictionary<string, int>()
            {
                {"160061", 0},
                {"160062", 0},
                {"160063", 0},
                {"160064", 0},
                {"160065", 0},
                {"160066", 0},
                {"160067", 0},
                {"160068", 0},
                {"160069", 0},
                {"160070", 0},
                {"160071", 0},
                {"160072", 0},
                {"160073", 0},
                {"160074", 0},
                {"160075", 0},
                {"160076", 0},
                {"160077", 0},
                {"160078", 0},
                {"160079", 0},
                {"160080", 0},
                {"160081", 0},
                {"160082", 0}
            };
            return contadores;
        }

        private Dictionary<string, int> getInitialLastDictionary()
        {
            Dictionary<string, int> contadores = new Dictionary<string, int>()
            {
                {"inicial", 0},
                {"final", 0},
            };
            return contadores;
        }

        private Dictionary<string, int> getYesNoDictionary()
        {
            Dictionary<string, int> contadores = new Dictionary<string, int>()
            {
                {"si", 0},
                {"no", 0},
            };
            return contadores;
        }
    }
}