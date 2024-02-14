using System.Text.Json;
using System.Text.Json.Serialization;
using HubIdentificacao.src.App.Model;

namespace HubIdentificacao.src.App.Validators
{
    public class DataTransformData
    {


        public string Serializer(Data data)
        {
            return JsonSerializer.Serialize(data);
        }

        public Data Deserializer(String data)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                //Lê valores numéricos definidos como string e escreve valores numéricos como strings
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                //Identa o JSON gerado
                WriteIndented = true,
                //Ignora propriedades com valor nulo ou padrão
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            if (string.IsNullOrEmpty(data))
            {
                throw new JsonException("Cliente inválido e/ou não identificado, retorno vazio.");
            }

            try
            {
                ApiResponse<Data>? responseObject = JsonSerializer.Deserialize<ApiResponse<Data>>(data, options);
                return responseObject?.Data;
            }
            catch (System.Exception)
            {
                throw;
            }

        }

        public string SerializerMask(Data data)
        {
            data.documento = DataMaskRule.MaskedRequiredDocument(data.documento);
            return JsonSerializer.Serialize(data);
        }

        public Data DeserializerMask(String data, String dataResp)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                //Lê valores numéricos definidos como string e escreve valores numéricos como strings
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                //Identa o JSON gerado
                WriteIndented = true,
                //Ignora propriedades com valor nulo ou padrão
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };
            if (string.IsNullOrEmpty(data))
            {
                throw new JsonException("Cliente inválido e/ou não identificado, retorno vazio.");
            }
            try
            {
                Data d = JsonSerializer.Deserialize<Data>(data, options);
                Data dR = JsonSerializer.Deserialize<ApiResponse<Data>>(dataResp, options).Data;
                d.UpdateData(dR);

                d.documento = DataMaskRule.MaskedRequiredDocument(d.documento);
                return d;
            }
            catch (ArgumentNullException ex)
            {
                // Lidere com argumentos nulos ou vazios
                throw new JsonException($" {ex.ParamName} - {ex.Message}");
            }
            catch (JsonException ex)
            {
                // Lidere com exceções de desserialização JSON
                throw new JsonException("Cliente inválido e/ou não identificado. Erro durante a desserialização JSON.", ex);
            }
            catch (Exception ex)
            {
                // Lidere com outras exceções
                throw new JsonException("Ocorreu um erro durante a operação de desserialização JSON.", ex);
            }
        }

        public Data DeserializerMask(String data)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                //Lê valores numéricos definidos como string e escreve valores numéricos como strings
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                //Identa o JSON gerado
                WriteIndented = true,
                //Ignora propriedades com valor nulo ou padrão
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };
            if (string.IsNullOrEmpty(data))
            {
                throw new JsonException("Cliente inválido e/ou não identificado, retorno vazio.");
            }
            try
            {
                Data? d = JsonSerializer.Deserialize<ApiResponse<Data>>(data, options).Data;
                DataMaskRule.MaskedRequiredDocument(d.documento);
                return d;
            }
            catch (System.Exception)
            {
                throw new JsonException("Cliente inválido e/ou não identificado. Erro durante a desserialização JSON.");
            }
        }

        internal Data? Masked(Data dados)
        {
            dados.documento = DataMaskRule.MaskedRequiredDocument(dados.documento);
            return dados;
        }
    }
}

