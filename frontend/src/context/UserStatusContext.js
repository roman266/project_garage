import React, { createContext, useState, useContext } from "react";

const UserStatusContext = createContext();

export const UserStatusProvider = ({ children }) => {
  const [userStatus, setUserStatus] = useState({});

  const updateUserStatus = (userId, status) => {
    setUserStatus((prevStatus) => ({
      ...prevStatus,
      [userId]: status,
    }));
  };

  return (
    <UserStatusContext.Provider value={{ userStatus, updateUserStatus }}>
      {children}
    </UserStatusContext.Provider>
  );
};

export const useUserStatus = () => {
  return useContext(UserStatusContext);
};
