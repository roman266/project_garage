import React, { useEffect, useState } from "react";
import FriendRequestsReceived from "./FriendRequestsReceived";
import FriendRequestsSent from "./FriendRequestsSent";
import FriendsList from "./FriendsList";
import { Box, Tabs, Tab } from "@mui/material";
import { API_URL } from "../../constants";
import Paper from '@mui/material/Paper';


const Friends = () => {
  const [receivedRequests, setReceivedRequests] = useState([]);
  const [sentRequests, setSentRequests] = useState([]);
  const [friends, setFriends] = useState([]);
  const [tabIndex, setTabIndex] = useState(0); // 0 - received, 1 - sent

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
      console.error("Error loading friends:", error);
    }
  };

  const handleAcceptRequest = (request) => {
    setReceivedRequests((prev) => prev.filter((req) => req.id !== request.id));
    setFriends((prev) => [...prev, request]);
  };

  const handleCancelRequest = (canceledRequestId) => {
    setSentRequests((prev) => prev.filter((req) => req.id !== canceledRequestId));
  };

  const handleIncomingCancelRequest = (reqId) => {
    setReceivedRequests((prev) => prev.filter((req) => req.id !== reqId));
  };

  return (
    <Box
  sx={{
    width: "70%",
    height: "93.3vh",
    display: "flex",
    justifyContent: "center"
  }}
>
  <Box
    sx={{
      display: "grid",
      gridTemplateColumns: "100% 100%",  
      gap: "10px",
      height: "93.4vh",
    }}
  >
    {/* Левая колонка */}
    <Paper
      elevation={3}
      sx={{
        display: "flex",
        flexDirection: "column",
        height: "100%",
        borderRadius: 2,
        overflow: "hidden",
      }}
    >
      <Tabs
        value={tabIndex}
        onChange={(e, newIndex) => setTabIndex(newIndex)}
        variant="fullWidth"
        sx={{
          borderBottom: 1,
          borderColor: "divider",
          backgroundColor: "white",
        }}
      >
        <Tab label="Received" />
        <Tab label="Sent" />
      </Tabs>
      <Box sx={{ flex: 1, overflowY: "auto" }}>
        {tabIndex === 0 && (
          <FriendRequestsReceived
            received={receivedRequests}
            loadMore={loadMore}
            handleAcceptRequest={handleAcceptRequest}
            handleCancelRequest={handleIncomingCancelRequest}
          />
        )}
        {tabIndex === 1 && (
          <FriendRequestsSent
            sent={sentRequests}
            loadMore={loadMore}
            handleCancelRequest={handleCancelRequest}
          />
        )}
      </Box>
    </Paper>

    {/* Правая колонка */}
    <Paper
      elevation={3}
      sx={{
        display: "flex",
        flexDirection: "column",
        height: "100%",
        borderRadius: 2,
        overflow: "hidden",
      }}
    >
      <FriendsList friends={friends} loadMore={loadMore} sx={{ flex: 1 }} />
    </Paper>
  </Box>
</Box>


  );
};

export default Friends;
