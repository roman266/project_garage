import { toast, Bounce } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';

export const notifyUserAboutRecievedMessage = (conversationId, senderInfo) => {
    // Default values in case senderInfo is missing or incomplete
    const senderName = senderInfo?.userName || senderInfo?.firstName || "Someone";
    const profilePicture = senderInfo?.profilePicture || "https://via.placeholder.com/40";

    toast(
        <div style={{ display: 'flex', alignItems: 'center' }}>
            <div style={{ marginRight: '10px' }}>
                <img
                    src={profilePicture}
                    alt={`${senderName}'s avatar`}
                    style={{
                        width: '40px',
                        height: '40px',
                        borderRadius: '50%',
                        objectFit: 'cover'
                    }}
                />
            </div>
            <div style={{ flex: 1 }}>
                <div style={{ fontWeight: 'bold', marginBottom: '3px' }}>
                    {senderName} sent you a message
                </div>
                <button
                    onClick={() => window.location.href = `/messages/${conversationId}`}
                    style={{
                        marginTop: "5px",
                        backgroundColor: "#1F4A7C",
                        color: "#fff",
                        border: "none",
                        borderRadius: "4px",
                        padding: "5px 10px",
                        cursor: "pointer",
                        fontSize: "12px",
                        transition: "background-color 0.2s"
                    }}
                >
                    View Message
                </button>
            </div>
        </div>,
        {
            position: "top-left",
            autoClose: 5000,
            hideProgressBar: false,
            closeOnClick: false,
            pauseOnHover: true,
            draggable: true,
            theme: "colored",
            transition: Bounce,
            icon: false, // Disable default icon since we're using custom avatar
        }
    );
};