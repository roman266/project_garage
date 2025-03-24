import { useState, useEffect } from "react";
import { 
  Button, 
  TextField, 
  Card, 
  CardContent, 
  Typography, 
  Box, 
  CircularProgress,
  IconButton,
  Container
} from "@mui/material";
import EmojiEmotionsIcon from "@mui/icons-material/EmojiEmotions";
import ClearIcon from '@mui/icons-material/Clear';
import { API_URL } from "../constants";

export default function CreatePostPage() {
  const [content, setContent] = useState("");
  const [image, setImage] = useState(null);
  const [imagePreview, setImagePreview] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (image) {
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result);
      };
      reader.readAsDataURL(image);
    } else {
      setImagePreview(null);
    }
  }, [image]);

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
      
      const response = await fetch(`${API_URL}/api/post/create`, {
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
    setContent(prev => prev + "😊");
  };

  const clearImage = () => {
    setImage(null);
    setImagePreview(null);
  };

  return (
    <Container maxWidth="md">
      <Box
        sx={{
          backgroundColor: "#DFDFDF",
          padding: "30px",
          borderRadius: "12px",
          boxShadow: "2px 2px 15px rgba(0,0,0,0.3)",
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          width: "450px",
          margin: "auto",
          mt: "100px",
          fontFamily: "Roboto, sans-serif",
        }}
      >
        <Typography variant="h4" gutterBottom sx={{ display: "flex", alignItems: "center", fontFamily: "Roboto, sans-serif", color: "#365B87" }}>
          Create Post
        </Typography>
        <form onSubmit={handleSubmit} style={{ width: "100%" }}>
          <TextField
            fullWidth
            placeholder="What's on your mind?"
            multiline
            rows={4}
            value={content}
            onChange={(e) => setContent(e.target.value)}
            variant="outlined"
            margin="normal"
            sx={{ fontFamily: "Roboto, sans-serif" }}
          />
          
          <Box sx={{ display: "flex", justifyContent: "flex-end" }}>
            <IconButton onClick={handleEmojiClick} color="primary">
              <EmojiEmotionsIcon />
            </IconButton>
          </Box>
          
          <Button
            variant="contained"
            component="label"
            sx={{ 
              backgroundColor: "#1F4A7C", 
              color: "white", 
              fontFamily: "Roboto, sans-serif",
              mt: 1,
              width: "100%"
            }}
          >
            Upload Image
            <input
              type="file"
              accept="image/*"
              hidden
              onChange={(e) => setImage(e.target.files[0])}
            />
          </Button>
          
          {imagePreview && (
            <Box sx={{ position: 'relative', mt: 2, width: '100%' }}>
              <img 
                src={imagePreview} 
                alt="Preview" 
                style={{ width: '100%', borderRadius: '4px' }} 
              />
              <IconButton 
                onClick={clearImage}
                sx={{ 
                  position: 'absolute', 
                  top: 5, 
                  right: 5, 
                  bgcolor: 'rgba(255,255,255,0.7)',
                  '&:hover': { bgcolor: 'rgba(255,255,255,0.9)' }
                }}
                size="small"
              >
                <ClearIcon />
              </IconButton>
            </Box>
          )}
          
          {error && (
            <Typography color="error" variant="body2" sx={{ mt: 1 }}>
              {error}
            </Typography>
          )}
          
          <Button 
            type="submit" 
            variant="contained" 
            disabled={loading}
            sx={{ 
              backgroundColor: "#1F4A7C", 
              color: "white", 
              fontFamily: "Roboto, sans-serif",
              mt: 2,
              width: "100%"
            }}
          >
            {loading ? <CircularProgress size={24} /> : "Post"}
          </Button>
        </form>
      </Box>
    </Container>
  );
}