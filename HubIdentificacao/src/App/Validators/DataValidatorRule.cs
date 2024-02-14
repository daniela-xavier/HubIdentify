using HubIdentificacao.src.App.Model;

namespace HubIdentificacao.src.App.Validators
{
    public class DataValidatorRule
    {
        public static void ValidateRequiredDocument(string document)
        {
            document = document.Replace(".", "").Replace("/", "").Replace("-", "");

            if (document.Length == 11)
            {
                if (!ValidateCPF(document))
                    throw new ArgumentException("ERR04: CPF inválido.", nameof(document));
            }
            else if (document.Length == 14)
            {
                if (!ValidateCNPJ(document))
                    throw new ArgumentException("ERR04: CNPJ inválido.", nameof(document));
            }
            else
            {
                throw new ArgumentException("ERR04: Tamanho do documento inválido.", nameof(document));
            }
        }

        private static bool ValidateCPF(string cpf)
        {
            if (cpf.Length != 11 || new string(cpf[0], 11) == cpf)
                return false;

            int[] weights = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int sum = 0;

            for (int i = 0; i < 9; i++)
                sum += int.Parse(cpf[i].ToString()) * weights[i];

            int remainder = sum % 11;
            int firstDigit = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cpf[9].ToString()) != firstDigit)
                return false;

            sum = 0;

            for (int i = 0; i < 10; i++)
                sum += int.Parse(cpf[i].ToString()) * weights[i];

            remainder = sum % 11;
            int secondDigit = remainder < 2 ? 0 : 11 - remainder;

            return int.Parse(cpf[10].ToString()) == secondDigit;
        }

        private static bool ValidateCNPJ(string cnpj)
        {
            if (cnpj.Length != 14 || new string(cnpj[0], 14) == cnpj)
                return false;

            int[] weights1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] weights2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            int sum = 0;

            for (int i = 0; i < 12; i++)
                sum += int.Parse(cnpj[i].ToString()) * weights1[i];

            int remainder = sum % 11;
            int firstDigit = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cnpj[12].ToString()) != firstDigit)
                return false;

            sum = 0;

            for (int i = 0; i < 13; i++)
                sum += int.Parse(cnpj[i].ToString()) * weights2[i];

            remainder = sum % 11;
            int secondDigit = remainder < 2 ? 0 : 11 - remainder;

            return int.Parse(cnpj[13].ToString()) == secondDigit;
        }
    }
}
