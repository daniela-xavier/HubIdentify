using HubIdentificacao.src.App.Dtos;
using HubIdentificacao.src.App.Model;

namespace HubIdentificacao.src.App.Interfaces
{
    public interface IIdentifyAPI    
    {       
                                
        Task<ResponseGeneral<IdentifyResponse>> GetIdentifyClient(Dados dados);

        Task<ResponseGeneral<IdentifyResponse>> SetUpdateClient(Dados dados);       


    }
}

 