import React, { useState } from 'react'
import ChatWindow from '../Components/ChatWindow'
import ChatsList from '../Components/ChatList'

export default function MessagesPage() {
  const [selectedChat, setSelectedChat] = useState(null);
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
        <ChatsList onSelectChat={setSelectedChat} />
        <ChatWindow selectedChat={selectedChat} />
      </div>
    </div>
  )
}

