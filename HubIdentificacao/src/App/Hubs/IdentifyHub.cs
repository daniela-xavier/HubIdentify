using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Net;
using Serilog;
using HubIdentificacao.src.App.Services;
using HubIdentificacao.src.App.Model;
using HubIdentificacao.src.App.Validators;
using HubIdentificacao.src.App.Dtos;

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

        public async Task IdentifyMessage(string documento, string agencia, string dataHora)
        {
            var response = Task.FromResult(new ResponseGeneral<IdentifyResponse>());

            string dadosRetorno = JsonSerializer.Serialize(new Data());

            try
            {
                // Validar os dados obrigatórios
                DataValidator.ValidateRequiredData(documento, agencia, dataHora);

                // Validar os dados de documento
                DataValidatorRule.ValidateRequiredDocument(documento);



                Data dados = new Data(documento, agencia, dataHora);

                response = _serviceIdentify.GetAPIIdentifyClient(dados);

                if (response.Result.CodeHttp == HttpStatusCode.Created)
                {
                    response.Result.Message = "Cliente identificado com sucesso";
                    dadosRetorno = JsonSerializer.Serialize(dados.UpdateData(response.Result.DataRetorn));
                }
                else
                {
                    response.Result.Message = "Cliente não identificado. HTTP: " + response.Result.CodeHttp + ". Error: " + response.Result.ErrorReturn;
                    dadosRetorno = JsonSerializer.Serialize(dadosRetorno);
                }

                _logger.LogInformation("IdentifyMessage invoked successfully.");
            }
            catch (Exception ex)
            {                
                response.Result.CodeHttp = HttpStatusCode.NoContent;
                response.Result.Message = ex.Message;
                //_logger.LogError(ex.Message);
               // Console.WriteLine(ex.Message);
            }

            await Clients.All.SendAsync("IdentifyMessage", response.Result.CodeHttp, response.Result.Message, dadosRetorno).ConfigureAwait(false);

        }

        public async Task UpdateIdentifyMessage(string clienteIdToken, string dataHora, string agencia, string numeroTicket, string dataHoraSenha)

        {
            var response = Task.FromResult(new ResponseGeneral<IdentifyResponse>());

            string dadosRetorno = JsonSerializer.Serialize(new Data());

            try
            {
                // Validar os dados obrigatórios
                DataValidator.ValidateRequiredTicketData(numeroTicket, dataHora, agencia, dataHoraSenha, clienteIdToken);

                try
                {
                    Data dados = new Data(clienteIdToken, dataHora, agencia, numeroTicket, dataHoraSenha);

                    response = _serviceIdentify.SetAPIUpdateIdentify(dados);

                    if (response.Result.CodeHttp == HttpStatusCode.Created)
                    {
                        response.Result.Message = "Cliente atualizado com sucesso";
                        dadosRetorno = JsonSerializer.Serialize(dados.UpdateData(response.Result.DataRetorn));
                    }
                    else
                    {
                        response.Result.Message = "Cliente não atualizado. HTTP: " + response.Result.CodeHttp + ". Error: " + response.Result.ErrorReturn;
                        dadosRetorno = JsonSerializer.Serialize(dadosRetorno);
                    }
                    _logger.LogInformation("UpdateIdentifyMessage invoked successfully.");

                }
                catch (Exception ex)
                {

                    _logger.LogError(ex.Message, "Ocorreu um erro inesperado em UpdateIdentifyMessage.");
                    Console.WriteLine(ex.Message);
                    response.Result.CodeHttp = HttpStatusCode.BadRequest;
                    response.Result.Message = ex.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Dados não preenchidos corretamente");
                Console.WriteLine(ex.Message);
                response.Result.CodeHttp = HttpStatusCode.NoContent;
                response.Result.Message = ex.Message;
            }

            await Clients.All.SendAsync("UpdateIdentifyMessage", response.Result.CodeHttp, response.Result.Message, dadosRetorno).ConfigureAwait(false);

        }


    }
}