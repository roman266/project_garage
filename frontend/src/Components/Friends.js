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

  useEffect(() => {
    fetchFriendsData();
  }, []);

  const fetchFriendsData = async () => {
    try {
      const receivedRes = await fetch(`${API_URL}/api/friends/my-requests/incoming`, {
        method: "GET",
        credentials: "include"
    });
      const sentRes = await fetch(`${API_URL}/api/friends/my-requests/outcoming`, {
        method: "GET",
        credentials: "include"
    });
      const friendsRes = await fetch(`${API_URL}/api/friends/my-requests/friends`, {
        method: "GET",
        credentials: "include"
    });


      if (!receivedRes.ok || !sentRes.ok || !friendsRes.ok) {
        throw new Error("Ошибка загрузки данных");
      }

      const receivedData = await receivedRes.json();
      const sentData = await sentRes.json();
      const friendsData = await friendsRes.json();

      setReceivedRequests(receivedData.friendList || []);
      setSentRequests(sentData.friendList || []);
      setFriends(friendsData.friendList || []);
    } catch (error) {
      console.error("Ошибка загрузки друзей:", error);
    }
  };

  return (
  <Box sx={{ display: "flex", justifyContent: "center", width: "100%", height: "100%" }}>
  <Box sx={{
    display: "grid",
    gridTemplateColumns: "39% 39%",
    gap: 2,
    height: "100%",
    width: "90%",
    marginLeft: "17%",
  }}>
    {/* Левый блок */}
    <Box sx={{ height: "100%", display: "grid", gridTemplateRows: "50% 50%" }}>
      <FriendRequestsReceived received={receivedRequests} fetchFriendsData={fetchFriendsData} />
      <Divider sx={{ marginY: 2 }} />
      <FriendRequestsSent sent={sentRequests} fetchFriendsData={fetchFriendsData} />
    </Box>

    {/* Правый блок */}
    <Box sx={{ height: "50%", overflowY: "auto" }}>
      <FriendsList friends={friends} />
    </Box>
  </Box>
</Box>

  );
};

export default Friends;
