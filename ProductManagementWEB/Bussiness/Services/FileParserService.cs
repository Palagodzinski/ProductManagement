using ProductManagementWEB.Models;
using System.Xml.Linq;

namespace ProductManagementWEB.Bussiness.Services
{
    public class FileParserService
    {
        public static List<Product> ParseThirdSupplierFiles(string filePath)
        {
            var products = new List<Product>();

            try
            {
                var doc = XDocument.Load($"Files/{filePath}");

                foreach (var productElement in doc.Descendants("produkt"))
                {
                    var images = new List<string>();

                    foreach (var imageElement in productElement.Descendants("zdjecie"))
                    {
                        var image = imageElement.Attribute("url").Value;

                        images.Add(image);
                    }

                    var product = new Product
                    {
                        Id = (string)productElement.Element("id"),
                        Name = (string)productElement.Element("nazwa"),
                        Description = (string)productElement.Element("dlugi_opis"),
                        Producer = (string)productElement.Element("kod_dostawcy"),
                        Quantity = int.Parse(productElement.Element("status").Value),
                        Images = images
                    };

                    products.Add(product);
                }

                return products;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static List<Product> ParseSecondSupplierFiles(string[] filePaths)
        {
            var products = new List<Product>();

            try
            {
                var doc1 = XDocument.Load($"Files/{filePaths[0]}");
                var doc2 = XDocument.Load($"Files/{filePaths[1]}");

                var products1 = doc1.Descendants("product")
                    .Select(x => new
                    {
                        Id = (string)x.Element("id"),
                        StockQuantity = x.Element("qty")
                    });

                var products2 = doc2.Descendants("product")
                    .Select(x => new
                    {
                        Id = (string)x.Element("id"),
                        Name = (string)x.Element("name"),
                        Description = (string)x.Element("desc"),
                        Images = x.Element("photos")
                    });

                var combinedProducts = from p1 in products1
                                       join p2 in products2 on p1.Id equals p2.Id
                                       select new XElement("product",
                                           new XAttribute("id", p1.Id),
                                           p1.StockQuantity,
                                           new XAttribute("desc", p2.Description),
                                           p2.Images,
                                           new XAttribute("name", p2.Name)
                                       );

                foreach (var productElement in combinedProducts)
                {
                    var images = new List<string>();

                    foreach (var imageElement in productElement.Descendants("photo"))
                    {
                        var image = imageElement.Value;

                        images.Add(image);
                    }

                    var product = new Product
                    {
                        Id = (string)productElement.Attribute("id").Value,
                        Name = (string)productElement.Attribute("name").Value,
                        Description = (string)productElement.Attribute("desc").Value,
                        Quantity = int.Parse(productElement.Element("qty").Value),
                        Images = images
                    };

                    products.Add(product);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return products;
        }

        public static List<Product> ParseFirstSupplierFiles(string[] filePaths)
        {
            var products = new List<Product>();

            try
            {
                var doc1 = XDocument.Load($"Files/{filePaths[0]}");
                var doc2 = XDocument.Load($"Files/{filePaths[1]}");

                var products1 = doc1.Descendants("product")
                        .Select(p => new
                        {
                            Id = (string)p.Attribute("id"),
                            Sizes = p.Element("sizes")
                        });

                var products2 = doc2.Descendants("product")
                                    .Select(p => new
                                    {
                                        Id = (string)p.Attribute("id"),
                                        Producer = p.Element("producer"),
                                        Description = p.Element("description"),
                                        Images = p.Element("images")
                                    });

                var combinedProducts = from p1 in products1
                                       join p2 in products2 on p1.Id equals p2.Id
                                       select new XElement("product",
                                           new XAttribute("id", p1.Id),
                                           p1.Sizes,
                                           p2.Producer,
                                           p2.Description,
                                           p2.Images
                                       );

                foreach (var productElement in combinedProducts)
                {
                    var images = new List<string>();
                    var description = productElement
                        .Descendants("long_desc")
                        .Where(e => (string)e.Attribute("{http://www.w3.org/XML/1998/namespace}lang") == "pol")
                        .Select(e => (string)e.Value)
                        .FirstOrDefault();

                    foreach (var imageElement in productElement.Descendants("image"))
                    {
                        var image = imageElement.Attribute("url").Value;

                        images.Add(image);
                    }

                    var product = new Product
                    {
                        Id = productElement.Attribute("id").Value,
                        Name = productElement.Descendants("name").FirstOrDefault()?.Value,
                        Description = description,
                        Producer = productElement.Descendants("producer").FirstOrDefault()?.Attribute("name").Value,
                        Quantity = int.Parse(productElement.Descendants("stock").FirstOrDefault()?.Attribute("quantity").Value),
                        Images = images
                    };

                    products.Add(product);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return products;
        }
    }
}