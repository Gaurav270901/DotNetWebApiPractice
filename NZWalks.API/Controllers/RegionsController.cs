using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Region;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private NZWalksDBContext dbcontext;

        public RegionsController(NZWalksDBContext dBContext)
        {
            this.dbcontext = dBContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var regions = await dbcontext.Regions.ToListAsync();
            var regionsDto = new List<RegionDTO>();
            foreach(var region in regions)
            {
                regionsDto.Add(new RegionDTO()
                {
                    Id = region.Id,
                    Code = region.Code,
                    Name = region.Name , 
                    RegionImageUrl = region.RegionImageUrl

                });
            }
            return Ok(regionsDto);
        }

        //get single regions

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var region = await dbcontext.Regions.FindAsync(id);

            if (region == null) return NotFound();
            var regionDto = new RegionDTO()
            {
                Id = region.Id,
                Code = region.Code,
                Name = region.Name,
                RegionImageUrl = region.RegionImageUrl
            };
            return Ok(regionDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto regionRequest)
        {
            // map dto to domain model
            Region domainModel = new Region()
            {
                Code = regionRequest.Code,
                Name = regionRequest.Name,
                RegionImageUrl = regionRequest.RegionImageUrl
            };

            await dbcontext.Regions.AddAsync(domainModel);
            await dbcontext.SaveChangesAsync();

            var regionDto = new RegionDTO
            {
                Id = domainModel.Id,
                Code = domainModel.Code,
                Name = domainModel.Name,
                RegionImageUrl = domainModel.RegionImageUrl
            };
            return CreatedAtAction(nameof(GetById), new { id = domainModel.Id }, regionDto);
        }
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody] UpdateRegionRequestDto updateRequest)
        {
            var region = await dbcontext.Regions.FirstOrDefaultAsync(x=>x.Id == id);
            if (region == null) return NotFound("No region found");

            region.Code = updateRequest.Code;
            region.Name = updateRequest.Name;
            region.RegionImageUrl = updateRequest.RegionImageUrl;

            await dbcontext.SaveChangesAsync();//this property is already tracked by dbcontext so no need to update it 

            var regionDto = new RegionDTO
            {
                Id = region.Id,
                Name = region.Name,
                Code = region.Code,
                RegionImageUrl = region.RegionImageUrl
            };

            return Ok(regionDto);

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var region = await dbcontext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if (region == null) return NotFound("No region found");

            dbcontext.Regions.Remove(region);
            await dbcontext.SaveChangesAsync();
            return Ok("Region deleted successfully"); 
        }

    }
}
