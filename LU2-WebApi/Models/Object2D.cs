using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LU2_WebApi.Models
{
    public class Object2D
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public float X { get; set; }

        [Required]
        public float Y { get; set; }

        [Required]
        public int EnvironmentId { get; set; }

        [ForeignKey("EnvironmentId")]
        public Environment2D? Environment { get; set; }
    }
}
