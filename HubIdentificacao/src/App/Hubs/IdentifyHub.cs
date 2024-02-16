using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Net;
using HubIdentificacao.src.App.Services;
using HubIdentificacao.src.App.Model;
using HubIdentificacao.src.App.Validators;
using HubIdentificacao.src.App.Dtos;
using System.Diagnostics;

namespace HubIdentificacao.src.App.Hubs
{
    public class IdentifyHub : Hub, IHub
    {
        private readonly ILogger<IdentifyHub> _logger;

        private readonly IService _serviceIdentify;

        private readonly DataTransformData _dataTransform;

        public IdentifyHub(ILogger<IdentifyHub> logger, IService serviceIdentify, DataTransformData dataTransform)
        {
            _logger = logger;
            _serviceIdentify = serviceIdentify;
            _dataTransform = dataTransform;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("connected");
            await base.OnConnectedAsync();
        }

        public async Task<object> GetHandshakeDetails()
        {
            var stopwatch = Stopwatch.StartNew();

            // Lógica para obter os detalhes do handshake
            var accessToken = Context.GetHttpContext().Request.Query["access_token"];
            var apiKey = Context.GetHttpContext().Request.Headers["x-itau-visual-apikey"];
            var correlationId = Context.GetHttpContext().Request.Headers["x-itau-visual-correlationID"];

            Console.WriteLine("accessToken:" + accessToken);
            Console.WriteLine("apiKey:" + apiKey);
            Console.WriteLine("correlationId:" + correlationId);

            return new
            {
                accessToken = accessToken,
                headers = new
                {
                    apiKey = apiKey,
                    correlationID = correlationId
                }
            };
            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;
            _logger.LogInformation("IdentifyMessage invoked successfully. {time}ms", time);
        }
        public async Task IdentifyMessage(string documento, string agencia, string dataHora)
        {
            var stopwatch = Stopwatch.StartNew();
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

            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;
            _logger.LogInformation("Processamento de IdentifyMessage. {time}ms", time);
            await Clients.All.SendAsync("IdentifyMessage", response.Result.CodeHttp, response.Result.Message, dadosRetorno).ConfigureAwait(false);
        }

        public async Task UpdateIdentifyMessage(string clienteIdToken, string dataHora, string agencia, string numeroTicket, string dataHoraSenha)

        {
            var stopwatch = Stopwatch.StartNew();
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

            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;
            _logger.LogInformation("Processamento de UpdateIdentifyMessage. {time}ms", time);
            await Clients.All.SendAsync("UpdateIdentifyMessage", response.Result.CodeHttp, response.Result.Message, dadosRetorno).ConfigureAwait(false);

        }


    }
}