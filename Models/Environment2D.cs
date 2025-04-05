using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LU2_WebApi.Models
{
    public class Environment2D
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Verwijzing naar de eigenaar (de user die is ingelogd)
        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
