using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository repository;

        public WalksController(IMapper mapper , IWalkRepository repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddWalkRequest createRequest)
        {
            //map ddrequestdto to domain modal
            var walkDomain = mapper.Map<Walk>(createRequest);
            await repository.CreateWalk(walkDomain);
            return Ok(mapper.Map<WalkDto>(walkDomain));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var walkList = await repository.GetAllWalks();

            return Ok(mapper.Map<List<WalkDto>>(walkList));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute]Guid id)
        {
            var walkDomain = await repository.GetById(id);
            if (walkDomain == null) return NotFound("No walk found for corresponding id");
            return Ok(mapper.Map<WalkDto>(walkDomain));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id , WalkUpdateRequest updateRequest)
        {
            var walkDomain = mapper.Map<Walk>(updateRequest);
            walkDomain = await repository.UpdateWalk(id, walkDomain);
            if (walkDomain == null) return NotFound("No walk found with corresponding id");

            return Ok(mapper.Map<WalkDto>(walkDomain));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteWalk([FromRoute]Guid id)
        {
            var walkDomain = await repository.DeleteWalk(id);
            if (walkDomain == null) return (NotFound("No walk found for correspongind id"));

            return Ok(mapper.Map<WalkDto>(walkDomain));
        }
    }
}
