# Integration Guide — Twilio Simulator

This tool is a local development simulator for **SMS (Twilio)**. It lets your
application send SMS traffic to a local endpoint and inspect it in a browser
UI instead of hitting a real Twilio account.

- **Base URL (default):** `http://localhost:5030`
  (also commonly run on `http://localhost:5000` — check your launch profile).
- **Auth:** none. Do not expose this tool publicly.
- **Content type:** `application/json` for all `POST` bodies.
- **Storage:** in-memory. **All data is cleared when the app restarts.**
- **UI:** open the base URL in a browser. Conversations on the left, messages
  on the right; the UI polls every 3 seconds and a chime plays when a new SMS
  arrives.

> Email simulation lives in the separate `EmailSimulator` project
> (default `http://localhost:5029`).

---

## Feature: Twilio (SMS)

Simulates two-way SMS. Your app **receives** a message by posting it to the
simulator; when an operator replies in the UI, the simulator **calls back** to
your app's webhook URL.

### 1. Receive an SMS (your app → simulator)

Delivers an inbound message into a conversation (grouped by the `from` phone
number). Include the `callbackUrl` your app exposes so replies can be posted
back to you.

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

| Field | Type | Required | Notes |
|---|---|---|---|
| `from` | string | yes | Sender phone (international format, e.g. `+55...`). Conversation key. |
| `to` | string | no | Destination/system number. |
| `messageBody` | string | yes | Message text. |
| `callbackUrl` | string | no | Where the simulator POSTs operator replies. Stored per conversation and overwritten on each receive. |

**Response `200 OK`**
```json
{ "messageId": "guid", "status": "received" }
```
**`400 Bad Request`** — `from` or `messageBody` missing.

### 2. List conversations

```http
GET /api/sms/conversations
```
```json
[
    {
        "id": "guid",
        "phoneNumber": "+5511999887766",
        "lastMessageAt": "2026-07-01T12:00:00Z",
        "lastMessagePreview": "Hello!"
    }
]
```

### 3. Get messages for a phone number

```http
GET /api/sms/messages/{phoneNumber}
```
`{phoneNumber}` must be URL-encoded (e.g. `%2B5511999887766` for `+55...`).
```json
[
    {
        "id": "guid",
        "from": "+5511999887766",
        "to": "+5511988776655",
        "messageBody": "Hello!",
        "direction": "Inbound",
        "createdAt": "2026-07-01T12:00:00Z"
    }
]
```
`direction` is `Inbound` (received by simulator) or `Outbound` (a reply).

### 4. Send a reply (usually triggered from the UI)

```http
POST /api/sms/reply
Content-Type: application/json

{
    "phoneNumber": "+5511999887766",
    "messageBody": "Reply message"
}
```
**Response `200 OK`** `{ "status": "sent" }` ·
**`400`** if fields missing · **`404`** if the conversation doesn't exist.

### 5. Reply callback (simulator → your app)

When a reply is sent, the simulator issues a `POST` to the conversation's
stored `callbackUrl` with this body — **your app must expose this endpoint**:

```json
{
    "from": "+5511988776655",
    "to": "+5511999887766",
    "messageBody": "Reply message"
}
```
`from`/`to` are swapped relative to the original inbound message (the reply
comes from your system number back to the customer).

### End-to-end flow (SMS)

```
Your App ──POST /api/sms/receive──▶ Simulator ──shows in UI
Operator types reply in UI ─────▶ Simulator ──POST callbackUrl──▶ Your App
```

---

## Quick reference

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/sms/receive` | Deliver an inbound SMS |
| GET | `/api/sms/conversations` | List conversations |
| GET | `/api/sms/messages/{phoneNumber}` | Messages in a conversation |
| POST | `/api/sms/reply` | Send an operator reply (fires callback) |
| — | *your* `callbackUrl` | Simulator posts replies back to you |
