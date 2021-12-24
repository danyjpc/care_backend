using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace care_core.model
{
    [Table("adm_game", Schema = "public")]
    public class AdmGame
    {
        [Key, Column("id")] 
        public long id { get; set; }

        [Column("title")]
        public string title { get; set; }

        [Column("phrase")] 
        public string phrase { get; set; }
        
    }
}