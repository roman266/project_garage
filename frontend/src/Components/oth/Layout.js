import React from "react";
import { Outlet, Link, useNavigate } from "react-router-dom";
import ProfileCard from "./ProfileCard";
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
  ListItemIcon
} from "@mui/material";
import { useAuth } from "../../context/AuthContext";

const drawerWidth = 240;

const menuItems = [
  { text: "My profile", imgSrc: "/profile.svg", pageHref: "/my-profile" },
  { text: "Friends", imgSrc: "/friends.svg", pageHref: "/friends" },
  { text: "Messages", imgSrc: "/messages.svg", pageHref: "/messages" },
  { text: "My posts", imgSrc: "/posts.svg", pageHref: "/my-posts" },
];

const Layout = () => {
  const { user, logout, isLoading } = useAuth();
  const navigate = useNavigate();
  
  // Handle logout with local navigation
  const handleLogout = async () => {
    try {
      await logout();
      navigate('/login', { replace: true });
    } catch (error) {
      console.error('Logout error:', error);
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
        {/* Header */}
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
              <img src="/search_icon.svg" alt="Search" style={{ width: 20, height: 20, opacity: 0.6 }} />
              <InputBase placeholder="Search" sx={{ marginLeft: 1, color: "white" }} />
            </Box>
          </Toolbar>
        </AppBar>

        {/* Main Content */}
        <Box 
          sx={{ 
            flex: 1, 
            backgroundColor: "#365B87", 
            marginTop: "63px", 
            overflowY: "auto" 
          }}
        >
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
};

export default Layout;
