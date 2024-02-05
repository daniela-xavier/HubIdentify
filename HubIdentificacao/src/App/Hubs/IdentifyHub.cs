using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using HubIdentificacao.src.App.Services;
using HubIdentificacao.src.App.Model;
using System.Net;

namespace HubIdentificacao.src.App.Hubs
{
    public class IdentifyHub : Hub, IHub
    {
        private readonly ILogger<IdentifyHub> _logger;

        private readonly IService _serviceIdentify;

        public IdentifyHub(ILogger<IdentifyHub> logger, IService serviceIdentify)
        {
            _logger = logger;
            _serviceIdentify = serviceIdentify;
        }

        public async Task IdentifyMessage(string documento, string agencia, string dataHoraLGPD)
        {
            Dados dados = new Dados(documento, agencia, dataHoraLGPD);
            string dadosReturno = JsonSerializer.Serialize(new Dados());

            try
            {
                var response = _serviceIdentify.GetAPIIdentifyClient(dados);
                if (response.Result.CodeHttp == HttpStatusCode.Created)
                    {
                        response.Result.Message = "Cliente identificado com sucesso";
                        dadosReturno = JsonSerializer.Serialize(response.Result.DataRetorn);
                    }
                    else
                    {
                        response.Result.Message = "Cliente não identificado. HTTP: "+response.Result.CodeHttp+". Error: "+response.Result.ErrorReturn;
                    }
                
                _logger.LogInformation("IdentifyMessage invoked successfully.");
                await Clients.All.SendAsync("IdentifyMessage", response.Result.CodeHttp, response.Result.Message, dadosReturno).ConfigureAwait(false);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex, "An unexpected error occurred in IdentifyMessage.");            
                Console.WriteLine(ex.Message);
                throw new Exception("Erro: HUBIDENT0003 - " + ex.Message);
            }
        }

        public async Task UpdateIdentifyMessage(string clientIdToken, string dataHoraLGPD, string agencia, string numeroTicket, string datahoraEmissao)

        {
            Dados dados = new Dados(clientIdToken, dataHoraLGPD, agencia, numeroTicket, datahoraEmissao);

            try
            {
                var response = _serviceIdentify.SetAPIUpdateIdentify(dados);                
                if (response.Result.CodeHttp == HttpStatusCode.Created)
                    {
                        response.Result.Message = "Cliente atualizado com sucesso";
                    }
                    else
                    {
                        response.Result.Message = "Cliente não atualizado. HTTP: "+response.Result.CodeHttp+". Error: "+response.Result.ErrorReturn;
                    }
                _logger.LogInformation("UpdateIdentifyMessage invoked successfully.");
                await Clients.All.SendAsync("UpdateIdentifyMessage", response.Result.CodeHttp, response.Result.Message, response.Result.DataRetorn).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in UpdateIdentifyMessage."); 
                Console.WriteLine(ex.Message);
                throw new Exception("Erro: HUBIDENT0005 - " + ex.Message);
            }


        }
    }
}