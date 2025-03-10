using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Services;
using Polly.Wrap;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDBContext dBContext;

        private readonly AsyncPolicyWrap _resiliencePolicy;

        public SQLWalkRepository(NZWalksDBContext dBContext, ResiliencePolicyService resiliencePolicyService)
        {
            this.dBContext = dBContext;
            this._resiliencePolicy = resiliencePolicyService.ResiliencePolicy; // ✅ Get the AsyncPolicyWrap
        }

        public async Task<Walk> CreateWalk(Walk walk)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {
                await dBContext.Walks.AddAsync(walk);
                await dBContext.SaveChangesAsync();
                return walk;
            });
        }


        public async Task<Walk?> DeleteWalk(Guid id)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {
                var existingWalk = await dBContext.Walks.FirstOrDefaultAsync(x => x.Id == id);
                if (existingWalk == null) return null;

                dBContext.Remove(existingWalk);
                await dBContext.SaveChangesAsync();
                return existingWalk;
            });

        }

        public async Task<List<Walk>> GetAllWalks(string? filterOn, string? filterQuery, string? sortBy, bool isAsc, int pageNumber = 1, int pageSize = 1000)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {
                var walks = dBContext.Walks.Include("Difficulty").Include("Region").AsQueryable();

                // Filtering
                if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
                {
                    if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    {
                        walks = walks.Where(x => x.Name.Contains(filterQuery));
                    }
                }

                // Sorting
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    {
                        walks = isAsc ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
                    }
                    else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                    {
                        walks = isAsc ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
                    }
                }

                // Pagination
                var skipResults = (pageNumber - 1) * pageSize;
                return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
            });
        }


        public async Task<Walk?> GetById(Guid id)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {
                return await dBContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x => x.Id == id);
            });
        }

        public async Task<Walk?> UpdateWalk(Guid id, Walk walk)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {
                var existingWalk = await dBContext.Walks.FirstOrDefaultAsync(x => x.Id == id);
                if (existingWalk == null) return null;

                existingWalk.Name = walk.Name;
                existingWalk.Description = walk.Description;
                existingWalk.LengthInKm = walk.LengthInKm;
                existingWalk.WalkImageUrl = walk.WalkImageUrl;
                existingWalk.RegionId = walk.RegionId;
                existingWalk.DifficultyId = walk.DifficultyId;

                await dBContext.SaveChangesAsync();
                return existingWalk;
            });
        }
    }
}
