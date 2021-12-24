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
    [Route("rest/surveys/v1")]
    [ApiController]
    [Authorize(Roles = CareConstants.ALL_ROLES_ALLOWED)]
    public class AdmQuestionController : ControllerBase
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmQuestion _admQuestion;
        private JsonResponse response;

        public AdmQuestionController(EntityDbContext dbContext, IAdmQuestion admQuestion)
        {
            _dbContext = dbContext;
            _admQuestion = admQuestion;
            response = new JsonResponse();
        }


        //Metodo que devuelve las preguntas de un formulario 
        [HttpGet("{form_id}/questions")]
        [AllowAnonymous]
        public IActionResult GetQuestionById(int form_id, [FromQuery] int status_id)
        {
            //Lista de las preguntas que se devolveran con el formato deseado
            List<AdmQuestionResponseDto> questionResponseDtos = new List<AdmQuestionResponseDto>();

            //Se busca los grupos del formulario
            var groups = _dbContext.admGroups.Where(x => x.form.form_id == form_id
                && x.status.typology_id == CareConstants.ESTADO_ACTIVO).Select(
                group => new
                {
                    group_id = group.group_id,
                    name_group = group.name_group
                }
            ).OrderBy(i => i.group_id).ToArray();

            //Se recorre los grupos para obtener sus preguntas
            foreach (var group in groups)
            {
                //Se busca las preguntas en base al group id
                var questions = _dbContext.admQuestions.Where(x => x.group.group_id == group.group_id 
                                                                   && x.status.typology_id == (status_id == 0 ? CareConstants.ESTADO_ACTIVO :status_id))
                    .Select(
                        quest => new AdmQuestionDto2
                        {
                            question_id = quest.question_id,
                            name_question = quest.name_question,
                            type = quest.type,
                            use_custom_option = quest.use_custom_option,
                            typology_id = quest.typology.typology_id,
                            order_index = quest.orderIndex
                            
                            /*options = GetListOption(quest.question_id, quest.typology.typology_id,
                                                    quest.use_custom_option, quest.type)*/
                        }
                    ).ToArray();

                    //Lista de las preguntas con el campo option y group
                    List<AdmQuestDto> quest = new List<AdmQuestDto>();

                    foreach (var item in questions)
                    {
                        quest.Add( new AdmQuestDto()
                        {
                            question_id = item.question_id,
                            name_question = item.name_question,
                            type = item.type,
                            use_custom_option = item.use_custom_option,
                            typology_id = item.typology_id,
                            options = GetListOption(item.question_id, item.typology_id,
                                                    item.use_custom_option),
                            
                            order_index = item.order_index

                        });
                    }


                //Se llena el listado de las preguntas con el formato final
                questionResponseDtos.Add(new AdmQuestionResponseDto()
                {
                    //group_id = 0,
                    name_group = group.name_group,
                    questions = quest.ToArray(),
                });
            }

            
            
            //sorting questions by id
            foreach (var questionResponseDto in questionResponseDtos)
            {
                questionResponseDto.questions =
                    questionResponseDto.questions.OrderBy(x => x.order_index).ThenBy(x=>x.question_id).ToArray();
            }

            
            //Se devuelve las preguntas por grupo
            return Ok(questionResponseDtos);
        }

        //Se utliza para llenar el campo option en una pregrunta
        private List<AdmQuestionOptionDto> GetListOption(int questionId, int typologyId, bool useOption)
        {
            List<AdmQuestionOptionDto> list = new List<AdmQuestionOptionDto>();

                if (useOption == false && typologyId != 160000)
                {
                    var list_typology = _dbContext.admTypologies.Where(x => x.parent_typology.typology_id == typologyId)
                        .Select(
                            ty => new
                            {
                                typology_id = ty.typology_id,
                                description = ty.description
                            }
                        ).ToArray();

                    foreach (var i in list_typology)
                    {
                        list.Add(new AdmQuestionOptionDto()
                        {
                            option_id = i.typology_id,
                            value = i.description,
                            status_id = null,

                            //removing properties that are not being used
                            question = null,
                            status = null,
                            created_by_user = null
                        });
                    }
                }
                else if (useOption == true && typologyId == 160000)
                {
                    var list_option = _dbContext.admOptions.Where(x => x.question.question_id == questionId && x.status.typology_id == CareConstants.ESTADO_ACTIVO)
                        .Select(
                            op => new
                            {
                                option_id = op.option_id,
                                value = op.value,
                                status_id = op.status.typology_id
                            }
                        ).OrderBy(x=>x.option_id).ToArray();

                    foreach (var i in list_option)
                    {
                        list.Add(new AdmQuestionOptionDto()
                        {
                            option_id = i.option_id,
                            value = i.value,
                            status_id = i.status_id,
                            
                            //removing properties that are not being used
                            question = null,
                            status = null,
                            created_by_user = null
                            
                        });
                    }
                }

            return list;
        }

        [HttpPost("{form_id}/questions")]
        public IActionResult Post([FromRoute] int form_id, [FromBody] AdmQuestionGroupDto[] questGroup)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    /*
                    AdmForm form = _dbContext.admForms.Find(form_id);
                    AdmTypology status = _dbContext.admTypologies.Find(160445);

                    foreach (var x in questGroup)
                    {
                        AdmUser user = _dbContext.admUsers.Find(x.created_by.user_id);
                        AdmGroup group = _dbContext.admGroups.Find(Group(x));

                        foreach (var item in x.questions)
                        {
                            AdmQuestion admquestion = new AdmQuestion();

                            AdmTypology typology = _dbContext.admTypologies.Find(item.typology_id);

                            admquestion.name_question = item.name_question;
                            admquestion.type = item.type;
                            admquestion.use_custom_option = item.use_custom_option;
                            admquestion.form = form;
                            admquestion.typology = typology;
                            admquestion.status = status;
                            admquestion.created_by_user = user;
                            admquestion.date_create = CsnFunctions.now();


                            if (item.type.Equals("select", StringComparison.OrdinalIgnoreCase)
                                && item.use_custom_option == true && item.typology_id == 160000)
                            {
                                Option(item, user.user_id);
                            }
                            _dbContext.Add(admquestion);
                        }
                        //Error trata de insertar un admPerson, y por la columna cui que no permite
                        //repetidos da error 
                        save();
                    }
                    */
                    scope.Complete();
                    response.msg = "En proceso";
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

        private int Group(AdmQuestionGroupDto qG)
        {
            AdmGroup admGroup = new AdmGroup();
            AdmUser user = _dbContext.admUsers.Find(qG.created_by.user_id);
            AdmTypology status = _dbContext.admTypologies.Find(160445);

            admGroup.name_group = qG.name_group;
            admGroup.status = status;
            admGroup.created_by_user = user;
            admGroup.date_created = CsnFunctions.now();

            _dbContext.Add(admGroup);
            save();

            return admGroup.group_id;
        }

        private void Option(AdmQuestionDto questionDto, int user_id)
        {
            AdmQuestion question = _dbContext.admQuestions.Find(questionDto.question_id);
            AdmTypology status = _dbContext.admTypologies.Find(160445);
            AdmUser user = _dbContext.admUsers.Find(user_id);

            foreach (var i in questionDto.options)
            {
                AdmOption admoption = new AdmOption();

                admoption.value = i.value;
                admoption.question = question;
                admoption.status = status;
                admoption.created_by_user = user;
                admoption.date_create = CsnFunctions.now();
                _dbContext.Add(admoption);
            }

            save();
        }


        private void save()
        {
            _dbContext.SaveChanges();
        }

        //meto de prueba para optener las tipologias hijo
        [HttpGet("childs/{id}")]
        public IActionResult GetPT(int id)
        {
            var list_typology = _dbContext.admTypologies.Where(x => x.parent_typology.typology_id == id).Select(
                ty => new
                {
                    typology_id = ty.typology_id,
                    description = ty.description
                }
            ).ToArray();

            return Ok(list_typology);
        }

        //method for getting questions from a group
        [HttpGet("form/{form_id}/group/{group_id}/questions")]
        public IActionResult getQuestionsByGroup(int form_id, int group_id, int estado)
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

                IEnumerable<AdmQuestionDto> questions = _admQuestion.getAllByGroup(form_id, group_id, estado);
                return new OkObjectResult(questions);
            }
            catch (Exception ex)
            {
                Log.Error("Error" + ex.Message);
                return StatusCode(400);
            }
        }
        
    }
}