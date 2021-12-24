using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using System;

namespace care_core.dto.AdmForm
{
    public class AdmFormDto
    {
        public int form_id { get; set; }
        public string name_form { get; set; }
        public bool is_public { get; set; }
        public int ? nrecorded { get; set; }
        public string ? lastrecorded { get; set; }
        public AdmModulDto module { get; set; } = new AdmModulDto () {};
        public AdmCategDto category { get; set; } = new AdmCategDto(){}; 
        public AdmTypologyDto status { get; set; } = new AdmTypologyDto() {};
        public AdmUserDto created_by_user { get; set; } = new AdmUserDto() {};
        public DateTime ? date_created { get; set; }


        public AdmFormDto()
        {
        }

        public AdmFormDto(int formId, string name_form, AdmModulDto module, AdmCategDto category, AdmTypologyDto status, AdmUserDto created_by_user)
        {
            this.form_id = form_id;
            this.name_form = name_form;
            this.module = module;
            this.category = category;
            this.status = status;
            this.created_by_user = created_by_user;
        }
    }
}