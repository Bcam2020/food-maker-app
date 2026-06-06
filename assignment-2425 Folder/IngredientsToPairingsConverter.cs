using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls;

namespace assignment_2425.Converters
{
    public class IngredientsToPairingsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ingredients = value as string;
            if (string.IsNullOrWhiteSpace(ingredients))
                return "No suggestions available";

            // Split ingredients and convert to lowercase
            var ingredientList = ingredients.Split(',')
                .Select(i => i.Trim().ToLower())
                .ToList();

            // Dictionary of ingredient pairings
            var pairingDict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "tomato", new List<string> { "basil", "olive oil", "garlic", "mozzarella", "oregano" } },
                { "chicken", new List<string> { "rosemary", "lemon", "garlic", "thyme", "paprika" } },
                { "avocado", new List<string> { "lime", "cilantro", "tomato", "onion", "jalapeño" } },
                { "spinach", new List<string> { "feta", "walnuts", "cranberries", "lemon", "balsamic vinegar" } },
                { "pasta", new List<string> { "parmesan", "olive oil", "basil", "garlic", "black pepper" } },
                { "bread", new List<string> { "butter", "jam", "avocado", "olive oil" } },
                { "peanut butter", new List<string> { "jelly", "banana", "honey", "chocolate" } },
                { "fruit", new List<string> { "yogurt", "honey", "mint", "nuts" } },
                { "beef", new List<string> { "black pepper", "onion", "thyme", "garlic", "red wine" } },
                { "fish", new List<string> { "lemon", "dill", "capers", "olive oil", "parsley" } },
                { "egg", new List<string> { "salt", "pepper", "chives", "cheddar", "sour cream" } },
                { "cheese", new List<string> { "crackers", "grapes", "olive oil", "basil", "figs" } },
                { "potato", new List<string> { "rosemary", "thyme", "cheddar", "sour cream", "bacon" } },
                { "carrot", new List<string> { "ginger", "honey", "cumin", "lemon", "turmeric" } },
                { "cucumber", new List<string> { "mint", "yogurt", "dill", "lemon", "feta" } },
                { "lemon", new List<string> { "thyme", "olive oil", "garlic", "rosemary", "honey" } },
                { "garlic", new List<string> { "butter", "parmesan", "basil", "olive oil", "rosemary" } },
                { "basil", new List<string> { "tomato", "mozzarella", "olive oil", "garlic", "pine nuts" } },
                { "olive oil", new List<string> { "balsamic vinegar", "lemon", "garlic", "rosemary", "thyme" } },
                { "rice", new List<string> { "soy sauce", "scallions", "sesame oil", "peanuts", "ginger" } },
                { "beans", new List<string> { "cumin", "chili powder", "onion", "cilantro", "tomato" } },
                { "yogurt", new List<string> { "cucumber", "mint", "honey", "granola", "lemon" } },
                { "honey", new List<string> { "yogurt", "lemon", "peanut butter", "banana", "cinnamon" } },
                { "broccoli", new List<string> { "cheddar", "garlic", "lemon", "almonds", "parmesan" } },
                { "almonds", new List<string> { "honey", "dates", "cinnamon", "chocolate", "blueberries" } },
                { "dates", new List<string> { "walnuts", "cinnamon", "oats", "vanilla", "almonds" } },
                { "chocolate", new List<string> { "strawberry", "mint", "almonds", "coffee", "banana" } },
                { "coffee", new List<string> { "chocolate", "cinnamon", "vanilla", "cream", "hazelnuts" } },
                { "cream", new List<string> { "strawberry", "blueberry", "vanilla", "coffee", "chocolate" } },
                { "strawberry", new List<string> { "basil", "mint", "cream", "chocolate", "sugar" } },
                { "blueberry", new List<string> { "lemon", "yogurt", "mint", "honey", "oats" } },
                { "mint", new List<string> { "chocolate", "lemon", "cucumber", "yogurt", "strawberry" } },
                { "dill", new List<string> { "salmon", "lemon", "cucumber", "yogurt", "capers" } }
            };

            // Gather unique pairings for each ingredient
            var suggestions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var ing in ingredientList)
            {
                if (pairingDict.TryGetValue(ing, out var pairs))
                {
                    foreach (var pair in pairs)
                        suggestions.Add(pair);
                }
            }
            return suggestions.Any() ? string.Join(", ", suggestions) : "No suggestions available";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
