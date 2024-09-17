using System.Collections.ObjectModel;
using static Cebv.core.data.OpcionesCebv;
using Cebv.core.domain;
using Cebv.core.modules.ubicacion.domain;
using Cebv.core.util.navigation;
using Cebv.core.util.reporte;
using Cebv.core.util.reporte.viewmodels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Catalogo = Cebv.core.util.reporte.viewmodels.Catalogo;

namespace Cebv.features.formulario_cebv.datos_del_reporte.presentation;

public partial class DatosReporteViewModel : ObservableObject
{
    private static IReporteService _reporteService = App.Current.Services.GetService<IReporteService>()!;

    private IFormularioCebvNavigationService _navigationService =
        App.Current.Services.GetService<IFormularioCebvNavigationService>()!;

    [ObservableProperty] private Reporte _reporte = null!;

    /**
     * Constructor de la clase.
     */
    public DatosReporteViewModel()
    {
        LoadAsync();
    }

    private async void LoadAsync()
    {
        var tipoMedioId = _reporteService.GetReporte().MedioConocimiento?.TipoMedio.Id;

        TiposMedios = await CebvNetwork.GetRoute<Catalogo>("tipos-medios");
        Medios = await CebvNetwork.GetByFilter<MedioConocimiento>
            ("medios", "tipo_medio_id", tipoMedioId.ToString() ?? "1");
        Estados = await UbicacionNetwork.GetEstados();
        Instituciones = await CebvNetwork.GetRoute<Catalogo>("instituciones");

        Reporte = _reporteService.GetReporte();
        TipoMedio = Reporte.MedioConocimiento?.TipoMedio!;

        // Esta seccion del formulario lidia con dos atributos de reportante.
        if (Reporte.Reportantes.Count == 0)
        {
            Reporte.Reportantes.Add(new Reportante
            {
                InformacionExclusivaBusqueda = false,
                PublicacionRegistroNacional = false
            });
        }
    }

    /**
     * Fuente de información.
     */
    [ObservableProperty] private ObservableCollection<Catalogo>? _tiposMedios;

    [ObservableProperty] private Catalogo? _tipoMedio;

    [ObservableProperty] private ObservableCollection<MedioConocimiento>? _medios;

    [ObservableProperty] private ObservableCollection<Estado>? _estados;

    [ObservableProperty] private ObservableCollection<Catalogo>? _instituciones;

    /**
     * Información de consentimiento.
     */
    [ObservableProperty] private Dictionary<string, bool?> _opcionesCebv = Opciones;

    [ObservableProperty] private string _informacionExclusivaBusqueda = No;

    [ObservableProperty] private string _publicacionInformacion = No;

    /**
     * Cargar medios segun el tipo de medio seleccionado.
     */
    async partial void OnTipoMedioChanged(Catalogo? value)
    {
        if (value?.Id is null) return;

        Medios = await CebvNetwork.GetByFilter<MedioConocimiento>
            ("medios", "tipo_medio_id", value.Id.ToString()!);
    }

    [RelayCommand]
    private void OnGuardarYSiguente(Type pageType)
    {
        _reporteService.Sync();
        _navigationService.Navigate(pageType);
    }
}