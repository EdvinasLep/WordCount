using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs;
using api.Models;
using api.Repositories;
using System.Text.RegularExpressions;

namespace api.Services
{
    public class WordCountSummaryService : IWordCountSummaryService
    {
        private readonly IWordCountRepository _wordCountRepository;

        public WordCountSummaryService(IWordCountRepository wordCountRepository)
        {
            _wordCountRepository = wordCountRepository;
        }

        public async Task<List<WordCountSummaryDto>> AnalyzeFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null");
            }

            var fileName = file.FileName;
            
            await _wordCountRepository.DeleteByFileNameAsync(fileName);

            string content;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                content = await reader.ReadToEndAsync();
            }

            var wordCounts = CountWords(content);

            var wordCountEntities = wordCounts.Select(wc => new WordCount
            {
                Id = Guid.NewGuid(),
                Word = wc.Key,
                Count = wc.Value,
                FileName = fileName,
                FileUploadTime = DateTime.UtcNow
            }).ToList();

            await _wordCountRepository.CreateAsync(wordCountEntities, fileName);

            return wordCounts.Select(wc => new WordCountSummaryDto
            {
                Word = wc.Key,
                Count = wc.Value
            }).OrderByDescending(x => x.Count).ToList();
        }

        public async Task<List<WordCountSummaryDto>> GetWordCountSummaryAsync(string fileName)
        {
            var allWordCounts = await _wordCountRepository.GetAllAsync();
            
            var fileWordCounts = allWordCounts
                .Where(wc => wc.FileName.ToLower() == fileName.ToLower())
                .GroupBy(wc => wc.Word.ToLower())
                .Select(g => new WordCountSummaryDto
                {
                    Word = g.First().Word,
                    Count = g.Sum(x => x.Count)
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            return fileWordCounts;
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