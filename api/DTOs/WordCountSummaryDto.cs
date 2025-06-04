using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    public class WordCountSummaryDto
    {
        public string? Word { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}