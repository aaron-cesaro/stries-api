using Post.Api.Database.Entities;
using System;
using System.Collections.Generic;

namespace Post.Api.Infrastructure.Data.Seeds
{
    public static class PostSeed
    {
        public static List<PostEntity> PostSeeds = new List<PostEntity>
        {
            new PostEntity
            {
                Id = Guid.Parse("3aa71d25-33b7-47fc-b59f-5089693d6601"),
                AuthorId = Guid.Parse("3aa71d25-33b7-47fc-b59f-5089693d6603"),
                Title = "Analisys 1",
                Summary = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent pellentesque urna quis elit eleifend cursus. Donec euismod magna quam, quis malesuada mi dictum a. Donec euismod felis tincidunt.",
                ImageUrl = "https://pbs.twimg.com/profile_images/1021763565194235904/ElgtjZDA_400x400.jpg",
                Status = Application.Models.PostStatus.draft,
                Body = new PostBody
                {
                    CompanyDescription = new Api.Database.Models.CompanyDescription
                    {
                        Ticker = "KOD.L",
                        CompanyName = "Kodal Minerls",
                        Description = "Kodal Minerals PLC (LON:KOD) is a junior explorer with a focus on West Africa, and in particular lithium and gold. The company is run by experienced exploration geologist Bernard Aylward, helped by Robert Wooldridge, who is a partner in UK broking house SP Angel. Steve Zaninovich became project manager in November 2018 to oversee the Bougouni Lithium project feasibility study and development plan. Steve is a Mining and Construction engineer with extensive direct experience in West Africa having been involved in projects in Ghana, Mauritania, Senegal, Burkina Faso, Cote d’Ivoire and Mali. He was previously Business Development manager for Lycopodium in Australia and has been on the board of Gryphon Resources, Indiana Resources and Maximus Resources",
                        Founded = "2009",
                        Exchange = "LON",
                        Sector = "Materials",
                        Industry = "Metal Mining",
                        Address = "Prince Frederick House 35 - 39 Maddox Street W1S 2PP United Kingdom",
                        PhoneNumber = "+44 2034632260",
                        WebsiteUrl = "https://www.kodalminerals.com/",
                        MarketCap = 34.630M,
                        Employees = 150,
                        Volume = 2_810_887_631,
                        AverageVolume = 13884.92M,
                        SharesOutstanding = 15_810_000,
                        Float = 8_123_000,
                        LastPrice = 0.22M,
                        Beta = 1.66M,
                        PeRatio = 0,
                        Eps = 0
                    },
                    CompanyThesis = new Api.Database.Models.CompanyThesis(),
                    CompanyValuation = new Api.Database.Models.CompanyValuation(),
                    CompanyFinancials = new Api.Database.Models.CompanyFinancials(),
                    CompanyOther = new Api.Database.Models.CompanyOther(),
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            new PostEntity
            {
                Id = Guid.Parse("638c857c-1e53-4dc3-9281-829b8e438902"),
                AuthorId = Guid.Parse("638c857c-1e53-4dc3-9281-829b8e438904"),
                Title = "Analisys 2",
                Summary = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent pellentesque urna quis elit eleifend cursus. Donec euismod magna quam, quis malesuada mi dictum a. Donec euismod felis tincidunt.",
                ImageUrl = "https://pbs.twimg.com/profile_images/1021763565194235904/ElgtjZDA_400x400.jpg",
                Status = Application.Models.PostStatus.published,
                Body = new PostBody
                {
                    CompanyDescription = new Api.Database.Models.CompanyDescription
                    {
                        Ticker = "KOD.L",
                        CompanyName = "Kodal Minerls",
                        Description = "Kodal Minerals PLC (LON:KOD) is a junior explorer with a focus on West Africa, and in particular lithium and gold. The company is run by experienced exploration geologist Bernard Aylward, helped by Robert Wooldridge, who is a partner in UK broking house SP Angel. Steve Zaninovich became project manager in November 2018 to oversee the Bougouni Lithium project feasibility study and development plan. Steve is a Mining and Construction engineer with extensive direct experience in West Africa having been involved in projects in Ghana, Mauritania, Senegal, Burkina Faso, Cote d’Ivoire and Mali. He was previously Business Development manager for Lycopodium in Australia and has been on the board of Gryphon Resources, Indiana Resources and Maximus Resources",
                        Founded = "2009",
                        Exchange = "LON",
                        Sector = "Materials",
                        Industry = "Metal Mining",
                        Address = "Prince Frederick House 35 - 39 Maddox Street W1S 2PP United Kingdom",
                        PhoneNumber = "+44 2034632260",
                        WebsiteUrl = "https://www.kodalminerals.com/",
                        MarketCap = 34.630M,
                        Employees = 150,
                        Volume = 2_810_887_631,
                        AverageVolume = 13884.92M,
                        SharesOutstanding = 15_810_000,
                        Float = 8_123_000,
                        LastPrice = 0.22M,
                        Beta = 1.66M,
                        PeRatio = 0,
                        Eps = 0
                    },
                    CompanyThesis = new Api.Database.Models.CompanyThesis(),
                    CompanyValuation = new Api.Database.Models.CompanyValuation(),
                    CompanyFinancials = new Api.Database.Models.CompanyFinancials(),
                    CompanyOther = new Api.Database.Models.CompanyOther(),
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
    }
}