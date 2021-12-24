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
    public class AdmOrganizationRepository : IAdmOrganization
    {
        private readonly EntityDbContext _dbContext;

        public AdmOrganizationRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Pendiente de definir el organization_type.typology_id a filtrar
        public IEnumerable<Object> getAll()
        {
            var organizations= _dbContext.admOrganizations.
            Where(x => x.status.typology_id == CareConstants.STATUS_ACTIVE
                    && x.organization_type.typology_id !=100)
            .Select(
                organization => new {
                    organization_id = organization.organization_id,
                    name_organization = organization.name_organization,
                    abbreviation = organization.abbreviation,
                    url = organization.url,
                    /*
                    type_organization = new{
                        typology_id=organization.organization_type.typology_id,
                        description = organization.organization_type.description
                    },
                    responsible_organization = organization.organization_responsible,
                    phone_number = organization.phone_number,
                    email = organization.email,
                    state = new {
                        typology_id = organization.state.typology_id,
                        description = organization.state.description
                    },
                    city = new {
                        typology_id = organization.city.typology_id,
                        description = organization.city.description
                    },
                    address = organization.address,
                    date = organization.date,
                    frequency_meeting = new{
                        typology_id = organization.frequency_meeting.typology_id,
                        description = organization.frequency_meeting.description
                    },
                    participation_space = new{
                        typology_id = organization.participation_space.typology_id,
                        description = organization.participation_space.description,
                    },
                    main_problem = organization.main_problem,
                    action_perform = organization.action_perform,*/
                    status = new {
                        typology_id = organization.status.typology_id,
                        description = organization.status.description
                    },    
                    created_by = organization.created_by,
                    date_created =organization.date_created
                }
            ).OrderByDescending(id =>id.organization_id).ToList();

            return organizations;
        }

        public Object getById(long id)
        {
            var organization= _dbContext.admOrganizations.Where(x => x.organization_id==id)
            .Select(
                organization => new {
                    organization_id = organization.organization_id,
                    name_organization = organization.name_organization,
                    abbreviation = organization.abbreviation,
                    url = organization.url,
                    /*
                    type_organization = new{
                        typology_id=organization.organization_type.typology_id,
                        description = organization.organization_type.description
                    },
                    responsible_organization = organization.organization_responsible,
                    phone_number = organization.phone_number,
                    email = organization.email,                   
                    state = new {
                        typology_id = organization.state.typology_id,
                        description = organization.state.description
                    },
                    city = new {
                        typology_id = organization.city.typology_id,
                        description = organization.city.description
                    },
                    address = organization.address,
                    date = organization.date,
                    frequency_meeting = new{
                        typology_id = organization.frequency_meeting.typology_id,
                        description = organization.frequency_meeting.description
                    },
                    participation_space = new{
                        typology_id = organization.participation_space.typology_id,
                        description = organization.participation_space.description,
                    },
                    main_problem = organization.main_problem,
                    action_perform = organization.action_perform,*/
                    status = new {
                        typology_id = organization.status.typology_id,
                        description = organization.status.description
                    },
                    created_by = organization.created_by,
                    date_created =organization.date_created
                }
            ).SingleOrDefault();

            return organization ;
        }

         public long persist(AdmOrganization admOrganization)
        {
            AdmTypology type_organization = _dbContext.admTypologies.Find(CareConstants.EMPTY_TYPOLOGY);
            AdmTypology state = _dbContext.admTypologies.Find(CareConstants.EMPTY_TYPOLOGY);
            AdmTypology city = _dbContext.admTypologies.Find(CareConstants.EMPTY_TYPOLOGY);
            AdmTypology frequency = _dbContext.admTypologies.Find(CareConstants.EMPTY_TYPOLOGY);
            AdmTypology participation_space = _dbContext.admTypologies.Find(CareConstants.EMPTY_TYPOLOGY);
            
            AdmTypology status = _dbContext.admTypologies.Find(admOrganization.status.typology_id);
            
            //AdmUser user = _dbContext.admUsers.Find(admOrganization.created_by_user.user_id);

            admOrganization.organization_type= type_organization;
            admOrganization.state = state;
            admOrganization.city = city;
            admOrganization.frequency_meeting = frequency;
            admOrganization.participation_space = participation_space;
            
            //admOrganization.created_by_user = user;
            admOrganization.status = status;
            admOrganization.date_created = CsnFunctions.now();

            _dbContext.Add(admOrganization);
            save();

            return admOrganization.organization_id;
        }



        public void upd(AdmOrganization admOrganization)
        {
            AdmOrganization updOrganization = _dbContext.admOrganizations.Find(admOrganization.organization_id);

            /*AdmTypology type_organization = _dbContext.admTypologies.Find(admOrganization.organization_type.typology_id);
            AdmTypology state = _dbContext.admTypologies.Find(admOrganization.state.typology_id);
            AdmTypology city = _dbContext.admTypologies.Find(admOrganization.city.typology_id);
            AdmTypology frequency = _dbContext.admTypologies.Find(admOrganization.frequency_meeting.typology_id);
            AdmTypology participation_space = _dbContext.admTypologies.Find(admOrganization.participation_space.typology_id);
            */
            AdmTypology status = _dbContext.admTypologies.Find(admOrganization.status.typology_id);

            updOrganization.name_organization=admOrganization.name_organization;
            updOrganization.abbreviation = admOrganization.abbreviation;
            updOrganization.url = admOrganization.url;

            /*updOrganization.organization_type = type_organization;
            updOrganization.state =state;
            updOrganization.city = city;
            updOrganization.frequency_meeting = frequency;
            updOrganization.participation_space = participation_space;*/
           
            updOrganization.status =status;

            _dbContext.Entry(updOrganization).State = EntityState.Modified;
            save();
        }


        //*************** ORGANIZATION MEMBER **********
        public IEnumerable<Object> getAllMembers(int id)
        {           
            var org_members = _dbContext.admOrganizationMembers
            .Where(x=> x.status.typology_id == CareConstants.STATUS_ACTIVE
                    && x.organization.organization_id == id)
            .Select(
                org_member => new{
                    organization_member_id= org_member.organization_member_id,
                    name_organization_member = org_member.name_organization_member,
                    phone_number = org_member.phone_number,
                    email = org_member.email,
                    organization = new {
                        organization_id= org_member.organization.organization_id,
                        name_organization = org_member.organization.name_organization
                    },
                    status = new {
                        typology_id = org_member.status.typology_id,
                        description = org_member.status.description
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

        public object getByIdMember(int id, int member_id)
        {
            var org_member = _dbContext.admOrganizationMembers
            .Where(x => x.organization.organization_id == id && x.organization_member_id == member_id)
            .Select(
                org_member => new{
                    organization_member_id= org_member.organization_member_id,
                    name_organization_member = org_member.name_organization_member,
                    phone_number = org_member.phone_number,
                    email = org_member.email,
                    organization = new {
                        organization_id= org_member.organization.organization_id,
                        name_organization = org_member.organization.name_organization
                    },
                    status = new {
                        typology_id = org_member.status.typology_id,
                        description = org_member.status.description
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

        public long persistMember(AdmOrganizationMember admOrganizationMember)
        {
            AdmOrganization organization = _dbContext.admOrganizations.Find(admOrganizationMember.organization.organization_id);
            AdmTypology status = _dbContext.admTypologies.Find(admOrganizationMember.status.typology_id);
            AdmUser user = _dbContext.admUsers.Find(admOrganizationMember.created_by_user.user_id);

            admOrganizationMember.organization=organization;
            admOrganizationMember.status = status;
            admOrganizationMember.created_by_user = user;
            admOrganizationMember.date_created = CsnFunctions.now();

            _dbContext.Add(admOrganizationMember);
            save();

            return admOrganizationMember.organization_member_id;
        }
        public void updMember(AdmOrganizationMember admOrganizationMember)
        {
            AdmOrganizationMember updAdmOrgMember = _dbContext.admOrganizationMembers.Find(admOrganizationMember.organization_member_id);

            AdmOrganization organization = _dbContext.admOrganizations.Find(admOrganizationMember.organization.organization_id);
            AdmTypology status = _dbContext.admTypologies.Find(admOrganizationMember.status.typology_id);

            updAdmOrgMember.name_organization_member=admOrganizationMember.name_organization_member;
            updAdmOrgMember.phone_number=admOrganizationMember.phone_number;
            updAdmOrgMember.email=admOrganizationMember.email;
            updAdmOrgMember.organization = organization;
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