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
    public class AdmSurveyRepository : IAdmSurvey
    {
        private readonly EntityDbContext _dbContext;

        public AdmSurveyRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IEnumerable<AdmSurveyDto> getAll(int estado, bool viewAnswers)
        {
            IEnumerable<AdmSurveyDto> boletas = _dbContext.admSurveys
                .Where(x => x.status.typology_id == (estado > 0 ? estado : CareConstants.ESTADO_ACTIVO))
                .Select(
                    boleta => new AdmSurveyDto
                    {
                        survey_id = boleta.survey_id,
                        form = new AdmFormDto()
                        {
                            form_id = boleta.form.form_id,
                            name_form = boleta.form.name_form,
                            category = null,
                            created_by_user = null,
                            module = null,
                            status = null
                        },
                        status = new AdmTypologyDto()
                        {
                            typology_id = boleta.status.typology_id,
                            description = boleta.status.description
                        },
                        created_by_user = new AdmUserDto()
                        {
                            user_id = boleta.created_by_user.user_id,
                            email = boleta.created_by_user.person.email
                        }
                    }
                ).OrderByDescending(x => x.survey_id).ToList();

            //adds answers if true
            addAnswers(viewAnswers, boletas);


            return boletas;
        }

        //viewAnswers is to append answers, onlyCounters is for showing stats
        public IEnumerable<AdmSurveyDto> getAllByForm(int form_id, bool viewAnswers, bool onlyCounters)
        {
            IEnumerable<AdmSurveyDto> boletas = _dbContext.admSurveys
                .Where(x => x.form.form_id == form_id && x.status.typology_id.Equals(CareConstants.ESTADO_ACTIVO))
                .Select(
                    boleta => new AdmSurveyDto()
                    {
                        survey_id = boleta.survey_id,
                        date_created = boleta.date_create,
                        form = new AdmFormDto()
                        {
                            form_id = boleta.form.form_id,
                            name_form = boleta.form.name_form,
                            category = null,
                            created_by_user = null,
                            module = new AdmModulDto()
                            {
                                module_id = boleta.form.module_category.module.module_id,
                                name_module = boleta.form.module_category.module.name_module
                            },
                            status = null
                        },
                        status = new AdmTypologyDto()
                        {
                            typology_id = boleta.status.typology_id,
                            description = boleta.status.description
                        },
                        created_by_user = new AdmUserDto()
                        {
                            user_id = boleta.created_by_user.user_id,
                            email = boleta.created_by_user.person.email
                        },
                        answers = null
                    }
                ).OrderBy(x => x.survey_id).ToList();

            //adds answers if true
            if (onlyCounters)
            {
                addAnswersOnlyCounters(viewAnswers, boletas, onlyCounters);
            }
            else
            {
                addAnswers(viewAnswers, boletas);
            }


            return boletas;
        }

        //onlyCounters is used for showing statistics (stats)
        private void addAnswers(bool viewAnswers, IEnumerable<AdmSurveyDto> boletas)
        {
            if (viewAnswers)
            {
                foreach (AdmSurveyDto boleta in boletas)
                {
                    boleta.answers = new List<AdmAnswerDto>();

                    boleta.answers = _dbContext.admAnswers.Where(x => x.survey.survey_id == boleta.survey_id 
                                                                      && x.status.typology_id == CareConstants.ESTADO_ACTIVO)
                        .Select(
                            respuesta => new AdmAnswerDto
                            {
                                answer = respuesta.answer,
                                answer_id = respuesta.answer_id,
                                date_created = respuesta.date_created,
                                question_id = respuesta.question.question_id,
                                question_type = respuesta.question.type,

                                created_by_user = new AdmUserDto()
                                {
                                    user_id = respuesta.created_by_user.user_id,
                                    email = respuesta.created_by_user.person.email
                                }
                            }
                        ).OrderBy(x => x.question_id).ToList();

                    foreach (var respuesta in boleta.answers)
                    {
                        respuesta.answer = getAnswerDescription(respuesta);
                    }
                }
            }
        }

        //method that returns typology description if question type is state or city
        private string getAnswerDescription(AdmAnswerDto respuesta)
        {
            //by default we set result to current answer
            string resultado = respuesta.answer;
            try
            {
                if (respuesta.question_type.Equals("state") || respuesta.question_type.Equals("city"))
                {
                    //check is value is a valid number
                    int typologyId;
                    bool isValidNumber = Int32.TryParse(respuesta.answer, out typologyId);
                    if (isValidNumber)
                    {
                        var typology = _dbContext.admTypologies.Find(typologyId);
                        //if typology exists, get description
                        if (typology != null)
                        {
                            resultado = typology.description;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //empty catch
                Log.Error(ex.Message);
            }

            return resultado;
        }

        //onlyCounters is used for showing statistics (stats)
        private void addAnswersOnlyCounters(bool viewAnswers, IEnumerable<AdmSurveyDto> boletas, bool onlyCounters)
        {
            if (viewAnswers)
            {
                foreach (AdmSurveyDto boleta in boletas)
                {
                    boleta.answers = new List<AdmAnswerDto>();

                    boleta.answers = _dbContext.admAnswers.Where(x => x.survey.survey_id == boleta.survey_id
                                                                      && x.question.use_for_counter == onlyCounters
                                                                      && x.status.typology_id ==
                                                                      CareConstants.ESTADO_ACTIVO)
                        .Select(
                            respuesta => new AdmAnswerDto
                            {
                                answer = respuesta.answer,
                                answer_id = respuesta.answer_id,
                                date_created = respuesta.date_created,
                                question_id = respuesta.question.question_id,
                                question_type = respuesta.question.type,
                                question_name = respuesta.question.name_question,

                                created_by_user = new AdmUserDto()
                                {
                                    user_id = respuesta.created_by_user.user_id,
                                    email = respuesta.created_by_user.person.email
                                }
                            }
                        ).OrderBy(x => x.question_id).ToList();
                }
            }
        }

        public IEnumerable<object> getAllSurveysUserInfo(int estado, int form_id)
        {
            IEnumerable<Object> boletas = _dbContext.admSurveys
                .Where(x => x.status.typology_id == (estado > 0 ? estado : CareConstants.ESTADO_ACTIVO)
                            && x.form.form_id == form_id)
                .Select(
                    boleta => new
                    {
                        survey_id = boleta.survey_id,
                        form = new AdmFormDto()
                        {
                            form_id = boleta.form.form_id,
                            name_form = boleta.form.name_form,
                            category = null,
                            created_by_user = null,
                            module = null,
                            status = null
                        },
                        status = new AdmTypologyDto()
                        {
                            typology_id = boleta.status.typology_id,
                            description = boleta.status.description
                        },
                        created_by_user = new
                        {
                            user_id = boleta.created_by_user.user_id,
                            email = boleta.created_by_user.person.email,

                            name = boleta.created_by_user.person.first_name + ' ' +
                                   boleta.created_by_user.person.last_name,
                            departamento = boleta.created_by_user.person.state.description,
                            municipio = boleta.created_by_user.person.city.description,
                        },
                        date_created = boleta.date_create
                    }
                ).OrderByDescending(x => x.survey_id).ToList();

            return boletas;
        }

        public IEnumerable<AdmSurveyDto> getSurveyCounterByForm(int formId)
        {
            IEnumerable<AdmSurveyDto> surveysDto = getAllByForm(formId, true, true);

            return surveysDto;
        }

        public AdmSurveyDto getSurveyById(int surveyId)
        {
            throw new NotImplementedException();
        }

        public int persist(AdmSurvey admSurvey)
        {
            try
            {
                _dbContext.Add(admSurvey);
            }
            catch (Exception ex)
            {
                Log.Error("Error: " + ex.Message);
            }

            save();

            return admSurvey.survey_id;
        }

        public int update(AdmSurvey admSurvey)
        {
            AdmSurvey currentSurvey = _dbContext.admSurveys.Find(admSurvey.survey_id);
            currentSurvey.status = admSurvey.status;

            _dbContext.Entry(currentSurvey).State = EntityState.Modified;
            save();

            return currentSurvey.survey_id;
        }

        public AdmAnswerDto getAnswerById(int answerId)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<AdmAnswerDto> getAll(int estado)
        {
            throw new NotImplementedException();
        }


        public void save()
        {
            _dbContext.SaveChanges();
        }
    }
}