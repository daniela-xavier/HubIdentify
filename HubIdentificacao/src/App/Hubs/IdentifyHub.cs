using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using HubIdentificacao.src.App.Services;
using HubIdentificacao.src.App.Model;
using System.Net;

namespace HubIdentificacao.src.App.Hubs
{
    public class IdentifyHub : Hub
    {
        //private readonly ILogger<IdentifyHub> _logger;        
        private readonly IService _serviceIdentify;

        public IdentifyHub(ServiceIdentify serviceIdentify)
        {
            _serviceIdentify = serviceIdentify;
        }

        public async Task IdentifyMessage(string documento, string agencia, string dataHoraLGPD)

        {
            Dados dados = new Dados(documento, agencia, dataHoraLGPD);

            var request = JsonSerializer.Serialize<Dados>(dados);

            try
            {
                var response = _serviceIdentify.GetAPIIdentifyClient(dados);

                if (response.Result.CodeHttp == HttpStatusCode.OK)
                {
                    response.Result.Message = "Cliente identificado com sucesso";
                }
                else
                {
                    response.Result.Message = "Cliente não identificado. HTTP: "+response.Result.CodeHttp+". Error: "+response.Result.ErrorReturn;
                }


                await Clients.All.SendAsync("IdentifyMessage", request, response).ConfigureAwait(false);

            }
            catch (Exception ex)


            {
                Console.WriteLine(ex.Message);
                throw new Exception("Erro: HUBIDENT0003 - " + ex.Message);
            }
        }



        public async Task UpdateIdentifyMessage(string clientIdToken, string dataHoraLGPD, string agencia, string numeroTicket, string datahoraEmissao)

        {
            Dados dados = new  Dados(clientIdToken, dataHoraLGPD, agencia, numeroTicket, datahoraEmissao);

            var request = JsonSerializer.Serialize<Dados>(dados);

            try
            {
                var response = _serviceIdentify.SetAPIUpdateIdentify(dados);
                
                if (response.Result.CodeHttp == HttpStatusCode.OK)
                {
                    response.Result.Message = "Cliente atualizado com sucesso";
                }
                else
                {
                    response.Result.Message = "Cliente não atualizado. HTTP: "+response.Result.CodeHttp+". Error: "+response.Result.ErrorReturn;
                }

                await Clients.All.SendAsync("UpdateIdentifyMessage", request, response).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Erro: HUBIDENT0005 - " + ex.Message);
            }


        }
    }
}