using System;
using System.Collections.Generic;
using System.Globalization;
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
using care_core.dto.AdmSearch;
using care_core.dto.AdmTypology;
using Serilog;

namespace care_core.controllers
{
    [Route("rest/search/v1")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmSearchController : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmTypology _admTypology;
        private readonly IAdmAnswer _admAnswer;

        private JsonResponse response;

        public AdmSearchController(EntityDbContext dbContext, IAdmTypology admTypology, IAdmAnswer admAnswer)
        {
            _dbContext = dbContext;
            _admTypology = admTypology;
            _admAnswer = admAnswer;
            response = new JsonResponse();
        }

        [HttpGet]
        public IActionResult GetAllSurveysUserInfo([FromQuery] string searchTerm)
        {
            AdmSearchDto respuesta = new AdmSearchDto();
            respuesta.total_records = 0;
            respuesta.records = new List<AdmSearchDto.AdmSearchResultDto>();
            //list containing typologies that match the search term
            List<AdmTypology> matchingTypologies = getTypologies(searchTerm).ToList();
            List<AdmAnswerDto> allAnswers = _admAnswer.getAllCompleteFormInfo().ToList();

            //check if answer matches to the typology id and adds it to the answer list
            //check only if there are matching typologies
            if (matchingTypologies.Count > 0)
            {
                List<AdmSearchDto.AdmSearchResultDto> typologyAnswerMatch =
                    getAnswerMatchByTypology(matchingTypologies, allAnswers);
                //Add elements to response list
                foreach (var searchResultDto in typologyAnswerMatch)
                {
                    respuesta.records.Add(searchResultDto);
                }
            }

            //check for questions different than typologies
            List<AdmSearchDto.AdmSearchResultDto> matchingAnswers = getMatchingNonTypology(allAnswers, searchTerm);
            if (matchingAnswers.Count > 0)
            {
                foreach (var searchResultDto in matchingAnswers)
                {
                    respuesta.records.Add(searchResultDto);
                }
            }

            respuesta.total_records = respuesta.records.Count;


            return Ok(respuesta);
        }

        //search for answers that match with the search term that are non typologies
        private List<AdmSearchDto.AdmSearchResultDto> getMatchingNonTypology(List<AdmAnswerDto> allAnswers,
            string searchTerm)
        {
            List<AdmSearchDto.AdmSearchResultDto> respuesta = new List<AdmSearchDto.AdmSearchResultDto>();
            var culture = CultureInfo.InvariantCulture.CompareInfo;
            //need to pass 2 checks in order to count for a match
            bool check1 = false;
            bool check2 = false;
            foreach (var answer in allAnswers)
            {
                //if searchTerm does form part of the answer, ignoring spanish symbols (acentos)
                if (culture.IndexOf(answer.answer, searchTerm, CompareOptions.IgnoreNonSpace) != -1)
                {
                    check1 = true;
                }

                //if answer  contains searchTerm ignoring case
                if (answer.answer.ToUpper().Contains(searchTerm.ToUpper()))
                {
                    check2 = true;
                }

                //need to pass 1 of the checks in order to count as a match
                if (check1 == true || check2 == true)
                {
                    //new instance for every match
                    AdmSearchDto.AdmSearchResultDto match = new AdmSearchDto.AdmSearchResultDto();
                    match.answer = answer.answer;
                    match.answer_id = answer.answer_id;
                    match.category_id = answer.survey.form.category.category_id;
                    match.category_name = answer.survey.form.category.name_category;
                    match.date_created = answer.date_created;
                    match.form_id = answer.survey.form.form_id;
                    match.form_name = answer.survey.form.name_form;
                    match.module_id = answer.survey.form.module.module_id;
                    match.module_name = answer.survey.form.module.name_module;
                    match.question = answer.question_name;
                    match.question_id = answer.question_id;
                    match.survey_id = answer.survey.survey_id;
                    //add element to return list
                    respuesta.Add(match);

                    check1 = false;
                    check2 = false;
                }
            }

            return respuesta;
        }

        //check if answer matches to the typology id and adds it to the answer list
        private List<AdmSearchDto.AdmSearchResultDto> getAnswerMatchByTypology(List<AdmTypology> matchingTypologies,
            List<AdmAnswerDto> allAnswers)
        {
            List<AdmSearchDto.AdmSearchResultDto> respuesta = new List<AdmSearchDto.AdmSearchResultDto>();
            foreach (var answer in allAnswers)
            {
                foreach (var typology in matchingTypologies)
                {
                    if (answer.answer.Equals(typology.typology_id.ToString()))
                    {
                        //new instance for every match
                        AdmSearchDto.AdmSearchResultDto match = new AdmSearchDto.AdmSearchResultDto();
                        match.answer = typology.description;
                        match.answer_id = answer.answer_id;
                        match.category_id = answer.survey.form.category.category_id;
                        match.category_name = answer.survey.form.category.name_category;
                        match.date_created = answer.date_created;
                        match.form_id = answer.survey.form.form_id;
                        match.form_name = answer.survey.form.name_form;
                        match.module_id = answer.survey.form.module.module_id;
                        match.module_name = answer.survey.form.module.name_module;
                        match.question = answer.question_name;
                        match.question_id = answer.question_id;
                        match.survey_id = answer.survey.survey_id;
                        //add element to return list
                        respuesta.Add(match);
                    }
                }
            }

            return respuesta;
        }

        //Returns typologies whose description matches the searchTerm ignoring spanish special symbols
        private List<AdmTypology> getTypologies(string searchTerm)
        {
            List<int> typologyIds = new List<int>();

            //Sends 0 to get all typologies
            List<AdmTypology> departamentos = _admTypology.getAll(0, false).ToList();
            var culture = CultureInfo.InvariantCulture.CompareInfo;
            //need to pass 2 checks in order to eliminate typology from matches
            bool check1 = false;
            bool check2 = false;
            foreach (var typology in departamentos.ToList())
            {
                //if searchTerm does not form part of typology description, ignoring spanish symbols (acentos)
                if (culture.IndexOf(typology.description, searchTerm, CompareOptions.IgnoreNonSpace) == -1)
                {
                    check1 = true;
                }

                //if typology description contains searchTerm ignoring case
                if (!typology.description.ToUpper().Contains(searchTerm.ToUpper()))
                {
                    check2 = true;
                }

                //need to pass 2 checks in order to eliminate typology from matches
                if (check1 == true && check2 == true)
                {
                    departamentos.Remove(typology);
                    check1 = false;
                    check2 = false;
                }
            }


            return departamentos;
        }
    }
}