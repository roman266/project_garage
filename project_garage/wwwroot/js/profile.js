const { useState, useEffect } = React;

function Profile({ userId }) {
  const [profileData, setProfileData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isFriendRequestSent, setIsFriendRequestSent] = useState(false);
  const [alertMessage, setAlertMessage] = useState('');
  const [alertType, setAlertType] = useState(''); 

  useEffect(() => {
    axios.get(`/Profile/GetProfileData/${userId}`)
      .then(response => {
        setProfileData(response.data.info);
        setLoading(false);
      })
      .catch(err => {
        setError('Error fetching profile data');
        setLoading(false);
      });
  }, [userId]);

  const handleAddFriend = () => {
    axios.post(`/Friend/Send/${userId}`)
      .then(response => {
        if (response.data.success) {
          setIsFriendRequestSent(true);
          setAlertMessage('Friend request sent successfully!');
          setAlertType('success'); 

          setTimeout(() => {
            setAlertMessage('');
            setAlertType('');
          }, 3000);
        }
      })
      .catch(err => {
        setAlertMessage('Something went wrong. Please try again!');
        setAlertType('error'); 

        setTimeout(() => {
          setAlertMessage('');
          setAlertType('');
        }, 3000);
      });
  };

  if (loading) return React.createElement('div', null, 'Loading...');
  if (error) return React.createElement('div', null, error);

  return React.createElement('div', { id: 'profile' },

    React.createElement(
      'div',
      { className: 'logo' },
      React.createElement(
        'h1',
        null,
        'Sigm',
        React.createElement('img', {
          className: 'logo-sigma',
          src: '/img/sigma%202.svg',
          alt: '',
        })
      )),
    React.createElement('h2', null, profileData.nickname),
    React.createElement('p', null, profileData.description),
    React.createElement('p', null, `Friends: ${profileData.friendsCount}`),
    React.createElement('p', null, `Posts: ${profileData.postsCount}`),

    alertMessage && React.createElement(
      'div',
      { className: `alert ${alertType === 'error' ? 'alert-error' : 'alert-success'}` },
      alertMessage
    ),

    React.createElement(
      'button', 
      {
        disabled: isFriendRequestSent || !profileData.canAddFriend, 
        onClick: handleAddFriend
      },
      isFriendRequestSent ? 'Request Sent' : 'Add Friend'
    ),
    React.createElement('input', { type: 'text', placeholder: 'Enter your message' })
  );
}

function App() {
  const userId = window.location.pathname.split('/').pop();

  return React.createElement(Profile, { userId: userId });
}

ReactDOM.render(React.createElement(App), document.getElementById('root'));
