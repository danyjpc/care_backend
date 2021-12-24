using System;

namespace care_core.util
{
    public class CareConstants
    {
        public static string TERMINAL_MODE { get; set; } = Environment.GetEnvironmentVariable("TERMINAL_MODE");

        public static string CONNECTION_STRING = "Host=" + Environment.GetEnvironmentVariable("SERVER_DB")
                                                         + ";Port=" +
                                                         Environment.GetEnvironmentVariable("SERVER_DB_PORT")
                                                         + ";User ID=" +
                                                         Environment.GetEnvironmentVariable("SERVER_DB_USER")
                                                         + ";Password=" +
                                                         Environment.GetEnvironmentVariable("SERVER_DB_PASS")
                                                         + ";Database=" +
                                                         Environment.GetEnvironmentVariable("SERVER_DB_NAME")+  ";Include Error Detail = true";


        //Default value constants
        
        public static long USER_ADMIN = 1;
        public static long UTC_CONFIG = -6;
        public static long STATUS_ACTIVE = 160445;
        public static int ESTADO_ACTIVO = 160445;
        public static int ESTADO_INACTIVO = 160447;
        public static string DEV_MAIL = "devs@mypeopleapps.com";
        public static int TOKEN_DAYS { get; set; } = 1;

        public const int GLOBAL_TYPOLOGY = 100;
        public const int PARENT_TREATMENT = 100;
        public const int EMPTY_TYPOLOGY = 160000;
        public const long DEFAULT_STATUS = 160445;
        public const long DEFAULT_TREATMENT_ENDED = 160605;
        public const int ZERO_DEFAULT = 0;
        public const decimal ZERO_DECIMAL_DEFAULT = (decimal) 0.00;
        public const string EMPTY_STRING = "";
        public const string HEX_COLOR_WHITE = "#ffffff";
        public const string NO_DESCRIPTION = "S/D";
        public const string DEFAULT_PERCENTAGE_STRING = "%";
        public const bool FALSE = false;
        public const bool TRUE = true;
        public static readonly DateTime DATE_TIME_NO_TIMEZONE = new DateTime(1900, 01, 01, 00, 00, 00);
        public const double COORDINATES_DECIMAL_DEFAULT = 999.99999999999999999999;
        public const string DEFAULT_UNDERSCORE = "_";
        public const string DEFAULT_AT = "@";
        public const int TYPOLOGY_IMAGE = 170039;
        public const int TYPOLOGY_PDF = 170038;
        public const int ONE_DEFAULT = 1;

        //Id for default category id on table adm_module_category
        public const int DEFAULT_CATEGORY_ID = 1;
        
        public const int DEFAULT_STATE_ID = 160060;
        public const int DEFAULT_CITY_ID = 160061;

        public const string ALL_ROLES_ALLOWED = "ADMINISTRADOR, OPERADOR";

        public const string OPERADOR_ROLE = "OPERADOR";
        public const string ADMINISTRADOR_ROLE = "ADMINISTRADOR";
        

        //Variables para enviar correo Mailkit - SMTP 
        public static string SMTP_NAME_FROM = Environment.GetEnvironmentVariable("SMTP_NAME_FROM");
        public static string SMTP_EMAIL_FROM = Environment.GetEnvironmentVariable("SMTP_NAME_FROM");
        public static string SMTP_HOST = Environment.GetEnvironmentVariable("SMTP_HOST");
        public static int SMTP_PORT = Int32.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
        public static string SMTP_USER = Environment.GetEnvironmentVariable("SMTP_USER");
        public static string SMTP_PASS = Environment.GetEnvironmentVariable("SMTP_PASS");
        
        //Variable for public (guest) user email
        public static string GUEST_USER_EMAIL = "guest@gmail.com";
    }
}