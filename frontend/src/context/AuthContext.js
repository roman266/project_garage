import React, { createContext, useState, useContext, useEffect, useRef } from 'react';
import API, { checkAuthentication } from '../utils/apiClient';
import { API_URL } from "../constants";
import {
  setupSignalRConnection,
  startSignalRConnection,
  stopSignalRConnection,
  setupNotificationHandlers
} from '../services/noitificationService';
import { useUnreadMessages } from "./UnreadMessagesContext";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const connectionRef = useRef(null);
  // Отримуємо функцію increment з контексту UnreadMessagesContext
  const { increment } = useUnreadMessages();

  const fetchUserProfile = async () => {
    setIsLoading(true);
    try {
      const { isAuthenticated: authStatus, profile } = await checkAuthentication();
      setIsAuthenticated(authStatus);
      setUser(profile);

      // If user is authenticated, setup SignalR connection
      if (authStatus) {
        // Check if we already have an active connection before setting up a new one
        if (!connectionRef.current || connectionRef.current.state === "Disconnected") {
          setupSignalRConnection(connectionRef);
          // Передаємо функцію increment до setupNotificationHandlers
          setupNotificationHandlers(connectionRef.current, increment);
          await startSignalRConnection(connectionRef);
        }
      }

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
      await stopSignalRConnection(connectionRef);
      setUser(null);
      setIsAuthenticated(false);
    } catch (error) {
      console.error('Logout error:', error);
      await stopSignalRConnection(connectionRef);
      setUser(null);
      setIsAuthenticated(false);
    }
  };

  useEffect(() => {
    fetchUserProfile();
  }, []);

  useEffect(() => {
    if (isAuthenticated && Notification.permission !== "granted" && Notification.permission !== "denied") {
      Notification.requestPermission();
    }
  }, [isAuthenticated]);

  const contextValue = {
    user,
    isAuthenticated,
    isLoading,
    login,
    logout,
    fetchUserProfile,
    getSignalRConnection: () => connectionRef.current
  };

  return <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};