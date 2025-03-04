import React from "react";
import { Box, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";

const ProfileCard = () => {
  const navigate = useNavigate();

  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        backgroundColor: "white",
        padding: 2,
        borderRadius: "8px",
        width: "100%",
        cursor: "pointer",
        "&:hover": { backgroundColor: "#f5f5f5" },
      }}
      onClick={() => navigate("/my-profile")} // Перенаправлення на сторінку профілю
    >
      <Box
        component="img"
        src="/profile_card.svg"
        alt="Profile"
        sx={{
          width: 80,
          height: 80,
          borderRadius: "50%",
          objectFit: "cover",
          marginRight: 2,
        }}
      />
      <Box>
        <Typography variant="h6" sx={{ fontSize: "19px", fontFamily: "roboto", fontWeight: "regular" }}>
          Profile name
        </Typography>
        <Typography variant="body2" sx={{ color: "gray" }}>
          Some notation
        </Typography>
      </Box>
    </Box>
  );
};

export default ProfileCard;
