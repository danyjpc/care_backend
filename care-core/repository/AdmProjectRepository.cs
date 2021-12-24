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
    public class AdmProjectRepository : IAdmProject
    {
        private readonly EntityDbContext _dbContext;

        public AdmProjectRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Object> getAll()
        {
            var projects = _dbContext.admProjects.Where(x => x.status.typology_id == CareConstants.DEFAULT_STATUS)
            .Select(
                project => new {
                    project_id = project.project_id,
                    name_project = project.name_project,
                    date = project.date,
                    organization = new {
                        organization_id =project.organization.organization_id,
                        name_organization = project.organization.name_organization
                    },
                    status = new {
                        typology_id = project.status.typology_id,
                        description = project.status.description
                    },
                    created_by_user = new{
                        user_id = project.created_by_user.user_id,
                        person = new {
                            person_id =project.created_by_user.person.person_id,
                            email = project.created_by_user.person.email,
                        }
                    },
                    date_created = project.date_create
                } 
            ).OrderByDescending(id => id.project_id).ToList();

            return projects;
        }

        public Object getById(long id)
        {
            var project = _dbContext.admProjects.Where(x => x.project_id == id)
            .Select(
                project => new{
                    project_id = project.project_id,
                    name_project = project.name_project,
                    date = project.date,
                    organization = new {
                        organization_id =project.organization.organization_id,
                        name_organization = project.organization.name_organization
                    },
                    status = new {
                        typology_id = project.status.typology_id,
                        description = project.status.description
                    },
                    created_by_user = new{
                        user_id = project.created_by_user.user_id,
                        person = new {
                            person_id =project.created_by_user.person.person_id,
                            email = project.created_by_user.person.email,
                        }
                    },
                    date_created = project.date_create
                } 
            ).SingleOrDefault();

            return project;
        }

        public long persist(AdmProject admProject)
        {
            AdmOrganization organization = _dbContext.admOrganizations.Find(admProject.organization.organization_id);
            AdmTypology status = _dbContext.admTypologies.Find(admProject.status.typology_id);
            AdmUser user = _dbContext.admUsers.Find(admProject.created_by_user.user_id);

            admProject.organization = organization;
            admProject.status = status;
            admProject.created_by_user =user;
            admProject.date_create = CsnFunctions.now();

            _dbContext.Add(admProject);
            save();

            return admProject.project_id;
        }

        public void upd(AdmProject admProject)
        {
            AdmProject updProject = _dbContext.admProjects.Find(admProject.project_id);

            AdmOrganization organization = _dbContext.admOrganizations.Find(admProject.organization.organization_id);
            AdmTypology status = _dbContext.admTypologies.Find(admProject.status.typology_id);

            updProject.name_project= admProject.name_project;
            updProject.date = admProject.date;
            updProject.organization = organization;
            updProject.status = status;

            _dbContext.Entry(updProject).State = EntityState.Modified;
            save();
        }

        //*********** Activity ***************
        public long persist(AdmProjectActivity admProjectActivity)
        {

            AdmTypology state = _dbContext.admTypologies.Find(admProjectActivity.state.typology_id);
            AdmTypology city = _dbContext.admTypologies.Find(admProjectActivity.city.typology_id);
            AdmTypology activity = _dbContext.admTypologies.Find(admProjectActivity.activity_type.typology_id);
            AdmProject project = _dbContext.admProjects.Find(admProjectActivity.project.project_id);
            AdmTypology status = _dbContext.admTypologies.Find(admProjectActivity.status.typology_id);
            AdmUser user = _dbContext.admUsers.Find(admProjectActivity.created_by_user.user_id);

            admProjectActivity.state =state;
            admProjectActivity.city = city;
            admProjectActivity.activity_type = activity;
            admProjectActivity.project = project;
            admProjectActivity.status = status;
            admProjectActivity.created_by_user = user;
            admProjectActivity.date_create = CsnFunctions.now();

            _dbContext.Add(admProjectActivity);
            save();
            return admProjectActivity.project_activity_id;
        }

        public IEnumerable<Object> getAllProyectActivities(int id)
        {
            var project_activities = _dbContext.admProjectActivities
            .Where(x => x.status.typology_id == CareConstants.DEFAULT_STATUS
                    && x.project.project_id == id)
            .Select(
                project_activity => new {
                    project_activity_id = project_activity.project_activity_id,
                    activity_address = project_activity.activity_address,
                    state = new {
                        typology_id = project_activity.state.typology_id,
                        description = project_activity.state.description
                    },
                    city = new {
                        typology_id = project_activity.city.typology_id,
                        description = project_activity.city.description
                    },
                    date = project_activity.date,
                    activity_type = new  {
                        typology_id = project_activity.activity_type.typology_id,
                        description = project_activity.activity_type.description
                    },
                    number_participant = project_activity.number_participant,
                    activity_cost = project_activity.activity_cost,
                    time_duration = project_activity.time_duration,
                    main_contribution = project_activity.main_contribution,
                    limit_challenge_solution = project_activity.limit_challenge_solution,
                    project = new {
                        project_id = project_activity.project.project_id,
                        name_project = project_activity.project.name_project
                    },
                    status = new {
                        typology_id = project_activity.status.typology_id,
                        description = project_activity.status.description
                    },
                    created_by_user = new{
                        user_id = project_activity.created_by_user.user_id,
                        person = new {
                            person_id =project_activity.created_by_user.person.person_id,
                            email = project_activity.created_by_user.person.email,
                        }
                    },
                    date_created = project_activity.date_create               
                }
            ).OrderByDescending( id => id.project_activity_id).ToList();

            return project_activities;
        }

        public object getByIdActivity(int project_id, int activity_id)
        {
            var project_activity = _dbContext.admProjectActivities
            .Where(x => x.project.project_id == project_id && x.project_activity_id == activity_id)
            .Select(
                project_activity => new {
                    project_activity_id = project_activity.project_activity_id,
                    activity_address = project_activity.activity_address,
                    state = new {
                        typology_id = project_activity.state.typology_id,
                        description = project_activity.state.description
                    },
                    city = new {
                        typology_id = project_activity.city.typology_id,
                        description = project_activity.city.description
                    },
                    date = project_activity.date,
                    activity_type = new  {
                        typology_id = project_activity.activity_type.typology_id,
                        description = project_activity.activity_type.description
                    },
                    number_participant = project_activity.number_participant,
                    activity_cost = project_activity.activity_cost,
                    time_duration = project_activity.time_duration,
                    main_contribution = project_activity.main_contribution,
                    limit_challenge_solution = project_activity.limit_challenge_solution,
                    project = new {
                        project_id = project_activity.project.project_id,
                        name_project = project_activity.project.name_project
                    },
                    status = new {
                        typology_id = project_activity.status.typology_id,
                        description = project_activity.status.description
                    },
                    created_by_user = new{
                        user_id = project_activity.created_by_user.user_id,
                        person = new {
                            person_id =project_activity.created_by_user.person.person_id,
                            email = project_activity.created_by_user.person.email,
                        }
                    },
                    date_created = project_activity.date_create               
                }
            ).SingleOrDefault();

            return project_activity;
        }

        public void updProjectActivity (AdmProjectActivity admProjectActivity)
        {
            AdmProjectActivity updPActivity = _dbContext.admProjectActivities.Find(admProjectActivity.project_activity_id);

            AdmTypology state = _dbContext.admTypologies.Find(admProjectActivity.state.typology_id);
            AdmTypology city = _dbContext.admTypologies.Find(admProjectActivity.city.typology_id);
            AdmTypology activity = _dbContext.admTypologies.Find(admProjectActivity.activity_type.typology_id);
            AdmProject project = _dbContext.admProjects.Find(admProjectActivity.project.project_id);
            AdmTypology status = _dbContext.admTypologies.Find(admProjectActivity.status.typology_id);

            updPActivity.activity_address = admProjectActivity.activity_address;
            updPActivity.state = state;
            updPActivity.city = city;
            updPActivity.date = admProjectActivity.date;
            updPActivity.activity_type = activity;
            updPActivity.number_participant = admProjectActivity.number_participant;
            updPActivity.activity_cost = admProjectActivity.activity_cost;
            updPActivity.time_duration = admProjectActivity.time_duration;
            updPActivity.main_contribution = admProjectActivity.main_contribution;
            updPActivity.limit_challenge_solution = admProjectActivity.limit_challenge_solution;
            updPActivity.project = project;
            updPActivity.status = status;

            _dbContext.Entry(updPActivity).State = EntityState.Modified;
            save();            
        }

        public void save()
        {
            _dbContext.SaveChanges();
        }

        
    }
}
