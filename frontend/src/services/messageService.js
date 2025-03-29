export const createMessage = (text, senderId, currentUserId, isSystem = false) => {
  const msgId = Date.now().toString();
  
  if (isSystem) {
    return {
      id: msgId,
      text: text,
      isSystem: true,
      sendedAt: new Date().toISOString()
    };
  }
  
  return {
    id: msgId,
    senderId: senderId,
    text: text,
    isCurrentUser: senderId === currentUserId,
    sendedAt: new Date().toISOString()
  };
};

export const cacheMessages = (chatId, messages) => {
  try {
    sessionStorage.setItem(`chat_messages_${chatId}`, JSON.stringify({
      messages: messages,
      timestamp: Date.now()
    }));
  } catch (error) {
    console.error("Error caching messages:", error);
  }
};

export const getCachedMessages = (chatId) => {
  try {
    const cachedData = sessionStorage.getItem(`chat_messages_${chatId}`);
    if (cachedData) {
      const { messages, timestamp } = JSON.parse(cachedData);
      // Check if cache is not too old (e.g., 30 minutes)
      if (Date.now() - timestamp < 30 * 60 * 1000) {
        return messages;
      }
    }
    return null;
  } catch (error) {
    console.error("Error getting cached messages:", error);
    return null;
  }
};