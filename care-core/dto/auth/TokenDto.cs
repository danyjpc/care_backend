namespace care_core.dto.auth
{
    public class TokenDto
    {
        //public long usuario_id { get; set; }

        public string code { get; set; }

        public string msg { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public long expire { get; set; }

        public string token { get; set; }
    }
}