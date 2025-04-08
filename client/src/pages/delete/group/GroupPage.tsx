import React, { useEffect, useState } from 'react'
import {
    Box,
    Button,
    FormControl,
    MenuItem,
    TextField,
    List,
    ListItem,
    ListItemText,
    Divider,
    Autocomplete,
    Typography,
} from '@mui/material'
import agent from '../../../app/api/agent'
import LoadingComponent from '../../../common/components/CustomLoading'
import { Overlay, subscriptionList } from '../../../common/constants/constants'
import { DeleteModel } from '../../../common/models/DeleteModel'
import { handleGenericSubmit } from '../../../app/util/util'
import { useMessageContext } from '@/app/context/MessageContext'

const GroupPage: React.FC = () => {
    const [subscription, setSubscription] = useState('')
    const [group, setGroup] = useState('')
    const [loading, setLoading] = useState(false)
    const [groupList, setGroupList] = useState<string[]>([])
    const [errors, setErrors] = useState<{ [key: string]: string }>({})
    const [resourceList, setResourceList] = useState<string[]>([]) // Resource list state
    const [showResourceBox, setShowResourceBox] = useState(false) // Control whether the resource box is shown or not

    const [isOpen, setIsOpen] = useState(true)
    const { addMessage } = useMessageContext()

    // const { setIsBlurred } = useBackgroundBlur() // Use hook to get setIsBlurred
    // // Use higher-order function to create a submit handler function
    // const handleGenericSubmit = withBackgroundBlur(setIsBlurred)

    // Initialization
    useEffect(() => {
        setSubscription('1e57c478-0901-4c02-8d35-49db234b78d2')
        agent.Create.getGroup('1e57c478-0901-4c02-8d35-49db234b78d2')
            .then((response) => {
                const sortedResponse = response.sort(
                    (a: string, b: string) => a.toLowerCase().localeCompare(b.toLowerCase()) // Sort case-insensitively
                )
                setGroupList(sortedResponse)
            })
            .catch((error) => console.log(error.response))
    }, [])

    // Update the resource list every minute
    useEffect(() => {
        const intervalId = setInterval(() => {
            if (group) {
                // Call API to get the resource list for this group
                agent.Delete.getResource(subscription, group)
                    .then((response) => {
                        // Format key-value pairs into a string array, e.g., "BVT-RebootBladeTest-1203-5675: Premium"
                        const resourceList = Object.keys(response).map((key) => `${key}: ${response[key]}`)

                        // Save the resource list (as string array)
                        setResourceList(resourceList)
                        setShowResourceBox(true) // Show the resource box
                    })
                    .catch((error) => {
                        console.log(error.response)
                        setResourceList([]) // Clear the resource list
                        setShowResourceBox(false) // Hide the resource box
                    })
            }
        }, 60000) // Update every 60000 milliseconds (1 minute)

        // Clear the interval to prevent memory leaks
        return () => {
            clearInterval(intervalId)
        }
    }, [subscription, group]) // Dependencies are subscription and group, it will start a new interval only when they change

    // Form validation
    const CheckForm = () => {
        const newErrors: { [key: string]: string } = {}
        if (!subscription) newErrors.subscription = 'Subscription cannot be empty'
        if (!group) newErrors.group = 'Group cannot be empty'
        setErrors(newErrors)
        return Object.keys(newErrors).length === 0 // Return whether there are errors
    }

    // API request to delete the group
    const apiPathFunction = async (data: DeleteModel) => {
        return await agent.Delete.sendDelGroupJsonT(data)
    }

    const handleSubmit = (event: React.FormEvent) => {
        const data: DeleteModel = {
            subscription,
            resourceGroupName: group,
        }
        handleGenericSubmit(event, data, apiPathFunction, CheckForm, setLoading)
        addMessage('Deletion is complete', `Cache deleted at ${new Date().toLocaleTimeString()}.`)
    }

    // Cancel button logic
    const handleCancel = () => {
        setSubscription('')
        setGroup('')
        setErrors({})
        setResourceList([]) // Clear the resource list on cancel
        setShowResourceBox(false) // Hide the resource box
    }

    // When the subscription selection changes
    const handleSubChange = (subscriptionid: string) => {
        setSubscription(subscriptionid)
        setGroup('') // Clear the group selection
        setResourceList([]) // Clear the resource list
        setShowResourceBox(false) // Hide the resource box
        setErrors((prevErrors) => ({ ...prevErrors, subscription: '' })) // Clear subscription errors
        agent.Create.getGroup(subscriptionid)
            .then((response) => {
                const sortedResponse = response.sort(
                    (a: string, b: string) => a.toLowerCase().localeCompare(b.toLowerCase()) // Sort case-insensitively
                )
                setGroupList(sortedResponse)
            })
            .catch((error) => console.log(error.response))
    }

    // When the group selection changes
    const handleGroupChange = (group: string) => {
        setGroup(group)
        setErrors((prevErrors) => ({ ...prevErrors, group: '' })) // Clear group errors

        // Call API to get the resource list for this group
        agent.Delete.getResource(subscription, group)
            .then((response) => {
                // Format key-value pairs into a string array, e.g., "BVT-RebootBladeTest-1203-5675: Premium"
                const resourceList = Object.keys(response).map((key) => `${key}: ${response[key]}`)

                // Save the resource list (as string array)
                setResourceList(resourceList)
                setShowResourceBox(true) // Show the resource box
            })
            .catch((error) => {
                console.log(error.response)
                setResourceList([]) // Clear the resource list
                setShowResourceBox(false) // Hide the resource box
            })
    }

    const handleInputChange =
        (field: string) => (_event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>, value: string) => {
            //const { value } = event.target;

            switch (field) {
                case 'group':
                    handleGroupChange(value)
                    break
                default:
                    break
            }
        }

    return (
        <Box>
            <Typography
                variant="h3"
                gutterBottom
                sx={{
                    textAlign: 'center',
                    fontWeight: 'bold',
                    background: 'linear-gradient(45deg, #1976d2, #9c27b0)', // Example gradient
                    WebkitBackgroundClip: 'text',
                    WebkitTextFillColor: 'transparent',
                    fontSize: '30px', // You can adjust the font size as needed
                }}
            >
                Delete Cache
            </Typography>
            <form className="submit-box" onSubmit={handleSubmit}>
                <Box sx={{ display: 'flex', justifyContent: 'center', flexDirection: 'column', alignItems: 'center' }}>
                    <FormControl variant="outlined" sx={{ width: '100%', marginTop: 2 }}>
                        <TextField
                            select
                            label={`Subscription`}
                            value={subscription}
                            onChange={(e) => handleSubChange(e.target.value)}
                            variant="outlined"
                            error={!!errors.subscription}
                            helperText={errors.subscription}
                            fullWidth
                            disabled={loading}
                        >
                            {subscriptionList.map((option) => (
                                <MenuItem key={option.value} value={option.value}>
                                    {option.label}
                                </MenuItem>
                            ))}
                        </TextField>
                    </FormControl>

                    <FormControl variant="outlined" sx={{ width: '100%', marginTop: 2 }}>
                        <Autocomplete
                            options={groupList}
                            value={group}
                            onChange={(event, value) =>
                                handleInputChange('group')(
                                    event as React.ChangeEvent<HTMLInputElement>,
                                    value as string
                                )
                            }
                            renderInput={(params) => (
                                <TextField
                                    {...params}
                                    label="Group"
                                    variant="outlined"
                                    error={!!errors.group}
                                    helperText={errors.group}
                                    fullWidth
                                />
                            )}
                        />
                    </FormControl>
                </Box>

                {/* Display the resource list box */}
                {showResourceBox && (
                    <Box
                        sx={{
                            marginTop: 3,
                            width: '100%',
                            padding: 2,
                            border: '1px solid #ccc',
                            borderRadius: 2,
                            maxHeight: 300, // Set max height
                            overflowY: 'auto', // Enable vertical scrolling
                        }}
                    >
                        <h3>ResourceList</h3>
                        {resourceList.length > 0 ? (
                            <List>
                                {resourceList.map((resource, index) => (
                                    <React.Fragment key={index}>
                                        <ListItem>
                                            <ListItemText primary={resource} />
                                        </ListItem>
                                        <Divider />
                                    </React.Fragment>
                                ))}
                            </List>
                        ) : (
                            <p>No resources found.</p>
                        )}
                    </Box>
                )}

                <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
                    <Button
                        type="submit"
                        variant="contained"
                        color="primary"
                        sx={{ mx: 1, textTransform: 'none' }}
                        disabled={loading || !showResourceBox || resourceList.length === 0} // Add showResourceBox as disable condition
                    >
                        Submit
                    </Button>

                    <Button
                        type="button"
                        variant="outlined"
                        color="secondary"
                        onClick={handleCancel}
                        sx={{ mx: 1, textTransform: 'none' }}
                        disabled={loading}
                    >
                        Cancel
                    </Button>
                </Box>
            </form>
            {loading && (
                <Overlay>
                    <LoadingComponent />
                </Overlay>
            )}
        </Box>
    )
}

export default GroupPage
