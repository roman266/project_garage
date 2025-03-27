import React, { useEffect, useState } from "react";
import { Box, Typography, Avatar, Container, Button } from "@mui/material";
import axios from "axios";
import { useParams } from "react-router-dom";

export default function UserProfilePage() {
    const [profile, setProfile] = useState({
        userName: "",
        firstName: "",
        lastName: "",
        description: "",
        profilePicture: "",
        postsCount: 0,
        friendsCount: 0,
        reactionsCount: 0,
    });

    const [friendRequestSent, setFriendRequestSent] = useState(false);
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

    const handleAddFriend = async () => {
        try {
            await axios.post(`${API_BASE_URL}/api/friends/send/${userId}`, {}, { withCredentials: true });
            setFriendRequestSent(true);
        } catch (error) {
            console.error("Error sending friend request", error);
        }
    };

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

                <Box sx={{ display: "flex", justifyContent: "space-between", maxWidth: "60%", width: "100%", mt: 2, mx: "auto" }}>
                    <Box sx={{ textAlign: "center", flex: 1 }}>
                        <Typography variant="body2" sx={{ fontWeight: "bold" }}>
                            {profile.postsCount}
                        </Typography>
                        <Typography variant="caption" sx={{ color: "#777" }}>
                            Posts
                        </Typography>
                    </Box>
                    <Box sx={{ textAlign: "center", flex: 1 }}>
                        <Typography variant="body2" sx={{ fontWeight: "bold" }}>
                            {profile.friendsCount}
                        </Typography>
                        <Typography variant="caption" sx={{ color: "#777" }}>
                            Friends
                        </Typography>
                    </Box>
                    <Box sx={{ textAlign: "center", flex: 1 }}>
                        <Typography variant="body2" sx={{ fontWeight: "bold" }}>
                            {profile.reactionsCount}
                        </Typography>
                        <Typography variant="caption" sx={{ color: "#777" }}>
                            Reactions
                        </Typography>
                    </Box>
                </Box>

                <Typography sx={{ fontStyle: "italic", color: "#777", mt: 3 }}>
                    {profile.description || "No description provided"}
                </Typography>

                {/* Кнопки */}
                <Box sx={{ display: "flex", justifyContent: "space-between", mt: 3 }}>
                    <Button
                        variant="contained"
                        sx={{
                            flex: 1,
                            mx: 1,
                            backgroundColor: friendRequestSent ? "#aaa" : "#365B87",
                            "&:hover": { backgroundColor: friendRequestSent ? "#aaa" : "#2C4A6E" },
                            padding: "6px 12px",
                            minWidth: "auto"
                        }}
                        onClick={handleAddFriend}
                        disabled={friendRequestSent}
                    >
                        {friendRequestSent ? "Request Sent" : "Add to Friends"}
                    </Button>
                    <Button
                        variant="outlined"
                        sx={{
                            flex: 1,
                            mx: 1,
                            borderColor: "#365B87",
                            color: "#365B87",
                            "&:hover": { borderColor: "#2C4A6E", color: "#2C4A6E" },
                            padding: "6px 12px",
                            minWidth: "auto"
                        }}
                    >
                        Posts
                    </Button>
                    <Button
                        variant="contained"
                        sx={{
                            flex: 1,
                            mx: 1,
                            backgroundColor: "#365B87",
                            "&:hover": { backgroundColor: "#388E3C" },
                            padding: "6px 12px",
                            minWidth: "auto"
                        }}
                    >
                        Message
                    </Button>
                </Box>
            </Container>
        </Box>
    );
}
