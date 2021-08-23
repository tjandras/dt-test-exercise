using System.Collections.Generic;

namespace digitalthinkers.Models
{
    public class CheckoutData
    {
        public CheckoutData()
        {
            Change = new Dictionary<int, int>();
        }

        public double InsertedAmount { get; set; }

        public bool IsValid { get; set; }

        public double ChangeValue { get; set; }

        public Dictionary<int, int> Change { get; set; }

        public string ErrorMessage { get; set; }
    }
}