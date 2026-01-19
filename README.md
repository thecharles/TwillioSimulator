# Twilio Simulator

A web-based Twilio SMS simulator built with ASP.NET Core MVC and TailwindCSS. Designed for local development and testing of SMS integrations without needing a real Twilio account.

## Features

- Receive SMS messages via REST API
- View all conversations in a chat-like interface
- Reply to messages with automatic callback to your application
- In-memory database (no setup required)
- No authentication required

## Requirements

- .NET 8.0 SDK
- Node.js (for TailwindCSS)

## Getting Started

### 1. Install dependencies

```bash
cd src/TwilioSimulator
npm install
```

### 2. Build TailwindCSS

```bash
npm run css:build
```

### 3. Run the application

```bash
dotnet run
```

The application will be available at `http://localhost:5000`

## API Endpoints

### Receive SMS (Payload1)

```http
POST /api/sms/receive
Content-Type: application/json

{
    "from": "+5511999887766",
    "to": "+5511988776655",
    "messageBody": "Hello!",
    "callbackUrl": "http://localhost:5174/api/webhook/twillio/callback"
}
```

**Response:**
```json
{
    "messageId": "guid",
    "status": "received"
}
```

### List Conversations

```http
GET /api/sms/conversations
```

**Response:**
```json
[
    {
        "id": "guid",
        "phoneNumber": "+5511999887766",
        "lastMessageAt": "2024-01-01T12:00:00Z",
        "lastMessagePreview": "Hello!"
    }
]
```

### Get Messages by Phone Number

```http
GET /api/sms/messages/{phoneNumber}
```

**Response:**
```json
[
    {
        "id": "guid",
        "from": "+5511999887766",
        "to": "+5511988776655",
        "messageBody": "Hello!",
        "direction": "Inbound",
        "createdAt": "2024-01-01T12:00:00Z"
    }
]
```

### Send Reply

```http
POST /api/sms/reply
Content-Type: application/json

{
    "phoneNumber": "+5511999887766",
    "messageBody": "Reply message"
}
```

When a reply is sent, the simulator will POST to the `callbackUrl` with the following payload (Payload2):

```json
{
    "from": "+5511988776655",
    "to": "+5511999887766",
    "messageBody": "Reply message"
}
```

## Project Structure

```
src/TwilioSimulator/
├── Controllers/
│   ├── Api/
│   │   └── SmsApiController.cs
│   └── HomeController.cs
├── Data/
│   └── SimulatorDbContext.cs
├── DTOs/
│   ├── ConversationDto.cs
│   ├── MessageDto.cs
│   ├── SendReplyDto.cs
│   ├── SmsReceiveDto.cs
│   └── SmsSendDto.cs
├── Models/
│   ├── Conversation.cs
│   ├── MessageDirection.cs
│   └── SmsMessage.cs
├── Services/
│   ├── CallbackService.cs
│   ├── ICallbackService.cs
│   ├── ISmsService.cs
│   └── SmsService.cs
├── Views/
│   ├── Home/
│   │   └── Index.cshtml
│   └── Shared/
│       └── _Layout.cshtml
└── wwwroot/
    ├── css/
    └── js/
        └── site.js
```

## Development

### Watch CSS changes

```bash
npm run css:watch
```

### Run with hot reload

```bash
dotnet watch run
```

## Notes

- Data is stored in-memory and will be lost when the application restarts
- The UI auto-refreshes every 3 seconds to show new messages
- Phone numbers should be in international format (e.g., +5511999887766)
