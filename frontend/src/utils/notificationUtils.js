import { toast, Bounce } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';

export const notifyUserAboutRecievedMessage = (conversationId) => {
    toast(
        <div>
            📩 You got a new message!
            <div>
                <button
                    onClick={() => window.location.href = `/messages/${conversationId}`}
                    style={{
                        marginTop: "5px",
                        backgroundColor: "#007bff",
                        color: "#fff",
                        border: "none",
                        borderRadius: "4px",
                        padding: "5px 10px",
                        cursor: "pointer"
                    }}
                >
                    View Message
                </button>
            </div>
        </div>,
        {
            position: "top-left",
            autoClose: 5000,
            hideProgressBar: true,
            closeOnClick: false,
            pauseOnHover: true,
            draggable: true,
            progress: 0,
            theme: "colored",
            transition: Bounce,
        }
    );
};