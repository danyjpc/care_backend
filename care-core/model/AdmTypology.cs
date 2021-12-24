using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_typology" , Schema="public")]
    public class AdmTypology
    {
        [Key,Column("typology_id")]
        [Required]
        public int typology_id { get; set; }

        [Column("parent_typology_id") , ForeignKey("parent_typology_id") ]
        public virtual AdmTypology  parent_typology { get; set; } 

        [Column("internal_id")]
        public long internal_id { get; set; } = CareConstants.ZERO_DEFAULT;
        
        [Column("description")]
        public string description { get; set; } = CareConstants.EMPTY_STRING;

        [Column("value_1")]
        public string value_1 { get; set; } = CareConstants.EMPTY_STRING;

        [Column("value_2")]
        public string value_2 { get; set; } = CareConstants.EMPTY_STRING;

        [Column("is_editable")]  
        public bool is_editable { get; set; } = CareConstants.FALSE;

        [Column("show_survey")]  
        public bool show_survey { get; set; } = CareConstants.FALSE;

        public virtual ICollection<AdmTypology> childs { get; set; }

        

        public override string ToString()
        {
            return $"{nameof(typology_id)}: {typology_id}, {nameof(parent_typology)}: {parent_typology}, {nameof(internal_id)}: {internal_id}, {nameof(description)}: {description}, {nameof(value_1)}: {value_1}, {nameof(value_2)}: {value_2}, {nameof(is_editable)}: {is_editable}, {nameof(show_survey)}: {show_survey}, {nameof(childs)}: {childs}";
        }
    }
}
