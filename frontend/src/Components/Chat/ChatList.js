import { useState, useEffect, useRef } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { List, ListItem, ListItemAvatar, Avatar, ListItemText, Typography, Paper, Dialog, DialogActions, DialogContent, DialogTitle, TextField, Button, IconButton } from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import axios from "axios";
import { API_URL } from "../../constants";

export default function ChatsList({ onSelectChat }) {
  const [chats, setChats] = useState([]);
  const [connection, setConnection] = useState(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [username, setUsername] = useState("");
  const [users, setUsers] = useState([]);
  const connectionRef = useRef(null);

  useEffect(() => {
    const fetchChats = async () => {
      try {
        const response = await axios.get(`${API_URL}/api/conversations/my-conversations`, { withCredentials: true });
        setChats(response.data);
      } catch (error) {
        console.error("Error fetching chats", error);
      }
    };
    fetchChats();
  }, []);

  useEffect(() => {
    const connect = new HubConnectionBuilder()
      .withUrl(`${API_URL}/chatHub`, { withCredentials: true })
      .build();

    connect.start()
      .then(() => {
        console.log("SignalR connected");
        connectionRef.current = connect;
        connect.on("ReceiveNewChat", (chat) => {
          setChats((prevChats) => [...prevChats, chat]);
        });
      })
      .catch((error) => console.error("SignalR connection error", error));

    setConnection(connect);
    return () => connect.stop();
  }, []);

  const handleSearchUsers = async () => {
    if (!username.trim()) return;
    try {
      const response = await axios.get(`${API_URL}/api/profile/search-users`, {
        params: { Query: username },
        withCredentials: true,
      });
      setUsers(response.data);
    } catch (error) {
      console.error("Error searching for users", error);
    }
  };

  const handleCreateChat = (user) => {
    axios.post(`${API_URL}/api/conversations/start/${user.id}`, {}, { withCredentials: true })
  .then((response) => {
    if (response.data && response.data.conversation) {
      setChats((prevChats) => [...prevChats, response.data.conversation]);
      onSelectChat(response.data.conversation.id); // Открываем чат после создания
    }
  })
  .catch((error) => console.error("Error creating chat:", error));

  };
  

  return (
    <Paper elevation={3} sx={{ width: "25%", padding: 2, borderRight: 1, borderColor: "grey.300" }}>
      <Typography variant="h6" color="primary" gutterBottom sx={{ display: "flex", alignItems: "center" }}>
        Messages
        <IconButton onClick={() => setOpenDialog(true)} sx={{ marginLeft: 2 }}>
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

      <Dialog open={openDialog} onClose={() => setOpenDialog(false)}>
        <DialogTitle>Search Users</DialogTitle>
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
          <Button onClick={handleSearchUsers} color="primary" fullWidth sx={{ marginTop: 2 }}>
            Search
          </Button>

          {users.length > 0 && (
            <List>
              {users.map((user) => (
                <ListItem button key={user.id} onClick={() => handleCreateChat(user)}>
                  <ListItemText primary={user.username} />
                </ListItem>
              ))}
            </List>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(false)} color="secondary">
            Cancel
          </Button>
        </DialogActions>
      </Dialog>
    </Paper>
  );
}
