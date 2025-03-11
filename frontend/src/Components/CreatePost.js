import { useState } from "react";
import { 
  Button, 
  TextField, 
  Card, 
  CardContent, 
  Typography, 
  Box, 
  CircularProgress,
  IconButton
} from "@mui/material";
import EmojiEmotionsIcon from "@mui/icons-material/EmojiEmotions";
import { useAuth } from "@/hooks/useAuth";
import { useFeed } from "@/hooks/useFeed";

export default function CreatePostPage() {
  const { user, isAuthenticated } = useAuth();
  const { addPost } = useFeed();
  const [content, setContent] = useState("");
  const [image, setImage] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  if (!isAuthenticated) {
    return <Typography align="center">You must be logged in to create a post.</Typography>;
  }

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!content.trim()) {
      setError("Post cannot be empty.");
      return;
    }
    setLoading(true);
    setError(null);

    try {
      await addPost({ content, image, user });
      setContent("");
      setImage(null);
    } catch (err) {
      setError("Something went wrong. Please try again.");
    }
    setLoading(false);
  };

  const handleEmojiClick = () => {
    // Simple emoji insertion - in a real app, you'd use a proper emoji picker
    setContent(prev => prev + "😊");
  };

  return (
    <Card sx={{ maxWidth: "600px", mx: "auto", mt: 3, p: 2 }}>
      <CardContent>
        <form onSubmit={handleSubmit}>
          <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
            <TextField
              value={content}
              onChange={(e) => setContent(e.target.value)}
              placeholder="What's on your mind?"
              multiline
              rows={4}
              fullWidth
              variant="outlined"
            />
            
            <Box sx={{ display: "flex", justifyContent: "flex-end" }}>
              <IconButton onClick={handleEmojiClick} color="primary">
                <EmojiEmotionsIcon />
              </IconButton>
            </Box>
            
            <Button
              variant="contained"
              component="label"
              sx={{ mt: 1 }}
            >
              Upload Image
              <input
                type="file"
                accept="image/*"
                hidden
                onChange={(e) => setImage(e.target.files[0])}
              />
            </Button>
            
            {image && (
              <Typography variant="body2">
                Selected file: {image.name}
              </Typography>
            )}
            
            {error && (
              <Typography color="error" variant="body2">
                {error}
              </Typography>
            )}
            
            <Button 
              type="submit" 
              variant="contained" 
              color="primary"
              disabled={loading}
              sx={{ mt: 1 }}
            >
              {loading ? <CircularProgress size={24} /> : "Post"}
            </Button>
          </Box>
        </form>
      </CardContent>
    </Card>
  );
}