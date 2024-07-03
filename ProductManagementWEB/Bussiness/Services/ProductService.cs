using Newtonsoft.Json;
using ProductManagementWEB.Models;
using System.Collections.Generic;

namespace ProductManagementWEB.Bussiness.Services
{
    public class ProductService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;       
        private const string SessionKey = "Products";

        public ProductService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<Product> GetAllProducts()
        {
            return GetSessionProducts();
        }

        public Product? GetProductById(string id)
        {
            var products = GetSessionProducts();
            return products.FirstOrDefault(p => p.Id == id);
        }

        public void FlagProduct(string id)
        {
            var products = GetSessionProducts();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                product.IsFlagged = true;
                SaveSessionProducts(products);
            }
        }
        private List<Product> LoadProducts(string[] filePaths)
        {
            var products = new List<Product>();
            
            products.AddRange(FileParserService.ParseFirstSupplierFiles([filePaths[0], filePaths[1]]));
            products.AddRange(FileParserService.ParseSecondSupplierFiles([filePaths[2], filePaths[3]]));
            products.AddRange(FileParserService.ParseThirdSupplierFiles(filePaths[4]));
            
            return products;
        }

        private void SaveSessionProducts(List<Product> products)
        {
            _httpContextAccessor.HttpContext.Session.SetString(SessionKey, JsonConvert.SerializeObject(products));
        }

        private List<Product> GetSessionProducts()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var productList = session.GetString(SessionKey);

            if (productList == null)
            {                
                string[] filePaths = { "dostawca1plik1.xml", "dostawca1plik2.xml", "dostawca2plik1.xml", "dostawca2plik2.xml", "dostawca3plik1.xml" };
                var products = LoadProducts(filePaths);
                session.SetString(SessionKey, JsonConvert.SerializeObject(products));
                return products;
            }
            else
            {
                return JsonConvert.DeserializeObject<List<Product>>(productList);
            }
        }
    }
}
