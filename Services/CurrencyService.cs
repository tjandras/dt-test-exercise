using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using digitalthinkers.Database;
using digitalthinkers.Interfaces;
using digitalthinkers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace digitalthinkers.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly CurrencyContext dbContext;

        private readonly ILogger<CurrencyService> logger;

        public CurrencyService(CurrencyContext dbContext, ILogger<CurrencyService> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<Dictionary<int, int>> GetCurrentStockAsync()
        {
            Dictionary<int, int> currentStock = ConvertCurrencyToDto(await GetCurrentStockFromDbAsync());
            return currentStock;
        }

        public async Task<Dictionary<int, int>> UpdateStockAsync(Dictionary<int, int> currencyUpdateData)
        {
            List<Currency> currentStock = await GetCurrentStockFromDbAsync();

            List<int> currencyToUpdate = currencyUpdateData.Select(kvp => kvp.Key).ToList();
            UpdateCurrencies(currencyUpdateData, currentStock, currencyToUpdate);

            if (currencyToUpdate.Any())
            {
                await AddNewCurrenciesAsync(currencyUpdateData, currentStock, currencyToUpdate);
            }

            await dbContext.SaveChangesAsync();

            LogChanges(currencyUpdateData);

            return ConvertCurrencyToDto(currentStock);
        }

        private async Task AddNewCurrenciesAsync(Dictionary<int, int> currencyUpdateData, List<Currency> currentStock, List<int> currencyToUpdate)
        {
            foreach (int bill in currencyToUpdate)
            {
                Currency newCurrency = new Currency(bill, currencyUpdateData[bill]);
                await dbContext.Currencies.AddAsync(newCurrency);
                currentStock.Add(newCurrency);
            }
        }

        private static void UpdateCurrencies(Dictionary<int, int> currencyUpdateData, List<Currency> currentStock, List<int> currencyToUpdate)
        {
            foreach (Currency currency in currentStock.Where(c => currencyToUpdate.Contains(c.Value)))
            {
                currency.Count += currencyUpdateData[currency.Value];
                currencyToUpdate.Remove(currency.Value);
            }
        }

        private async Task<List<Currency>> GetCurrentStockFromDbAsync()
        {
            return await dbContext.Currencies.ToListAsync();
        }

        private static Dictionary<int, int> ConvertCurrencyToDto(IEnumerable<Currency> currency)
        {
            return currency.ToDictionary(kvp => kvp.Value, kvp => kvp.Count);
        }

        private void LogChanges(Dictionary<int, int> currencyUpdateData)
        {
            var updateResult = String.Join(", ", currencyUpdateData.Select(c => $"{c.Value} pcs of {c.Key}").ToArray());
            logger.LogInformation($"The following bills and coind were loaded into/payed out from the machine: {updateResult}.");
        }
    }
}