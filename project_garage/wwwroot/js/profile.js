const Profile = ({ userId }) => {
    const [profileData, setProfileData] = React.useState(null);

    React.useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await axios.get(`/Profile/GetProfileData/${userId}`);
                if (response.data.success) {
                    setProfileData(response.data.info);
                } else {
                    console.error("Failed to load profile data:", response.data.message);
                }
            } catch (error) {
                console.error("Error fetching profile data:", error);
            }
        };
        fetchData();
    }, [userId]);

    if (!profileData) {
        return <div>Loading...</div>;
    }

    return (
        <div>
            <h1>{profileData.Nickname}</h1>
            <p>{profileData.Description}</p>
            <p>Friends: {profileData.FriendsCount}</p>
            <p>Posts: {profileData.PostsCount}</p>
        </div>
    );
};

// Експорт компонента у глобальний об'єкт
window.Profile = Profile;


