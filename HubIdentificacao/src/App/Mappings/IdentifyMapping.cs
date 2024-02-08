using AutoMapper;
using HubIdentificacao.src.App.Controllers;
using HubIdentificacao.src.App.Dtos;
using HubIdentificacao.src.App.Model;

namespace HubIdentificacao.src.App.MAppings
{
    public class IdentifyMapping : Profile
    {
       
    public IdentifyMapping()
    {
        CreateMap(typeof (ResponseGeneral<>), typeof(ResponseGeneral<>));
        CreateMap<IdentifyResponse, Data>();
        CreateMap<Data, IdentifyResponse>();

    }

              
    }
}