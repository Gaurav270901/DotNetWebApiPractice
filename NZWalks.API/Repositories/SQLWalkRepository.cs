using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDBContext dBContext;

        public SQLWalkRepository(NZWalksDBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public async Task<Walk> CreateWalk(Walk walk)
        {
            await dBContext.Walks.AddAsync(walk);
            await dBContext.SaveChangesAsync();
            return walk; 
        }

        public async Task<List<Walk>> GetAllWalks()
        {
            return (await dBContext.Walks.Include("Difficulty").Include("Region").ToListAsync());
        }
    }
}
