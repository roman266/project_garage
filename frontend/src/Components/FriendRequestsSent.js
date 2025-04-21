import React from "react";
import { Card, Typography, IconButton, Avatar, Box } from "@mui/material";
import { Close } from "@mui/icons-material";
import { API_URL } from "../constants";

const FriendRequestsSent = ({ sent, fetchFriendsData }) => {
  const handleCancel = async (requestId) => {
    try {
      await fetch(`${API_URL}/api/friends/reject/${requestId}`, { method: "POST" });
      fetchFriendsData();
    } catch (error) {
      console.error("Ошибка при отмене заявки:", error);
    }
  };

  return (
    <Box sx={{ backgroundColor: "White" }}>
      <Typography variant="h6" sx={{ fontWeight: 600, marginBottom: 1, color: "#345" }}>Sent Friends request</Typography>
      {sent.map((user) => (
        <Card key={user.requestId} sx={{ display: "flex", alignItems: "center", background: "#f5f5f5", padding: 1, borderRadius: 2, marginBottom: 1 }}>
          <Avatar sx={{ width: 50, height: 50, marginRight: 2 }} src={user.avatar} />
          <Typography sx={{ flexGrow: 1, fontSize: 16, fontWeight: 500 }}>{user.senderName}</Typography>
          <IconButton size="small" color="error" onClick={() => handleCancel(user.requestId)}>
            <Close />
          </IconButton>
        </Card>
      ))}
    </Box>
  );
};

export default FriendRequestsSent;
