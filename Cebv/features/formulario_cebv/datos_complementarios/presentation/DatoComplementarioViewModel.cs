using System.Collections.ObjectModel;
using Cebv.core.domain;
using Cebv.core.util.navigation;
using Cebv.core.util.reporte;
using Cebv.core.util.reporte.viewmodels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Cebv.features.formulario_cebv.datos_complementarios.presentation;

public partial class DatoComplementarioViewModel : ObservableObject
{
    private readonly IReporteService _reporteService =
        App.Current.Services.GetService<IReporteService>()!;

    private readonly IFormularioCebvNavigationService _navigationService =
        App.Current.Services.GetService<IFormularioCebvNavigationService>()!;

    [ObservableProperty] private Reporte _reporte = new();

    public DatoComplementarioViewModel()
    {
        InitAsync();
        
        Reporte = _reporteService.GetReporte();

        Reporte.DatoComplementario ??= new();
    }

    private async void InitAsync()
    {
        Estados = await CebvNetwork.GetRoute<Estado>("estados");

        var reporte = _reporteService.GetReporte();

        var estadoId =
            reporte.DatoComplementario?.Direccion.Asentamiento?.Municipio?.Estado?.Id;

        var municipioId =
            reporte.DatoComplementario?.Direccion.Asentamiento?.Municipio?.Id;

        if (estadoId is not null)
        {
            Municipios = await CebvNetwork.GetByFilter<Municipio>("municipios", "estado_id", estadoId);
            EstadoSelected = Estados.FirstOrDefault(x => x.Id == estadoId);
        }

        if (municipioId is null) return;
        {
            Asentamientos = await CebvNetwork.GetByFilter<Asentamiento>("asentamientos", "municipio_id", municipioId);
            MunicipioSelected = Municipios.FirstOrDefault(x => x.Id == municipioId)!;
        }
    }

    [ObservableProperty] private ObservableCollection<Estado> _estados = new();
    [ObservableProperty] private ObservableCollection<Municipio> _municipios = new();
    [ObservableProperty] private ObservableCollection<Asentamiento> _asentamientos = new();

    [ObservableProperty] private Estado? _estadoSelected;
    [ObservableProperty] private Municipio? _municipioSelected;

    async partial void OnEstadoSelectedChanged(Estado? value)
    {
        if (value?.Id is null) return;
        Municipios = await CebvNetwork.GetByFilter<Municipio>("municipios", "estado_id", value.Id);
    }

    async partial void OnMunicipioSelectedChanged(Municipio? value)
    {
        if (value?.Id is null) return;
        Asentamientos = await CebvNetwork.GetByFilter<Asentamiento>("asentamientos", "municipio_id", value.Id);
    }

    [RelayCommand]
    private void OnGuardarYSiguiente(Type pageType)
    {
        _reporteService.Sync();
        _navigationService.Navigate(pageType);
    }
}