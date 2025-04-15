import { useState, useEffect, useRef } from "react";
import { List, ListItem, ListItemAvatar, Avatar, ListItemText, Typography, Paper, Button, Box, Badge } from "@mui/material";
import axios from "axios";
import { API_URL } from "../../constants";

export default function ChatsList({ onSelectChat, currentChatId }) {
  const [chats, setChats] = useState([]);
  const [lastConversationId, setLastConversationId] = useState(null);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(false);
  const [unreadCounts, setUnreadCounts] = useState({});
  const listRef = useRef(null);

  const fetchUnreadCount = async (conversationId) => {
    try {
      const response = await axios.get(`${API_URL}/api/message/${conversationId}/unreaded-count`, {
        withCredentials: true
      });
      console.log(`Unread count for chat ${conversationId}:`, response.data);
      return response.data;
    } catch (error) {
      console.error(`Error fetching unread count for chat ${conversationId}:`, error);
      return 0;
    }
  };

  const fetchChats = async (loadMore = false) => {
    try {
      setLoading(true);
      const params = {
        limit: 15,
        lastConversationId: loadMore ? lastConversationId : null
      };

      const response = await axios.get(`${API_URL}/api/conversations/my-conversations`, {
        params,
        withCredentials: true
      });

      let newChats = [];
      if (response.data.$values) {
        newChats = response.data.$values;
      } else if (response.data.conversationList) {
        newChats = response.data.conversationList;
      } else if (Array.isArray(response.data)) {
        newChats = response.data;
      }

      if (newChats.length > 0) {
        setLastConversationId(newChats[newChats.length - 1].conversationId);
        setHasMore(newChats.length >= 15);

        const unreadCountsData = {};
        await Promise.all(
          newChats.map(async (chat) => {
            const count = await fetchUnreadCount(chat.conversationId);
            unreadCountsData[chat.conversationId] = count;
          })
        );

        setUnreadCounts(prev => ({ ...prev, ...unreadCountsData }));

        if (loadMore) {
          setChats(prev => [...prev, ...newChats]);
        } else {
          setChats(newChats);
        }
      } else {
        setHasMore(false);
      }
    } catch (error) {
      console.error("Error fetching chats", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchChats();
  }, []);

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
      fetchChats(true);
    }
  };

  // Add a function to refresh chats when needed
  const refreshChats = () => {
    setChats([]);
    setLastConversationId(null);
    setHasMore(true);
    setUnreadCounts({});
    fetchChats(false);
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
                // Скидаємо лічильник непрочитаних повідомлень при виборі чату
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