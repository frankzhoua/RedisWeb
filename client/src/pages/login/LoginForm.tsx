import React, { useState } from 'react'
import { Box, TextField, Button, Typography, Link, useTheme, CircularProgress } from '@mui/material'
import { useAuth } from '../../app/context/AuthContext'
import agent from '../../app/api/agent'
import { useNavigate } from 'react-router-dom'
import { useAuthState } from '@/app/context/AuthStateContext'

const LoginForm: React.FC = () => {
    const theme = useTheme()
    const navigate = useNavigate()
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [isLoading, setIsLoading] = useState(false)
    const [error, setError] = useState('')
    const [success, setSuccess] = useState('')
    const { setIsLoggedIn, setToken } = useAuth()
    const { currentForm, setCurrentForm } = useAuthState() // 使用新命名的 hook

    const handleLogin = async (event: React.FormEvent) => {
        event.preventDefault()
        setIsLoading(true)
        setError('')
        setSuccess('')
        try {
            const response = await agent.Auth.login(username, password)
            localStorage.setItem('authToken', response.token)
            setToken(response.token)
            setIsLoggedIn(true)
            setSuccess('Login successful!')
            navigate('/create')
        } catch (error) {
            const errorMessage =
                (error as { data: { message: string } })?.data?.message || 'Invalid username or password.'
            setError(errorMessage)
        } finally {
            setIsLoading(false)
        }
    }
    const handleClick = (event: React.MouseEvent<HTMLAnchorElement>) => {
        event.preventDefault() // 阻止默认跳转行为（如果需要）
        setCurrentForm('forgotPassword')
    }
    const handleUpClick = (event: React.MouseEvent<HTMLAnchorElement>) => {
        event.preventDefault() // 阻止默认跳转行为（如果需要）
        setCurrentForm('signup')
    }

    return (
        <Box
            sx={{
                display: 'flex',
                justifyContent: 'center', // 水平居中
                alignItems: 'center', // 垂直居中
                minHeight: '100vh', // 占满视口高度
            }}
        >
            <Box
                sx={{
                    width: 500, // 增大宽度
                    padding: 5, // 增大内边距
                    backgroundColor: 'rgba(255, 255, 255, 0.5)', // 更明显的背景
                    backdropFilter: 'blur(10px)', // 背景模糊效果
                    borderRadius: 4, // 更圆滑的边框
                    boxShadow: theme.shadows[6], // 强一点的阴影
                    color: theme.palette.text.primary,
                    border: `1px solid rgba(255, 255, 255, 0.6)`, // 半透明边框
                }}
            >
                {/* 标题部分 */}
                <Typography
                    variant="h3" // 更大的标题
                    gutterBottom
                    sx={{
                        textAlign: 'center',
                        fontWeight: 'bold',
                        background: `linear-gradient(45deg, ${theme.palette.primary.main}, ${theme.palette.secondary.main})`,
                        WebkitBackgroundClip: 'text',
                        WebkitTextFillColor: 'transparent',
                    }}
                >
                    Sign In
                </Typography>

                {/* 辅助文字部分 */}
                <Typography
                    variant="body1" // 更大的辅助文字
                    gutterBottom
                    sx={{
                        textAlign: 'center',
                        color: theme.palette.text.secondary,
                    }}
                >
                    Don't have an account?{' '}
                    <Link
                        href="#"
                        sx={{
                            color: theme.palette.primary.main,
                            fontWeight: 'bold',
                            textDecoration: 'none',
                            '&:hover': {
                                textDecoration: 'underline',
                                color: theme.palette.primary.dark,
                            },
                        }}
                        onClick={handleUpClick}
                    >
                        Sign up
                    </Link>
                </Typography>

                <form onSubmit={handleLogin}>
                    {/* 输入框部分 */}
                    <TextField
                        fullWidth
                        required
                        label="Username"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        margin="normal"
                        type="Username"
                        variant="outlined"
                        InputProps={{
                            style: { color: theme.palette.text.primary },
                        }}
                        InputLabelProps={{
                            style: { color: theme.palette.text.secondary },
                        }}
                        sx={{
                            '& .MuiOutlinedInput-root': {
                                '& fieldset': {
                                    borderColor: theme.palette.grey[400],
                                },
                                '&:hover fieldset': {
                                    borderColor: theme.palette.primary.main,
                                },
                                '&.Mui-focused fieldset': {
                                    borderColor: theme.palette.primary.dark,
                                },
                            },
                        }}
                        disabled={isLoading}
                    />
                    <TextField
                        fullWidth
                        required
                        label="Password"
                        margin="normal"
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        variant="outlined"
                        InputProps={{
                            style: { color: theme.palette.text.primary },
                        }}
                        InputLabelProps={{
                            style: { color: theme.palette.text.secondary },
                        }}
                        sx={{
                            '& .MuiOutlinedInput-root': {
                                '& fieldset': {
                                    borderColor: theme.palette.grey[400],
                                },
                                '&:hover fieldset': {
                                    borderColor: theme.palette.primary.main,
                                },
                                '&.Mui-focused fieldset': {
                                    borderColor: theme.palette.primary.dark,
                                },
                            },
                        }}
                        disabled={isLoading}
                    />

                    {/* 忘记密码链接 */}
                    <Link
                        href="#"
                        variant="body2"
                        sx={{
                            display: 'block',
                            marginBottom: 3,
                            textAlign: 'right',
                            color: theme.palette.text.secondary,
                            '&:hover': {
                                textDecoration: 'underline',
                                color: theme.palette.primary.main,
                            },
                        }}
                        onClick={handleClick}
                    >
                        Forgot password?
                    </Link>

                    {/* 按钮部分 */}
                    <Button
                        fullWidth
                        type="submit"
                        disabled={isLoading}
                        variant="contained"
                        sx={{
                            backgroundColor: theme.palette.primary.main,
                            color: theme.palette.primary.contrastText,
                            fontWeight: 'bold',
                            textTransform: 'none',
                            fontSize: '1.2rem', // 增大字体
                            padding: '12px', // 增大按钮
                            '&:hover': {
                                backgroundColor: theme.palette.primary.dark,
                            },
                        }}
                    >
                        {isLoading ? (
                            <CircularProgress
                                sx={{
                                    color: theme.palette.primary.contrastText,
                                }}
                            />
                        ) : (
                            'Sign in'
                        )}
                    </Button>
                </form>

                {/* 底部说明文字 */}
                <Typography
                    variant="body1"
                    sx={{
                        marginTop: 3,
                        color: theme.palette.text.secondary,
                        textAlign: 'center',
                        fontStyle: 'italic',
                    }}
                >
                    Enter <strong>email</strong> and <strong>password </strong>
                </Typography>
            </Box>
        </Box>
    )
}

export default LoginForm
