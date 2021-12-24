using System;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.util;

namespace care_core.dto.AdmForm
{
    public class AdmModuleCategoryDto
    {
        
        public int module_category_id { get; set;}
        public AdmModuleDto module { get; set; } = new AdmModuleDto(){};
        public AdmCategoryDto category { get; set; } = new AdmCategoryDto() {};
        public DateTime date_create { get; set; }

        public AdmModuleCategoryDto()
        {
        }
    }
}