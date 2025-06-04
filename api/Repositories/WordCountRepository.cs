using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class WordCountRepository : IWordCountRepository
    {
        private readonly WordCountContext _context;

        public WordCountRepository(WordCountContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(List<WordCount> wordCount, string fileName)
        {
            foreach (var wc in wordCount)
            {
                wc.FileName = fileName;
                wc.FileUploadTime = DateTime.UtcNow;
                _context.WordCounts.Add(wc);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<WordCount>> GetAllAsync()
        {
            return await _context.WordCounts.ToListAsync();
        }

        public async Task DeleteByFileNameAsync(string fileName)
        {
            var wordCounts = await _context.WordCounts
                .Where(wc => wc.FileName.ToLower() == fileName.ToLower())
                .ToListAsync();

            if (wordCounts.Any())
            {
                _context.WordCounts.RemoveRange(wordCounts);
                await _context.SaveChangesAsync();
            }
        }
    }
}