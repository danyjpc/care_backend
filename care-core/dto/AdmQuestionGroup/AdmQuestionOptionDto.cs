
using System;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.util;

namespace care_core.dto.AdmQuestionGroup
{
    public class AdmQuestionOptionDto
    {
        public int option_id { get; set;}
        public string value { get; set;}
        public int? status_id { get; set; }
        
        public AdmQuestionDto question = new AdmQuestionDto() { };
        
        public AdmTypologyDto status = new AdmTypologyDto() { };

        public AdmUserDto created_by_user = new AdmUserDto() { };

        public AdmQuestionOptionDto(){

        }
        public AdmQuestionOptionDto(int questionId, string value){
            this.option_id = questionId;
            this.value = value;
        }
    }
}