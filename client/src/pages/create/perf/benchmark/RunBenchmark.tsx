import React from 'react'
import Typography from '@mui/material/Typography'
import Container from '@mui/material/Container'
import Box from '@mui/material/Box'
import TextField from '@mui/material/TextField'
import MenuItem from '@mui/material/MenuItem'
import Select, { SelectChangeEvent } from '@mui/material/Select'
import InputLabel from '@mui/material/InputLabel'
import RadioGroup from '@mui/material/RadioGroup'
import FormControlLabel from '@mui/material/FormControlLabel'
import FormLabel from '@mui/material/FormLabel'
import Radio from '@mui/material/Radio'
import Checkbox from '@mui/material/Checkbox'
import FormGroup from '@mui/material/FormGroup'
import Grid from '@mui/material/Grid'
import Button from '@mui/material/Button'
import { useNavigate } from 'react-router-dom'
import agent from '@/app/api/agent'

const RunBenchmark = () => {
    const [name, setname] = React.useState('')
    const [region, setregion] = React.useState('East US 2 EUAP')
    const [description, setdescription] = React.useState('')
    const [primary, setprimary] = React.useState('')
    const [clients, setclients] = React.useState('')
    const [size, setsize] = React.useState('')
    const [requests, setrequests] = React.useState('')
    const [times, settimes] = React.useState('10')
    const [pipeline, setpipeline] = React.useState('')
    const [threads, setthreads] = React.useState('')
    const navigate = useNavigate()
    const [loading, setLoading] = React.useState(false)

    const handleChange = (event: SelectChangeEvent) => {
        setregion(event.target.value as string)
    }
    
    // 生成时间戳
    const generateTimeStamp = () => {
        const now = new Date();
        return now.toISOString().replace(/[-:.TZ]/g, '');
    };

    const handleSubmit = async () => {
        if (!name || !primary || !clients || !size || !requests || !threads) {
            alert('Please fill out all required fields.');
            return;
          }

        setLoading(true);
        const timeStamp = generateTimeStamp();

        const body = {
            name,
            pw:primary,
            region,
            description,
            clients,
            threads,
            size,
            requests,
            pipeline,
            times,
            // timeStamp
        }
    
        try {
            const response = await agent.Create.sendBenchmarkRunJson(body)
            console.log('Submission successful:', response)
            navigate('/create/Statistics');
        } catch (error) {
            console.error('Error submitting data:', error)
        }
    }

    const handleRoutine = () => {
        //常规做法
    }

    return (
        <div>
            <Container maxWidth="xl">
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
                    Run Test
                </Typography>
                <Box sx={{ display: 'flex', flexWrap: 'wrap', height: '60vh', overflowY: 'auto' }}>
                    <Box
                        sx={{
                            flex: '0 0 50%',
                            //maxHeight: '90vh',
                            overflowY: 'auto',
                            paddingRight: '16px',
                        }}
                    >
                        <Grid container spacing={3}>
                            <Grid item xs={12}>
                                <InputLabel>Cache Name(e.g : Verifyperformance-P5-EUS2E.redis.cache.windows.net )</InputLabel>
                                <TextField
                                    fullWidth
                                    variant="outlined"
                                    value={name}
                                    onChange={(e) => setname(e.target.value)}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <InputLabel>Primary String</InputLabel>
                                <TextField
                                    fullWidth
                                    variant="outlined"
                                    value={primary}
                                    onChange={(e) => setprimary(e.target.value)}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <InputLabel>Region</InputLabel>
                                <Select fullWidth value={region} onChange={handleChange}>
                                    <MenuItem value="East US 2 EUAP">East US 2 EUAP</MenuItem>
                                    {/* <MenuItem value="Central US EUAP">Central US EUAP</MenuItem>
                                    <MenuItem value="East US">East US</MenuItem> */}
                                </Select>
                            </Grid>
                            <Grid item xs={12}>
                                <InputLabel>Enter a description</InputLabel>
                                <TextField
                                    fullWidth
                                    variant="outlined"
                                    value={description}
                                    onChange={(e) => setdescription(e.target.value)}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <FormLabel>Select test type</FormLabel>
                                <RadioGroup row defaultValue="No-SSL">
                                    <FormControlLabel value="No-SSL" control={<Radio />} label="No-SSL" />
                                    <FormControlLabel value="SSL" control={<Radio />} label="SSL" />
                                </RadioGroup>
                            </Grid>
                            <Grid item xs={12}>
                                <InputLabel>Select test way</InputLabel>
                                <FormGroup row>
                                    <FormControlLabel control={<Checkbox />} label="Set" />
                                    <FormControlLabel control={<Checkbox defaultChecked />} label="Get" />
                                </FormGroup>
                            </Grid>
                        </Grid>
                    </Box>

                    <Box
                        sx={{
                            flex: '0 0 50%',
                            //maxHeight: '90vh',
                            overflowY: 'auto',
                            paddingLeft: '16px',
                        }}
                    >
                        <Grid container spacing={2}>
                            <Grid item xs={12}>
                                <InputLabel>Clients (-c)</InputLabel>
                                <TextField
                                    fullWidth
                                    variant="outlined"
                                    type="number"
                                    value={clients}
                                    onChange={(e) => {
                                        const value = e.target.value;
                                        // 确保输入的值为正数且大于0
                                        if (!value || (parseInt(value) > 0 && /^\d+$/.test(value))) {
                                            setclients(value);
                                        }
                                    }}
                                    error={clients !== '' && parseInt(clients) <= 0} // 显示错误状态
                                    helperText={
                                        clients !== '' && parseInt(clients) <= 0
                                            ? 'Clients must be a positive number greater than 0.'
                                            : ''
                                    } // 错误提示
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <InputLabel>Threads(-t)</InputLabel>
                                <TextField
                                    fullWidth
                                    variant="outlined"
                                    type="number"
                                    value={threads}
                                    onChange={(e) => {
                                        const value = e.target.value;
                                        // 确保输入的值为正数且大于0
                                        if (!value || (parseInt(value) > 0 && /^\d+$/.test(value))) {
                                            setthreads(value);
                                        }
                                    }}
                                    error={threads !== '' && parseInt(threads) <= 0} // 显示错误状态
                                    helperText={
                                        threads !== '' && parseInt(threads) <= 0
                                            ? 'threads must be a positive number greater than 0.'
                                            : ''
                                    } // 错误提示
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <InputLabel>Payload size (bytes) (-d)</InputLabel>
                                <TextField
                                    fullWidth
                                    variant="outlined"
                                    type="number"
                                    value={size}
                                    onChange={(e) => {
                                        const value = e.target.value;
                                        // 确保输入的值为正数且大于0
                                        if (!value || (parseInt(value) > 0 && /^\d+$/.test(value))) {
                                            setsize(value);
                                        }
                                    }}
                                    error={size !== '' && parseInt(size) <= 0} // 显示错误状态
                                    helperText={
                                        size !== '' && parseInt(size) <= 0
                                            ? 'threads must be a positive number greater than 0.'
                                            : ''
                                    } // 错误提示
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <InputLabel>Number of requests (-n)</InputLabel>
                                <TextField
                                    fullWidth
                                    variant="outlined"
                                    type="number"
                                    value={requests}
                                    onChange={(e) => {
                                        const value = e.target.value;
                                        // 确保输入的值为正数且大于0
                                        if (!value || (parseInt(value) > 0 && /^\d+$/.test(value))) {
                                            setrequests(value);
                                        }
                                    }}
                                    error={requests !== '' && parseInt(requests) <= 0} // 显示错误状态
                                    helperText={
                                        requests !== '' && parseInt(requests) <= 0
                                            ? 'threads must be a positive number greater than 0.'
                                            : ''
                                    } // 错误提示
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <InputLabel>Pipeline (-P)</InputLabel>
                                <TextField
                                    fullWidth
                                    variant="outlined"
                                    type="number"
                                    value={pipeline}
                                    onChange={(e) => {
                                        const value = e.target.value;
                                        // 确保输入的值为正数且大于0
                                        if (!value || (parseInt(value) > 0 && /^\d+$/.test(value))) {
                                            setpipeline(value);
                                        }
                                    }}
                                    error={pipeline !== '' && parseInt(pipeline) <= 0} // 显示错误状态
                                    helperText={
                                        pipeline !== '' && parseInt(pipeline) <= 0
                                            ? 'threads must be a positive number greater than 0.'
                                            : ''
                                    } // 错误提示
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <InputLabel>Run times (-i)</InputLabel>
                                <TextField
                                    fullWidth
                                    variant="outlined"
                                    type="number"
                                    value={times}
                                    onChange={(e) => {
                                        const value = e.target.value;
                                        // 确保输入的值为正数且大于0
                                        if (!value || (parseInt(value) > 0 && /^\d+$/.test(value))) {
                                            settimes(value);
                                        }
                                    }}
                                    error={times !== '' && parseInt(times) <= 0} // 显示错误状态
                                    helperText={
                                        times !== '' && parseInt(times) <= 0
                                            ? 'threads must be a positive number greater than 0.'
                                            : ''
                                    } // 错误提示
                                />
                            </Grid>
                        </Grid>
                    </Box>
                </Box>
                <Box mt={3} display="flex" justifyContent="center" gap={80}>
                <Button
                    variant="contained"
                    onClick={handleSubmit}
                    sx={{
                        background: 'linear-gradient(45deg, #1976d2, #9c27b0)',
                        color: 'white',
                        fontWeight: 'bold',
                        borderRadius: '20px',
                        padding: '10px 60px',
                        textTransform: 'none',
                        '&:hover': {
                            background: 'linear-gradient(45deg, #1565c0, #7b1fa2)',
                        },
                    }}
                >
                    Submit
                </Button>
                <Button
                    variant="outlined"
                    onClick={() => {
                        setname('');
                        setregion('East US 2 EUAP');
                        setdescription('');
                        setprimary('');
                        setclients('');
                        setsize('');
                        setrequests('');
                        settimes('10');
                        setpipeline('');
                        setthreads('');
                    }}
                    sx={{
                        color: '#1976d2',
                        fontWeight: 'bold',
                        borderRadius: '20px',
                        border: '2px solid #1976d2',
                        padding: '10px 60px',
                        textTransform: 'none',
                        '&:hover': {
                            borderColor: '#1565c0',
                            color: '#1565c0',
                        },
                    }}
                >
                    Cancel
                </Button>
            </Box>
            </Container>
        </div>
    )
}

export default RunBenchmark
