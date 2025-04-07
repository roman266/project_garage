import { useState, useEffect, useRef } from "react";
import { List, ListItem, ListItemAvatar, Avatar, ListItemText, Typography, Paper, Button, Box } from "@mui/material";
import axios from "axios";
import { API_URL } from "../../constants";

export default function ChatsList({ onSelectChat }) {
  const [chats, setChats] = useState([]);
  const [lastConversationId, setLastConversationId] = useState(null);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(false);
  const listRef = useRef(null);

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
      
      // Handle the response data structure
      let newChats = [];
      if (response.data.$values) {
        newChats = response.data.$values;
      } else if (response.data.conversationList) {
        newChats = response.data.conversationList;
      } else if (Array.isArray(response.data)) {
        newChats = response.data;
      }
      
      if (newChats.length > 0) {
        // Use conversationId instead of id
        setLastConversationId(newChats[newChats.length - 1].conversationId);
        setHasMore(newChats.length >= 15);
        
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
    
    // When user scrolls to bottom (with a small threshold)
    if (scrollHeight - scrollTop - clientHeight < 50) {
      if (hasMore && !loading) {
        loadMoreChats();
      }
    }
  };

  const loadMoreChats = () => {
    if (!loading) {
      fetchChats(true);
    }
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
              }}
              sx={{ 
                cursor: 'pointer',
                '&:hover': { backgroundColor: '#f5f5f5' }
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