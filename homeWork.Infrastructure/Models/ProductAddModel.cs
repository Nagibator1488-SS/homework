namespace DotNetHomeWork.Infrastructure.Models
{
    public class ProductAddModel
    {
        public string Name { get; set; }
        public int Price { get; set; }

        public ProductAddModel(string name, int price)
        {
            Name = name;
            Price = price;
        }
    }
}
