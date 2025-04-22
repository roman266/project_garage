import React, { useEffect, useState } from "react";
import { Box, Typography, Button, Avatar, Container, TextField, Dialog, DialogActions, DialogContent, DialogTitle, Alert, Snackbar } from "@mui/material";
import axios from "axios";
import InterestForm from '../Components/Interests/InterestForm';

export default function MyProfilePage() {
  const [profile, setProfile] = useState({
    userName: "",
    email: "",
    firstName: "",
    lastName: "",
    description: "",
    profilePicture: "",
  });

  const handleAvatarClick = () => {
    const fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.accept = 'image/*';
    fileInput.onchange = handleAvatarChange;
    fileInput.click();
  };

  const handleAvatarChange = async (event) => {
    if (event.target.files && event.target.files[0]) {
      const file = event.target.files[0];
      const formData = new FormData();
      // Змінюємо ім'я поля з 'avatar' на 'file', щоб відповідало очікуванням бекенду
      formData.append('file', file);

      try {
        const response = await axios.post(`${API_BASE_URL}/api/profile/upload-avatar`, formData, {
          withCredentials: true,
          headers: {
            'Content-Type': 'multipart/form-data'
          }
        });

        // Оновлюємо аватар у стані профілю
        setProfile(prevProfile => ({
          ...prevProfile,
          profilePicture: response.data.avatarUrl
        }));

        setAlertInfo({
          open: true,
          message: "Avatar uploaded successfully!",
          severity: "success"
        });
      } catch (error) {
        console.error("Error uploading avatar", error);
        setAlertInfo({
          open: true,
          message: error.response?.data?.message || "Failed to upload avatar",
          severity: "error"
        });
      }
    }
  };
  const [open, setOpen] = useState(false);
  const [emailDialogOpen, setEmailDialogOpen] = useState(false);
  const [passwordDialogOpen, setPasswordDialogOpen] = useState(false);
  const [formData, setFormData] = useState({
    userName: "",
    email: "",
    firstName: "",
    lastName: "",
    description: "",
    password: "",
    newPassword: "",
    confirmPassword: "",
  });
  const API_BASE_URL = process.env.REACT_APP_HTTPS_API_URL;
  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}/api/profile/me`, {
          withCredentials: true,
        });


        setProfile(response.data.profile);
      } catch (error) {
        console.error("Error fetching profile data", error);
      }
    };

    fetchProfile();
  }, []);

  const handleClickOpen = () => {
    setFormData({
      userName: profile.userName,
      email: profile.email,
      firstName: profile.firstName,
      lastName: profile.lastName,
      description: profile.description,
      password: "",
    });
    setOpen(true);
  };

  const changeEmailOnClick = () => {
    setFormData({
      email: "",
      password: "",
    });
    setEmailDialogOpen(true);
  };

  const changePasswordOnClick = () => {
    setFormData({
      confirmationCode: "",
      newPassword: "",
    });
    setPasswordDialogOpen(true);
  };

  const handleSendPasswordResetEmail = async () => {
    try {
      await axios.post(`${API_BASE_URL}/api/profile/send-password-verify-email`, {}, {
        withCredentials: true,
      });

      setAlertInfo({
        open: true,
        message: "Confirmation code sent to your email!",
        severity: "success"
      });
    } catch (error) {
      console.error("Error sending confirmation code", error);
      setAlertInfo({
        open: true,
        message: error.response?.data?.message || "Failed to send confirmation code",
        severity: "error"
      });
    }
  };

  const [alertInfo, setAlertInfo] = useState({
    open: false,
    message: "",
    severity: "success" // 'success', 'error', 'warning', 'info'
  });

  const handleClose = () => {
    setOpen(false);
    setEmailDialogOpen(false);
    setPasswordDialogOpen(false);
  };

  const handleAlertClose = () => {
    setAlertInfo({ ...alertInfo, open: false });
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleEditSave = async () => {
    try {
      await axios.post(`${API_BASE_URL}/api/profile/me/edit`, formData, {
        withCredentials: true,
      });


      setProfile((prevProfile) => ({ ...prevProfile, ...formData }));
      handleClose();
      setAlertInfo({
        open: true,
        message: "Profile updated successfully!",
        severity: "success"
      });
    } catch (error) {
      console.error("Error saving profile data", error);
      setAlertInfo({
        open: true,
        message: error.response?.data?.message || "Failed to update profile",
        severity: "error"
      });
    }
  };

  const handleEmailSave = async () => {
    try {
      await axios.patch(`${API_BASE_URL}/api/profile/change-email`, formData, {
        withCredentials: true,
      });


      setProfile((prevProfile) => ({ ...prevProfile, email: formData.email }));
      handleClose();
      setAlertInfo({
        open: true,
        message: "Email changed successfully! Please check your inbox to confirm the new email.",
        severity: "success"
      });
    } catch (error) {
      console.error("Error saving profile data", error);
      setAlertInfo({
        open: true,
        message: error.response?.data?.message || "Failed to change email",
        severity: "error"
      });
    }
  };

  const handlePasswordSave = async () => {
    try {
      await axios.patch(`${API_BASE_URL}/api/profile/change-password`, {
        confirmationCode: formData.confirmationCode,
        newPassword: formData.newPassword
      }, {
        withCredentials: true,
      });

      handleClose();
      setAlertInfo({
        open: true,
        message: "Password changed successfully!",
        severity: "success"
      });
    } catch (error) {
      console.error("Error changing password", error);
      setAlertInfo({
        open: true,
        message: error.response?.data?.message || "Failed to change password",
        severity: "error"
      });
    }
  };

  const [interestsDialogOpen, setInterestsDialogOpen] = useState(false);

  const handleInterestsClick = () => {
    setInterestsDialogOpen(true);
  };

  const handleInterestsClose = () => {
    setInterestsDialogOpen(false);
  };

  return (
    <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", minHeight: "70vh", backgroundColor: "#365B87" }}>
      <Container maxWidth="sm" sx={{ backgroundColor: "white", padding: "40px", borderRadius: "12px", boxShadow: "2px 2px 15px rgba(0,0,0,0.3)", textAlign: "center" }}>
        <Typography variant="h5" sx={{ color: "#365B87", textAlign: "left", marginBottom: 2 }}>My profile</Typography>
        <Avatar
          src={profile.profilePicture}
          sx={{
            width: 80,
            height: 80,
            margin: "20px auto",
            backgroundColor: "black",
            cursor: "pointer",
            '&:hover': {
              opacity: 0.8,
              boxShadow: '0 0 10px rgba(0,0,0,0.3)'
            }
          }}
          onClick={handleAvatarClick}
        />
        <Typography variant="caption" sx={{ display: 'block', mb: 2, color: 'text.secondary' }}>
          Click on avatar to upload new image
        </Typography>

        <Box sx={{ textAlign: "left" }}>
          <Typography sx={{ fontWeight: "bold", color: "#365B87" }}>Username</Typography>
          <Typography sx={{ marginBottom: 2, borderBottom: "1px solid #ccc", display: "inline-block", width: "100%" }}>{profile.userName}</Typography>

          <Typography sx={{ fontWeight: "bold", color: "#365B87" }}>First Name</Typography>
          <Typography sx={{ marginBottom: 2, borderBottom: "1px solid #ccc", display: "inline-block", width: "100%" }}>{profile.firstName}</Typography>

          <Typography sx={{ fontWeight: "bold", color: "#365B87" }}>Last Name</Typography>
          <Typography sx={{ marginBottom: 2, borderBottom: "1px solid #ccc", display: "inline-block", width: "100%" }}>{profile.lastName}</Typography>

          <Typography sx={{ fontWeight: "bold", color: "#365B87" }}>Description</Typography>
          <Typography sx={{ marginBottom: 2, borderBottom: "1px solid #ccc", display: "inline-block", width: "100%" }}>{profile.description}</Typography>

          <Typography sx={{ fontWeight: "bold", color: "#365B87" }}>Email</Typography>
          <Typography sx={{ marginBottom: 2, borderBottom: "1px solid #ccc", display: "inline-block", width: "100%" }}>{profile.email}</Typography>
        </Box>
        <Box sx={{ display: "flex", justifyContent: "flex-end" }}>
          <Button variant="contained" sx={{ backgroundColor: "#1F4A7C", color: "white", marginTop: 2 }} onClick={handleClickOpen}>Edit</Button>
          <Button variant="contained" sx={{ backgroundColor: "#1F4A7C", color: "white", marginTop: 2, marginLeft: 2 }} onClick={changeEmailOnClick}>Change Email</Button>
          <Button variant="contained" sx={{ backgroundColor: "#1F4A7C", color: "white", marginTop: 2, marginLeft: 2 }} onClick={changePasswordOnClick}>Change Password</Button>
          <Button variant="contained" sx={{ backgroundColor: "#1F4A7C", color: "white", marginTop: 2, marginLeft: 2 }} onClick={handleInterestsClick}>Manage Interests</Button>
        </Box>
      </Container>

      <Dialog open={open} onClose={handleClose}>
        <DialogTitle>Edit Profile</DialogTitle>
        <DialogContent>
          <TextField margin="dense" name="userName" label="Username" fullWidth value={formData.userName} onChange={handleChange} />
          <TextField margin="dense" name="firstName" label="First Name" fullWidth value={formData.firstName} onChange={handleChange} />
          <TextField margin="dense" name="lastName" label="Last Name" fullWidth value={formData.lastName} onChange={handleChange} />
          <TextField margin="dense" name="description" label="Description" fullWidth value={formData.description} onChange={handleChange} />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} color="primary">Cancel</Button>
          <Button onClick={handleEditSave} color="primary">Save</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={emailDialogOpen} onClose={handleClose}>
        <DialogTitle>Change Email</DialogTitle>
        <DialogContent>
          <TextField margin="dense" name="password" label="Enter your password" type="password" fullWidth value={formData.password} onChange={handleChange} />
          <TextField margin="dense" name="email" label="Enter new email" fullWidth value={formData.email} onChange={handleChange} />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} color="primary">Cancel</Button>
          <Button onClick={handleEmailSave} color="primary">Save</Button>
        </DialogActions>
      </Dialog>

      {/* Add Snackbar with Alert */}
      <Snackbar
        open={alertInfo.open}
        autoHideDuration={6000}
        onClose={handleAlertClose}
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
      >
        <Alert
          onClose={handleAlertClose}
          severity={alertInfo.severity}
          sx={{ width: '100%' }}
        >
          {alertInfo.message}
        </Alert>
      </Snackbar>

      <Dialog open={passwordDialogOpen} onClose={handleClose}>
        <DialogTitle>Change Password</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
            <TextField
              margin="dense"
              name="confirmationCode"
              label="Confirmation Code"
              fullWidth
              value={formData.confirmationCode}
              onChange={handleChange}
            />
            <Button
              variant="contained"
              color="primary"
              sx={{ ml: 1, mt: 1, height: 40 }}
              onClick={handleSendPasswordResetEmail}
            >
              Send Email
            </Button>
          </Box>
          <TextField
            margin="dense"
            name="newPassword"
            label="New Password"
            type="password"
            fullWidth
            value={formData.newPassword}
            onChange={handleChange}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} color="primary">Cancel</Button>
          <Button onClick={handlePasswordSave} color="primary">Save</Button>
        </DialogActions>
      </Dialog>

      {/* Залишаємо тільки один Snackbar */}
      <Snackbar
        open={alertInfo.open}
        autoHideDuration={6000}
        onClose={handleAlertClose}
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
      >
        <Alert
          onClose={handleAlertClose}
          severity={alertInfo.severity}
          sx={{ width: '100%' }}
        >
          {alertInfo.message}
        </Alert>
      </Snackbar>
      <Dialog open={interestsDialogOpen} onClose={handleInterestsClose} maxWidth="sm" fullWidth>
        <DialogTitle>Manage Your Interests</DialogTitle>
        <DialogContent>
          <InterestForm userId={profile.id} />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleInterestsClose} color="primary">Close</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
