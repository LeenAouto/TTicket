using System.ComponentModel.DataAnnotations;

namespace TTicket.Models.DTOs
{
    public class ProductDto
    {
        [MaxLength(256)]
        public string? Name { get; set; }
    }
}
