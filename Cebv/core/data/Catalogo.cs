using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Cebv.core.data;

public class CatalogosWrapped
{
    [property: JsonPropertyName("data")] public ObservableCollection<Catalogo> Data { get; set; }
}

public class Catalogo
{
    [property: JsonPropertyName("id")] public int Id { get; set; }
    [property: JsonPropertyName("nombre")] public string? Nombre { get; set; }
}