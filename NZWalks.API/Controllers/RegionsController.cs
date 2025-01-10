﻿using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetAll()
        {
            var regions = dbcontext.Regions.ToList();
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
        public IActionResult GetById(Guid id)
        {
            var region = dbcontext.Regions.Find(id);


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
        public IActionResult Create([FromBody] AddRegionRequestDto regionRequest)
        {
            // map dto to domain model
            Region domainModel = new Region()
            {
                Code = regionRequest.Code,
                Name = regionRequest.Name,
                RegionImageUrl = regionRequest.RegionImageUrl
            };

            dbcontext.Regions.Add(domainModel);
            dbcontext.SaveChanges();

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
        public IActionResult Update([FromRoute]Guid id, [FromBody] UpdateRegionRequestDto updateRequest)
        {
            var region = dbcontext.Regions.FirstOrDefault(x=>x.Id == id);
            if (region == null) return NotFound("No region found");

            region.Code = updateRequest.Code;
            region.Name = updateRequest.Name;
            region.RegionImageUrl = updateRequest.RegionImageUrl;

            dbcontext.SaveChanges();//this property is already tracked by dbcontext so no need to update it 

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
        public IActionResult Delete([FromRoute] Guid id)
        {
            var region = dbcontext.Regions.FirstOrDefault(x => x.Id == id);
            if (region == null) return NotFound("No region found");

            dbcontext.Regions.Remove(region);
            dbcontext.SaveChanges();
            return Ok("Region deleted successfully");
        }

    }
}
