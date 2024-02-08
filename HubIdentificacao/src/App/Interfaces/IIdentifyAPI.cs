using HubIdentificacao.src.App.Dtos;
using HubIdentificacao.src.App.Model;

namespace HubIdentificacao.src.App.Interfaces
{
    public interface IIdentifyAPI    
    {       
                                
        Task<ResponseGeneral<IdentifyResponse>> GetIdentifyClient(Data dados);

        Task<ResponseGeneral<IdentifyResponse>> SetUpdateClient(Data dados);       


    }
}

 