import { createBrowserRouter, Navigate } from 'react-router-dom'
import App from '../../layout/App'
import HomePage from '@/pages/home/HomePage'
import ServerError from '../errors/ServerError'
import NotFound from '../errors/NotFound'
import CreatePage from '@/pages/create/CreatePage'
import OtherPage from '@/pages/other/OtherPage'
import BvtPage from '@/pages/create/bvt/BvtPage'
import ManPage from '@/pages/create/man/ManPage'
import PerfPage from '@/pages/create/perf/PerfPage'
import AltPage from '@/pages/create/alt/AltPage'
import DeletePage from '@/pages/delete/DeletePage'
import InsertPage from '@/pages/other/insert/InsertPage'
import MedianPage from '@/pages/other/median/MedianPage'
import GroupPage from '@/pages/delete/group/GroupPage'
import Signal from '@/pages/other/signalR/Signal'
import RunBenchmark from '@/pages/create/perf/benchmark/RunBenchmark'
import Statistics from '@/pages/create/perf/statistics/Statistics'
import DataDisplayPage from '@/pages/create/perf/data/Dashboard'
import Routine from '@/pages/create/perf/Routine/routine'
import TxtExcelMerger from '@/pages/create/perf/InsertExcel/insertexcel'

export const router = createBrowserRouter([
    {
        path: '/',
        element: <App />,
        children: [
            { path: '', element: <HomePage /> },
            {
                path: 'create',
                element: <CreatePage />,
                children: [
                    { path: 'bvt', element: <BvtPage /> },
                    { path: 'man', element: <ManPage /> },
                    { path: 'perf', element: <PerfPage /> },
                    { path: 'benchmark', element: <RunBenchmark /> },
                    { path: 'statistics', element: <Statistics /> },
                    { path: 'dataDisplayPage/:timeStamp', element: <DataDisplayPage /> },
                    { path: 'alt', element: <AltPage /> },
                    { path: 'routine', element: <Routine /> },
                    { path: 'TxtExcelMerger', element: <TxtExcelMerger /> },
                ],
            },
            {
                path: 'delete',
                element: <DeletePage />,
                children: [{ path: 'group', element: <GroupPage /> }],
            },
            {
                path: 'more',
                element: <OtherPage />,
                children: [
                    { path: 'insert', element: <InsertPage /> },
                    { path: 'median', element: <MedianPage /> },
                    { path: 'signal', element: <Signal /> },
                ],
            },
            { path: 'server-error', element: <ServerError /> },
            { path: 'not-found', element: <NotFound /> }, //404
            { path: '*', element: <Navigate replace to="/not-found" /> },
        ],
    },
])
