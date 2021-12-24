using System;
using System.Collections.Generic;
using care_core.dto.AdmCase;
using care_core.dto.AdmForm;
using care_core.dto.AdmQuestionGroup;
using care_core.dto.AdmSurvey;
using care_core.model;

namespace care_core.repository.interfaces
{
    public interface IAdmSurvey
    {
        IEnumerable<AdmSurveyDto> getAll(int estado, bool viewAnswers);
        IEnumerable<AdmSurveyDto> getAllByForm(int form_id, bool viewAnswers,bool onlyCounters);
        AdmSurveyDto getSurveyById(int surveyId);
        int persist(AdmSurvey admSurvey);
        int update(AdmSurvey admSurvey);
        void save();

        public IEnumerable<AdmSurveyDto> getSurveyCounterByForm(int formId);
        public IEnumerable<object> getAllSurveysUserInfo(int estado, int form_id);
        

    }
}