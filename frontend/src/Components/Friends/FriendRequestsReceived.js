import React from "react";
import { Box, Typography, Button } from "@mui/material";
import IncomingRequestCard from "./IncomingRequestCard";

const FriendRequestsReceived = ({ received, loadMore }) => (
  <Box sx={{ backgroundColor: "White", padding: 2, borderRadius: 2 }}>
    <Typography variant="h6" sx={{ fontWeight: 600, marginBottom: 1, color: "#345" }}>
      Friends request
    </Typography>

    {received?.length ? (
      <>
        {received.map((user) => (
          <IncomingRequestCard key={user.id} user={user}/>
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
