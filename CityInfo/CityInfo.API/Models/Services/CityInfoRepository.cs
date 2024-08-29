using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Models.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;
        public CityInfoRepository(CityInfoContext context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {
            if(includePointsOfInterest)
            {
                return await _context.Cities.Include(c => c.PointsofInterest)
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }

            return await _context.Cities
                .Where(c=>c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId
            ,int pointOfinterestId)
        {
            return await _context.PointsOfInterests
                .Where(p => p.CityId == cityId && p.Id == pointOfinterestId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsForCityAsync(int cityId)
        {
            return await _context.PointsOfInterests
                .Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, 
            PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId,false);
            if (city != null)
            {
                city.PointsofInterest.Add(pointOfInterest);
            }
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterests.Remove(pointOfInterest);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
