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
  Container,
  FormControl,
  InputLabel,
  Select,
  MenuItem
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
  const [categories, setCategories] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState("");
  const [loadingCategories, setLoadingCategories] = useState(false);

  useEffect(() => {
    // Fetch categories when component mounts
    fetchCategories();
    
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

  const fetchCategories = async () => {
    setLoadingCategories(true);
    try {
      const response = await fetch(`${API_URL}/api/post/get-categories`, {
        credentials: "include",
      });
      
      if (!response.ok) {
        throw new Error('Failed to fetch categories');
      }
      
      const data = await response.json();
      // Ensure data is an array
      const categoriesArray = Array.isArray(data) ? data : 
                             (data.$values ? data.$values : []);
      
      setCategories(categoriesArray);
      
      // Set default category if available
      if (categoriesArray.length > 0) {
        setSelectedCategory(categoriesArray[0].id.toString());
      }
    } catch (err) {
      console.error("Error fetching categories:", err);
      setCategories([]); // Ensure categories is always an array
    } finally {
      setLoadingCategories(false);
    }
  };

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
    if (!selectedCategory) {
      setError("Please select a category.");
      return;
    }
    
    setLoading(true);
    setError(null);

    try {
      const formData = new FormData();
      formData.append('description', content);
      formData.append('image', image);
      
      // Find the selected category object to get both ID and name
      const selectedCategoryObj = categories.find(cat => cat.id.toString() === selectedCategory);
      
      // Add both category ID and name
      formData.append('CategoryId', selectedCategory);
      formData.append('Category', selectedCategoryObj ? selectedCategoryObj.name : '');
      
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
      // Reset to default category if available
      if (categories.length > 0) {
        setSelectedCategory(categories[0].id.toString());
      } else {
        setSelectedCategory("");
      }
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
          
          <FormControl fullWidth margin="normal">
            <InputLabel id="category-select-label">Category</InputLabel>
            <Select
              labelId="category-select-label"
              id="category-select"
              value={selectedCategory}
              label="Category"
              onChange={(e) => setSelectedCategory(e.target.value)}
              disabled={loadingCategories}
            >
              {loadingCategories ? (
                <MenuItem value="">
                  <CircularProgress size={20} />
                </MenuItem>
              ) : (
                categories.map((category) => (
                  <MenuItem key={category.id} value={category.id.toString()}>
                    {category.name}
                  </MenuItem>
                ))
              )}
            </Select>
          </FormControl>
          
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
            <Box sx={{ 
              position: 'relative', 
              mt: 2, 
              width: '100%', 
              height: '300px', 
              display: 'flex',
              justifyContent: 'center',
              alignItems: 'center',
              backgroundColor: 'black',
              borderRadius: '4px',
              overflow: 'hidden'
            }}>
              <img 
                src={imagePreview} 
                alt="Preview" 
                style={{ 
                  maxWidth: '100%', 
                  maxHeight: '100%',
                  objectFit: 'contain'
                }} 
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