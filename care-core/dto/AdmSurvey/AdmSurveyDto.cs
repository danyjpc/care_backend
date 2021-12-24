using System;
using System.Collections.Generic;
using care_core.dto.AdmForm;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.util;

namespace care_core.dto.AdmSurvey
{
    public class AdmSurveyDto
    {
        public int survey_id { get; set; }
        public AdmFormDto form { get; set; }
        public AdmTypologyDto status { get; set; }
        public AdmUserDto created_by_user { get; set; }
        public DateTime date_created { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;
        
        public List<AdmAnswerDto.AdmAnswerDto> answers { get; set; }


        public AdmSurveyDto()
        {
        }

        public AdmSurveyDto(int surveyId, AdmFormDto form, AdmTypologyDto status, AdmUserDto createdByUser, DateTime dateCreated)
        {
            survey_id = surveyId;
            this.form = form;
            this.status = status;
            created_by_user = createdByUser;
            date_created = dateCreated;
        }
    }
}