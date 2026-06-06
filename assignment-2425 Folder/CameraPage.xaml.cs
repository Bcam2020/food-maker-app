using System;
using System.IO;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;

namespace assignment_2425
{
    public partial class CameraPage : ContentPage
    {
        public CameraPage()
        {
            InitializeComponent();
        }


        private async void OnCapturePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                        using var sourceStream = await photo.OpenReadAsync();
                        using var localFileStream = File.OpenWrite(localFilePath);
                        await sourceStream.CopyToAsync(localFileStream);

                        CapturedImage.Source = ImageSource.FromFile(localFilePath);
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Camera not supported on this device.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Capture failed: {ex.Message}", "OK");
            }
        }

        private async void OnPickPhotoClicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo != null)
                {
                    string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using var sourceStream = await photo.OpenReadAsync();
                    using var localFileStream = File.OpenWrite(localFilePath);
                    await sourceStream.CopyToAsync(localFileStream);

                    CapturedImage.Source = ImageSource.FromFile(localFilePath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Pick photo failed: {ex.Message}", "OK");
            }
        }
    }
}
