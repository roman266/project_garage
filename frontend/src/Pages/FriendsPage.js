import React from "react";

import Friends from "../Components/Friends/Friends";

const FriendsPage = () => {
  const receivedRequests = [
    { id: 1, name: "Alice", avatar: "https://via.placeholder.com/40" },
    { id: 2, name: "Bob", avatar: "https://via.placeholder.com/40" },
  ];
  
  const sentRequests = [
    { id: 3, name: "Charlie", avatar: "https://via.placeholder.com/40" },
  ];
  
  const friends = [
    { id: 4, name: "David", avatar: "https://via.placeholder.com/40" },
    { id: 5, name: "Emma", avatar: "https://via.placeholder.com/40" },
  ]
  
  return (
    <Friends receivedRequests={receivedRequests} sentRequests={sentRequests} friends={friends} />
  );
};

export default FriendsPage;
