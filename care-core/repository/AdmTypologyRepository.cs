using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.EntityFrameworkCore;

namespace care_core.repository
{
    public class AdmTypologyRepository : IAdmTypology
    {
        private readonly EntityDbContext _dbContext;

        public AdmTypologyRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<AdmTypology> getAll(long parent_id, bool showInSurvey)
        {
            //used for searching endpoint
            if (parent_id == 0)
            {
                return _dbContext.admTypologies
                    .Select
                    (
                        tipology => new AdmTypology
                        {
                            typology_id = tipology.typology_id,
                            internal_id = tipology.internal_id,
                            description = tipology.description,
                            value_1 = tipology.value_1,
                            value_2 = tipology.value_2,
                            is_editable = tipology.is_editable,
                            show_survey = tipology.show_survey
                        }
                    ).OrderBy(x => x.typology_id);
            }
            return _dbContext.admTypologies.Where(x => x.parent_typology.typology_id == parent_id
                && x.show_survey == showInSurvey)
                .Select
                (
                    tipology => new AdmTypology
                    {
                        typology_id = tipology.typology_id,
                        internal_id = tipology.internal_id,
                        description = tipology.description,
                        value_1 = tipology.value_1,
                        value_2 = tipology.value_2,
                        is_editable = tipology.is_editable,
                        show_survey = tipology.show_survey
                    }
                ).OrderBy(x => x.typology_id);
        }

        public AdmTypology getById(int? id)
        {
            return _dbContext.admTypologies
                .Where(x => x.typology_id == id)
                .Select(
                    tipology => new AdmTypology
                    {
                        typology_id = tipology.typology_id,
                        internal_id = tipology.internal_id,
                        description = tipology.description,
                        value_1 = tipology.value_1,
                        value_2 = tipology.value_2,
                        is_editable = tipology.is_editable,
                        show_survey = tipology.show_survey,
                        childs = tipology.childs
                    }
                ).SingleOrDefault();
        }

        public long persist(AdmTypology admTypology)
        {
            admTypology = this.setDefaultValues(admTypology);
            AdmTypology parent = _dbContext.admTypologies.Find(admTypology.parent_typology.typology_id);

            admTypology.parent_typology = parent;

            _dbContext.Add(admTypology);

            save();

            return admTypology.typology_id;
        }

        private AdmTypology setDefaultValues(AdmTypology admTypology)
        {
            if (admTypology.parent_typology == null)
            {
                AdmTypology parentTypology = new AdmTypology();
                parentTypology.typology_id = CareConstants.GLOBAL_TYPOLOGY;
                admTypology.parent_typology = parentTypology;
            }

            return admTypology;
        }


        public void upd(AdmTypology tipology)
        {
            AdmTypology updTypologia = _dbContext.admTypologies.Find(tipology.typology_id);

            if (tipology.parent_typology != null)
            {
                AdmTypology padreTypologia = _dbContext.admTypologies.Find(tipology.parent_typology.typology_id);
                updTypologia.parent_typology = padreTypologia;
            }

            if (tipology.description != null)
            {
                updTypologia.description = tipology.description;
            }

            if (tipology.value_1 != null)
            {
                updTypologia.value_1 = tipology.value_1;
            }

            if (tipology.value_2 != null)
            {
                updTypologia.value_2 = tipology.value_2;
            }

            if (tipology.show_survey != null)
            {
                updTypologia.show_survey = tipology.show_survey;
            }


            _dbContext.Entry(updTypologia).State = EntityState.Modified;
            save();
        }
        
        public void save()
        {
            _dbContext.SaveChanges();
        }
    }
}