﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Role_Based_Authorization.Data;
using Microsoft.EntityFrameworkCore;
using Role_Based_Authorization.Entities;

namespace Role_Based_Authorization.AuthenticationAndAuthorization;

[Obsolete]
internal class BasicAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock,
    BloggingContext DbContext)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.NoResult();

        if (!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var authHeader))
            return AuthenticateResult.Fail("Unknown scheme.");

        var encodedCredentials = authHeader.Parameter;
        var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials!));
        var userNameAndPassword = decodedCredentials.Split(':');

        var user = await DbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.UserName == userNameAndPassword[0]);

        if (user is null || user.Password != userNameAndPassword[1])
            return AuthenticateResult.Fail("Invalid username or password.");

        // create user roles claims.
        var claims = await DbContext
            .Set<UserRole>()
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => new Claim(ClaimTypes.Role, ur.Role))
            .ToListAsync();

        // add name id claim.
        claims.Add(new(ClaimTypes.NameIdentifier, user.Id.ToString()));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);

    }
}