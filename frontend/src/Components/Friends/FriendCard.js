import React from "react";
import { Card, Typography, IconButton, Avatar, Box } from "@mui/material";
import { Chat, Close } from "@mui/icons-material";
import { useNavigate } from "react-router-dom";
import { API_URL } from "../../constants";

const FriendCard = ({ user, deleteFriend }) => {
  const navigate = useNavigate(); 
  const defaultAvatar = "/user-avatar.png"; 

  const goChat = async (userId) => {
    try {
      // Пробуємо отримати приватний чат
      const response = await fetch(`${API_URL}/api/conversations/get-private/${userId}`, {
        method: "GET",
        credentials: "include"
      });
  
      let conversation;
  
      if (response.status === 200) {
        conversation = await response.json(); // Існуючий чат
      } else if (response.status === 404) {
        // Якщо нема чату — створюємо новий
        const createResponse = await fetch(`${API_URL}/api/conversations/start/${userId}`, {
          method: "POST",
          credentials: "include"
        });
  
        if (!createResponse.ok) {
          throw new Error("Не вдалося створити чат");
        }
  
        conversation = await createResponse.json(); // Новий чат
      } else {
        throw new Error("Помилка при отриманні чату");
      }
  
      // Перенаправляємо на чат по id
      navigate(`/messages/${conversation.id}`);
    } catch (error) {
      console.error("Помилка під час переходу до чату:", error);
    }
  };

  const  onCancel = async (requestId) => {
      try {
          const response = await fetch(
              `${API_URL}/api/friends/reject/${requestId}`,
              {
                  method: "DELETE",
                  credentials: "include"
              }
               
          )
          console.log(response.data);
      } catch (ex)  {
          console.log(ex);
      }
  } 

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
      <Box>
        <IconButton size="small" color="error" onClick={() => {onCancel(user.id); deleteFriend(user.id)}}>
          <Close />
        </IconButton>
        <IconButton size="small" onClick={() => {goChat(user.friendId)}} sx={{ color: "#3f5975" }}>
          <Chat sx={{ fontSize: 28 }} />
        </IconButton>
      </Box>
    </Card>
  );
};

export default FriendCard;
