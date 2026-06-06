namespace assignment_2425.Services
{
    public class IngredientPairingService
    {
        // A simple dictionary for ingredient pairings.
        // This is case-insensitive and can be expanded as needed.
        private readonly Dictionary<string, List<string>> _pairingDictionary = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "tomato", new List<string> { "basil", "mozzarella", "olive oil", "garlic" } },
            { "chicken", new List<string> { "rosemary", "thyme", "lemon", "garlic" } },
            { "beef", new List<string> { "black pepper", "garlic", "thyme", "rosemary" } },
            { "avocado", new List<string> { "lime", "tomato", "cilantro", "red onion" } },
            { "spinach", new List<string> { "feta", "walnuts", "dried cranberries", "balsamic vinegar" } },
            { "pasta", new List<string> { "parmesan", "olive oil", "basil", "garlic" } },
            // ... add more curated pairings as needed.
        };

        /// <summary>
        /// Returns a list of recommended complementary ingredients for a given ingredient.
        /// </summary>
        public List<string> GetPairings(string ingredient)
        {
            if (string.IsNullOrWhiteSpace(ingredient))
                return new List<string>();

            if (_pairingDictionary.TryGetValue(ingredient, out var pairings))
                return pairings;

            // Optionally, return a default suggestion or an empty list if the ingredient isn't found.
            return new List<string>();
        }

        /// <summary>
        /// For a list of ingredients, returns a mapping of each ingredient to its recommended pairings.
        /// </summary>
        public Dictionary<string, List<string>> GetPairingsForIngredients(IEnumerable<string> ingredients)
        {
            var result = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var ingredient in ingredients)
            {
                var pairings = GetPairings(ingredient);
                if (pairings.Any())
                    result[ingredient] = pairings;
            }
            return result;
        }
    }
}
