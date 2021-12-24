using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using care_core.util;

namespace care_core.model
{
    [Table("adm_permission", Schema = "public")]
    public class AdmPermission
    {
        [Required]
        [Key, Column("permission_id")]
        public int permission_id { get; set; }

        [Column("name_permission")]
        public string name_permission { get; set; } = CareConstants.EMPTY_STRING;

        [Column("alias")]
        public string alias { get; set; } = CareConstants.EMPTY_STRING;

        [Column("status_id"), ForeignKey("status_id")]
        public AdmTypology status { get; set; } = new AdmTypology(){};

        [Column("created_by"), ForeignKey("created_by")]
        public AdmUser created_by_user { get; set; } = new AdmUser() {};

        [Column("date_created")]
        public DateTime date_created { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

    }
}