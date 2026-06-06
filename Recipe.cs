using System.Collections.Generic;

namespace assignment_2425.Models
{
    public class Recipe
    {
        public string Name { get; set; } = "";

        // Using a list of strings to store ingredients (e.g. "2 slices Bread", "1 tbsp Peanut Butter")
        public List<string> Ingredients { get; set; } = new List<string>();

        public string Instructions { get; set; } = "";
        public string ImageUrl { get; set; } = "";
    }
}
