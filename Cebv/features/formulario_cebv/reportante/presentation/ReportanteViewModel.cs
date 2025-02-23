using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Cebv.core.domain;
using Cebv.core.modules.persona.presentation;
using Cebv.core.util;
using Cebv.core.util.enums;
using Cebv.core.util.navigation;
using Cebv.core.util.reporte;
using Cebv.core.util.reporte.viewmodels;
using Cebv.core.util.snackbar;
using Cebv.features.formulario_cebv.reportante.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Controls;
using static Cebv.core.data.OpcionesCebv;
using static Cebv.core.util.CollectionsHelper;
using static Cebv.core.util.enums.TipoContacto;
using static Cebv.core.util.UiState;
using Catalogo = Cebv.core.util.reporte.viewmodels.Catalogo;

namespace Cebv.features.formulario_cebv.reportante.presentation;

public partial class ReportanteViewModel : ObservableValidator
{
    private readonly ISnackbarService _snackBarService =
        App.Current.Services.GetService<ISnackbarService>()!;
    
    private readonly IReporteService _reporteService =
        App.Current.Services.GetService<IReporteService>()!;

    private readonly IFormularioCebvNavigationService _navigationService =
        App.Current.Services.GetService<IFormularioCebvNavigationService>()!;

    [ObservableProperty] private UiState _uiState;

    [ObservableProperty] private Dictionary<string, bool?> _opcionesCebv = Opciones;
    [ObservableProperty] private Reporte _reporte = null!;
    [ObservableProperty] private Reportante _reportante = new();
    [ObservableProperty] private PersonaViewModel _persona = new();
    [ObservableProperty] private Direccion? _direccion;

    [ObservableProperty] private ObservableCollection<Catalogo> _gruposVulnerables = new();
    [ObservableProperty] private ObservableCollection<Catalogo> _colectivos = new();
    [ObservableProperty] private ObservableCollection<Estado> _estados = new();
    [ObservableProperty] private ObservableCollection<Municipio> _municipios = new();
    [ObservableProperty] private ObservableCollection<Asentamiento> _asentamientos = new();

    [ObservableProperty] private Estado? _estadoSelected;
    
    [ObservableProperty] 
    [Required(ErrorMessage = "El campo Municipio es requerido")]
    private Municipio? _municipioSelected;
    
    [ObservableProperty] private Catalogo? _grupoVulnerableSelected;

    [ObservableProperty] private string? _noTelefonoFijo;
    [ObservableProperty] private string? _observacionesFijo;

    [ObservableProperty] private string? _nombreContacto;
    [ObservableProperty] private string? _observacionesContacto;

    [ObservableProperty] private int? _edadAproxmida;

    [ObservableProperty] private bool _tieneTelefonosMoviles;
    [ObservableProperty] private bool _tieneTelefonosFijos;
    [ObservableProperty] private bool _tieneCorreos;
    [ObservableProperty] private bool _tienePertenenciasGrupales;

    public ReportanteViewModel()
    {
        UiState = Edit;
        InitAsync();

        Reporte = _reporteService.GetReporte();
        if (!Reporte.Reportantes.Any()) Reporte.Reportantes.Add(Reportante);
        Reportante = Reporte.Reportantes.FirstOrDefault()!;

        Direccion = Reportante.Persona.Direcciones.FirstOrDefault();
        EnsureObjectExists(ref _direccion, Reportante.Persona.Direcciones);

        EdadAproxmida = CalculateAge(Reportante.Persona.FechaNacimiento);

        TieneTelefonosMoviles = Reportante.Persona.Telefonos.Any(x => (bool)x.EsMovil!);
        TieneTelefonosFijos = Reportante.Persona.Telefonos.Any(x => (bool)!x.EsMovil!);
        TieneCorreos = Reportante.Persona.Contactos.Any(x => x.Tipo == CorreoElectronico);
        TienePertenenciasGrupales = Reportante.Persona.GruposVulnerables.Any();

        Reportante.Persona.Estudios ??= new();
        Reportante.Persona.ContextoFamiliar ??= new();
    }

    private async void InitAsync()
    {
        Colectivos = await CebvNetwork.GetRoute<Catalogo>("colectivos");
        GruposVulnerables = await CebvNetwork.GetRoute<Catalogo>("grupos-vulnerables");
        Estados = await CebvNetwork.GetRoute<Estado>("estados");

        var reportante = _reporteService.GetReporte().Reportantes.FirstOrDefault();

        var est =
            reportante?.Persona.Direcciones.FirstOrDefault()?.Asentamiento?.Municipio?.Estado;

        var mpio =
            reportante?.Persona.Direcciones.FirstOrDefault()?.Asentamiento?.Municipio;

        if (est is not null)
        {
            EstadoSelected = est;
            Municipios = await CebvNetwork.GetByFilter<Municipio>("municpios", "estado_id", est.Id);
        }

        if (mpio is not null)
        {
            MunicipioSelected = mpio;
            Asentamientos = await CebvNetwork.GetByFilter<Asentamiento>("asentamientos", "municipio_id", mpio.Id);
        }
    }

