using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitStore.Data;
using PitStore.Models;
using PitStore.Services;

namespace PitStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly IImageService _imageService;

        public ProductController(ApplicationDbContext context, ICartService cartService, IProductService productService, IImageService imageService)
        {
            _context = context;
            _cartService = cartService;
            _productService = productService;
            _imageService = imageService;
        }

        public async Task<IActionResult> Index(string category, string team, string searchString, string sortOrder)
        {
            var products = await _productService.GetAllProductsAsync();

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category != null && p.Category.Name == category);
            }

            if (!string.IsNullOrEmpty(team))
            {
                products = products.Where(p => p.Team != null && p.Team.Name == team);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                             p.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Teams = await _context.Teams.ToListAsync();
            ViewBag.Brands = await _context.Brands.ToListAsync();
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentTeam = team;
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentSort = sortOrder;

            return View(products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            // İlgili ürünleri getir
            ViewBag.RelatedProducts = await _context.Products
                .Where(p => p.Category != null && p.Category == product.Category && p.Id != product.Id)
                .Take(4)
                .ToListAsync();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                IsVerifiedPurchase = true // You might want to check if the user has actually purchased the product
            };

            _context.Reviews.Add(review);

            // Update product rating
            var updatedProduct = await _productService.GetProductByIdAsync(productId);
            if (updatedProduct != null)
            {
                product.Reviews = updatedProduct.Reviews;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = productId });
        }

        [HttpPost]
        public async Task<IActionResult> Rate(int productId, int rating)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Json(new { success = false, message = "Please log in to rate products." });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not found." });
            }

            var product = await _context.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            var existingReview = product.Reviews
                .FirstOrDefault(r => r.UserId == userId);

            if (existingReview != null)
            {
                existingReview.Rating = rating;
            }
            else
            {
                var review = new Review
                {
                    ProductId = productId,
                    UserId = userId,
                    Rating = rating,
                    Comment = string.Empty
                };

                _context.Reviews.Add(review);
            }

            // Update product rating
            var updatedProduct = await _productService.GetProductByIdAsync(productId);
            if (updatedProduct != null)
            {
                product.Reviews = updatedProduct.Reviews;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, averageRating = product.AverageRating });
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            await _cartService.AddToCartAsync(userId, productId, quantity);
            return RedirectToAction("Index", "Cart");
        }

        // Admin kontrolü
        private bool IsAdmin()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return User.Identity?.IsAuthenticated == true && 
                   !string.IsNullOrEmpty(userId) && 
                   _context.Users.Any(u => u.Id == userId && u.IsAdmin);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            if (!IsAdmin()) return Unauthorized();
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!IsAdmin()) return Unauthorized();
            if (ModelState.IsValid)
            {
                try
                {
                    // Ana resmi yükle
                    if (product.ImageFile != null)
                    {
                        product.ImageUrl = await _imageService.UploadImageAsync(product.ImageFile, "products");
                    }

                    // Ek resimleri yükle
                    if (product.AdditionalImage1File != null)
                    {
                        product.AdditionalImage1Url = await _imageService.UploadImageAsync(product.AdditionalImage1File, "products");
                    }

                    if (product.AdditionalImage2File != null)
                    {
                        product.AdditionalImage2Url = await _imageService.UploadImageAsync(product.AdditionalImage2File, "products");
                    }

                    if (product.AdditionalImage3File != null)
                    {
                        product.AdditionalImage3Url = await _imageService.UploadImageAsync(product.AdditionalImage3File, "products");
                    }

                    await _productService.CreateProductAsync(product);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ürün oluşturulurken bir hata oluştu: " + ex.Message);
                }
            }
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return Unauthorized();
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (!IsAdmin()) return Unauthorized();
            if (id != product.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _productService.GetProductByIdAsync(id);
                    if (existingProduct == null) return NotFound();

                    // Ana resmi güncelle
                    if (product.ImageFile != null)
                    {
                        if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                        {
                            await _imageService.DeleteImageAsync(existingProduct.ImageUrl);
                        }
                        product.ImageUrl = await _imageService.UploadImageAsync(product.ImageFile, "products");
                    }
                    else
                    {
                        product.ImageUrl = existingProduct.ImageUrl;
                    }

                    // Ek resimleri güncelle
                    if (product.AdditionalImage1File != null)
                    {
                        if (!string.IsNullOrEmpty(existingProduct.AdditionalImage1Url))
                        {
                            await _imageService.DeleteImageAsync(existingProduct.AdditionalImage1Url);
                        }
                        product.AdditionalImage1Url = await _imageService.UploadImageAsync(product.AdditionalImage1File, "products");
                    }
                    else
                    {
                        product.AdditionalImage1Url = existingProduct.AdditionalImage1Url;
                    }

                    if (product.AdditionalImage2File != null)
                    {
                        if (!string.IsNullOrEmpty(existingProduct.AdditionalImage2Url))
                        {
                            await _imageService.DeleteImageAsync(existingProduct.AdditionalImage2Url);
                        }
                        product.AdditionalImage2Url = await _imageService.UploadImageAsync(product.AdditionalImage2File, "products");
                    }
                    else
                    {
                        product.AdditionalImage2Url = existingProduct.AdditionalImage2Url;
                    }

                    if (product.AdditionalImage3File != null)
                    {
                        if (!string.IsNullOrEmpty(existingProduct.AdditionalImage3Url))
                        {
                            await _imageService.DeleteImageAsync(existingProduct.AdditionalImage3Url);
                        }
                        product.AdditionalImage3Url = await _imageService.UploadImageAsync(product.AdditionalImage3File, "products");
                    }
                    else
                    {
                        product.AdditionalImage3Url = existingProduct.AdditionalImage3Url;
                    }

                    await _productService.UpdateProductAsync(id, product);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ürün güncellenirken bir hata oluştu: " + ex.Message);
                }
            }
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin()) return Unauthorized();
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return Unauthorized();
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            try
            {
                // Ürün resimlerini sil
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    _imageService.DeleteImage(product.ImageUrl);
                }
                if (!string.IsNullOrEmpty(product.AdditionalImage1Url))
                {
                    _imageService.DeleteImage(product.AdditionalImage1Url);
                }
                if (!string.IsNullOrEmpty(product.AdditionalImage2Url))
                {
                    _imageService.DeleteImage(product.AdditionalImage2Url);
                }
                if (!string.IsNullOrEmpty(product.AdditionalImage3Url))
                {
                    _imageService.DeleteImage(product.AdditionalImage3Url);
                }

                await _productService.DeleteProductAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ürün silinirken bir hata oluştu: " + ex.Message);
                return View(product);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int productId, string imageType)
        {
            if (!IsAdmin()) return Unauthorized();
            
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null) return NotFound();

            try
            {
                switch (imageType.ToLower())
                {
                    case "main":
                        if (!string.IsNullOrEmpty(product.ImageUrl))
                        {
                            await _imageService.DeleteImageAsync(product.ImageUrl);
                            product.ImageUrl = string.Empty;
                        }
                        break;
                    case "additional1":
                        if (!string.IsNullOrEmpty(product.AdditionalImage1Url))
                        {
                            await _imageService.DeleteImageAsync(product.AdditionalImage1Url);
                            product.AdditionalImage1Url = string.Empty;
                        }
                        break;
                    case "additional2":
                        if (!string.IsNullOrEmpty(product.AdditionalImage2Url))
                        {
                            await _imageService.DeleteImageAsync(product.AdditionalImage2Url);
                            product.AdditionalImage2Url = string.Empty;
                        }
                        break;
                    case "additional3":
                        if (!string.IsNullOrEmpty(product.AdditionalImage3Url))
                        {
                            await _imageService.DeleteImageAsync(product.AdditionalImage3Url);
                            product.AdditionalImage3Url = string.Empty;
                        }
                        break;
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSimilarProducts(int productId, int categoryId)
        {
            var similarProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Team)
                .Where(p => p.Id != productId && p.CategoryId == categoryId)
                .Take(4)
                .ToListAsync();

            return PartialView("_SimilarProducts", similarProducts);
        }
    }
} 