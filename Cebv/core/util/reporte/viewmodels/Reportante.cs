﻿using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Cebv.core.util.reporte.viewmodels;

[JsonObject(MemberSerialization.OptIn)]
public partial class Reportante : ObservableObject
{
    [JsonConstructor]
    public Reportante(int? id,
        int? reporte_id,
        Persona? persona,
        Catalogo? parentesco,
        Catalogo? colectivo,
        bool denuncia_anonima,
        bool? informacion_consentimiento,
        bool? informacion_exclusiva_busqueda,
        bool? publicacion_registro_nacional,
        bool? publicacion_boletin,
        bool? pertenencia_colectivo,
        string? participacion_busquedas,
        string? descripcion_extorsion,
        string? descripcion_donde_proviene,
        string? informacion_relevante,
        int? edad_estimada,
        DateTime? created_at,
        DateTime? updated_at)
    {
        Id = id;
        ReporteId = reporte_id;
        Persona = persona;
        Parentesco = parentesco;
        Colectivo = colectivo;
        DenunciaAnonima = denuncia_anonima;
        InformacionConsentimiento = informacion_consentimiento;
        InformacionExclusivaBusqueda = informacion_exclusiva_busqueda;
        PublicacionRegistroNacional = publicacion_registro_nacional;
        PublicacionBoletin = publicacion_boletin;
        PertenenciaColectivo = pertenencia_colectivo;
        InformacionRelevante = informacion_relevante;
        ParticipacionBusquedas = participacion_busquedas;
        DescripcionExtorsion = descripcion_extorsion;
        DescripcionDondeProviene = descripcion_donde_proviene;
        EdadEstimada = edad_estimada;
        CreatedAt = created_at;
        UpdatedAt = updated_at;
    }

    public Reportante() { }
    
    [ObservableProperty, JsonProperty(PropertyName = "id")]
    private int? _id;
    
    [ObservableProperty, JsonProperty(PropertyName = "reporte_id")]
    private int? _reporteId;
    
    [ObservableProperty, JsonProperty(PropertyName = "persona")]
    private Persona _persona = new();
    
    [ObservableProperty, JsonProperty(PropertyName = "colectivo")]
    private Catalogo _colectivo;
    
    [ObservableProperty, JsonProperty(PropertyName = "pertenencia_colectivo")]
    private bool? _pertenenciaColectivo;
    
    [ObservableProperty, JsonProperty(PropertyName = "parentesco")]
    private Catalogo? _parentesco;
    
    [ObservableProperty, JsonProperty(PropertyName = "denuncia_anonima")]
    private bool _denunciaAnonima;
    
    [ObservableProperty, JsonProperty(PropertyName = "informacion_consentimiento")]
    private bool? _informacionConsentimiento;
    
    [ObservableProperty, JsonProperty(PropertyName = "informacion_exclusiva_busqueda")]
    private bool? _informacionExclusivaBusqueda;
    
    [ObservableProperty, JsonProperty(PropertyName = "publicacion_registro_nacional")]
    private bool? _publicacionRegistroNacional;
    
    [ObservableProperty, JsonProperty(PropertyName = "publicacion_boletin")]
    private bool? _publicacionBoletin;
    
    [ObservableProperty, JsonProperty(PropertyName = "informacion_relevante")]
    private string? _informacionRelevante;
    
    [ObservableProperty, JsonProperty(PropertyName = "participacion_busquedas")]
    private string? _participacionBusquedas;
    
    [ObservableProperty, JsonProperty(PropertyName = "descripcion_extorsion")]
    private string? _descripcionExtorsion;
    
    [ObservableProperty, JsonProperty(PropertyName = "descripcion_donde_proviene")]
    private string? _descripcionDondeProviene;
    
    [ObservableProperty, JsonProperty(PropertyName = "edad_estimada")]
    private int? _edadEstimada;
    
    [ObservableProperty, JsonProperty(PropertyName = "created_at")]
    private DateTime? _createdAt;
    
    [ObservableProperty, JsonProperty(PropertyName = "updated_at")]
    private DateTime? _updatedAt;
    
}