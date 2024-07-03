
namespace ProductManagementWEB.Models
{
    public class Product
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Producer { get; set; }

        public int Quantity { get; set; }

        public List<string> Images { get; set; } = new List<string>();

        public bool IsFlagged { get; set; }
    }
}
