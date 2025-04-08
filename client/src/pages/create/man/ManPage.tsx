import React, { useEffect, useState } from 'react'
import {
    Box,
    Button,
    RadioGroup,
    SelectChangeEvent,
    Radio,
    FormControl,
    FormControlLabel,
    TextField,
    CircularProgress,
    MenuItem,
    Typography,
    InputLabel,
    Select,
    OutlinedInput,
    Checkbox,
    FormHelperText,
    ListItemText,
} from '@mui/material'
import { Autocomplete } from '@mui/material'
import agent from '../../../app/api/agent'
import { ManModel } from '../../../common/models/DataModel'
import { handleGenericSubmit } from '../../../app/util/util'
import { ManualTestCaseNames, Overlay, subscriptionList } from '../../../common/constants/constants'
import LoadingComponent from '@/common/components/CustomLoading'

const ManPage: React.FC = () => {
    const [subscription, setSubscription] = useState('')
    const [group, setGroup] = useState('')
    const [groupList, setGroupList] = useState<string[]>([])
    const [loading, setLoading] = useState(false)
    const [errors, setErrors] = useState<{ [key: string]: string }>({})
    const [selectedNames, setSelectedNames] = useState<string[]>([])
    const [quantity, setQuantity] = useState('')
    const [option, setOption] = useState('case')
    const [name, setName] = useState('')

    // Initialize load
    useEffect(() => {
        setSubscription('1e57c478-0901-4c02-8d35-49db234b78d2')
        fetchGroupList('1e57c478-0901-4c02-8d35-49db234b78d2')
    }, [])

    // Get group list
    const fetchGroupList = (subscriptionId: string) => {
        agent.Create.getGroup(subscriptionId)
            .then((response) => {
                const sortedResponse = response.sort(
                    (a: string, b: string) => a.toLowerCase().localeCompare(b.toLowerCase()) // Sort ignoring case
                )
                setGroupList(sortedResponse)
            })
            .catch((error) => console.error(error))
    }

    // Form validation
    const checkForm = () => {
        const newErrors: { [key: string]: string } = {}
        if (!subscription) newErrors.subscription = 'Subscription cannot be empty'
        if (!group) newErrors.group = 'Group cannot be empty'
        if (selectedNames.length === 0) newErrors.selectedNames = 'Select at least one case'
        if (selectedNames.length === 1 && quantity.trim() === '') newErrors.quantity = 'Quantity cannot be empty'
        setErrors(newErrors)
        return Object.keys(newErrors).length === 0
    }
    const handleSelectChange = (event: SelectChangeEvent<string[]>) => {
        const value = event.target.value as string[]
        setSelectedNames(value)
        if (value.length > 0) {
            setErrors((prevErrors) => ({ ...prevErrors, selectedNames: '' }))
        }
    }
    const handleInputChange = (field: string) => (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { value } = event.target
        switch (field) {
            case 'name':
                setName(value)
                setErrors((prevErrors) => ({ ...prevErrors, name: '' }))
                break
            case 'quantity':
                setQuantity(value)
                setErrors((prevErrors) => ({ ...prevErrors, quantity: '' }))
                break
            default:
                break
        }
    }
    // Submit handler
    const handleSubmit = (event: React.FormEvent) => {
        event.preventDefault()
        if (!checkForm()) return

        const data: ManModel = {
            name,
            subscription,
            group,
            ...(option === 'case' && {
                cases: selectedNames,
                ...(selectedNames.length === 1 && { quantity: quantity }),
            }),
        }
        const customMessage = 'Once started, the cache used in MAN will be created!'
        handleGenericSubmit(event, data, () => agent.Create.sendManJson(data), checkForm, setLoading, customMessage)
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
                Create: Manual Cache
            </Typography>
            <form className="submit-box" onSubmit={handleSubmit}>
                <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                    <FormControl variant="outlined" sx={{ width: '100%', marginTop: 2 }}>
                        <TextField
                            select
                            label="Subscription"
                            value={subscription}
                            onChange={(e) => setSubscription(e.target.value)}
                            error={!!errors.subscription}
                            helperText={errors.subscription}
                            fullWidth
                        >
                            {[
                                {
                                    value: '1e57c478-0901-4c02-8d35-49db234b78d2',
                                    label: 'Cache Team - Vendor CTI Testing 2',
                                },
                            ].map((option) => (
                                <MenuItem key={option.value} value={option.value}>
                                    {option.label}
                                </MenuItem>
                            ))}
                        </TextField>
                    </FormControl>

                    <FormControl sx={{ width: '100%', marginTop: 2 }}>
                        <Autocomplete
                            freeSolo={false} // Disallow custom input
                            options={groupList}
                            value={group}
                            onChange={(_e, newValue) => setGroup(newValue || '')}
                            renderInput={(params) => (
                                <TextField
                                    {...params}
                                    label="Group"
                                    variant="outlined"
                                    error={!!errors.group}
                                    helperText={errors.group}
                                />
                            )}
                        />
                    </FormControl>

                    <FormControl component="fieldset" sx={{ marginTop: 2 }}>
                        <RadioGroup row value={option} onChange={(e) => setOption(e.target.value)}>
                            {/* <FormControlLabel value="all" control={<Radio />} label="All" /> */}
                            <FormControlLabel value="case" control={<Radio />} label="Case" />
                        </RadioGroup>
                    </FormControl>

                    {option === 'case' && (
                        <>
                            <FormControl
                                variant="outlined"
                                sx={{ width: '100%', marginTop: 2 }}
                                error={!!errors.selectedNames}
                            >
                                <InputLabel id="names-label">Case</InputLabel>
                                <Select
                                    labelId="names-label"
                                    multiple
                                    value={selectedNames}
                                    onChange={handleSelectChange}
                                    input={<OutlinedInput label="Case" />}
                                    renderValue={(selected) =>
                                        selected.length > 2 ? `Select  ${selected.length} ` : selected.join(', ')
                                    }
                                >
                                    {ManualTestCaseNames.map((name) => {
                                        const isDisabled =
                                            name.startsWith('15318672') ||
                                            name.startsWith('15379626') ||
                                            name.startsWith('24879297')
                                        return (
                                            <MenuItem
                                                key={name}
                                                value={name}
                                                disabled={isDisabled}
                                                sx={isDisabled ? { color: 'red' } : {}}
                                            >
                                                <Checkbox checked={selectedNames.includes(name)} />
                                                <ListItemText primary={name} />
                                            </MenuItem>
                                        )
                                    })}
                                </Select>
                                {errors.selectedNames && <FormHelperText error>{errors.selectedNames}</FormHelperText>}
                            </FormControl>

                            {selectedNames.length === 1 && (
                                <TextField
                                    label="quantity"
                                    type="number"
                                    value={quantity}
                                    onChange={handleInputChange('quantity')}
                                    variant="outlined"
                                    error={!!errors.quantity}
                                    helperText={errors.quantity}
                                    sx={{ width: '100%', marginTop: 2 }}
                                />
                            )}
                        </>
                    )}

                    <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
                        <Button
                            type="submit"
                            variant="contained"
                            color="primary"
                            disabled={loading}
                            sx={{ mx: 1, textTransform: 'none' }}
                        >
                            Submit
                        </Button>
                        <Button
                            type="button"
                            variant="outlined"
                            color="secondary"
                            disabled={loading}
                            onClick={() => {
                                setSubscription('')
                                setGroup('')
                                setSelectedNames([])
                                setErrors({})
                            }}
                            sx={{ mx: 1, textTransform: 'none' }}
                        >
                            Cancel
                        </Button>
                    </Box>
                </Box>
            </form>
            {loading && (
                <Overlay>
                    <LoadingComponent message="Submitting, please wait..." />
                </Overlay>
            )}
        </Box>
    )
}

export default ManPage
