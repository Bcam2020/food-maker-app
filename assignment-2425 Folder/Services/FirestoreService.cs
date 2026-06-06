using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using assignment_2425.Models;

namespace assignment_2425.Services
{
    public static class FirestoreService
    {
        private const string ProjectId = "assignment-2425";
        private static string FirestoreBaseUrl =>
            $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents";

        // ---------------------------------------------------------------
        // 1. Friends
        // ---------------------------------------------------------------
        public static async Task AddFriendAsync(string localUserId, string friendId, string friendName, string idToken)
        {
            string url = $"{FirestoreBaseUrl}/users/{localUserId}/Friends/{friendId}";

            var fieldsObject = new
            {
                fields = new
                {
                    FriendId = new { stringValue = friendId },
                    Name = new { stringValue = friendName }
                }
            };

            string jsonBody = JsonSerializer.Serialize(fieldsObject);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");
            var response = await client.PatchAsync(url, content);
            response.EnsureSuccessStatusCode();
        }

        public static async Task<List<Friend>> GetUserFriendsAsync(string userId, string idToken)
        {
            string url = $"{FirestoreBaseUrl}/users/{userId}/Friends";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<Friend>();

            string json = await response.Content.ReadAsStringAsync();
            var rootNode = JsonNode.Parse(json);
            var documents = rootNode?["documents"]?.AsArray();
            var friends = new List<Friend>();

            if (documents != null)
            {
                foreach (var doc in documents)
                {
                    var fieldsNode = doc?["fields"];
                    if (fieldsNode == null) continue;

                    string friendId = fieldsNode["FriendId"]?["stringValue"]?.ToString() ?? "";
                    string friendName = fieldsNode["Name"]?["stringValue"]?.ToString() ?? "";

                    friends.Add(new Friend
                    {
                        FriendId = friendId,
                        Name = friendName
                    });
                }
            }

            return friends;
        }

