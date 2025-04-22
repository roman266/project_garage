import React, { useEffect, useRef, useState, useCallback } from 'react';
import { Box, Typography, List, ListItem, ListItemText, Avatar, CircularProgress } from '@mui/material';
import { Link } from 'react-router-dom';
import axios from 'axios';

const SearchResults = ({ results, query, onClose }) => {
  const resultsRef = useRef(null);
  const [users, setUsers] = useState(results || []);
  const [loading, setLoading] = useState(false);
  const [hasMore, setHasMore] = useState(true);
  const [lastUserId, setLastUserId] = useState(null);

  const API_BASE_URL = process.env.REACT_APP_HTTPS_API_URL;

  useEffect(() => {
    setUsers(results || []);
    if (results?.length > 0) {
      setLastUserId(results[results.length - 1].id);
      setHasMore(results.length >= 10);
    } else {
      setHasMore(false);
    }
  }, [results]);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (resultsRef.current && !resultsRef.current.contains(event.target)) {
        onClose();
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [onClose]);

  const loadMoreUsers = useCallback(async () => {
    if (loading || !hasMore) return;

    setLoading(true);
    try {
      const response = await axios.get(`${API_BASE_URL}/api/search/users`, {
        params: {
          query,
          lastUserId,
          limit: 10
        },
        withCredentials: true
      });

      const newUsers = response.data?.$values || [];

      if (newUsers.length < 10) setHasMore(false);
      if (newUsers.length > 0) {
        setUsers(prev => [...prev, ...newUsers]);
        setLastUserId(newUsers[newUsers.length - 1].id);
      }
    } catch (error) {
      console.error("Error loading more users", error);
    } finally {
      setLoading(false);
    }
  }, [API_BASE_URL, query, lastUserId, hasMore, loading]);

  const handleScroll = useCallback(() => {
    if (!resultsRef.current) return;

    const { scrollTop, scrollHeight, clientHeight } = resultsRef.current;

    // Якщо скролл близький до кінця — завантажити ще
    if (scrollTop + clientHeight >= scrollHeight - 10) {
      loadMoreUsers();
    }
  }, [loadMoreUsers]);

  useEffect(() => {
    const currentRef = resultsRef.current;
    if (currentRef) {
      currentRef.addEventListener('scroll', handleScroll);
    }

    return () => {
      if (currentRef) {
        currentRef.removeEventListener('scroll', handleScroll);
      }
    };
  }, [handleScroll]);

  if (typeof results === 'string') {
    return (
      <Box ref={resultsRef} sx={{ ...popupStyles }}>
        <Typography variant="body1" sx={{ color: '#777' }}>{results}</Typography>
      </Box>
    );
  }

  if (!Array.isArray(users) || users.length === 0) {
    return null;
  }

  return (
    <Box ref={resultsRef} sx={{ ...popupStyles }}>
      <List>
        {users.map(({ id, userName, firstName, lastName, profilePicture, activeStatus }) => {
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
                <Box sx={{
                  position: 'absolute',
                  bottom: 0,
                  right: 3,
                  width: 10,
                  height: 10,
                  borderRadius: '50%',
                  backgroundColor: isActive ? 'green' : 'gray',
                  border: '2px solid white',
                }} />
              </Box>
              <ListItemText primary={`${displayFirstName} ${displayLastName}`.trim()} secondary={`@${userName}`} />
            </ListItem>
          );
        })}
      </List>

      {loading && (
        <Box display="flex" justifyContent="center" mt={1} mb={1}>
          <CircularProgress size={20} />
        </Box>
      )}
    </Box>
  );
};

const popupStyles = {
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
};

export default SearchResults;
