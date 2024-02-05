using System.Dynamic;
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

        public async Task<ResponseGeneral<IdentifyResponse>> GetIdentifyClient(Dados dados)
        {
            var uri = new Uri($"http://localhost:3000/gestaoatendimento-identificacao/v1/cadastros/");          

            var request = new HttpRequestMessage(HttpMethod.Post, uri);

            var json = JsonSerializer.Serialize(dados);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                        
            var response = new ResponseGeneral<IdentifyResponse>();

            using (var client = new HttpClient())            
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var responseApi = await client.SendAsync(request);

                var contenResp = responseApi.Content.ReadAsStringAsync().Result;                

                if (responseApi.IsSuccessStatusCode)
                    {                        
                        response.CodeHttp = responseApi.StatusCode;
                        response.DataRetorn =  JsonSerializer.Deserialize<Dados>(contenResp);
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

            var uri = new Uri($"localhost:3000/gestaoatendimento-identificacao/v1/cadastros/");  

            var request = new HttpRequestMessage(HttpMethod.Patch,uri);                       

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
                        var objResponse = JsonSerializer.Deserialize<Dados>(contenResp);
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

