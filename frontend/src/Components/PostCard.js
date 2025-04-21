import React, { useState } from "react";
import {
  Card,
  CardContent,
  CardActions,
  IconButton,
  Typography,
  Avatar,
  Box,
  Divider,
  Modal,
  TextField,
  Button,
} from "@mui/material";
import { ThumbUp, Comment } from "@mui/icons-material";

const PostCard = ({
  avatarUrl,
  profileName,
  postDescription,
  postImage,
  contentText,
  likesCount: initialLikes,
  comments: initialComments = [],
  onLike,
  onComment,
}) => {
  const [likesCount, setLikesCount] = useState(initialLikes);
  const [comments, setComments] = useState(initialComments);
  const [liked, setLiked] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);
  const [newComment, setNewComment] = useState("");

  const handleLike = () => {
    setLiked(!liked);
    setLikesCount(prev => prev + (liked ? -1 : 1));
    if (onLike) onLike();
  };

  const handleAddComment = () => {
    if (newComment.trim() !== "") {
      const updatedComments = [...comments, newComment.trim()];
      setComments(updatedComments);
      setNewComment("");
      if (onComment) onComment();
    }
  };

  return (
    <>
      <Card sx={{ maxWidth: 600, margin: 'auto', borderRadius: 0, boxShadow: 'none', borderBottom: '1px solid #e0e0e0' }}>
        <Box sx={{ display: 'flex', alignItems: 'center', padding: 2 }}>
          <Avatar src={avatarUrl} sx={{ bgcolor: 'grey.400', marginRight: 2 }} />
          <Box>
            <Typography variant="subtitle1" fontWeight={600}>{profileName}</Typography>
            <Typography variant="caption" color="text.secondary">{postDescription}</Typography>
          </Box>
        </Box>

        <CardContent sx={{ paddingTop: 0, paddingBottom: 1 }}>
          {postImage && (
            <Box
              component="img"
              src={postImage}
              alt="post"
              sx={{
                width: '100%',
                maxHeight: 500,
                objectFit: 'contain',
                borderRadius: 2,
                marginBottom: 2,
              }}
            />
          )}
          <Typography variant="body2" fontWeight={500} sx={{ marginBottom: 1 }}>{contentText}</Typography>
          <Divider sx={{ marginBottom: 1 }} />
        </CardContent>

        <CardActions sx={{ justifyContent: 'space-around', paddingBottom: 2 }}>
          <IconButton onClick={handleLike} color={liked ? 'primary' : 'default'}>
            <ThumbUp fontSize="small" />
            <Typography variant="caption" sx={{ marginLeft: 0.5 }}>{likesCount} Like</Typography>
          </IconButton>
          <IconButton onClick={() => setModalOpen(true)}>
            <Comment fontSize="small" />
            <Typography variant="caption" sx={{ marginLeft: 0.5 }}>{comments.length} Comments</Typography>
          </IconButton>
        </CardActions>
      </Card>

      <Modal open={modalOpen} onClose={() => setModalOpen(false)}>
        <Box sx={{
          position: 'absolute',
          top: '50%',
          left: '50%',
          transform: 'translate(-50%, -50%)',
          width: 400,
          bgcolor: 'background.paper',
          borderRadius: 2,
          boxShadow: 24,
          p: 3,
        }}>
          <Typography variant="h6" gutterBottom>Comments</Typography>
          <Box sx={{ maxHeight: 300, overflowY: 'auto', mb: 2 }}>
            {comments.map((c, i) => (
              <Typography key={i} variant="body2" sx={{ mb: 1 }}>- {c}</Typography>
            ))}
          </Box>
          <Box sx={{ display: 'flex', gap: 1 }}>
            <TextField
              fullWidth
              variant="outlined"
              size="small"
              placeholder="Write a comment..."
              value={newComment}
              onChange={(e) => setNewComment(e.target.value)}
            />
            <Button variant="contained" onClick={handleAddComment}>Post</Button>
          </Box>
        </Box>
      </Modal>
    </>
  );
};

export default PostCard;
