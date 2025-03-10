import { Navigate } from "react-router-dom";

const ProtectedRoute = ({ children }) => {
const token = 1 /*(localStorage.getItem("token");*/

  if (!token) {
    return <Navigate to="/login" />;
  }
  return children;
};

export default ProtectedRoute;
