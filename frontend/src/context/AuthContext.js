import React, { createContext, useState, useContext, useEffect } from 'react';
import API, { checkAuthentication } from '../utils/apiClient';

// Create the context
const AuthContext = createContext(null);

// Provider component
export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  // Function to fetch user profile and update authentication state
  const fetchUserProfile = async () => {
    setIsLoading(true);
    try {
      const { isAuthenticated: authStatus, profile } = await checkAuthentication();
      setIsAuthenticated(authStatus);
      setUser(profile);
      return profile;
    } catch (error) {
      console.error('Error checking authentication:', error);
      setIsAuthenticated(false);
      setUser(null);
      return null;
    } finally {
      setIsLoading(false);
    }
  };

  // Login function
  const login = async (credentials) => {
    try {
      setIsLoading(true);
      const response = await API.post('/account/login', credentials);
      console.log("Login response:", response.data);
      
      // After successful login, fetch user profile to update auth state
      await fetchUserProfile();
      
      return { success: true, data: response.data };
    } catch (error) {
      console.error('Login error:', error);
      setIsAuthenticated(false);
      return { 
        success: false, 
        error: error.response?.data?.message || 'Login failed' 
      };
    } finally {
      setIsLoading(false);
    }
  };

  // Logout function
  const logout = async () => {
    try {
      await API.post('/account/logout');
      setUser(null);
      setIsAuthenticated(false);
    } catch (error) {
      console.error('Logout error:', error);
      // Still clear the auth state even if the API call fails
      setUser(null);
      setIsAuthenticated(false);
    }
  };

  // Check authentication status on mount
  useEffect(() => {
    fetchUserProfile();
  }, []);

  // Context value
  const contextValue = {
    user,
    isAuthenticated,
    isLoading,
    login,
    logout,
    fetchUserProfile
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