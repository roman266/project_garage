import React, { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Grid,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  IconButton
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import axios from 'axios';
import { API_URL } from '../constants';
import PostCard from './PostCard';

const MyPosts = () => {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [hasMore, setHasMore] = useState(true);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [currentPost, setCurrentPost] = useState(null);
  const [editedText, setEditedText] = useState('');
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [postToDelete, setPostToDelete] = useState(null);

  useEffect(() => {
    fetchPosts();
  }, []);

  const fetchPosts = async (lastPostId = null) => {
    try {
      setLoading(true);
      const response = await axios.get(`${API_URL}/api/post/my-posts`, {
        params: {
          lastPostId: lastPostId,
          limit: 15
        },
        withCredentials: true
      });

      // Handle the response format with $values array
      const postsData = response.data && response.data.$values
        ? response.data.$values
        : (Array.isArray(response.data) ? response.data : []);

      if (postsData.length < 15) {
        setHasMore(false);
      }

      if (lastPostId) {
        setPosts(prev => [...prev, ...postsData]);
      } else {
        setPosts(postsData);
      }

      console.log('Posts data:', postsData);
    } catch (error) {
      console.error('Error fetching posts:', error);
      setPosts([]);
    } finally {
      setLoading(false);
    }
  };

  const handleEditClick = (post) => {
    setCurrentPost(post);
    setEditedText(post.postDescription);
    setEditDialogOpen(true);
  };

  const handleEditSave = async () => {
    try {
      await axios.patch(`${API_URL}/api/post/edit/${currentPost.postId}`, {
        postId: currentPost.postId,
        description: editedText  // Changed from 'text' to 'description' to match backend DTO
      }, {
        withCredentials: true
      });

      setPosts(posts.map(post =>
        post.postId === currentPost.postId ? { ...post, postDescription: editedText } : post
      ));

      setEditDialogOpen(false);
    } catch (error) {
      console.error('Error updating post:', error);
    }
  };

  const handleDeleteClick = (post) => {
    setPostToDelete(post);
    setDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    try {
      await axios.delete(`${API_URL}/api/post/delete/${postToDelete.postId}`, {
        withCredentials: true
      });

      setPosts(posts.filter(post => post.postId !== postToDelete.postId));
      setDeleteDialogOpen(false);
    } catch (error) {
      console.error('Error deleting post:', error);
    }
  };

  const loadMorePosts = () => {
    if (posts.length > 0) {
      const lastPostId = posts[posts.length - 1].postId;
      fetchPosts(lastPostId);
    }
  };

  return (
    <Box sx={{ padding: 3, backgroundColor: '#F0F8FF' }}>
      <Typography color='#365B87' variant="h4" gutterBottom>
        My Posts
      </Typography>

      {loading && (!posts || posts.length === 0) ? (
        <Box display="flex" justifyContent="center" my={4}>
          <CircularProgress />
        </Box>
      ) : (!posts || posts.length === 0) ? (
        <Typography variant="body1" color="textSecondary" align="center" my={4}>
          You haven't created any posts yet.
        </Typography>
      ) : (
        <>
          <Box sx={{ maxHeight: 'calc(100vh - 200px)', overflow: 'auto', pr: 1 }}>
            <Grid container spacing={3}>
              {Array.isArray(posts) && posts.map(post => (
                <Grid item xs={12} key={post.postId}>
                  <Box position="relative">
                    <Box position="absolute" right={10} top={10} zIndex={1} sx={{ display: 'flex', gap: 1 }}>
                      <IconButton
                        size="small"
                        onClick={() => handleEditClick(post)}
                        sx={{ bgcolor: 'white', '&:hover': { bgcolor: 'white' } }}
                      >
                        <EditIcon />
                      </IconButton>
                      <IconButton
                        size="small"
                        onClick={() => handleDeleteClick(post)}
                        sx={{ bgcolor: 'white', '&:hover': { bgcolor: 'white' } }}
                      >
                        <DeleteIcon />
                      </IconButton>
                    </Box>
                    <PostCard
                      avatarUrl={post.senderAvatarUlr}
                      profileName={post.senderNickName}
                      postDescription={new Date(post.postDate).toLocaleString()}
                      postImage={post.postImageUrl}
                      contentText={post.postDescription}
                      likesCount={post.likesCount || 0}
                      comments={post.comments || []}
                    />
                  </Box>
                </Grid>
              ))}
            </Grid>

            {hasMore && (
              <Box display="flex" justifyContent="center" mt={3}>
                <Button
                  variant="outlined"
                  onClick={loadMorePosts}
                  disabled={loading}
                >
                  {loading ? <CircularProgress size={24} /> : 'Load More'}
                </Button>
              </Box>
            )}
          </Box>
        </>
      )}

      {/* Keep existing dialogs */}
      {/* Edit Dialog */}
      <Dialog open={editDialogOpen} onClose={() => setEditDialogOpen(false)}>
        <DialogTitle>Edit Post</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Post Text"
            fullWidth
            multiline
            rows={4}
            value={editedText}
            onChange={(e) => setEditedText(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleEditSave} color="primary">Save</Button>
        </DialogActions>
      </Dialog>

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Delete Post</DialogTitle>
        <DialogContent>
          <Typography>Are you sure you want to delete this post? This action cannot be undone.</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleDeleteConfirm} color="error">Delete</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default MyPosts;