        public static async Task DeleteFriendAsync(string localUserId, string friendId, string idToken)
        {
            string url = $"{FirestoreBaseUrl}/users/{localUserId}/Friends/{friendId}";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");
            var response = await client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        // ---------------------------------------------------------------
        // 2. Global Recipes
        // ---------------------------------------------------------------
        public static async Task<List<Recipe>> GetAllRecipesAsync(string idToken)
        {
            string url = $"{FirestoreBaseUrl}/Recipes";
            using var client = new HttpClient();
            if (!string.IsNullOrEmpty(idToken))
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<Recipe>();

            string json = await response.Content.ReadAsStringAsync();
            var rootNode = JsonNode.Parse(json);
            var documents = rootNode?["documents"]?.AsArray();
            var results = new List<Recipe>();

            if (documents != null)
            {
                foreach (var doc in documents)
                {
                    var fieldsNode = doc?["fields"];
                    if (fieldsNode == null)
                        continue;

                    // Extract document ID
                    string docName = doc?["name"]?.ToString() ?? "";
                    string recipeId = "";
                    int lastSlash = docName.LastIndexOf('/');
                    if (lastSlash >= 0 && lastSlash < docName.Length - 1)
                        recipeId = docName.Substring(lastSlash + 1);

                    var nameValue = fieldsNode["Name"]?["stringValue"]?.ToString() ?? "";
                    var instructionsValue = fieldsNode["Instructions"]?["stringValue"]?.ToString() ?? "";
                    var imageUrlValue = fieldsNode["ImageUrl"]?["stringValue"]?.ToString() ?? "";

                    // Single String Parsing for Ingredients
                    var ingredientsString = fieldsNode["Ingredients"]?["stringValue"]?.ToString() ?? "";
                    var ingredientsList = new List<string>();
                    if (!string.IsNullOrEmpty(ingredientsString))
                    {
                        ingredientsList = ingredientsString.Split('|').Select(x => x.Trim()).ToList();
                    }

                    results.Add(new Recipe
                    {
                        RecipeId = recipeId,
                        Name = nameValue,
                        Ingredients = ingredientsList,
                        Instructions = instructionsValue,
                        ImageUrl = imageUrlValue
                    });
                }
            }

            return results;
        }

        public static async Task SetGlobalRecipeAsync(string recipeId, Recipe recipe, string idToken)
        {
            string url = $"{FirestoreBaseUrl}/Recipes/{recipeId}";
            string joinedIngredients = string.Join(" | ", recipe.Ingredients ?? new List<string>());

            var fieldsObject = new
            {
                fields = new
                {
                    Name = new { stringValue = recipe.Name },
                    Instructions = new { stringValue = recipe.Instructions },
                    ImageUrl = new { stringValue = recipe.ImageUrl },
                    Ingredients = new { stringValue = joinedIngredients }
                }
            };

            string jsonBody = JsonSerializer.Serialize(fieldsObject);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            if (!string.IsNullOrEmpty(idToken))
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");

            var response = await client.PatchAsync(url, content);
            response.EnsureSuccessStatusCode();
        }

        // ---------------------------------------------------------------
        // 3. User Document / Profile
        // ---------------------------------------------------------------
        public static async Task EnsureUserDocExistsAsync(string userId, string idToken)
        {
            string docUrl = $"{FirestoreBaseUrl}/users/{userId}";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");

            var getResponse = await client.GetAsync(docUrl);
            if (getResponse.IsSuccessStatusCode)
                return;

            string postUrl = $"{FirestoreBaseUrl}/users?documentId={userId}";
            var fieldsObject = new
            {
                fields = new
                {
                    Username = new { stringValue = "New User" },
                    Email = new { stringValue = "" },
                    ProfilePicture = new { stringValue = "" }
                }
            };

            string jsonBody = JsonSerializer.Serialize(fieldsObject);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var postResponse = await client.PostAsync(postUrl, content);
            postResponse.EnsureSuccessStatusCode();
        }

        public static async Task<List<UserProfile>> SearchAllUsersAsync(string idToken)
        {
            string url = $"{FirestoreBaseUrl}/users";
            using var client = new HttpClient();
            if (!string.IsNullOrEmpty(idToken))
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<UserProfile>();

            string json = await response.Content.ReadAsStringAsync();
            var rootNode = JsonNode.Parse(json);
            var documents = rootNode?["documents"]?.AsArray();
            var results = new List<UserProfile>();

            if (documents != null)
            {
                foreach (var doc in documents)
                {
                    var fieldsNode = doc?["fields"];
                    if (fieldsNode == null)
                        continue;

                    string docName = doc?["name"]?.ToString() ?? "";
                    string userId = docName.Substring(docName.LastIndexOf('/') + 1);

                    var username = fieldsNode["Username"]?["stringValue"]?.ToString() ?? "";
                    var email = fieldsNode["Email"]?["stringValue"]?.ToString() ?? "";
                    var pic = fieldsNode["ProfilePicture"]?["stringValue"]?.ToString() ?? "";

                    results.Add(new UserProfile
                    {
                        UserId = userId,
                        Username = username,
                        Email = email,
                        ProfilePicture = pic
                    });
                }
            }

            return results;
        }

        public static async Task<UserProfile> GetUserProfileAsync(string userId, string idToken)
        {
            string url = $"{FirestoreBaseUrl}/users/{userId}";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            string json = await response.Content.ReadAsStringAsync();
            var rootNode = JsonNode.Parse(json);
            var fieldsNode = rootNode?["fields"];
            if (fieldsNode == null)
                return null;

            var profile = new UserProfile
            {
                UserId = userId,
                Username = fieldsNode["Username"]?["stringValue"]?.ToString() ?? "",
                Email = fieldsNode["Email"]?["stringValue"]?.ToString() ?? "",
                ProfilePicture = fieldsNode["ProfilePicture"]?["stringValue"]?.ToString() ?? ""
            };
            return profile;
        }

        public static async Task SetUserProfileAsync(string userId, UserProfile profile, string idToken)
        {
            string url = $"{FirestoreBaseUrl}/users/{userId}";
            var fieldsObject = new
            {
                fields = new
                {
                    Username = new { stringValue = profile.Username },
                    Email = new { stringValue = profile.Email },
                    ProfilePicture = new { stringValue = profile.ProfilePicture }
                }
            };

            string jsonBody = JsonSerializer.Serialize(fieldsObject);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");
            var response = await client.PatchAsync(url, content);
            response.EnsureSuccessStatusCode();
        }

        // ---------------------------------------------------------------
        // 4. User Recipes
        // ---------------------------------------------------------------
        public static async Task<List<Recipe>> GetUserRecipesAsync(string userId, string idToken)
        {
            string url = $"{FirestoreBaseUrl}/users/{userId}/Recipes";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<Recipe>();

            string json = await response.Content.ReadAsStringAsync();
            var rootNode = JsonNode.Parse(json);
            var documents = rootNode?["documents"]?.AsArray();
            var results = new List<Recipe>();

            if (documents != null)
            {
                foreach (var doc in documents)
                {
                    var fieldsNode = doc?["fields"];
                    if (fieldsNode == null)
                        continue;

                    // Extract document ID
                    string docName = doc?["name"]?.ToString() ?? "";
                    string recipeId = "";
                    int lastSlash = docName.LastIndexOf('/');
                    if (lastSlash >= 0 && lastSlash < docName.Length - 1)
                        recipeId = docName.Substring(lastSlash + 1);

                    var nameValue = fieldsNode["Name"]?["stringValue"]?.ToString() ?? "";
                    var instructionsValue = fieldsNode["Instructions"]?["stringValue"]?.ToString() ?? "";
                    var imageUrlValue = fieldsNode["ImageUrl"]?["stringValue"]?.ToString() ?? "";

                    // Single String Parsing for Ingredients
                    var ingredientsString = fieldsNode["Ingredients"]?["stringValue"]?.ToString() ?? "";
                    var ingredientsList = new List<string>();
                    if (!string.IsNullOrEmpty(ingredientsString))
                    {
                        ingredientsList = ingredientsString.Split('|').Select(x => x.Trim()).ToList();
                    }

                    results.Add(new Recipe
                    {
                        RecipeId = recipeId,
                        Name = nameValue,
                        Ingredients = ingredientsList,
                        Instructions = instructionsValue,
                        ImageUrl = imageUrlValue
                    });
                }
            }

            return results;
        }

        public static async Task SetUserRecipeAsync(string userId, string recipeId, Recipe recipe, string idToken)
        {
            string url = $"{FirestoreBaseUrl}/users/{userId}/Recipes/{recipeId}";
            string joinedIngredients = string.Join(" | ", recipe.Ingredients ?? new List<string>());

            var fieldsObject = new
            {
                fields = new
                {
                    Name = new { stringValue = recipe.Name },
                    Instructions = new { stringValue = recipe.Instructions },
                    ImageUrl = new { stringValue = recipe.ImageUrl },
                    Ingredients = new { stringValue = joinedIngredients }
                }
            };

            string jsonBody = JsonSerializer.Serialize(fieldsObject);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");
            var response = await client.PatchAsync(url, content);
            response.EnsureSuccessStatusCode();
        }

        public static async Task DeleteUserRecipeAsync(string userId, string recipeId, string idToken)
        {
            string url = $"{FirestoreBaseUrl}/users/{userId}/Recipes/{recipeId}";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");
            var response = await client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        // ---------------------------------------------------------------
        // 5. User Favorites
        // ---------------------------------------------------------------
        public static async Task<List<Recipe>> GetUserFavoritesAsync(string userId, string idToken)
        {
            string url = $"{FirestoreBaseUrl}/users/{userId}/Favorites";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<Recipe>();

            string json = await response.Content.ReadAsStringAsync();
            var rootNode = JsonNode.Parse(json);
            var documents = rootNode?["documents"]?.AsArray();
            var favorites = new List<Recipe>();

            if (documents != null)
            {
                foreach (var doc in documents)
                {
                    var fieldsNode = doc?["fields"];
                    if (fieldsNode == null)
                        continue;

                    var nameValue = fieldsNode["Name"]?["stringValue"]?.ToString() ?? "";
                    var instructionsValue = fieldsNode["Instructions"]?["stringValue"]?.ToString() ?? "";
                    var imageUrlValue = fieldsNode["ImageUrl"]?["stringValue"]?.ToString() ?? "";

                    var ingredientsString = fieldsNode["Ingredients"]?["stringValue"]?.ToString() ?? "";
                    var ingredientsList = new List<string>();
                    if (!string.IsNullOrEmpty(ingredientsString))
                    {
                        ingredientsList = ingredientsString.Split('|').Select(x => x.Trim()).ToList();
                    }

                    string docName = doc?["name"]?.ToString() ?? "";
                    string favoriteId = "";
                    int lastSlash = docName.LastIndexOf('/');
                    if (lastSlash >= 0 && lastSlash < docName.Length - 1)
                        favoriteId = docName.Substring(lastSlash + 1);

                    favorites.Add(new Recipe
                    {
                        RecipeId = favoriteId,
                        Name = nameValue,
                        Ingredients = ingredientsList,
                        Instructions = instructionsValue,
                        ImageUrl = imageUrlValue
                    });
                }
            }

            return favorites;
        }

        public static async Task SetUserFavoriteAsync(string userId, string favoriteId, Recipe recipe, string idToken)
        {
            // Use POST to create/overwrite the favorite doc
            string url = $"{FirestoreBaseUrl}/users/{userId}/Favorites?documentId={favoriteId}";
            string joinedIngredients = string.Join(" | ", recipe.Ingredients ?? new List<string>());

            var fieldsObject = new
            {
                fields = new
                {
                    Name = new { stringValue = recipe.Name },
                    Instructions = new { stringValue = recipe.Instructions },
                    ImageUrl = new { stringValue = recipe.ImageUrl },
                    Ingredients = new { stringValue = joinedIngredients }
                }
            };

            string jsonBody = JsonSerializer.Serialize(fieldsObject);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }

        public static async Task AddUserFavoriteAsync(string userId, string recipeId, Recipe recipe, string idToken)
        {
            await SetUserFavoriteAsync(userId, recipeId, recipe, idToken);
        }

        public static async Task DeleteUserFavoriteAsync(string userId, string favoriteId, string idToken)
        {
            string url = $"{FirestoreBaseUrl}/users/{userId}/Favorites/{favoriteId}";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");
            var response = await client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        // ---------------------------------------------------------------
        // 6. Recipe Comments
        // ---------------------------------------------------------------
        public static async Task<List<Comment>> GetCommentsForRecipeAsync(string recipeId, string idToken)
        {
            string url = $"{FirestoreBaseUrl}/Recipes/{recipeId}/Comments";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<Comment>();

            string json = await response.Content.ReadAsStringAsync();
            var rootNode = JsonNode.Parse(json);
            var documents = rootNode?["documents"]?.AsArray();
            var comments = new List<Comment>();

            if (documents != null)
            {
                foreach (var doc in documents)
                {
                    var fieldsNode = doc?["fields"];
                    if (fieldsNode == null)
                        continue;

                    string timestampStr = fieldsNode["Timestamp"]?["timestampValue"]?.ToString() ?? "";
                    DateTime timestamp;
                    DateTime.TryParse(timestampStr, out timestamp);

                    comments.Add(new Comment
                    {
                        CommentId = doc?["name"]?.ToString() ?? "",
                        RecipeId = recipeId,
                        UserName = fieldsNode["UserName"]?["stringValue"]?.ToString() ?? "",
                        Text = fieldsNode["Text"]?["stringValue"]?.ToString() ?? "",
                        Timestamp = timestamp
                    });
                }
            }
            return comments;
        }

        public static async Task AddCommentAsync(string recipeId, Comment comment, string idToken)
        {
            string commentId = Guid.NewGuid().ToString("N");
            string url = $"{FirestoreBaseUrl}/Recipes/{recipeId}/Comments/{commentId}";

            var fieldsObject = new
            {
                fields = new
                {
                    UserName = new { stringValue = comment.UserName },
                    Text = new { stringValue = comment.Text },
                    Timestamp = new { timestampValue = DateTime.UtcNow.ToString("o") }
                }
            };

            string jsonBody = JsonSerializer.Serialize(fieldsObject);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");
            var response = await client.PatchAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
    }

    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PatchAsync(
            this HttpClient client, string requestUri, HttpContent content)
        {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
            {
                Content = content
            };
            return await client.SendAsync(request);
        }
    }
}
