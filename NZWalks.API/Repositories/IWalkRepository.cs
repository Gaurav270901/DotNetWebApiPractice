﻿using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public interface IWalkRepository
    {
        Task<Walk> CreateWalk(Walk walk);
        Task<List<Walk>> GetAllWalks(string? filterOn = null, string? filterQuery = null,
           string? sortBy = null, bool isAsc = true, int pageNumber = 1, int pageSize = 1000);
        Task<Walk?> GetById(Guid id);
        Task<Walk?> UpdateWalk(Guid id , Walk walk);
        Task<Walk> DeleteWalk(Guid id);
        
    }
}
