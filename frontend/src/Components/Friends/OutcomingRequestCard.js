import React from "react";
import { Card, Typography, IconButton, Avatar, Box } from "@mui/material";
import { Check, Close, Chat } from "@mui/icons-material";
import { API_URL } from "../../constants";

const onCancel = (requestId) => {
    console.log("Cansel outcoming friend request");
} 

const OutcomingRequestCard = ({ user }) => (
    <Card sx={{ display: "flex", alignItems: "center", background: "#f5f5f5", padding: 1, borderRadius: 2, marginBottom: 1 }}>
      <Avatar sx={{ width: 50, height: 50, marginRight: 2 }} src={user.avatarUrl} />
      <Box sx={{ flexGrow: 1 }}>
        <Typography sx={{ fontSize: 16, fontWeight: 500 }}>{user.nickName}</Typography>
        <Typography variant="caption" sx={{ color: user.activeStatus === "Online" ? "green" : "gray" }}>{user.activeStatus}</Typography>
      </Box>
      <IconButton size="small" color="error" onClick={() => onCancel(user.id)}><Close /></IconButton>
    </Card>
  );

export default OutcomingRequestCard;