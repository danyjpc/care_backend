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
    public class AdmCaseTracingRepository : IAdmCaseTracing
    {
        private readonly EntityDbContext _dbContext;

        public AdmCaseTracingRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Object> getAll(int case_id)
        {
            var AdmCaseTracing =_dbContext.admCaseTracings
            .Where(x => x.cases.case_id == case_id && x.status.typology_id == CareConstants.DEFAULT_STATUS)
            .Select(
                caseTracing => new{
                    tracing_id = caseTracing.tracing_id,
                    tracing_status = new{
                        typology_id = caseTracing.tracing_status.typology_id,
                        description = caseTracing.tracing_status.description
                    },
                    partner_relationship = caseTracing.partner_relationship,
                    children_relationship = caseTracing.children_relationship,
                    support_reason = caseTracing.support_reason,
                    tracing_diagnosis = caseTracing.tracing_diagnosis,
                    tracing_description = caseTracing.tracing_description,
                    observations = caseTracing.observations,
                    cases = new{
                        case_id = caseTracing.cases.case_id,
                    },
                    status = new{
                        typology_id = caseTracing.status.typology_id,
                        description = caseTracing.status.description
                    },
                    created_by_user = new{
                        user_id = caseTracing.created_by_user.user_id,
                        person = new{
                            first_name = caseTracing.created_by_user.person.first_name,
                            email = caseTracing.created_by_user.person.email
                        }
                    },
                    date_create = caseTracing.date_create
                }
            ) 
            .OrderByDescending(id => id.tracing_id).ToList();
            return AdmCaseTracing;
        }

        public Object getCaseTracingById(int case_id, int tracing_id)
        {
            var AdmCaseTracing =_dbContext.admCaseTracings
            .Where(x => x.cases.case_id == case_id && x.tracing_id == tracing_id)
            .Select(
                caseTracing => new{
                    tracing_id = caseTracing.tracing_id,
                    tracing_status = new{
                        typology_id = caseTracing.tracing_status.typology_id,
                        description = caseTracing.tracing_status.description
                    },
                    partner_relationship = caseTracing.partner_relationship,
                    children_relationship = caseTracing.children_relationship,
                    support_reason = caseTracing.support_reason,
                    tracing_diagnosis = caseTracing.tracing_diagnosis,
                    tracing_description = caseTracing.tracing_description,
                    observations = caseTracing.observations,
                    cases = new{
                        case_id = caseTracing.cases.case_id,
                    },
                    status = new{
                        typology_id = caseTracing.status.typology_id,
                        description = caseTracing.status.description
                    },
                    created_by_user = new{
                        user_id = caseTracing.created_by_user.user_id,
                        person = new{
                            first_name = caseTracing.created_by_user.person.first_name,
                            email = caseTracing.created_by_user.person.email
                        }
                    },
                    date_create = caseTracing.date_create
                }
            ).SingleOrDefault();
            return AdmCaseTracing;
        }

        public long persist(AdmCaseTracing admCaseTracing)
        {
            AdmTypology tracing_status = _dbContext.admTypologies.Find(admCaseTracing.tracing_status.typology_id);
            AdmCase cases = _dbContext.admCases.Find(admCaseTracing.cases.case_id);
            AdmTypology status = _dbContext.admTypologies.Find(admCaseTracing.status.typology_id);
            AdmUser created_by = _dbContext.admUsers.Find(admCaseTracing.created_by_user.user_id);

            admCaseTracing.tracing_status = tracing_status;
            admCaseTracing.cases = cases;
            admCaseTracing.status = status;
            admCaseTracing.created_by_user = created_by;
            admCaseTracing.date_create = CsnFunctions.now();

            _dbContext.Add(admCaseTracing);
            save();

            return admCaseTracing.tracing_id;
        }

        public  void upd(AdmCaseTracing admCaseTracing)
        {
            AdmCaseTracing updTracing = _dbContext.admCaseTracings.Find(admCaseTracing.tracing_id);

            AdmTypology tracing_status = _dbContext.admTypologies.Find(admCaseTracing.tracing_status.typology_id);
            AdmCase cases = _dbContext.admCases.Find(admCaseTracing.cases.case_id);
            AdmTypology status = _dbContext.admTypologies.Find(admCaseTracing.status.typology_id);
            AdmUser created_by = _dbContext.admUsers.Find(admCaseTracing.created_by_user.user_id);

            updTracing.tracing_status = tracing_status;
            updTracing.partner_relationship = admCaseTracing.partner_relationship;
            updTracing.children_relationship = admCaseTracing.children_relationship;
            updTracing.support_reason = admCaseTracing.support_reason;
            updTracing.tracing_diagnosis = admCaseTracing.tracing_diagnosis;
            updTracing.tracing_description = admCaseTracing.tracing_description;
            updTracing.observations = admCaseTracing.observations;
            updTracing.cases = cases;
            updTracing.status = status;
            //updTracing.created_by_user = created_by;

            _dbContext.Entry(updTracing).State = EntityState.Modified;
            save();
        }

        public void save()
        {
            _dbContext.SaveChanges();
        }

        
    }
}