namespace DotNetHomeWork.Infrastructure.Models
{
    public class ProductDeleteModel
    {
        public ProductDeleteModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
