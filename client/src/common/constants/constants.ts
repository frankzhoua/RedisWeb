import { Box, createTheme, Theme } from '@mui/material'
import { Button, keyframes, ListItem, ListItemText, Paper, styled } from '@mui/material'
import md5 from 'md5'

// Subscription list
export const subscriptionList = [
    {
        value: '1e57c478-0901-4c02-8d35-49db234b78d2',
        label: 'Cache Team - Vendor CTI Testing 2',
    },
    {
        value: '32353108-c7dc-4873-9ce8-a7d4d731673d',
        label: 'CacheTeam - RedisTiP',
    },
    {
        value: 'fc2f20f5-602a-4ebd-97e6-4fae3f1f6424',
        label: 'CacheTeam - Redis Perf and Stress Resources',
    },
]

// Test case list
export const BVTTestCaseNames = [
    'FlushBladeTest',
    'DataAccessConfigurationBladeTest',
    'OverviewBladeTest',
    'AccessKeysBladeTest',
    'AdvancedSettingsBladeTest',
    'RebootBladeTest', // Need multiple shards
    'ScaleBladeTest',
    'ClusterSizeBladeTest',
    'DataPersistenceBladeTest-NotPremium', // NotPremium
    'DataPersistenceBladeTest-Premium',
    'ManagedIdentityBladeTest',
    'ScheduleUpdatesBladeTest',
    'GeoreplicationBladeTest', // Need two caches
    'VirtualNetworkBladeTest',
    //"PrivateEndpointBladeTest", // Could not be created
    'FirewallBladeTest',
    'PropertiesBladeTest',
    'Import-ExportBladeTest',
    'PortalOwnedBladeTest',
    'LocalizationTest',
]

// Test case list for manual tests
export const ManualTestCaseNames = ['8672', '8659', '8673']

export const user = {
    avatar: `https://www.gravatar.com/avatar/${md5('v-xinzhang6@microsoft.com')}?d=identicon`, // d=identicon means if no matching avatar is found, it will return a default icon avatar
    username: 'Zhang Xin', // Username
    email: 'v-xinzhang6@microsoft.com', // User email
}

export const Overlay = styled(Box)({
    position: 'fixed',
    top: 0,
    left: 0,
    width: '100%',
    height: '100%',
    backgroundColor: 'rgba(0, 0, 0, 0.5)', // Semi-transparent black
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    zIndex: 1000, // Ensure it overlays other elements
})

// Constants.ts

export const loginTextStyles = {
    ml: 2,
    color: (theme: Theme) => theme.palette.common.white, // Use theme to dynamically set color
    fontWeight: 'bold', // Bold text
    fontSize: '1rem', // Font size
    // textTransform: 'uppercase', // Convert to uppercase
    letterSpacing: 1.0, // Letter spacing
    textAlign: 'center', // Center align text
    // backgroundColor: 'rgba(0, 0, 0, 0.3)', // Transparent background color
    padding: '5px 15px', // Padding
    borderRadius: '20px', // Rounded corners
}

// Create theme
export const theme = createTheme({
    palette: {
        primary: {
            main: '#1976d2',
        },
        secondary: {
            main: '#ff4081',
        },
    },
    typography: {
        fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
        h5: {
            fontWeight: 700,
            color: '#2c3e50',
        },
        h6: {
            fontWeight: 600,
            color: '#34495e',
        },
        body1: {
            fontWeight: 400,
            color: '#555',
        },
    },
})

const slideInAnimation = keyframes`
  from {
    transform: translateX(100%);
  }
  to {
    transform: translateX(0);
  }
`

// Create click animation
const clickAnimation = keyframes`
  0% {
    transform: scale(1);
  }
  50% {
    transform: scale(0.95);
  }
  100% {
    transform: scale(1);
  }
`

export const StyledPaper = styled(Paper)`
    padding: 20px;
    border-radius: 10px;
    transition: background-color 0.3s;
    animation: ${slideInAnimation} 0.5s ease-in-out;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
    background-color: #ffffff;
`

export const StyledListItem = styled(ListItem)`
    transition: background-color 0.3s;
    &:hover {
        background-color: #e0f7fa;
    }
    &:active {
        animation: ${clickAnimation} 0.2s ease-in-out;
    }
`

export const StyledListItemText = styled(ListItemText)`
    text-align: left;
`

export const StyledButton = styled(Button)`
    transition: background-color 0.3s;
    &:hover {
        background-color: #1565c0;
    }
    &:active {
        animation: ${clickAnimation} 0.2s ease-in-out;
    }
`
