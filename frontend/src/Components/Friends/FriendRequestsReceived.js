import React from "react";
import { Card, Typography, IconButton, Avatar, Box, Button } from "@mui/material";
import { Check, Close } from "@mui/icons-material";
import { API_URL } from "../../constants";

const FriendRequestsReceived = ({ received, fetchFriendsData, loadMore }) => {
  // Process the received data to handle the nested structure
  const processRequests = () => {
    if (!received) return [];
    
    // Check if received has $values property (matches the backend format)
    if (received.$values) {
      return received.$values;
    }
    
    // If it's already an array, return it
    if (Array.isArray(received)) {
      return received;
    }
    
    return [];
  };
  
  const receivedList = processRequests();

  const handleAccept = async (requestId) => {
    try {
      await fetch(`${API_URL}/api/friends/accept/${requestId}`, { 
        method: "POST",
        credentials: "include"
      });
      fetchFriendsData();
    } catch (error) {
      console.error("Error accepting request:", error);
    }
  };

  const handleReject = async (requestId) => {
    try {
      await fetch(`${API_URL}/api/friends/reject/${requestId}`, { 
        method: "POST",
        credentials: "include"
      });
      fetchFriendsData();
    } catch (error) {
      console.error("Error rejecting request:", error);
    }
  };

  return (
    <Box sx={{ backgroundColor: "White", padding: 2, borderRadius: 2, overflow: "auto", maxHeight: "100%" }}>
      <Typography variant="h6" sx={{ fontWeight: 600, marginBottom: 1, color: "#345" }}>Friends request</Typography>
      
      {receivedList.length === 0 ? (
        <Typography sx={{ color: "text.secondary", textAlign: "center", my: 2 }}>
          No friend requests received
        </Typography>
      ) : (
        <>
          {receivedList.map((user) => (
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
              <IconButton size="small" color="success" onClick={() => handleAccept(user.id || user.Id)}>
                <Check />
              </IconButton>
              <IconButton size="small" color="error" onClick={() => handleReject(user.id || user.Id)}>
                <Close />
              </IconButton>
            </Card>
          ))}
          
          {receivedList.length >= 20 && (
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

export default FriendRequestsReceived;
