import React from "react";
import { Box, Typography, Avatar } from "@mui/material";

const ProfileCard = ({ profile }) => {
  if (!profile) {
    return (
      <Box sx={{ padding: 2, display: "flex", flexDirection: "column", alignItems: "center" }}>
        <Typography>Loading profile...</Typography>
      </Box>
    );
  }

  return (
    <Box sx={{ padding: 2, display: "flex", flexDirection: "column", alignItems: "center" }}>
      <Avatar
        src={profile.profilePicture}
        alt={profile.userName}
        sx={{ width: 80, height: 80, marginBottom: 1 }}
      />
      <Typography variant="h6" sx={{ fontWeight: "bold", color: "#365B87" }}>
        {profile.userName}
      </Typography>
      
      {(profile.firstName !== "None" || profile.lastName !== "None") && (
        <Typography variant="body1" sx={{ color: "#555" }}>
          {profile.firstName !== "None" ? profile.firstName : ""} {profile.lastName !== "None" ? profile.lastName : ""}
        </Typography>
      )}
      
      {profile.description !== "None" && (
        <Typography variant="body2" sx={{ color: "#777", textAlign: "center", mt: 1 }}>
          {profile.description}
        </Typography>
      )}
      
      <Box sx={{ display: "flex", justifyContent: "space-between", width: "100%", mt: 2 }}>
        <Box sx={{ textAlign: "center" }}>
          <Typography variant="body2" sx={{ fontWeight: "bold" }}>
            {profile.postsCount}
          </Typography>
          <Typography variant="caption" sx={{ color: "#777" }}>
            Posts
          </Typography>
        </Box>
        <Box sx={{ textAlign: "center" }}>
          <Typography variant="body2" sx={{ fontWeight: "bold" }}>
            {profile.friendsCount}
          </Typography>
          <Typography variant="caption" sx={{ color: "#777" }}>
            Friends
          </Typography>
        </Box>
        <Box sx={{ textAlign: "center" }}>
          <Typography variant="body2" sx={{ fontWeight: "bold" }}>
            {profile.reactionsCount}
          </Typography>
          <Typography variant="caption" sx={{ color: "#777" }}>
            Reactions
          </Typography>
        </Box>
      </Box>
    </Box>
  );
};

export default ProfileCard;