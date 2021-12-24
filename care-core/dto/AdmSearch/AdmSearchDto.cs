using System;
using System.Collections.Generic;

namespace care_core.dto.AdmSearch
{
    public class AdmSearchDto
    {
        public int total_records { get; set; }
        public List<AdmSearchResultDto> records { get; set; }

        public class AdmSearchResultDto
        {
            public int form_id { get; set; }
            public string form_name { get; set; }
            public int survey_id { get; set; }
            public int question_id { get; set; }
            public string question { get; set; }
            public int answer_id { get; set; }
            public string answer { get; set; }
            public int module_id { get; set; }
            public string module_name { get; set; }
            public int category_id { get; set; }
            public string category_name { get; set; }
            public DateTime date_created { get; set; }
        }
    }
}