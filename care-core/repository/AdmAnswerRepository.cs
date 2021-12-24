using System;
using System.Collections.Generic;
using System.Linq;
using care_core.dto.AdmAnswerDto;
using care_core.dto.AdmCase;
using care_core.dto.AdmForm;
using care_core.dto.AdmOrganization;
using care_core.dto.AdmOrganizationMember;
using care_core.dto.AdmPerson;
using care_core.dto.AdmQuestionGroup;
using care_core.dto.AdmSurvey;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace care_core.repository
{
    public class AdmAnswerRepository : IAdmAnswer
    {
        private readonly EntityDbContext _dbContext;

        public AdmAnswerRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        

        public IEnumerable<AdmAnswerDto> getAllByForm(int form_id)
        {
            IEnumerable<AdmAnswerDto> respuestas = _dbContext.admAnswers.Where(x => x.survey.form.form_id == form_id
            && x.status.typology_id == CareConstants.ESTADO_ACTIVO).Select(
                pregunta => new AdmAnswerDto
                {
                    answer = pregunta.answer,
                    answer_id = pregunta.answer_id,
                    date_created = pregunta.date_created,
                    question_id = pregunta.question.question_id,
                    survey = new AdmSurveyDto()
                    {
                        survey_id = pregunta.survey.survey_id,
                        created_by_user = null,
                        status = null,
                        form = new AdmFormDto()
                        {
                            form_id = pregunta.survey.form.form_id,
                            name_form = pregunta.survey.form.name_form,
                            category = null,
                            created_by_user = null,
                            module = null,
                            status = null
                        },
                    },
                    
                    created_by_user = new AdmUserDto()
                    {
                        user_id = pregunta.created_by_user.user_id,
                        email = pregunta.created_by_user.person.email
                    }
                }
            ).OrderBy(x=>x.date_created).ToList();
            

            return respuestas;
        }

        public AdmAnswerDto getAnswerById(int answerId)
        {
            throw new NotImplementedException();
        }

        public int persist(AdmAnswer admQuestion)
        {
            throw new NotImplementedException();
        }

        public int update(AdmAnswer admAnswer)
        {
            AdmAnswer currentAnswer = _dbContext.admAnswers.Find(admAnswer.answer_id);
            currentAnswer.status = currentAnswer.status;
            
            _dbContext.Entry(currentAnswer).State = EntityState.Modified;
            save();
            return currentAnswer.answer_id;
        }

        public IEnumerable<AdmAnswerDto> getAll()
        {
            IEnumerable<AdmAnswerDto> respuestas = _dbContext.admAnswers
                .Where(x=>x.status.typology_id == CareConstants.ESTADO_ACTIVO)
                .Select(
                pregunta => new AdmAnswerDto
                {
                    answer = pregunta.answer,
                    answer_id = pregunta.answer_id,
                    date_created = pregunta.date_created,
                    question_id = pregunta.question.question_id,
                    survey = new AdmSurveyDto()
                    {
                        survey_id = pregunta.survey.survey_id,
                        created_by_user = null,
                        status = null,
                        form = new AdmFormDto()
                        {
                            form_id = pregunta.survey.form.form_id,
                            name_form = pregunta.survey.form.name_form,
                            category = null,
                            created_by_user = null,
                            module = null,
                            status = null
                        },
                    },
                    
                    created_by_user = new AdmUserDto()
                    {
                        user_id = pregunta.created_by_user.user_id,
                        email = pregunta.created_by_user.person.email,
                        date_create = null
                    }
                }
            ).OrderByDescending(x=>x.answer_id).ToList();

            return respuestas;
        }

        //method used for search endpoint, same as getAll but returns form complete properties
        public IEnumerable<AdmAnswerDto> getAllCompleteFormInfo()
        {
            IEnumerable<AdmAnswerDto> respuestas = _dbContext.admAnswers
                .Where(x=>x.status.typology_id == CareConstants.ESTADO_ACTIVO)
                .Select(
                respuesta => new AdmAnswerDto
                {
                    answer = respuesta.answer,
                    answer_id = respuesta.answer_id,
                    date_created = respuesta.date_created,
                    question_id = respuesta.question.question_id,
                    question_name = respuesta.question.name_question,
                    survey = new AdmSurveyDto()
                    {
                        survey_id = respuesta.survey.survey_id,
                        created_by_user = null,
                        status = null,
                        form = new AdmFormDto()
                        {
                            form_id = respuesta.survey.form.form_id,
                            name_form = respuesta.survey.form.name_form,
                            category = new AdmCategDto()
                            {
                                category_id = respuesta.survey.form.module_category.category.category_id,
                                name_category = respuesta.survey.form.module_category.category.name_category
                            },
                            created_by_user = null,
                            module = new AdmModulDto()
                            {
                                module_id = respuesta.survey.form.module_category.module.module_id,
                                name_module = respuesta.survey.form.module_category.module.name_module
                            },
                            status = null
                        },
                    },
                    
                    created_by_user = new AdmUserDto()
                    {
                        user_id = respuesta.created_by_user.user_id,
                        email = respuesta.created_by_user.person.email,
                        date_create = null
                    }
                }
            ).OrderByDescending(x=>x.answer_id).ToList();

            return respuestas;
        }

        public IEnumerable<AdmAnswer> getAnswersByQuestion(int question_id)
        {
            IEnumerable<AdmAnswer> respuestas = _dbContext.admAnswers
                .Where(x=>x.question.question_id == question_id)
                .Select(
                    respuesta => new AdmAnswer
                    {
                        answer = respuesta.answer,
                        answer_id = respuesta.answer_id,
                        date_created = respuesta.date_created,
                        question = new AdmQuestion()
                        {
                            question_id = question_id
                        },
                        survey = new AdmSurvey()
                        {
                            survey_id = respuesta.survey.survey_id,
                            form = new AdmForm()
                            {
                                form_id = respuesta.survey.form.form_id,
                            },
                        },
                    
                        created_by_user = new AdmUser()
                        {
                            user_id = respuesta.created_by_user.user_id
                        }
                    }
                ).OrderByDescending(x=>x.answer_id).ToList();

            return respuestas;
        }

        public void save()
        {
            _dbContext.SaveChanges();
        }
    }
}