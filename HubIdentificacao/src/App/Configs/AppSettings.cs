using Microsoft.Extensions.Configuration;
namespace HubIdentificacao.src.App.Configs;
public class AppSettings
{
    public string ApiKey { get; set; }
    public string ApiKeyIntegration { get; set; }
    public string Path { get; set; }
    public string HubUrl { get; set; }
    public string Rota { get; set; }
    public string Dominio { get; set; }
    public string ClientSecret { get; set; }
    public string ClientId { get; set; }

    public AppSettings(IConfiguration configuration)
    {
        configuration.GetSection("API").Bind(this);
        configuration.GetSection("Serilog").Bind(this);
        configuration.GetSection("AllowedOrigins").Bind(this);
    }

    public static void SetupSerilogPath(IConfiguration configuration,AppSettings appSettings)
    {
        var appDir = AppDomain.CurrentDomain.BaseDirectory;
        //var appSettings = new AppSettings(configuration); // Instanciando AppSettings com as configurações

        var logPath = configuration["Serilog:WriteTo:1:Args:path"]; // Acessando a propriedade Path através da instância de AppSettings

        if (logPath != null)
        {
            logPath = logPath.Replace("{AppDir}", appDir);
            appSettings.Path = logPath;
            configuration["Serilog:WriteTo:1:Args:path"] = logPath;
        }
        else
        {
            // Lida com o cenário em que a propriedade Path é nula
            // Aqui você pode registrar um aviso ou tomar outra ação apropriada
            Console.WriteLine("A propriedade 'Path' em appSettings é nula.");
        }
    }
}
