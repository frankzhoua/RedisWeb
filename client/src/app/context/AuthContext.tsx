import React, { createContext, useState, useContext, ReactNode, useEffect } from 'react'

// Define the type for AuthContext
interface AuthContextType {
    isLoggedIn: boolean
    setIsLoggedIn: React.Dispatch<React.SetStateAction<boolean>>
    token: string | null
    setToken: React.Dispatch<React.SetStateAction<string | null>>
    currentForm: 'login' | 'signup' | 'forgotPassword'
    setCurrentForm: React.Dispatch<React.SetStateAction<'login' | 'signup' | 'forgotPassword'>>
    role: 'admin' | 'user'
    setRole: React.Dispatch<React.SetStateAction<'admin' | 'user'>>
    name: string
    setName: React.Dispatch<React.SetStateAction<string>>
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

// AuthProvider component that provides all authentication-related state
export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [isLoggedIn, setIsLoggedIn] = useState<boolean>(false)
    const [token, setToken] = useState<string | null>(localStorage.getItem('authToken'))
    const [currentForm, setCurrentForm] = useState<'login' | 'signup' | 'forgotPassword'>('login')
    const [role, setRole] = useState<'admin' | 'user'>('user') // Default role is a regular user
    const [name, setName] = useState<string>(localStorage.getItem('username') || '') // 从 localStorage 读取 name

    useEffect(() => {
        if (token) {
            setIsLoggedIn(true) // If a token exists, set to logged in
        }
    }, [token])

    // 监听 `name` 变化，并存入 localStorage
    useEffect(() => {
        if (name) {
            localStorage.setItem('username', name) // 存储到 localStorage
        } else {
            localStorage.removeItem('username') // 清除存储
            setCurrentForm('login') // name 为空时，切换到 login
        }
    }, [name])

    return (
        <AuthContext.Provider
            value={{
                isLoggedIn,
                setIsLoggedIn,
                token,
                setToken,
                currentForm,
                setCurrentForm,
                role,
                setRole,
                name,
                setName,
            }}
        >
            {children}
        </AuthContext.Provider>
    )
}

// Custom hook for accessing AuthContext
export const useAuth = (): AuthContextType => {
    const context = useContext(AuthContext)
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider')
    }
    return context
}
