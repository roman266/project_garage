import React from "react";
import { Card, Typography, IconButton, Avatar, Box } from "@mui/material";
import { Check, Close, Chat } from "@mui/icons-material";
import axios from "axios";
import { API_URL } from "../../constants";

const onAccept = async (requestId, token) => {
    // try {
    //   const response = await axios.post(
    //     `${API_URL}/friends/accept/${requestId}`,
    //     {}, 
    //     { headers: { Authorization: `Bearer ${token}` } }
    //   );
  
    //   console.log("Заявка принята:", response.data);
    // } catch (error) {
    //   console.error("Ошибка при принятии заявки:", error.response?.data || error.message);
        console.log("accept");
    }
const onReject = async () => {
    console.log("reject");
}
  
const IncomingRequestCard = ({ user }) => (
    <Card sx={{ display: "flex", alignItems: "center", background: "#f5f5f5", padding: 1, borderRadius: 2, marginBottom: 1 }}>
      <Avatar sx={{ width: 50, height: 50, marginRight: 2 }} src={user.avatarUrl} />
      <Box sx={{ flexGrow: 1 }}>
        <Typography sx={{ fontSize: 16, fontWeight: 500 }}>{user.nickName}</Typography>
        <Typography variant="caption" sx={{ color: user.activeStatus === "Online" ? "green" : "gray" }}>{user.activeStatus}</Typography>
      </Box>
      <IconButton size="small" color="success" onClick={() => onAccept(user.id, user.id)}><Check /></IconButton>
      <IconButton size="small" color="error" onClick={() => onReject(user.id)}><Close /></IconButton>
    </Card>
  );

export default IncomingRequestCard;