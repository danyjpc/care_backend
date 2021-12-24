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
using care_core.dto.AdmSurvey;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using NPOI.OpenXmlFormats.Dml.Diagram;
using Serilog;

namespace care_core.controllers
{
    [Route("rest/stats/v1")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmGeneralStatsController : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmSurvey _admSurvey;
        private readonly IAdmModule _admModule;
        private readonly IAdmForm _admForm;
        private readonly IAdmUser _admUser;


        private JsonResponse response;

        public AdmGeneralStatsController(EntityDbContext dbContext, IAdmSurvey admSurvey, IAdmModule admModule,
            IAdmForm admForm,
            IAdmUser admUser)
        {
            _dbContext = dbContext;
            _admSurvey = admSurvey;
            _admModule = admModule;
            _admForm = admForm;
            _admUser = admUser;
            response = new JsonResponse();
        }

        [HttpGet("general")]
        public IActionResult GetAllSurveysUserInfo([FromQuery] int module_id,
            [FromQuery] int state,
            [FromQuery] string date_start,
            [FromQuery] string date_end)
        {
            //check if module filter should be applied
            IEnumerable<AdmModuleDto> moduleResult = new List<AdmModuleDto>();
            if (module_id == 0)
            {
                moduleResult = _admModule.getAll(CareConstants.ESTADO_ACTIVO);
            }
            else
            {
                AdmModuleDto module = _admModule.getModuleById(module_id);
                if (module == null)
                {
                    response.code = "404";
                    response.msg = "Module not found";
                    return new NotFoundObjectResult(moduleResult);
                }

                moduleResult = moduleResult.Append(module);
            }
            
           

            //check if dates are valid
            //YYYY-MM-DD
            //January 1st of current year
            DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1);
            //December 31st of current year
            DateTime endDate = new DateTime(DateTime.Now.Year, 12, 31);
            if (StringMatcher.isDate(date_start) && StringMatcher.isDate(date_end))
            {
                startDate = Convert.ToDateTime(date_start + " 00:00:00");
                endDate = Convert.ToDateTime(date_end + " 23:59:59");
            }

            //if start date is bigger (later) than end date return empty list and bad request
            if (startDate > endDate)
            {
                return new BadRequestObjectResult(new List<AdmModulDto>());
            }
            //List that contains all forms
            List<AdmFormDto> formDtos = new List<AdmFormDto>();
            //List that contains all surveys
            List<AdmSurveyDto> surveyDtos = new List<AdmSurveyDto>();
            if (!moduleResult.Any()) return Ok(moduleResult);

            //check every survey and add to totals
            AdmGeneralStatsDto responseObject = new AdmGeneralStatsDto();
            responseObject.modules = new Dictionary<string, int>();
            responseObject.forms = new Dictionary<string, int>();
            responseObject.dates = getMonthDictionary(startDate, endDate);
            responseObject.states = getStateIdDictionary();
            responseObject.user_surveys = getUserDictionary();

            foreach (var module in moduleResult)
            {
                foreach (var form in _admForm.getAllByModule(module.module_id))
                {
                    //adding forms to forms list
                    formDtos.Add(form);
                }
            }

            //get surveys by form
            foreach (var form in formDtos)
            {
                //filter by dates
                foreach (var survey in _admSurvey.getAllByForm(form.form_id, true, true)
                    .Where(x=>x.date_created >= startDate && x.date_created <= endDate))
                {
                    surveyDtos.Add(survey);
                }
            }
            
            //filter by state
            //removes surveys that do not contain state question and state id as answer
            if (state > 0)
            {
                foreach (AdmSurveyDto survey in surveyDtos.ToList())
                {
                    bool removeSurvey = true;
                    foreach (AdmAnswerDto answer in survey.answers.ToList())
                    {
                        
                        //if survey has matching elements it is kept, else it is removed from the list
                        if (answer.question_type.Equals("state") && answer.answer.Equals(state.ToString()))
                        {
                            removeSurvey = false;
                        }
                    }

                    if (removeSurvey)
                    {
                        surveyDtos.Remove(survey);
                    }
                }
                
            }

            foreach (var module in moduleResult)
            {
                //Log.Error("Calculating Module ID: " + module.module_id);
                List<AdmSurveyDto> moduleSurveys =
                    surveyDtos.Where(survey => survey.form.module.module_id == module.module_id).ToList();

                //setting module dictionary values
                responseObject.modules.Add(module.name_module, moduleSurveys.Count);

                foreach (var survey in moduleSurveys)
                {
                    //Log.Error("State Total: " + moduleStats.state_total);

                    //Totaling state answers
                    foreach (var answer in survey.answers)
                    {
                        if (answer.question_type.Equals("state") && !answer.answer.Equals(""))
                        {
                            responseObject.states[answer.answer] += 1;
                        }
                    }
                }

                //calculating for each form in module
                foreach (var form in formDtos)
                {
                    if (form.module.module_id == module.module_id)
                    {
                        //get surveys by form
                        List<AdmSurveyDto> formSurveys =
                            surveyDtos.Where(survey => survey.form.form_id == form.form_id).ToList();

                        //setting form dictionary
                        responseObject.forms.Add(form.name_form, formSurveys.Count);

                        foreach (var survey in formSurveys)
                        {
                            //counting for month dictionary
                            //get month name lowercase and year
                            string matchValue = survey.date_created.ToString("MMMM").ToLower() + " " +
                                                survey.date_created.Year;
                            responseObject.dates[matchValue] += 1;

                            //counting for user dictionary
                            responseObject.user_surveys[survey.created_by_user.email] += 1;
                        }
                    }
                }
            }

            //change state dictionary from typology id to state code
            responseObject.states = setStateCode(responseObject.states);
            
            //remove values with 0 on counter
            responseObject.dates = removeZeroValueCounter(responseObject.dates);
            responseObject.forms = removeZeroValueCounter(responseObject.forms);
            responseObject.modules = removeZeroValueCounter(responseObject.modules);
            responseObject.states = removeZeroValueCounter(responseObject.states);
            responseObject.user_surveys = removeZeroValueCounter(responseObject.user_surveys);
            
            return Ok(responseObject);
        }

