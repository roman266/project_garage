const { useState, useEffect } = React;

function Profile() {
  const [profileData, setProfileData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    axios.get('/Profile/GetProfileData/341e3e0a-2a4c-4ec0-b0f0-e03f4b8906dd')
      .then(response => {
        setProfileData(response.data.info);
        setLoading(false);
      })
      .catch(err => {
        setError('Error fetching profile data');
        setLoading(false);
      });
  }, []);

  if (loading) return React.createElement('div', null, 'Loading...');
  if (error) return React.createElement('div', null, error);

  return React.createElement('div', { id: 'profile' },
    React.createElement('h1', null, profileData.nickname),
    React.createElement('p', null, profileData.description),
    React.createElement('p', null, `Friends: ${profileData.friendsCount}`),
    React.createElement('p', null, `Posts: ${profileData.postsCount}`),
    React.createElement('button', { disabled: !profileData.canAddFriend },
      profileData.canAddFriend ? 'Add Friend' : 'Already Friends'
    )
  );
}

ReactDOM.render(React.createElement(Profile), document.getElementById('root'));
