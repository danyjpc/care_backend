using System;
using System.Collections.Generic;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.util;

namespace care_core.dto.AdmForm
{
    //Dto used to pivot answers and questions
    //https://dev.azure.com/People-Apps/CARE/_workitems/edit/1608/
    public class AdmReportAnswerDto
    {
        public int surveyId { get; set; }
        public string userName { get; set; }
        public DateTime dateCreated { get; set; }
        public List<AdmQuestionAnswerDto> elementos { get; set; }

        public class AdmQuestionAnswerDto
        {
            public int preguntaId { get; set; }
            public string respuesta { get; set; }
        }
    }
    
}