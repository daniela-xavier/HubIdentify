using System.Dynamic;
using System.Text.Json;
using HubIdentificacao.src.App.Dtos;
using HubIdentificacao.src.App.Interfaces;
using HubIdentificacao.src.App.Model;

namespace HubIdentificacao.src.App.Controllers
{
    public class IdentifyAPI : IIdentifyAPI
    {

        public async Task<ResponseGeneral<IdentifyResponse>> GetIdentifyClient(Dados dados)
        {
            var uri = new Uri("localhost:3000/gestaoatendimento-identificacao/v1/cadastros/identificacao");                         

            var request = new HttpRequestMessage(HttpMethod.Post, uri);

            var content = JsonSerializer.Serialize(dados);
                       
            request.Content = new StringContent(content);            

            var response = new ResponseGeneral<IdentifyResponse>();

            using (var client = new HttpClient())
            {
                var responseApi = await client.SendAsync(request);
                var contenResp = await responseApi.Content.ReadAsStringAsync();
                var objResponse = JsonSerializer.Deserialize<IdentifyResponse>(contenResp);

                if (responseApi.IsSuccessStatusCode)
                {
                    response.CodeHttp = responseApi.StatusCode;
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

        public async Task<ResponseGeneral<IdentifyResponse>> SetUpdateClient(Dados dados)
        {         

            var uri = new Uri($"localhost:3000/gestaoatendimento-identificacao/v1/cadastros/:{dados.documento}");  

            var request = new HttpRequestMessage(HttpMethod.Patch,uri);                       

            var content = JsonSerializer.Serialize(dados);
                       
            request.Content = new StringContent(content);            

            var response = new ResponseGeneral<IdentifyResponse>();

            using (var client = new HttpClient())
            {
                var responseApi = await client.SendAsync(request);
                var contenResp = await responseApi.Content.ReadAsStringAsync();
                var objResponse = JsonSerializer.Deserialize<IdentifyResponse>(contenResp);

                if (responseApi.IsSuccessStatusCode)
                {
                    response.CodeHttp = responseApi.StatusCode;
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

