using System.Dynamic;
using System.Net;
using HubIdentificacao.src.App.Controllers;
using HubIdentificacao.src.App.Model;
using HubIdentificacao.src.App.Interfaces;

namespace HubIdentificacao.src.App.Dtos
{
    public class IdentifyResponse    
    {
        private readonly IIdentifyDados _dados;

         public IdentifyResponse(Identify dados){
            _dados = dados;
        }

    }
}