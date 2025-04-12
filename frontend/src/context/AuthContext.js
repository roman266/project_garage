import React, { createContext, useState, useContext, useEffect } from 'react';
import API, { checkAuthentication } from '../utils/apiClient';
import * as signalR from "@microsoft/signalr";
import { API_URL } from "../constants";
import { notifyUserAboutRecievedMessage } from "../utils/notificationUtils";

// Create the context
const AuthContext = createContext(null);

// Provider component
export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [connection, setConnection] = useState(null);

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

    newConnection.on("ReceivedMessage", (message) => {

      let conversationId;

      if (typeof message === 'object' && message.arguments && Array.isArray(message.arguments)) {
        conversationId = message.arguments[0];
      } else if (typeof message === 'string') {
        try {
          const parsedMessage = JSON.parse(message);
          if (parsedMessage.arguments && Array.isArray(parsedMessage.arguments)) {
            conversationId = parsedMessage.arguments[0];
          }
        } catch (e) {
          conversationId = message;
        }
      } else {
        conversationId = message;
      }

      notifyUserAboutRecievedMessage(conversationId);
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

  // Function to fetch user profile and update authentication state
  const fetchUserProfile = async () => {
    setIsLoading(true);
    try {
      const { isAuthenticated: authStatus, profile } = await checkAuthentication();
      setIsAuthenticated(authStatus);
      setUser(profile);

      // If user is authenticated, setup SignalR connection
      if (authStatus) {
        // Check if we already have an active connection before setting up a new one
        if (!connection || connection.state === signalR.HubConnectionState.Disconnected) {
          setupSignalRConnection();
          await startSignalRConnection();
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
    getSignalRConnection: () => connection
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