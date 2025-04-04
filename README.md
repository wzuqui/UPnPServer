# UPnP Server Configuration

Este arquivo contém a configuração para o servidor UPnP e o serviço Cloudflare. A configuração inclui URLs, intervalos de atualização, portas e registros DNS.

## Como Ativar o UPnP Server

Para ativar o UPnP Server, siga os passos abaixo:

1. Abra o arquivo `appsettings.json`.
2. Localize a seção `UPnPServer`.
3. Certifique-se de que o campo `Enabled` esteja definido como `true`.
4. Salve o arquivo e reinicie o servidor.

## Portas Privadas e Públicas

- **Private Port**: A porta privada é a porta na qual o servidor UPnP está ouvindo localmente. No exemplo acima, a porta privada é `5000`.
- **Public Port**: A porta pública é a porta na qual o servidor UPnP está acessível na rede externa. No exemplo acima, a porta pública é `8080`.

## Configuração do Cloudflare

### Obtendo o Bearer Token

1. Acesse o [Cloudflare Dashboard](https://dash.cloudflare.com/).
2. Vá para a seção "My Profile".
3. Clique em "API Tokens".
4. Clique em "Create Token".
5. Selecione o modelo "Edit zone DNS" e clique em "Continue to summary".
6. Dê um nome ao token e clique em "Create Token".
7. Copie o token gerado e cole-o no campo `BearerToken` no arquivo `appsettings.json`.

### Obtendo o Zone ID

1. Acesse o [Cloudflare Dashboard](https://dash.cloudflare.com/).
2. Selecione o domínio para o qual você deseja configurar o DNS.
3. Clique na aba "Overview".
4. O Zone ID está disponível na direita da página.

### Populando os Registros

1. Acesse o [Cloudflare Dashboard](https://dash.cloudflare.com/).
2. Selecione o domínio para o qual você deseja configurar o DNS.
3. Clique na aba "DNS".
4. Clique em "Add record".
5. Preencha os campos `Name`, `Type`, `TTL` e `Proxied` conforme necessário.
6. Clique em "Save".

> [!IMPORTANT]
> Os records na section "Cloudflare" do `appsettings.json` não são criados nem removidos somente atualizado com ip público.
