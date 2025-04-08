import React, { useEffect, useState } from 'react';
import {
    Typography, Box, Grid, TextField, Table, TableBody, TableCell, TableContainer,
    TableHead, TableRow, Paper, Button, Backdrop, CircularProgress, Alert,
    FormControl, Autocomplete, Snackbar
} from '@mui/material';
import { Card, CardContent } from '@mui/material';
import ComputerIcon from '@mui/icons-material/Computer';
import CircleIcon from '@mui/icons-material/Circle';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import agent from '@/app/api/agent';
import axios from 'axios';

const vmList = [
    { name: 'P1,P2', status: 'on' },
    { name: 'P3,P4', status: 'off' },
    { name: 'P5', status: 'on' },
    { name: 'SC0,C1', status: 'on' },
    { name: 'SC2,C3', status: 'off' },
    { name: 'SC4,C5,C6', status: 'on' },
    { name: 'BC0,C1', status: 'off' },
    { name: 'BC3,C4', status: 'on' },
    { name: 'BC4,C5,C6', status: 'off' },
];

const tableData = [
    {
        cacheName: 'Premium',
        testType: 'SSL',
        command: 'Get',
        clients: 64,
        threads: 16,
        requests: '1M',
        size: 1024,
        pipeline: 20,
    },
    {
        cacheName: 'Standard',
        testType: 'SSL',
        command: 'Get',
        clients: 32,
        threads: 16,
        requests: '1M',
        size: 1024,
        pipeline: 10,
    },
    {
        cacheName: 'Basic',
        testType: 'SSL',
        command: 'Get',
        clients: 16,
        threads: 16,
        requests: '1M',
        size: 1024,
        pipeline: 10,
    },
];

