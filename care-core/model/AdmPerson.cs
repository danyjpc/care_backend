using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using care_core.util;

namespace care_core.model
{
    [Table("adm_person", Schema = "public")]
    public class AdmPerson
    {
        [Required]
        [Key, Column("person_id")]
        public int person_id { get; set; }

        [Column("first_name")]
        public string first_name { get; set; } = CareConstants.EMPTY_STRING;

        [Column("last_name")]
        public string last_name { get; set; } = CareConstants.EMPTY_STRING; 

        [Column("birthday")]
        public DateTime birthday { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

        [Column("phone_number")]
        public long phone_number { get; set; }= CareConstants.ZERO_DEFAULT;
        
        [Column("cui")]
        public long cui { get; set; }= CareConstants.ZERO_DEFAULT;

        [Column("cultural_identity_id"), ForeignKey("cultural_identity_id")]
        public AdmTypology cultural_identity { get; set; } = new AdmTypology() { };

        [Column("state_id"), ForeignKey("state_id")]
        public AdmTypology state { get; set; } = new AdmTypology() { };

        [Column("city_id"), ForeignKey("city_id")]
        public AdmTypology city { get; set; } = new AdmTypology() { };

        [Column("occupation_id"), ForeignKey("occupation_id")]
        public AdmTypology occupation { get; set; } = new AdmTypology() { };

        [Column("marital_status_id"), ForeignKey("marital_status_id")]
        public AdmTypology marital_status { get; set; } = new AdmTypology() { };

        [Column("education_id"), ForeignKey("education_id")]
        public AdmTypology education { get; set; } = new AdmTypology() { };

        [Column("spoken_language_id"), ForeignKey("spoken_language_id")]
        public AdmTypology spoken_language { get; set; } = new AdmTypology() { };

        [Column("address_line")]
        public string address_line { get; set; } = CareConstants.EMPTY_STRING;
        
        [Column("email")]
        public string email { get; set; } = CareConstants.EMPTY_STRING;
        
        [Column("daughters_no")]
        public int daughters_no { get; set;} = CareConstants.ZERO_DEFAULT;
        
        [Column("sons_no")]
        public int sons_no { get; set; } = CareConstants.ZERO_DEFAULT;

        [Column("genre_id"), ForeignKey("genre_id")]
        public AdmTypology genre { get; set; } = new AdmTypology();

        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology();
        
        [Column("date_created")]
        public DateTime date_created { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;
        
        
    }
}
