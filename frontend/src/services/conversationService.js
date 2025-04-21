import { API_URL } from "../constants";
import axios from "axios";
import { HubConnectionBuilder, HttpTransportType } from "@microsoft/signalr";

export const fetchUnreadCount = async (conversationId) => {
    try {
        const response = await axios.get(`${API_URL}/api/message/${conversationId}/unreaded-count`, {
            withCredentials: true,
        });
        return response.data;
    }
    catch (error) {
        console.error(`Error fetching unread count for chat ${conversationId}:`, error);
        return 0;
    }
}

export const fetchChats = async (lastConversationId = null, limit = 15) => {
    try {
        const params = {
            limit,
            lastConversationId: lastConversationId
        };

        const response = await axios.get(`${API_URL}/api/conversations/my-conversations`, {
            params,
            withCredentials: true
        });

        let chats = [];
        if (response.data.$values) {
            chats = response.data.$values;
        } else if (response.data.conversationList) {
            chats = response.data.conversationList;
        } else if (Array.isArray(response.data)) {
            chats = response.data;
        }

        const hasMore = chats.length >= limit;
        const newLastConversationId = chats.length > 0 ? chats[chats.length - 1].conversationId : null;

        return {
            chats,
            hasMore,
            lastConversationId: newLastConversationId
        };
    } catch (error) {
        console.error("Error fetching chats", error);
        throw error;
    }
};

// Singleton connection for notifications
let notificationConnection = null;
// Track active chat subscriptions
const activeChats = new Set();
// Track callback handlers
let messageCallbacks = [];

// Setup and get the notification connection
export const getNotificationConnection = async () => {
    if (notificationConnection && notificationConnection.state === "Connected") {
        return notificationConnection;
    }

    // Create a new connection if one doesn't exist or is disconnected
    notificationConnection = new HubConnectionBuilder()
        .withUrl(`${API_URL}/chatHub`, {
            withCredentials: true,
            skipNegotiation: false,
            transport: HttpTransportType.WebSockets
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .build();

    // Set up connection event handlers
    notificationConnection.onclose((error) => {
        console.log('Notification connection closed', error);
    });

    notificationConnection.onreconnecting((error) => {
        console.log('Notification connection reconnecting...', error);
    });

    notificationConnection.onreconnected(async (connectionId) => {
        console.log('Notification connection reconnected', connectionId);

        // Rejoin all active chats after reconnection
        try {
            await notificationConnection.invoke("OnLoginConnection");

            // Rejoin all active chats
            for (const chatId of activeChats) {
                try {
                    await notificationConnection.invoke("JoinChat", chatId);
                    console.log(`Rejoined chat ${chatId} after reconnection`);
                } catch (error) {
                    console.error(`Failed to rejoin chat ${chatId} after reconnection:`, error);
                }
            }
        } catch (error) {
            console.error("Error during reconnection process:", error);
        }
    });

    try {
        await notificationConnection.start();
        await notificationConnection.invoke("OnLoginConnection");
        console.log("Notification connection started");
        return notificationConnection;
    } catch (error) {
        console.error("Error starting notification connection:", error);
        return null;
    }
};

// Setup chat notifications
export const setupChatNotifications = async (onNewMessage) => {
    try {
        const connection = await getNotificationConnection();

        if (!connection) {
            console.error("Failed to get notification connection");
            return false;
        }

        // Store the callback for later use
        if (onNewMessage && typeof onNewMessage === 'function') {
            // Avoid duplicate callbacks
            if (!messageCallbacks.includes(onNewMessage)) {
                messageCallbacks.push(onNewMessage);
            }
        }

        // Remove existing handler if any
        connection.off("ReceivedMessage");

        // Register for the ReceivedMessage event
        connection.on("ReceivedMessage", (message) => {
            // Extract conversation ID from the message
            const conversationId = message.conversationId;
            console.log("Received new message notification for conversation:", conversationId);

            // Call all registered callbacks
            messageCallbacks.forEach(callback => {
                try {
                    callback(conversationId, message);
                } catch (error) {
                    console.error("Error in message callback:", error);
                }
            });
        });

        return true;
    } catch (error) {
        console.error("Error setting up chat notifications:", error);
        return false;
    }
};

export const joinChat = async (chatId) => {
    try {
        const connection = await getNotificationConnection();

        if (!connection) {
            console.error("Failed to get notification connection");
            return false;
        }

        await connection.invoke("JoinChat", chatId);
        activeChats.add(chatId);
        console.log(`Joined chat: ${chatId}`);
        return true;
    } catch (error) {
        console.error(`Error joining chat ${chatId}:`, error);
        return false;
    }
};

// Leave a specific chat
export const leaveChat = async (chatId) => {
    try {
        const connection = await getNotificationConnection();

        if (!connection) {
            console.error("Failed to get notification connection");
            return false;
        }

        await connection.invoke("LeaveChat", chatId);
        activeChats.delete(chatId);
        console.log(`Left chat: ${chatId}`);
        return true;
    } catch (error) {
        console.error(`Error leaving chat ${chatId}:`, error);
        return false;
    }
};

// Function to find a chat by ID from the existing chats list
export const findChatById = (chatId, existingChats) => {
    if (!chatId || !existingChats || !Array.isArray(existingChats)) {
        return null;
    }

    return existingChats.find(chat => chat.conversationId === chatId) || null;
};

// Join multiple chats at once
export const joinMultipleChats = async (chatIds) => {
    if (!Array.isArray(chatIds) || chatIds.length === 0) {
        return false;
    }

    try {
        const connection = await getNotificationConnection();

        if (!connection) {
            console.error("Failed to get notification connection");
            return false;
        }

        for (const chatId of chatIds) {
            try {
                await connection.invoke("JoinChat", chatId);
                activeChats.add(chatId);
                console.log(`Joined chat: ${chatId}`);
            } catch (error) {
                console.error(`Error joining chat ${chatId}:`, error);
            }
        }

        return true;
    } catch (error) {
        console.error("Error joining multiple chats:", error);
        return false;
    }
};

// Clean up function to remove a specific callback
export const removeMessageCallback = (callbackToRemove) => {
    messageCallbacks = messageCallbacks.filter(callback => callback !== callbackToRemove);
};
