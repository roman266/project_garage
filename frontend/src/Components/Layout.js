import React, { useState } from "react";
import { Outlet, Link, useNavigate } from "react-router-dom";
import ProfileCard from "./ProfileCard";
import SearchResults from "./SearchResults";
import {
  AppBar,
  Toolbar,
  Typography,
  InputBase,
  Box,
  Drawer,
  List,
  ListItem,
  ListItemText,
  Button,
  ListItemIcon,
  IconButton
} from "@mui/material";
import { useAuth } from "../context/AuthContext";
import SearchIcon from "@mui/icons-material/Search";
import axios from "axios";

const drawerWidth = 240;

// Используйте относительные пути для ссылок
const menuItems = [
  { text: "My profile", imgSrc: "/profile.svg", pageHref: "/my-profile" },
  { text: "Friends", imgSrc: "/friends.svg", pageHref: "/friends" },
  { text: "Messages", imgSrc: "/messages.svg", pageHref: "/messages" },
  { text: "My posts", imgSrc: "/posts.svg", pageHref: "/my-posts" },
];

const Layout = () => {
  const { user, logout, isLoading } = useAuth();
  const navigate = useNavigate();
  
  const [searchQuery, setSearchQuery] = useState("");
  const [searchResults, setSearchResults] = useState([]);
    
  // Handle logout with local navigation
  const handleLogout = async () => {
    try {
      await logout();
      navigate('/login', { replace: true });
    } catch (error) {
      console.error('Logout error:', error);
    }
  };

  const handleSearch = async () => {
    if (!searchQuery.trim()) return;

    try {
      const response = await axios.get(`https://localhost:7126/api/profile/search-users?query=${searchQuery}`, {
        withCredentials: true
      });

      if (response.data.message === "No users founded") {
        setSearchResults("No users found.");
      } else {
        setSearchResults(response.data.message.$values || []);
      }
    } catch (error) {
      console.error('Search error:', error);
      setSearchResults([]);
    }
  };

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <Box sx={{ display: "flex", height: "100vh", width: "100vw", overflow: "hidden" }}>
      <Drawer
        variant="permanent"
        sx={{
          flexShrink: 0,
          "& .MuiDrawer-paper": {
            width: drawerWidth,
            boxSizing: "border-box",
            backgroundColor: "white",
            color: "black",
          },
        }}
      >
        <Box sx={{ padding: 2 }}>
          <Typography
            variant="h6"
            sx={{
              display: "flex",
              alignItems: "center",
              fontSize: "60px",
              fontWeight: "light",
              fontFamily: "roboto",
              color: "#365B87",
            }}
          >
            Sigm
            <img src="/sigma_2.svg" alt="Sigma Logo" style={{ height: "60px", marginLeft: "2px" }} />
          </Typography>
        </Box>
        <ProfileCard profile={user} />
        <List>
          {menuItems.map(({ text, imgSrc, pageHref }) => (
            <ListItem key={text} sx={{ cursor: "pointer", paddingY: 1 }}>
              <Link to={pageHref} style={{ textDecoration: 'none', color: 'inherit' }}>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                  <ListItemIcon sx={{ minWidth: 30 }}>
                    <img src={imgSrc} alt={text} style={{ width: 24, height: 24 }} />
                  </ListItemIcon>
                  <ListItemText
                    primary={text}
                    sx={{
                      marginLeft: 1,
                      fontSize: "16px",
                      fontWeight: 500,
                      color: "#2B2B2B",
                    }}
                  />
                </Box>
              </Link>
            </ListItem>
          ))}
        </List>
        <Box sx={{ padding: 2 }}>
          <Link to={"create-post"}>
            <Button variant="contained" color="primary" fullWidth sx={{ borderRadius: "40px", backgroundColor: "#365B87" }}>
              Post
            </Button>
          </Link>
          <Button
            variant="contained"
            color="primary"
            fullWidth
            sx={{ borderRadius: "40px", backgroundColor: "#365B87", marginTop: "10px" }}
            onClick={handleLogout}
          >
            Log out
          </Button>
        </Box>
      </Drawer>

      <Box sx={{ flex: 1, display: "flex", flexDirection: "column", height: "100vh", marginLeft: `${drawerWidth}px` }}>
        <AppBar position="fixed" sx={{ backgroundColor: "#2b2b2b", boxShadow: "none", zIndex: 1100 }}>
          <Toolbar>
            <Typography variant="h6" sx={{ flexGrow: 1, color: "white" }}>
              Dashboard
            </Typography>
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                backgroundColor: "rgba(255, 255, 255, 0.1)",
                padding: "6px 12px",
                borderRadius: "24px",
              }}
            >
              <IconButton onClick={handleSearch}>
                <SearchIcon style={{ color: "white" }} />
              </IconButton>
              <InputBase
                placeholder="Search"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                sx={{ marginLeft: 1, color: "white" }}
              />
            </Box>
          </Toolbar>
        </AppBar>

        <Box sx={{ flex: 1, backgroundColor: "#365B87", marginTop: "63px", padding: 2 }}>
          <Outlet />
          <SearchResults results={searchResults} />
        </Box>
      </Box>
    </Box>
  );
};

export default Layout;