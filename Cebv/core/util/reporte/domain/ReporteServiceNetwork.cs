﻿using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows.Media.Imaging;
using Cebv.core.domain;
using Cebv.core.util.reporte.viewmodels;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cebv.core.util.reporte.domain;

public partial class ReporteResponse : ObservableObject
{
    [JsonConstructor]
    public ReporteResponse(Reporte data)
    {
        Data = data;
    }

    [ObservableProperty] private Reporte? _data;
}

public abstract class ReporteServiceNetwork
{
    private static HttpClient Client => CebvClientHandler.SharedClient;

    // Metodo generado con Gemini
    private static Stream BitmapImageToStream(BitmapImage bitmapImage)
    {
        MemoryStream memoryStream = new MemoryStream(); // Use MemoryStream for in-memory operations
        BitmapEncoder encoder = new PngBitmapEncoder(); // Or JpegBitmapEncoder
        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
        encoder.Save(memoryStream);
        memoryStream.Position = 0; // Reset stream position to the beginning for reading
        return memoryStream;
    }

    public static BitmapImage Base64StringToBitmapImage(string base64String)
    {
        try
        {
            // Convert base64 string to byte array
            byte[] imageBytes = Convert.FromBase64String(base64String);

            // Create a memory stream from the byte array
            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
            {
                // Create the BitmapImage
                BitmapImage bitmapImage = new BitmapImage();

                // Important: BeginInit/EndInit for proper image initialization
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption =
                    BitmapCacheOption.OnLoad; // Load image fully into memory for better performance
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // Prevent cross-thread access issues in WPF

                return bitmapImage;
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., invalid base64, corrupt image data)
            Console.WriteLine($"Error decoding base64 image: {ex.Message}");
            return null; // Or return a default image
        }
    }

    public static string BitmapImageToBase64(BitmapImage bitmapImage)
    {
        if (bitmapImage == null) return null;

        // Encode to the desired format (e.g., Png, Jpeg)
        BitmapEncoder encoder = new PngBitmapEncoder(); // Or JpegBitmapEncoder for JPEG
        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

        // Use MemoryStream to store the encoded data
        using (MemoryStream memoryStream = new MemoryStream())
        {
            encoder.Save(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();

            // Convert bytes to base64 string
            return Convert.ToBase64String(imageBytes);
        }
    }
    
    public static async Task<Reporte> ShowReporte(int id)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"/api/reportes/{id}", UriKind.Relative),
            Method = HttpMethod.Get
        };

        var response = await Client.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Reportes>(json)?.Data!;
    }

    public static async Task<Reporte> Sync(Reporte reporte)
    {
        var json = JsonConvert.SerializeObject(reporte);
        Console.Write($"Request: {JObject.Parse(json).ToString(Formatting.Indented)}\n \n");

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri("/api/actualizar/reporte", UriKind.Relative),
            Method = HttpMethod.Post,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await Client.SendAsync(request);
        json = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Response: {JObject.Parse(json).ToString(Formatting.Indented)}");
        return JsonConvert.DeserializeObject<ReporteResponse>(json)?.Data!;
    }

    public static async Task<bool> SetFolios(int reporte_id)
    {
        var response = await Client.GetAsync($"api/reportes/asignar_folio/{reporte_id}");
        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response: {JObject.Parse(json).ToString(Formatting.Indented)}");

        return response.IsSuccessStatusCode;
    }

    public static async Task SubirFotosDesaparecido(int desaparecido_id, List<BitmapImage> imagenes,
        BitmapImage? imagen_boletin)
    {
        var form = new MultipartFormDataContent();
        var count = 0;
        foreach (var imagen in imagenes)
        {
            var content = new StreamContent(BitmapImageToStream(imagen));
            var filename = Path.GetFileName(imagen.UriSource.ToString());

            form.Add(content, $"file_{count + 1}", filename);
            count++;
        }

        if (imagen_boletin != null)
        {
            var content = new StreamContent(BitmapImageToStream(imagen_boletin));
            var filename = Path.GetFileName(imagen_boletin.UriSource.ToString());
            form.Add(content, "boletin", filename);
        }

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"/api/desaparecidos/fotos/{desaparecido_id}", UriKind.Relative),
            Method = HttpMethod.Post,
            Content = form
        };

        var response = await Client.SendAsync(request);
    }

    public static async Task<BitmapImage?> GetImage(int sena_id)
    {
        if (sena_id <= 0) return null;
        var response = await Client.GetAsync($"/api/senas-particulares/foto/{sena_id}");
        if (!response.IsSuccessStatusCode) return null;
        var base64 = await response.Content.ReadAsStringAsync();
        return Base64StringToBitmapImage(base64);
    }
}