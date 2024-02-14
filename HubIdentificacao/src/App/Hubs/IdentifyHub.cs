using Microsoft.AspNetCore.SignalR;

using System.Net;
using Serilog;
using HubIdentificacao.src.App.Services;
using HubIdentificacao.src.App.Model;
using HubIdentificacao.src.App.Validators;
using HubIdentificacao.src.App.Dtos;
using System.Diagnostics;
using System.Reflection;

namespace HubIdentificacao.src.App.Hubs
{
    public class IdentifyHub : Hub, IHub
    {
        private readonly ILogger<IdentifyHub> _logger;

        private readonly IService _serviceIdentify;

        private readonly DataTransformData _dataTransform;

        public IdentifyHub(ILogger<IdentifyHub> logger, IService serviceIdentify,DataTransformData dataTransform)
        {
            _logger = logger;
            _serviceIdentify = serviceIdentify;
            _dataTransform = dataTransform;
        }

        public async Task IdentifyMessage(string documento, string agencia, string dataHora)
        {
            
            var response = Task.FromResult(new ResponseGeneral<IdentifyResponse>());

            string dadosRetorno = _dataTransform.Serializer(new Data());
            
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
                    dadosRetorno = _dataTransform.Serializer(response.Result.DataRetorn);                 
                    _logger.LogInformation("IdentifyMessage invoked successfully.");
                }
                else
                {
                    response.Result.Message = "Cliente não identificado.";
                    dadosRetorno = _dataTransform.Serializer(response.Result.DataRetorn);
                    _logger.LogInformation("IdentifyMessage invoked unsuccessfully.");
                }

            }
            catch (Exception ex)
            {
                response = Task.FromResult(new ResponseGeneral<IdentifyResponse>());
                response.Result.CodeHttp = HttpStatusCode.UnprocessableEntity;
                response.Result.Message = ex.Message;
                _logger.LogError(ex.Message);
            }
            await Clients.All.SendAsync("IdentifyMessage", response.Result.CodeHttp, response.Result.Message, dadosRetorno).ConfigureAwait(false);
        }

        public async Task UpdateIdentifyMessage(string clienteIdToken, string dataHora, string agencia, string numeroTicket, string dataHoraSenha)

        {
            var response = Task.FromResult(new ResponseGeneral<IdentifyResponse>());

            string dadosRetorno = _dataTransform.Serializer(new Data());

            try
            {
                // Validar os dados obrigatórios
                DataValidator.ValidateRequiredTicketData(numeroTicket, dataHora, agencia, dataHoraSenha, clienteIdToken);

                    Data dados = new Data(clienteIdToken, dataHora, agencia, numeroTicket, dataHoraSenha);

                    response = _serviceIdentify.SetAPIUpdateIdentify(dados);

                    if (response.Result.CodeHttp == HttpStatusCode.Created)
                    {
                        response.Result.Message = "Cliente atualizado com sucesso";
                    }
                    else
                    {
                        response.Result.Message = "Cliente não atualizado.";
                    }
                    _logger.LogInformation("UpdateIdentifyMessage invoked successfully.");

            }
            catch (Exception ex)
            {
                response = Task.FromResult(new ResponseGeneral<IdentifyResponse>());
                response.Result.CodeHttp = HttpStatusCode.UnprocessableEntity;
                response.Result.Message = ex.Message;
                _logger.LogError(ex.Message);
            }

            await Clients.All.SendAsync("UpdateIdentifyMessage", response.Result.CodeHttp, response.Result.Message, dadosRetorno).ConfigureAwait(false);

        }


    }
}