using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LU2_WebApi.Models
{
    public class Environment2D
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int MaxX { get; set; }
        public int MaxY { get; set; }

        [ForeignKey("UserId")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

    }
}
