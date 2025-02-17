import React from "react";
import { Outlet } from "react-router-dom";
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

const drawerWidth = 240;

const menuItems = [
  { text: "My profile", imgSrc: "/profile.svg" },
  { text: "Friends", imgSrc: "/friends.svg" },
  { text: "Messages", imgSrc: "/messages.svg" },
  { text: "My posts", imgSrc: "/posts.svg" },
];

const Layout = () => {
  return (
    <Box sx={{ display: "flex", height: "100vh", width: "100vw", overflow: "hidden" }}>
      {/* Sidebar */}
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
        <div style={{ backgroundColor: "#DDDDDD", width: "75%", height: "1px", margin: "0 auto" }} />
        <ProfileCard />
        <List>
          {menuItems.map(({ text, imgSrc }) => (
            <ListItem button key={text} sx={{ cursor: "pointer" }}>
              <ListItemIcon>
                <img src={imgSrc} alt={text} style={{ width: 24, height: 24 }} />
              </ListItemIcon>
              <ListItemText primary={text} sx={{ margin: "0" }} />
            </ListItem>
          ))}
        </List>
        <Box sx={{ padding: 2 }}>
          <Button variant="contained" color="primary" fullWidth sx={{ borderRadius: "40px", backgroundColor: "#365B87" }}>
            Post
          </Button>
        </Box>
      </Drawer>

      {/* Основная часть */}
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
        <Box sx={{ flex: 1, backgroundColor: "#365B87", padding: "20px", marginTop: "64px" }}>
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
};

export default Layout;
