import React from "react";
import { useNavigate } from "react-router-dom";
import { Card, Typography, IconButton, Avatar, Box } from "@mui/material";
import { Chat } from "@mui/icons-material";

const FriendsList = ({ friends }) => {
  const navigate = useNavigate();

  return (
    <Box sx={{ backgroundColor: "White", height: "100%" }}>
      <Typography variant="h6" sx={{ fontWeight: 600, marginBottom: 1, color: "#345" }}>Friends</Typography>
      {friends.map((friend) => (
        <Card key={friend.id} sx={{ display: "flex", alignItems: "center", background: "#f5f5f5", padding: 1, borderRadius: 2, marginBottom: 1 }}>
          <Avatar sx={{ width: 50, height: 50, marginRight: 2 }} src={friend.avatar} />
          <Typography sx={{ flexGrow: 1, fontSize: 16, fontWeight: 500 }}>{friend.senderName}</Typography>
          <IconButton size="small" color="primary" onClick={() => navigate("/messages")}>
            <Chat />
          </IconButton>
        </Card>
      ))}
    </Box>
  );
};

export default FriendsList;
