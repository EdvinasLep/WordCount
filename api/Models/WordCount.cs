using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class WordCount
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string? FileName { get; set; }

        [Required]
        public string? Word { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public DateTime FileUploadTime { get; set; }
    }
}