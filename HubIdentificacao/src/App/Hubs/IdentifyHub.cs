using HubIdentificacao.src.App.Services;
using HubIdentificacao.src.App.Model;
using HubIdentificacao.src.App.Validators;
using HubIdentificacao.src.App.Dtos;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace HubIdentificacao.src.App.Hubs
{
    public class IdentifyHub : Hub, IHub
    {
        private readonly ILogger<IdentifyHub> _logger;

        private readonly IService _serviceIdentify;

        private readonly DataTransformData _dataTransform;

        private readonly DataAutorization _dataAutorization;

        public IdentifyHub(ILogger<IdentifyHub> logger, IService serviceIdentify, DataTransformData dataTransform, DataAutorization dataAutorization)
        {
            _logger = logger;
            _serviceIdentify = serviceIdentify;
            _dataTransform = dataTransform;
            _dataAutorization = dataAutorization;
        }
        public async Task<object> GetHandshakeDetails()
        {
            var stopwatch = Stopwatch.StartNew();

            // Lógica para obter o detalhes do handshake
            //var accessToken = Guid.NewGuid().ToString(); // Gere um token de autenticação aleatório

            var accessToken = Context.GetHttpContext().Request.Headers["token"];

            var correlationId = Context.GetHttpContext().Request.Headers["Sec-Websocket-Key"];
            correlationId += Guid.NewGuid().ToString();

            _logger.LogInformation("accessToken: {accessToken}", accessToken);
            _logger.LogInformation("correlationId: {correlationId}", correlationId);

            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;
            _logger.LogInformation("GetHandshakeDetails invoked successfully. {time}ms", time);

            return new
            {
                accessToken = accessToken,
                headers = new
                {
                    correlationID = correlationId
                }
            };
        }

        public async Task IdentifyMessage(string documento, string agencia, string dataHora)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = Task.FromResult(new ResponseGeneral<IdentifyResponse>());

            string dadosRetorno = _dataTransform.Serializer(new Data());

            string dataHoraAtual = DateTime.Now.ToString("_yyyyMMdd_HHmmss");

            var traceId = Context.GetHttpContext().Request.Query["Id"].ToString()+dataHoraAtual;

            var accessToken = Context.GetHttpContext().Request.Query["access_token"];

            try
            {
                // Validar token
                bool tokenValido = _dataAutorization.ValidateToken(accessToken);
                if (tokenValido)
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
                        //_logger.LogInformation("IdentifyMessage invoked successfully."+" - "+$"TraceId: {traceId}");
                    }
                    else
                    {
                        response.Result.Message = "Cliente não identificado.";
                        dadosRetorno = _dataTransform.Serializer(response.Result.DataRetorn);
                        // _logger.LogInformation("IdentifyMessage invoked unsuccessfully."+" - "+$"TraceId: {traceId}");
                    }
                }

            }
            catch (SecurityTokenException ex)
            {
                response = Task.FromResult(new ResponseGeneral<IdentifyResponse>());
                response.Result.CodeHttp = HttpStatusCode.Unauthorized;
                response.Result.Message = ex.Message;
                _logger.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                response = Task.FromResult(new ResponseGeneral<IdentifyResponse>());
                response.Result.CodeHttp = HttpStatusCode.UnprocessableEntity;
                response.Result.Message = ex.Message;
                _logger.LogError(ex.Message);
            }

            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;
            _logger.LogInformation("Processamento de IdentifyMessage. {time}ms  - TraceId: {TraceId}", time, traceId);
            await Clients.All.SendAsync("IdentifyMessage", response.Result.CodeHttp, response.Result.Message, dadosRetorno).ConfigureAwait(false);
        }

        public async Task UpdateIdentifyMessage(string clienteIdToken, string dataHora, string agencia, string numeroTicket, string dataHoraSenha)

        {
            var stopwatch = Stopwatch.StartNew();
            var response = Task.FromResult(new ResponseGeneral<IdentifyResponse>());

            string dadosRetorno = _dataTransform.Serializer(new Data());

            string dataHoraAtual = DateTime.Now.ToString("_yyyyMMdd_HHmmss");

            var traceId = Context.GetHttpContext().Request.Query["Id"].ToString()+dataHoraAtual;

            var accessToken = Context.GetHttpContext().Request.Query["access_token"];

            try
            {
                // Validar token
                bool tokenValido = _dataAutorization.ValidateToken(accessToken);
                if (tokenValido)
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
            }
            catch (Exception ex)
            {
                response = Task.FromResult(new ResponseGeneral<IdentifyResponse>());
                response.Result.CodeHttp = HttpStatusCode.UnprocessableEntity;
                response.Result.Message = ex.Message;
                _logger.LogError(ex.Message);
            }

            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;
            _logger.LogInformation("Processamento de UpdateIdentifyMessage. {time}ms  - TraceId: {TraceId}", time, traceId);
            await Clients.All.SendAsync("UpdateIdentifyMessage", response.Result.CodeHttp, response.Result.Message, dadosRetorno).ConfigureAwait(false);

        }


    }
}