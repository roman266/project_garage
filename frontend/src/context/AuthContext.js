import React, { createContext, useState, useContext, useEffect } from 'react';
import api from '../utils/apiClient';
import { useNavigate } from 'react-router-dom';

// Create the context
const AuthContext = createContext(null);

// Provider component
export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [refreshAttempted, setRefreshAttempted] = useState(false);
  const navigate = useNavigate();

  // Function to fetch user profile
  const fetchUserProfile = async () => {
    try {
      setIsLoading(true);
      console.log("Fetching user profile...");
      const response = await api.get('/api/profile/me');
      console.log("Profile response:", response.data);
      
      // Store the complete profile data
      setUser(response.data.profile);
      setIsAuthenticated(true);
      return response.data.profile;
    } catch (error) {
      console.error('Error fetching user profile:', error);
      // Check if this is a 401 error
      if (error.response && error.response.status === 401) {
        console.log("Unauthorized, setting auth state to false");
        setIsAuthenticated(false);
        setUser(null);
      } else {
        // For other errors, don't change authentication state
        console.log("Error wasn't a 401, keeping current auth state");
      }
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  // Login function
  const login = async (credentials) => {
    try {
      setIsLoading(true);
      const response = await api.post('/api/account/login', credentials);
      console.log("Login response:", response.data);
      
      // Fetch user profile after successful login
      const profile = await fetchUserProfile();
      console.log("Profile after login:", profile);
      
      // Make sure isAuthenticated is set to true
      setIsAuthenticated(true);
      
      return { success: true, data: response.data };
    } catch (error) {
      console.error('Login error:', error);
      setIsAuthenticated(false);
      return { success: false, error: error.response?.data?.message || 'Login failed' };
    } finally {
      setIsLoading(false);
    }
  };

  // Logout function
  const logout = async () => {
    try {
      await api.post('/api/account/logout');
      setUser(null);
      setIsAuthenticated(false);
      setRefreshAttempted(false); // Reset refresh attempt on logout
    } catch (error) {
      console.error('Logout error:', error);
      // Still clear the auth state even if the API call fails
      setUser(null);
      setIsAuthenticated(false);
      setRefreshAttempted(false); // Reset refresh attempt on logout
    }
  };

  // Function to handle token refresh
  const refreshToken = async () => {
    if (refreshAttempted) {
      console.log("Token refresh already attempted, not trying again");
      return false;
    }
    
    try {
      setRefreshAttempted(true);
      console.log("Attempting to refresh token");
      await api.post('/api/account/refresh-token');
      // If successful, try to fetch profile again
      const profile = await fetchUserProfile();
      return !!profile;
    } catch (error) {
      console.error("Token refresh failed:", error);
      setIsAuthenticated(false);
      setUser(null);
      return false;
    }
  };

  // Check authentication status on mount
  useEffect(() => {
    const checkAuth = async () => {
      try {
        await fetchUserProfile();
      } catch (error) {
        console.error("Failed to fetch profile:", error);
      }
    };
    
    checkAuth();
  }, []);

  // Context value
  const contextValue = {
    user,
    isAuthenticated,
    isLoading,
    login,
    logout,
    fetchUserProfile,
    refreshToken
  };

  return <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>;
};

// Custom hook to use the auth context
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};