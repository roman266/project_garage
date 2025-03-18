import axios from "axios";
import { API_URL } from "../constants";

// Flag to prevent multiple refresh attempts
let isRefreshing = false;
let refreshFailed = false;

// Create axios instance with interceptors for token refresh
const api = axios.create({
  baseURL: API_URL,
  withCredentials: true
});

// Add response interceptor for handling token refresh
api.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config;
    
    // For debugging
    console.log("API Error:", error.response?.status, originalRequest.url);
    
    // If this is a refresh token request that failed
    if (originalRequest.url === '/api/account/refresh-token') {
      console.log("Refresh token request failed");
      window.location.href = '/login';
      return Promise.reject(error);
    }
    
    // If the error is 401 and we haven't tried to refresh the token yet
    if (error.response?.status === 401 && !originalRequest._retry && !isRefreshing) {
      console.log("Attempting to refresh token");
      originalRequest._retry = true;
      isRefreshing = true;
      
      try {
        // Attempt to refresh the token
        const refreshResponse = await axios.post(`${API_URL}/api/account/refresh-token`, {}, { 
          withCredentials: true 
        });
        
        console.log("Token refresh successful", refreshResponse.status);
        
        // Reset the flag after successful refresh
        isRefreshing = false;
        
        // Retry the original request with the new token
        return api(originalRequest);
      } catch (refreshError) {
        // If refresh fails, mark as failed and redirect to login
        console.error("Token refresh failed:", refreshError);
        isRefreshing = false;
        refreshFailed = true;
        
        // Clear the failed flag after a delay
        setTimeout(() => {
          refreshFailed = false;
        }, 5000);
        
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }
    
    // For other errors, just reject the promise
    return Promise.reject(error);
  }
);

export default api;