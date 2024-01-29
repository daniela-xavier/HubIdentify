using Microsoft.AspNetCore.Mvc;
using HubIdentificacao.src.App.Services;
using HubIdentificacao.src.App.Model;
using HubIdentificacao.src.App.Controllers;
using HubIdentificacao.src.App.Dtos;
using HubIdentificacao.src.App.Interfaces;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using AutoMapper;

namespace HubIdentificacao.src.App.Services
{
public class ServiceIdentify : IService
    {
        Guid IService.Id { get; } = Guid.NewGuid();

        private readonly IIdentifyDados _dados;

        private readonly IIdentifyAPI _dadosApi;

        private readonly IMapper _mapper;

        public ServiceIdentify(IIdentifyDados dados, IIdentifyAPI dadosApi, IMapper mapper)
        {
            _dados = dados;
            _dadosApi = dadosApi;
            _mapper = mapper;
        }
       

        public async Task<ResponseGeneral<IdentifyResponse>> GetAPIIdentifyClient(Dados dados)
        {
           var d = await _dadosApi.GetIdentifyClient(dados);
           return _mapper.Map<ResponseGeneral<IdentifyResponse>>(d);
        }

        public async Task<ResponseGeneral<IdentifyResponse>> SetAPIUpdateIdentify(Dados dados)
        {
            var d = await _dadosApi.SetUpdateClient(dados);
            return _mapper.Map<ResponseGeneral<IdentifyResponse>>(d);
        }
    }

}