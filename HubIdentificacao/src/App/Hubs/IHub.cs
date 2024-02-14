using Microsoft.AspNetCore.SignalR;

namespace HubIdentificacao.src.App.Hubs
{
    public interface IHub
    {
        Task IdentifyMessage(string documento, string agencia, string dataHora);

        Task UpdateIdentifyMessage(string clientIdToken, string dataHora, string agencia, string numeroTicket, string datahoraEmissao);
    }
}