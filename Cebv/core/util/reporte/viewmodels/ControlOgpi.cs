using Cebv.core.util.reporte.data;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Cebv.core.util.reporte.viewmodels;

[JsonObject(MemberSerialization.OptIn)]
public partial class ControlOgpi : ObservableObject
{
    [JsonConstructor]
    public ControlOgpi(
        int? id,
        int? reporteId,
        BasicResource? estatusRndpno,
        string? folioFub,
        string? autoridadIngresaFub,
        DateTime? fechaCodificacion,
        string? nombreCodificador,
        string? observaciones,
        string? numeroTarjeta
    )
    {
        Id = id;
        ReporteId = reporteId;
        EstatusRndpno = estatusRndpno;
        FolioFub = folioFub;
        AutoridadIngresaFub = autoridadIngresaFub;
        FechaCodificacion = fechaCodificacion;
        NombreCodificador = nombreCodificador;
        Observaciones = observaciones;
        NumeroTarjeta = numeroTarjeta;
    }

    public ControlOgpi()
    {
    }

    [ObservableProperty, JsonProperty(PropertyName = "id")]
    private int? _id;

    [ObservableProperty, JsonProperty(PropertyName = "reporte_id")]
    private int? _reporteId;

    [ObservableProperty, JsonProperty("estatus_rndpno")]
    private BasicResource? _estatusRndpno;

    [ObservableProperty, JsonProperty("folio_fub")]
    private string? _folioFub;

    [ObservableProperty, JsonProperty("autoridad_ingresa_fub")]
    private string? _autoridadIngresaFub;

    [ObservableProperty, JsonProperty(PropertyName = "fecha_codificacion")]
    private DateTime? _fechaCodificacion;

    [ObservableProperty, JsonProperty(PropertyName = "nombre_codificador")]
    private string? _nombreCodificador;

    [ObservableProperty, JsonProperty(PropertyName = "observaciones")]
    private string? _observaciones;

    [ObservableProperty, JsonProperty(PropertyName = "numero_tarjeta")]
    private string? _numeroTarjeta;
}