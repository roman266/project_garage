import React, { useEffect, useState } from "react";
import { Box, Typography, Button, Avatar, Container } from "@mui/material";
import axios from "axios";

export default function MyProfilePage() {
  const [profile, setProfile] = useState({
    userName: "",
    email: "",
    profilePicture: "",
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
          >
            Edit
          </Button>
        </Box>
      </Container>
    </Box>
  );
}