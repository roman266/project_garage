import React from 'react';
import { useAuth } from '../api';

const Home = () => {
  const { user, isLoading, isAuthenticated } = useAuth();

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      <h1>Home Page</h1>
      {isAuthenticated ? (
        <p>Welcome to the home page, {user?.firstName || 'User'}!</p>
      ) : (
        <div style={{ color: 'red', marginTop: '20px' }}>
          Authentication failed. Please log in again.
        </div>
      )}
    </div>
  );
};

export default Home;