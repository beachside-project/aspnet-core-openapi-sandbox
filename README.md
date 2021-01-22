# 🚀 ASP.NET Core x OpenAPI sandbox project

## Projects Overview

### 🍖 [AspnetCore31](./src/AspnetCore31)

- ASP.NET Core 3.1 Web API
- Swashbuckle
- Xml comment

More detail: [BEACHSIDE BLOG - ASP.NET Core で Open API (Swagger) の設定の基礎 (.NET Core 3.1 と .NET 5)](https://blog.beachside.dev/entry/2021/01/22/123000)

### 🍖 [AspnetCore50](./src/AspnetCore50)

Same as "AspnetCore31".

- ASP.NET Core 5.0 Web API
- Swashbuckle
- Xml comment

More detail: [BEACHSIDE BLOG - ASP.NET Core で Open API (Swagger) の設定の基礎 (.NET Core 3.1 と .NET 5)](https://blog.beachside.dev/entry/2021/01/22/123000)

### 🍖 [AspnetCore50WithAzureAdAuthorizationCode](./src/AspnetCore50WithAzureAdAuthorizationCode)


- ASP.NET Core 5.0 Web API (Azure AD Authorization)
- Swashbuckle
- Swagger UI Authorize (Azure AD authorization - Authorization Code Flow with PKCE)

Need to update appsettings.json as following:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AzureAd": {
    "clientId": "! Set Azure AD application's client id",
    "metadataAddress": "! Set Azure AD application's 'OpenID Connect metadata document' endpoint",
    "AuthorizationUrl": "! Set Azure AD application's 'OAuth 2.0 authorization endpoint (v2)' endpoint",,
    "tokenUrl": "! Set Azure AD application's 'OAuth 2.0 token endpoint (v2)' endpoint",,
    "apiScope": "! Set Azure AD application's target API permission (e.g. 'api://....')",,
  },
  "AllowedHosts": "*"
}
```

More detail: [BEACHSIDE BLOG - Swagger UI で Azure AD の認証する (ASP.NET Core, Authorization Code Flow with PKCE)](https://blog.beachside.dev/entry/2021/01/25/123000)


### 🍖 [AspnetCore50WithAzureAdBearerSet](./src/AspnetCore50WithAzureAdBearerSet)

- ASP.NET Core 5.0 Web API (Azure AD Authorization)
- Swashbuckle
- Swagger UI Authorization (Set Bearer token)

Need to update appsettings.json as following:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "InformatioPn"
    }
  },
  "AzureAd": {
    "clientId": "! Set Azure AD application's client id",
    "metadataAddress": "! Set Azure AD application's 'OpenID Connect metadata document' endpoint",
  },
  "AllowedHosts": "*"
}
```

More detail: [BEACHSIDE BLOG - ASP.NET Core で Open API (Swagger) の設定の基礎 (.NET Core 3.1 と .NET 5)](https://blog.beachside.dev/entry/2021/01/22/123000)

