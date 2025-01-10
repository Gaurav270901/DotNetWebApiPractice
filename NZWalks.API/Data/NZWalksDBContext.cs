using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models.Domain;
using System;
using System.Collections.Generic;

namespace NZWalks.API.Data
{
    public class NZWalksDBContext : DbContext
    {
        public NZWalksDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        // DbSet represents collections of entities in the database
        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Walk> Walks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for Difficulty
            var difficulties = new List<Difficulty>
            {
                new Difficulty
                {
                    Id = Guid.Parse("7ed1bcf7-b9a8-4d9b-8777-9e88c5e78126"),
                    Name = "Easy"
                },
                new Difficulty
                {
                    Id = Guid.Parse("f61a2f59-914f-4d69-9c3d-7f4f69096349"),
                    Name = "Medium"
                },
                new Difficulty
                {
                    Id = Guid.Parse("3b4cfdc7-725a-4edc-8fd7-2a841788bc1e"),
                    Name = "Hard"
                }
            };
            modelBuilder.Entity<Difficulty>().HasData(difficulties);

            // Seed data for Region
            var regions = new List<Region>
            {
                new Region
                {
                    Id = Guid.Parse("f7248fc3-2585-4efb-8d1d-1c555f4087f6"),
                    Name = "Auckland",
                    Code = "AKL",
                    RegionImageUrl = "https://images.pexels.com/photos/5169056/pexels-photo-5169056.jpeg"
                },
                new Region
                {
                    Id = Guid.Parse("6884f7d7-ad1f-4101-8df3-7a6fa7387d81"),
                    Name = "Northland",
                    Code = "NTL",
                    RegionImageUrl = null
                },
                new Region
                {
                    Id = Guid.Parse("14ceba71-4b51-4777-9b17-46602cf66153"),
                    Name = "Bay Of Plenty",
                    Code = "BOP",
                    RegionImageUrl = null
                },
                new Region
                {
                    Id = Guid.Parse("cfa06ed2-bf65-4b65-93ed-c9d286ddb0de"),
                    Name = "Wellington",
                    Code = "WGN",
                    RegionImageUrl = "https://images.pexels.com/photos/4350631/pexels-photo-4350631.jpeg"
                },
                new Region
                {
                    Id = Guid.Parse("906cb139-415a-4bbb-a174-1a1faf9fb1f6"),
                    Name = "Nelson",
                    Code = "NSN",
                    RegionImageUrl = "https://images.pexels.com/photos/13918194/pexels-photo-13918194.jpeg"
                },
                new Region
                {
                    Id = Guid.Parse("f077a22e-4248-4bf6-b564-c7cf4e250263"),
                    Name = "Southland",
                    Code = "STL",
                    RegionImageUrl = null
                },
            };
            modelBuilder.Entity<Region>().HasData(regions);
        }
    }
}
