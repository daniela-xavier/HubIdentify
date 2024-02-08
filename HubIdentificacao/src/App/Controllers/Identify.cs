using System.Text.Json;
using System.Dynamic;
using HubIdentificacao.src.App.Dtos;
using HubIdentificacao.src.App.Model;
using HubIdentificacao.src.App.Interfaces;

namespace HubIdentificacao.src.App.Controllers
{
    public class Identify : IIdentifyDados
    {
        private Data _dados;

        public Identify(Data dados)
        {
            _dados = dados;
        }

        public Data SetIdentify(Data dados)
        {
            _dados = dados;
            return _dados;
        }
        
        public Data SetUpdateIdentify(Data dados)
        {
            _dados.clienteIdToken = dados.clienteIdToken;
            _dados.dataHora = dados.dataHora;
            _dados.numeroTicket = dados.numeroTicket;
            _dados.dataHoraSenha = dados.dataHoraSenha;
            return _dados;
        }

    }
}