using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PitStore.Data;
using PitStore.Models;

namespace PitStore.Services
{
    public class ProductSeeder
    {
        private readonly ApplicationDbContext _context;

        public ProductSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (!await _context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Tişört", Description = "F1 Takım Tişörtleri", ImageUrl = "/images/categories/tshirt.jpg" },
                    new Category { Name = "Şapka", Description = "F1 Takım Şapkaları", ImageUrl = "/images/categories/hat.jpg" },
                    new Category { Name = "Aksesuar", Description = "F1 Aksesuarları", ImageUrl = "/images/categories/accessory.jpg" }
                };
                await _context.Categories.AddRangeAsync(categories);
                await _context.SaveChangesAsync();
            }

            if (!await _context.Teams.AnyAsync())
            {
                var teams = new List<Team>
                {
                    new Team { Name = "Mercedes", LogoUrl = "/images/teams/mercedes.png", PrimaryColor = "#00D2BE", SecondaryColor = "#222" },
                    new Team { Name = "Red Bull", LogoUrl = "/images/teams/redbull.png", PrimaryColor = "#1E41FF", SecondaryColor = "#FFF" },
                    new Team { Name = "Ferrari", LogoUrl = "/images/teams/ferrari.png", PrimaryColor = "#DC0000", SecondaryColor = "#FFF" },
                    new Team { Name = "McLaren", LogoUrl = "/images/teams/mclaren.png", PrimaryColor = "#FF8700", SecondaryColor = "#222" },
                    new Team { Name = "Alpine", LogoUrl = "/images/teams/alpine.png", PrimaryColor = "#0090FF", SecondaryColor = "#FFF" },
                    new Team { Name = "Aston Martin", LogoUrl = "/images/teams/astonmartin.png", PrimaryColor = "#006F62", SecondaryColor = "#FFF" }
                };
                await _context.Teams.AddRangeAsync(teams);
                await _context.SaveChangesAsync();
            }

            if (!await _context.Products.AnyAsync())
            {
                var categoryTshirt = await _context.Categories.FirstAsync(c => c.Name == "Tişört");
                var categoryHat = await _context.Categories.FirstAsync(c => c.Name == "Şapka");
                var mercedes = await _context.Teams.FirstAsync(t => t.Name == "Mercedes");
                var redbull = await _context.Teams.FirstAsync(t => t.Name == "Red Bull");
                var ferrari = await _context.Teams.FirstAsync(t => t.Name == "Ferrari");

                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Mercedes Takım Tişörtü 2024",
                        Description = "Resmi Mercedes-AMG Petronas Formula 1 Takım Tişörtü.",
                        Price = 1299.99m,
                        StockQuantity = 50,
                        CategoryId = categoryTshirt.Id,
                        TeamId = mercedes.Id,
                        ImageUrl = "/images/products/mercedes-tshirt.jpg",
                        TeamLogoUrl = mercedes.LogoUrl,
                        Specifications = "%100 pamuk, nefes alabilir kumaş.",
                        Material = "Pamuk",
                        Size = "M",
                        Color = "Siyah",
                        DriverName = "Lewis Hamilton",
                        Season = "2024",
                        IsLimitedEdition = false,
                        IsSigned = false
                    },
                    new Product
                    {
                        Name = "Red Bull Racing Şapka 2024",
                        Description = "Red Bull Racing resmi takım şapkası.",
                        Price = 699.99m,
                        StockQuantity = 30,
                        CategoryId = categoryHat.Id,
                        TeamId = redbull.Id,
                        ImageUrl = "/images/products/redbull-hat.jpg",
                        TeamLogoUrl = redbull.LogoUrl,
                        Specifications = "Ayarlanabilir, %100 polyester.",
                        Material = "Polyester",
                        Size = "Standart",
                        Color = "Lacivert",
                        DriverName = "Max Verstappen",
                        Season = "2024",
                        IsLimitedEdition = true,
                        IsSigned = false
                    },
                    new Product
                    {
                        Name = "Ferrari Takım Tişörtü 2024",
                        Description = "Scuderia Ferrari resmi takım tişörtü.",
                        Price = 1399.99m,
                        StockQuantity = 40,
                        CategoryId = categoryTshirt.Id,
                        TeamId = ferrari.Id,
                        ImageUrl = "/images/products/ferrari-tshirt.jpg",
                        TeamLogoUrl = ferrari.LogoUrl,
                        Specifications = "%100 pamuk, kırmızı renk.",
                        Material = "Pamuk",
                        Size = "L",
                        Color = "Kırmızı",
                        DriverName = "Charles Leclerc",
                        Season = "2024",
                        IsLimitedEdition = false,
                        IsSigned = false
                    }
                };
                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
            }
        }
    }
} 