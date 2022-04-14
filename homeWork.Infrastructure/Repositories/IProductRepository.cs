using System.Threading.Tasks;
using DotNetHomeWork.Infrastructure.Models;

namespace DotNetHomeWork.Infrastructure.Repositories
{
    public interface IProductRepository
    {
        Task<ProductStored> AddOrUpdateProductAsync(ProductAddModel product);
        Task DeleteProductAsync(ProductDeleteModel product);
        Task<ProductStored> GetProductAsync(string name);
    }
}