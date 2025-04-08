import { Box, Button, keyframes, ListItem, ListItemText, Paper, styled } from '@mui/material'
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

// Test case list
export const ManualTestCaseNames = [
    '15318672: Verify portal E2E',   //Need to create manually
    '15318659: Verify Redis Enterprise persistence scenarios',
    '15318673: Verify zone redundant multi-replica cache',
    '15318674: Verify VNET injected cache scenario',
    '15318675: Verify Regenerate Keys scenario',
    '15319070: Verify Cluster scaling scenarios', 
    '15319116: Verify MRPP scaling scenarios',
    '15320703: Verify geo-replication operations',
    '15379626: Premium cache with private link', //Need to create manually
    '15379676: Clustered and non-clustered premium cache AOF/RDB persistence',
    '15379764: Clustered AOF enabled cache in VNET',
    '15379484: Premium cache',
    '15379874: Clustered MRPP cache in VNET', 
    '15380305: Clustered geo-replicated cache',
    '21253924: Verify Azure Watson crash dump integration',
    '16021106: Geo DNS records should start pointing to new Secondary domain after the Geo Failover.',
    '16021140: Initiate Geo failover on Geo Secondary after rebooting Geo Primary',
    '16021226: Perform Geo Failover from UX and validate states and notifications',
    '24879297: Verify OSS cache deletion scenario',//Need to create manually
    '25064413: Verify cache creation with AAD', //Requires API creation
    '26639809: Enterprise Redis geo-replicated database updates', //Requires API creation
]


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
