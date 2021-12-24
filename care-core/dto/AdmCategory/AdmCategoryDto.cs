using System;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.util;

namespace care_core.dto.AdmForm
{
    public class AdmCategoryDto
    {
        
        public int category_id { get; set;}
        public string name_category { get; set;}
        public string icon { get; set; }
        public string color { get; set; }
        public AdmTypologyDto status { get; set; } = new AdmTypologyDto(){};
        public AdmUserDto created_by_user { get; set; } = new AdmUserDto() {};
        public DateTime date_create { get; set; }

        public AdmCategoryDto()
        {
        }

        public AdmCategoryDto(int categoryId, string nameCategory, string icon, string color, DateTime dateCreate)
        {
            category_id = categoryId;
            name_category = nameCategory;
            this.icon = icon;
            this.color = color;
            date_create = dateCreate;
        }
    }
}