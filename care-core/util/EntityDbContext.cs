using care_core.model;
using Microsoft.EntityFrameworkCore;

namespace care_core.util
{
    public class EntityDbContext  : DbContext
    {
        public EntityDbContext(DbContextOptions<EntityDbContext> options) : base(options)
        {
        }
        
        public DbSet<AdmGame> admgames { get; set; }
        public DbSet<AdmUser> admUsers { get; set; }
        public DbSet<AdmTypology> admTypologies { get; set; }
        public DbSet<AdmPerson> admPersons { get; set; }
        public DbSet<AdmOrganization> admOrganizations { get; set; }
        public DbSet<AdmOrganizationMember> admOrganizationMembers { get; set;}
        public DbSet<AdmCase> admCases{ get; set; }
        public DbSet<AdmProject> admProjects { get; set; }
        public DbSet<AdmProjectActivity> admProjectActivities { get; set; }
        public DbSet<AdmCaseTracing> admCaseTracings { get; set; }

        public DbSet<AdmForm> admForms { get; set; }
        public DbSet<AdmQuestion> admQuestions { get; set; }
        public DbSet<AdmModule> admModules { get; set; }
        public DbSet<AdmCategory> admCategories { get; set; }
        public DbSet<AdmModuleCategory> admModuleCategories { get; set; }
        public DbSet<AdmAnswer> admAnswers { get; set; }
        public DbSet<AdmOption> admOptions { get; set; }
        public DbSet<AdmGroup> admGroups { get; set; }
        public DbSet<AdmPermission> admPermissions { get; set; }
        public DbSet<AdmUserPermission> admUserPermissions { get; set; }
        public DbSet<AdmSurvey> admSurveys { get; set; }
        
        public DbSet<AdmGeneralConfig> admGeneralConfigs { get; set; }
    }
}