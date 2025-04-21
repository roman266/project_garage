import { useState, useEffect, useRef, useCallback } from "react";
import { List, ListItem, ListItemAvatar, Avatar, ListItemText, Typography, Paper, Button, Box, Badge } from "@mui/material";
import { 
  fetchChats, 
  fetchUnreadCount, 
  setupChatNotifications, 
  findChatById,
  joinMultipleChats,
  removeMessageCallback
} from "../../services/conversationService";

export default function ChatsList({ onSelectChat, currentChatId }) {
  const [chats, setChats] = useState([]);
  const [lastConversationId, setLastConversationId] = useState(null);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(false);
  const [unreadCounts, setUnreadCounts] = useState({});
  const listRef = useRef(null);
  const chatsRef = useRef([]);

  // Update the ref whenever chats change
  useEffect(() => {
    chatsRef.current = chats;
  }, [chats]);

  const loadChats = async (loadMore = false) => {
    try {
      setLoading(true);
      const result = await fetchChats(loadMore ? lastConversationId : null);

      if (result.chats.length > 0) {
        setLastConversationId(result.lastConversationId);
        setHasMore(result.hasMore);

        const unreadCountsData = {};
        await Promise.all(
          result.chats.map(async (chat) => {
            const count = await fetchUnreadCount(chat.conversationId);
            unreadCountsData[chat.conversationId] = count;
          })
        );

        setUnreadCounts(prev => ({ ...prev, ...unreadCountsData }));

        if (loadMore) {
          setChats(prev => [...prev, ...result.chats]);
        } else {
          setChats(result.chats);
        }
        
        // Join all loaded chats to receive notifications
        const chatIds = result.chats.map(chat => chat.conversationId);
        await joinMultipleChats(chatIds);
      } else {
        setHasMore(false);
      }
    } catch (error) {
      console.error("Error fetching chats", error);
    } finally {
      setLoading(false);
    }
  };

  // Handle new message notifications - using useCallback to maintain reference
  const handleNewMessage = useCallback(async (conversationId, messageData) => {
    console.log("Handling new message for conversation:", conversationId);

    // Increment unread count for this conversation
    setUnreadCounts(prev => ({
      ...prev,
      [conversationId]: (prev[conversationId] || 0) + 1
    }));

    // Check if this conversation is already in our list
    const existingChatIndex = chatsRef.current.findIndex(chat => chat.conversationId === conversationId);

    if (existingChatIndex !== -1) {
      // Move this conversation to the top of the list
      const updatedChats = [...chatsRef.current];
      const chatToMove = updatedChats.splice(existingChatIndex, 1)[0];
      updatedChats.unshift(chatToMove);
      setChats(updatedChats);
    } else {
      // This is a new conversation or one that wasn't loaded yet
      // We need to reload all chats since we don't have a direct endpoint to get a single chat
      loadChats();
    }
  }, []);

  useEffect(() => {
    loadChats();

    // Set up SignalR notification handling
    setupChatNotifications(handleNewMessage)
      .then(success => {
        if (success) {
          console.log("Chat notifications set up successfully");
        } else {
          console.error("Failed to set up chat notifications");
        }
      })
      .catch(error => {
        console.error("Error setting up chat notifications:", error);
      });

    return () => {
      // Clean up by removing our callback when component unmounts
      removeMessageCallback(handleNewMessage);
    };
  }, [handleNewMessage]);

  const handleScroll = (e) => {
    const { scrollTop, scrollHeight, clientHeight } = e.target;

    if (scrollHeight - scrollTop - clientHeight < 50) {
      if (hasMore && !loading) {
        console.log("Loading more chats...");
        loadMoreChats();
      }
    }
  };

  const loadMoreChats = () => {
    if (!loading) {
      console.log("Fetching more chats with lastConversationId:", lastConversationId);
      loadChats(true);
    }
  };

  // Add a function to refresh chats when needed
  const refreshChats = () => {
    setChats([]);
    setLastConversationId(null);
    setHasMore(true);
    setUnreadCounts({});
    loadChats(false);
  };

  return (
    <Paper elevation={3} sx={{ width: "25%", padding: 2, borderRight: 1, borderColor: "grey.300", display: "flex", flexDirection: "column", height: "100%" }}>
      <Typography variant="h6" color="primary" gutterBottom>
        Messages
      </Typography>
      <List
        ref={listRef}
        sx={{
          flexGrow: 1,
          overflow: "auto",
          maxHeight: "calc(100vh - 150px)",
          '&::-webkit-scrollbar': {
            width: '8px',
          },
          '&::-webkit-scrollbar-thumb': {
            backgroundColor: 'rgba(0,0,0,0.2)',
            borderRadius: '4px',
          }
        }}
        onScroll={handleScroll}
      >
        {chats.length > 0 ? (
          chats.map((chat) => (
            <ListItem
              button
              key={chat.conversationId}
              onClick={() => {
                console.log("Chat selected:", chat.conversationId);
                onSelectChat(chat.conversationId);
                // Reset unread count when selecting a chat
                setUnreadCounts(prev => ({ ...prev, [chat.conversationId]: 0 }));
              }}
              sx={{
                cursor: 'pointer',
                backgroundColor: currentChatId === chat.conversationId ? '#e6e6e6' : 'transparent',
                '&:hover': { backgroundColor: currentChatId === chat.conversationId ? '#e6e6e6' : '#f5f5f5' },
                position: 'relative'
              }}
            >
              <ListItemAvatar>
                <Avatar
                  sx={{ bgcolor: "black" }}
                  src={chat.profilePictureUrl !== "None" ? chat.profilePictureUrl : null}
                />
              </ListItemAvatar>
              <ListItemText
                primary={chat.userName || "Unknown User"}
                secondary={
                  <Typography
                    variant="body2"
                    sx={{ color: chat.activeStatus === "Online" ? "green" : "gray" }}
                  >
                    {chat.activeStatus || "Offline"}
                  </Typography>
                }
              />
              {unreadCounts[chat.conversationId] > 0 && (
                <Badge
                  badgeContent={unreadCounts[chat.conversationId]}
                  color="primary"
                  sx={{
                    '& .MuiBadge-badge': {
                      right: 0,
                      top: '50%',
                      transform: 'translateY(-50%)',
                      backgroundColor: '#e53935',
                      color: 'white',
                    }
                  }}
                />
              )}
            </ListItem>
          ))
        ) : (
          <Typography sx={{ p: 2, textAlign: 'center', color: 'text.secondary' }}>
            No conversations yet
          </Typography>
        )}
        {loading && (
          <Box sx={{ textAlign: 'center', p: 2 }}>
            <Typography variant="body2" color="text.secondary">
              Loading...
            </Typography>
          </Box>
        )}
      </List>
    </Paper>
  );
}