using System;
using System.Collections.Generic;
using System.Linq;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.EntityFrameworkCore;

using Serilog;

namespace care_core.repository
{
    public class AdmOrganizationMRepository : IAdmOrganizationMember
    {
        private readonly EntityDbContext _dbContext;

        public AdmOrganizationMRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Object> getAll()
        {           
            var org_members = _dbContext.admOrganizationMembers
            .Where(x=> x.status.typology_id == CareConstants.STATUS_ACTIVE)
            .Select(
                org_member => new{
                    organization_member_id= org_member.organization_member_id,
                    name_organization_member = org_member.name_organization_member,
                    phone_number = org_member.phone_number,
                    email = org_member.email,
                    organization = new {
                        organization_id= org_member.organization.organization_id
                    },
                    status = new {
                        typology_id = org_member.status.typology_id
                    },
                    created_by = new {
                        user_id = org_member.created_by_user.user_id,
                        persona = new {
                            first_name = org_member.created_by_user.person.first_name,
                            email = org_member.created_by_user.person.email
                        }
                    },
                    date_created =org_member.date_created
                }
            )
            .OrderByDescending( id => id.organization_member_id).ToList();
            return org_members;
        }

        public object getById(long id)
        {
            var org_member = _dbContext.admOrganizationMembers
            .Where(x => x.organization_member_id ==id)
            .Select(
                org_member => new{
                    organization_member_id= org_member.organization_member_id,
                    name_organization_member = org_member.name_organization_member,
                    phone_number = org_member.phone_number,
                    email = org_member.email,
                    organization = new {
                        organization_id= org_member.organization.organization_id
                    },
                    status = new {
                        typology_id = org_member.status.typology_id
                    },
                    created_by = new {
                        user_id = org_member.created_by_user.user_id,
                        persona = new {
                            first_name = org_member.created_by_user.person.first_name,
                            email = org_member.created_by_user.person.email
                        }
                    },
                    date_created =org_member.date_created
                }
            )
            .SingleOrDefault();
            return org_member;
        }

        public long persist(AdmOrganizationMember admOrganizationMember)
        {
            AdmOrganization organization = _dbContext.admOrganizations.Find(admOrganizationMember.organization.organization_id);
            AdmTypology status = _dbContext.admTypologies.Find(admOrganizationMember.status.typology_id);

            admOrganizationMember.organization=organization;
            admOrganizationMember.status = status;
            admOrganizationMember.date_created = CsnFunctions.now();

            _dbContext.Add(admOrganizationMember);
            save();

            return admOrganizationMember.organization_member_id;
        }
        public void upd(AdmOrganizationMember admOrganizationMember)
        {
            AdmOrganizationMember updAdmOrgMember = _dbContext.admOrganizationMembers.Find(admOrganizationMember.organization_member_id);

            AdmOrganization organization = _dbContext.admOrganizations.Find(admOrganizationMember.organization.organization_id);
            AdmTypology status = _dbContext.admTypologies.Find(admOrganizationMember.status.typology_id);

            updAdmOrgMember.name_organization_member=admOrganizationMember.name_organization_member;
            updAdmOrgMember.phone_number=admOrganizationMember.phone_number;
            updAdmOrgMember.email=admOrganizationMember.email;
            updAdmOrgMember.organization =organization;
            updAdmOrgMember.status = status;

            _dbContext.Entry(updAdmOrgMember).State = EntityState.Modified;
            save();
        }

        public void save()
        {
            _dbContext.SaveChanges();
        }

        
    }
}