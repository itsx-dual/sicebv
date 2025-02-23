using System.ComponentModel.DataAnnotations;
using Cebv.core.util.reporte.data;
using Cebv.core.util.reporte.viewmodels;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Cebv.features.formulario_cebv.datos_de_localizacion.data;

[JsonObject(MemberSerialization.OptIn)]
public partial class Localizacion : ObservableValidator
{
    [JsonConstructor]
    public Localizacion(
        int? id,
        int? desaparecidoId,
        Catalogo? sitio,
        Catalogo? area,
        Municipio? municipioLocalizacion,
        BasicResource? hipotesisDeb,
        DateTime? fechaLocalizacion,
        DateTime? fechaHallazgo,
        DateTime? fechaIdentificacionFamiliar,
        string? sintesisLocalizacion,
        string? descripcionCondicionPersona,
        string? descripcionCausasFallecimiento
    )
    {
        Id = id;
        DesaparecidoId = desaparecidoId;
        Sitio = sitio;
        Area = area;
        MunicipioLocalizacion = municipioLocalizacion;
        HipotesisDeb = hipotesisDeb;
        FechaLocalizacion = fechaLocalizacion;
        FechaHallazgo = fechaHallazgo;
        FechaIdentificacionFamiliar = fechaIdentificacionFamiliar;
        SintesisLocalizacion = sintesisLocalizacion;
        DescripcionCondicionPersona = descripcionCondicionPersona;
        DescripcionCausasFallecimiento = descripcionCausasFallecimiento;
    }

    public Localizacion()
    {
    }

    [ObservableProperty, JsonProperty("id")]
    private int? _id;

    [ObservableProperty, JsonProperty("desaparecido_id")]
    private int? _desaparecidoId;

    [ObservableProperty, JsonProperty("sitio")]
    private Catalogo? _sitio;

    [ObservableProperty, JsonProperty("area")]
    private Catalogo? _area;

    [ObservableProperty, JsonProperty("municipio_localizacion")]
    [Required(ErrorMessage = "El campo Municipio Localizacion es requerido")]
    private Municipio? _municipioLocalizacion;

    [ObservableProperty, JsonProperty("hipotesis_deb")]
    private BasicResource? _hipotesisDeb;

    [ObservableProperty, JsonProperty("fecha_localizacion")]
    [Required(ErrorMessage = "El campo Fecha Localizacion es requerido")]
    private DateTime? _fechaLocalizacion;

    [ObservableProperty, JsonProperty("fecha_hallazgo")]
    private DateTime? _fechaHallazgo;

    [ObservableProperty, JsonProperty("fecha_identificacion_familiar")]
    private DateTime? _fechaIdentificacionFamiliar;

    [ObservableProperty, JsonProperty("sintesis_localizacion")]
    [Required(ErrorMessage = "El campo Sintesis Localizacion es requerido")] 
    [MinLength(2, ErrorMessage = "El campo Sintesis Localizacion debe tener al menos 2 caracteres")]
    private string? _sintesisLocalizacion;

    [ObservableProperty, JsonProperty("descripcion_condicion_persona")]
    private string? _descripcionCondicionPersona;

    [ObservableProperty, JsonProperty("descripcion_causas_fallecimiento")]
    private string? _descripcionCausasFallecimiento;
    
    public void Validar()  
    {
        ValidateAllProperties();
    }
}