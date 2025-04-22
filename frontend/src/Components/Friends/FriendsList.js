import React from "react";
import { Box, Typography, Button } from "@mui/material";
import FriendCard from "./FriendCard";

const FriendsList = ({ friends, loadMore, deleteFriend }) => (
  <Box 
    sx={{ 
      backgroundColor: "White", 
      padding: 2, 
      borderRadius: 2, 
      maxHeight: "100%", // Ограничение по высоте
      overflowY: "auto" // Включаем вертикальный скролл
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
        padding: 1,
        mt: 6
      }}
    >
      Friends
    </Typography>

    {friends?.length ? (
      <>
        {friends.map((friend) => (
          <Box key={friend.id} sx={{ mb: 2 }}>
            <FriendCard user={friend} deleteFriend={deleteFriend} />
          </Box>
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
