import React from "react";

import Friends from "../Components/Friends/Friends";
import FriendCard from "../Components/Friends/FriendCard";
import IncomingRequestCard from "../Components/Friends/IncomingRequestCard";
import OutcomingRequestCard from "../Components/Friends/OutcomingRequestCard";
import { API_URL } from "../constants";

const FriendsPage = () => {  
  return (
    <div>
      <Friends/>
    </div>
  );
};

export default FriendsPage;
