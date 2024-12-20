using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VeilleApp.Model
{
    [Index(nameof(Guid), IsUnique = true)]
    public class Veille
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        [Required]
        public string Link { get; set; } = string.Empty;
        [Required]
        public string Publisher { get; set; } = string.Empty;
        public DateTime PublishTime { get; set; }
        [Required]
        public string Guid { get; set; }
    }
}