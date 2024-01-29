using System.Text.Json;
using System.Dynamic;
using HubIdentificacao.src.App.Dtos;
using HubIdentificacao.src.App.Model;
using HubIdentificacao.src.App.Interfaces;

namespace HubIdentificacao.src.App.Controllers
{
    public class Identify : IIdentifyDados
    {
        private Dados _dados;

        public Identify(Dados dados)
        {
            _dados = dados;
        }

        public Dados SetIdentify(Dados dados)
        {
            _dados = dados;
            return _dados;
        }
        
        public Dados SetUpdateIdentify(Dados dados)
        {
            _dados.idClientToken = dados.idClientToken;
            _dados.dataHoraLGPD = dados.dataHoraLGPD;
            _dados.numeroTicket = dados.numeroTicket;
            _dados.dataHoraEmissao = dados.dataHoraEmissao;
            return _dados;
        }

    }
}