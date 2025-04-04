import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

// PublicRoute component - only accessible for non-authenticated users
const PublicRoute = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuth();
  
  if (isLoading) {
    return <div>Loading authentication...</div>;
  }

  // If user is authenticated, redirect to home page
  if (isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  // If user is not authenticated, allow access to the public route
  return children;
};

export default PublicRoute;