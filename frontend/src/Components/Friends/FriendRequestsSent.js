import React from "react";
import { Card, Typography, IconButton, Avatar, Box, Button } from "@mui/material";
import { Close } from "@mui/icons-material";
import { API_URL } from "../../constants";

const FriendRequestsSent = ({ sent, fetchFriendsData, loadMore }) => {
  // Process the sent data to handle the nested structure
  const processRequests = () => {
    if (!sent) return [];
    
    // Check if sent has $values property (matches the backend format)
    if (sent.$values) {
      return sent.$values;
    }
    
    // If it's already an array, return it
    if (Array.isArray(sent)) {
      return sent;
    }
    
    return [];
  };
  
  const sentList = processRequests();

  const handleCancel = async (requestId) => {
    try {
      await fetch(`${API_URL}/api/friends/cancel/${requestId}`, { 
        method: "POST",
        credentials: "include"
      });
      fetchFriendsData();
    } catch (error) {
      console.error("Error canceling request:", error);
    }
  };

  return (
    <Box sx={{ backgroundColor: "White", padding: 2, borderRadius: 2, overflow: "auto", maxHeight: "100%" }}>
      <Typography variant="h6" sx={{ fontWeight: 600, marginBottom: 1, color: "#345" }}>Sent requests</Typography>
      
      {sentList.length === 0 ? (
        <Typography sx={{ color: "text.secondary", textAlign: "center", my: 2 }}>
          No friend requests sent
        </Typography>
      ) : (
        <>
          {sentList.map((user) => (
            <Card 
              key={user.id || user.Id} 
              sx={{ display: "flex", alignItems: "center", background: "#f5f5f5", padding: 1, borderRadius: 2, marginBottom: 1 }}
            >
              <Avatar 
                sx={{ width: 50, height: 50, marginRight: 2 }} 
                src={user.avatarUrl !== "None" ? user.avatarUrl || user.AvatarUrl : null} 
              />
              <Box sx={{ flexGrow: 1 }}>
                <Typography sx={{ fontSize: 16, fontWeight: 500 }}>
                  {user.nickName || user.NickName}
                </Typography>
                <Typography 
                  variant="caption" 
                  sx={{ 
                    color: (user.activeStatus === "Online" || user.ActiveStatus === "Online") ? "green" : "gray" 
                  }}
                >
                  {user.activeStatus || user.ActiveStatus}
                </Typography>
              </Box>
              <IconButton size="small" color="error" onClick={() => handleCancel(user.id || user.Id)}>
                <Close />
              </IconButton>
            </Card>
          ))}
          
          {sentList.length >= 20 && (
            <Button 
              variant="outlined" 
              fullWidth 
              sx={{ mt: 2 }} 
              onClick={loadMore}
            >
              Load More
            </Button>
          )}
        </>
      )}
    </Box>
  );
};

export default FriendRequestsSent;
