import React from "react";
import { Card, Typography, IconButton, Avatar, Box } from "@mui/material";
import { Chat } from "@mui/icons-material";
import { useNavigate } from "react-router-dom";

const FriendCard = ({ user }) => {
  const navigate = useNavigate(); 
  const defaultAvatar = "/user-avatar.png"; 

  const goChat = () => {
    navigate('/messages'); 
  };

  return (
    <Card 
      sx={{ 
        display: "flex", 
        alignItems: "center", 
        justifyContent: "space-between", 
        background: "#f5f5f5", 
        padding: 1.5, 
        borderRadius: 2 
      }}
    >
      <Box sx={{ display: "flex", alignItems: "center" }}>
        <Avatar 
          sx={{ width: 40, height: 40, marginRight: 1.5 }} 
          src={user.avatarUrl || defaultAvatar} 
        />
        <Box>
          <Typography sx={{ fontSize: 16, fontWeight: 500 }}>{user.nickName}</Typography>
          <Typography variant="caption" sx={{ color: "gray" }}>{user.activeStatus}</Typography>
        </Box>
      </Box>
      <IconButton size="small" onClick={goChat} sx={{ color: "#3f5975" }}>
        <Chat sx={{ fontSize: 28 }} />
      </IconButton>
    </Card>
  );
};

export default FriendCard;
