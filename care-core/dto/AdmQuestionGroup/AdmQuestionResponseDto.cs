
using care_core.dto.AdmQuestionGroup;
using care_core.util;

namespace care_core.dto.AdmQuestionGroup
{
    public class AdmQuestionResponseDto
    {
        //public int group_id { get; set; }
        public string name_group { get; set; }
        public AdmQuestDto[] questions { get; set; }

        public AdmQuestionResponseDto(){

        }
        /*public AdmQuestionResponseDto(int group_id, string name_group, AdmQuestionDto [] questions){
            this.group_id = group_id;
            this.name_group = name_group;
            this.questions = questions;
        }*/
        public AdmQuestionResponseDto( string name_group, AdmQuestDto [] questions){
            this.name_group = name_group;
            this.questions = questions;
        }
    }
}