using System.Collections.Generic;

namespace care_core.dto.AdmSurvey
{
    public class AdmGeneralStatsDto
    {
       //dictionaries used for json response
       public Dictionary<string, int> modules { get; set; }
       public Dictionary<string, int> forms { get; set; }
       public Dictionary<string, int> dates { get; set; }
       public Dictionary<string, int> states { get; set; }
       public Dictionary<string, int> user_surveys { get; set; }
       
    }
}