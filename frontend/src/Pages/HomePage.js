import React, { useEffect, useState } from "react";
import { Box, Typography, Card, CardContent, IconButton, Avatar, Pagination } from "@mui/material";
import { ThumbUp, ThumbUpOutlined, Comment, Share } from "@mui/icons-material";
import API, { addReaction, deleteReaction, getEntityReactions } from "../utils/apiClient";

const HomePage = () => {
  const [posts, setPosts] = useState([]);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [userReactions, setUserReactions] = useState({}); // Зберігатиме реакції користувача
  const postsPerPage = 10;

  // Функція для отримання реакцій поста
  const fetchReactions = async (postId) => {
    try {
      const response = await getEntityReactions(postId);
      if (response.success && response.data) {
        const reactions = response.data.$values || response.data;
        const userReaction = reactions.find((r) => r.userId === "current_user_id"); // Замініть на реальний userId
        setUserReactions((prev) => ({
          ...prev,
          [postId]: userReaction || null,
        }));
        return reactions.length;
      }
      return 0;
    } catch (err) {
      console.error(`Error fetching reactions for post ${postId}:`, err);
      return 0;
    }
  };

  useEffect(() => {
    const fetchFeed = async () => {
      try {
        const response = await API.get(`/home/feed?page=${page}&limit=${postsPerPage}`);
        const data = response.data;

        if (!data.success) {
          setError(`Failed to fetch posts: ${data.message}`);
          return;
        }

        if (!data.data || !data.data.posts) {
          setError("Invalid response structure: posts not found");
          return;
        }

        const fetchedPosts = data.data.posts?.$values || [];
        if (!Array.isArray(fetchedPosts)) {
          setError("Invalid post data format");
          return;
        }

        // Отримати кількість реакцій для кожного поста
        const postsWithReactions = await Promise.all(
          fetchedPosts.map(async (post) => {
            const reactionCount = await fetchReactions(post.id);
            return { ...post, likes: reactionCount };
          })
        );

        setPosts(postsWithReactions);
        setTotalPages(data.data.totalPages || 1);
      } catch (error) {
        setError(`Error fetching feed: ${error.response?.data?.message || error.message}`);
      }
    };

    fetchFeed();
  }, [page]);

  // Функція для додавання/видалення лайка
  const handleLike = async (postId) => {
    try {
      const currentReaction = userReactions[postId];
      if (currentReaction) {
        // Видалити реакцію
        await deleteReaction(currentReaction.id);
        setUserReactions((prev) => ({ ...prev, [postId]: null }));
        setPosts((prevPosts) =>
          prevPosts.map((post) =>
            post.id === postId ? { ...post, likes: post.likes - 1 } : post
          )
        );
      } else {
        // Додати реакцію
        const reactionData = {
          entityId: postId,
          reactionTypeId: "like", // ID для "лайка" з вашої бази даних
        };
        await addReaction(reactionData);
        const newReaction = { id: "temp-id", userId: "current_user_id", reactionTypeId: "like" }; // Замініть на реальні дані
        setUserReactions((prev) => ({ ...prev, [postId]: newReaction }));
        setPosts((prevPosts) =>
          prevPosts.map((post) =>
            post.id === postId ? { ...post, likes: post.likes + 1 } : post
          )
        );
      }
    } catch (error) {
      setError(`Error handling reaction: ${error.response?.data?.error || error.message}`);
    }
  };

  const handlePageChange = (event, value) => {
    setPage(value);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  if (error) {
    return <Typography color="error">{error}</Typography>;
  }

  return (
    <Box
      sx={{
        maxWidth: { xs: "calc(100% - 240px)", md: 600 },
        mx: "auto",
        pt: 3,
        pb: 4,
        boxSizing: "border-box",
        width: "100%",
        overflowX: "hidden",
      }}
    >
      {Array.isArray(posts) && posts.length > 0 ? (
        posts.map((post) => (
          <Card
            key={post.id}
            sx={{
              mb: 3,
              boxShadow: "none",
              border: "1px solid #ddd",
              borderRadius: "8px",
              width: "100%",
              boxSizing: "border-box",
            }}
          >
            <CardContent>
              <Box sx={{ display: "flex", alignItems: "center", mb: 1 }}>
                <Avatar src={post.author?.profilePicture || ""} alt="Author" sx={{ width: 40, height: 40, mr: 2 }} />
                <Box>
                  <Typography variant="h6" sx={{ fontSize: "1.1rem", fontWeight: "bold" }}>
                    {post.author ? `${post.author.firstName} ${post.author.lastName}` : "Unknown User"}
                  </Typography>
                  <Typography variant="body2" color="textSecondary" sx={{ fontSize: "0.9rem" }}>
                    @{post.author?.username || "unknown"} ·{" "}
                    {post.createdAt
                      ? new Date(post.createdAt).toLocaleDateString("uk-UA", {
                          month: "long",
                          day: "numeric",
                          year: "numeric",
                        })
                      : "Unknown Date"}
                  </Typography>
                </Box>
              </Box>
              <Typography sx={{ mt: 1, color: "#444" }}>{post.content || "No content"}</Typography>
              {post.imageUrl && (
                <Box sx={{ mt: 2 }}>
                  <img
                    src={post.imageUrl}
                    alt="Post"
                    style={{
                      width: "100%",
                      maxHeight: "300px",
                      objectFit: "cover",
                      borderRadius: "8px",
                    }}
                  />
                </Box>
              )}
            </CardContent>
            <CardContent sx={{ display: "flex", justifyContent: "start", gap: 2 }}>
              <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
                <IconButton size="small" onClick={() => handleLike(post.id)}>
                  {userReactions[post.id] ? (
                    <ThumbUp sx={{ color: "#1F4A7C" }} />
                  ) : (
                    <ThumbUpOutlined sx={{ color: "#1F4A7C" }} />
                  )}
                </IconButton>
                <Typography>{post.likes || 0}</Typography>
              </Box>
              <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
                <IconButton size="small">
                  <Comment sx={{ color: "#1F4A7C" }} />
                </IconButton>
                <Typography>{post.comments?.length || 0}</Typography>
              </Box>
              <Box sx={{ display: "flex", alignItems: "center", gap: 0.5 }}>
                <IconButton size="small">
                  <Share sx={{ color: "#1F4A7C" }} />
                </IconButton>
                <Typography>{post.shares || 0}</Typography>
              </Box>
            </CardContent>
          </Card>
        ))
      ) : (
        <Typography>Ще немає постів.</Typography>
      )}

      {totalPages > 1 && (
        <Box
          sx={{
            py: 2,
            display: "flex",
            justifyContent: "center",
            borderTop: "1px solid #ddd",
            backgroundColor: "#fff",
            mt: 2,
            mb: 4,
            width: "100%",
            boxSizing: "border-box",
          }}
        >
          <Pagination
            count={totalPages}
            page={page}
            onChange={handlePageChange}
            color="primary"
            showFirstButton
            showLastButton
            siblingCount={1}
            boundaryCount={1}
            sx={{
              "& .MuiPaginationItem-root": {
                color: "#666",
                fontSize: "1rem",
                fontWeight: "500",
                borderRadius: "4px",
                margin: "0 4px",
                "&:hover": {
                  backgroundColor: "transparent",
                  color: "#1F4A7C",
                },
              },
              "& .Mui-selected": {
                backgroundColor: "#1F4A7C",
                color: "white",
                "&:hover": {
                  backgroundColor: "#163a5f",
                },
              },
              "& .MuiPaginationItem-ellipsis": {
                color: "#666",
                fontSize: "1rem",
              },
            }}
            renderItem={(item) => {
              if (item.type === "previous") {
                return (
                  <Box
                    component="button"
                    onClick={item.onClick}
                    disabled={item.disabled}
                    sx={{
                      display: "inline-flex",
                      alignItems: "center",
                      backgroundColor: "#1F4A7C",
                      color: "white",
                      borderRadius: "8px",
                      padding: "8px 16px",
                      fontSize: "1rem",
                      fontWeight: "500",
                      border: "none",
                      cursor: item.disabled ? "not-allowed" : "pointer",
                      opacity: item.disabled ? 0.5 : 1,
                      "&:hover": !item.disabled && {
                        backgroundColor: "#163a5f",
                      },
                      mr: 2,
                    }}
                  >
                    <span style={{ marginRight: "8px" }}>{"<"}</span>
                    Previous
                  </Box>
                );
              }
              if (item.type === "next") {
                return (
                  <Box
                    component="button"
                    onClick={item.onClick}
                    disabled={item.disabled}
                    sx={{
                      display: "inline-flex",
                      alignItems: "center",
                      backgroundColor: "#1F4A7C",
                      color: "white",
                      borderRadius: "8px",
                      padding: "8px 16px",
                      fontSize: "1rem",
                      fontWeight: "500",
                      border: "none",
                      cursor: item.disabled ? "not-allowed" : "pointer",
                      opacity: item.disabled ? 0.5 : 1,
                      "&:hover": !item.disabled && {
                        backgroundColor: "#163a5f",
                      },
                      ml: 2,
                    }}
                  >
                    Next
                    <span style={{ marginLeft: "8px" }}>{">"}</span>
                  </Box>
                );
              }
              if (item.type === "page") {
                return (
                  <Box
                    component="button"
                    onClick={item.onClick}
                    disabled={item.disabled}
                    sx={{
                      display: "inline-flex",
                      alignItems: "center",
                      backgroundColor: item.selected ? "#1F4A7C" : "transparent",
                      color: item.selected ? "white" : "#666",
                      fontSize: "1rem",
                      fontWeight: "500",
                      border: "none",
                      borderRadius: "4px",
                      padding: "4px 8px",
                      cursor: "pointer",
                      "&:hover": {
                        backgroundColor: item.selected ? "#163a5f" : "transparent",
                        color: item.selected ? "white" : "#1F4A7C",
                      },
                      margin: "0 4px",
                    }}
                  >
                    {item.page}
                  </Box>
                );
              }
              if (item.type === "ellipsis" || item.type === "start-ellipsis" || item.type === "end-ellipsis") {
                return (
                  <Box
                    sx={{
                      display: "inline-flex",
                      alignItems: "center",
                      color: "#666",
                      fontSize: "1rem",
                      fontWeight: "500",
                      padding: "4px 8px",
                    }}
                  >
                    ...
                  </Box>
                );
              }
              return null;
            }}
          />
        </Box>
      )}
    </Box>
  );
};

export default HomePage;