        private Dictionary<string, int> removeZeroValueCounter(Dictionary<string, int> dictionary)
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();
            foreach (var element in dictionary)
            {
                if (element.Value > 0)
                {
                    resultado.Add(element.Key, element.Value);
                }
            }

            return resultado;

        }

        //returns a dictionary that contains users' emails
        private Dictionary<string, int> getUserDictionary()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            List<AdmUserDto> users = _admUser.getAllDto().ToList();
            foreach (var user in users)
            {
                result.Add(user.email, 0);
            }

            return result;
        }

        //dictionary used for counting by date
        private Dictionary<string, int> getMonthDictionary(DateTime startDate, DateTime endDate)
        {
            //generate month's name for every year between startDate and endDate
            Dictionary<string, int> resultado = new Dictionary<string, int>();
            for (int i = startDate.Year; i <= endDate.Year; i++)
            {   resultado.Add("january " + i, 0);
                resultado.Add("february " + i, 0);
                resultado.Add("march " + i, 0);
                resultado.Add("april " + i, 0);
                resultado.Add("may " + i, 0);
                resultado.Add("june " + i, 0);
                resultado.Add("july " + i, 0);
                resultado.Add("august " + i, 0);
                resultado.Add("september " + i, 0);
                resultado.Add("october " + i, 0);
                resultado.Add("november " + i, 0);
                resultado.Add("december " + i, 0);
                
            }
            return resultado;
        }

        //once we complete counting every answer by state, we replace the dictionary with one containing the
        //typology value_1 (iso code for states)
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

        //sets typology ids to dictionary values, we use the typology id as the answer for state type questions
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
    }
}