import React, { useState } from 'react'
import ChatWindow from '../Components/Chat/ChatWindow'
import ChatsList from '../Components/Chat/ChatList'

export default function MessagesPage() {
  const [selectedChatId, setSelectedChatId] = useState(null);
  
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
        <ChatsList onSelectChat={setSelectedChatId} />
        <ChatWindow selectedChatId={selectedChatId} />
      </div>
    </div>
  )
}

