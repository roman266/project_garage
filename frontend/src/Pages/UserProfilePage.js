import React, { useEffect, useState } from "react";
import { Box, Typography, Avatar, Container, Snackbar, Alert } from "@mui/material";
import axios from "axios";
import { useParams } from "react-router-dom";

export default function UserProfilePage() {
  const [profile, setProfile] = useState({
    userName: "",
    firstName: "",
    lastName: "",
    description: "",
    profilePicture: "",
  });

  const { userId } = useParams();
  const API_BASE_URL = process.env.REACT_APP_HTTPS_API_URL;

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
  }, [userId, API_BASE_URL]);

  return (
    <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", minHeight: "70vh", backgroundColor: "#365B87" }}>
      <Container maxWidth="sm" sx={{ backgroundColor: "white", padding: "40px", borderRadius: "12px", boxShadow: "2px 2px 15px rgba(0,0,0,0.3)", textAlign: "center" }}>
        <Avatar 
          src={profile.profilePicture} 
          sx={{ width: 100, height: 100, margin: "20px auto", backgroundColor: "black" }}
        />
        <Typography variant="h5" sx={{ color: "#365B87", fontWeight: "bold" }}>
          {profile.firstName} {profile.lastName}
        </Typography>
        <Typography sx={{ color: "#555", marginBottom: 2 }}>
          @{profile.userName}
        </Typography>
        <Typography sx={{ fontStyle: "italic", color: "#777" }}>
          {profile.description || "No description provided"}
        </Typography>
      </Container>
    </Box>
  );
}
