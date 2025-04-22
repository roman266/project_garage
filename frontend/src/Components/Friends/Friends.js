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
  const [offsets, setOffsets] = useState({
    received: 0,
    sent: 0,
    friends: 0,
  });
  

  const loadMore = (type) => {
    fetchFriendsData(type, true);
  };
  

  useEffect(() => {
    fetchFriendsData();
  }, []);

  const fetchFriendsData = async (type = "all", isLoadMore = false) => {
    const limit = 20;
    const newOffsets = { ...offsets };
  
    try {
      if (type === "received" || type === "all") {
        const res = await fetch(`${API_URL}/api/friends/my-requests/incoming?limit=${limit}&offset=${offsets.received}`, {
          credentials: "include",
        });
        const data = await res.json();
        setReceivedRequests(prev =>
          isLoadMore ? [...prev, ...(data?.$values || [])] : data?.$values || []
        );
        newOffsets.received += limit;
      }
  
      if (type === "sent" || type === "all") {
        const res = await fetch(`${API_URL}/api/friends/my-requests/outcoming?limit=${limit}&offset=${offsets.sent}`, {
          credentials: "include",
        });
        const data = await res.json();
        setSentRequests(prev =>
          isLoadMore ? [...prev, ...(data?.$values || [])] : data?.$values || []
        );
        newOffsets.sent += limit;
      }
  
      if (type === "friends" || type === "all") {
        const res = await fetch(`${API_URL}/api/friends/my-requests/friends?limit=${limit}&offset=${offsets.friends}`, {
          credentials: "include",
        });
        const data = await res.json();
        setFriends(prev =>
          isLoadMore ? [...prev, ...(data?.$values || [])] : data?.$values || []
        );
        newOffsets.friends += limit;
      }
  
      setOffsets(newOffsets);
    } catch (error) {
      console.error("Error loading data:", error);
    }
  };
  

  const handleAcceptRequest = (request) => {
    setReceivedRequests((prev) => prev.filter((req) => req.id !== request.id));
    setFriends((prev) => [...prev, request]);
  };

  const handleCancelRequest = (canceledRequestId) => {
    setSentRequests((prev) => prev.filter((req) => req.id !== canceledRequestId));
  };

  const deleteFriend = (deletedFriendId) => {
    setFriends((prev) => prev.filter((req) => req.id !== deletedFriendId));
  }

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
            loadMore={() => loadMore("received")}
            handleAcceptRequest={handleAcceptRequest}
            handleCancelRequest={handleIncomingCancelRequest}
          />        
        )}
        {tabIndex === 1 && (
            <FriendRequestsSent
            sent={sentRequests}
            loadMore={() => loadMore("sent")}
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
      <FriendsList
      friends={friends}
      loadMore={() => loadMore("friends")}
      deleteFriend={deleteFriend}
    />

    </Paper>
  </Box>
</Box>


  );
};

export default Friends;
