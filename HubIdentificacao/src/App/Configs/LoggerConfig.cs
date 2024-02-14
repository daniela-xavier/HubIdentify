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

            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // Lê as configurações do arquivo JSON
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
