using HubIdentificacao.src.App.Model;

namespace HubIdentificacao.src.App.Validators
{
    public class DataValidator
    {
        public static void ValidateRequiredData(string documento,string  dataHora,string agencia)
        {
            if (string.IsNullOrEmpty(documento) || string.IsNullOrEmpty(dataHora) || string.IsNullOrEmpty(agencia))
            {
                throw new ArgumentException("Documento, dataHora e agencia são campos obrigatórios.");
            }
        }

        public static void ValidateRequiredTicketData(string numeroTicket, string dataHora, string agencia, string dataHoraSenha, string clienteIdToken)
        {
            if (string.IsNullOrEmpty(numeroTicket) || string.IsNullOrEmpty(dataHora) || string.IsNullOrEmpty(agencia) || string.IsNullOrEmpty(dataHoraSenha) || string.IsNullOrEmpty(clienteIdToken))
            {
                throw new ArgumentException("Número do ticket, dataHora, agencia, dataHoraEmissao e clienteIdToken são campos obrigatórios.");
            }
        }
    }
}
