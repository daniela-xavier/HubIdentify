using System.Text.RegularExpressions;
using HubIdentificacao.src.App.Model;

namespace HubIdentificacao.src.App.Validators
{
    public class DataMaskRule
    {
        public static string MaskedRequiredDocument(string documento)
        {
            // Expressão regular para verificar se há caracteres especiais
            Regex regex = new Regex("[^a-zA-Z0-9]");

            // Verifique se a string contém caracteres especiais
            if (regex.IsMatch(documento))
            {
                documento = Regex.Replace(documento, "[^a-zA-Z0-9]", "");      
            }            
            
            if (documento.Length == 11)
            {               
                return MaskCPF(documento);;
            }
            else if (documento.Length == 14)
            {                
                return MaskCNPJ(documento);;
            }
            else
            {
                throw new ArgumentException("ERR04: Tamanho do documento inválido.", nameof(documento));
            }
        }

        static string MaskCPF(string cpf)
        {
            return "XXX.XXX.XXX-" + cpf.Substring(9);
        }

        static string MaskCNPJ(string cnpj)
        {
            return "XX.XXX.XXX/XXXX-" + cnpj.Substring(12);
        }
    }
}
