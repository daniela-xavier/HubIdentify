using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Microsoft.Extensions.Options;
using HubIdentificacao.src.App.Configs;
using HubIdentificacao.src.App.Validators;
using System.Collections.Generic;

namespace HubIdentificacao.Tests.ValidateNegotiateTests
{
    public class ValidateNegotiateTests
    {
        [Fact]
        public async Task Invoke_ComApiKeyValida_RetornaProximoMiddleware()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("AppSettings:ApiKey", "chaveApiKeyValida"),
                    new KeyValuePair<string, string>("AppSettings:ApiKeyIntegration", "chaveApiKeyIntegracaoValida")
                })
                .Build();

            var appSettings = new AppSettings(configuration);

            var appSettingsMock = new Mock<IOptions<AppSettings>>();
            appSettingsMock.Setup(x => x.Value).Returns(appSettings);

            var contextMock = new DefaultHttpContext();
            contextMock.Request.Path = "/identifyClient/negotiate";
            contextMock.Request.Headers["x-itau-apikey"] = "chaveApiKeyValida";

            var nextDelegate = new RequestDelegate(context =>
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                return Task.CompletedTask;
            });

            // Aqui, você deve passar diretamente a instância de AppSettings em vez de appSettingsMock.Object
            var validateNegotiate = new ValidateNegotiate(nextDelegate, appSettings);

            // Act
            await validateNegotiate.Invoke(contextMock);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, contextMock.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_SemApiKey_RetornaBadRequest()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("AppSettings:ApiKey", "chaveApiKeyValida"),
                    new KeyValuePair<string, string>("AppSettings:ApiKeyIntegration", "chaveApiKeyIntegracaoValida")
                })
                .Build();

            var appSettings = new AppSettings(configuration);

            var appSettingsMock = new Mock<IOptions<AppSettings>>();
            appSettingsMock.Setup(x => x.Value).Returns(appSettings);

            var contextMock = new DefaultHttpContext();
            contextMock.Request.Path = "/identifyClient/negotiate";

            var responseStream = new MemoryStream();
            contextMock.Response.Body = responseStream;

            var nextDelegate = new RequestDelegate(context =>
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                return Task.CompletedTask;
            });

            // Aqui, você deve passar diretamente a instância de AppSettings em vez de appSettingsMock.Object
            var validateNegotiate = new ValidateNegotiate(nextDelegate, appSettings);

            // Act
            await validateNegotiate.Invoke(contextMock);

            // Assert
            responseStream.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(responseStream).ReadToEnd();
            Assert.Equal(StatusCodes.Status400BadRequest, contextMock.Response.StatusCode);
            Assert.Equal("Cabeçalhos obrigatórios não estão presentes.", responseBody);
        }
    }
}
