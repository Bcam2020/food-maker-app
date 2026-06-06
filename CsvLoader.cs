using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using assignment_2425.Models;

namespace assignment_2425.Services
{
    public static class CsvLoader
    {
        /// <summary>
        /// Downloads a CSV file from a URL and parses it into a List of Recipe objects.
        /// </summary>
        /// <param name="csvUrl">
        /// URL to a CSV containing columns: Name,Ingredients,Instructions,ImageUrl
        /// </param>
        public static async Task<List<Recipe>> DownloadAndParseRecipesAsync(string csvUrl)
        {
            using var client = new HttpClient();
            string csvContent = await client.GetStringAsync(csvUrl);

            var lines = csvContent
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // If there's only 1 or 0 lines, there's no real data beyond a header
            if (lines.Length < 2)
                return new List<Recipe>();

            // Skip the header (first line)
            var dataLines = lines[1..];
            var recipes = new List<Recipe>();

            foreach (var line in dataLines)
            {
                // Quick naive split by comma. For more robust CSV, consider a CSV parser library.
                var columns = line.Split(',');
                if (columns.Length < 3)
                    continue;

                string name = columns[0].Trim().Trim('"');
                string ingredientString = columns[1].Trim().Trim('"');
                string instructions = columns[2].Trim().Trim('"');
                string imageUrl = columns.Length > 3 ? columns[3].Trim().Trim('"') : "";

                // Convert the single string to a list (split by '|', for example)
                var ingredientList = ingredientString
                    .Split('|', StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => i.Trim())
                    .ToList();

                recipes.Add(new Recipe
                {
                    Name = name,
                    Ingredients = ingredientList,
                    Instructions = instructions,
                    ImageUrl = imageUrl
                });
            }

            return recipes;
        }
    }
}
