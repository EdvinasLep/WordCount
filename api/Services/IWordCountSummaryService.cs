using api.DTOs;

namespace api.Services
{
    public interface IWordCountSummaryService
    {
        Task<List<WordCountSummaryDto>> AnalyzeFileAsync(IFormFile file);
        Task<List<WordCountSummaryDto>> GetWordCountSummaryAsync(string fileName);
    }
}