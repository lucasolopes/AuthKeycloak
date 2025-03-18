# AuthKeycloak - API ASP.NET Core com Autenticação Keycloak

Este projeto demonstra como implementar autenticação e autorização em uma API ASP.NET Core usando o Keycloak como provedor de identidade. Inclui controle de acesso baseado em funções, políticas de autorização personalizadas e verificação de identidade do usuário.

## 📋 Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/products/docker-desktop/)
- [Docker Compose](https://docs.docker.com/compose/install/)

## 🚀 Começando

### Clone o Repositório

```bash
git clone https://seu-repositorio-url/AuthKeycloak.git
cd AuthKeycloak
```

### Executando o Projeto

O projeto usa Docker Compose para configurar todos os serviços necessários:

```bash
docker-compose up -d
```

Isso iniciará:
- Servidor Keycloak (acessível em http://localhost:8080)
- Banco de dados PostgreSQL (para o Keycloak)
- API ASP.NET Core (acessível em http://localhost:3000)

### Configuração Inicial do Keycloak

1. Acesse o console de administração do Keycloak em http://localhost:8080
2. Faça login com as credenciais padrão:
   - Usuário: `admin`
   - Senha: `admin`
3. Crie um novo realm chamado `dotnet-api`
4. Nas configurações do realm (Realm Settings):
   - Vá para a aba "Login"
   - Habilite "User Registration" para permitir que usuários se registrem
5. Crie um cliente:
   - ID do Cliente: `public-client`
   - Segredo do cliente: `u0Y7e3wFGtqXH0ZBHqvpQzj6BpjotYyi` (ou atualize em appsettings.json)
6. Configure o cliente:
   - Habilite o "Implicit Flow"
   - Em "Valid Redirect URIs", adicione: `http://host:port/*` (substitua host:port pelos valores reais)
   - Em "Web Origins", adicione: `http://host:port` (substitua host:port pelos valores reais)
7. Crie funções:
   - `api_user`
   - `api_admin`
   - `api_manager`
8. Crie usuários de teste e atribua funções

## 🛠️ Estrutura do Projeto

- **AuthKeycloak**: Projeto principal ASP.NET Core
  - **/Controllers**: Endpoints da API
  - **/Authorization**: Manipuladores e requisitos de autorização personalizados
  - **/Extensions**: Extensões de coleção de serviços
  - **/Models**: Objetos de transferência de dados

## 🔒 Endpoints da API

A API fornece vários endpoints seguros para demonstrar diferentes cenários de autorização:

### Secured Controller

- `GET /api/secured/public` - Endpoint público (não requer autenticação)
- `GET /api/secured/user` - Requer função `api_user`
- `GET /api/secured/admin` - Requer função `api_admin`
- `GET /api/secured/manager-or-admin` - Requer função `api_manager` ou `api_admin`
- `GET /api/secured/user-info` - Mostra as claims do usuário do token JWT (requer autenticação)

### User Controller

- `GET /api/user/{userId}` - Obtém o perfil do usuário (requer mesma identidade de usuário)
- `PUT /api/user/{userId}` - Atualiza o perfil do usuário (requer mesma identidade de usuário)

## 🔐 Políticas de Autorização

A aplicação implementa várias políticas de autorização:

- **ApiUser**: Requer a função `api_user`
- **ApiAdmin**: Requer a função `api_admin`
- **ApiManagerOrAdmin**: Requer a função `api_manager` ou `api_admin`
- **SomenteProprioUsuario**: Política personalizada que verifica se o usuário autenticado corresponde ao ID de usuário solicitado

## 🔍 Manipulador de Autorização Personalizado

O projeto inclui um manipulador de autorização personalizado (`MesmaIdentidadeHandler`) que impõe a verificação de identidade do usuário para ações que só devem ser realizadas pelo proprietário do recurso.

## 🧪 Testando a API

1. Acesse a interface Swagger em http://localhost:3000/swagger
2. Clique no botão "Authorize"
3. Use o fluxo de autenticação do Keycloak para fazer login
4. Teste os diferentes endpoints com várias funções de usuário

## ⚙️ Configuração

As principais configurações estão em `appsettings.json`:

```json
"Keycloak": {
  "realm": "dotnet-api",
  "AuthorizationUrl": "http://localhost:8080/realms/dotnet-api/protocol/openid-connect/auth",
  "ssl-required": "none",
  "resource": "public-client",
  "verify-token-audience": true,
  "tokenEndpoint": "http://keycloak:8080/realms/dotnet-api/protocol/openid-connect/token",
  "credentials": {
    "secret": "u0Y7e3wFGtqXH0ZBHqvpQzj6BpjotYyi"
  }
},
"Authentication": {
  "MetadataAddress": "http://keycloak:8080/realms/dotnet-api/.well-known/openid-configuration",
  "ValidIssuer": "http://localhost:8080/realms/dotnet-api",
  "Audience": "account"
}
```

## 📚 Dependências Principais

- **Keycloak.AuthServices.Authentication**: Fornece integração de autenticação com Keycloak
- **Keycloak.AuthServices.Authorization**: Fornece integração de autorização com Keycloak
- **Microsoft.AspNetCore.Authentication.JwtBearer**: Autenticação de token JWT
- **Microsoft.AspNetCore.Authorization**: Políticas de autorização
- **Swashbuckle.AspNetCore**: Documentação Swagger

## 🐳 Configuração Docker

A solução inclui suporte para Docker:
- **Dockerfile**: Compila a aplicação ASP.NET Core
- **docker-compose.yml**: Orquestra os serviços da API, Keycloak e PostgreSQL

## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para enviar um Pull Request.

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo LICENSE para detalhes.