import { HubConnectionBuilder, HttpTransportType } from "@microsoft/signalr";
import { API_URL, WEBSOCKET_URL } from "../constants";

export const createChatConnection = (chatId, callbacks) => {
  // Use the HTTPS URL for SignalR connection
  // SignalR will handle the upgrade to WebSocket internally
  const hubUrl = `${API_URL}/chatHub`;
  
  const connection = new HubConnectionBuilder()
    .withUrl(hubUrl, { 
      withCredentials: true,
      skipNegotiation: false,
      transport: HttpTransportType.WebSockets
    })
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .build();

  // Add connection event handlers
  connection.onclose((error) => {
    console.log("SignalR connection closed", error);
    if (callbacks.onConnectionClosed) {
      callbacks.onConnectionClosed(error);
    }
  });

  connection.onreconnecting((error) => {
    console.log("SignalR reconnecting", error);
    if (callbacks.onReconnecting) {
      callbacks.onReconnecting(error);
    }
  });

  connection.onreconnected((connectionId) => {
    console.log("SignalR reconnected", connectionId);
    if (callbacks.onReconnected) {
      callbacks.onReconnected(connectionId);
    }
    // Rejoin the chat after reconnection
    connection.invoke("JoinChat", chatId).catch(err => {
      console.error("Error rejoining chat after reconnection", err);
    });
  });

  const setupConnection = async () => {
    try {
      await connection.start();
      console.log("SignalR connected");
      
      await connection.invoke("JoinChat", chatId);
      console.log(`Joined chat group: ${chatId}`);
      
      // Set up message handlers
      if (callbacks.onReceiveMessage) {
        // Update handler for receiving messages with file
        connection.on("ReceiveMessage", (senderId, text, fileUrl) => {
          callbacks.onReceiveMessage(senderId, text, fileUrl);
        });
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
    sendMessage: async (messageDto) => {
      try {
        // Змінюємо метод, щоб він приймав повний об'єкт messageDto замість окремих параметрів
        await connection.invoke("SendMessage", messageDto);
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