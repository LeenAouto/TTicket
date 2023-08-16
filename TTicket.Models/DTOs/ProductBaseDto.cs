using System.ComponentModel.DataAnnotations;

namespace TTicket.Models.DTOs
{
    public class ProductBaseDto
    {
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;
    }
}
