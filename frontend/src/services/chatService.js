import { HubConnectionBuilder } from "@microsoft/signalr";
import { API_URL, WEBSOKET_URL } from "../constants";

export const createChatConnection = (chatId, callbacks) => {
  const hubUrl = WEBSOKET_URL || `${API_URL}/chatHub`;
  
  const connection = new HubConnectionBuilder()
    .withUrl(hubUrl, { withCredentials: true })
    .build();

  const setupConnection = async () => {
    try {
      await connection.start();
      console.log("SignalR connected");
      
      await connection.invoke("JoinChat", chatId);
      console.log(`Joined chat group: ${chatId}`);
      
      // Set up message handlers
      if (callbacks.onReceiveMessage) {
        connection.on("ReceiveMessage", callbacks.onReceiveMessage);
      }
      
      if (callbacks.onReceiveSystemMessage) {
        connection.on("ReceiveSystemMessage", callbacks.onReceiveSystemMessage);
      }
      
      return connection;
    } catch (error) {
      console.error("SignalR connection error", error);
      throw error;
    }
  };

  return {
    connection,
    setupConnection,
    sendMessage: async (chatId, message) => {
      try {
        await connection.invoke("SendMessage", chatId, message);
        return true;
      } catch (error) {
        console.error("Error sending message", error);
        return false;
      }
    },
    leaveChat: async (chatId) => {
      try {
        await connection.invoke("LeaveChat", chatId);
        await connection.stop();
        return true;
      } catch (error) {
        console.error("Error leaving chat", error);
        return false;
      }
    }
  };
};

export const fetchChatMessages = async (chatId, lastMessageId = null, limit = 20) => {
  try {
    let url = `${API_URL}/api/message/${chatId}`;
    
    if (lastMessageId) {
      url += `?lastMessageId=${lastMessageId}&messageLimit=${limit}`;
    }
    
    const response = await fetch(url, {
      credentials: 'include'
    });
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    const data = await response.json();
    return data.$values || data || [];
  } catch (error) {
    console.error("Error fetching messages:", error);
    throw error;
  }
};

export const fetchChatInfo = async (chatId) => {
  try {
    const response = await fetch(`${API_URL}/api/conversations/my-conversations`, {
      credentials: 'include'
    });
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    const data = await response.json();
    const conversations = data.$values || data || [];
    return conversations.find(c => c.conversationId === chatId);
  } catch (error) {
    console.error("Error fetching chat info:", error);
    throw error;
  }
};