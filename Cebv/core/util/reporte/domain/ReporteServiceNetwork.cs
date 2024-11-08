﻿using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows.Media.Imaging;
using Cebv.core.domain;
using Cebv.core.domain.paginated_resource;
using Cebv.core.util.reporte.viewmodels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cebv.core.util.reporte.domain;

public abstract class ReporteServiceNetwork
{
    private static HttpClient Client => CebvClientHandler.SharedClient;

    public static async Task<Reporte> ShowReporte(int id)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"/api/reportes/{id}", UriKind.Relative),
            Method = HttpMethod.Get
        };

        var response = await Client.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<PaginatedResource<Reporte>>(json)?.Data!;
    }

    public static async Task<Reporte> Sync(Reporte reporte)
    {
        var json = JsonConvert.SerializeObject(reporte);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"Request: {JObject.Parse(json).ToString(Formatting.Indented)}\n \n");

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri("/api/actualizar/reporte", UriKind.Relative),
            Method = HttpMethod.Post,
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await Client.SendAsync(request);
        json = await response.Content.ReadAsStringAsync();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Response: {JObject.Parse(json).ToString(Formatting.Indented)}");
        Console.ForegroundColor = ConsoleColor.White;
        return JsonConvert.DeserializeObject<PaginatedResource<Reporte>>(json)?.Data!;
    }

    public static async Task<bool> SetFolios(int reporteId)
    {
        var response = await Client.GetAsync($"api/reportes/asignar_folio/{reporteId}");
        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response: {JObject.Parse(json).ToString(Formatting.Indented)}");

        return response.IsSuccessStatusCode;
    }

    public static async Task SubirFotosDesaparecido(int desaparecidoId, List<BitmapImage> imagenes,
        BitmapImage? imagenBoletin)
    {
        var form = new MultipartFormDataContent();
        var count = 0;
        foreach (var imagen in imagenes)
        {
            var content = new StreamContent(ImageUtils.BitmapImageToStream(imagen));
            var filename = Path.GetFileName(imagen.UriSource.ToString());

            form.Add(content, $"file_{count + 1}", filename);
            count++;
        }

        if (imagenBoletin != null)
        {
            var content = new StreamContent(ImageUtils.BitmapImageToStream(imagenBoletin));
            var filename = Path.GetFileName(imagenBoletin.UriSource.ToString());
            form.Add(content, "boletin", filename);
        }

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"/api/desaparecidos/fotos/{desaparecidoId}", UriKind.Relative),
            Method = HttpMethod.Post,
            Content = form
        };

        await Client.SendAsync(request);
    }

    public static async Task<BitmapImage?> GetImage(int senaId)
    {
        if (senaId <= 0) return null;
        var response = await Client.GetAsync($"/api/senas-particulares/foto/{senaId}");
        if (!response.IsSuccessStatusCode) return null;
        var base64 = await response.Content.ReadAsStringAsync();
        return ImageUtils.Base64StringToBitmapImage(base64);
    }
}