using System;
using System.Collections.Generic;
using System.Linq;
using care_core.dto.AdmCase;
using care_core.dto.AdmOrganization;
using care_core.dto.AdmOrganizationMember;
using care_core.dto.AdmPerson;
using care_core.dto.AdmQuestionGroup;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace care_core.repository
{
    public class AdmQuestionRepository : IAdmQuestion
    {
        private readonly EntityDbContext _dbContext;
        private readonly IAdmTypology _admTypology;

        public AdmQuestionRepository(EntityDbContext dbContext, IAdmTypology admTypology)
        {
            _dbContext = dbContext;
            _admTypology = admTypology;
        }

        public IEnumerable<AdmQuestionDto> getAllByGroup(int formId, int groupId, int estado)
        {
            IEnumerable<AdmQuestionDto> preguntas = _dbContext.admQuestions.Where(x => x.group.group_id == groupId
                && x.group.form.form_id == formId
                && x.status.typology_id == (estado > 0 ? estado : CareConstants.ESTADO_ACTIVO)).Select(
                pregunta => new AdmQuestionDto
                {
                    question_id = pregunta.question_id,
                    name_question = pregunta.name_question,
                    type = pregunta.type,
                    use_custom_option = pregunta.use_custom_option,
                    use_for_counter = pregunta.use_for_counter,
                    typology_id = pregunta.typology.typology_id,
                    status = new AdmTypologyDto()
                    {
                        typology_id = pregunta.status.typology_id,
                        description = pregunta.status.description
                    },
                    created_by_user = new AdmUserDto()
                    {
                        user_id = pregunta.created_by_user.user_id,
                        email = pregunta.created_by_user.person.email
                    },
                    order_index = pregunta.orderIndex
                }
            ).ToList();

            foreach (AdmQuestionDto pregunta in preguntas)
            {
                //adds options only if use_custom_option is true
                if (pregunta.use_custom_option != null && (bool) pregunta.use_custom_option)
                {
                    pregunta.options = _dbContext.admOptions.Where(y => y.question.question_id == pregunta.question_id && y.status.typology_id == CareConstants.ESTADO_ACTIVO)
                        .Select(
                            option => new AdmQuestionOptionDto()
                            {
                                option_id = option.option_id,
                                value = option.value,
                                status_id = option.status.typology_id,
                                created_by_user = null,
                                question = null,
                                status = null
                            }).OrderBy(y=>y.option_id).ToList();
                }
                else if (pregunta.use_custom_option != null && (bool) pregunta.use_custom_option == false)
                {
                    pregunta.options = new List<AdmQuestionOptionDto>();
                }

                //Adding options when value is a typology
                //searching the typology is needed
                //if list is empty and typology is different than 160000 append childs to the list
                if (pregunta.options != null && pregunta.options.Count == 0 &&
                    pregunta.typology_id != CareConstants.EMPTY_TYPOLOGY)
                {
                    //gettint typology
                    AdmTypology admTypology = _dbContext.admTypologies.Find(pregunta.typology_id);
                    if (admTypology != null)
                    {
                        //getting childs
                        List<AdmTypology> typologies = _admTypology.getAll(admTypology.typology_id, false).ToList();
                        //setting option values
                        foreach (AdmTypology typology in typologies)
                        {
                            AdmQuestionOptionDto optionDto =
                                new AdmQuestionOptionDto();
                            optionDto.created_by_user = null;
                            optionDto.status = null;
                            optionDto.question = null;
                            optionDto.option_id = typology.typology_id;
                            optionDto.value = typology.description;
                            pregunta.options.Add(optionDto);
                        }
                    }
                }
            }

            //sorting questions by id
            preguntas = preguntas.OrderBy(x => x.order_index).ThenBy(x=>x.question_id).ToList();
            //sorting question options
            foreach (var pregunta in preguntas)
            {
                if (pregunta.options != null)
                {
                    pregunta.options = pregunta.options.OrderBy(x => x.option_id).ToList();
                }
            }

            return preguntas;
        }

        public IEnumerable<AdmQuestionDto> getAllByForm(int form_id)
        {
            IEnumerable<AdmQuestionDto> preguntasDto = (
                from preguntas in _dbContext.admQuestions
                join grupos in _dbContext.admGroups
                    on preguntas.@group.group_id equals grupos.group_id
                join formularios in _dbContext.admForms
                    on grupos.form.form_id equals formularios.form_id
                where (formularios.form_id.Equals(form_id) && preguntas.status.typology_id.Equals(CareConstants.ESTADO_ACTIVO))
                select new AdmQuestionDto()
                {
                    question_id = preguntas.question_id,
                    name_question = preguntas.name_question,
                    type = preguntas.type,
                    use_custom_option = preguntas.use_custom_option,
                    use_for_counter = preguntas.use_for_counter,
                    typology_id = preguntas.typology.typology_id,
                    status = new AdmTypologyDto()
                    {
                        typology_id = preguntas.status.typology_id,
                        description = preguntas.status.description
                    },
                    created_by_user = new AdmUserDto()
                    {
                        user_id = preguntas.created_by_user.user_id,
                        email = preguntas.created_by_user.person.email
                    },
                    date_created = preguntas.date_create
                }).OrderBy(x => x.question_id).ToList();

            return preguntasDto;
        }

        public AdmQuestionDto getQuestionById(int questionId)
        {
            throw new NotImplementedException();
        }

        public int persist(AdmQuestion admQuestion)
        {
            _dbContext.Add(admQuestion);
            this.save();

            return admQuestion.question_id;
        }

        public int update(AdmQuestion admQuestion)
        {
            AdmQuestion currentQuestion = _dbContext.admQuestions.Find(admQuestion.question_id);
            currentQuestion.name_question = admQuestion.name_question;
            currentQuestion.type = admQuestion.type;
            currentQuestion.use_custom_option = admQuestion.use_custom_option;
            currentQuestion.use_for_counter = admQuestion.use_for_counter;
            currentQuestion.typology = admQuestion.typology;
            currentQuestion.status = admQuestion.status;
            currentQuestion.use_custom_option = admQuestion.use_custom_option;
            currentQuestion.orderIndex = admQuestion.orderIndex;

            _dbContext.Entry(currentQuestion).State = EntityState.Modified;
            save();
            return currentQuestion.question_id;
        }


        IEnumerable<AdmQuestionDto> IAdmQuestion.getAll(int estado)
        {
            throw new NotImplementedException();
        }

        public void save()
        {
            _dbContext.SaveChanges();
        }
    }
}