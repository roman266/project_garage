import { useState, useEffect, useRef } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { Box, Typography, Avatar, Paper, IconButton, InputBase } from "@mui/material";
import InsertEmoticonIcon from "@mui/icons-material/InsertEmoticon";
import AddIcon from "@mui/icons-material/Add";
import axios from "axios";
import { API_URL } from "../constants";

export default function ChatWindow({ selectedChatId }) {
  const [messages, setMessages] = useState([]);
  const [connection, setConnection] = useState(null);
  const [newMessage, setNewMessage] = useState("");
  const connectionRef = useRef(null);

  useEffect(() => {
    if (!selectedChatId) return;

    const connect = new HubConnectionBuilder()
      .withUrl(`${API_URL}/chatHub`, { withCredentials: true })
      .build();

    connect.start()
      .then(() => {
        console.log("SignalR connected");
        connect.on("ReceiveMessage", (message) => {
          setMessages((prevMessages) => [...prevMessages, message]);
        });
        connectionRef.current = connect;

        axios.get(`${API_URL}/api/conversations/${selectedChatId}/messages`, { withCredentials: true })
          .then((response) => setMessages(response.data))
          .catch((error) => console.error("Error fetching messages", error));
      })
      .catch((error) => console.error("SignalR connection error", error));

    setConnection(connect);

    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop();
      }
    };
  }, [selectedChatId]);

  const handleSendMessage = async () => {
    if (newMessage.trim() && connection) {
      try {
        await connection.invoke("SendMessage", selectedChatId, newMessage);
        setMessages((prevMessages) => [...prevMessages, { text: newMessage, isMe: true }]);
        setNewMessage("");
      } catch (error) {
        console.error("Error sending message", error);
      }
    }
  };

  if (!selectedChatId) return (
    <Box display="flex" flex={1} alignItems="center" justifyContent="center" color="gray" backgroundColor="white">
      Select a chat
    </Box>
  );

  return (
    <Box display="flex" flexDirection="column" flex={1} component={Paper} elevation={2}>
      <Box display="flex" alignItems="center" padding={2} bgcolor="#e0e0e0">
        <Avatar sx={{ bgcolor: "black", marginRight: 2 }} />
        <Typography variant="h6">Chat {selectedChatId}</Typography>
      </Box>
      <Box flex={1} padding={2} bgcolor="white" overflow="auto" display="flex" flexDirection="column">
        {messages.map((msg, index) => (
          <Box
            key={index}
            sx={{
              maxWidth: "60%",
              padding: "10px 15px",
              borderRadius: "18px",
              bgcolor: msg.isMe ? "black" : "#1e88e5",
              color: "white",
              alignSelf: msg.isMe ? "flex-end" : "flex-start",
              marginBottom: 1,
            }}
          >
            {msg.text}
          </Box>
        ))}
      </Box>
      <Box padding={2} bgcolor="#e0e0e0" display="flex" alignItems="center">
        <Box sx={{ display: "flex", alignItems: "center", backgroundColor: "#d6d6d6", borderRadius: 50, flex: 1, padding: "5px 10px" }}>
          <InputBase
            sx={{ flex: 1, marginLeft: 1 }}
            placeholder="Type message here"
            value={newMessage}
            onChange={(e) => setNewMessage(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleSendMessage()}
          />
        </Box>
        <IconButton><InsertEmoticonIcon sx={{ color: "#1e497c" }} /></IconButton>
        <IconButton><AddIcon sx={{ color: "#1e497c" }} /></IconButton>
      </Box>
    </Box>
  );
}
