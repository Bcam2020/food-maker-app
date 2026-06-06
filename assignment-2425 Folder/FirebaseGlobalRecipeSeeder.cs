using System;
using System.Threading.Tasks;
using assignment_2425.Services;
using assignment_2425.Models;

namespace assignment_2425
{
    public static class FirebaseGlobalRecipeSeeder
    {
        /// <summary>
        /// Reads sample_recipes.csv from a public Storage URL, then seeds the /Recipes collection in Firestore.
        /// </summary>
        public static async Task SeedGlobalRecipesFromCsvAsync(string idToken, string csvUrl)
        {
            try
            {
                var recipes = await CsvLoader.DownloadAndParseRecipesAsync(csvUrl);
                Console.WriteLine($"Parsed {recipes.Count} recipes from CSV.");

                // For each recipe, store it in Firestore's /Recipes
                foreach (var recipe in recipes)
                {
                    string recipeId = Guid.NewGuid().ToString("N");
                    try
                    {
                        await FirestoreService.SetGlobalRecipeAsync(recipeId, recipe, idToken);
                        Console.WriteLine($"Seeded global recipe: {recipe.Name}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error seeding recipe {recipe.Name}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading CSV: {ex.Message}");
            }
        }
    }
}
