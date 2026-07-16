# Sobre a funcionalidade
Desenvolver um simulador do Twillio.

# Onde salvar os códigos
Na pasta `./src`.

# Payload1 - Payload de recebimento de mensagem SMS
{
	"from": "celular de origem no formato +5511997886688",
	"to": "celular de destino",
	"messageBody": "Mensagem",
	"callbackUrl": "url completa para recebimento da resposta da SMS, ex: http://localhost:5174/api/webhook/twillio/callback"
}

# Payload2 - Payload de envio de mensagem
{
	"from": "celular de origem no formato +5511997886688",
	"to": "celular de destino",
	"messageBody": "Mensagem"
}

# Requisitos funcionais
- Não deve exigir login.
- O banco de dados deve ficar na memória.
- O usuário deverá poder visualizar qualquer mensagem recebida.
- O usuário poderá responder qualquer mensagem recebida.
- Ao responder o sistema deverá enviar uma mensagem para o endereço da callback enviado no payload de envio.

# Características do sistema
- Deve ser uma aplicação web desenvolvida em ASP.NET CORE MVC.
- Deve utilizar TailwindCSS para estilização.
- Banco de ficar na memória.
- Sistema não vai acessado por muita gente, só durante o desenvolvimento.

# Usabilidade do usuário
- O layout deve ser dividido verticalmente em 2 parcelas, a primeira com 20% de tamanho e a segunda com o resto.
- Primeira parte será a lista de todos os números que recebeu SMS
- Segunda parte será todas as SMS recebida do número selecionado na parte primeira (lado esquerdo).
- No lado direito, no rodapé, deve ser possível responder a SMS. A resposta será enviada o payload2 para a url de callback (callbackUrl do Payload1).