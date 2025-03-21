import React, { useEffect, useState } from "react";
import { Container, Typography, Button } from "@mui/material";
import { useNavigate, useLocation } from "react-router-dom";
import API from "../utils/apiClient";
import { API_URL } from "../constants";

const EmailConfirmed = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [isConfirming, setIsConfirming] = useState(true);
  const [confirmationResult, setConfirmationResult] = useState({
    success: false,
    message: ""
  });

  useEffect(() => {
    const confirmEmail = async () => {
      try {
        // Get userId and code from URL query parameters
        const queryParams = new URLSearchParams(location.search);
        const userId = queryParams.get("userId");
        const code = queryParams.get("code");

        if (!userId || !code) {
          setConfirmationResult({
            success: false,
            message: "Invalid confirmation link. Missing parameters."
          });
          setIsConfirming(false);
          return;
        }

        // Send confirmation request
        const response = await fetch(`${API_URL}/api/account/confirmEmail?userId=${encodeURIComponent(userId)}&code=${encodeURIComponent(code)}`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          }
        });
        
        const data = await response.json();
        
        if (response.ok) {
          setConfirmationResult({
            success: true,
            message: data.message || "Email successfully confirmed!"
          });
        } else {
          throw new Error(data.message || "Failed to confirm email");
        }
      } catch (error) {
        setConfirmationResult({
          success: false,
          message: error.message || "Failed to confirm email. Please try again."
        });
      } finally {
        setIsConfirming(false);
      }
    };

    confirmEmail();
  }, [location]);

  return (
    <Container maxWidth="sm" style={{ textAlign: "center", marginTop: "50px" }}>
      {isConfirming ? (
        <Typography variant="h5">Confirming your email...</Typography>
      ) : (
        <>
          <Typography variant="h4" gutterBottom color={confirmationResult.success ? "primary" : "error"}>
            {confirmationResult.success ? "Email Confirmed!" : "Confirmation Failed"}
          </Typography>
          
          <Typography variant="body1" gutterBottom>
            {confirmationResult.message}
          </Typography>
          
          <Button 
            variant="contained" 
            color="primary" 
            style={{ marginTop: "20px" }}
            onClick={() => navigate("/login")}
          >
            Go to Login
          </Button>
        </>
      )}
    </Container>
  );
};

export default EmailConfirmed;
