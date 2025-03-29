import { useState, useEffect, useRef } from "react";
import { List, ListItem, ListItemAvatar, Avatar, ListItemText, Typography, Paper, Button, Box } from "@mui/material";
import axios from "axios";
import { API_URL } from "../constants";

export default function ChatsList({ onSelectChat }) {
  const [chats, setChats] = useState([]);
  const [lastConversationId, setLastConversationId] = useState(null);
  const [hasMore, setHasMore] = useState(true);

  const fetchChats = async (loadMore = false) => {
    try {
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
    }
  };

  useEffect(() => {
    fetchChats();
  }, []);

  const loadMoreChats = () => {
    fetchChats(true);
  };

  return (
    <Paper elevation={3} sx={{ width: "25%", padding: 2, borderRight: 1, borderColor: "grey.300", display: "flex", flexDirection: "column" }}>
      <Typography variant="h6" color="primary" gutterBottom>
        Messages
      </Typography>
      <List sx={{ flexGrow: 1, overflow: "auto" }}>
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
      </List>
      {hasMore && (
        <Box sx={{ textAlign: "center", mt: 1 }}>
          <Button variant="outlined" onClick={loadMoreChats}>
            Load More
          </Button>
        </Box>
      )}
    </Paper>
  );
}
