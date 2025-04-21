import React, { useState, useEffect } from 'react';
import { fetchUserInterests, removeUserInterest, fetchAllInterests, addUserInterests } from './interestService';
import {
    Box, Typography, List, ListItem, ListItemText, IconButton, Alert, Paper,
    Container, Button, Dialog, DialogTitle, DialogContent, DialogActions,
    Checkbox, FormControlLabel, FormGroup
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import AddIcon from '@mui/icons-material/Add';

const InterestForm = () => {
    const [userInterests, setUserInterests] = useState([]);
    const [allInterests, setAllInterests] = useState([]);
    const [error, setError] = useState(null);
    const [openDialog, setOpenDialog] = useState(false);
    const [selectedInterests, setSelectedInterests] = useState([]);

    useEffect(() => {
        loadUserInterests();
        loadAllInterests();
    }, []);

    const loadAllInterests = async () => {
        try {
            const response = await fetchAllInterests();
            // Handle the $values array from the response
            const interests = response.$values || [];
            setAllInterests(interests);
        } catch (err) {
            console.error('Failed to load all interests:', err);
            setAllInterests([]);
        }
    };

    const loadUserInterests = async () => {
        try {
            const response = await fetchUserInterests();
            // Handle the $values array from the response
            const interests = response.$values || [];
            setUserInterests(interests);
            setError(null);
        } catch (err) {
            setError('Failed to load interests');
            console.error(err);
            setUserInterests([]);
        }
    };

    const handleDelete = async (interestId) => {
        try {
            await removeUserInterest(interestId);
            setUserInterests(userInterests.filter(interest => interest.id !== interestId));
            setError(null);
        } catch (err) {
            setError('Failed to delete interest');
            console.error(err);
        }
    };

    const handleOpenDialog = () => {
        setSelectedInterests([]);
        setOpenDialog(true);
    };

    const handleCloseDialog = () => {
        setOpenDialog(false);
    };

    const handleCheckboxChange = (interestId) => {
        setSelectedInterests(prev => {
            if (prev.includes(interestId)) {
                return prev.filter(id => id !== interestId);
            }
            return [...prev, interestId];
        });
    };

    const handleAddInterests = async () => {
        try {
            // Send only the IDs to the backend
            await addUserInterests(selectedInterests);
            await loadUserInterests();
            handleCloseDialog();
        } catch (err) {
            setError('Failed to add interests');
            console.error(err);
        }
    };

    return (
        <Container maxWidth="sm">
            <Box sx={{ mt: 4, mb: 4 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                    <Typography variant="h4" component="h1">
                        My Interests
                    </Typography>
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={handleOpenDialog}
                    >
                        Add New
                    </Button>
                </Box>

                {error && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        {error}
                    </Alert>
                )}

                <Paper elevation={2}>
                    <List>
                        {userInterests.map(interest => (
                            <ListItem
                                key={interest.id || interest.Id}  // Handle both casing possibilities
                                secondaryAction={
                                    <IconButton
                                        edge="end"
                                        aria-label="delete"
                                        onClick={() => handleDelete(interest.id || interest.Id)}
                                        color="error"
                                    >
                                        <DeleteIcon />
                                    </IconButton>
                                }
                            >
                                <ListItemText primary={interest.name || interest.Name} />
                            </ListItem>
                        ))}
                        {userInterests.length === 0 && (
                            <ListItem>
                                <ListItemText
                                    primary="No interests added yet"
                                    sx={{ textAlign: 'center', color: 'text.secondary' }}
                                />
                            </ListItem>
                        )}
                    </List>
                </Paper>

                <Dialog open={openDialog} onClose={handleCloseDialog}>
                    <DialogTitle>Add New Interests</DialogTitle>
                    <DialogContent>
                        <FormGroup>
                            {allInterests
                                .filter(interest => !userInterests.some(ui => (ui.id || ui.Id) === (interest.id || interest.Id)))
                                .map(interest => (
                                    <FormControlLabel
                                        key={interest.id || interest.Id}
                                        control={
                                            <Checkbox
                                                checked={selectedInterests.includes(interest.id || interest.Id)}
                                                onChange={() => handleCheckboxChange(interest.id || interest.Id)}
                                            />
                                        }
                                        label={interest.name || interest.Name}
                                    />
                                ))}
                        </FormGroup>
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={handleCloseDialog}>Cancel</Button>
                        <Button
                            onClick={handleAddInterests}
                            variant="contained"
                            disabled={selectedInterests.length === 0}
                        >
                            Add Selected
                        </Button>
                    </DialogActions>
                </Dialog>
            </Box>
        </Container>
    );
};

export default InterestForm;