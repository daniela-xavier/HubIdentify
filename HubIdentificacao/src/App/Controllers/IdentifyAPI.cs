using System.Dynamic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HubIdentificacao.src.App.Dtos;
using HubIdentificacao.src.App.Interfaces;
using HubIdentificacao.src.App.Model;
using Microsoft.AspNetCore.Mvc;

namespace HubIdentificacao.src.App.Controllers
{
    public class IdentifyAPI : IIdentifyAPI
    {
        private readonly ILogger _logger;

        public IdentifyAPI(ILogger<IdentifyAPI> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

        public async Task<ResponseGeneral<IdentifyResponse>> GetIdentifyClient(Data dados)
        {
            var uri = new Uri($"http://localhost:3000/gestaoatendimento-identificacao/v1/cadastros/");

            var request = new HttpRequestMessage(HttpMethod.Post, uri);

            var json = JsonSerializer.Serialize(dados);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = new ResponseGeneral<IdentifyResponse>();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                //Lê valores numéricos definidos como string e escreve valores numéricos como strings
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                //Identa o JSON gerado
                WriteIndented = true,
                //Ignora propriedades com valor nulo ou padrão
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var responseApi = await client.SendAsync(request);

                var contenResp = responseApi.Content.ReadAsStringAsync().Result;

                if (responseApi.IsSuccessStatusCode)
                {
                    response.CodeHttp = responseApi.StatusCode;

                    if (response.CodeHttp == HttpStatusCode.Created)
                    {
                        try
                        {
                            // Desserializando o conteúdo JSON em um objeto do tipo Data
                            var dataObject = JsonSerializer.Deserialize<ApiResponse<Data>>(contenResp, options);
                            response.DataRetorn = dataObject?.Data;

                        }
                        catch (JsonException ex)
                        {
                            _logger.LogError(ex, "Erro durante a desserialização JSON");
                            Console.WriteLine("Erro durante a desserialização JSON" + ex.Message);
                        }

                    }
                    else
                    {

                        _logger.LogError(response.CodeHttp + "Cliente não identificado. ");
                        Console.WriteLine(response.CodeHttp + "Cliente não identificado. ");
                        throw new ArgumentException("ERR04: Cliente não identificado/inválido.");
                    }

                }
                else
                {
                    response.CodeHttp = responseApi.StatusCode;
                    //response.ErrorReturn = JsonSerializer.Deserialize<ExpandoObject>(contenResp);
                    _logger.LogError("Erro durante as chamadas de API" + response.CodeHttp);
                    Console.WriteLine("Erro durante as chamadas de API" + response.CodeHttp);
                }
                return response;
            }
        }

        public async Task<ResponseGeneral<IdentifyResponse>> SetUpdateClient(Data dados)
        {

            var uri = new Uri($"localhost:3000/gestaoatendimento-identificacao/v1/cadastros/${{dados.documento}}");

            var request = new HttpRequestMessage(HttpMethod.Patch, uri);

            var json = JsonSerializer.Serialize(dados);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = new ResponseGeneral<IdentifyResponse>();

            using (var client = new HttpClient())
            {
                var responseApi = await client.SendAsync(request);
                var contenResp = await responseApi.Content.ReadAsStringAsync();

                if (responseApi.IsSuccessStatusCode)
                {
                    response.CodeHttp = responseApi.StatusCode;
                    var objResponse = JsonSerializer.Deserialize<Data>(contenResp);
                    response.DataRetorn = objResponse;
                }
                else
                {
                    response.CodeHttp = responseApi.StatusCode;
                    response.ErrorReturn = JsonSerializer.Deserialize<ExpandoObject>(contenResp);
                }
                return response;
            }
        }
    }
}

