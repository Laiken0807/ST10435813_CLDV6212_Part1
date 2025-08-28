using LKcosmetics_CLDV6212_POE.Models;
using LKcosmetics_CLDV6212_POE.Services;
using Microsoft.AspNetCore.Mvc;

namespace LKcosmetics_CLDV6212_POE.Controllers
{
    public class ProductsController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly BlobStorageService _blobStorageService;

        public ProductsController(TableStorageService tableStorageService, BlobStorageService blobStorageService)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _tableStorageService.GetAllProductsAsync();

            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    product.ImageUrl = _blobStorageService.GetBlobSasUri(product.ImageUrl);
                }
            }

            return View(products);
        }

        [HttpGet]
        public IActionResult AddProduct() => View();

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, IFormFile imageFile)
        {
            product.PartitionKey = "Product";
            product.RowKey = Guid.NewGuid().ToString();

            if (imageFile != null && imageFile.Length > 0)
            {
                var blobUrl = await _blobStorageService.UploadFileAsync(imageFile);
                product.ImageUrl = blobUrl; 
            }

            await _tableStorageService.AddProductAsync(product);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string partitionKey, string rowKey, string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
                await _blobStorageService.DeleteFileAsync(imageUrl);

            await _tableStorageService.DeleteProductAsync(partitionKey, rowKey);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
            if (product == null) return NotFound();
            return View("EditProduct", product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (!ModelState.IsValid) return View(product);
            await _tableStorageService.UpdateProductAsync(product);
            return RedirectToAction("Index");
        }
    }
}
