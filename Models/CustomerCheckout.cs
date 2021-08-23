using System.Collections.Generic;

namespace digitalthinkers.Models
{
    public class CustomerCheckout
    {
        public Dictionary<int, int> Inserted { get; set; }

        public int Price { get; set; }
    }
}