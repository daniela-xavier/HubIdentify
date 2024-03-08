using System.Net;
using System.Net.Http.Headers;
using System.Text;
using HubIdentificacao.src.App.Dtos;
using HubIdentificacao.src.App.Interfaces;
using HubIdentificacao.src.App.Validators;
using HubIdentificacao.src.App.Model;
using HubIdentificacao.src.App.Configs;

namespace HubIdentificacao.src.App.Controllers
{
    public class IdentifyAPI : IIdentifyAPI
    {
        private readonly ILogger _logger;

        private readonly DataTransformData _dataTransform;

        private readonly AppSettings _appSettings;

        public IdentifyAPI(ILogger<IdentifyAPI> logger, DataTransformData dataTransform, AppSettings appSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataTransform = dataTransform;
            _appSettings = appSettings;
        }

        public async Task<ResponseGeneral<IdentifyResponse>> GetIdentifyClient(Data dados)
        {
            var uri = new Uri(_appSettings.HubUrl+_appSettings.Rota+_appSettings.Dominio);

            var request = new HttpRequestMessage(HttpMethod.Post, uri);

            var json = _dataTransform.Serializer(dados);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = new ResponseGeneral<IdentifyResponse>();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var responseApi = await client.SendAsync(request);

                var contenResp = responseApi.Content.ReadAsStringAsync().Result;

                try
                {
                    if (responseApi.IsSuccessStatusCode)
                    {

                        if (responseApi.StatusCode == HttpStatusCode.Created)
                        {                            
                            response.DataRetorn = _dataTransform.DeserializerMask(json,contenResp);
                            response.CodeHttp = responseApi.StatusCode;
                        }
                        else if (responseApi.StatusCode == HttpStatusCode.NoContent)
                        {
                            response.DataRetorn = _dataTransform.Masked(dados);
                            response.CodeHttp = responseApi.StatusCode;
                        }
                        else if (responseApi.StatusCode == HttpStatusCode.ResetContent)
                        {
                            response.DataRetorn = dados;
                            response.CodeHttp = responseApi.StatusCode;
                        }
                        else
                        {
                            throw new ArgumentException("Cliente não identificado/inválido. Erro durante o retorno das APIs");
                        }

                    }
                    else
                    {
                        response.CodeHttp = responseApi.StatusCode;
                        _logger.LogError("Erro durante as chamadas de API" + response.CodeHttp);
                        Console.WriteLine("Erro durante as chamadas de API" + response.CodeHttp);
                    }
                }
                catch (System.Exception)
                {
                    response.CodeHttp = HttpStatusCode.UnprocessableEntity;
                    _logger.LogError("Erro durante as chamadas de API" + response.CodeHttp);
                    Console.WriteLine("Erro durante as chamadas de API" + response.CodeHttp);
                }

                return response;
            }
        }

        public async Task<ResponseGeneral<IdentifyResponse>> SetUpdateClient(Data dados)
        {

            var uri = new Uri(_appSettings.HubUrl+_appSettings.Rota+_appSettings.Dominio+"/"+dados.clienteIdToken);

            var request = new HttpRequestMessage(HttpMethod.Patch, uri);

            var json = _dataTransform.Serializer(dados);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = new ResponseGeneral<IdentifyResponse>();

            using (var client = new HttpClient())
            {
                var responseApi = await client.SendAsync(request);

                try
                {
                    if (responseApi.IsSuccessStatusCode)
                    {

                        if (responseApi.StatusCode == HttpStatusCode.Created)
                        {
                            response.DataRetorn = dados;
                            response.CodeHttp = responseApi.StatusCode;
                        }
                        else if (responseApi.StatusCode == HttpStatusCode.NoContent)
                        {
                            response.DataRetorn = dados;
                            response.CodeHttp = responseApi.StatusCode;
                        }
                        else if (responseApi.StatusCode == HttpStatusCode.ResetContent)
                        {
                            response.DataRetorn = dados;
                            response.CodeHttp = responseApi.StatusCode;
                        }
                        else
                        {
                            throw new ArgumentException("ERR04: Cliente não identificado/inválido. Erro durante o retorno das APIs");
                        }

                    }
                    else
                    {
                        response.CodeHttp = responseApi.StatusCode;
                        _logger.LogError("Erro durante as chamadas de API" + response.CodeHttp);
                        Console.WriteLine("Erro durante as chamadas de API" + response.CodeHttp);
                    }
                }
                catch (System.Exception)
                {
                    response.CodeHttp = responseApi.StatusCode;
                    _logger.LogError("Erro durante as chamadas de API" + response.CodeHttp);
                    Console.WriteLine("Erro durante as chamadas de API" + response.CodeHttp);
                }

                return response;
            }
        }
    }
}

