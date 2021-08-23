using System.Threading.Tasks;
using digitalthinkers.Models;

namespace digitalthinkers.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutData> CheckoutUserAsync(CustomerCheckout checkoutData);
    }
}