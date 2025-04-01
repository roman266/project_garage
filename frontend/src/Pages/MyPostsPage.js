import React from 'react';
import { Box, Container } from '@mui/material';
import MyPosts from '../Components/MyPosts';

export default function MyPostsPage() {
  return (
    <Container maxWidth="lg">
      <Box sx={{ width: '100%', margin: '20px auto' }}>
        <MyPosts />
      </Box>
    </Container>
  )
}
