using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public interface IWalkRepository
    {
        Task<Walk> CreateWalk(Walk walk);
        Task<List<Walk>> GetAllWalks();
        Task<Walk?> GetById(Guid id);
        Task<Walk?> UpdateWalk(Guid id , Walk walk);
        Task<Walk> DeleteWalk(Guid id);
        
    }
}
