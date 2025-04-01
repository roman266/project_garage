import React from "react";
import { Box, Typography, Button } from "@mui/material";
import FriendCard from "./FriendCard";

const FriendsList = ({ friends, loadMore }) => (
  <Box sx={{ backgroundColor: "White", padding: 2, borderRadius: 2 }}>
    <Typography variant="h6" sx={{ fontWeight: 600, marginBottom: 1, color: "#345" }}>
      Friends
    </Typography>

    {friends?.length ? (
      <>
        {friends.map((friend) => (
          <FriendCard key={friend.id} user={friend} />
        ))}
        {friends.length >= 20 && (
          <Button variant="outlined" fullWidth sx={{ mt: 2 }} onClick={loadMore}>
            Load More
          </Button>
        )}
      </>
    ) : (
      <Typography sx={{ color: "text.secondary", textAlign: "center", my: 2 }}>
        You don't have any friends yet
      </Typography>
    )}
  </Box>
);

export default FriendsList;
