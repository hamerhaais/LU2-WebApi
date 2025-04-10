using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LU2_WebApi.Models
{
    public class Environment2D
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int MaxHeight;
        public int MaxLength;

        [Required]
        [ForeignKey("UserId")]
        public string UserId { get; set; } = string.Empty;
    }
}
