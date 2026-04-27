import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import CssBaseline from '@mui/material/CssBaseline'
import './index.css'
import './theme/Theme.css'
import App from './App.jsx'
import { AuthProvider } from './context/AuthContext'
import { ThemeModeProvider } from './theme/ThemeModeProvider'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <ThemeModeProvider>
      <CssBaseline />
      <BrowserRouter>
        <AuthProvider>
          <App />
        </AuthProvider>
      </BrowserRouter>
    </ThemeModeProvider>
  </StrictMode>,
)
