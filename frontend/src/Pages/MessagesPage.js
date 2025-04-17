import React, { useState, useEffect, useRef } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import ChatWindow from '../Components/Chat/ChatWindow'
import ChatsList from '../Components/Chat/ChatList'

export default function MessagesPage() {
  const { conversationId } = useParams();
  const [selectedChatId, setSelectedChatId] = useState(null);
  const navigate = useNavigate();
  const chatListRef = useRef(null);

  // Set the selectedChatId when the conversationId from URL params changes
  useEffect(() => {
    if (conversationId) {
      console.log("URL conversationId changed to:", conversationId);
      setSelectedChatId(conversationId);
    } else {
      // Reset selectedChatId when on the /messages route without an ID
      setSelectedChatId(null);
    }
  }, [conversationId]);

  // Custom handler for chat selection that also updates the URL
  const handleSelectChat = (chatId) => {
    console.log("Chat selected:", chatId);
    // First update the state
    setSelectedChatId(chatId);
    // Then navigate to the URL
    navigate(`/messages/${chatId}`);
  };

  return (
    <div style={{
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      height: '100%',
      width: '100%'
    }}>
      <div style={{
        display: 'flex',
        gap: '20px',
        width: '1200px',
        height: '100%'
      }}>
        {/* Pass both the original onSelectChat function and the current selectedChatId */}
        <ChatsList
          ref={chatListRef}
          onSelectChat={handleSelectChat}
          currentChatId={selectedChatId}
        />
        <ChatWindow selectedChatId={selectedChatId} />
      </div>
    </div>
  )
}

