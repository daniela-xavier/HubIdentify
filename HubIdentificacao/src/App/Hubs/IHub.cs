using Microsoft.AspNetCore.SignalR;

namespace HubIdentificacao.src.App.Hubs
{
    public interface IHub
    {
        Task IdentifyMessage(string documento, string agencia, string dataHoraLGPD);

        Task UpdateIdentifyMessage(string clientIdToken, string dataHoraLGPD, string agencia, string numeroTicket, string datahoraEmissao);
    }
}