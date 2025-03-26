import React, { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    Button,
    Typography,
    Box,
} from '@mui/material'
import axios from 'axios'
import agent from '@/app/api/agent'

interface Parameter {
    name: string
    region: string
    description: string
    clients: string
    threads: string
    size: string
    requests: string
    pipeline: string
    timeStamp:  string
    status: '1' | '2' | '3' | '4'
}

const Statistics: React.FC = () => {
    const [parameters, setParameters] = useState<Parameter[]>([])
    const navigate = useNavigate()

    const fetchData = async () => {
        try {
            const response = await axios.get('https://localhost:7179/api/Data/GetBenchmarkRequestData')
            // const response = await agent.Create.getBenchmarkRunJson()
            console.log('Fetched data:', response)
            setParameters(response.data.reverse())
        } catch (error) {
            console.error('Error fetching the parameters!', error)
        }
    }

    useEffect(() => {
        fetchData()
    }, [])

    const handleNavigate = (timeStamp: string) => {
        navigate(`/create/dataDisplayPage/${timeStamp}`);
    };

    const renderStatusButton = (status: Parameter['status'], timeStamp : string) => {
        const statusMap: Record<
            Parameter['status'],
            {
                label: string
                color: 'inherit' | 'success' | 'warning' | 'primary' | 'secondary' | 'error' | 'info'
                disabled: boolean
            }
        > = {
            '3': { label: 'Successful', color: 'success', disabled: false },
            '2': { label: 'In Progress', color: 'inherit', disabled: true },
            '1': { label: 'Running', color: 'inherit', disabled: true },
            '4': { label: 'Error', color: 'error', disabled: true },
        }

        const { label, color, disabled } = statusMap[status]
        return (
            <Button
                variant="contained"
                color={color} // 传递枚举值
                onClick={() => handleNavigate(timeStamp)}
                disabled={disabled}
                size="small"
                sx={{
                    textTransform: 'none',
                    fontSize: '14px',
                    width: '150px', // Ensure all buttons have the same size
                    height: '40px', // Ensure consistent height
                    opacity: 1, // Keep opacity as 1 even for disabled button
                    backgroundColor: disabled ? `${color}.main` : `${color}.light`, // Manually control the background color
                    '&:disabled': {
                        backgroundColor: `${color}.main`, // Ensure background color remains the same when disabled
                        opacity: 1, // Make sure opacity stays normal for disabled buttons
                        color: 'white', // Set text color to white for disabled buttons
                    },
                }}
            >
                {label}
            </Button>
        )
    }

    return (
        <Box sx={{ padding: '20px', marginLeft: '5%', minHeight: '60vh', overflow: 'auto' }}>
            <Typography
                variant="h3"
                gutterBottom
                align="center"
                sx={{
                    fontWeight: 'bold',
                    background: 'linear-gradient(45deg, #1976d2, #9c27b0)',
                    WebkitBackgroundClip: 'text',
                    WebkitTextFillColor: 'transparent',
                    fontSize: '30px',
                }}
            >
                Statistics
            </Typography>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Name</TableCell>
                            <TableCell>Region</TableCell>
                            <TableCell>Description</TableCell>
                            <TableCell>Clients</TableCell>
                            <TableCell>Threads</TableCell>
                            <TableCell>Size</TableCell>
                            <TableCell>Requests</TableCell>
                            <TableCell>Pipeline</TableCell>
                            <TableCell>Status</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {parameters.map((param, index) => (
                            <React.Fragment key={index}>
                                <TableRow>
                                    <TableCell>{param.name}</TableCell>
                                    <TableCell>{param.region}</TableCell>
                                    <TableCell>{param.description}</TableCell>
                                    <TableCell>{param.clients}</TableCell>
                                    <TableCell>{param.threads}</TableCell>
                                    <TableCell>{param.size}</TableCell>
                                    <TableCell>{param.requests}</TableCell>
                                    <TableCell>{param.pipeline}</TableCell>
                                    <TableCell>{renderStatusButton(param.status,param.timeStamp)}</TableCell>
                                </TableRow>
                                {index !== parameters.length - 1 && (
                                    <TableRow>
                                        <TableCell colSpan={9} sx={{ borderBottom: '2px solid #f0f0f0' }}></TableCell>
                                    </TableRow>
                                )}
                            </React.Fragment>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </Box>
    )
}

export default Statistics
