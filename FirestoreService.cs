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

		// Base Firestore endpoint
		private static string FirestoreBaseUrl =>
			$"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents";

		/// <summary>
		/// Example: Get ALL recipes from global /Recipes collection in Firestore.
		/// (Rename or remove if not needed.)
		/// </summary>
		public static async Task<List<Recipe>> GetAllRecipesAsync(string idToken)
		{
			string url = $"{FirestoreBaseUrl}/Recipes";
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
					if (fieldsNode == null) continue;

					var nameValue = fieldsNode["Name"]?["stringValue"]?.ToString() ?? "";
					var instructionsValue = fieldsNode["Instructions"]?["stringValue"]?.ToString() ?? "";
					var imageUrlValue = fieldsNode["ImageUrl"]?["stringValue"]?.ToString() ?? "";

					// Firestore array: fields->Ingredients->arrayValue->values
					var ingredientsArray = fieldsNode["Ingredients"]?["arrayValue"]?["values"]?.AsArray();
					var ingredientsList = ingredientsArray != null
						? ingredientsArray.Select(ing => ing?["stringValue"]?.ToString() ?? "").ToList()
						: new List<string>();

					results.Add(new Recipe
					{
						Name = nameValue,
						Ingredients = ingredientsList,
						Instructions = instructionsValue,
						ImageUrl = imageUrlValue
					});
				}
			}
			return results;
		}

		/// <summary>
		/// Example: Saves/updates a recipe in global /Recipes collection.
		/// (Rename or remove if not needed.)
		/// </summary>
		public static async Task SetGlobalRecipeAsync(string recipeId, Recipe recipe, string idToken)
		{
			string url = $"{FirestoreBaseUrl}/Recipes/{recipeId}";

			var fieldsObject = new
			{
				fields = new
				{
					Name = new { stringValue = recipe.Name },
					Instructions = new { stringValue = recipe.Instructions },
					ImageUrl = new { stringValue = recipe.ImageUrl },
					Ingredients = new
					{
						arrayValue = new
						{
							values = recipe.Ingredients.Select(i => new { stringValue = i }).ToList()
						}
					}
				}
			};

			string jsonBody = JsonSerializer.Serialize(fieldsObject);
			var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {idToken}");
			var response = await client.PatchAsync(url, content);
			response.EnsureSuccessStatusCode();
		}

		// ----------------------------------------------------------------
		// If your code calls "GetUserFavoritesAsync" or "SetUserFavoriteAsync",
		// define them here. Example:
		// ----------------------------------------------------------------

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
					if (fieldsNode == null) continue;

					var nameValue = fieldsNode["Name"]?["stringValue"]?.ToString() ?? "";
					var instructionsValue = fieldsNode["Instructions"]?["stringValue"]?.ToString() ?? "";
					var imageUrlValue = fieldsNode["ImageUrl"]?["stringValue"]?.ToString() ?? "";

					var ingredientsArray = fieldsNode["Ingredients"]?["arrayValue"]?["values"]?.AsArray();
					var ingredientsList = ingredientsArray != null
						? ingredientsArray.Select(ing => ing?["stringValue"]?.ToString() ?? "").ToList()
						: new List<string>();

					favorites.Add(new Recipe
					{
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
			string url = $"{FirestoreBaseUrl}/users/{userId}/Favorites/{favoriteId}";

			var fieldsObject = new
			{
				fields = new
				{
					Name = new { stringValue = recipe.Name },
					Instructions = new { stringValue = recipe.Instructions },
					ImageUrl = new { stringValue = recipe.ImageUrl },
					Ingredients = new
					{
						arrayValue = new
						{
							values = recipe.Ingredients.Select(i => new { stringValue = i }).ToList()
						}
					}
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

	// A small extension for HttpClient to allow PATCH
	public static class HttpClientExtensions
	{
		public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
		{
			var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
			{
				Content = content
			};
			return await client.SendAsync(request);
		}
	}
}
