import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../utils/apiClient';

const Home = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    // Check authentication status
    const checkAuth = async () => {
      try {
        const response = await api.get('/api/profile/me');
        console.log('Auth check successful:', response.data);
        setLoading(false);
      } catch (error) {
        console.error('Auth check failed:', error);
        setError('Authentication failed. Please log in again.');
        // Don't automatically redirect here to prevent loops
        setLoading(false);
      }
    };

    checkAuth();
  }, [navigate]);

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      <h1>Home Page</h1>
      <p>Welcome to the home page!</p>
      {error && (
        <div style={{ color: 'red', marginTop: '20px' }}>
          {error}
        </div>
      )}
    </div>
  );
};

export default Home;