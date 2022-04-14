using System.Threading.Tasks;
using DotNetHomeWork.Core.Models;

namespace DotNetHomeWork.Core.Interfaces
{
    public interface IProductService
    {
        Task<Product> AddProductAsync(string name, int price);
        Task<Product> GetProductAsync(string name);
        Task DeleteProductAsync(string name);
        Task<Product> UpdateProductPriceAsync(string name, int newPrice);
        Task<bool> TryBuyProductAsync(string name, int moneyAmount);
    }
}