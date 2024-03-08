using HubIdentificacao.src.App.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HubIdentificacao.src.App.Validators
{
    public class DataAutorization
    {
        private readonly AppSettings _appSettings;

        private readonly ILogger<DataAutorization> _logger;


        public DataAutorization(AppSettings appSettings, ILogger<DataAutorization> _logger)
        {
            _appSettings = appSettings;
        }

        public bool ValidateToken(string accessToken)
        {
            //Console.WriteLine("Avaliando token");           
            var issuer = _appSettings.issuer;
            try
            {                
                var jwtToken = new JwtSecurityToken(accessToken);

                // Validar exp (tempo de expiração)
                if (jwtToken.Payload.Exp != null)
                {
                    var exp = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(jwtToken.Payload.Exp));
                    if (exp <= DateTimeOffset.UtcNow)
                    {
                        _logger.LogError("Token expirado" + accessToken);
                        throw new SecurityTokenException("Token expirado");
                    }
                }

                // Validar iat (tempo de emissão)
                if (jwtToken.Payload.Iat != null)
                {
                    var iat = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(jwtToken.Payload.Iat));
                    if (iat > DateTimeOffset.UtcNow)
                    {
                        _logger.LogError("Token incorreto iat. token" + accessToken);
                        throw new SecurityTokenException("Token incorreto iat");
                    }
                }

                /*// Validar escopo
                var requiredScopes = new[] { "gestao-atendimento-passante.write", "gestao-atendimento-passante.read" }; // Defina seus escopos requeridos aqui
                if (!requiredScopes.All(scope => jwtToken.Payload.Claims.Any(c => c.Type == "scope" && c.Value == scope)))
                {
                    throw new SecurityTokenException("Escopo não encontrado ou incorreto");
                }*/

                // Validar sujeito
                if (string.IsNullOrEmpty(jwtToken.Payload.Sub))
                {
                    _logger.LogError("sub não encontrado ou incorreto" + accessToken);
                    throw new SecurityTokenException("sub não encontrado ou incorreto");
                }

                //Console.WriteLine("Token de acesso válido recebido: " + accessToken);
                // Aqui você pode processar a mensagem recebida com o token de acesso válido
                return true;
            }
            catch (SecurityTokenException)
            {
                _logger.LogError("Token inválido ou não encontrado" + accessToken);
                throw new SecurityTokenException("Token inválido ou não encontrado");
            }
        }
    }    
}
