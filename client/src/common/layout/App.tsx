
import Header from "./Header";
import { Container, CssBaseline, ThemeProvider,  } from "@mui/material";
import { Outlet } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';
import { useTheme } from "../../app/context/ThemeContext";
import { darkTheme, lightTheme } from "./theme";

function App() {
  const { isDarkMode } = useTheme();

  return (
    <ThemeProvider theme={isDarkMode ? darkTheme : lightTheme}>
        <ToastContainer position='bottom-right' hideProgressBar theme="colored"/>
        <CssBaseline/>
        <Header />
        <Container>
          <Outlet/> {/* This is a placeholder for the child components */}
        </Container>   
      </ThemeProvider>     
  )
}

export default App