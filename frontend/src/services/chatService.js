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
            isEdited: message.isEdited,
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
      
      if (callbacks.onMessageDeleted) {
        connection.on("MessageDeleted", function(messageId) {
          console.log(`ChatService received MessageDeleted for message ${messageId}`);
          callbacks.onMessageDeleted(messageId);
        });
      }

      if (callbacks.onMessageUpdated) {
        connection.on("MessageUpdated", function(messageId) {
          console.log(`ChatService received MessageUpdated for message ${messageId}`);
          callbacks.onMessageUpdated(messageId);
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
        
        // Отримуємо ID розмови з збереженого повідомлення або з DTO
        const conversationId = savedMessage.conversationId || messageDto.ConversationId;
        
        // Надсилаємо повідомлення через SignalR
        await connection.invoke("SendMessage", {
          id: savedMessage.id,
          conversationId: conversationId,
          senderId: savedMessage.senderId,
          text: savedMessage.text,
          imageUrl: savedMessage.imageUrl,
          sendedAt: savedMessage.sendedAt,
          isReaden: savedMessage.isReaden,
          isVisible: savedMessage.isVisible
        });
        
        // Отримуємо список учасників розмови
        const membersResponse = await fetch(`${API_URL}/api/conversations/${conversationId}/get-members`, {
          credentials: 'include'
        });
        
        if (!membersResponse.ok) {
          console.error("Failed to fetch conversation members");
        } else {
          const members = await membersResponse.json();
          
          // Отримуємо список ID учасників
          const memberIds = members.$values 
            ? members.$values.map(m => m.userId || m.id) 
            : Array.isArray(members) 
              ? members.map(m => m.userId || m.id)
              : [];
          
          // Викликаємо метод для сповіщення всіх учасників розмови
          if (memberIds.length > 0) {
            await connection.invoke("NotifyUsersAboutReceivedMessage", conversationId, memberIds);
          }
        }
        
        // Оновлюємо час останнього повідомлення в розмові
        try {
          await fetch(`${API_URL}/api/conversations/message-sended/${conversationId}`, {
            method: 'PATCH',
            credentials: 'include'
          });
        } catch (updateError) {
          console.error("Error updating last message time:", updateError);
        }

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

      } 
      catch (error) 
      {
        console.error("Error reading message", error);
        return false;
      }
    },
    // Add delete message function inside the returned object
    deleteMessage: async (messageId, conversationId) => {
      try {
        const response = await fetch(`${API_URL}/api/message/${messageId}/delete`, {
          method: 'DELETE',
          credentials: 'include'
        });

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        // Use the connection instance from this context
        await connection.invoke("DeleteMessage", messageId, conversationId);

        return true;
      }
      catch (error) {
        console.error("Error deleting message", error);
        return false;
      }
    },
    updateMessage: async (messageId, conversationId, newText) => {
      try{
        const response = await fetch(`${API_URL}/api/message/${messageId}/update`, 
        {
          method: 'PATCH',
          credentials: 'include',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(newText),
        }
        );

        if(!response.ok){
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        await connection.invoke("UpdateMessage", messageId, conversationId, newText);

        return true;
      }
      catch (error) {
        console.error("Error updating message", error);
        return false;
      }
    },
    leaveChat: async (chatId) => {
      try 
      {
        await connection.invoke("LeaveChat", chatId);
        await connection.stop();
        return true;
      } 
        catch (error) 
      {
        console.error("Error leaving chat", error);
        return false;
      }
    }
  };
};

// Remove this standalone function since we've moved it inside createChatConnection
// export const deleteChatMessages = async (messageId, conversationId) => { ... }
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