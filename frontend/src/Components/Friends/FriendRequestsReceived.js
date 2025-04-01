import React from "react";
import { Box, Typography, Button } from "@mui/material";
import IncomingRequestCard from "./IncomingRequestCard";

const FriendRequestsReceived = ({ received, loadMore, handleAcceptRequest, handleCancelRequest }) => (
  <Box 
    sx={{ 
      backgroundColor: "White", 
      padding: 2, 
      borderRadius: 2, 
      maxHeight: "100%", // Ограничение по высоте (можно менять)
      overflowY: "auto" // Включаем скролл
    }}
  >
    <Typography 
      variant="h6" 
      sx={{ 
        fontWeight: 600, 
        marginBottom: 1, 
        color: "#345", 
        position: "sticky", 
        top: 0, 
        backgroundColor: "White", 
        zIndex: 1, 
        padding: 1
      }}
    >
      Friends request
    </Typography>

    {received?.length ? (
      <>
        {received.map((user) => (
          <Box key={user.id} sx={{ mb: 1.5 }}>
            <IncomingRequestCard 
              user={user} 
              handleAcceptRequest={handleAcceptRequest} 
              handleCancelRequest={handleCancelRequest} 
            />
          </Box>
        ))}
        {received.length >= 20 && (
          <Button variant="outlined" fullWidth sx={{ mt: 2 }} onClick={loadMore}>
            Load More
          </Button>
        )}
      </>
    ) : (
      <Typography sx={{ color: "text.secondary", textAlign: "center", my: 2 }}>
        No friend requests received
      </Typography>
    )}
  </Box>
);


export default FriendRequestsReceived;
