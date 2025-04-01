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

    const [friendStatus, setFriendStatus] = useState("loading"); // "Add to Friends", "Request Sent", "Friend"
    const { userId } = useParams();
    const API_BASE_URL = process.env.REACT_APP_HTTPS_API_URL;

    useEffect(() => {
        fetchProfile();
        checkFriendStatus();
    }, [userId]);

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

    const checkFriendStatus = async () => {
        try {
            const [friendsRes, incomingRes, outgoingRes] = await Promise.all([
                axios.get(`${API_BASE_URL}/api/friends/my-requests/friends?lastfriendId=${userId}`, { withCredentials: true }),
                axios.get(`${API_BASE_URL}/api/friends/my-requests/incoming`, { withCredentials: true }),
                axios.get(`${API_BASE_URL}/api/friends/my-requests/outcoming`, { withCredentials: true }),
            ]);

            const friends = friendsRes.data || [];
            const incomingRequests = incomingRes.data || [];
            const outgoingRequests = outgoingRes.data || [];

            if (friends.some(f => f.FriendId === userId || f.UserId === userId)) {
                setFriendStatus("Remove Friend");
            } else if (outgoingRequests.some(r => r.FriendId === userId)) {
                setFriendStatus("Request Sent");
            } else if (incomingRequests.some(r => r.UserId === userId)) {
                setFriendStatus("Accept Request");
            } else {
                setFriendStatus("Add to Friends");
            }
        } catch (error) {
            console.error("Error checking friend status", error);
            setFriendStatus("Add to Friends");
        }
    };

    const handleRemoveFriend = async () => {
        try {
            await axios.delete(`${API_BASE_URL}/api/friends/reject/${userId}`, { withCredentials: true });
            setFriendStatus("Add to Friends");
        } catch (error) {
            console.error("Error removing friend", error);
        }
    };



    const handleAddFriend = async () => {
        try {
            await axios.post(`${API_BASE_URL}/api/friends/send/${userId}`, {}, { withCredentials: true });
            setFriendStatus("Request Sent");
        } catch (error) {
            console.error("Error sending friend request", error);
        }
    };

    const handleAcceptFriend = async () => {
        try {
            const incomingRequests = await axios.get(`${API_BASE_URL}/api/friends/my-requests/incoming`, { withCredentials: true });
            const request = incomingRequests.data.find(r => r.UserId === userId);
            if (request) {
                await axios.post(`${API_BASE_URL}/api/friends/accept/${request.Id}`, {}, { withCredentials: true });
                setFriendStatus("Friend");
            }
        } catch (error) {
            console.error("Error accepting friend request", error);
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
                    {friendStatus === "Add to Friends" && (
                        <Button variant="contained" sx={{ flex: 1, mx: 1, backgroundColor: "#365B87", color: "white" }} onClick={handleAddFriend}>
                            Add to Friends
                        </Button>
                    )}
                    {friendStatus === "Request Sent" && (
                        <Button variant="outlined" sx={{ flex: 1, mx: 1, color: "#777" }} disabled>
                            Request Sent
                        </Button>
                    )}
                    {friendStatus === "Accept Request" && (
                        <Button variant="contained" sx={{ flex: 1, mx: 1, backgroundColor: "#4CAF50", color: "white" }} onClick={handleAcceptFriend}>
                            Accept Request
                        </Button>
                    )}
                    {friendStatus === "Remove Friend" && (
                        <Button variant="contained" sx={{ flex: 1, mx: 1, backgroundColor: "#d32f2f", color: "white" }} onClick={handleRemoveFriend}>
                            Remove Friend
                        </Button>
                    )}

                    <Button variant="outlined" sx={{ flex: 1, mx: 1, borderColor: "#365B87", color: "#365B87" }}>
                        Posts
                    </Button>
                    <Button variant="contained" sx={{ flex: 1, mx: 1, backgroundColor: "#365B87" }}>
                        Message
                    </Button>
                </Box>
            </Container>
        </Box>
    );
}
