using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Services;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;

namespace NZWalks.API.Repositories
{
    public class SQLRegionRepository : IRegionRepository
    {
        private NZWalksDBContext dbContext;
        private readonly AsyncPolicyWrap _resiliencePolicy;


        public SQLRegionRepository(NZWalksDBContext dBContext, ResiliencePolicyService resiliencePolicyService)
        {
            this.dbContext = dBContext;
            // Define retry policy (3 retries with exponential backoff)
            _resiliencePolicy = resiliencePolicyService.ResiliencePolicy;

        }

        public async Task<Region?> CreateRegion(Region region)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {
                throw new Exception("Simulated database failure");

                dbContext.Regions.Add(region);
                await dbContext.SaveChangesAsync();
                return region;
            });

        }

        public async Task<Region?> DeleteRegion(Guid regionId)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {


                var existingRegion = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == regionId);
                if (existingRegion == null)
                {
                    return null;
                }

                dbContext.Regions.Remove(existingRegion);
                await dbContext.SaveChangesAsync();
                return existingRegion;
            });

        }

        public async Task<List<Region>> GetAllAsync()
        {
            //return await dbContext.Regions.ToListAsync();
            return await _resiliencePolicy.ExecuteAsync(async () => await dbContext.Regions.ToListAsync());
        }

        public async Task<Region?> GetById(Guid id)
        {
            // return await dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id);
            return await _resiliencePolicy.ExecuteAsync(async () => await dbContext.Regions.FirstOrDefaultAsync(r => r.Id == id));

        }

        public async Task<Region?> UpdateRegion(Guid id, Region region)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {


                var existingRegion = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
                if (existingRegion == null) return null;

                existingRegion.Name = region.Name;
                existingRegion.Code = region.Code;
                existingRegion.RegionImageUrl = region.RegionImageUrl;

                await dbContext.SaveChangesAsync();

                return existingRegion;
            });
        }
    }
}
