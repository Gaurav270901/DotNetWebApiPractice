using System.Globalization;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Region;
using NZWalks.API.CustomActionFilter;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize ]
    public class RegionsController : ControllerBase
    {
        private NZWalksDBContext dbcontext;
        private IRegionRepository regionRepository;
        private IMapper mapper;

        public RegionsController(NZWalksDBContext dBContext ,IRegionRepository regionRepository , IMapper mapper)
        {
            this.dbcontext = dBContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper; 
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var regions = await regionRepository.GetAllAsync();
            //var regionsDto = new List<RegionDTO>();
            //foreach(var region in regions)
            //{
            //    regionsDto.Add(new RegionDTO()
            //    {
            //        Id = region.Id,
            //        Code = region.Code,
            //        Name = region.Name , 
            //        RegionImageUrl = region.RegionImageUrl

            //    });
            //} this done in one line of mapper

            return Ok(mapper.Map<List<RegionDTO>>(regions));
        }

        //get single regions

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var region = await regionRepository.GetById(id);

            if (region == null) return NotFound();
            //var regionDto = new RegionDTO()
            //{
            //    Id = region.Id,
            //    Code = region.Code,
            //    Name = region.Name,
            //    RegionImageUrl = region.RegionImageUrl
            //};

            return Ok(mapper.Map<RegionDTO>(region));
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto regionRequest)
        {
            // map dto to domain model

            //if(!ModelState.IsValid) return BadRequest(ModelState);
            Region domainModel = mapper.Map<Region>(regionRequest);

            await regionRepository.CreateRegion(domainModel);

            var regionDto =mapper.Map<RegionDTO>(domainModel);
            return CreatedAtAction(nameof(GetById), new { id = domainModel.Id }, regionDto);
        }

        [HttpPut]
        [Route("{id}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody] UpdateRegionRequestDto updateRequest)
        {
            //if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedRegion = mapper.Map<Region>(updateRequest);
            updatedRegion = await regionRepository.UpdateRegion(id, updatedRegion);
            if (updatedRegion == null) return NotFound("Region not found");
            var regionDto = mapper.Map<RegionDTO>(updatedRegion);
            return Ok(regionDto);

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var region = await regionRepository.DeleteRegion(id);
            if (region == null) return NotFound("No region found");
            return Ok("Region deleted successfully"); 
        }

    }
}
