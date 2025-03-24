import React, { useEffect } from "react";
import { TextField, Button, Box, Typography, Container } from "@mui/material";
import { useFormik } from "formik";
import * as yup from "yup";
import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";

const validationSchema = yup.object({
  email: yup.string().email("Enter correct email").required("Email is required"),
  password: yup.string().min(8, "Password should be at least 8 characters").required("Password is required"),
});

const LoginForm = () => {
  const { login, isAuthenticated, isLoading } = useAuth();
  const navigate = useNavigate();

  // Redirect if already authenticated
  useEffect(() => {
    if (isAuthenticated && !isLoading) {
      navigate('/', { replace: true });
    }
  }, [isAuthenticated, isLoading, navigate]);

  const formik = useFormik({
    initialValues: {
      email: "",
      password: "",
    },
    validationSchema,
    onSubmit: async (values, { setSubmitting, setErrors }) => {
      try {
        const result = await login(values);
        
        if (result.success) {
          console.log("Login successful");
          navigate('/', { replace: true });
        } else {
          setErrors({ email: result.error || "Invalid email or password" });
        }
      } catch (error) {
        console.error("Login error:", error);
        setErrors({ email: "Invalid email or password" });
      } finally {
        setSubmitting(false);
      }
    },
  });

  return (
    <Container maxWidth="md">
      <Box
        sx={{
          backgroundColor: "#DFDFDF",
          padding: "30px",
          borderRadius: "12px",
          boxShadow: "2px 2px 15px rgba(0,0,0,0.3)",
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          width: "450px",
          margin: "auto",
          mt: "100px",
          fontFamily: "Roboto, sans-serif",
        }}
      >
        <Typography variant="h4" gutterBottom sx={{ display: "flex", alignItems: "center", fontFamily: "Roboto, sans-serif", color: "#365B87" }}>
          Sigm<img src="/sigma_2.svg" alt="Sigma" style={{ height: 40, marginLeft: 5 }} />
        </Typography>
        <form onSubmit={formik.handleSubmit} style={{ width: "100%" }}>
          <TextField
            fullWidth
            label="Email"
            variant="outlined"
            type="email"
            name="email"
            value={formik.values.email}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            error={formik.touched.email && Boolean(formik.errors.email)}
            helperText={formik.touched.email && formik.errors.email}
            margin="normal"
            sx={{ fontFamily: "Roboto, sans-serif" }}
          />
          <TextField
            fullWidth
            label="Password"
            variant="outlined"
            type="password"
            name="password"
            value={formik.values.password}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            error={formik.touched.password && Boolean(formik.errors.password)}
            helperText={formik.touched.password && formik.errors.password}
            margin="normal"
            sx={{ fontFamily: "Roboto, sans-serif" }}
          />
          <Box sx={{ display: "flex", justifyContent: "space-between", mt: 2, gap: 2 }}>
            <Button
              type="submit"
              variant="contained"
              sx={{ backgroundColor: "#1F4A7C", color: "white", flex: 1, fontFamily: "Roboto, sans-serif" }}
              disabled={formik.isSubmitting}
            >
              {formik.isSubmitting ? "Signing In..." : "Login"}
            </Button>
            <Button
              href="/registration"
              variant="contained"
              sx={{ backgroundColor: "#1F4A7C", color: "white", flex: 1, fontFamily: "Roboto, sans-serif" }}
            >
              Sign Up
            </Button>
          </Box>
        </form>
        <Typography variant="body1" sx={{ marginY: 2, fontFamily: "Roboto, sans-serif", color: "#365B87", fontWeight: "medium" }}>Login with</Typography>
        <Box sx={{ display: "flex", gap: 2 }}>
          <img src="/google_icon.svg" alt="Google" style={{ height: 40, cursor: "pointer" }} />
          <img src="/facebook_icon.svg" alt="Facebook" style={{ height: 40, cursor: "pointer" }} />
          <img src="/github_icon.svg" alt="GitHub" style={{ height: 40, cursor: "pointer" }} />
        </Box>
      </Box>
    </Container>
  );
};

export default LoginForm;
