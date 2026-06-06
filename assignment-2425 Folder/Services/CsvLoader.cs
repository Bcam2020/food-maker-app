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
        /// Downloads a CSV file from a URL and parses it into a list of Recipe objects.
        /// Columns: Name,Ingredients,Instructions,ImageUrl
        /// Example row: "Banana Pancake","Banana | Egg","Mash banana with egg",""
        /// </summary>
        public static async Task<List<Recipe>> DownloadAndParseRecipesAsync(string csvUrl)
        {
            using var client = new HttpClient();
            string csvContent = await client.GetStringAsync(csvUrl);

            var lines = csvContent
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length < 2)
                return new List<Recipe>();

            // Skip header
            var dataLines = lines[1..];
            var recipes = new List<Recipe>();

            foreach (var line in dataLines)
            {
                var columns = line.Split(',');
                if (columns.Length < 3)
                    continue;

                string name = columns[0].Trim().Trim('"');
                string ingredientsString = columns[1].Trim().Trim('"');
                string instructions = columns[2].Trim().Trim('"');
                string imageUrl = columns.Length > 3 ? columns[3].Trim().Trim('"') : "";

                // Convert "Egg | Milk" into a list
                var ingredientsList = ingredientsString
                    .Split('|', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                recipes.Add(new Recipe
                {
                    Name = name,
                    Ingredients = ingredientsList,
                    Instructions = instructions,
                    ImageUrl = imageUrl
                });
            }

            return recipes;
        }
    }
}
