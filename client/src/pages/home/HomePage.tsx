import React, { useEffect, useState } from 'react'
import { Box, Typography, IconButton, useTheme } from '@mui/material'
import ArrowBackIosIcon from '@mui/icons-material/ArrowBackIos'
import ArrowForwardIosIcon from '@mui/icons-material/ArrowForwardIos'
import './HomePage.css'
import { Hero, images, ImageWrapper, Indicator, IndicatorDot } from './constants'
import LoginForm from '../login/LoginForm'
import SignUpForm from '../login/SignUpForm'
import ForgotPasswordForm from '../login/ForgotPasswordForm'
import { useAuth } from '../../app/context/AuthContext'

const HomePage: React.FC = () => {
    const theme = useTheme()
    const { isLoggedIn, currentForm, role } = useAuth()
    const [currentIndex, setCurrentIndex] = useState(0)
    const [isHovering, setIsHovering] = useState(false)

    useEffect(() => {
        const interval = isHovering
            ? null
            : setInterval(() => {
                  setCurrentIndex((prevIndex) => (prevIndex + 1) % images.length)
              }, 3000)

        return () => {
            if (interval != null) {
                clearInterval(interval)
            }
        }
    }, [isHovering])

    const handlePrev = () => {
        setCurrentIndex((prevIndex) => (prevIndex - 1 + images.length) % images.length)
    }

    const handleNext = () => {
        setCurrentIndex((prevIndex) => (prevIndex + 1) % images.length)
    }

    return (
        <Box
            sx={{
                position: 'relative',
                minHeight: '90vh',
                overflow: 'hidden',
                backgroundColor: theme.palette.background.default,
                color: theme.palette.text.primary,
            }}
        >
            {/* Image carousel area */}
            <Hero
                onMouseEnter={() => setIsHovering(true)}
                onMouseLeave={() => setIsHovering(false)}
                sx={{
                    position: 'relative',
                }}
            >
                {/* Dark mode overlay */}
                {theme.palette.mode === 'dark' && (
                    <Box
                        sx={{
                            position: 'absolute',
                            top: 0,
                            left: 0,
                            width: '100%',
                            height: '100%',
                            backgroundColor: 'rgba(0, 0, 0, 0.5)', // Semi-transparent black overlay
                            zIndex: 11, // Ensure the overlay is on top of the image
                            pointerEvents: 'none', // The overlay doesn't intercept mouse events
                        }}
                    />
                )}

                {/* LoginForm component */}
                <Box
                    sx={{
                        position: 'absolute',
                        top: '50%',
                        left: '80%',
                        transform: 'translate(-50%, -50%)',
                        zIndex: 10, // Ensure LoginForm is on top of the image
                    }}
                >
                    {/* Show LoginForm only when currentForm is 'login' and not logged in */}
                    {currentForm === 'login' && !isLoggedIn && <LoginForm />}

                    {/* Show SignUpForm only when currentForm is 'signup' and role is 'admin' */}
                    {currentForm === 'signup' && role === 'admin' && <SignUpForm />}

                    {/* Show ForgotPasswordForm only when currentForm is 'forgotPassword' and not logged in */}
                    {currentForm === 'forgotPassword' && !isLoggedIn && <ForgotPasswordForm />}
                </Box>

                {/* Image carousel */}
                <ImageWrapper shift={-1}>
                    <img
                        src={images[(currentIndex - 1 + images.length) % images.length]}
                        alt="Previous"
                        className="image"
                    />
                </ImageWrapper>

                <ImageWrapper shift={0}>
                    <img src={images[currentIndex]} alt="Current" className="image" />
                </ImageWrapper>

                <ImageWrapper shift={1}>
                    <img src={images[(currentIndex + 1) % images.length]} alt="Next" className="image" />
                </ImageWrapper>

                {/* Navigation arrows */}
                {/* <IconButton
                    onClick={handlePrev}
                    sx={{
                        position: 'absolute',
                        left: 16,
                        top: '50%',
                        transform: 'translateY(-50%)',
                        color: theme.palette.primary.main,
                        zIndex: 10, // Ensure arrows are on top of the overlay
                    }}
                >
                    <ArrowBackIosIcon fontSize="large" />
                </IconButton>

                <IconButton
                    onClick={handleNext}
                    sx={{
                        position: 'absolute',
                        right: 16,
                        top: '50%',
                        transform: 'translateY(-50%)',
                        color: theme.palette.primary.main,
                        zIndex: 10, // Ensure arrows are on top of the overlay
                    }}
                >
                    <ArrowForwardIosIcon fontSize="large" />
                </IconButton> */}

                {/* Indicator dots
                <Indicator>
                    {images.map((_, index) => (
                        <IndicatorDot
                            key={index}
                            active={index === currentIndex}
                            onClick={() => setCurrentIndex(index)}
                            sx={{
                                cursor: 'pointer',
                                backgroundColor:
                                    index === currentIndex ? theme.palette.primary.main : theme.palette.grey[500],
                            }}
                        />
                    ))}
                </Indicator> */}
            </Hero>
        </Box>
    )
}

export default HomePage
