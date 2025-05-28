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
            // Seed Categories
            if (!await _context.Categories.AnyAsync(c => c.Name == "Tişört"))
            {
                await _context.Categories.AddAsync(new Category { Name = "Tişört", Description = "F1 Takım Tişörtleri", ImageUrl = "/images/categories/tshirt.jpg" });
            }
            if (!await _context.Categories.AnyAsync(c => c.Name == "Şapka"))
            {
                await _context.Categories.AddAsync(new Category { Name = "Şapka", Description = "F1 Takım Şapkaları", ImageUrl = "/images/categories/hat.jpg" });
            }
            await _context.SaveChangesAsync();

            // Seed Teams
            if (!await _context.Teams.AnyAsync(t => t.Name == "Mercedes"))
            {
                await _context.Teams.AddAsync(new Team { Name = "Mercedes", LogoUrl = "/images/teams/mercedes.png" });
            }
            if (!await _context.Teams.AnyAsync(t => t.Name == "Red Bull"))
            {
                await _context.Teams.AddAsync(new Team { Name = "Red Bull", LogoUrl = "/images/teams/redbull.png" });
            }
            if (!await _context.Teams.AnyAsync(t => t.Name == "Ferrari"))
            {
                await _context.Teams.AddAsync(new Team { Name = "Ferrari", LogoUrl = "/images/teams/ferrari.png" });
            }
            if (!await _context.Teams.AnyAsync(t => t.Name == "McLaren"))
            {
                await _context.Teams.AddAsync(new Team { Name = "McLaren", LogoUrl = "/images/teams/mclaren.png" });
            }
            if (!await _context.Teams.AnyAsync(t => t.Name == "Alpine"))
            {
                await _context.Teams.AddAsync(new Team { Name = "Alpine", LogoUrl = "/images/teams/alpine.png" });
            }
            if (!await _context.Teams.AnyAsync(t => t.Name == "Aston Martin"))
            {
                await _context.Teams.AddAsync(new Team { Name = "Aston Martin", LogoUrl = "/images/teams/astonmartin.png" });
            }
            await _context.SaveChangesAsync();

            // Get all categories and teams for use throughout the method
            var categoryTshirt = await _context.Categories.FirstAsync(c => c.Name == "Tişört");
            var categoryHat = await _context.Categories.FirstAsync(c => c.Name == "Şapka");
            var mercedes = await _context.Teams.FirstAsync(t => t.Name == "Mercedes");
            var redbull = await _context.Teams.FirstAsync(t => t.Name == "Red Bull");
            var ferrari = await _context.Teams.FirstAsync(t => t.Name == "Ferrari");
            var mclaren = await _context.Teams.FirstAsync(t => t.Name == "McLaren");
            var alpine = await _context.Teams.FirstAsync(t => t.Name == "Alpine");
            var aston = await _context.Teams.FirstAsync(t => t.Name == "Aston Martin");

            // Seed initial products if they don't exist
            if (!await _context.Products.AnyAsync())
            {
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

            // Add new categories if they don't exist
            if (!await _context.Categories.AnyAsync(c => c.Name == "Model Araba"))
            {
                await _context.Categories.AddAsync(new Category { Name = "Model Araba", Description = "F1 Model Arabalar", ImageUrl = "/images/categories/modelcar.jpg" });
            }
            if (!await _context.Categories.AnyAsync(c => c.Name == "Elektronik"))
            {
                await _context.Categories.AddAsync(new Category { Name = "Elektronik", Description = "F1 Elektronik Ürünler", ImageUrl = "/images/categories/electronics.jpg" });
            }
            if (!await _context.Categories.AnyAsync(c => c.Name == "Poster"))
            {
                await _context.Categories.AddAsync(new Category { Name = "Poster", Description = "F1 Posterleri", ImageUrl = "/images/categories/poster.jpg" });
            }
            if (!await _context.Categories.AnyAsync(c => c.Name == "Anahtarlık"))
            {
                await _context.Categories.AddAsync(new Category { Name = "Anahtarlık", Description = "F1 Anahtarlıklar", ImageUrl = "/images/categories/keychain.jpg" });
            }
            await _context.SaveChangesAsync();

            // Get new categories
            var categoryModelCar = await _context.Categories.FirstAsync(c => c.Name == "Model Araba");
            var categoryElectronics = await _context.Categories.FirstAsync(c => c.Name == "Elektronik");
            var categoryPoster = await _context.Categories.FirstAsync(c => c.Name == "Poster");
            var categoryKeychain = await _context.Categories.FirstAsync(c => c.Name == "Anahtarlık");

            // Add new products
            var newProducts = new List<Product>
            {
                new Product
                {
                    Name = "McLaren Model Araba 2024",
                    Description = "McLaren MCL38 1:18 ölçekli model araba. Formula 1 koleksiyonunuz için mükemmel bir parça.",
                    Price = 2499.99m,
                    StockQuantity = 15,
                    CategoryId = categoryModelCar.Id,
                    TeamId = mclaren.Id,
                    ImageUrl = "/images/products/mclaren-modelcar.jpg",
                    TeamLogoUrl = mclaren.LogoUrl,
                    Specifications = "1:18 ölçek, metal gövde, detaylı iç tasarım.",
                    Material = "Metal",
                    Size = "1:18",
                    Color = "Turuncu",
                    Season = "2024",
                    IsLimitedEdition = true,
                    IsSigned = false
                },
                new Product
                {
                    Name = "Alpine Bluetooth Kulaklık",
                    Description = "Alpine F1 Team logolu kablosuz bluetooth kulaklık. Yüksek ses kalitesi ve uzun pil ömrü.",
                    Price = 1799.99m,
                    StockQuantity = 25,
                    CategoryId = categoryElectronics.Id,
                    TeamId = alpine.Id,
                    ImageUrl = "/images/products/alpine-headphones.jpg",
                    TeamLogoUrl = alpine.LogoUrl,
                    Specifications = "Bluetooth 5.0, 20 saat pil, mikrofon.",
                    Material = "Plastik",
                    Color = "Mavi",
                    Season = "2024",
                    IsLimitedEdition = false,
                    IsSigned = false
                },
                new Product
                {
                    Name = "Aston Martin F1 Poster",
                    Description = "Aston Martin Aramco Cognizant F1 Team 2024 sezonu özel posteri. 50x70 cm, yüksek kaliteli baskı.",
                    Price = 299.99m,
                    StockQuantity = 40,
                    CategoryId = categoryPoster.Id,
                    TeamId = aston.Id,
                    ImageUrl = "/images/products/aston-poster.jpg",
                    TeamLogoUrl = aston.LogoUrl,
                    Specifications = "50x70 cm, mat kağıt, çerçevesiz.",
                    Material = "Kağıt",
                    Size = "50x70 cm",
                    Color = "Yeşil",
                    Season = "2024",
                    IsLimitedEdition = false,
                    IsSigned = false
                },
                new Product
                {
                    Name = "Ferrari Anahtarlık",
                    Description = "Scuderia Ferrari logolu metal anahtarlık. Formula 1 tutkunları için şık bir aksesuar.",
                    Price = 149.99m,
                    StockQuantity = 100,
                    CategoryId = categoryKeychain.Id,
                    TeamId = ferrari.Id,
                    ImageUrl = "/images/products/ferrari-keychain.jpg",
                    TeamLogoUrl = ferrari.LogoUrl,
                    Specifications = "Metal, kırmızı detaylı.",
                    Material = "Metal",
                    Color = "Kırmızı",
                    Season = "2024",
                    IsLimitedEdition = false,
                    IsSigned = false
                },
                new Product
                {
                    Name = "Red Bull F1 Aksesuar Seti",
                    Description = "Red Bull Racing takımına özel anahtarlık, bileklik ve sticker seti.",
                    Price = 399.99m,
                    StockQuantity = 60,
                    CategoryId = categoryKeychain.Id,
                    TeamId = redbull.Id,
                    ImageUrl = "/images/products/redbull-accessory.jpg",
                    TeamLogoUrl = redbull.LogoUrl,
                    Specifications = "3'lü set: anahtarlık, bileklik, sticker.",
                    Material = "Plastik/Metal",
                    Color = "Lacivert",
                    Season = "2024",
                    IsLimitedEdition = false,
                    IsSigned = false
                }
            };
            
            // Add products if they don't exist
            if (!await _context.Products.AnyAsync(p => p.Name == "McLaren Model Araba 2024"))
                await _context.Products.AddAsync(newProducts[0]);
            if (!await _context.Products.AnyAsync(p => p.Name == "Alpine Bluetooth Kulaklık"))
                await _context.Products.AddAsync(newProducts[1]);
            if (!await _context.Products.AnyAsync(p => p.Name == "Aston Martin F1 Poster"))
                await _context.Products.AddAsync(newProducts[2]);
            if (!await _context.Products.AnyAsync(p => p.Name == "Ferrari Anahtarlık"))
                await _context.Products.AddAsync(newProducts[3]);
            if (!await _context.Products.AnyAsync(p => p.Name == "Red Bull F1 Aksesuar Seti"))
                await _context.Products.AddAsync(newProducts[4]);
            
            await _context.SaveChangesAsync();
        }
    }
} 