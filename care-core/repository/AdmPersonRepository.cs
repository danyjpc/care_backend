using System;
using System.Collections.Generic;
using System.Linq;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Serilog;
using Serilog.Core;

namespace care_core.repository
{
    public class AdmPersonRepository : IAdmPerson
    {
        private readonly EntityDbContext _dbContext;

        public AdmPersonRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IEnumerable<AdmPerson> getBaseResults(int estado, int personId, long personDpi, string personMail, bool useDevsMail)
        {
            IEnumerable<AdmPerson> respuesta = (
                from person in _dbContext.admPersons
                join culturalIdentity in _dbContext.admTypologies on person.cultural_identity.typology_id equals
                    culturalIdentity.typology_id
                join state in _dbContext.admTypologies on person.state.typology_id equals state.typology_id
                join city in _dbContext.admTypologies on person.city.typology_id equals city.typology_id
                join occupation in _dbContext.admTypologies on person.occupation.typology_id equals occupation
                    .typology_id
                join maritalStatus in _dbContext.admTypologies on person.marital_status.typology_id equals
                    maritalStatus.typology_id
                join education in _dbContext.admTypologies on person.education.typology_id equals education
                    .typology_id
                join spokenLanguage in _dbContext.admTypologies on person.spoken_language.typology_id equals
                    spokenLanguage.typology_id
                join genreTypo in _dbContext.admTypologies on person.genre.typology_id equals genreTypo
                    .typology_id
                join statusTypo in _dbContext.admTypologies on person.status.typology_id equals statusTypo
                    .typology_id
                where (
                    statusTypo.typology_id.Equals((estado > 0 ? estado : statusTypo.typology_id))
                    &&
                    person.person_id.Equals((personId > 0 ? personId : person.person_id))
                    &&
                    person.cui.Equals((personDpi > 0 ? personDpi : person.cui))
                    &&
                    person.email.Equals(!personMail.Equals("") ? personMail : person.email)
                    //There are cases where is necessary to check on devs email, like persisting a person to avoid a duplicate mail error
                    //even if the field is unique on the database, which causes an error
                    && (useDevsMail == false ? !person.email.Equals(CareConstants.DEV_MAIL) : person.email.Equals(person.email))
                )
                select new AdmPerson()
                {
                    person_id = person.person_id,
                    first_name = person.first_name,
                    last_name = person.last_name,
                    birthday = person.birthday,
                    phone_number = person.phone_number,
                    cui = person.cui,
                    cultural_identity = new AdmTypology()
                    {
                        typology_id = culturalIdentity.typology_id,
                        description = culturalIdentity.description
                    },
                    state = new AdmTypology()
                    {
                        typology_id = state.typology_id,
                        description = state.description,
                    },
                    city = new AdmTypology()
                    {
                        typology_id = city.typology_id,
                        description = city.description,
                    },
                    occupation = new AdmTypology()
                    {
                        typology_id = occupation.typology_id,
                        description = occupation.description,
                    },
                    marital_status = new AdmTypology()
                    {
                        typology_id = maritalStatus.typology_id,
                        description = maritalStatus.description,
                    },
                    education = new AdmTypology()
                    {
                        typology_id = education.typology_id,
                        description = education.description,
                    },
                    spoken_language = new AdmTypology()
                    {
                        typology_id = spokenLanguage.typology_id,
                        description = spokenLanguage.description,
                    },
                    address_line = person.address_line,
                    email = person.email,
                    daughters_no = person.daughters_no,
                    sons_no = person.sons_no,
                    genre = new AdmTypology()
                    {
                        typology_id = genreTypo.typology_id,
                        description = genreTypo.description,
                    },
                    status = new AdmTypology()
                    {
                        typology_id = statusTypo.typology_id,
                        description = statusTypo.description,
                    },

                    date_created = person.date_created
                }
            ).OrderBy(x => x.person_id).ToList();

            return respuesta;
        }


        public IEnumerable<Object> getAll(int estado)
        {
            return getBaseResults(estado, 0, 0, "", false);
        }

        public AdmPerson getById(int person_id)
        {
            return getBaseResults(0, person_id, 0, "", false).SingleOrDefault();
        }

        public long persist(AdmPerson admPerson)
        {
            try
            {
                _dbContext.Add(admPerson);
            }
            catch (Exception ex)
            {
                Log.Error("Error: " + ex.Message);
            }

            save();

            return admPerson.person_id;
        }

        public void upd(AdmPerson admPerson)
        {
            throw new System.NotImplementedException();
        }

        public void save()
        {
            _dbContext.SaveChanges();
        }

        public AdmPerson findByDpi(long personDpi)
        {
            return getBaseResults(0, 0, personDpi, "", true).SingleOrDefault();
        }

        public AdmPerson findByEmail(string personMail, bool useDevMail)
        {
            return getBaseResults(0, 0, 0, personMail, true).SingleOrDefault();
        }

        //Method that persist person for user, takes:
        //name, last name, dpi, birthdate, email, password, state, city, created by
        //T1563 https://dev.azure.com/People-Apps/CARE/_workitems/edit/1563/
        public AdmPerson persistDefaultValues(AdmPerson admPerson)
        {
            try
            {
                AdmTypology emptyTypology = _dbContext.admTypologies.Find(CareConstants.EMPTY_TYPOLOGY);
                admPerson.phone_number = CareConstants.ZERO_DEFAULT;
                admPerson.address_line = CareConstants.EMPTY_STRING;
                admPerson.cultural_identity = emptyTypology;
                admPerson.occupation = emptyTypology;
                admPerson.marital_status = emptyTypology;
                admPerson.spoken_language = emptyTypology;
                admPerson.education = emptyTypology;
                admPerson.address_line = CareConstants.EMPTY_STRING;
                admPerson.daughters_no = CareConstants.ZERO_DEFAULT;
                admPerson.sons_no = CareConstants.ZERO_DEFAULT;
                admPerson.genre = emptyTypology;
                admPerson.status = _dbContext.admTypologies.Find(CareConstants.ESTADO_ACTIVO);
                admPerson.date_created = CsnFunctions.now();

                this.persist(admPerson);
            }
            catch (Exception ex)
            {
                Log.Error("Error: " + ex.Message);
            }

            return _dbContext.admPersons.Find(admPerson.person_id);
        }

        public object findPersonByCui(int personCui)
        {
            return (
                from person in _dbContext.admPersons
                where person.cui.Equals(personCui)
                select new
                {
                    person_id = person.person_id
                }
            ).OrderBy(x => x.person_id).SingleOrDefault();
        }

        public object findPersonByEmail(string email)
        {
            return (
                from person in _dbContext.admPersons
                where person.email.Equals(email)
                select new
                {
                    person_id = person.person_id
                }
            ).OrderBy(x => x.person_id).SingleOrDefault();
        }
    }
}