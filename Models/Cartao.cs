using System.Text.Json.Serialization;

namespace Trellura.Models
{
  public class Cartao
  {
    public Cartao(string titulo)
    {
      Titulo = titulo;
      Status = StatusDoCartao.Tarefa;
    }

    public Cartao(int id, string titulo)
    {
      Id = id;
      Titulo = titulo;
      Status = StatusDoCartao.Tarefa;
    }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Titulo { get; set; }
    
    [JsonPropertyName("status")]
    public StatusDoCartao Status { get; set; }
  }
}