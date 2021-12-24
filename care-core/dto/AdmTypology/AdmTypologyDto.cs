using System;

namespace care_core.dto.AdmTypology
{
    public class AdmTypologyDto
    {
        public int? typology_id { get; set; }
        
        public long? parent_typology { get; set; }

        public long? internal_id { get; set; }
        
        public string description { get; set; }

        public string value_1 { get; set; }

        public string value_2 { get; set; }
        
        public bool? is_editable { get; set; }

        public AdmTypologyDto()
        {
        }

        public AdmTypologyDto(int typologyId, string description)
        {
            typology_id = typologyId;
            this.description = description;
        }
    }
    
    
}