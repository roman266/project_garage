import React, { useEffect, useState } from "react";
import { Box, Typography, Button, Avatar, Container, TextField, Dialog, DialogActions, DialogContent, DialogTitle } from "@mui/material";
import axios from "axios";

export default function MyProfilePage() {
  const [profile, setProfile] = useState({
    userName: "",
    email: "",
    firstName: "",
    lastName: "",
    description: "",
    profilePicture: "",
  });
	
  const [open, setOpen] = useState(false);
  const [formData, setFormData] = useState({
    userName: "",
    email: "",
    firstName: "",
    lastName: "",
    description: "",
    password: "",
  });
  const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;
  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const token = localStorage.getItem("token");
        if (!token) throw new Error("No token found");

        const response = await axios.get(`${API_BASE_URL}/api/profile/me`, {
			headers: { Authorization: `Bearer ${token}` },
		});

        setProfile(response.data.profile);
      } catch (error) {
        console.error("Error fetching profile data", error);
      }
    };

    fetchProfile();
  }, []);

  const handleClickOpen = () => {
    setFormData({
      userName: profile.userName,
      email: profile.email,
      firstName: profile.firstName,
      lastName: profile.lastName,
      description: profile.description,
      password: "",
    });
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSave = async () => {
    try {
      const token = localStorage.getItem("token");
      if (!token) throw new Error("No token found");

      await axios.post(`${API_BASE_URL}/api/profile/me/edit`, formData, {
		headers: { Authorization: `Bearer ${token}` },
	  });

      setProfile((prevProfile) => ({ ...prevProfile, ...formData }));
      handleClose();
    } catch (error) {
      console.error("Error saving profile data", error);
    }
  };

  return (
    <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", minHeight: "70vh", backgroundColor: "#365B87" }}>
      <Container maxWidth="sm" sx={{ backgroundColor: "white", padding: "40px", borderRadius: "12px", boxShadow: "2px 2px 15px rgba(0,0,0,0.3)", textAlign: "center" }}>
        <Typography variant="h5" sx={{ color: "#365B87", textAlign: "left", marginBottom: 2 }}>My profile</Typography>
        <Avatar src={profile.profilePicture} sx={{ width: 80, height: 80, margin: "20px auto", backgroundColor: "black" }} />
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
        <Box sx={{ display: "flex", justifyContent: "flex-end" }}>
          <Button variant="contained" sx={{ backgroundColor: "#1F4A7C", color: "white", marginTop: 2 }} onClick={handleClickOpen}>Edit</Button>
        </Box>
      </Container>

      <Dialog open={open} onClose={handleClose}>
        <DialogTitle>Edit Profile</DialogTitle>
        <DialogContent>
          <TextField margin="dense" name="userName" label="Username" fullWidth value={formData.userName} onChange={handleChange} />
          <TextField margin="dense" name="firstName" label="First Name" fullWidth value={formData.firstName} onChange={handleChange} />
          <TextField margin="dense" name="lastName" label="Last Name" fullWidth value={formData.lastName} onChange={handleChange} />
          <TextField margin="dense" name="description" label="Description" fullWidth value={formData.description} onChange={handleChange} />
          <TextField margin="dense" name="email" label="Email" fullWidth value={formData.email} onChange={handleChange} />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} color="primary">Cancel</Button>
          <Button onClick={handleSave} color="primary">Save</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
