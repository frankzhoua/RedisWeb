import React from 'react';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import ComputerIcon from '@mui/icons-material/Computer';
import CircleIcon from '@mui/icons-material/Circle';
import TextField from '@mui/material/TextField';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import Button from '@mui/material/Button';
import { useEffect,useState } from 'react';
import Backdrop from '@mui/material/Backdrop';
import CircularProgress from '@mui/material/CircularProgress';
import agent from '@/app/api/agent';
import { Card, CardContent } from '@mui/material'
import DatePicker from 'react-datepicker'; // 用于日期选择
import 'react-datepicker/dist/react-datepicker.css'; // 引入日期选择样式
import axios from 'axios';
import {
    Alert,
    FormControl,
    Autocomplete,
} from '@mui/material'

const vmList = [
    { name: 'P1P2', status: 'on' },
    { name: 'P3P4', status: 'off' },
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
    const [group, setGroup] = useState('')
    const [groupList, setGroupList] = useState<string[]>([])
    const [errors, setErrors] = useState<{ [key: string]: string }>({})
    const [subscription, setSubscription] = useState('')
    const [insertMessage, setInsertMessage] = useState('');
    const [loading, setLoading] = useState(false);
      // 这里指定 selectedDate 的类型为 Date 或 null
    const [selectedDate, setSelectedDate] = useState<Date | null>(null); 


    // Initialize
    useEffect(() => {
        setSubscription('1e57c478-0901-4c02-8d35-49db234b78d2')
        agent.Create.getGroup('1e57c478-0901-4c02-8d35-49db234b78d2')
            .then((response) => {
                const sortedResponse = response.sort(
                    (a: string, b: string) => a.toLowerCase().localeCompare(b.toLowerCase()) // Sort ignoring case
                )
                setGroupList(sortedResponse)
            })
            .catch((error) => console.log(error.response))
    }, [])

    const handleInsertGroup = async () => {
        if (!group) {
            alert("请输入 Group Name！");
            return;
        }
    
        setLoading(true); // 显示加载框
    
        try {
            await axios.post(
                "https://localhost:7179/api/BenchmarkRun/InsertQCommandByGroupName",
                JSON.stringify(group),
                {
                    headers: {
                        "Content-Type": "application/json"
                    }
                }
            );
            setInsertMessage("Successfully inserted into the queue, please click the 'Run' button to run");
            alert("Success！");
        } catch (error) {
            console.error("Insert failed:", error);
            alert("Insert failed, please check whether the service is running properly!");
        } finally {
            setLoading(false); // 无论成功失败都隐藏加载框
        }
    };
    const handleRunTasks = async () => {
        setLoading(true); // 显示加载框
        try {
            // 发送请求到后端，注意这里只是发起请求，不关心后端是否完成
            axios.post("https://localhost:7179/api/BenchmarkRun/execute-tasks", {}, {
                headers: {
                    "Content-Type": "application/json"
                }
            });
    
            // 请求发送成功后立即弹出提示
            alert("The task run request has been sent, go to the Statistics page to see how it is running!");
        } catch (error) {
            console.error("Run tasks failed:", error);
            alert("Task running request failed, please check whether the service is running properly!");
        } finally {
            setLoading(false); // 无论成功失败都隐藏加载框
        }
    };
    
    const handleFetchResult = async () => {
        if (!selectedDate) {
            alert('Make sure that Date is selected');
            return;
        }
    
        setLoading(true); // 显示加载框
        try {
            await axios.post("https://localhost:7179/api/BenchmarkRun/FinalDataTest", 
            selectedDate,  // 直接传递时间字段
            {
                headers: {
                    "Content-Type": "application/json"
                }
            });
            alert("The results have been processed, you can go below to get the results!");
        } catch (error) {
            console.error("Fetch result failed:", error);
            alert("Task running request failed, please check whether the service is running properly!");
        } finally {
            setLoading(false); // 无论成功失败都隐藏加载框
        }
    };
    
    // 处理输入框变化
    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setCacheDate(event.target.value);
    };
     // 发送请求并下载 TXT 文件
    const fetchAndDownloadTxt = async () => {
        if (!cacheDate) {
            alert("请输入 Cache Date!");
            return;
        }

        try {
            const response = await axios.get(
                `https://localhost:7179/api/BenchmarkRun/GetBenchmarkData?date=${cacheDate}`,
                { responseType: "blob" }
            );

            // 创建 Blob 并下载文件
            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement("a");
            link.href = url;
            link.setAttribute("download", `BenchmarkData_${cacheDate}.txt`);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (error) {
            console.error("Error downloading the file:", error);
            alert("Download failed, please check whether the server is running properly!");
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
                <TableContainer component={Paper} sx={{ width: '90%', maxWidth: 1200, borderRadius: '0px', boxShadow: 3, overflowX: 'auto' }}>
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
                {/* Select Group + 插入运行队列 */}
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
                     {/* Select Date */}
                    <Box display="flex" alignItems="center" justifyContent="space-between">
                    <Typography variant="body1" sx={{ fontWeight: 'bold' }}>
                        Select Date:
                    </Typography>
                    <DatePicker
                        selected={selectedDate}
                        onChange={(date) => setSelectedDate(date)}
                        dateFormat="Pp" // 显示日期和时间
                        placeholderText="Select a date"
                    />
                    </Box>
                            {/* Insert, Run, and Result Buttons */}
                <Box sx={{ display: 'flex', gap: 2, width: '100%', justifyContent: 'space-between' }}>
                    {/* Insert Button */}
                    <Button
                        variant="contained"
                        sx={{
                            width: '30%',
                            borderRadius: '8px',
                            background: '#1976d2',
                            '&:hover': { background: '#1565c0' },
                            height: '48px',
                        }}
                        onClick={handleInsertGroup}
                    >
                        Insert
                    </Button>

                    {/* Run Button */}
                    <Button
                        variant="contained"
                        sx={{
                            width: '30%',
                            borderRadius: '8px',
                            background: '#1976d2',
                            '&:hover': { background: '#1565c0' },
                            height: '48px',
                        }}
                        onClick={handleRunTasks}
                    >
                        Run
                    </Button>

                    {/* Result Button */}
                    <Button
                        variant="contained"
                        sx={{
                            width: '30%',
                            borderRadius: '8px',
                            background: '#1976d2',
                            '&:hover': { background: '#1565c0' },
                            height: '48px',
                        }}
                        onClick={handleFetchResult}
                    >
                        Result
                    </Button>
                </Box>
                        </Box>

                {/* Select Cache Date + 查找结果 */}
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
                    <Button
                        variant="contained"
                        sx={{
                            width: '100%',
                            borderRadius: '8px',
                            background: '#1976d2',
                            '&:hover': { background: '#1565c0' },
                            height: '48px', // 保持一致
                        }}
                        onClick={fetchAndDownloadTxt}
                    >
                        查找结果
                    </Button>
                </Box>
            </Box>

            <Backdrop
                sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }}
                open={loading}
            >
                <CircularProgress color="inherit" />
            </Backdrop>

        </React.Fragment>
        
    );
};

export default Routine;
