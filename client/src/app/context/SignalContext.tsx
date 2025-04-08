import React, { createContext, useContext, useEffect, useState } from 'react'
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr'
import agent from '../api/agent'
import axios from 'axios'

// Type definition for SignalContext
interface SignalContextType {
    connection: HubConnection | null
    randomObjects: { name: string; time: string; status: string }[] // Stores random objects
    isConnected: boolean
    clearRandomObjects: () => void // Clears random objects
    sendRandomObjectManually: () => void // Sends a random object manually
    startTimerManually: () => void // Starts the timer manually
    stopTimerManually: () => void // Stops the timer manually
}

export const SignalContext = createContext<SignalContextType | undefined>(undefined)

interface SignalProviderProps {
    children: React.ReactNode
}

export const SignalProvider: React.FC<SignalProviderProps> = ({ children }) => {
    const [connection, setConnection] = useState<HubConnection | null>(null)
    const [randomObjects, setRandomObjects] = useState<{ name: string; time: string; status: string }[]>([])
    const [isConnected, setIsConnected] = useState<boolean>(false)

    useEffect(() => {
        // Create and initialize SignalR connection
        const newConnection = new HubConnectionBuilder()
            .withUrl(`${axios.defaults.baseURL}${agent.SignalR.hubUrl}`, { withCredentials: true })
            .build()

        // Start the connection
        const startConnection = async () => {
            try {
                await newConnection.start()
                console.log('Connected to SignalR Hub')
                setIsConnected(true)
            } catch (err) {
                console.error('Error while starting connection: ' + err)
                setIsConnected(false)
            }
        }

        // Attempt to reconnect when the connection is lost
        newConnection.onclose(async (error) => {
            console.log('Connection lost, attempting to reconnect...', error)
            setIsConnected(false)
            await startConnection() // Attempt to reconnect
        })

        // Update randomObjects when data is received
        newConnection.on('ReceiveRandomObject', (randomObject: { name: string; time: string; status: string }) => {
            console.log('Received random object:', randomObject)
            setRandomObjects((prevObjects) => [...prevObjects, randomObject])
        })

        // Initialize the connection
        startConnection()

        setConnection(newConnection)

        // Clean up the connection
        return () => {
            newConnection.stop().catch((err) => console.error('Error stopping connection: ' + err))
        }
    }, []) // Initialize only once when the component is mounted

    // Clears randomObjects
    const clearRandomObjects = () => {
        setRandomObjects([]) // Clear the received objects
    }

    // Sends a request manually
    const sendRandomObjectManually = () => {
        if (connection && isConnected) {
            connection
                .send('SendRandomObjectManually')
                .then(() => {
                    console.log('Manual request sent')
                })
                .catch((err) => console.log('Error sending manual request: ' + err))
        } else {
            console.log('Cannot send manual request, connection not established.')
        }
    }

    // Starts the timer manually
    const startTimerManually = () => {
        if (connection && isConnected) {
            connection
                .send('StartTimerManually')
                .then(() => {
                    console.log('Timer started manually')
                })
                .catch((err) => console.log('Error starting timer manually: ' + err))
        }
    }

    // Stops the timer manually
    const stopTimerManually = () => {
        if (connection && isConnected) {
            connection
                .send('StopTimerManually')
                .then(() => {
                    console.log('Timer stopped manually')
                })
                .catch((err) => console.log('Error stopping timer manually: ' + err))
        }
    }

    return (
        <SignalContext.Provider
            value={{
                connection,
                randomObjects,
                isConnected,
                clearRandomObjects,
                sendRandomObjectManually,
                startTimerManually,
                stopTimerManually,
            }}
        >
            {children}
        </SignalContext.Provider>
    )
}

export const useSignalContext = (): SignalContextType => {
    const context = useContext(SignalContext)
    if (!context) {
        throw new Error('useSignalContext must be used within a SignalProvider')
    }
    return context
}
