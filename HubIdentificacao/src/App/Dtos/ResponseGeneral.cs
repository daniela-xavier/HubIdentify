using System.Dynamic;
using System.Net;
using System.Text.Json.Serialization;
using HubIdentificacao.src.App.Hubs;
using HubIdentificacao.src.App.Model;

namespace HubIdentificacao.src.App.Dtos
{
    public class ResponseGeneral<T> where T : class
    {       

        [JsonIgnore]
        public HttpStatusCode CodeHttp { get; set; }

        [JsonIgnore]
        public Data? DataRetorn { get; set; }

        [JsonIgnore]
        public ExpandoObject? ErrorReturn { get; set; }

        [JsonIgnore]
        public string? Message { get; set; }

    }

}