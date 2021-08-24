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

            await UpdateMachineStockAsync(isWithdrawal: false, checkoutData.Inserted);

            if (result.ChangeValue == 0)
            {
                return result;
            }

            await CalculateChangeAsync(result);

            if (result.IsValid)
            {
                await UpdateMachineStockAsync(isWithdrawal: true, result.Change);
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
                Inserted = checkoutData.Inserted,
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
            // Sort available currency
            SortedDictionary<int, int> currencyAvailable = new SortedDictionary<int, int>(await currencyService.GetCurrentStockAsync());
            double remainingChange = checkoutData.ChangeValue;
            // Going backwards on sorted currency, starting with the greatest bill
            for (int i = currencyAvailable.Count - 1; i >= 0 && remainingChange > 0; --i)
            {
                KeyValuePair<int, int> currency = currencyAvailable.ElementAt(i);
                double value = double.Parse(currency.Key.ToString());

                // Skip currency if it is greater than the change needed or there is none left of this currency
                if (value > remainingChange || currency.Value == 0)
                {
                    continue;
                }

                // Count how many can be used of this bill or coin
                int currencyCount = (int)Math.Floor(remainingChange / value);
                var usedCount = Math.Min(currencyCount, currency.Value);

                // Refresh data
                checkoutData.Change.Add(currency.Key, usedCount);
                remainingChange -= currency.Key * usedCount;
            }

            // If the remaining change value is greater than zero, the machine cabbot give back
            if (remainingChange != 0)
            {
                checkoutData.IsValid = false;
                checkoutData.ErrorMessage = "The machine cannot give back change sufficiently";
            };
        }

        private async Task UpdateMachineStockAsync(bool isWithdrawal, Dictionary<int, int> change)
        {
            if (isWithdrawal)
            {
                change = change.ToDictionary(kvp => kvp.Key, kvp => kvp.Value * -1);
            }
            _ = await currencyService.UpdateStockAsync(change);
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