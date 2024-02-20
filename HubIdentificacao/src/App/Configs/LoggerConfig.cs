using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HubIdentificacao.src.App.Configs
{
    public class LoggerConfig
    {
        private readonly AppSettings _appSettings;

        public LoggerConfig(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public Serilog.ILogger CreateLogger(IConfiguration configuration)
        {
            var DataLog = DateTime.Now.ToString("yyyyMMdd");

            // Usa a instância de AppSettings injetada para acessar as configurações
            var logPath = configuration["Serilog:WriteTo:1:Args:path"];

            var appDir = AppDomain.CurrentDomain.BaseDirectory;

            // Certifique-se de que o caminho não está vazio ou nulo antes de usar
            if (!string.IsNullOrEmpty(logPath))
            {
                logPath = logPath.Replace("{AppDir}", appDir);
                configuration["Serilog:WriteTo:1:Args:path"] = logPath;

                // Verifica se o diretório de logs existe e, se não existir, cria-o
                var logsDirectory = Path.Combine(logPath, "logs");
                if (!Directory.Exists(logsDirectory))
                {
                    Directory.CreateDirectory(logsDirectory);
                }

                var excludedWords = new List<string> { "Index", "root", "Request", "handler" }; // Lista de palavras-chave a serem excluídas
                var method = "IdentifyMessage";

                var ApiKeyID = configuration["API:ApiKey"];

                return new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration) // Lê as configurações do arquivo JSON
                    .Enrich.WithCorrelationId()
                    .Enrich.WithProperty("method", method)
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
            else
            {
                // Lida com o cenário em que o caminho do log é nulo ou vazio
                Console.WriteLine("Caminho do log não encontrado nas configurações.");
                // Retorna um logger padrão se o caminho do log não estiver disponível
                return new LoggerConfiguration().CreateLogger();
            }
        }
    }
}
