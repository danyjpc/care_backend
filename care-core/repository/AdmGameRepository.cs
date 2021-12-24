using System;
using System.Collections.Generic;
using System.Linq;
using care_core.model;
using care_core.repository.interfaces;
using care_core.util;

using Microsoft.EntityFrameworkCore;

namespace care_core.repository
{
    public class AdmGameRepository : IAdmGame
    {
        private readonly EntityDbContext _dbContext;

        public AdmGameRepository(EntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<AdmGame> getAll()
        {
            return _dbContext.admgames.Select(
                game => new AdmGame
                {
                    id = game.id,
                    title = game.title,
                    phrase = game.phrase
                }
            );
        }

        public IEnumerable<Object> reportGetAll()
        {
            return (from game in _dbContext.admgames
                    select new
                    {
                        game.id,
                        game.title,
                        game.phrase
                    }
                ).ToList();
        }

        public AdmGame getById(long id)
        {
            return _dbContext.admgames
                .Where(x => x.id == id)
                .Select(
                    game => new AdmGame
                    {
                        id = game.id,
                        title = game.title,
                        phrase = game.phrase
                    }
                ).SingleOrDefault();
        }

        public long persist(AdmGame admGame)
        {
            _dbContext.Add(admGame);

            save();

            return admGame.id;
        }

        public void upd(AdmGame admGame)
        {
            AdmGame updaGame = _dbContext.admgames.Find(admGame.id);
            updaGame.phrase = admGame.phrase;
            updaGame.title = admGame.title;

            _dbContext.Entry(updaGame).State = EntityState.Modified;
            save();
        }

        public void save()
        {
            _dbContext.SaveChanges();
        }
    }
}