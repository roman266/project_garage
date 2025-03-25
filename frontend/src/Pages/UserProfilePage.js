import React, { useEffect, useState } from "react";
import { Box, Typography, Button, Avatar, Container, TextField, Dialog, DialogActions, DialogContent, DialogTitle, Alert, Snackbar } from "@mui/material";
import axios from "axios";
import { useParams } from "react-router-dom";

export default function UserProfilePage() {
  const [profile, setProfile] = useState({
    userName: "",
    email: "",
    firstName: "",
    lastName: "",
    description: "",
    profilePicture: "",
  });

  const [open, setOpen] = useState(false);
  const API_BASE_URL = process.env.REACT_APP_HTTPS_API_URL;
  const { userId } = useParams();
  const [loading, setLoading] = useState(true);
  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}/api/profile/${userId}`, {
			withCredentials: true,
		});

        setProfile(response.data.profile);
      } catch (error) {
        console.error("Error fetching profile data", error);
      }
    };

    fetchProfile();
  }, []);

  const [alertInfo, setAlertInfo] = useState({
    open: false,
    message: "",
    severity: "success" // 'success', 'error', 'warning', 'info'
  });
  
  const handleClose = () => {
    setOpen(false);
  };

  const handleAlertClose = () => {
    setAlertInfo({ ...alertInfo, open: false });
  };

  return (
    <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", minHeight: "70vh", backgroundColor: "#365B87" }}>
      <Container maxWidth="sm" sx={{ backgroundColor: "white", padding: "40px", borderRadius: "12px", boxShadow: "2px 2px 15px rgba(0,0,0,0.3)", textAlign: "center" }}>
        <Typography variant="h5" sx={{ color: "#365B87", textAlign: "left", marginBottom: 2 }}>My profile</Typography>
        <Avatar 
          src={profile.profilePicture} 
          sx={{ 
            width: 80, 
            height: 80, 
            margin: "20px auto", 
            backgroundColor: "black",
            cursor: "pointer",
            '&:hover': {
              opacity: 0.8,
              boxShadow: '0 0 10px rgba(0,0,0,0.3)'
            }
          }} 
        />
        
        <Box sx={{ textAlign: "left" }}>
          <Typography sx={{ fontWeight: "bold", color: "#365B87" }}>Username</Typography>
          <Typography sx={{ marginBottom: 2, borderBottom: "1px solid #ccc", display: "inline-block", width: "100%" }}>{profile.userName}</Typography>
          
          <Typography sx={{ fontWeight: "bold", color: "#365B87" }}>First Name</Typography>
          <Typography sx={{ marginBottom: 2, borderBottom: "1px solid #ccc", display: "inline-block", width: "100%" }}>{profile.firstName}</Typography>
          
          <Typography sx={{ fontWeight: "bold", color: "#365B87" }}>Last Name</Typography>
          <Typography sx={{ marginBottom: 2, borderBottom: "1px solid #ccc", display: "inline-block", width: "100%" }}>{profile.lastName}</Typography>
          
          <Typography sx={{ fontWeight: "bold", color: "#365B87" }}>Description</Typography>
          <Typography sx={{ marginBottom: 2, borderBottom: "1px solid #ccc", display: "inline-block", width: "100%" }}>{profile.description}</Typography>
          
          <Typography sx={{ fontWeight: "bold", color: "#365B87" }}>Email</Typography>
          <Typography sx={{ marginBottom: 2, borderBottom: "1px solid #ccc", display: "inline-block", width: "100%" }}>{profile.email}</Typography>
        </Box>
      </Container>
      
      {/* Add Snackbar with Alert */}
      <Snackbar 
        open={alertInfo.open} 
        autoHideDuration={6000} 
        onClose={handleAlertClose}
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
      >
        <Alert 
          onClose={handleAlertClose} 
          severity={alertInfo.severity} 
          sx={{ width: '100%' }}
        >
          {alertInfo.message}
        </Alert>
      </Snackbar>
	  
      {/* Залишаємо тільки один Snackbar */}
      <Snackbar 
        open={alertInfo.open} 
        autoHideDuration={6000} 
        onClose={handleAlertClose}
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
      >
        <Alert 
          onClose={handleAlertClose} 
          severity={alertInfo.severity} 
          sx={{ width: '100%' }}
        >
          {alertInfo.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}
