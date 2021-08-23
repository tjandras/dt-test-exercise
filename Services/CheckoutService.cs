using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using digitalthinkers.Interfaces;
using digitalthinkers.Models;
using Microsoft.Extensions.Logging;

namespace digitalthinkers.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICurrencyService currencyService;
        private readonly ILogger<CheckoutService> logger;

        public CheckoutService(ICurrencyService currencyService, ILogger<CheckoutService> logger)
        {
            this.currencyService = currencyService;
            this.logger = logger;
        }

        public async Task<CheckoutData> CheckoutUserAsync(CustomerCheckout checkoutData)
        {
            CheckoutData result = ValidateData(checkoutData);

            if (!result.IsValid)
            {
                return result;
            }

            if (result.ChangeValue == 0)
            {
                return result;
            }

            await CalculateChangeAsync(result);

            if (result.IsValid)
            {
                await UpdateMachineStockAsync(result.Change);
            }

            return result;
        }

        private CheckoutData ValidateData(CustomerCheckout checkoutData)
        {
            if (checkoutData.Price <= 0)
            {
                return GetErrorResult("Invalid price speciefied");
            }

            if (!checkoutData.Inserted.Any() || checkoutData.Inserted.Any(c => c.Value < 0))
            {
                return GetErrorResult("No bills or coins inserted or negative count is provided");
            }

            var insertedValue = CalculateInsertedValue(checkoutData);
            if (insertedValue < checkoutData.Price)
            {
                return GetErrorResult("Inserted bills and coins are not sufficient");
            }

            var result = new CheckoutData
            {
                IsValid = true,
                InsertedAmount = insertedValue,
                ChangeValue = insertedValue - checkoutData.Price
            };

            return result;
        }

        private double CalculateInsertedValue(CustomerCheckout checkoutData)
        {
            double sum = 0;
            foreach (KeyValuePair<int, int> currency in checkoutData.Inserted)
            {
                sum += currency.Key * currency.Value;
            }

            return sum;
        }

        private async Task CalculateChangeAsync(CheckoutData checkoutData)
        {
            SortedDictionary<int, int> currencyAvailable = new SortedDictionary<int, int>(await currencyService.GetCurrentStockAsync());
            double remainingChange = checkoutData.ChangeValue;
            for (int i = currencyAvailable.Count - 1; i >= 0 && remainingChange > 0; --i)
            {
                KeyValuePair<int, int> currency = currencyAvailable.ElementAt(i);
                double value = double.Parse(currency.Key.ToString());
                if (value > remainingChange || currency.Value == 0)
                {
                    continue;
                }
                int currencyCount = (int)Math.Floor(remainingChange / value);
                var usedCount = Math.Min(currencyCount, currency.Value);
                checkoutData.Change.Add(currency.Key, usedCount);
                remainingChange -= currency.Key * usedCount;
            }

            if (remainingChange != 0)
            {
                checkoutData.IsValid = false;
                checkoutData.ErrorMessage = "The machine cannot give back change sufficiently";
            };
        }

        private async Task UpdateMachineStockAsync(Dictionary<int, int> change)
        {
            var updateData = change.ToDictionary(kvp => kvp.Key, kvp => kvp.Value * -1);
            _ = await currencyService.UpdateStockAsync(updateData);
        }

        private static CheckoutData GetErrorResult(string message)
        {
            return new CheckoutData
            {
                IsValid = false,
                ErrorMessage = message
            };
        }
    }
}