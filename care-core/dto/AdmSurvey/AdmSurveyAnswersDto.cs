
using System;
using System.Collections.Generic;
using care_core.dto.AdmForm;


namespace care_core.dto.AdmSurvey
{
    public class AdmSurveyAnswersDto
    {
        public int form_id { get; set; }
        public string name_form { get; set; }
        public List<string> question_labels { get; set; }
        public List<List<string>> answers { get; set; }

    }

    
}