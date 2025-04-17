import { API_URL } from "../constants";
import React, { createContext, useContext, useEffect, useState, useCallback } from "react";
import axios from "axios";

const UnreadMessagesContext = createContext();

export const useUnreadMessages = () => {
    const context = useContext(UnreadMessagesContext);
    if (!context) {
        throw new Error('useUnreadMessages must be used within an UnreadMessagesProvider');
    }
    return context;
};

export const UnreadMessagesProvider = ({ children }) => {
    const [unreadCount, setUnreadCount] = useState(0);

    const fetchUnreadCount = useCallback(async () => {
        try {
            const response = await axios.get(`${API_URL}/api/message/my/unreaden`, {
                withCredentials: true,
            });
            setUnreadCount(response.data);
            console.log("Full response:", response.data);
        } catch (error) {
            console.error('Error fetching unread messages count:', error);
        }
    }, []);

    useEffect(() => {
        fetchUnreadCount();

        const interval = setInterval(fetchUnreadCount, 30000);

        return () => clearInterval(interval);
    }, [fetchUnreadCount]);

    const increment = () => setUnreadCount((prev) => prev + 1);
    const decrement = () => setUnreadCount((prev) => (prev > 0 ? prev - 1 : 0));
    const reset = (count) => setUnreadCount(count);

    return (
        <UnreadMessagesContext.Provider value={{ unreadCount, increment, decrement, reset, fetchUnreadCount }}>
            {children}
        </UnreadMessagesContext.Provider>
    );
};