# Plano de Implementação - Simulador Twilio

## Visão Geral
Desenvolver um simulador do Twilio em ASP.NET Core MVC com banco de dados em memória, permitindo receber e responder mensagens SMS via interface web.

---

## Fase 1: Configuração do Projeto

### 1.1 Criar projeto ASP.NET Core MVC
- Criar solução e projeto MVC na pasta `./src`
- Configurar para .NET 8 (LTS)
- Remover autenticação (não exige login)

### 1.2 Configurar TailwindCSS
- Instalar TailwindCSS via npm
- Configurar `tailwind.config.js`
- Criar arquivo CSS base com diretivas do Tailwind
- Configurar build do CSS no pipeline

### 1.3 Configurar Banco de Dados em Memória
- Instalar `Microsoft.EntityFrameworkCore.InMemory`
- Criar `DbContext` para a aplicação
- Registrar no `Program.cs` com `UseInMemoryDatabase`

---

## Fase 2: Modelagem de Dados

### 2.1 Criar Entidades
```
src/
├── Models/
│   ├── SmsMessage.cs      # Entidade de mensagem SMS
│   └── Conversation.cs    # Agrupamento por número de telefone
```

#### SmsMessage
- `Id` (Guid)
- `From` (string) - número de origem
- `To` (string) - número de destino
- `MessageBody` (string) - conteúdo da mensagem
- `CallbackUrl` (string, nullable) - URL para callback
- `Direction` (enum: Inbound/Outbound) - direção da mensagem
- `CreatedAt` (DateTime) - timestamp
- `ConversationId` (Guid) - FK para conversa

#### Conversation
- `Id` (Guid)
- `PhoneNumber` (string) - número do contato
- `CallbackUrl` (string) - última URL de callback recebida
- `Messages` (List<SmsMessage>) - mensagens da conversa
- `LastMessageAt` (DateTime) - última atividade

### 2.2 Criar DbContext
- `SimulatorDbContext` com DbSets para as entidades
- Configurar relacionamentos

---

## Fase 3: Camada de Serviços

### 3.1 Criar Serviços
```
src/
├── Services/
│   ├── ISmsService.cs
│   ├── SmsService.cs
│   └── ICallbackService.cs
│   └── CallbackService.cs
```

#### ISmsService
- `ReceiveMessageAsync(SmsReceiveDto)` - receber SMS (Payload1)
- `GetConversationsAsync()` - listar todas as conversas
- `GetMessagesByPhoneAsync(phoneNumber)` - mensagens de um número
- `SendReplyAsync(phoneNumber, messageBody)` - enviar resposta

#### ICallbackService
- `SendCallbackAsync(url, payload)` - enviar Payload2 para callback

---

## Fase 4: API Endpoints

### 4.1 Criar Controllers
```
src/
├── Controllers/
│   ├── Api/
│   │   └── SmsApiController.cs   # API REST
│   └── HomeController.cs          # MVC Views
```

#### SmsApiController (API)
- `POST /api/sms/receive` - Receber SMS (Payload1)
  - Salva mensagem no banco
  - Armazena callbackUrl para respostas futuras

### 4.2 DTOs
```
src/
├── DTOs/
│   ├── SmsReceiveDto.cs    # Payload1
│   └── SmsSendDto.cs       # Payload2
```

---

## Fase 5: Interface Web (Frontend)

### 5.1 Layout Principal
```
src/
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml
│   └── Home/
│       └── Index.cshtml
```

#### Estrutura do Layout
```
+------------------+----------------------------------------+
|                  |                                        |
|   Lista de       |     Mensagens da Conversa              |
|   Contatos       |     Selecionada                        |
|                  |                                        |
|   (20% largura)  |     (80% largura)                      |
|                  |                                        |
|                  |----------------------------------------|
|                  |   [Campo de resposta] [Enviar]         |
+------------------+----------------------------------------+
```

### 5.2 Componentes da Interface

#### Painel Esquerdo (20%)
- Lista de números de telefone que enviaram SMS
- Indicador de última mensagem recebida
- Highlight do contato selecionado

#### Painel Direito (80%)
- Header com número do contato selecionado
- Lista de mensagens (estilo chat)
  - Mensagens recebidas (alinhadas à esquerda)
  - Mensagens enviadas (alinhadas à direita)
- Footer com campo de texto e botão enviar

### 5.3 JavaScript/AJAX
- Atualização dinâmica da lista de contatos
- Carregamento de mensagens ao selecionar contato
- Envio de resposta via AJAX
- Polling ou SignalR para novas mensagens (opcional)

---

## Fase 6: Fluxo de Resposta

### 6.1 Processo de Resposta
1. Usuário digita mensagem no campo de resposta
2. Clica em "Enviar"
3. Sistema:
   - Salva mensagem como Outbound no banco
   - Envia POST para `callbackUrl` com Payload2
   - Atualiza interface

### 6.2 Tratamento de Erros
- Log de erros de callback
- Feedback visual para o usuário

---

## Fase 7: Testes e Refinamentos

### 7.1 Testes Manuais
- Testar recebimento de SMS via Postman/curl
- Testar visualização na interface
- Testar envio de resposta

### 7.2 Refinamentos
- Ajustes de estilo TailwindCSS
- Melhorias de UX
- Tratamento de casos de borda

---

## Estrutura Final do Projeto
```
src/
├── TwilioSimulator.csproj
├── Program.cs
├── appsettings.json
├── package.json                 # TailwindCSS
├── tailwind.config.js
├── Controllers/
│   ├── Api/
│   │   └── SmsApiController.cs
│   └── HomeController.cs
├── Data/
│   └── SimulatorDbContext.cs
├── DTOs/
│   ├── SmsReceiveDto.cs
│   └── SmsSendDto.cs
├── Models/
│   ├── SmsMessage.cs
│   └── Conversation.cs
├── Services/
│   ├── ISmsService.cs
│   ├── SmsService.cs
│   ├── ICallbackService.cs
│   └── CallbackService.cs
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml
│   └── Home/
│       └── Index.cshtml
└── wwwroot/
    ├── css/
    │   ├── input.css
    │   └── output.css
    └── js/
        └── site.js
```

---

## Ordem de Implementação Sugerida

1. **Projeto Base** - Criar projeto e configurar dependências
2. **Banco de Dados** - Modelos, DbContext, configuração em memória
3. **Serviços** - Lógica de negócio
4. **API** - Endpoint de recebimento de SMS
5. **Interface** - Layout e componentes visuais
6. **Integração** - Conectar frontend com backend
7. **Callback** - Implementar envio de resposta
8. **Polimento** - Ajustes finais e testes

---

## Considerações Técnicas

- **Sem persistência**: Dados são perdidos ao reiniciar a aplicação (conforme requisito)
- **Sem autenticação**: Acesso direto à aplicação
- **HttpClient**: Usar `IHttpClientFactory` para chamadas de callback
- **Responsividade**: TailwindCSS facilita design responsivo, mas foco no desktop
