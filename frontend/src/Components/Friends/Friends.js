import React, { useEffect, useState } from "react";
import FriendRequestsReceived from "./FriendRequestsReceived";
import FriendRequestsSent from "./FriendRequestsSent";
import FriendsList from "./FriendsList";
import { Box, Divider } from "@mui/material";
import { API_URL } from "../../constants";

const Friends = () => {
  const [receivedRequests, setReceivedRequests] = useState([]);
  const [sentRequests, setSentRequests] = useState([]);
  const [friends, setFriends] = useState([]);

  const loadMore = () => {
    console.log("loadMore");
  };

  useEffect(() => {
    fetchFriendsData();
  }, []);

  const fetchFriendsData = async () => {
    try {
      const receivedRes = await fetch(`${API_URL}/api/friends/my-requests/incoming?limit=20`, {
        method: "GET",
        credentials: "include",
      });
      const sentRes = await fetch(`${API_URL}/api/friends/my-requests/outcoming?limit=20`, {
        method: "GET",
        credentials: "include",
      });
      const friendsRes = await fetch(`${API_URL}/api/friends/my-requests/friends`, {
        method: "GET",
        credentials: "include",
      });

      const receivedData = await receivedRes.json();
      const sentData = await sentRes.json();
      const friendsData = await friendsRes.json();

      setReceivedRequests(receivedData?.$values || []);
      setSentRequests(sentData?.$values || []);
      setFriends(friendsData?.$values || []);
    } catch (error) {
      console.error("Ошибка загрузки друзей:", error);
    }
  };

  const handleAcceptRequest = (request) => {
    setReceivedRequests((prev) => prev.filter((req) => req.id !== request.id))
    setFriends((prev) => [...prev, request]);
  };
  
  const handleCancelRequest = (canceledRequestId) => {
    setSentRequests((prev) => prev.filter((req) => req.id !== canceledRequestId));
  };

  const handleIncomingCancelRequest = (reqId) => {
    setReceivedRequests((prev) => prev.filter((req) => req.id !== reqId))
  }

  return (
    <Box sx={{ display: "flex", justifyContent: "center", width: "100%", height: "100vh" }}>
      <Box
        sx={{
          display: "grid",
          gridTemplateColumns: "39% 39%",
          width: "90%",
          marginLeft: "17%",
        }}
      >
        <Box sx={{ display: "grid", height: "100%", gridTemplateRows: "47% 47%" }}>
          <FriendRequestsReceived received={receivedRequests} loadMore={loadMore} handleAcceptRequest={handleAcceptRequest}  handleCancelRequest={handleIncomingCancelRequest}/>
          <FriendRequestsSent sent={sentRequests} loadMore={loadMore}  handleCancelRequest={handleCancelRequest}/>
        </Box>

        <FriendsList friends={friends} loadMore={loadMore} sx={{ flex: 1 }} />
      </Box>
    </Box>
  );
};

export default Friends;
