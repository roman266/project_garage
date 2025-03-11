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
// Fix the import paths to use relative paths instead of alias

export default function CreatePostPage() {
  const [content, setContent] = useState("");
  const [image, setImage] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Remove authentication check
  // if (!isAuthenticated) {
  //   return <Typography align="center">You must be logged in to create a post.</Typography>;
  // }

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!content.trim()) {
      setError("Post cannot be empty.");
      return;
    }
    if (!image) {
      setError("Image is required.");
      return;
    }
    
    setLoading(true);
    setError(null);

    try {
      const formData = new FormData();
      formData.append('description', content);
      formData.append('image', image);
      
      const response = await fetch('https://localhost:7126/api/post/create', {
        method: 'POST',
        body: formData,
        credentials: "include",
      });

      if (!response.ok) {
        throw new Error('Failed to create post');
      }

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