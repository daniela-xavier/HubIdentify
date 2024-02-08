using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using HubIdentificacao.src.App.Services;
using HubIdentificacao.src.App.Model;
using System.Net;
using HubIdentificacao.src.App.Validators;

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
            try
            {
                // Validar os dados obrigatórios
                DataValidator.ValidateRequiredData(documento, agencia, dataHora);
            }
            catch (Exception ex)
            {                
                _logger.LogError(ex, "Dados obrigatórios não preenchidos");            
                Console.WriteLine(ex.Message);
                throw new Exception("Erro: HUBIDENT0003 - " + ex.Message);
            }                     
            
            
            try
            {              

                Data dados = new Data(documento, agencia, dataHora);
                string dadosRetorno = JsonSerializer.Serialize(new Data());

                var response = _serviceIdentify.GetAPIIdentifyClient(dados);

                if (response.Result.CodeHttp == HttpStatusCode.Created)
                    {
                        response.Result.Message = "Cliente identificado com sucesso";
                        dadosRetorno = JsonSerializer.Serialize(dados.UpdateData(response.Result.DataRetorn));  
                    }
                    else
                    {
                        response.Result.Message = "Cliente não identificado. HTTP: "+response.Result.CodeHttp+". Error: "+response.Result.ErrorReturn;
                        dadosRetorno = JsonSerializer.Serialize(dadosRetorno);                        
                    }
                
                _logger.LogInformation("IdentifyMessage invoked successfully.");
                await Clients.All.SendAsync("IdentifyMessage", response.Result.CodeHttp, response.Result.Message, dadosRetorno).ConfigureAwait(false);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex, "An unexpected error occurred in IdentifyMessage.");            
                Console.WriteLine(ex.Message);
                throw new Exception("Erro: HUBIDENT0003 - " + ex.Message);
            }
        }

        public async Task UpdateIdentifyMessage(string clienteIdToken, string dataHora, string agencia, string numeroTicket, string dataHoraSenha)

        {
             try
            {
                // Validar os dados obrigatórios
                DataValidator.ValidateRequiredTicketData(numeroTicket, dataHora, agencia, dataHoraSenha, clienteIdToken);
            }
            catch (Exception ex)
            {                
                _logger.LogError(ex, "Dados obrigatórios não preenchidos");            
                Console.WriteLine(ex.Message);
                throw new Exception("Erro: HUBIDENT0003 - " + ex.Message);
            }

            Data dados = new Data(clienteIdToken, dataHora, agencia, numeroTicket, dataHoraSenha);

            string dadosRetorno = JsonSerializer.Serialize(new Data());

            try
            {              
                var response = _serviceIdentify.SetAPIUpdateIdentify(dados);         

                if (response.Result.CodeHttp == HttpStatusCode.Created)
                    {
                        response.Result.Message = "Cliente atualizado com sucesso";
                        dadosRetorno = JsonSerializer.Serialize(dados.UpdateData(response.Result.DataRetorn));                       
                    }
                    else
                    {
                        response.Result.Message = "Cliente não atualizado. HTTP: "+response.Result.CodeHttp+". Error: "+response.Result.ErrorReturn;
                        dadosRetorno = JsonSerializer.Serialize(dadosRetorno);  
                    }
                _logger.LogInformation("UpdateIdentifyMessage invoked successfully.");
                await Clients.All.SendAsync("UpdateIdentifyMessage", response.Result.CodeHttp, response.Result.Message, dadosRetorno).ConfigureAwait(false);
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