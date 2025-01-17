import React, { useState } from "react";

const RegisterForm = () => {
  const [formData, setFormData] = useState({
    userName: "",
    email: "",
    password: "",
    confirmPassword: "",
  });

  const [errors, setErrors] = useState({
    userName: "",
    email: "",
    password: "",
    confirmPassword: "",
  });

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const validate = () => {
    const newErrors = {};

    // Проверка на обязательность полей и подтверждение пароля
    if (!formData.userName) newErrors.userName = "User Name is required";
    if (!formData.email) newErrors.email = "Email is required";
    if (!formData.password) newErrors.password = "Password is required";
    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = "Passwords do not match";
    }

    return newErrors;
  };

  const fetchData = async () => {
  setLoading(true);
      setError(null);

      try {
        const response = await fetch({url: "http://localhost:5021/Account/Register", method: "POST", body: JSON.stringify(formData), headers: { "Content-Type": "application/json" }});

        if (!response.ok) {
          throw new Error(`Error: ${response.status} ${response.statusText}`);
        }

        const result = await response.json();
        setData(result);
      } catch (err) {
        if (err instanceof Error) {
          setError(err.message);
        } else {
          setError("An unknown error occurred");
        }
      } finally {
        setLoading(false);
      }
  };

  return (
    <form onSubmit={handleSubmit} className="signup-form">
      <div className="form-group">
        <label>UserName:</label>
        <input
          type="text"
          name="userName"
          value={formData.userName}
          onChange={handleChange}
          className={errors.userName ? "error" : ""}
        />
        {errors.userName && <span className="error-message">{errors.userName}</span>}
      </div>

      <div className="form-group">
        <label>Email:</label>
        <input
          type="email"
          name="email"
          value={formData.email}
          onChange={handleChange}
          className={errors.email ? "error" : ""}
        />
        {errors.email && <span className="error-message">{errors.email}</span>}
      </div>

      <div className="form-group">
        <label>Password:</label>
        <input
          type="password"
          name="password"
          value={formData.password}
          onChange={handleChange}
          className={errors.password ? "error" : ""}
        />
        {errors.password && <span className="error-message">{errors.password}</span>}
      </div>

      <div className="form-group">
        <label>Confirm Password:</label>
        <input
          type="password"
          name="confirmPassword"
          value={formData.confirmPassword}
          onChange={handleChange}
          className={errors.confirmPassword ? "error" : ""}
        />
        {errors.confirmPassword && <span className="error-message">{errors.confirmPassword}</span>}
      </div>

      <button type="submit" className="submit-btn">Sign Up</button>
    </form>
  );
};

export default RegisterForm;
