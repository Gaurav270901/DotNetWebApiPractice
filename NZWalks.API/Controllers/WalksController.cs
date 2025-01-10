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
    }
}
