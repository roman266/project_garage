import axios from "axios";
import { API_URL } from "../constants";

const API = axios.create({
  baseURL: `${API_URL}/api`,
  withCredentials: true,
});

let isRefreshing = false;
let refreshSubscribers = [];
let authFailureCallback = null;
let publicPaths = ['/login', '/registration', '/confirmEmail']; // Add public paths that don't need redirection

// Функція для встановлення колбеку при невдалій автентифікації
export const setAuthFailureCallback = (callback) => {
  authFailureCallback = callback;
};

// Function to check if current path is public
const isPublicPath = () => {
  const path = window.location.pathname;
  return publicPaths.some(publicPath => path === publicPath);
};

const subscribeToRefresh = (callback) => {
  refreshSubscribers.push(callback);
};

const onRefreshed = () => {
  refreshSubscribers.forEach((callback) => callback());
  refreshSubscribers = [];
};

// Функція для перевірки автентифікації користувача
export const checkAuthentication = async () => {
  try {
    const response = await API.get('/profile/me');
    return { isAuthenticated: true, profile: response.data.profile };
  } catch (error) {
    if (error.response?.status === 401) {
      try {
        // Спроба оновити токен
        await axios.post(`${API_URL}/api/account/refresh-token`, {}, { 
          withCredentials: true 
        });
        
        // Якщо оновлення токена успішне, повторно перевіряємо профіль
        const retryResponse = await API.get('/profile/me');
        return { isAuthenticated: true, profile: retryResponse.data.profile };
      } catch (refreshError) {
        // Якщо оновлення токена не вдалося, користувач не автентифікований
        // Don't trigger callback if we're on a public path
        if (authFailureCallback && !isPublicPath()) {
          authFailureCallback();
        }
        return { isAuthenticated: false, profile: null };
      }
    }
    return { isAuthenticated: false, profile: null };
  }
};

// Додаємо методи для роботи з реакціями
export const addReaction = async (reactionData) => {
  const response = await API.post("/reactions/add", reactionData);
  return response.data;
};

export const deleteReaction = async (reactionId) => {
  const response = await API.delete(`/reactions/delete/${reactionId}`);
  return response.data;
};

export const getEntityReactions = async (entityId) => {
  const response = await API.get(`/reactions/entity/${entityId}`);
  return response.data;
};

API.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve) => {
          subscribeToRefresh(() => {
            resolve(axios(originalRequest));
          });
        });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        await axios.post(`${API_URL}/api/account/refresh-token`, {}, { 
          withCredentials: true 
        });
        
        onRefreshed();
        isRefreshing = false;

        return axios(originalRequest);
      } catch (refreshError) {
        isRefreshing = false;
        
        // Don't redirect if we're on a public path
        if (authFailureCallback && !isPublicPath()) {
          authFailureCallback();
        } else if (!isPublicPath()) {
          // Запасний варіант, якщо колбек не встановлено і ми не на публічній сторінці
          window.location.href = "/login";
        }
        
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default API;