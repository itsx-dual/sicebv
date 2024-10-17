using System.Collections.ObjectModel;
using static Cebv.core.data.OpcionesCebv;
using Cebv.core.domain;
using Cebv.core.util;
using Cebv.core.util.navigation;
using Cebv.core.util.reporte;
using Cebv.core.util.reporte.viewmodels;
using Cebv.core.util.snackbar;
using Cebv.features.formulario_cebv.contexto.data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;
using Catalogo = Cebv.core.util.reporte.viewmodels.Catalogo;

namespace Cebv.features.formulario_cebv.contexto.presentation;

public partial class ContextoViewModel : ObservableObject
{
    private readonly ISnackbarService _snackBarService =
        App.Current.Services.GetService<ISnackbarService>()!;
    
    private readonly IReporteService _reporteService =
        App.Current.Services.GetService<IReporteService>()!;

    private readonly IFormularioCebvNavigationService _navigationService =
        App.Current.Services.GetService<IFormularioCebvNavigationService>()!;

    [ObservableProperty] private Reporte _reporte = null!;
    [ObservableProperty] private Desaparecido _desaparecido = new();
    [ObservableProperty] private Dictionary<string, bool?> _opcionesCebv = Opciones;

    public ContextoViewModel()
    {
        LoadAsync();
        
        Reporte = _reporteService.GetReporte();

        if (!Reporte.Desaparecidos.Any()) Reporte.Desaparecidos.Add(Desaparecido);
        Desaparecido = Reporte.Desaparecidos.FirstOrDefault()!;

        Desaparecido.Persona.ContextoFamiliar ??= new();
        Desaparecido.Persona.ContextoEconomico ??= new();
    }

    private async void LoadAsync()
    {
        Parentescos = await CebvNetwork.GetRoute<Catalogo>("parentescos");
        Pasatiempos = await CebvNetwork.GetRoute<Catalogo>("pasatiempos");
        Clubes = await CebvNetwork.GetRoute<Catalogo>("clubes");
        TiposRedesSociales = await CebvNetwork.GetRoute<Catalogo>("tipos-redes-sociales");
    }

    [ObservableProperty] private ObservableCollection<Catalogo> _parentescos = new();
    [ObservableProperty] private ObservableCollection<Catalogo> _pasatiempos = new();
    [ObservableProperty] private ObservableCollection<Catalogo> _clubes = new();
    [ObservableProperty] private ObservableCollection<Catalogo> _tiposRedesSociales = new();

    [ObservableProperty] private Familiar _familiar = new();

    [ObservableProperty] private Catalogo? _pasatiempo;
    [ObservableProperty] private Catalogo? _club;
    [ObservableProperty] private Amistad _amistad = new();

    /**
     * Familiares
     */
    [RelayCommand]
    private void OnGuardarFamiliar()
    {
        if (Familiar.Nombre is null) return;

        Desaparecido.Persona.Familiares.Add(Familiar);

        Familiar = new();
    }

    [RelayCommand]
    private void OnEliminarFamiliar(Familiar familiar)
    {
        Desaparecido.Persona.Familiares.Remove(familiar);
    }

    /**
     * Pasatiempos
     */
    [RelayCommand]
    private void OnGuardarPasatiempo()
    {
        if (Pasatiempo is null) return;

        var pasatiempo = new PasatiempoPersona(null, null, Pasatiempo);

        Desaparecido.Persona.Pasatiempos.Add(pasatiempo);

        Pasatiempo = null;
    }

    [RelayCommand]
    private void OnEliminarPasatiempo(PasatiempoPersona pasatiempo)
    {
        Desaparecido.Persona.Pasatiempos.Remove(pasatiempo);
    }

    /**
     * Clubes
     */
    [RelayCommand]
    private void OnGuardarClub()
    {
        if (Club is null) return;

        var club = new ClubPersona(null, null, Club);

        Desaparecido.Persona.Clubes.Add(club);

        Club = null;
    }

    [RelayCommand]
    private void OnEliminarClub(ClubPersona club)
    {
        Desaparecido.Persona.Clubes.Remove(club);
    }

    [RelayCommand]
    private void OnGuardarAmistad()
    {
        if (Amistad.Nombre is null) return;

        Desaparecido.Persona.Amistades.Add(Amistad);

        Amistad = new();
    }

    [RelayCommand]
    private void OnEliminarAmistad(Amistad amistad)
    {
        Desaparecido.Persona.Amistades.Remove(amistad);
    }


    /**
     * Comando para guardar y navegar a la siguiente pagina
     */
    [RelayCommand]
    private void OnGuardarYSiguente(Type pageType)
    {
        _reporteService.Sync();
        _navigationService.Navigate(pageType);
    }
}