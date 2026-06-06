using SQLite;
using assignment_2425.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace assignment_2425.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Recipe>().Wait();
        }

        // Add a new recipe
        public Task<int> AddRecipeAsync(Recipe recipe)
        {
            return _database.InsertAsync(recipe);
        }

        // Get all recipes
        public Task<List<Recipe>> GetRecipesAsync()
        {
            return _database.Table<Recipe>().ToListAsync();
        }

        // Search for recipes based on ingredients
        public Task<List<Recipe>> GetRecipesByIngredientAsync(string ingredient)
        {
            return _database.Table<Recipe>()
                .Where(r => r.Ingredients.Contains(ingredient))
                .ToListAsync();
        }
    }
}
