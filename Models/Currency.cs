using System.ComponentModel.DataAnnotations;

namespace digitalthinkers.Models
{
    public class Currency
    {
        public Currency()
        {
        }

        public Currency(int value, int count)
        {
            Value = value;
            Count = count;
        }

        [Key]
        public int Value { get; set; }

        public int Count { get; set; }
    }
}