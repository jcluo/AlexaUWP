using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.UI.Popups;

public class Library
{
    private MediaCapture capture;
    private InMemoryRandomAccessStream buffer;
    private Uri alexaWebHost = new Uri("http://192.168.2.55:5000");

    public static bool Recording;

    private async Task<bool> init()
    {
        if (buffer != null)
        {
            buffer.Dispose();
        }
        buffer = new InMemoryRandomAccessStream();

        if (capture != null)
        {
            capture.Dispose();
        }
        
        try
        {
            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Audio
            };
            capture = new MediaCapture();
            await capture.InitializeAsync(settings);
            capture.RecordLimitationExceeded += async (MediaCapture sender) =>
            {
                await Stop();
                throw new Exception("Exceed Recording Limits!");
            };
            capture.Failed += (MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs) =>
            {
                Recording = false;
                throw new Exception(string.Format("Code: {0}. {1}", errorEventArgs.Code, errorEventArgs.Message));
            };
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null && ex.InnerException.GetType() == typeof(UnauthorizedAccessException))
            {
                throw ex.InnerException;
            }
            throw;
        }
        return true;
    }

    public async void Record()
    {
        await init();
        await capture.StartRecordToStreamAsync(MediaEncodingProfile.CreateWav(AudioEncodingQuality.Low), buffer);
    }

    public async Task<InMemoryRandomAccessStream> Stop()
    {
        await capture.StopRecordAsync();

        using (var httpClient = new HttpClient())
        {
            var requestContent = new HttpMultipartFormDataContent();
            var requestUri = new Uri(alexaWebHost.ToString() + "audio");
            var streamContent = new HttpStreamContent(buffer.GetInputStreamAt(0));
            streamContent.Headers.ContentType = Windows.Web.Http.Headers.HttpMediaTypeHeaderValue.Parse("audio/wav");
            streamContent.Headers.ContentLength = buffer.Size;
            requestContent.Add(streamContent, "data", "blob");
            try
            {
                var response = await httpClient.PostAsync(requestUri, requestContent);

                if (response.StatusCode == Windows.Web.Http.HttpStatusCode.Ok)
                {
                    InMemoryRandomAccessStream respBuffer = new InMemoryRandomAccessStream();
                    await response.Content.WriteToStreamAsync(respBuffer);
                    return respBuffer;
                }
            }
            catch
            {
                var dialog = new MessageDialog("No connection!");
                await dialog.ShowAsync();
            }
            return null;
        }
    }
}