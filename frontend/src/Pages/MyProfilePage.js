import React, { useEffect, useState } from "react";
import { Box, Typography, Button, Avatar, Container, TextField, Dialog, DialogActions, DialogContent, DialogTitle } from "@mui/material";
import axios from "axios";

export default function MyProfilePage() {
  const [profile, setProfile] = useState({
    userName: "",
    email: "",
    profilePicture: "",
  });

  const [open, setOpen] = useState(false);
  const [formData, setFormData] = useState({
    userName: "",
    email: "",
    password: "",
  });

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const token = localStorage.getItem("token"); // Получение токена из localStorage
        console.log("Token:", token); // Логирование токена для отладки
        if (!token) {
          throw new Error("No token found");
        }

        const response = await axios.get("http://localhost:5021/api/profile/me", {
          headers: {
            Authorization: `Bearer ${token}`, // Добавление токена в заголовок запроса
          },
        });

        console.log("Profile data:", response.data.profile); // Логирование данных профиля
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
      const token = localStorage.getItem("token"); // Получение токена из localStorage
      if (!token) {
        throw new Error("No token found");
      }

      await axios.post("http://localhost:5021/api/profile/me/edit", formData, {
        headers: {
          Authorization: `Bearer ${token}`, // Добавление токена в заголовок запроса
        },
      });

      setProfile((prevProfile) => ({
        ...prevProfile,
        userName: formData.userName,
        email: formData.email,
      }));

      handleClose();
    } catch (error) {
      console.error("Error saving profile data", error);
    }
  };

  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        minHeight: "70vh",
        backgroundColor: "#365B87",
      }}
    >
      <Container
        maxWidth="sm"
        sx={{
          backgroundColor: "white",
          padding: "40px",
          borderRadius: "12px",
          boxShadow: "2px 2px 15px rgba(0,0,0,0.3)",
          textAlign: "center",
        }}
      >
        <Typography
          variant="h5"
          sx={{ fontFamily: "Roboto, sans-serif", color: "#365B87", textAlign: "left", marginBottom: 2 }}
        >
          My profile
        </Typography>
        <Avatar
          src={profile.profilePicture}
          sx={{
            width: 80,
            height: 80,
            margin: "20px auto",
            backgroundColor: "black",
          }}
        />
        <Box sx={{ textAlign: "left" }}>
          <Typography sx={{ fontWeight: "bold", color: "#365B87" }}>Username</Typography>
          <Typography sx={{ marginBottom: 2, borderBottom: "1px solid #ccc", display: "inline-block", width: "100%" }}>{profile.userName}</Typography>
          
          <Typography sx={{ fontWeight: "bold", color: "#365B87" }}>Email</Typography>
          <Typography sx={{ marginBottom: 2, borderBottom: "1px solid #ccc", display: "inline-block", width: "100%" }}>{profile.email}</Typography>
          
          <Typography sx={{ fontWeight: "bold", color: "#365B87" }}>Password</Typography>
          <Typography sx={{ marginBottom: 2 }}>**********</Typography>
        </Box>
        <Box sx={{ display: "flex", justifyContent: "flex-end" }}>
          <Button
            variant="contained"
            sx={{ backgroundColor: "#1F4A7C", color: "white", marginTop: 2, width: "100px", height: "26px" }}
            onClick={handleClickOpen}
          >
            Edit
          </Button>
        </Box>
      </Container>

      <Dialog open={open} onClose={handleClose}>
        <DialogTitle>Edit Profile</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            name="userName"
            label="Username"
            type="text"
            fullWidth
            value={formData.userName}
            onChange={handleChange}
          />
          <TextField
            margin="dense"
            name="email"
            label="Email Address"
            type="email"
            fullWidth
            value={formData.email}
            onChange={handleChange}
          />
          <TextField
            margin="dense"
            name="password"
            label="Password"
            type="password"
            fullWidth
            value={formData.password}
            onChange={handleChange}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} color="primary">
            Cancel
          </Button>
          <Button onClick={handleSave} color="primary">
            Save
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}