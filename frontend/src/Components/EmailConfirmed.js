import React from "react";
import { Container, Typography, Button, TextField } from "@mui/material";
import { Formik, Form, Field } from "formik";
import * as Yup from "yup";
import { useNavigate } from "react-router-dom";
import { API_URL } from "../constants";


const validationSchema = Yup.object({
  email: Yup.string().email("Невірний формат email").required("Обов'язкове поле"),
  password: Yup.string().min(6, "Мінімум 6 символів").required("Обов'язкове поле"),
});

const EmailConfirmed = () => {
  const navigate = useNavigate();

  return (
    <Container maxWidth="sm" style={{ textAlign: "center", marginTop: "50px" }}>
      <Typography variant="h4" gutterBottom>
        Email успішно підтверджено!
      </Typography>
      <Typography variant="body1" gutterBottom>
        Тепер ви можете увійти в свій акаунт.
      </Typography>
      <Formik
        initialValues={{ email: "", password: "" }}
        validationSchema={validationSchema}
        onSubmit={(values) => {
          console.log("Login Data:", values);
          navigate("/dashboard");
        }}
      >
        {({ errors, touched }) => (
          <Form style={{ marginTop: "20px" }}>
            <Field
              as={TextField}
              name="email"
              label="Email"
              fullWidth
              margin="normal"
              error={touched.email && Boolean(errors.email)}
              helperText={touched.email && errors.email}
            />
            <Field
              as={TextField}
              name="password"
              label="Пароль"
              type="password"
              fullWidth
              margin="normal"
              error={touched.password && Boolean(errors.password)}
              helperText={touched.password && errors.password}
            />
            <Button type="submit" variant="contained" color="primary" fullWidth>
              Увійти
            </Button>
          </Form>
        )}
      </Formik>
    </Container>
  );
};

export default EmailConfirmed;
