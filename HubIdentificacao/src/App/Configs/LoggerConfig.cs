using Serilog;
using Serilog.Events;

namespace HubIdentificacao.src.App.Configs
{
    public static class LoggerConfig
    {
        public static Serilog.ILogger CreateLogger(IConfiguration configuration)
        {           

            // Substitua o placeholder {AppDir} pelo diretório base do aplicativo
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var DataLog =  DateTime.Now.ToString("yyyyMMdd");
            configuration["Serilog:WriteTo:1:Args:path"] = configuration["Serilog:WriteTo:1:Args:path"].Replace("{AppDir}", appDir);
           
           
            // Verifica se o diretório de logs existe e, se não existir, cria-o
            var logsDirectory = Path.Combine(appDir, "logs");
            if (!Directory.Exists(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }

            var excludedWords = new List<string> { "Index", "root", "Request", "handler" }; // Lista de palavras-chave a serem excluídas
            var method = "IdentifyMessage";            

            //nome extraido da máquinas, porém é um defit pois podemos usar outros tipos de maquinas com outros nomes
            var hostname = "T1150012402001"; //Environment.MachineName;

            //agencia extraida do nome da máquina, porém é um defit pois podemos usar outros tipos de maquinas com outros nomes
            var agencia = hostname.Substring(2, Math.Min(hostname.Length, 4));

            var ApiKeyID = "ApiKeyID";

            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // Lê as configurações do arquivo JSON
                .Enrich.WithCorrelationId()     
                .Enrich.WithProperty("method", method) 
                .Enrich.WithProperty("agencia", agencia) 
                .Enrich.WithProperty("hostname", hostname) 
                .Enrich.WithProperty("ApiKeyID", ApiKeyID) 
                .Filter.ByExcluding(logEvent =>
                {
                    // Exclui mensagens de log INFO que contêm palavras-chave na lista
                    if (logEvent.Level == LogEventLevel.Information && 
                        excludedWords.Any(word => logEvent.MessageTemplate.Text.Contains(word)))
                    {
                        return true;
                    }
                    return false;
                })
                .CreateLogger();

        }
    }
}
