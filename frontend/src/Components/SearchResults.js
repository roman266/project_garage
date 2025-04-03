import React from 'react';
import { Box, Typography, List, ListItem, ListItemText, Avatar } from '@mui/material';
import { Link } from 'react-router-dom';

const SearchResults = ({ results }) => {
  if (typeof results === 'string') {
    return (
      <Box sx={{
        backgroundColor: 'white',
        borderRadius: '8px',
        boxShadow: 1,
        mt: 1,
        p: 2,
        position: 'absolute',
        width: '250px',
        maxHeight: '300px',
        overflowY: 'auto'
      }}>
        <Typography variant="body1" sx={{ color: '#777' }}>{results}</Typography>
      </Box>
    );
  }

  if (!Array.isArray(results) || results.length === 0) {
    return null;
  }

  return (
    <Box sx={{
      backgroundColor: 'white',
      borderRadius: '8px',
      boxShadow: 1,
      mt: 1,
      p: 1,
      position: 'absolute',
      width: '250px',
      maxHeight: '300px',
      overflowY: 'auto'
    }}>
      <List>
        {results.map(({ id, userName, firstName, lastName, profilePicture }) => {
          const displayFirstName = firstName && firstName !== 'None' ? firstName : 'Unknown';
          const displayLastName = lastName && lastName !== 'None' ? lastName : '';
          const avatarSrc = profilePicture && profilePicture !== 'None' ? profilePicture : '/default-avatar.png';
          
          return (
            <ListItem key={id} sx={{ borderBottom: '1px solid #ddd' }} component={Link} to={`/profile/${id}`}>
              <Avatar src={avatarSrc} sx={{ width: 32, height: 32, marginRight: 1 }} />
              <ListItemText primary={`${displayFirstName} ${displayLastName}`.trim()} secondary={`@${userName}`} />
            </ListItem>
          );
        })}
      </List>
    </Box>
  );
};

export default SearchResults;
