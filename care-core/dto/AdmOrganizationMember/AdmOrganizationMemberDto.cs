namespace care_core.dto.AdmOrganizationMember
{
    public class AdmOrganizationMemberDto
    {
        public int organization_member_id { get; set; }
        public string name_organization_member { get; set; }


        public AdmOrganizationMemberDto()
        {
        }
        
        public AdmOrganizationMemberDto(int organizationMemberId, string nameOrganizationMember)
        {
            organization_member_id = organizationMemberId;
            name_organization_member = nameOrganizationMember;
        }
    }
}