using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;

namespace Inspekta.Client.Providers;

public sealed class JwtAuthorizationMessageHandler(ILocalStorageService localStorage) : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage = localStorage;
    private const string TokenKey = "authToken";

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization is null)
        {
            var token = await _localStorage.GetItemAsync<string>(TokenKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(token);

                    if (jwt.ValidTo > DateTime.UtcNow)
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    else
                        await _localStorage.RemoveItemAsync(TokenKey, cancellationToken);
                }
                catch
                {
                    await _localStorage.RemoveItemAsync(TokenKey, cancellationToken);
                }
            }
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            await _localStorage.RemoveItemAsync(TokenKey, cancellationToken);

        return response;
    }
}