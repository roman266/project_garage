import { HubConnectionBuilder, HttpTransportType } from "@microsoft/signalr";
import { API_URL, WEBSOCKET_URL } from "../constants";

export const createChatConnection = (chatId, callbacks) => {
  const hubUrl = `${API_URL}/chatHub`;
  
  const connection = new HubConnectionBuilder()
    .withUrl(hubUrl, { 
      withCredentials: true,
      skipNegotiation: false,
      transport: HttpTransportType.WebSockets
    })
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .build();

  connection.onclose((error) => {
    if (callbacks.onConnectionClosed) {
      callbacks.onConnectionClosed(error);
    }
  });

  connection.onreconnecting((error) => {
    if (callbacks.onReconnecting) {
      callbacks.onReconnecting(error);
    }
  });

  connection.onreconnected((connectionId) => {
    if (callbacks.onReconnected) {
      callbacks.onReconnected(connectionId);
    }
    connection.invoke("JoinChat", chatId).catch(err => {
      console.error("Error rejoining chat after reconnection", err);
    });
  });

  const setupConnection = async () => {
    try {
      await connection.start();
      
      await connection.invoke("JoinChat", chatId);
      
      if (callbacks.onReceiveMessage) {
        connection.on("ReceiveMessage", function(message) {
          const messageDate = message.sendedAt ? new Date(message.sendedAt) : new Date();
          
          const messageObj = {
            senderId: message.senderId,
            text: message.text,
            imageUrl: message.imageUrl,
            messageId: message.id,
            isReaden: message.isReaden,
            conversationId: message.conversationId || chatId,
            id: message.id,
            sendedAt: message.sendedAt,
            formattedLocalTime: messageDate.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})
          };
          
          callbacks.onReceiveMessage(messageObj);
          
          if (messageObj.messageId && messageObj.conversationId) {
            try {
              fetch(`${API_URL}/api/message/${messageObj.conversationId}/${messageObj.messageId}/read`, {
                method: 'PATCH',
                credentials: 'include'
              })
              .then(response => {
                if (response.ok) {
                  return response.json();
                } else {
                  return false;
                }
              })
              .then(isReadenResult => {
                if (isReadenResult) {
                  connection.invoke("ReadMessage", messageObj.conversationId, messageObj.messageId)
                    .catch(err => console.error("Error invoking ReadMessage:", err));
                }
              })
              .catch(error => {
                console.error("Error automatically marking message as read:", error);
              });
            } catch (error) {
              console.error("Error in read message process:", error);
            }
          }
        });
      }
      
      if (callbacks.onReceiveSystemMessage) {
        connection.on("ReceiveSystemMessage", callbacks.onReceiveSystemMessage);
      }
      
      if (callbacks.onMessageReaden) {
        connection.on("MessageReaden", function(messageId) {
          callbacks.onMessageReaden(messageId);
        });
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
        const response = await fetch(`${API_URL}/api/message/send`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(messageDto),
          credentials: 'include'
        });
        
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const savedMessage = await response.json();
        
        const messageDate = savedMessage.sendedAt ? new Date(savedMessage.sendedAt) : new Date();
        savedMessage.formattedLocalTime = messageDate.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'});
        
        await connection.invoke("SendMessage", {
          id: savedMessage.id,
          conversationId: savedMessage.conversationId || messageDto.ConversationId,
          senderId: savedMessage.senderId,
          text: savedMessage.text,
          imageUrl: savedMessage.imageUrl,
          sendedAt: savedMessage.sendedAt,
          isReaden: savedMessage.isReaden,
          isVisible: savedMessage.isVisible
        });
        
        return savedMessage;
      } catch (error) {
        console.error("Error sending message", error);
        return false;
      }
    },
    readMessage: async (conversationId, messageId) => {
      try {
        const response = await fetch(`${API_URL}/api/message/${conversationId}/${messageId}/read`, {
          method: 'PATCH',
          credentials: 'include'
        });
        
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const isReaden = await response.json();
        
        if (isReaden) {
          await connection.invoke("ReadMessage", conversationId, messageId);
        }
        
        return isReaden;
      } catch (error) {
        console.error("Error reading message", error);
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
    const messages = data.$values || data || [];

    return messages.map(message => {
      const messageDate = message.sendedAt ? new Date(message.sendedAt) : new Date();
      
      return {
        ...message,
        sendedAt: messageDate.toISOString(),
        formattedLocalTime: messageDate.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})
      };
    });
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