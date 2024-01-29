using HubIdentificacao.src.App.Dtos;
using HubIdentificacao.src.App.Model;

namespace HubIdentificacao.src.App.Interfaces
{
    public interface IIdentifyDados       
    {              

        Dados SetIdentify(Dados dados);        

        Dados SetUpdateIdentify(Dados dados);


    }
}

 