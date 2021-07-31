using System;

namespace puchealth.Models
{
    public class Bookmark
    {
        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}