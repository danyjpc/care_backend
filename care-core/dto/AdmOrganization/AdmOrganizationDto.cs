namespace care_core.dto.AdmOrganization
{
    public class AdmOrganizationDto
    {
        public int organization_id { get; set; }
        public string name_organization { get; set; }


        public AdmOrganizationDto()
        {
        }

        public AdmOrganizationDto(int organizationId, string nameOrganization)
        {
            organization_id = organizationId;
            name_organization = nameOrganization;
        }
    }
}