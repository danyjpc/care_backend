using System;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.util;

namespace care_core.dto.AdmPerson
{
    public class AdmPersonDto
    {
        public int person_id { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public DateTime? birthday { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

        public long? phone_number { get; set; }

        public long cui { get; set; }


        public AdmTypologyDto cultural_identity { get; set; }

        public AdmTypologyDto state { get; set; }

        public AdmTypologyDto city { get; set; } 

        public AdmTypologyDto occupation { get; set; } 

        public AdmTypologyDto marital_status { get; set; } 

        public AdmTypologyDto education { get; set; } 

        public AdmTypologyDto spoken_language { get; set; }

        public string address_line { get; set; }

        public string email { get; set; }

        public int? daughters_no { get; set; }

        public int? sons_no { get; set; }

        public AdmTypologyDto genre { get; set; } 

        public AdmTypologyDto status { get; set; } 
        
        public DateTime? date_created { get; set; }

        public AdmPersonDto()
        {
        }

        public AdmPersonDto(int personId, string firstName, string lastName, long cui)
        {
            person_id = personId;
            first_name = firstName;
            last_name = lastName;
            this.cui = cui;
        }
    }
}