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
    const [friendStatus, setFriendStatus] = useState("Add to Friends");

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

        const fetchFriendStatus = async () => {
            try {
                const [outcomingRes, incomingRes, friendsRes] = await Promise.all([
                    axios.get(`${API_BASE_URL}/api/friends/my-requests/outcoming`, { withCredentials: true }),
                    axios.get(`${API_BASE_URL}/api/friends/my-requests/incoming`, { withCredentials: true }),
                    axios.get(`${API_BASE_URL}/api/friends/my-requests/friends`, { withCredentials: true })
                ]);

                const isRequestSent = outcomingRes.data.some(request => request.friendId === userId);
                const isRequestReceived = incomingRes.data.some(request => request.userId === userId);
                const isFriend = friendsRes.data.some(friend => friend.friendId === userId);

                if (isFriend) {
                    setFriendStatus("Already Friend");
                } else if (isRequestSent || isRequestReceived) {
                    setFriendStatus("Request Sent");
                } else {
                    setFriendStatus("Add to Friends");
                }
            } catch (error) {
                console.error("Error fetching friend status", error);
            }
        };

        fetchProfile();
        fetchFriendStatus();
    }, [userId, API_BASE_URL]);

    const handleAddFriend = async () => {
        try {
            await axios.post(`${API_BASE_URL}/api/friends/send/${userId}`, {}, { withCredentials: true });
            setFriendStatus("Request Sent");
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

                <Box sx={{ display: "flex", justifyContent: "space-between", mt: 3 }}>
                    <Button
                        variant="contained"
                        sx={{
                            flex: 1,
                            mx: 1,
                            backgroundColor: friendStatus === "Already Friend" ? "#388E3C" : "#365B87",
                            "&:hover": { backgroundColor: friendStatus === "Already Friend" ? "#2C4A6E" : "#2C4A6E" },
                            padding: "6px 12px",
                            minWidth: "auto"
                        }}
                        onClick={friendStatus === "Add to Friends" ? handleAddFriend : null}
                        disabled={friendStatus !== "Add to Friends"}
                    >
                        {friendStatus}
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