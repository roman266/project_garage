import React, { useEffect, useState } from "react";
import {
  Box,
  Typography,
  Avatar,
  Container,
  Button,
  Grid,
  Card,
  CardMedia,
  IconButton,
  CircularProgress,
} from "@mui/material";
import axios from "axios";
import { useParams } from "react-router-dom";
import { useNavigate } from "react-router-dom";


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
    activeStatus: "Offline",
  });
  const [friendStatus, setFriendStatus] = useState("Add to Friends");
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
    const [hasMore, setHasMore] = useState(true);

    const navigate = useNavigate();


  const { userId } = useParams();
  const API_BASE_URL = process.env.REACT_APP_HTTPS_API_URL;

  useEffect(() => {
    fetchProfile();
    fetchFriendStatus();
    fetchPosts();
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
    
    const handleMessage = async () => {
        try {

            const response = await axios.get(`${API_BASE_URL}/api/conversations/my-conversations`, {
                withCredentials: true,
            });

            const conversations = response.data?.$values || [];

            const existingConversation = conversations.find(
                (conv) => conv.isPrivate && conv.userName === profile.userName
            );

            if (existingConversation) {

                navigate(`/messages/${existingConversation.conversationId}`);
            } else {

                await axios.post(`${API_BASE_URL}/api/conversations/start/${userId}`, {}, { withCredentials: true });

                // Після створення — знову отримаємо всі переписки
                const updatedResponse = await axios.get(`${API_BASE_URL}/api/conversations/my-conversations`, {
                    withCredentials: true,
                });

                const updatedConversations = updatedResponse.data?.$values || [];

                const newConversation = updatedConversations.find(
                    (conv) => conv.isPrivate && conv.userName === profile.userName
                );

                if (newConversation) {
                    navigate(`/messages/${newConversation.conversationId}`);
                } else {
                    console.error("Failed to find new conversation after creation.");
                }
            }
        } catch (error) {
            console.error("Error handling message button:", error);
        }
    };


  const fetchFriendStatus = async () => {
    try {
      const [outcomingRes, incomingRes, friendsRes] = await Promise.all([
        axios.get(`${API_BASE_URL}/api/friends/my-requests/outcoming`, { withCredentials: true }),
        axios.get(`${API_BASE_URL}/api/friends/my-requests/incoming`, { withCredentials: true }),
        axios.get(`${API_BASE_URL}/api/friends/my-requests/friends`, { withCredentials: true }),
      ]);

      const outcoming = outcomingRes.data.$values;
      const incoming = incomingRes.data.$values;
      const friends = friendsRes.data.$values;

      const isRequestSent = outcoming.some(req => req.friendId === userId || req.userId === userId);
      const isRequestReceived = incoming.some(req => req.friendId === userId || req.userId === userId);
      const isFriend = friends.some(f => f.friendId === userId || f.userId === userId);

      if (isFriend) setFriendStatus("Already Friend");
      else if (isRequestSent || isRequestReceived) setFriendStatus("Request Sent");
      else setFriendStatus("Add to Friends");
    } catch (error) {
      console.error("Error fetching friend status", error);
    }
  };

  const fetchPosts = async (lastPostId = null) => {
    try {
      setLoading(true);
      const response = await axios.get(`${API_BASE_URL}/api/post/user-posts`, {
        params: {
          userId,
          lastPostId,
          limit: 15,
        },
        withCredentials: true,
      });

      const postsData = response.data?.$values || [];
      if (postsData.length < 15) setHasMore(false);

      setPosts(prev => lastPostId ? [...prev, ...postsData] : postsData);
    } catch (error) {
      console.error("Error fetching posts:", error);
      setPosts([]);
    } finally {
      setLoading(false);
    }
  };

  const loadMorePosts = () => {
    if (posts.length > 0) {
      const lastPostId = posts[posts.length - 1].postId;
      fetchPosts(lastPostId);
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

  return (
    <Box sx={{ display: "flex", justifyContent: "center", minHeight: "100vh", backgroundColor: "#365B87", py: 4 }}>
      <Container maxWidth="sm" sx={{ backgroundColor: "white", borderRadius: "12px", boxShadow: 3, p: 4, textAlign: "center" }}>
        <Avatar src={profile.profilePicture} sx={{ width: 100, height: 100, mx: "auto", mb: 2, bgcolor: "black" }} />
        <Typography
          variant="body2"
          sx={{
            color: profile.activeStatus === "Online" ? "green" : "gray",
            fontWeight: "bold",
            mt: -4,
			ml: 13,
          }}
        >
          {profile.activeStatus}
        </Typography>
        <Typography variant="h5" sx={{ color: "#365B87", fontWeight: "bold" }} gutterBottom>
          {profile.firstName} {profile.lastName}
        </Typography>
        <Typography sx={{ color: "#555", mb: 2 }}>@{profile.userName}</Typography>

        <Box sx={{ display: "flex", justifyContent: "space-between", mb: 2 }}>
          <Box sx={{ textAlign: "center", flex: 1 }}>
            <Typography variant="body2" sx={{ fontWeight: "bold" }}>{profile.postsCount}</Typography>
            <Typography variant="caption" sx={{ color: "#777" }}>Posts</Typography>
          </Box>
          <Box sx={{ textAlign: "center", flex: 1 }}>
            <Typography variant="body2" sx={{ fontWeight: "bold" }}>{profile.friendsCount}</Typography>
            <Typography variant="caption" sx={{ color: "#777" }}>Friends</Typography>
          </Box>
          <Box sx={{ textAlign: "center", flex: 1 }}>
            <Typography variant="body2" sx={{ fontWeight: "bold" }}>{profile.reactionsCount}</Typography>
            <Typography variant="caption" sx={{ color: "#777" }}>Reactions</Typography>
          </Box>
        </Box>

        <Typography sx={{ fontStyle: "italic", color: "#777", mb: 3 }}>
          {profile.description || "No description provided"}
        </Typography>

        <Box sx={{ display: "flex", justifyContent: "space-between", mb: 4 }}>
          <Button
            variant="contained"
            sx={{
              flex: 1,
              mx: 1,
              bgcolor: friendStatus === "Already Friend" ? "#388E3C" : "#365B87",
              "&:hover": { bgcolor: "#2C4A6E" },
            }}
            onClick={friendStatus === "Add to Friends" ? handleAddFriend : null}
            disabled={friendStatus !== "Add to Friends"}
          >
            {friendStatus}
          </Button>
                  <Button
                      variant="contained"
                      sx={{
                          flex: 1,
                          mx: 1,
                          bgcolor: "#365B87",
                          "&:hover": { bgcolor: "#388E3C" }
                      }}
                      onClick={handleMessage}
                  >
            Message
          </Button>
        </Box>

        {/* POSTS */}
        <Grid container spacing={3} sx={{ textAlign: "left" }}>
          {loading && posts.length === 0 ? (
            <Box display="flex" justifyContent="center" width="100%" my={3}>
              <CircularProgress />
            </Box>
          ) : (
            posts.map(post => (
              <Grid item xs={12} key={post.postId}>
                <Card>
                  <Box sx={{ p: 2 }}>
                    <Box sx={{ display: "flex", alignItems: "center", mb: 1 }}>
                      <Avatar
                        src={post.senderAvatarUlr}
                        alt={post.senderNickName}
                        sx={{ width: 40, height: 40, mr: 2 }}
                      />
                      <Box>
                        <Typography variant="subtitle2">{post.senderNickName}</Typography>
                        <Typography variant="caption" color="text.secondary">
                          {new Date(post.postDate).toLocaleString()}
                        </Typography>
                      </Box>
                    </Box>
                    {post.postImageUrl && (
                      <Box
                        sx={{
                          width: "100%",
                          height: { xs: 300, md: 400 },
                          backgroundColor: "black",
                          display: "flex",
                          justifyContent: "center",
                          alignItems: "center",
                          mb: 2,
                        }}
                      >
                        <CardMedia
                          component="img"
                          sx={{ maxWidth: "100%", maxHeight: "100%", objectFit: "contain" }}
                          image={post.postImageUrl}
                          alt={post.postDescription}
                        />
                      </Box>
                    )}
                    <Typography variant="body2">{post.postDescription}</Typography>
                  </Box>
                </Card>
              </Grid>
            ))
          )}
        </Grid>

        {hasMore && !loading && (
          <Box display="flex" justifyContent="center" mt={3}>
            <Button variant="outlined" onClick={loadMorePosts}>
              Load More
            </Button>
          </Box>
        )}
      </Container>
    </Box>
  );
}