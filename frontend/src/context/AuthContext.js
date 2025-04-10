import React, { createContext, useState, useContext, useEffect } from 'react';
import API, { checkAuthentication } from '../utils/apiClient';
import * as signalR from "@microsoft/signalr";
import { API_URL } from "../constants";

// Create the context
const AuthContext = createContext(null);

// Provider component
export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [connection, setConnection] = useState(null);
  const [notifications, setNotifications] = useState([]);

  // SignalR connection setup
  const setupSignalRConnection = () => {
    if (connection) return connection;

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_URL}/chatHub`, {
        withCredentials: true,
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .build();

    // Set up message notification handler
    newConnection.on("RecievedMessage", (message) => {
      console.log("Received notification about new message:", message);
      
      // Add the notification to the state
      setNotifications(prev => [
        ...prev, 
        {
          id: Date.now(),
          type: 'message',
          data: message,
          read: false,
          timestamp: new Date()
        }
      ]);
      
      // You can also trigger a browser notification here if needed
      if (Notification.permission === "granted") {
        const sender = message.senderName || "Someone";
        new Notification("New Message", {
          body: `${sender}: ${message.text || "Sent you a message"}`,
          icon: "/notification-icon.png" // Add your notification icon path
        });
      }
    });

    setConnection(newConnection);
    return newConnection;
  };

  // Start SignalR connection
  const startSignalRConnection = async () => {
    const conn = connection || setupSignalRConnection();
    
    try {
      if (conn.state === signalR.HubConnectionState.Disconnected) {
        await conn.start();
        await conn.invoke("OnLoginConnection");
        console.log("SignalR Connected");
        return true;
      }
      return true;
    } catch (error) {
      console.error("SignalR Connection Error:", error);
      return false;
    }
  };

  // Stop SignalR connection
  const stopSignalRConnection = async () => {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
      try {
        await connection.invoke("LogOut");
        await connection.stop();
        console.log("SignalR Disconnected");
        return true;
      } catch (error) {
        console.error("SignalR Disconnection Error:", error);
        return false;
      }
    }
    return true;
  };

  // Mark notification as read
  const markNotificationAsRead = (notificationId) => {
    setNotifications(prev => 
      prev.map(notification => 
        notification.id === notificationId 
          ? { ...notification, read: true } 
          : notification
      )
    );
  };

  // Clear all notifications
  const clearNotifications = () => {
    setNotifications([]);
  };

  // Function to fetch user profile and update authentication state
  const fetchUserProfile = async () => {
    setIsLoading(true);
    try {
      const { isAuthenticated: authStatus, profile } = await checkAuthentication();
      setIsAuthenticated(authStatus);
      setUser(profile);
      
      // If user is authenticated, setup SignalR connection
      if (authStatus) {
        setupSignalRConnection();
        await startSignalRConnection();
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
      // Disconnect SignalR before clearing auth state
      await stopSignalRConnection();
      setUser(null);
      setIsAuthenticated(false);
    } catch (error) {
      console.error('Logout error:', error);
      // Still clear the auth state even if the API call fails
      await stopSignalRConnection();
      setUser(null);
      setIsAuthenticated(false);
    }
  };

  // Check authentication status on mount
  useEffect(() => {
    fetchUserProfile();
  }, []);

  // Request notification permission when authenticated
  useEffect(() => {
    if (isAuthenticated && Notification.permission !== "granted" && Notification.permission !== "denied") {
      Notification.requestPermission();
    }
  }, [isAuthenticated]);

  // Context value
  const contextValue = {
    user,
    isAuthenticated,
    isLoading,
    login,
    logout,
    fetchUserProfile,
    getSignalRConnection: () => connection,
    notifications,
    markNotificationAsRead,
    clearNotifications,
    unreadNotificationsCount: notifications.filter(n => !n.read).length
  };

  return <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>;
};

// Custom hook to use the auth context
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};