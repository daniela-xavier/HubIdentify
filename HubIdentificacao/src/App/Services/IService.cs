using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using HubIdentificacao.src.App.Dtos;
using HubIdentificacao.src.App.Model;

namespace HubIdentificacao.src.App.Services
{
public interface IService
    {
        Guid Id { get; }
       
        public Task<ResponseGeneral<IdentifyResponse>> GetAPIIdentifyClient(Data dados);

        public Task<ResponseGeneral<IdentifyResponse>> SetAPIUpdateIdentify(Data dados);

    }
}