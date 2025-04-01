import React from "react";
import { Box, Typography, Button } from "@mui/material";
import OutcomingRequestCard from "./OutcomingRequestCard";



const FriendRequestsSent = ({ sent, loadMore }) => (
  <Box sx={{ backgroundColor: "White", padding: 2, borderRadius: 2 }}>
    <Typography variant="h6" sx={{ fontWeight: 600, marginBottom: 1, color: "#345" }}>
      Sent requests
    </Typography>

    {sent?.length ? (
      <>
        {sent.map((user) => (
          <OutcomingRequestCard key={user.id} user={user}/>
        ))}
        {sent.length >= 20 && (
          <Button variant="outlined" fullWidth sx={{ mt: 2 }} onClick={loadMore}>
            Load More
          </Button>
        )}
      </>
    ) : (
      <Typography sx={{ color: "text.secondary", textAlign: "center", my: 2 }}>
        No friend requests sent
      </Typography>
    )}
  </Box>
);

export default FriendRequestsSent;
