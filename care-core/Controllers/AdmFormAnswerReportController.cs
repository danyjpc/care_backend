using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using care_core.Controllers.util;
using care_core.dto.AdmForm;
using care_core.dto.AdmQuestionGroup;
using care_core.dto.AdmSurvey;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Expressions;
using Serilog;
using Serilog.Core;

namespace care_core.Controllers
{
    [Route("rest/reports/v1")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmFormAnswerReportController : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmQuestion _admQuestion;
        private readonly IAdmAnswer _admAnswer;
        private readonly IAdmSurvey _admSurvey;
        private JsonResponse response;

        public AdmFormAnswerReportController(EntityDbContext dbContext, IAdmQuestion admQuestion, IAdmAnswer admAnswer,
            IAdmSurvey admSurvey)
        {
            _dbContext = dbContext;
            _admQuestion = admQuestion;
            _admAnswer = admAnswer;
            _admSurvey = admSurvey;
            response = new JsonResponse();
        }

        //Metodo que devuelve las preguntas y respuestas de un formulario tabuladas
        [HttpGet("form/{formId}")]
        public IActionResult getFormAnswerReport(
            [FromRoute] int formId,
            [FromQuery] string name_file,
            [FromQuery] string type)
        {
            name_file ??= "fileName";
            type ??= "excel";
            AdmForm form = _dbContext.admForms.Find(formId);
            if (form == null)
            {
                response.code = "400";
                response.msg = "Form not found";
                return new BadRequestObjectResult(response);
            }

            //Listado de boletas por formulario, estan ya incluyen la respuesta
            List<AdmSurveyDto> boletas = _admSurvey.getAllByForm(formId, true, false).ToList();

            //Listado de preguntas por formulario
            List<AdmQuestionDto> preguntas = _admQuestion.getAllByForm(formId).ToList();

            //Lista que contendra las respuestas a cada pregunta para utilizar en la creacion del archivo excel 
            List<AdmReportAnswerDto> pivote = new List<AdmReportAnswerDto>();

            if (boletas.Count == 0)
            {
                return Ok(pivote);
            }


            foreach (var boleta in boletas)
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
            //check missing questions for every survey
            //if a question has no answer it is added with an empty string
            
            //get id list from questions
            List<int> questionIds = new List<int>(preguntas.Count);
            questionIds.AddRange(preguntas.Select(x=>x.question_id));
            
            foreach (var reportAnswerDto in pivote)
            {
                //get question id list from survey answers
                List<int> currentQuestionIds = new List<int>(reportAnswerDto.elementos.Count);
                currentQuestionIds.AddRange(reportAnswerDto.elementos.Select(x=>x.preguntaId));

                //check if a question has no answer, if so, add that question id to the list with a blank answer
                var missingQuestionsIds = questionIds.Where(x => !currentQuestionIds.Contains(x)).ToList();
                foreach (var questionId in missingQuestionsIds)
                {
                    reportAnswerDto.elementos.Add(new AdmReportAnswerDto.AdmQuestionAnswerDto(){preguntaId = questionId, respuesta = ""});
                }

                //order the survey answers by id
                reportAnswerDto.elementos = reportAnswerDto.elementos.OrderBy(x => x.preguntaId).ToList();
            }
            //for testing
            //return Ok(pivote);
            //return Ok(preguntas);

            if (type != "excel")
            {
                return File(CsnFunctions.createAnswerReportCsv(pivote, name_file, preguntas),
                    System.Net.Mime.MediaTypeNames.Text.Plain, name_file + ".csv");
               
            }
            //returns xlsx by default
            return File(CsnFunctions.createAnswerReportExcel(pivote, name_file, preguntas),
                System.Net.Mime.MediaTypeNames.Application.Octet, name_file + ".xlsx");
            
        }
    }
}