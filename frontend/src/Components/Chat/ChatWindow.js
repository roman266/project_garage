import { useState, useEffect, useRef } from "react";
import { Box, Typography, Avatar, Paper, IconButton, InputBase, CircularProgress } from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import axios from "axios";
<<<<<<< HEAD:frontend/src/Components/Chat/ChatWindow.js
import { API_URL } from "../../constants";
=======
import { API_URL } from "../constants";
import { createChatConnection, fetchChatMessages, fetchChatInfo } from "../services/chatService";
import EmojiPickerComponent from "./EmojiPickerComponent";
>>>>>>> b1c80e9c34457a4010dee6a86f34a94b4096ae45:frontend/src/Components/ChatWindow.js

export default function ChatWindow({ selectedChatId }) {
  const [messages, setMessages] = useState([]);
  const [connection, setConnection] = useState(null);
  const [newMessage, setNewMessage] = useState("");
  const [chatInfo, setChatInfo] = useState(null);
  const connectionRef = useRef(null);
  const [currentUserId, setCurrentUserId] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [hasMoreMessages, setHasMoreMessages] = useState(true);
  const messagesContainerRef = useRef(null);
  const messageLimit = 20;
  const messageIdsRef = useRef(new Set());
  const chatServiceRef = useRef(null);
  // Add state for file attachment
  const [attachedFile, setAttachedFile] = useState(null);
  const fileInputRef = useRef(null);

<<<<<<< HEAD:frontend/src/Components/Chat/ChatWindow.js
  useEffect(() => {
    if (!selectedChatId) return;

    const connect = new HubConnectionBuilder()
      .withUrl(`${API_URL}/chatHub`, { withCredentials: true })
      .build();

    connect.start()
      .then(() => {
        console.log("SignalR connected");
        connect.on("ReceiveMessage", (message) => {
          setMessages((prevMessages) => [...prevMessages, message]);
        });
        connectionRef.current = connect;

        axios.get(`${API_URL}/api/conversations/${selectedChatId}/messages`, { withCredentials: true })
          .then((response) => setMessages(response.data))
          .catch((error) => console.error("Error fetching messages", error));
=======
  useEffect(() => {
    axios.get(`${API_URL}/api/user/current`, { withCredentials: true })
      .then(response => {
        setCurrentUserId(response.data.id);
      })
      .catch(error => console.error("Error getting current user:", error));
  }, []);

  const cacheMessages = (chatId, messages) => {
    try {
      sessionStorage.setItem(`chat_messages_${chatId}`, JSON.stringify({
        messages: messages,
        timestamp: Date.now()
      }));
    } catch (error) {
      console.error("Error caching messages:", error);
    }
  };

  const scrollToBottom = () => {
    if (messagesContainerRef.current) {
      messagesContainerRef.current.scrollTop = messagesContainerRef.current.scrollHeight;
    }
  };

  useEffect(() => {
    if (messagesContainerRef.current && messages.length > 0) {
      const lastMessage = messages[messages.length - 1];
      if (lastMessage.isCurrentUser || lastMessage.isSystem) {
        setTimeout(scrollToBottom, 100);
      }
    }
  }, [messages]);

  const addMessageToState = (newMsg, chatId) => {
    if (newMsg.id) {
      messageIdsRef.current.add(newMsg.id);
    }
    
    setMessages(prevMessages => {
      const updatedMessages = [...prevMessages, newMsg];
      if (chatId) {
        cacheMessages(chatId, updatedMessages);
      }
      return updatedMessages;
    });
  };

  const createMessage = (text, senderId, isSystem = false, imageUrl = null) => {
    const msgId = Date.now().toString();
    
    if (isSystem) {
      return {
        id: msgId,
        text: text,
        isSystem: true,
        sendedAt: new Date().toISOString()
      };
    }
    
    return {
      id: msgId,
      senderId: senderId,
      text: text,
      imageUrl: imageUrl,
      isCurrentUser: senderId === currentUserId,
      sendedAt: new Date().toISOString()
    };
  };

  const loadOlderMessages = async () => {
    if (!selectedChatId || isLoading || !hasMoreMessages || messages.length === 0) return;
    
    setIsLoading(true);
    try {
      const scrollContainer = messagesContainerRef.current;
      const previousScrollHeight = scrollContainer.scrollHeight;
      
      const oldestMessage = messages[0];
      const oldestMessageId = oldestMessage.id;
      
      const olderMessages = await fetchChatMessages(selectedChatId, oldestMessageId, messageLimit);
      
      if (olderMessages.length === 0 || olderMessages.length < messageLimit) {
        setHasMoreMessages(false);
      }
      
      if (olderMessages.length > 0) {
        const uniqueOlderMessages = olderMessages.filter(msg => {
          if (!messageIdsRef.current.has(msg.id)) {
            messageIdsRef.current.add(msg.id);
            return true;
          }
          return false;
        });
        
        setMessages(prevMessages => [...uniqueOlderMessages, ...prevMessages]);
        
        setTimeout(() => {
          if (scrollContainer) {
            scrollContainer.scrollTop = scrollContainer.scrollHeight - previousScrollHeight;
          }
        }, 0);
      }
    } catch (error) {
      console.error("Error loading older messages:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleScroll = () => {
    if (!messagesContainerRef.current) return;
    
    const { scrollTop } = messagesContainerRef.current;
    
    if (scrollTop < 50 && !isLoading && hasMoreMessages) {
      loadOlderMessages();
    }
  };

  useEffect(() => {
    if (!selectedChatId) return;

    setMessages([]);
    setHasMoreMessages(true);
    messageIdsRef.current.clear();
    
    // Fetch chat info
    fetchChatInfo(selectedChatId)
      .then(info => {
        if (info) {
          setChatInfo(info);
        }
>>>>>>> b1c80e9c34457a4010dee6a86f34a94b4096ae45:frontend/src/Components/ChatWindow.js
      })
      .catch(err => console.error("Error fetching chat info", err));

    // Create chat connection with callbacks
    const chatService = createChatConnection(selectedChatId, {
      onReceiveMessage: (senderId, text, imageUrl) => {
        const newMsg = createMessage(text, senderId, false, imageUrl);
        addMessageToState(newMsg, selectedChatId);
        setTimeout(scrollToBottom, 100);
      },
      onReceiveSystemMessage: (message) => {
        const newMsg = createMessage(message, null, true);
        addMessageToState(newMsg, selectedChatId);
        setTimeout(scrollToBottom, 100);
      }
    });
    
    chatServiceRef.current = chatService;
    
    // Setup connection and load messages
    chatService.setupConnection()
      .then(conn => {
        setConnection(conn);
        connectionRef.current = conn;
        
        // Load initial messages
        return fetchChatMessages(selectedChatId);
      })
      .then(messageData => {
        const uniqueMessages = messageData.filter(msg => {
          if (!messageIdsRef.current.has(msg.id)) {
            messageIdsRef.current.add(msg.id);
            return true;
          }
          return false;
        });
        
        setMessages(uniqueMessages);
        
        if (uniqueMessages.length < messageLimit) {
          setHasMoreMessages(false);
        }
        
        setTimeout(scrollToBottom, 100);
      })
      .catch(error => console.error("Error setting up chat:", error));

    // Cleanup function
    return () => {
      if (chatServiceRef.current) {
        chatServiceRef.current.leaveChat(selectedChatId);
      }
    };
  }, [selectedChatId, currentUserId]);

  // File selection handler
  const handleFileSelect = (event) => {
    const file = event.target.files[0];
    if (file) {
      setAttachedFile(file);
    }
  };

  // Open file dialog
  const openFileDialog = () => {
    fileInputRef.current.click();
  };

  const handleSendMessage = async () => {
    if ((newMessage.trim() || attachedFile) && chatServiceRef.current) {
      const messageText = newMessage.trim();
      try {
<<<<<<< HEAD:frontend/src/Components/Chat/ChatWindow.js
        await connection.invoke("SendMessage", selectedChatId, newMessage);
        setMessages((prevMessages) => [...prevMessages, { text: newMessage, isMe: true }]);
=======
>>>>>>> b1c80e9c34457a4010dee6a86f34a94b4096ae45:frontend/src/Components/ChatWindow.js
        setNewMessage("");
        
        // Додаємо тимчасове повідомлення до UI
        const tempMsg = createMessage(
          messageText + (attachedFile ? (messageText ? ' ' : '') + `[Uploading: ${attachedFile.name}]` : ''), 
          currentUserId
        );
        addMessageToState(tempMsg, selectedChatId);
        
        let imageUrl = "None"; // Default value when no file is attached
        
        // Якщо є прикріплений файл, спочатку завантажуємо його
        if (attachedFile) {
          const formData = new FormData();
          formData.append('image', attachedFile);
          
          // Завантажуємо зображення через API
          const response = await axios.post(`${API_URL}/api/message/upload-image`, formData, {
            withCredentials: true,
            headers: {
              'Content-Type': 'multipart/form-data'
            }
          });
          
          imageUrl = response.data;
        }
        
        // Створюємо DTO для відправки через SignalR
        const messageDto = {
          ConversationId: selectedChatId,
          Text: messageText,
          ImageUrl: imageUrl
        };
        
        // Відправляємо повідомлення через SignalR
        await chatServiceRef.current.connection.invoke("SendMessage", messageDto);
        
        // Очищаємо стан прикріпленого файлу після відправки
        if (attachedFile) {
          setAttachedFile(null);
        }
      } catch (error) {
        console.error("Error sending message", error);
      }
    }
  };

  if (!selectedChatId) return (
    <Box display="flex" flex={1} alignItems="center" justifyContent="center" color="gray" backgroundColor="white">
      Select a chat
    </Box>
  );

  // Решта коду залишається без змін
  return (
    <Box display="flex" flexDirection="column" flex={1} component={Paper} elevation={2}>
      <Box display="flex" alignItems="center" padding={2} bgcolor="#e0e0e0">
        <Avatar 
          sx={{ bgcolor: "black", marginRight: 2 }} 
          src={chatInfo?.profilePictureUrl !== "None" ? chatInfo?.profilePictureUrl : null}
        />
        <Typography variant="h6">{chatInfo?.userName || `Chat ${selectedChatId.substring(0, 8)}...`}</Typography>
      </Box>
      <Box 
        flex={1} 
        padding={2} 
        bgcolor="white" 
        overflow="auto" 
        display="flex" 
        flexDirection="column"
        ref={messagesContainerRef}
        onScroll={handleScroll}
        sx={{ 
          height: "calc(100vh - 200px)", 
          maxHeight: "calc(100vh - 200px)" 
        }}
      >
        {isLoading && (
          <Box display="flex" justifyContent="center" my={1}>
            <CircularProgress size={24} />
          </Box>
        )}
        
        {messages
          .filter((msg, index, self) => 
            index === self.findIndex(m => m.id === msg.id)
          )
          .slice()
          .sort((a, b) => new Date(a.sendedAt) - new Date(b.sendedAt))
          .map((msg, index) => (
            <Box
              key={msg.id || index}
              sx={{
                maxWidth: "60%",
                padding: "10px 15px",
                borderRadius: "18px",
                bgcolor: msg.isSystem ? "#f5f5f5" : (msg.isCurrentUser ? "#e0e0e0" : "#1e88e5"),
                color: msg.isSystem ? "gray" : (msg.isCurrentUser ? "black" : "white"),
                alignSelf: msg.isSystem ? "center" : (msg.isCurrentUser ? "flex-end" : "flex-start"),
                marginBottom: 1,
                fontStyle: msg.isSystem ? "italic" : "normal",
              }}
            >
              {msg.text || msg.content}
              {msg.imageUrl && msg.imageUrl !== "None" && (
                <Box mt={1}>
                  <img 
                    src={msg.imageUrl} 
                    alt="Attached" 
                    style={{ 
                      maxWidth: '100%', 
                      borderRadius: '8px',
                      maxHeight: '200px'
                    }} 
                  />
                </Box>
              )}
            </Box>
          ))}
      </Box>
<<<<<<< HEAD:frontend/src/Components/Chat/ChatWindow.js
      <Box padding={2} bgcolor="#e0e0e0" display="flex" alignItems="center">
        <Box sx={{ display: "flex", alignItems: "center", backgroundColor: "#d6d6d6", borderRadius: 50, flex: 1, padding: "5px 10px" }}>
=======
      <Box 
        padding={2} 
        bgcolor="#e0e0e0" 
        display="flex" 
        alignItems="center" 
        borderRadius={1.7}
        sx={{
          position: "sticky",
          bottom: 0,
          zIndex: 1,
          borderTop: "1px solid #ccc"
        }}
      >
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            backgroundColor: "#d6d6d6",
            borderRadius: 50,
            flex: 1,
            padding: "5px 10px",
          }}
        >
          {attachedFile && (
            <Box 
              sx={{ 
                display: 'flex', 
                alignItems: 'center', 
                backgroundColor: '#c4c4c4',
                padding: '2px 8px',
                borderRadius: 10,
                marginRight: 1,
                fontSize: '0.8rem'
              }}
            >
              <Typography variant="caption" sx={{ marginRight: 1 }}>
                {attachedFile.name.length > 15 
                  ? attachedFile.name.substring(0, 12) + '...' 
                  : attachedFile.name}
              </Typography>
              <IconButton 
                size="small" 
                onClick={() => setAttachedFile(null)}
                sx={{ padding: '2px' }}
              >
                ✕
              </IconButton>
            </Box>
          )}
>>>>>>> b1c80e9c34457a4010dee6a86f34a94b4096ae45:frontend/src/Components/ChatWindow.js
          <InputBase
            sx={{ flex: 1, marginLeft: 1 }}
            placeholder="Type message here"
            value={newMessage}
            onChange={(e) => setNewMessage(e.target.value)}
<<<<<<< HEAD:frontend/src/Components/Chat/ChatWindow.js
            onKeyDown={(e) => e.key === "Enter" && handleSendMessage()}
          />
        </Box>
        <IconButton><InsertEmoticonIcon sx={{ color: "#1e497c" }} /></IconButton>
        <IconButton><AddIcon sx={{ color: "#1e497c" }} /></IconButton>
=======
            onKeyDown={(e) => {
              if (e.key === "Enter" && (newMessage.trim() || attachedFile)) {
                handleSendMessage();
              }
            }}
          />
        </Box>
        <IconButton 
          sx={{ marginLeft: 1 }}
          onClick={handleSendMessage}
          disabled={!newMessage.trim() && !attachedFile}
        >
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#1e497c" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <line x1="22" y1="2" x2="11" y2="13"></line>
            <polygon points="22 2 15 22 11 13 2 9 22 2"></polygon>
          </svg>
        </IconButton>
        
        <EmojiPickerComponent 
          onEmojiSelect={(emoji) => setNewMessage(prev => prev + emoji)} 
        />
        
        <IconButton 
          sx={{ marginLeft: 1 }}
          onClick={openFileDialog}
        >
          <AddIcon sx={{ color: "#1e497c" }} />
        </IconButton>
        <input
          type="file"
          ref={fileInputRef}
          style={{ display: 'none' }}
          onChange={handleFileSelect}
          accept="image/*"
        />
>>>>>>> b1c80e9c34457a4010dee6a86f34a94b4096ae45:frontend/src/Components/ChatWindow.js
      </Box>
    </Box>
  );
}
