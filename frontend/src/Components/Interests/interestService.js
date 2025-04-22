import axios from 'axios';
import { API_URL } from '../../constants';

export const fetchAllInterests = async () => {
    try {
        const response = await axios.get(`${API_URL}/api/interest/get-all`, {
            withCredentials: true
        });
        console.log('API Response for all interests:', response.data);
        return response.data; // Return the entire response data
    } catch (error) {
        console.error('Error fetching interests:', error);
        throw error;
    }
};

export const fetchUserInterests = async () => {
    try {
        const response = await axios.get(`${API_URL}/api/interest/my`, {
            withCredentials: true
        });
        console.log('API Response for user interests:', response.data);
        return response.data; // Return the entire response data
    } catch (error) {
        console.error('Error fetching user interests:', error);
        throw error;
    }
};

export const addUserInterests = async (interestIds) => {
    try {
        const response = await axios.post(`${API_URL}/api/interest/add`, interestIds, {
            withCredentials: true
        });
        return response.data;
    } catch (error) {
        console.error('Error adding user interests:', error);
        throw error;
    }
};

export const removeUserInterest = async (interestId) => {
    try {
        const response = await axios.delete(`${API_URL}/api/interest/remove/${interestId}`, {
            withCredentials: true
        });
        return response.data;
    } catch (error) {
        console.error('Error removing user interest:', error);
        throw error;
    }
};