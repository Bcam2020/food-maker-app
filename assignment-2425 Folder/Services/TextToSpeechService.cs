using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel.Communication;

namespace assignment_2425.Services
{
    public static class TextToSpeechService
    {
        public static async Task SpeakAsync(string text)
        {
            await TextToSpeech.Default.SpeakAsync(text);
        }
    }
}
