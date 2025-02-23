﻿using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Cebv.core.util.reporte.data;
using Cebv.features.formulario_cebv.circunstancias_desaparicion.data;
using Cebv.features.formulario_cebv.datos_complementarios.data;
using Cebv.features.formulario_cebv.desaparicion_forzada.data;
using Cebv.features.formulario_cebv.folio_expediente.data;
using Cebv.features.formulario_cebv.generacion_documentos.data;
using Cebv.features.formulario_cebv.vehiculos_involucrados.data;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Cebv.core.util.reporte.viewmodels;

/// <summary>
/// Representacion del reporte dado por el enpoint de
/// "/api/reportes"
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public partial class Reporte : ObservableValidator
{
    [JsonConstructor]
    public Reporte(
        int id,
        bool? estaTerminado,
        BasicResource? tipoReporte,
        MedioConocimiento? medioConocimiento,
        Estado? estado,
        TipoHipotesis? hipotesisOficial,
        string? tipoDesaparicion,
        Catalogo? institucionOrigen,
        ObservableCollection<Reportante> reportantes,
        ObservableCollection<Desaparecido> desaparecidos,
        HechosDesaparicion? hechosDesaparicion,
        DateTime? fechaCreacion,
        DateTime? fechaActualizacion,
        ControlOgpi? controlOgpi,
        ObservableCollection<Expediente> expedientes,
        DesaparicionForzada? desaparicionForzada,
        ObservableCollection<Perpetrador> perpetradores,
        DatoComplementario? datoComplementario,
        ObservableCollection<Vehiculo> vehiculos,
        ExpedienteFisico? expedienteFisico,
        GeneracionDocumento? generacionDocumento
    )
    {
        Id = id;
        EstaTerminado = estaTerminado;
        TipoReporte = tipoReporte;
        MedioConocimiento = medioConocimiento;
        Estado = estado;
        HipotesisOficial = hipotesisOficial;
        TipoDesaparicion = tipoDesaparicion;
        Reportantes = reportantes;
        Desaparecidos = desaparecidos;
        HechosDesaparicion = hechosDesaparicion;
        FechaCreacion = fechaCreacion;
        FechaActualizacion = fechaActualizacion;
        InstitucionOrigen = institucionOrigen;
        ControlOgpi = controlOgpi;
        Expedientes = expedientes;
        DesaparicionForzada = desaparicionForzada;
        Perpetradores = perpetradores;
        DatoComplementario = datoComplementario;
        Vehiculos = vehiculos;
        ExpedienteFisico = expedienteFisico;
        GeneracionDocumento = generacionDocumento;
    }

    public Reporte()
    {
        Id = -1;
    }

    [ObservableProperty, JsonProperty(PropertyName = "id")]
    private int _id;

    [ObservableProperty, JsonProperty(PropertyName = "esta_terminado")]
    private bool? _estaTerminado;

    [ObservableProperty, JsonProperty(PropertyName = "tipo_reporte")]
    [Required(ErrorMessage = "El campo tipo reporte es obligatorio")]
    private BasicResource? _tipoReporte;

    [ObservableProperty, JsonProperty(PropertyName = "area_atiende")]
    private Catalogo? _areaAtiende;

    [ObservableProperty, JsonProperty(PropertyName = "medio_conocimiento")]
    [Required(ErrorMessage = "Se debe seleccionar el medio de conocimiento especifico.")]
    private MedioConocimiento? _medioConocimiento;

    [ObservableProperty, JsonProperty(PropertyName = "estado")]
    private Estado? _estado;

    [ObservableProperty, JsonProperty(PropertyName = "zona_estado")]
    private Catalogo? _zonaEstado;

    [ObservableProperty, JsonProperty(PropertyName = "hipotesis_oficial")]
    private TipoHipotesis? _hipotesisOficial;

    [ObservableProperty, JsonProperty(PropertyName = "tipo_desaparicion")]
    private string? _tipoDesaparicion;

    [ObservableProperty, JsonProperty(PropertyName = "institucion_origen")]
    private Catalogo? _institucionOrigen;

    [ObservableProperty, JsonProperty(PropertyName = "reportantes")]
    private ObservableCollection<Reportante> _reportantes = [];

    [ObservableProperty, JsonProperty(PropertyName = "desaparecidos")]
    private ObservableCollection<Desaparecido> _desaparecidos = [];

    [ObservableProperty, JsonProperty(PropertyName = "hechos_desaparicion")]
    private HechosDesaparicion? _hechosDesaparicion;

    [ObservableProperty, JsonProperty(PropertyName = "hipotesis")]
    private ObservableCollection<Hipotesis> _hipotesis = [];

    [ObservableProperty, JsonProperty(PropertyName = "fecha_creacion")]
    private DateTime? _fechaCreacion;

    [ObservableProperty, JsonProperty(PropertyName = "fecha_actualizacion")]
    private DateTime? _fechaActualizacion;

    [ObservableProperty, JsonProperty("control_ogpi")]
    private ControlOgpi? _controlOgpi;

    [ObservableProperty, JsonProperty("expedientes")]
    private ObservableCollection<Expediente> _expedientes = [];

    [ObservableProperty, JsonProperty("desaparicion_forzada")]
    private DesaparicionForzada? _desaparicionForzada;

    [ObservableProperty, JsonProperty("perpetradores")]
    private ObservableCollection<Perpetrador> _perpetradores = [];

    [ObservableProperty, JsonProperty("dato_complementario")]
    private DatoComplementario? _datoComplementario;

    [ObservableProperty, JsonProperty("vehiculos")]
    private ObservableCollection<Vehiculo> _vehiculos = [];

    [ObservableProperty, JsonProperty("expediente_fisico")]
    private ExpedienteFisico? _expedienteFisico;

    [ObservableProperty, JsonProperty("generacion_documento")]
    private GeneracionDocumento? _generacionDocumento;
    
    public void ValidarReporteEncuadre()
    {
        ValidateProperty(MedioConocimiento, nameof(MedioConocimiento));
    }
}