const Routine = () => {
    const [cacheDate, setCacheDate] = useState('');
    const [group, setGroup] = useState('');
    const [groupList, setGroupList] = useState<string[]>([]);
    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const [subscription, setSubscription] = useState('');
    const [loading, setLoading] = useState(false);
    const [selectedDate, setSelectedDate] = useState<Date | null>(null);

    // Snackbar 状态
    const [snackbarOpen, setSnackbarOpen] = useState(false);
    const [snackbarMessage, setSnackbarMessage] = useState('');
    const [snackbarSeverity, setSnackbarSeverity] = useState<'success' | 'error' | 'info' | 'warning'>('info');

    const showSnackbar = (message: string, severity: 'success' | 'error' | 'info' | 'warning' = 'info') => {
        setSnackbarMessage(message);
        setSnackbarSeverity(severity);
        setSnackbarOpen(true);
    };

    const handleSnackbarClose = () => {
        setSnackbarOpen(false);
    };

    useEffect(() => {
        setSubscription('1e57c478-0901-4c02-8d35-49db234b78d2');
        agent.Create.getGroup('1e57c478-0901-4c02-8d35-49db234b78d2')
            .then((response) => {
                const sortedResponse = response.sort(
                    (a: string, b: string) => a.toLowerCase().localeCompare(b.toLowerCase())
                );
                setGroupList(sortedResponse);
            })
            .catch((error) => {
                console.log(error.response);
                showSnackbar('Failed to load the Group list', 'error');
            });
    }, []);

    const handleInsertGroup = async () => {
        if (!group) {
            showSnackbar("Please select Group Name!", "warning");
            return;
        }

        setLoading(true);
        try {
            await axios.post(
                "https://localhost:7179/api/BenchmarkRun/InsertQCommandByGroupName",
                JSON.stringify(group),
                { headers: { "Content-Type": "application/json" } }
            );
            showSnackbar("Insert successful, ready to execute!", "success");
        } catch (error) {
            console.error("Insert failed:", error);
            showSnackbar("Failed, please check whether the service is running properly!", "error");
        } finally {
            setLoading(false);
        }
    };

    const handleRunTasks = async () => {
        setLoading(true);
        try {
            await axios.post("https://localhost:7179/api/BenchmarkRun/execute-tasks", {}, {
                headers: { "Content-Type": "application/json" }
            });
            showSnackbar("The execution request has been sent, please go to the Statistics page to check the running status", "info");
        } catch (error) {
            console.error("Run tasks failed:", error);
            showSnackbar("Failed, please check whether the service is running properly!", "error");
        } finally {
            setLoading(false);
        }
    };

    const handleFetchResult = async () => {
        if (!selectedDate) {
            showSnackbar('Please select a date!', 'warning');
            return;
        }

        setLoading(true);
        try {
            await axios.post(
                "https://localhost:7179/api/BenchmarkRun/FinalDataTest",
                selectedDate,
                { headers: { "Content-Type": "application/json" } }
            );
            showSnackbar("Processing is finished, you can download the results!", "success");
        } catch (error) {
            console.error("Fetch result failed:", error);
            showSnackbar("Failed, please check whether the service is running properly!", "error");
        } finally {
            setLoading(false);
        }
    };

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setCacheDate(event.target.value);
    };

    const fetchAndDownloadTxt = async () => {
        if (!cacheDate) {
            showSnackbar("Please enter Cache Date!", "warning");
            return;
        }

        try {
            const response = await axios.get(
                `https://localhost:7179/api/BenchmarkRun/GetBenchmarkData?date=${cacheDate}`,
                { responseType: "blob" }
            );

            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement("a");
            link.href = url;
            link.setAttribute("download", `BenchmarkData_${cacheDate}.txt`);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);

            showSnackbar("Download success!", "success");
        } catch (error) {
            console.error("Error downloading the file:", error);
            showSnackbar("Download failed, please check whether the service is running properly!", "error");
        }
    };

    return (
        <React.Fragment>
            <Box textAlign="center">
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
                    Routine Test
                </Typography>
                <Grid container spacing={3} justifyContent="center" mt={2}>
                    {vmList.map((vm) => (
                        <Grid item xs={4} key={vm.name}>
                            <Box display="flex" alignItems="center" justifyContent="center" gap={1}>
                                <ComputerIcon sx={{ fontSize: 40, color: '#1976d2' }} />
                                <Typography variant="h6">{vm.name}</Typography>
                                <CircleIcon sx={{ fontSize: 20, color: vm.status === 'on' ? 'green' : 'red' }} />
                            </Box>
                        </Grid>
                    ))}
                </Grid>
            </Box>

            <Box mt={5} display="flex" justifyContent="flex-start" width="50vw" sx={{ marginLeft: '-50px', overflowX: 'auto' }}>
                <TableContainer component={Paper} sx={{ width: '90%', maxWidth: 1200, borderRadius: '0px', boxShadow: 3 }}>
                    <Typography variant="h6" sx={{ p: 2, fontWeight: 'bold', borderBottom: '2px solid #1976d2', textAlign: 'center' }}>
                        Cache SKU Test configuration
                    </Typography>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>Cache SKU</TableCell>
                                <TableCell>Test Type</TableCell>
                                <TableCell>Command</TableCell>
                                <TableCell>Clients</TableCell>
                                <TableCell>Threads</TableCell>
                                <TableCell>Requests</TableCell>
                                <TableCell>Size (bytes)</TableCell>
                                <TableCell>Pipeline</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {tableData.map((row, index) => (
                                <TableRow key={index}>
                                    <TableCell>{row.cacheName}</TableCell>
                                    <TableCell>{row.testType}</TableCell>
                                    <TableCell>{row.command}</TableCell>
                                    <TableCell>{row.clients}</TableCell>
                                    <TableCell>{row.threads}</TableCell>
                                    <TableCell>{row.requests}</TableCell>
                                    <TableCell>{row.size}</TableCell>
                                    <TableCell>{row.pipeline}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </Box>

            <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', mt: 3, gap: 3 }}>
                <Box sx={{ width: '100%', maxWidth: 500, display: 'flex', flexDirection: 'column', gap: 2 }}>
                    <Box display="flex" alignItems="center" justifyContent="space-between">
                        <Typography variant="body1" sx={{ fontWeight: 'bold' }}>
                            Select Group:
                        </Typography>
                        <FormControl variant="outlined" sx={{ width: 250 }}>
                            <Autocomplete
                                options={groupList}
                                value={group}
                                onChange={(_event, newValue) => {
                                    setGroup(newValue || '');
                                    setErrors((prevErrors) => ({ ...prevErrors, group: '' }));
                                }}
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
                    </Box>

                    <Box display="flex" alignItems="center" justifyContent="space-between">
                        <Typography variant="body1" sx={{ fontWeight: 'bold' }}>
                            Select Date:
                        </Typography>
                        <DatePicker
                            selected={selectedDate}
                            onChange={(date) => setSelectedDate(date)}
                            dateFormat="Pp"
                            placeholderText="Select a date"
                        />
                    </Box>

                    <Box sx={{ display: 'flex', gap: 2, width: '100%', justifyContent: 'space-between' }}>
                        <Button variant="contained" sx={{ width: '30%', borderRadius: '8px' }} onClick={handleInsertGroup}>
                            Insert
                        </Button>
                        <Button variant="contained" sx={{ width: '30%', borderRadius: '8px' }} onClick={handleRunTasks}>
                            Run
                        </Button>
                        <Button variant="contained" sx={{ width: '30%', borderRadius: '8px' }} onClick={handleFetchResult}>
                            Result
                        </Button>
                    </Box>
                </Box>

                <Box sx={{ width: '100%', maxWidth: 500, display: 'flex', flexDirection: 'column', gap: 2 }}>
                    <Box display="flex" alignItems="center" justifyContent="space-between">
                        <Typography variant="body1" sx={{ fontWeight: 'bold' }}>
                            Select Cache:
                        </Typography>
                        <TextField
                            id="Get_data"
                            label="Cache Date"
                            variant="outlined"
                            value={cacheDate}
                            onChange={handleInputChange}
                            sx={{ width: 250 }}
                        />
                    </Box>
                    <Button variant="contained" sx={{ width: '100%', borderRadius: '8px' }} onClick={fetchAndDownloadTxt}>
                        查找结果
                    </Button>
                </Box>
            </Box>

            <Backdrop sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }} open={loading}>
                <CircularProgress color="inherit" />
            </Backdrop>

            {/* Snackbar 弹出提示框 */}
            <Snackbar
                open={snackbarOpen}
                autoHideDuration={4000}
                onClose={handleSnackbarClose}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
            >
                <Alert onClose={handleSnackbarClose} severity={snackbarSeverity} sx={{ width: '100%' }}>
                    {snackbarMessage}
                </Alert>
            </Snackbar>
        </React.Fragment>
    );
};

export default Routine;
