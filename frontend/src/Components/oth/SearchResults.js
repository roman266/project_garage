import React, { useEffect, useRef } from 'react';
import { Box, Typography, List, ListItem, ListItemText, Avatar, Button } from '@mui/material';
import { Link } from 'react-router-dom';

const SearchResults = ({ results, onClose, onLoadMore, hasMoreResults }) => {
  const resultsRef = useRef(null);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (resultsRef.current && !resultsRef.current.contains(event.target)) {
        onClose();
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [onClose]);

  if (typeof results === 'string') {
    return (
      <Box
        ref={resultsRef}
        sx={{
          backgroundColor: 'white',
          borderRadius: '8px',
          boxShadow: 1,
          mt: 1,
          p: 2,
          position: 'fixed',
          top: '64px',
          right: '16px',
          width: '250px',
          maxHeight: '300px',
          overflowY: 'auto',
          zIndex: 1101,
        }}
      >
        <Typography variant="body1" sx={{ color: '#777' }}>{results}</Typography>
      </Box>
    );
  }

  if (!Array.isArray(results) || results.length === 0) {
    return null;
  }

  return (
    <Box
      ref={resultsRef}
      sx={{
        backgroundColor: 'white',
        borderRadius: '8px',
        boxShadow: 1,
        mt: 1,
        p: 1,
        position: 'fixed',
        top: '64px',
        right: '16px',
        width: '250px',
        maxHeight: '300px',
        overflowY: 'auto',
        zIndex: 1101,
        display: 'flex',
        flexDirection: 'column'
      }}
    >
      <List sx={{ flex: 1, overflowY: 'auto' }}> {/* Allow scrolling for results */}
        {results.map(({ id, userName, firstName, lastName, profilePicture, activeStatus }) => {
          const displayFirstName = firstName && firstName !== 'None' ? firstName : '';
          const displayLastName = lastName && lastName !== 'None' ? lastName : '';
          const avatarSrc = profilePicture && profilePicture !== 'None' ? profilePicture : '/default-avatar.png';
          const isActive = activeStatus === 'Online';

          return (
            <ListItem
              key={id}
              sx={{ borderBottom: '1px solid #ddd', position: 'relative' }}
              component={Link}
              to={`/profile/${id}`}
              onClick={onClose}
            >
              <Box sx={{ position: 'relative', display: 'inline-block' }}>
                <Avatar src={avatarSrc} sx={{ width: 32, height: 32, marginRight: 0.5 }} />
                <Box
                  sx={{
                    position: 'absolute',
                    bottom: 0,
                    right: 3,
                    width: 10,
                    height: 10,
                    borderRadius: '50%',
                    backgroundColor: isActive ? 'green' : 'gray',
                    border: '2px solid white',
                  }}
                />
              </Box>
              <ListItemText primary={`${displayFirstName} ${displayLastName}`.trim()} secondary={`@${userName}`} />
            </ListItem>
          );
        })}
      </List>
      {hasMoreResults && (
        <Button
          variant="contained"
          color="primary"
          fullWidth
          onClick={onLoadMore}
          sx={{ mt: 1 }}
        >
          Load More
        </Button>
      )}
    </Box>
  );
};

export default SearchResults;