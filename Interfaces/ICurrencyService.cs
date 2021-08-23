using System.Collections.Generic;
using System.Threading.Tasks;
using digitalthinkers.Models;

namespace digitalthinkers.Interfaces
{
    public interface ICurrencyService
    {
        Task<Dictionary<int, int>> GetCurrentStockAsync();

        Task<Dictionary<int, int>> UpdateStockAsync(Dictionary<int, int> currencyUpdateData);
    }
}