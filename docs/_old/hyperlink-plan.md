# Plano: Converter Links em Hyperlinks nas Mensagens

## Objetivo
Detectar URLs nas mensagens SMS e convertê-las em hyperlinks clicáveis com `target="_blank"`.

## Arquivo a Modificar
- `src/TwilioSimulator/wwwroot/js/site.js`

## Implementação

### 1. Criar função `linkify(text)`

Criar uma nova função que:
1. Recebe o texto da mensagem
2. Detecta URLs usando regex
3. Substitui URLs por tags `<a>` com `target="_blank"`
4. Retorna o HTML resultante

**Regex para detectar URLs:**
```javascript
/(https?:\/\/[^\s<>"']+)/gi
```

Esta regex detecta:
- URLs começando com `http://` ou `https://`
- Continua até encontrar espaço, `<`, `>`, `"` ou `'`

### 2. Modificar `renderMessages()`

Na linha 93, onde temos:
```javascript
<div class="text-sm">${escapeHtml(msg.messageBody)}</div>
```

Alterar para:
```javascript
<div class="text-sm">${linkify(escapeHtml(msg.messageBody))}</div>
```

**Ordem importante:** Primeiro escapar HTML, depois aplicar linkify.

### 3. Estilização dos Links

Adicionar classes CSS inline para os links:
- `text-blue-500` ou `text-indigo-400` (para links em mensagens outbound/brancas)
- `underline` para indicar que é clicável
- `hover:text-blue-700` para feedback visual

Para mensagens inbound (fundo branco): `text-blue-600`
Para mensagens outbound (fundo indigo): `text-indigo-200 underline`

### 4. Segurança

- Adicionar `rel="noopener noreferrer"` nos links para segurança
- O `escapeHtml()` já é aplicado antes, prevenindo XSS

## Código da Função `linkify`

```javascript
function linkify(text) {
    const urlRegex = /(https?:\/\/[^\s<>"']+)/gi;
    return text.replace(urlRegex, (url) => {
        return `<a href="${url}" target="_blank" rel="noopener noreferrer" class="underline hover:opacity-80">${url}</a>`;
    });
}
```

## Testes

1. Mensagem sem link: "Olá, tudo bem?" → Deve permanecer igual
2. Mensagem com link: "Acesse https://exemplo.com para mais info" → Link clicável
3. Mensagem com múltiplos links: "Veja https://a.com e https://b.com" → Ambos clicáveis
4. Link com caracteres especiais: "https://exemplo.com/path?q=teste&id=1" → Deve funcionar
5. Mensagem com HTML malicioso: "<script>alert('xss')</script>" → Deve ser escapado

## Ordem de Execução

1. Adicionar função `linkify()` no arquivo `site.js`
2. Modificar a função `renderMessages()` para usar `linkify()`
3. Testar manualmente
