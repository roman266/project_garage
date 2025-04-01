import React from "react";
import { Box, Typography, Button } from "@mui/material";
import OutcomingRequestCard from "./OutcomingRequestCard";



const FriendRequestsSent = ({ sent, loadMore, handleCancelRequest }) => (
  <Box 
    sx={{ 
      backgroundColor: "White", 
      padding: 2, 
      borderRadius: 2, 
      maxHeight: "100%", 
      overflowY: "auto" 
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
  Sent requests
</Typography>

    {sent?.length ? (
      <>
        {sent.map((user) => (
          <Box key={user.id} sx={{ mb: 1.5 }}>
            <OutcomingRequestCard user={user} handleCancelRequest={handleCancelRequest}/>
          </Box>
        ))}
      </>
    ) : (
      <Typography sx={{ color: "text.secondary", textAlign: "center", my: 2 }}>
        No friend requests sent
      </Typography>
    )}
  </Box>
);

export default FriendRequestsSent;
