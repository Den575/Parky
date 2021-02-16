using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDbContext _db;

        public TrailRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CreateTrail(Trail Trail)
        {
            _db.Trails.Add(Trail);
            return Save();
        }

        public bool DeleteTrail(Trail Trail)
        {
            _db.Trails.Remove(Trail);
            return Save();
        }

        public Trail GetTrail(int nationParkId)
        {
            return _db.Trails.FirstOrDefault(i => i.Id == nationParkId);
        }

        public ICollection<Trail> GetTrails()
        {
            return _db.Trails.OrderBy(i => i.Id).ToList();
        }

        public bool TrailExist(string name)
        {
            bool value = _db.Trails.Any(i => i.Name.ToLower().Trim() == name.Trim().ToLower());
            return value;
        }

        public bool TrailExist(int id)
        {
            bool value = _db.Trails.Any(i => i.Id == id);
            return value;
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateTrail(Trail Trail)
        {
            _db.Trails.Update(Trail);
            return Save();
        }

        public ICollection<Trail> GetTrailsInNationalPark(int npId)
        {
            return _db.Trails.Include(d=>d.NationalPark).Where(d=>d.NationalParkId==npId).ToList();
        }
    }
}
