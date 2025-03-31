import React, { useEffect, useState } from "react";
import FriendRequestsReceived from "./FriendRequestsReceived";
import FriendRequestsSent from "./FriendRequestsSent";
import FriendsList from "./FriendsList";
import { Box, Divider } from "@mui/material";
import { API_URL } from "../constants";

const Friends = () => {
  const [receivedRequests, setReceivedRequests] = useState([]);
  const [sentRequests, setSentRequests] = useState([]);
  const [friends, setFriends] = useState([]);
  
  // Add state for last IDs to implement pagination
  const [lastReceivedId, setLastReceivedId] = useState(null);
  const [lastSentId, setLastSentId] = useState(null);
  const [lastFriendId, setLastFriendId] = useState(null);

  useEffect(() => {
    fetchFriendsData();
  }, []);

  const fetchFriendsData = async (loadMore = false) => {
    try {
      // Use the last IDs for pagination if loading more
      const receivedRes = await fetch(
        `${API_URL}/api/friends/my-requests/incoming?limit=20${loadMore && lastReceivedId ? `&lastRequestId=${lastReceivedId}` : ''}`, 
        {
          method: "GET",
          credentials: "include"
        }
      );
      
      const sentRes = await fetch(
        `${API_URL}/api/friends/my-requests/outcoming?limit=20${loadMore && lastSentId ? `&lastRequestId=${lastSentId}` : ''}`, 
        {
          method: "GET",
          credentials: "include"
        }
      );
      
      const friendsRes = await fetch(
        `${API_URL}/api/friends/my-requests/friends${loadMore && lastFriendId ? `?lastRequestId=${lastFriendId}` : ''}`, 
        {
          method: "GET",
          credentials: "include",
        }
      );

      if (!receivedRes.ok || !sentRes.ok || !friendsRes.ok) {
        throw new Error("Помилка отримання даних");
      }

      const receivedData = await receivedRes.json();
      const sentData = await sentRes.json();
      const friendsData = await friendsRes.json();

      console.log("Received data:", receivedData);
      console.log("Sent data:", sentData);
      console.log("Friends data:", friendsData);

      // Process the data arrays
      const receivedArray = Array.isArray(receivedData) ? receivedData : [];
      const sentArray = Array.isArray(sentData) ? sentData : [];
      const friendsArray = Array.isArray(friendsData) ? friendsData : [];
      
      // Update last IDs for pagination if there are items
      if (receivedArray.length > 0) {
        setLastReceivedId(receivedArray[receivedArray.length - 1].Id);
      }
      
      if (sentArray.length > 0) {
        setLastSentId(sentArray[sentArray.length - 1].Id);
      }
      
      if (friendsArray.length > 0) {
        setLastFriendId(friendsArray[friendsArray.length - 1].Id);
      }

      // If loading more, append to existing data, otherwise replace
      if (loadMore) {
        setReceivedRequests(prev => [...prev, ...receivedArray]);
        setSentRequests(prev => [...prev, ...sentArray]);
        setFriends(prev => [...prev, ...friendsArray]);
      } else {
        setReceivedRequests(receivedArray);
        setSentRequests(sentArray);
        setFriends(friendsArray);
      }
    } catch (error) {
      console.error("Помилка завантаження друзів:", error);
    }
  };

  // Function to load more items
  const loadMore = () => {
    fetchFriendsData(true);
  };

  return (
    <Box sx={{ display: "flex", justifyContent: "center", width: "100%", height: "100%" }}>
      <Box sx={{
        display: "grid",
        gridTemplateColumns: "39% 39%",
        gap: 2,
        height: "100%",
        width: "90%",
        marginLeft: "17%"
      }}>
        {/* Левый блок */}
        <Box sx={{ height: "100%", display: "grid", gridTemplateRows: "50% 50%" }}>
          <FriendRequestsReceived 
            received={receivedRequests} 
            fetchFriendsData={fetchFriendsData} 
            loadMore={loadMore} 
          />
          <Divider sx={{ marginY: 2, position: "absolute" }} />
          <FriendRequestsSent 
            sent={sentRequests} 
            fetchFriendsData={fetchFriendsData} 
            loadMore={loadMore} 
          />
        </Box>

        {/* Правый блок */}
        <FriendsList 
          friends={friends} 
          loadMore={loadMore} 
        />
      </Box>
    </Box>
  );
};

export default Friends;
