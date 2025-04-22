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
      // Try to get private chat
      const response = await fetch(`${API_URL}/api/conversations/get-private/${userId}`, {
        method: "GET",
        credentials: "include"
      });

      let conversationId;

      if (response.status === 200) {
        const data = await response.json();
        conversationId = data.id; // Existing chat
      } else if (response.status === 404) {
        // If no chat exists - create new one
        const createResponse = await fetch(`${API_URL}/api/conversations/start/${userId}`, {
          method: "POST",
          credentials: "include"
        });

        if (!createResponse.ok) {
          throw new Error("Failed to create chat");
        }

        const data = await createResponse.json();
        conversationId = data.id; // New chat
      } else {
        throw new Error("Error getting chat");
      }

      // Redirect to chat by id
      navigate(`/messages/${conversationId}`);
    } catch (error) {
      console.error("Error during chat navigation:", error);
    }
  };

  const onCancel = async (requestId) => {
    try {
      const response = await fetch(
        `${API_URL}/api/friends/reject/${requestId}`,
        {
          method: "DELETE",
          credentials: "include"
        }

      )
      console.log(response.data);
    } catch (ex) {
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
        <IconButton size="small" color="error" onClick={() => { onCancel(user.id); deleteFriend(user.id) }}>
          <Close />
        </IconButton>
        <IconButton size="small" onClick={() => { goChat(user.friendId) }} sx={{ color: "#3f5975" }}>
          <Chat sx={{ fontSize: 28 }} />
        </IconButton>
      </Box>
    </Card>
  );
};

export default FriendCard;
