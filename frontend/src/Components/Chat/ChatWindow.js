import { useState, useEffect, useRef } from "react";
import { Box, Typography, Avatar, Paper, IconButton, InputBase, CircularProgress, Dialog, DialogTitle, DialogContent, DialogActions, Button, TextField } from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import CheckIcon from "@mui/icons-material/Check";
import DoneAllIcon from "@mui/icons-material/DoneAll";
import axios from "axios";
import { API_URL } from "../../constants";
import { createChatConnection, fetchChatMessages, fetchChatInfo } from "../../services/chatService";
import EmojiPickerComponent from "./EmojiPickerComponent";

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
  const [attachedFile, setAttachedFile] = useState(null);
  const fileInputRef = useRef(null);
  const [selectedMessage, setSelectedMessage] = useState(null);
  const [isMessageDialogOpen, setIsMessageDialogOpen] = useState(false);
  const [editedMessageText, setEditedMessageText] = useState("");

  const formatMessageTime = (dateString) => {
    try {
      const date = new Date(dateString);
      return date.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'});
    } catch (error) {
      console.error("Error formatting date:", error);
      return "??:??";
    }
  };

  useEffect(() => {
    axios.get(`${API_URL}/api/profile/me`, { withCredentials: true })
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

  const markMessagesAsRead = async (messagesToRead) => {
    if (!chatServiceRef.current || !selectedChatId || !currentUserId) return;
    
    const unreadMessages = messagesToRead.filter(msg => 
      !msg.isCurrentUser && 
      !msg.isSystem && 
      !msg.isReaden
    );
    
    for (const message of unreadMessages) {
      try {
        const isReaden = await chatServiceRef.current.readMessage(selectedChatId, message.id);
        
        if (isReaden) {
          setMessages(prevMessages => 
            prevMessages.map(msg => 
              msg.id === message.id ? { ...msg, isReaden: true } : msg
            )
          );
        }
      } catch (error) {
        console.error("Error marking message as read:", error);
      }
    }
  };

  useEffect(() => {
    if (messages.length > 0 && selectedChatId && currentUserId) {
      setTimeout(() => {
        markMessagesAsRead(messages);
      }, 1000);
    }
  }, [messages, selectedChatId, currentUserId]);

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
    const now = new Date();
    
    if (isSystem) {
      return {
        id: msgId,
        text: text,
        isSystem: true,
        sendedAt: now.toISOString(),
        formattedLocalTime: now.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})
      };
    }
    
    return {
      id: msgId,
      senderId: senderId,
      text: text,
      imageUrl: imageUrl,
      isCurrentUser: senderId === currentUserId,
      sendedAt: now.toISOString(),
      formattedLocalTime: now.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})
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
          
          const messagesToRead = uniqueOlderMessages.filter(msg => 
            !msg.isCurrentUser && !msg.isSystem && !msg.isReaden
          );
          
          for (const message of messagesToRead) {
            chatServiceRef.current.readMessage(selectedChatId, message.id)
              .catch(error => {
                console.error(`Error marking message ${message.id} as read:`, error);
              });
          }
        }, 500);
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
    
    fetchChatInfo(selectedChatId)
      .then(info => {
        if (info) {
          setChatInfo(info);
        }
      })
      .catch(err => console.error("Error fetching chat info", err));

    
    
    const chatService = createChatConnection(selectedChatId, {
      onReceiveMessage: (message) => {
        const newMsg = {
          id: message.id,
          senderId: message.senderId,
          text: message.text,
          imageUrl: message.imageUrl !== "None" ? message.imageUrl : null,
          isCurrentUser: message.senderId === currentUserId,
          isReaden: message.isReaden || false,
          isVisible: message.isVisible,
          sendedAt: message.sendedAt || new Date().toISOString(),
          conversationId: message.conversationId || selectedChatId,
          formattedLocalTime: message.formattedLocalTime
        };
        
        addMessageToState(newMsg, selectedChatId);
        setTimeout(scrollToBottom, 100);
      },
      onReceiveSystemMessage: (message) => {
        const newMsg = createMessage(message, null, true);
        addMessageToState(newMsg, selectedChatId);
        setTimeout(scrollToBottom, 100);
      },
      onMessageReaden: (messageId) => {
        setMessages(prevMessages => {
          const updatedMessages = prevMessages.map(msg => 
            msg.id === messageId ? { ...msg, isReaden: true } : msg
          );
          return updatedMessages;
        });
      },
      onMessageDeleted: (messageId) => {
        setMessages(prevMessages =>
          prevMessages.map(msg => 
            msg.id === messageId 
              ? { ...msg, isDeleted: true, text: "This message was deleted" } 
              : msg
          )
        );
      },
      onMessageUpdated: (messageId, newText) => {
        setMessages(prevMessages =>
          prevMessages.map(msg => 
            msg.id === messageId 
              ? { ...msg, text: newText } 
              : msg
          )
        );
      }
    });
    
    chatServiceRef.current = chatService;
    
    chatService.setupConnection()
      .then(conn => {
        setConnection(conn);
        connectionRef.current = conn;
        
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
        
        setTimeout(() => {
          scrollToBottom();
          
          const messagesToRead = uniqueMessages.filter(msg => 
            !msg.isCurrentUser && !msg.isSystem && !msg.isReaden
          );
          
          for (const message of messagesToRead) {
            chatServiceRef.current.readMessage(selectedChatId, message.id)
              .catch(error => {
                console.error(`Error marking message ${message.id} as read:`, error);
              });
          }
        }, 500);
      })
      .catch(error => console.error("Error setting up chat:", error));

    return () => {
      if (chatServiceRef.current) {
        chatServiceRef.current.leaveChat(selectedChatId);
      }
    };
  }, [selectedChatId, currentUserId]);

  const handleFileSelect = (event) => {
    const file = event.target.files[0];
    if (file) {
      setAttachedFile(file);
    }
  };

  const openFileDialog = () => {
    fileInputRef.current.click();
  };

  const handleSendMessage = async () => {
    if ((newMessage.trim() || attachedFile) && chatServiceRef.current) {
      const messageText = newMessage.trim();
      try {
        setNewMessage("");
        
        const now = new Date();
        const tempMsg = {
          ...createMessage(
            messageText + (attachedFile ? (messageText ? ' ' : '') + `[Uploading: ${attachedFile.name}]` : ''), 
            currentUserId
          ),
          formattedLocalTime: now.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})
        };
        
        addMessageToState(tempMsg, selectedChatId);
        
        let imageUrl = "None";
        
        if (attachedFile) {
          const formData = new FormData();
          formData.append('image', attachedFile);
          
          const response = await axios.post(`${API_URL}/api/message/upload-image`, formData, {
            withCredentials: true,
            headers: {
              'Content-Type': 'multipart/form-data'
            }
          });
          
          imageUrl = response.data;
        }
        
        const messageDto = {
          ConversationId: selectedChatId,
          Text: messageText,
          ImageUrl: imageUrl
        };
        
        const savedMessage = await chatServiceRef.current.sendMessage(messageDto);
        
        if (savedMessage && savedMessage.id) {
          setMessages(prevMessages => 
            prevMessages.map(msg => 
              msg.id === tempMsg.id 
                ? { 
                    ...msg, 
                    id: savedMessage.id,
                    isReaden: savedMessage.isReaden || false,
                    sendedAt: savedMessage.sendedAt,
                    formattedLocalTime: savedMessage.formattedLocalTime
                  } 
                : msg
            )
          );
        }
        
        await axios.patch(`${API_URL}/api/conversations/message-sended/${selectedChatId}`, {}, {
          withCredentials: true
        });
        
        if (attachedFile) {
          setAttachedFile(null);
        }
      } catch (error) {
        console.error("Error sending message", error);
      }
    }
  };

  const handleMessageClick = (message) => {
    // Only allow actions on user's own messages that aren't deleted
    if (message.isCurrentUser && !message.isDeleted && !message.isSystem) {
      setSelectedMessage(message);
      setEditedMessageText(message.text || "");
      setIsMessageDialogOpen(true);
    }
  };

  const handleCloseMessageDialog = () => {
    setIsMessageDialogOpen(false);
    setSelectedMessage(null);
    setEditedMessageText("");
  };

  const handleDeleteMessage = async () => {
    if (!selectedMessage || !chatServiceRef.current) return;
    
    try {
      // Fix the parameter order - should be (messageId, conversationId)
      const isDeleted = await chatServiceRef.current.deleteMessage(selectedMessage.id, selectedChatId);
      
      if (isDeleted) {
        setMessages(prevMessages =>
          prevMessages.map(msg => 
            msg.id === selectedMessage.id 
              ? { ...msg, isDeleted: true, text: "This message was deleted" } 
              : msg
          )
        );
      }
      
      handleCloseMessageDialog();
    } catch (error) {
      console.error("Error deleting message:", error);
    }
  };

  const handleUpdateMessage = async () => {
    if (!selectedMessage || !chatServiceRef.current || !editedMessageText.trim()) return;
    
    try {
      // Fix the parameter order - should be (messageId, conversationId, newText)
      const isUpdated = await chatServiceRef.current.updateMessage(
        selectedMessage.id,
        selectedChatId, 
        editedMessageText
      );
      
      if (isUpdated) {
        setMessages(prevMessages =>
          prevMessages.map(msg => 
            msg.id === selectedMessage.id 
              ? { ...msg, text: editedMessageText } 
              : msg
          )
        );
      }
      
      handleCloseMessageDialog();
    } catch (error) {
      console.error("Error updating message:", error);
    }
  };

  if (!selectedChatId) return <Box display="flex" flex={1} alignItems="center" justifyContent="center" color="gray" backgroundColor="white">Select a chat</Box>;

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
              onClick={() => handleMessageClick(msg)}
              sx={{
                maxWidth: "60%",
                padding: "10px 15px",
                borderRadius: "18px",
                bgcolor: msg.isDeleted ? "#f0f0f0" : (msg.isSystem ? "#f5f5f5" : (msg.isCurrentUser ? "#b3b3b3" : "#365B87")),
                color: msg.isDeleted ? "#888" : (msg.isSystem ? "gray" : (msg.isCurrentUser ? "black" : "white")),
                alignSelf: msg.isSystem ? "center" : (msg.isCurrentUser ? "flex-end" : "flex-start"),
                marginBottom: 1,
                fontStyle: msg.isDeleted || msg.isSystem ? "italic" : "normal",
                position: "relative",
                cursor: msg.isCurrentUser && !msg.isDeleted && !msg.isSystem ? "pointer" : "default",
                "&:hover": {
                  opacity: msg.isCurrentUser && !msg.isDeleted && !msg.isSystem ? 0.9 : 1,
                }
              }}
            >
              {msg.isDeleted ? "This message was deleted" : (msg.text || msg.content)}
              {!msg.isDeleted && msg.imageUrl && msg.imageUrl !== "None" && (
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
              <Typography 
                variant="caption" 
                sx={{ 
                  display: "block", 
                  textAlign: msg.isCurrentUser ? "right" : "left",
                  fontSize: "0.7rem",
                  marginTop: "4px",
                  opacity: 0.8,
                  color: msg.isSystem ? "gray" : (msg.isCurrentUser ? "black" : "white"),
                }}
              >
                {msg.formattedLocalTime || formatMessageTime(msg.sendedAt)}
              </Typography>
              {msg.isCurrentUser && (
                <Box 
                  sx={{ 
                    position: "absolute", 
                    right: 5, 
                    bottom: 2, 
                    fontSize: "0.8rem", 
                    color: "white",
                    display: "flex",
                    alignItems: "center"
                  }}
                >
                  {msg.isReaden ? <DoneAllIcon fontSize="inherit" /> : <CheckIcon fontSize="inherit" />}
                </Box>
              )}
              
              {!msg.isCurrentUser && !msg.isSystem && (
                <Box 
                  sx={{ 
                    position: "absolute", 
                    left: 5, 
                    bottom: 2, 
                    fontSize: "0.8rem", 
                    color: "#cccccc",
                    display: "flex",
                    alignItems: "center"
                  }}
                >
                  {msg.isReaden ? <DoneAllIcon fontSize="inherit" /> : <CheckIcon fontSize="inherit" />}
                </Box>
              )}
            </Box>
          ))}
      </Box>
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
          <InputBase
            sx={{ flex: 1, marginLeft: 1 }}
            placeholder="Type message here"
            value={newMessage}
            onChange={(e) => setNewMessage(e.target.value)}
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
      </Box>
      
      {/* Add Dialog component here */}
      <Dialog open={isMessageDialogOpen} onClose={handleCloseMessageDialog}>
        <DialogTitle>Message Options</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Edit Message"
            type="text"
            fullWidth
            value={editedMessageText}
            onChange={(e) => setEditedMessageText(e.target.value)}
            disabled={!selectedMessage || selectedMessage.isDeleted}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseMessageDialog} color="primary">
            Cancel
          </Button>
          <Button 
            onClick={handleDeleteMessage} 
            color="error"
            disabled={!selectedMessage || selectedMessage.isDeleted}
          >
            Delete
          </Button>
          <Button 
            onClick={handleUpdateMessage} 
            color="primary"
            disabled={!selectedMessage || selectedMessage.isDeleted || !editedMessageText.trim()}
          >
            Update
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}