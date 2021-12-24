using System;
using System.Collections.Generic;
using System.Linq;
using care_core.dto.AdmCase;
using care_core.dto.AdmOrganization;
using care_core.dto.AdmOrganizationMember;
using care_core.dto.AdmPerson;
using care_core.dto.AdmQuestionGroup;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace care_core.repository
{
    public class AdmOptionRepository : IAdmOption
    {
        private readonly EntityDbContext _dbContext;

        public AdmOptionRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<AdmQuestionOptionDto> getAll(int estado)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AdmQuestionOptionDto> getAllByQuestion(int questionId, int estado)
        {
            IEnumerable<AdmQuestionOptionDto> opciones = _dbContext.admOptions.Where(x =>
                x.question.question_id == questionId
                && x.status.typology_id == (estado > 0 ? estado : x.status.typology_id)).Select(
                opcion => new AdmQuestionOptionDto()
                {
                    option_id = opcion.option_id,
                    value = opcion.value,
                    question = new AdmQuestionDto()
                    {
                        question_id = opcion.question.question_id,
                        name_question = opcion.question.name_question
                    },
                    status = new AdmTypologyDto()
                    {
                        typology_id = opcion.status.typology_id,
                        description = opcion.status.description
                    },
                    created_by_user = new AdmUserDto()
                    {
                        user_id = opcion.created_by_user.user_id,
                        email = opcion.created_by_user.person.email
                    }
                }).ToList();

            //appending group to evety quest
            return opciones;
        }

        public AdmQuestionOptionDto getOptionById(int optionId)
        {
            throw new NotImplementedException();
        }

        public int persist(AdmOption admOption)
        {
            _dbContext.Add(admOption);
            this.save();

            return admOption.option_id;
        }

        public int update(AdmOption admOption)
        {
            AdmOption currentOption = _dbContext.admOptions.Find(admOption.option_id);
            currentOption.status = admOption.status;
            currentOption.value = admOption.value;

            _dbContext.Entry(currentOption).State = EntityState.Modified;
            save();
            return currentOption.option_id;
        }

        public void save()
        {
            _dbContext.SaveChanges();
        }
    }
}