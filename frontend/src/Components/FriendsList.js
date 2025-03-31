import React from "react";
import { Card, Typography, IconButton, Avatar, Box, Button } from "@mui/material";
import { Delete } from "@mui/icons-material";
import { API_URL } from "../constants";

const FriendsList = ({ friends, loadMore }) => {
  // Process the friends data to handle the nested structure
  const processFriends = () => {
    if (!friends) return [];
    
    // Check if friends has $values property (matches the backend format)
    if (friends.$values) {
      return friends.$values;
    }
    
    // If it's already an array, return it
    if (Array.isArray(friends)) {
      return friends;
    }
    
    return [];
  };
  
  const friendsList = processFriends();

  const handleRemoveFriend = async (friendId) => {
    try {
      await fetch(`${API_URL}/api/friends/remove/${friendId}`, { 
        method: "POST",
        credentials: "include"
      });
      // Refresh the friends list
      window.location.reload();
    } catch (error) {
      console.error("Error removing friend:", error);
    }
  };

  return (
    <Box sx={{ backgroundColor: "White", padding: 2, borderRadius: 2, overflow: "auto", maxHeight: "100%" }}>
      <Typography variant="h6" sx={{ fontWeight: 600, marginBottom: 1, color: "#345" }}>Friends</Typography>
      
      {friendsList.length === 0 ? (
        <Typography sx={{ color: "text.secondary", textAlign: "center", my: 2 }}>
          You don't have any friends yet
        </Typography>
      ) : (
        <>
          {friendsList.map((friend) => (
            <Card 
              key={friend.id || friend.Id} 
              sx={{ display: "flex", alignItems: "center", background: "#f5f5f5", padding: 1, borderRadius: 2, marginBottom: 1 }}
            >
              <Avatar 
                sx={{ width: 50, height: 50, marginRight: 2 }} 
                src={friend.avatarUrl !== "None" ? friend.avatarUrl || friend.AvatarUrl : null} 
              />
              <Box sx={{ flexGrow: 1 }}>
                <Typography sx={{ fontSize: 16, fontWeight: 500 }}>
                  {friend.nickName || friend.NickName}
                </Typography>
                <Typography 
                  variant="caption" 
                  sx={{ 
                    color: (friend.activeStatus === "Online" || friend.ActiveStatus === "Online") ? "green" : "gray" 
                  }}
                >
                  {friend.activeStatus || friend.ActiveStatus}
                </Typography>
              </Box>
              <IconButton 
                size="small" 
                color="error" 
                onClick={() => handleRemoveFriend(friend.friendId || friend.FriendId)}
              >
                <Delete />
              </IconButton>
            </Card>
          ))}
          
          {friendsList.length >= 20 && (
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

export default FriendsList;
