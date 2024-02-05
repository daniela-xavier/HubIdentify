using System.Text.Json.Serialization;

namespace HubIdentificacao.src.App.Model
{
    public class Dados
    {
        public Dados()
        {
        }

        public Dados(string? documento, string? agencia, string? dataHoraLGPD)
        {
            this.documento = documento;
            this.agencia = agencia;
            this.dataHoraLGPD = dataHoraLGPD;
        }

        public Dados(string? idClientToken, string? dataHoraLGPD,string? agencia, string? numeroTicket, string? dataHoraEmissao)
        {            
            this.agencia = agencia;
            this.idClientToken = idClientToken;
            this.dataHoraLGPD = dataHoraLGPD;
            this.numeroTicket = numeroTicket;
            this.dataHoraEmissao = dataHoraEmissao;
        }

        [JsonPropertyName("documento")]
        public string? documento { get; set; }

        [JsonPropertyName("agencia")]
        public string? agencia { get; set; }

        [JsonPropertyName("clienteIdToken")]
        public string? idClientToken { get; set; }

        [JsonPropertyName("dataHora")]
        public string? dataHoraLGPD { get; set; }

        [JsonPropertyName("ticket")]
        public string? numeroTicket { get; set; }

        [JsonPropertyName("dataHoraEmissao")]
        public string? dataHoraEmissao { get; set; }        

        [JsonPropertyName("tipoDocumento")]
        public string? tipoDocumento { get; set; }        

        [JsonPropertyName("idPrioridade")]
        public string? idPrioridade { get; set; }

        [JsonPropertyName("idCategoria")]
        public string? idCategoria { get; set; }

        [JsonPropertyName("idServico")]
        public string? idServico { get; set; }

        [JsonPropertyName("idAtividade")]
        public string? idAtividade { get; set; }

        [JsonPropertyName("nome")]
        public string? nome { get; set; }

      
                
    }
    
}