import { createBrowserRouter, Navigate } from "react-router-dom";
import App from "../../common/layout/App";
import HomePage from "../../features/home/HomePage";
import Catalog from "../../features/catalog/Catalog";
import CompanyDetail from "../../features/catalog/CompanyDetail";
import AboutPage from "../../features/about/AboutPage";
import ContactPage from "../../features/contact/ContactPage";
import ServerError from "../errors/ServerError";
import NotFound from "../errors/NotFound";
import Delete from "../../features/delete/DeletePage";
import CreatePage from "../../features/create/CreatePage";
import OtherPage from "../../features/other/OtherPage";

export const router = createBrowserRouter([
    {
        path: '/',
        element: <App />,
        children:[
            {path: '',element: <HomePage/>},
            {path: 'create',element: <CreatePage/>},
            {path: 'delete',element: <Delete/>},
            {path: 'other',element: <OtherPage/>},
            {path: 'catalog',element: <Catalog/>},
            {path: 'catalog/:id',element: <CompanyDetail/>},
            {path: 'about',element: <AboutPage/>},
            {path: 'contact',element: <ContactPage/>},
            {path: 'test',element: <CreatePage/>},
            {path: 'server-error',element: <ServerError/>},
            {path: 'not-found',element: <NotFound/>}, //404
            {path: '*',element: <Navigate replace to ='/not-found'/>}
        ]
    }
])