    private static int? CalculateAge(DateTime? birthDate)
    {
        if (!birthDate.HasValue) // Check if birthDate is null
            return null;

        int years = DateTime.Now.Year - birthDate.Value.Year;
        if (birthDate.Value.AddYears(years) > DateTime.Now)
            years--;
        return years;
    }

    partial void OnEdadAproxmidaChanged(int? value)
    {
        if (Reporte.Reportantes.Count > 1) Reportante.EdadEstimadaAnhos = value;
    }

    async partial void OnEstadoSelectedChanged(Estado? value)
    {
        if (value is null) return;
        Municipios = await CebvNetwork.GetByFilter<Municipio>("municipios", "estado_id", value.Id);
    }

    async partial void OnMunicipioSelectedChanged(Municipio? value)
    {
        if (value is null) return;
        Asentamientos = await CebvNetwork.GetByFilter<Asentamiento>("asentamientos", "municipio_id", value.Id);
    }
    

    [RelayCommand]
    private void OnAddTelefonoFijo()
    {
        if (NoTelefonoFijo is null) return;

        Reportante.Persona.Telefonos.Add(new Telefono
        {
            Numero = NoTelefonoFijo,
            Observaciones = ObservacionesFijo,
            EsMovil = false,
            Compania = null
        });

        NoTelefonoFijo = null;
        ObservacionesFijo = null;
    }

    [RelayCommand]
    private void OnAddContacto()
    {
        if (NombreContacto is null) return;

        Reportante.Persona.Contactos.Add(new Contacto
        {
            Nombre = NombreContacto,
            Observaciones = ObservacionesContacto,
            Tipo = CorreoElectronico
        });

        NombreContacto = null;
        ObservacionesContacto = null;
    }

    [RelayCommand]
    private void OnAddGrupoVulnerabilidad()
    {
        if (GrupoVulnerableSelected is null) return;
        Reportante.Persona.GruposVulnerables.Add(GrupoVulnerableSelected);
        GrupoVulnerableSelected = null;
    }

    [RelayCommand]
    private void OnRemoveGrupoVulnerabilidad(Catalogo catalogo) =>
        Reportante.Persona.GruposVulnerables.Remove(catalogo);

    [RelayCommand]
    private void OnEliminarTelefono(Telefono telefono) => Reportante.Persona.Telefonos.Remove(telefono);

    [RelayCommand]
    private void OnEliminarContacto(Contacto contacto) => Reportante.Persona.Contactos.Remove(contacto);

    private bool _cancelar = true;
    private async Task<bool> EnlistarCampos()
    {
        bool confirmacion = false;

        var properties = ReportanteDictionary.GetReportante(Reportante, Reporte, this, TelefonoMovil); 
        var emptyElements = ListEmptyElements.GetEmptyElements(properties);
        
        if (emptyElements.Count > 0)
        {
            var dialogo = new ShowDialog();

            // Esperar a que se muestre el ContentDialog
            await dialogo.ShowContentDialogCommand.ExecuteAsync(emptyElements);
            
            if (dialogo.Confirmacion == "Guardar") confirmacion = true;
            else if (dialogo.Confirmacion == "No guardar") return _cancelar = false;
        }
        else confirmacion = true;

        return confirmacion;
    }
    
    public void Validate() => ValidateAllProperties();
    
    [RelayCommand]
    private async Task OnGuardarYSiguiente(Type pageType)
    {
        if (ReportanteDictionary.ValidateReportante(this, Reportante) == Validaciones.ExistenErrores)
        {
            string errores = ListEmptyElements.GetAllValidationMessages(new List<ObservableValidator> 
                { this, Reportante.Persona });
            
            _snackBarService.Show(
                "Error en los campos",
                "Por favor, revise los campos obligatorios y corrija los siguientes errores:\n" + errores,
                ControlAppearance.Danger,
                new SymbolIcon(SymbolRegular.Warning48),
                new TimeSpan(0, 0, 10));
            return;
        }
        
        if (ReportanteDictionary.ValidateReportante(this, Reportante) == Validaciones.HayInstanciasNulas)
        {
            _snackBarService.Show(
                "Instancias nulas",
                "Instancias nulas aun no cargadas, por favor espere a que se carguen",
                ControlAppearance.Danger,
                new SymbolIcon(SymbolRegular.Warning48),
                new TimeSpan(0, 0, 10));
            return;
        }
        
        if (!await EnlistarCampos())
        {
            if (!_cancelar) _navigationService.Navigate(pageType);
                
            return;
        }
        
        // Si hay algo en los campos de telefonos, se agregan antes de sincronizar,
        // este es el comportamiento que la CEBV espera.
        AddTelefonoMovilCommand.Execute(null);
        AddTelefonoFijoCommand.Execute(null);
        AddContactoCommand.Execute(null);
        AddGrupoVulnerabilidadCommand.Execute(null);

        _reporteService.Sync();
        _navigationService.Navigate(pageType);
    }
};