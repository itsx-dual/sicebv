﻿using System.Collections.ObjectModel;
using System.Net.Http;
using Cebv.core.domain.paginated_resource;
using Cebv.core.util.reporte.viewmodels;
using Newtonsoft.Json;

namespace Cebv.core.domain;

public class InegiNetwork
{
    private static HttpClient Client => CebvClientHandler.SharedClient;

    public static async Task<ObservableCollection<Estado>> GetEstados()
    {
        var request = await Client.GetAsync("/api/estados");
        var response = await request.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<PaginatedResource<ObservableCollection<Estado>>>(response)?.Data ?? [];
    }

    public static async Task<ObservableCollection<Municipio>> GetMunicipiosDeEstado(string? estado_id)
    {
        if (estado_id is null) return [];
        var request = await Client.GetAsync($"/api/municipios?search={estado_id}");
        var response = await request.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<PaginatedResource<ObservableCollection<Municipio>>>(response)?.Data!;
    }

    public static async Task<ObservableCollection<Asentamiento>> GetAsentamientosDeMunicipio(string? municipio_id)
    {
        if (municipio_id is null) return [];
        var request = await Client.GetAsync($"/api/asentamientos?search={municipio_id}");
        var response = await request.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<PaginatedResource<ObservableCollection<Asentamiento>>>(response)?.Data!;
    }
}