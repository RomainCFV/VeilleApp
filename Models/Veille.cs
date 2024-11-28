using System.ComponentModel.DataAnnotations;

namespace VeilleApp.Model
{
    public class Veille
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime PublishTime { get; set; }
    }
}