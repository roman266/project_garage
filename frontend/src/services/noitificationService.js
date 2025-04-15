import { API_URL } from "../constants";
import { notifyUserAboutRecievedMessage } from "../utils/notificationUtils";
import * as signalR from "@microsoft/signalr";
// Видаляємо імпорт useUnreadMessages, оскільки ми не можемо використовувати його тут напряму
// import { useUnreadMessages } from "../context/UnreadMessagesContext";


export const setupSignalRConnection = (connectionRef) => {
    if (connectionRef.current) return connectionRef.current;

    const newConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${API_URL}/chatHub`, {
            withCredentials: true,
            skipNegotiation: false,
            transport: signalR.HttpTransportType.WebSockets
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .build();

    setupNotificationHandlers(newConnection);
    connectionRef.current = newConnection;
    return newConnection;
};

// Змінюємо функцію, щоб вона приймала incrementUnreadCount як параметр
export const setupNotificationHandlers = (connection, incrementUnreadCount) => {
    if (!connection) return;

    connection.off("ReceivedMessage");

    connection.on("ReceivedMessage", (message, senderInfo) => {
        let conversationId;
        let sender = null;

        // Парсинг та перевірка conversationId
        if (typeof message === 'object' && message.arguments && Array.isArray(message.arguments)) {
            conversationId = message.arguments[0];
        } else if (typeof message === 'string') {
            try {
                const parsedMessage = JSON.parse(message);
                if (parsedMessage.arguments && Array.isArray(parsedMessage.arguments)) {
                    conversationId = parsedMessage.arguments[0];
                }
            } catch (e) {
                conversationId = message;
            }
        } else {
            conversationId = message;
        }

        // Обробка senderInfo як об'єкта
        if (senderInfo) {
            if (typeof senderInfo === 'string') {
                try {
                    sender = JSON.parse(senderInfo);
                } catch (e) {
                    console.error("Error parsing senderInfo string:", e);
                    sender = { name: senderInfo };
                }
            } else if (typeof senderInfo === 'object') {
                sender = senderInfo;
            }
        }

        // Збільшуємо лічильник непрочитаних повідомлень
        if (incrementUnreadCount) {
            incrementUnreadCount();
        }

        notifyUserAboutRecievedMessage(conversationId, sender);
    });
};

export const startSignalRConnection = async (connectionRef) => {
    const conn = connectionRef.current;
    if (!conn) return false;

    try {
        if (conn.state === signalR.HubConnectionState.Disconnected) {
            await conn.start();
            await conn.invoke("OnLoginConnection");
            return true;
        }
        return true;
    } catch (error) {
        console.error("SignalR Connection Error:", error);
        return false;
    }
};

export const stopSignalRConnection = async (connectionRef) => {
    const conn = connectionRef.current;
    if (!conn || conn.state !== signalR.HubConnectionState.Connected) {
        return true;
    }

    try {
        await conn.invoke("LogOut");
        await conn.stop();
        console.log("SignalR Disconnected");
        return true;
    } catch (error) {
        console.error("SignalR Disconnection Error:", error);
        return false;
    }
};