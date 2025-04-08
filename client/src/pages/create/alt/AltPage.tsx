import React, { useEffect, useState } from 'react'
import {
    Box,
    Button,
    FormControl,
    MenuItem,
    TextField,
    InputLabel,
    Select,
    CircularProgress,
    Typography,
} from '@mui/material'
import Autocomplete from '@mui/material/Autocomplete' // 引入 Autocomplete
import agent from '../../../app/api/agent'
import { PerfModel } from '../../../common/models/DataModel'
import { Overlay, subscriptionList } from '../../../common/constants/constants'
import LoadingComponent from '../../../common/components/CustomLoading'
import { handleGenericSubmit } from '../../../app/util/util'

const AltPage: React.FC = () => {
    // Initialization code remains the same
    const today = new Date()
    const month = String(today.getMonth() + 1).padStart(2, '0')
    const day = String(today.getDate()).padStart(2, '0')
    const formattedDate = `${month}${day}`
    const [subscription, setSubscription] = useState('fc2f20f5-602a-4ebd-97e6-4fae3f1f6424')
    const [group, setGroup] = useState('')
    const [region, setRegion] = useState('')
    const [cacheName, setCacheName] = useState(`alt-eus2e-{SKU}-${formattedDate}`)
    const [loading, setLoading] = useState(false)
    const [sku, setSku] = useState('All')
    const [groupList, setGroupList] = useState<string[]>([])
    const [errors, setErrors] = useState<{ [key: string]: string }>({})

    useEffect(() => {
        // Get and sort groups
        setSubscription('fc2f20f5-602a-4ebd-97e6-4fae3f1f6424')
        setRegion('East US 2 EUAP')

        agent.Create.getGroup('fc2f20f5-602a-4ebd-97e6-4fae3f1f6424')
            .then((response) => {
                const sortedResponse = response.sort(
                    (a: string, b: string) => a.toLowerCase().localeCompare(b.toLowerCase()) // 忽略大小写排序
                )
                setGroupList(sortedResponse)
                if (sortedResponse.includes('alt-cluster-test')) {
                    setGroup('alt-cluster-test')
                }
            })
            .catch((error) => console.error(error.response))
    }, [])

    const handleInputChange =
        (field: string) =>
        (_event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>, value: string | null) => {
            if (field === 'group') {
                setGroup(value || '')
                setErrors((prevErrors) => ({ ...prevErrors, group: '' }))
            }
        }

    const handleSubmit = (event: React.FormEvent) => {
        const data: PerfModel = { subscription, group, sku }
        handleGenericSubmit(event, data, async (data) => await agent.Create.sendAltJson(data), CheckForm, setLoading)
    }

    const handleCancel = () => {
        setSubscription('')
        setGroup('')
        setRegion('')
        setErrors({})
    }

    const CheckForm = () => {
        const newErrors: { [key: string]: string } = {}
        if (!subscription) newErrors.subscription = 'Subscription cannot be empty'
        if (!group) newErrors.group = 'Group cannot be empty'
        if (!region) newErrors.region = 'Region cannot be empty'
        setErrors(newErrors)
        return Object.keys(newErrors).length === 0
    }

    const handleSubChange = (subscriptionId: string) => {
        setSubscription(subscriptionId)
        setErrors((prevErrors) => ({ ...prevErrors, subscription: '' }))

        agent.Create.getGroup(subscriptionId)
            .then((response) => {
                const sortedResponse = response.sort(
                    (a: string, b: string) => a.toLowerCase().localeCompare(b.toLowerCase()) // 忽略大小写排序
                )
                setGroupList(sortedResponse)
            })
            .catch((error) => console.error(error.response))
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
                Create: ALT Cache
            </Typography>
            <form className="submit-box" onSubmit={handleSubmit}>
                <Box
                    sx={{
                        display: 'flex',
                        justifyContent: 'center',
                        flexDirection: 'column',
                        alignItems: 'center',
                    }}
                >
                    <FormControl variant="outlined" sx={{ width: '100%', marginTop: 2 }}>
                        <TextField
                            select
                            label="Subscription"
                            value={subscription}
                            onChange={(e) => handleSubChange(e.target.value)}
                            variant="outlined"
                            error={!!errors.subscription}
                            helperText={errors.subscription}
                            fullWidth
                        >
                            {subscriptionList.map((option) => (
                                <MenuItem key={option.value} value={option.value}>
                                    {option.label}
                                </MenuItem>
                            ))}
                        </TextField>
                    </FormControl>

                    <FormControl variant="outlined" sx={{ width: '100%', marginTop: 2 }}>
                        <InputLabel id="cacheName-select-label">CacheName</InputLabel>
                        <Select
                            labelId="cacheName-select-label"
                            id="cacheName-select"
                            value={cacheName}
                            label="CacheName"
                            onChange={(e) => setCacheName(e.target.value)}
                        >
                            <MenuItem value={cacheName}>{cacheName}</MenuItem>
                        </Select>
                    </FormControl>

                    <FormControl variant="outlined" sx={{ width: '100%', marginTop: 2 }}>
                        <Autocomplete
                            options={groupList}
                            value={group}
                            onChange={(event, value) =>
                                handleInputChange('group')(event as React.ChangeEvent<HTMLInputElement>, value)
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

                    <FormControl variant="outlined" sx={{ width: '100%', marginTop: 2 }}>
                        <TextField
                            select
                            label="Region"
                            value={region}
                            onChange={(e) => setRegion(e.target.value)}
                            variant="outlined"
                            error={!!errors.region}
                            helperText={errors.region}
                            fullWidth
                        >
                            {['East US 2 EUAP'].map((item) => (
                                <MenuItem key={item} value={item}>
                                    {item}
                                </MenuItem>
                            ))}
                        </TextField>
                    </FormControl>

                    <FormControl variant="outlined" sx={{ width: '100%', marginTop: 2 }}>
                        <InputLabel id="sku-simple-select-label">SKU</InputLabel>
                        <Select
                            labelId="sku-simple-select-label"
                            id="sku-simple-select"
                            value={sku}
                            label="SKU"
                            onChange={(e) => setSku(e.target.value)}
                        >
                            <MenuItem value="All">All</MenuItem>
                            <MenuItem value="Basic">Basic</MenuItem>
                            <MenuItem value="Standard">Standard</MenuItem>
                            <MenuItem value="Premium">Premium</MenuItem>
                        </Select>
                    </FormControl>
                </Box>

                <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
                    <Button type="submit" variant="contained" color="primary" sx={{ mx: 1, textTransform: 'none' }}>
                        Submit
                    </Button>
                    <Button
                        type="button"
                        variant="outlined"
                        color="secondary"
                        onClick={handleCancel}
                        sx={{ mx: 1, textTransform: 'none' }}
                    >
                        Cancel
                    </Button>
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

export default AltPage
