import React from "react";
import { TextField, Button, Box, Typography, Container } from "@mui/material";
import { useFormik } from "formik";
import * as yup from "yup";

const validationSchema = yup.object({
  email: yup.string().email("Enter correct email").required("Email is required"),
  username: yup.string().min(3, "Username should be at least 3 characters").required("Username is required"),
  password: yup.string().min(8, "Password should be at least 8 characters").required("Password is required"),
  confirmPassword: yup
    .string()
    .oneOf([yup.ref("password"), null], "Passwords must match")
    .required("Confirm Password is required"),
});

const RegisterForm = () => {
  const formik = useFormik({
    initialValues: {
      email: "",
      username: "",
      password: "",
      confirmPassword: "",
    },
    validationSchema,
    onSubmit: async (values, { setSubmitting, setErrors }) => {
      try {
        const response = await fetch("http://localhost:5021/register", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(values),
        });
    
        const data = await response.json(); 
    
        if (!response.ok) {
          throw new Error(data.message || "Something went wrong");
        }
    
        console.log("Registration successful:", data);
      } catch (error) {
        console.error("Registration error:", error);
      } finally {
        setSubmitting(false);
      }
    },    
  });

  return (
    <Container maxWidth="xs">
      <Box
        sx={{
          backgroundColor: "#E0E0E0",
          padding: "30px",
          borderRadius: "12px",
          boxShadow: "2px 2px 15px rgba(0,0,0,0.3)",
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          width: "400px",
          margin: "auto",
          mt: "100px",
          fontFamily: "Roboto, sans-serif",
        }}
      >
        <Typography variant="h4" gutterBottom sx={{ fontFamily: "Roboto, sans-serif", color: "#1F4A7C" }}>
          Sign Up
        </Typography>
        <form onSubmit={formik.handleSubmit} style={{ width: "100%" }}>
          <Typography sx={{color: "#6A7788"}}>Email</Typography>
          <TextField
            fullWidth
            placeholder="Type your email"
            variant="outlined"
            type="email"
            name="email"
            value={formik.values.email}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            error={formik.touched.email && Boolean(formik.errors.email)}
            helperText={formik.touched.email && formik.errors.email}
            sx={{marginBottom: "10px"}}
          />
          <Typography sx={{color: "#6A7788"}}>User name</Typography>
          <TextField
            fullWidth
            placeholder="Type your user name"
            variant="outlined"
            name="username"
            value={formik.values.username}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            error={formik.touched.username && Boolean(formik.errors.username)}
            helperText={formik.touched.username && formik.errors.username}
            sx={{  marginBottom: "10px" }}
          />
          <Typography sx={{color: "#6A7788"}}>Password</Typography>
          <TextField
            fullWidth
            placeholder="Type your password"
            variant="outlined"
            type="password"
            name="password"
            value={formik.values.password}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            error={formik.touched.password && Boolean(formik.errors.password)}
            helperText={formik.touched.password && formik.errors.password}
            sx={{marginBottom: "10px"}}
          />
          <TextField
            fullWidth
            placeholder="Confirm Password"
            variant="outlined"
            type="password"
            name="confirmPassword"
            value={formik.values.confirmPassword}
            onChange={formik.handleChange}
            onBlur={formik.handleBlur}
            error={formik.touched.confirmPassword && Boolean(formik.errors.confirmPassword)}
            helperText={formik.touched.confirmPassword && formik.errors.confirmPassword}
            sx={{mb: "10px"}}
          />
          <Button
            type="submit"
            variant="contained"
            sx={{ backgroundColor: "#1F4A7C", color: "white", width: "100%", mt: 2 }}
            disabled={formik.isSubmitting}
          >
            {formik.isSubmitting ? "Signing Up..." : "Sign Up"}
          </Button>
        </form>
      </Box>
    </Container>
  );
};

export default RegisterForm;
