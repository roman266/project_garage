import React, { useState } from 'react';
import { Box, IconButton, Grid, Paper } from '@mui/material';
import InsertEmoticonIcon from '@mui/icons-material/InsertEmoticon';

const EmojiPickerComponent = ({ onEmojiSelect }) => {
  const [showPicker, setShowPicker] = useState(false);
  
  // Common emojis - expanded list
  const emojis = [
    '😀', '😁', '😂', '🤣', '😃', '😄', '😅', '😆', 
    '😉', '😊', '😋', '😎', '😍', '😘', '🥰', '😗',
    '😙', '😚', '🙂', '🤗', '🤩', '🤔', '🤨', '😐',
    '😑', '😶', '🙄', '😏', '😣', '😥', '😮', '🤐',
    '😯', '😪', '😫', '😴', '😌', '😛', '😜', '😝',
    '🤤', '😒', '😓', '😔', '😕', '🙃', '🤑', '😲',
    '😳', '🥺', '😱', '😨', '😰', '😢', '😭', '😖',
    '😟', '😩', '😤', '😠', '😡', '🤬', '😈', '👿',
    '💀', '☠️', '💩', '🤡', '👹', '👺', '👻', '👽',
    '👾', '🤖', '😺', '😸', '😹', '😻', '😼', '😽',
    '🙀', '😿', '😾', '🙈', '🙉', '🙊', '💋', '💌',
    '💘', '💝', '💖', '💗', '💓', '💞', '💕', '💟'
  ];

  const handleEmojiClick = (emoji) => {
    onEmojiSelect(emoji);
    setShowPicker(false);
  };

  return (
    <Box sx={{ position: 'relative' }}>
      <IconButton 
        sx={{ marginLeft: 1 }}
        onClick={() => setShowPicker(!showPicker)}
      >
        <InsertEmoticonIcon sx={{ color: "#1e497c" }} />
      </IconButton>
      
      {showPicker && (
        <Box 
          sx={{ 
            position: 'absolute', 
            bottom: '40px', 
            right: '0px',
            zIndex: 10,
            boxShadow: 3,
            width: '250px'
          }}
        >
          <Paper sx={{ padding: 1 }}>
            <Box 
              sx={{ 
                maxHeight: '200px', 
                overflowY: 'auto',
                '&::-webkit-scrollbar': {
                  width: '8px',
                },
                '&::-webkit-scrollbar-thumb': {
                  backgroundColor: '#c1c1c1',
                  borderRadius: '4px',
                },
              }}
            >
              <Grid container spacing={0.5}>
                {emojis.map((emoji, index) => (
                  <Grid item xs={2.4} key={index} sx={{ textAlign: 'center' }}>
                    <IconButton 
                      size="small" 
                      onClick={() => handleEmojiClick(emoji)}
                      sx={{ fontSize: '1.2rem', padding: '4px' }}
                    >
                      {emoji}
                    </IconButton>
                  </Grid>
                ))}
              </Grid>
            </Box>
          </Paper>
        </Box>
      )}
    </Box>
  );
};

export default EmojiPickerComponent;