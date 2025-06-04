using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs;
using System.Text.RegularExpressions;

namespace api.Services
{
    public class WordCountSummaryService : IWordCountSummaryService
    {
        public async Task<List<WordCountSummaryDto>> AnalyzeFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null");
            }
            
            string content;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                content = await reader.ReadToEndAsync();
            }

            var wordCounts = CountWords(content);

            return wordCounts.Select(wc => new WordCountSummaryDto
            {
                Word = wc.Key,
                Count = wc.Value
            }).OrderByDescending(x => x.Count).ToList();
        }

        private Dictionary<string, int> CountWords(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return new Dictionary<string, int>();
            }

            var words = Regex.Split(content.ToLower(), @"\W+")
                           .Where(word => !string.IsNullOrEmpty(word))
                           .Where(word => word.Length > 1)
                           .ToList();

            var wordCount = new Dictionary<string, int>();
            foreach (var word in words)
            {
                if (wordCount.ContainsKey(word))
                {
                    wordCount[word]++;
                }
                else
                {
                    wordCount[word] = 1;
                }
            }

            return wordCount;
        }
    }
}