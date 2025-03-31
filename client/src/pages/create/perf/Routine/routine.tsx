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
import { useState } from 'react';
import axios from 'axios';

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
            
            <Box component="form" sx={{ mt: 3, display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 2 }}>
                <Box display="flex" alignItems="center" gap={2}>
                    <Typography variant="body1" sx={{ fontWeight: 'bold' }}>Select Cache：</Typography>
                    <TextField id="outlined-basic" label="Cache Date" variant="outlined" />
                </Box>
                <Button variant="contained" sx={{ mt: 2, borderRadius: '8px', background: '#1976d2', '&:hover': { background: '#1565c0' } }}>确定</Button>
            </Box>
                <Box component="form" sx={{ mt: 3, display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 2 }}>
                <Box display="flex" alignItems="center" gap={2}>
                    <Typography variant="body1" sx={{ fontWeight: 'bold' }}>Select Cache：</Typography>
                    <TextField
                        id="Get_data"
                        label="Cache Date"
                        variant="outlined"
                        value={cacheDate}
                        onChange={handleInputChange}
                    />
                </Box>
                <Button
                    variant="contained"
                    sx={{ mt: 2, borderRadius: '8px', background: '#1976d2', '&:hover': { background: '#1565c0' } }}
                    onClick={fetchAndDownloadTxt} // 绑定点击事件
                >
                    查找结果
                </Button>
            </Box>
        </React.Fragment>
    );
};

export default Routine;
