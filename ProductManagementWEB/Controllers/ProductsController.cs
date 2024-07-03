using Microsoft.AspNetCore.Mvc;
using ProductManagementWEB.Bussiness.Services;

namespace ProductManagementWEB.Controllers
{
    public class ProductsController : Controller
    {
        private ProductService productService;

        public ProductsController(IHttpContextAccessor httpContextAccessor)
        {
            productService = new ProductService(httpContextAccessor);            
        }

        public IActionResult Index()
        {
            var products = productService.GetAllProducts();
            return View(products);
        }

        public IActionResult Details(string id)
        {
            var product = productService.GetProductById(id);
            if (product is null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult FlagProduct(string id)
        {
            productService.FlagProduct(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SaveFlags()
        {
            var flaggedProducts = productService.GetAllProducts().Where(p => p.IsFlagged).ToList();
            var numberOfFlaggedProducts = flaggedProducts.Count;   

            TempData["SuccessMessage"] = $"Zapisano {numberOfFlaggedProducts} oflagowanych produktów.";

            return RedirectToAction("Index");
        }
    }
}
