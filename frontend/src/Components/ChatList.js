import { useState, useEffect } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { List, ListItem, ListItemAvatar, Avatar, ListItemText, Typography, Paper, Dialog, DialogActions, DialogContent, DialogTitle, TextField, Button, IconButton } from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import axios from "axios";

export default function ChatsList({ onSelectChat }) {
  const [chats, setChats] = useState([]);
  const [connection, setConnection] = useState(null);
  const [openDialog, setOpenDialog] = useState(false); // State to handle dialog visibility
  const [username, setUsername] = useState(""); // State for the input field

  useEffect(() => {
    const connect = new HubConnectionBuilder()
      .withUrl("http://localhost:5000/chatHub", { withCredentials: true })
      .build();

    connect.start()
      .then(() => {
        console.log("SignalR connected");

        connect.on("ReceiveNewChat", (chat) => {
          setChats((prevChats) => [...prevChats, chat]);
        });

        setConnection(connect);
      })
      .catch((error) => console.error("SignalR connection error", error));

    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, []);

  useEffect(() => {
    axios.get("http://localhost:5000/api/conversation/my-conversations", { withCredentials: true })
      .then((response) => setChats(response.data.conversationList))
      .catch((error) => console.error("Error fetching chats", error));
  }, []);

  // Function to open the dialog
  const handleOpenDialog = () => {
    setOpenDialog(true);
  };

  // Function to close the dialog
  const handleCloseDialog = () => {
    setUsername(""); 
    setOpenDialog(false);
  };

  // Function to handle creating a new chat
  const handleCreateChat = () => {
    if (!username.trim()) return; // If username is empty, do nothing

    axios.post("http://localhost:5000/api/conversation/create", { username }, { withCredentials: true })
      .then((response) => {
        const newChat = response.data; // Assuming the response contains the new chat data
        setChats((prevChats) => [...prevChats, newChat]);
        setOpenDialog(false); // Close the dialog
        setUsername(""); // Clear the input field
      })
      .catch((error) => console.error("Error creating chat", error));
  };

  return (
    <Paper elevation={3} sx={{ width: "25%", padding: 2, borderRight: 1, borderColor: "grey.300" }}>
      <Typography variant="h6" color="primary" gutterBottom sx={{ display: "flex", alignItems: "center" }}>
        Messages
        <IconButton onClick={handleOpenDialog} sx={{ marginLeft: 2 }}>
          <SearchIcon />
        </IconButton>
      </Typography>
      <List>
        {chats.map((chat) => (
          <ListItem button key={chat.id} onClick={() => onSelectChat(chat.id)}>
            <ListItemAvatar>
              <Avatar sx={{ bgcolor: "black" }} />
            </ListItemAvatar>
            <ListItemText primary={chat.name} secondary={chat.lastMessage || "No messages"} />
          </ListItem>
        ))}
      </List>

      {/* Dialog for searching and creating a new chat */}
      <Dialog open={openDialog} onClose={handleCloseDialog}>
        <DialogTitle>Create New Chat</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Enter Username"
            fullWidth
            variant="outlined"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog} color="secondary">
            Cancel
          </Button>
          <Button onClick={handleCreateChat} color="primary">
            Create Chat
          </Button>
        </DialogActions>
      </Dialog>
    </Paper>
  );
}
