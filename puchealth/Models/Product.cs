using System;

namespace puchealth.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public Uri Image { get; set; } = null!;

        public decimal Price { get; set; }

        public double? ReviewScore { get; set; } = null!;
    }
}