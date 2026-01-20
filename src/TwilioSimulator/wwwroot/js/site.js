let selectedPhoneNumber = null;
let pollingInterval = null;

document.addEventListener('DOMContentLoaded', function () {
    loadConversations();
    startPolling();

    const replyForm = document.getElementById('reply-form');
    if (replyForm) {
        replyForm.addEventListener('submit', handleReplySubmit);
    }
});

function startPolling() {
    pollingInterval = setInterval(() => {
        loadConversations();
        if (selectedPhoneNumber) {
            loadMessages(selectedPhoneNumber, false);
        }
    }, 3000);
}

async function loadConversations() {
    try {
        const response = await fetch('/api/sms/conversations');
        const conversations = await response.json();
        renderConversations(conversations);
    } catch (error) {
        console.error('Erro ao carregar conversas:', error);
    }
}

function renderConversations(conversations) {
    const container = document.getElementById('contacts-list');

    if (conversations.length === 0) {
        container.innerHTML = `
            <div class="p-4 text-gray-500 text-center text-sm">
                Nenhuma conversa ainda
            </div>
        `;
        return;
    }

    container.innerHTML = conversations.map(conv => `
        <div class="contact-item p-4 border-b border-gray-100 cursor-pointer hover:bg-gray-50 transition-colors ${selectedPhoneNumber === conv.phoneNumber ? 'bg-indigo-50 border-l-4 border-l-indigo-600' : ''}"
             onclick="selectConversation('${conv.phoneNumber}')">
            <div class="font-medium text-gray-800">${conv.phoneNumber}</div>
            <div class="text-sm text-gray-500 truncate">${conv.lastMessagePreview || 'Sem mensagens'}</div>
            <div class="text-xs text-gray-400 mt-1">${formatDate(conv.lastMessageAt)}</div>
        </div>
    `).join('');
}

async function selectConversation(phoneNumber) {
    selectedPhoneNumber = phoneNumber;
    loadConversations();
    await loadMessages(phoneNumber, true);

    document.getElementById('chat-header').innerHTML = `
        <h2 class="text-lg font-semibold text-gray-700">${phoneNumber}</h2>
    `;

    document.getElementById('reply-container').classList.remove('hidden');
}

async function loadMessages(phoneNumber, scrollToBottom = true) {
    try {
        const response = await fetch(`/api/sms/messages/${encodeURIComponent(phoneNumber)}`);
        const messages = await response.json();
        renderMessages(messages, scrollToBottom);
    } catch (error) {
        console.error('Erro ao carregar mensagens:', error);
    }
}

function renderMessages(messages, scrollToBottom) {
    const container = document.getElementById('messages-container');

    if (messages.length === 0) {
        container.innerHTML = `
            <div class="flex items-center justify-center h-full text-gray-400">
                <p>Nenhuma mensagem ainda</p>
            </div>
        `;
        return;
    }

    container.innerHTML = messages.map(msg => {
        const isInbound = msg.direction === 'Inbound';
        return `
            <div class="mb-4 flex ${isInbound ? 'justify-start' : 'justify-end'}">
                <div class="max-w-xs lg:max-w-md px-4 py-2 rounded-lg ${isInbound ? 'bg-white border border-gray-200' : 'bg-indigo-600 text-white'}">
                    <div class="text-sm">${linkify(escapeHtml(msg.messageBody))}</div>
                    <div class="text-xs mt-1 ${isInbound ? 'text-gray-400' : 'text-indigo-200'}">${formatDate(msg.createdAt)}</div>
                </div>
            </div>
        `;
    }).join('');

    if (scrollToBottom) {
        container.scrollTop = container.scrollHeight;
    }
}

async function handleReplySubmit(e) {
    e.preventDefault();

    const input = document.getElementById('reply-input');
    const messageBody = input.value.trim();

    if (!messageBody || !selectedPhoneNumber) {
        return;
    }

    try {
        const response = await fetch('/api/sms/reply', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                phoneNumber: selectedPhoneNumber,
                messageBody: messageBody
            })
        });

        if (response.ok) {
            input.value = '';
            await loadMessages(selectedPhoneNumber, true);
        } else {
            const error = await response.json();
            alert('Erro ao enviar resposta: ' + (error.error || 'Erro desconhecido'));
        }
    } catch (error) {
        console.error('Erro ao enviar resposta:', error);
        alert('Erro ao enviar resposta');
    }
}

function formatDate(dateString) {
    const date = new Date(dateString);
    const now = new Date();
    const diff = now - date;
    const oneDay = 24 * 60 * 60 * 1000;

    if (diff < oneDay && date.getDate() === now.getDate()) {
        return date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
    } else {
        return date.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' });
    }
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function linkify(text) {
    const urlRegex = /(https?:\/\/[^\s<>"']+)/gi;
    return text.replace(urlRegex, (url) => {
        return `<a href="${url}" target="_blank" rel="noopener noreferrer" class="underline hover:opacity-80">${url}</a>`;
    });
}
