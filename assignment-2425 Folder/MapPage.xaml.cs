using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace assignment_2425
{
    public partial class MapPage : ContentPage
    {
        private double UserLatitude;
        private double UserLongitude;
        private List<NearbyStore> _nearbyStores = new();

        public MapPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            App.SetAppTheme();
            // Use device geolocation service to get current location
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location == null)
                {
                    // In case there is no last known location, force a new read.
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                    location = await Geolocation.GetLocationAsync(request);
                }

                if (location != null)
                {
                    UserLatitude = location.Latitude;
                    UserLongitude = location.Longitude;
                }
                else
                {
                    await DisplayAlert("Location Error", "Unable to get your current location.", "OK");
                    // Provide fallback location if necessary
                    UserLatitude = 0;
                    UserLongitude = 0;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to retrieve location: " + ex.Message, "OK");
                // Provide fallback values
                UserLatitude = 0;
                UserLongitude = 0;
            }
            LoadMap();
        }

        // Navigate back to IngredientsPage
        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("IngredientsPage");
        }

        // Recenter the map to user's location
        private void OnRecenterClicked(object sender, EventArgs e)
        {
            MapWebView.Eval($"window.recenterMap({UserLatitude}, {UserLongitude});");
        }

        // Focus map on the selected store marker
        private void OnStoreSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
            {
                var selectedStore = e.CurrentSelection[0] as NearbyStore;
                if (selectedStore != null)
                {
                    MapWebView.Eval($"window.focusMarker({selectedStore.Latitude}, {selectedStore.Longitude});");
                }
                ((CollectionView)sender).SelectedItem = null;
            }
        }

        // Refresh map and store list
        private async void OnRefresh(object sender, EventArgs e)
        {
            await LoadMapAsync();
            StoresRefreshView.IsRefreshing = false;
        }

        // Asynchronously load map and nearby stores
        private async Task LoadMapAsync()
        {
            try
            {
                MapLoader.IsVisible = true;
                MapLoader.IsRunning = true;
                string storesJson = await GetNearbyStoresJson(UserLatitude, UserLongitude);
                ParseOverpassResponse(storesJson);
                StoresCollectionView.ItemsSource = _nearbyStores;
                string mapHtml = GenerateMapHtml(storesJson);
                MapWebView.Source = new HtmlWebViewSource { Html = mapHtml };
                await MapWebView.FadeTo(1, 500);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Map loading failed: {ex.Message}", "OK");
            }
            finally
            {
                MapLoader.IsRunning = false;
                MapLoader.IsVisible = false;
            }
        }

        // Wrapper to call the async map load
        private async void LoadMap()
        {
            await LoadMapAsync();
        }

        // Fetch nearby stores JSON from Overpass API
        private async Task<string> GetNearbyStoresJson(double lat, double lng)
        {
            try
            {
                string overpassQuery = $@"[out:json];(node[shop=supermarket](around:5000,{lat},{lng});node[amenity=grocery](around:5000,{lat},{lng}););out;";
                string url = $"https://overpass-api.de/api/interpreter?data={System.Net.WebUtility.UrlEncode(overpassQuery)}";
                using var httpClient = new HttpClient();
                return await httpClient.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to fetch store data: " + ex.Message, "OK");
                return "{\"elements\":[]}";
            }
        }

        // Parse the JSON response and populate nearby stores list
        private void ParseOverpassResponse(string storesJson)
        {
            var doc = JsonDocument.Parse(storesJson);
            if (!doc.RootElement.TryGetProperty("elements", out var elementsProp))
                return;
            _nearbyStores.Clear();
            foreach (var element in elementsProp.EnumerateArray())
            {
                if (element.TryGetProperty("lat", out var latProp) && element.TryGetProperty("lon", out var lonProp))
                {
                    double storeLat = latProp.GetDouble();
                    double storeLon = lonProp.GetDouble();
                    string storeName = "Unknown Store";
                    if (element.TryGetProperty("tags", out var tagsProp) && tagsProp.TryGetProperty("name", out var nameProp))
                    {
                        storeName = nameProp.GetString();
                    }
                    _nearbyStores.Add(new NearbyStore { Name = storeName, Latitude = storeLat, Longitude = storeLon });
                }
            }
        }

        // Generate HTML with embedded Leaflet map and store markers
        private string GenerateMapHtml(string storesJson)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
    <title>Nearby Grocery Stores</title>
    <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0'/>
    <link rel='stylesheet' href='https://unpkg.com/leaflet/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet/dist/leaflet.js'></script>
    <style>
        html, body {{
            margin: 0;
            padding: 0;
            height: 100%;
            width: 100%;
            overflow: hidden;
        }}
        #map {{
            width: 100%;
            height: 100%;
            position: absolute;
        }}
    </style>
</head>
<body>
    <div id='map'></div>
    <script>
        var userLat = {UserLatitude};
        var userLng = {UserLongitude};
        var map = L.map('map').setView([userLat, userLng], 13);
        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            attribution: '&copy; OpenStreetMap contributors'
        }}).addTo(map);
        var userIcon = L.icon({{
            iconUrl: 'https://leafletjs.com/examples/custom-icons/leaf-red.png',
            iconSize: [38, 38]
        }});
        L.marker([userLat, userLng], {{ icon: userIcon }})
         .addTo(map)
         .bindPopup('You are here');
        var data = {storesJson};
        var markerDict = {{}};
        var storeIcon = L.icon({{
            iconUrl: 'https://cdn-icons-png.flaticon.com/512/3075/3075977.png',
            iconSize: [32, 32]
        }});
        if (data && data.elements) {{
            data.elements.forEach(function(store) {{
                if (store.lat && store.lon) {{
                    var storeName = (store.tags && store.tags.name) ? store.tags.name : 'Unknown Store';
                    var marker = L.marker([store.lat, store.lon], {{ icon: storeIcon }})
                                 .addTo(map)
                                 .bindPopup('Store: ' + storeName);
                    var key = store.lat + ',' + store.lon;
                    markerDict[key] = marker;
                }}
            }});
        }}
        window.recenterMap = function(lat, lng) {{
            map.setView([lat, lng], 13);
        }};
        window.focusMarker = function(lat, lng) {{
            var key = lat + ',' + lng;
            var m = markerDict[key];
            if (m) {{
                map.setView([lat, lng], 16);
                m.openPopup();
            }}
        }};
    </script>
</body>
</html>";
        }
    }

    // Model representing a nearby store
    public class NearbyStore
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
