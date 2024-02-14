using System.Text.Json.Serialization;

namespace HubIdentificacao.src.App.Model
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }

}