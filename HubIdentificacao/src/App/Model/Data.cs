using System.Text.Json.Serialization;

namespace HubIdentificacao.src.App.Model
{
    public class Data
    {

        [JsonPropertyName("nome")]
        public string? nome { get; set; }

        [JsonPropertyName("idPrioridade")]
        public string? idPrioridade { get; set; }

        [JsonPropertyName("clienteIdToken")]
        public string? clienteIdToken { get; set; }

        [JsonPropertyName("idCategoria")]
        public string? idCategoria { get; set; }

        [JsonPropertyName("documento")]
        public string? documento { get; set; }

        [JsonPropertyName("agencia")]
        public string? agencia { get; set; }

        [JsonPropertyName("dataHora")]
        public string? dataHora { get; set; }

        [JsonPropertyName("ticket")]
        public string? numeroTicket { get; set; }

        [JsonPropertyName("dataHoraSenha")]
        public string? dataHoraSenha { get; set; }

        [JsonPropertyName("tipoDocumento")]
        public string? tipoDocumento { get; set; }

        [JsonPropertyName("idServico")]
        public string? idServico { get; set; }

        [JsonPropertyName("idAtividade")]
        public string? idAtividade { get; set; }

        public Data()
        {
        }

        public Data(string? documento, string? agencia, string? dataHora)
        {
            this.documento = documento;
            this.agencia = agencia;
            this.dataHora = dataHora;
        }


        public Data(string clienteIdToken, string dataHora, string? agencia, string? numeroTicket, string? dataHoraSenha)
        {
            this.agencia = agencia;
            this.clienteIdToken = clienteIdToken;
            this.dataHora = dataHora;
            this.numeroTicket = numeroTicket;
            this.dataHoraSenha = dataHoraSenha;
        }

        public Data(string nome, string idPrioridade, string clienteIdToken, string idCategoria)
        {
            this.nome = nome;
            this.idPrioridade = idPrioridade;
            this.clienteIdToken = clienteIdToken;
            this.idCategoria = idCategoria;
        }

        public Data UpdateData(Data? dataRetorn)
        {

            if (dataRetorn != null)
            {
                this.clienteIdToken = dataRetorn.clienteIdToken;
                this.idCategoria = dataRetorn.idCategoria;
                this.idPrioridade = dataRetorn.idPrioridade;
                this.nome = dataRetorn.nome;

                return this;
            }
            else
            {
                // Se não houver dados fornecidos para atualização, retornar os próprios dados sem modificação
                return this;
            }
        }
    }

}