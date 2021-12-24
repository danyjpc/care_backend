using System;
using care_core.dto.AdmSurvey;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.util;

namespace care_core.dto.AdmAnswerDto
{
    public class AdmAnswerDto
    {
        public int answer_id { get; set;}

      
        public AdmSurveyDto survey { get; set;}

       
        public int question_id { get; set; }
        public string question_type { get; set; }
        public string question_name { get; set; }
     
        public string answer { get; set;} = CareConstants.EMPTY_STRING;
        
     
        public AdmUserDto created_by_user { get; set; } = new AdmUserDto() {};

       
        public DateTime date_created { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

        public AdmTypologyDto status { get; set; }
    }
}