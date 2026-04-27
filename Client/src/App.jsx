import { Route, Routes } from 'react-router-dom'
import { Box } from '@mui/material'
import PrivateRoute from './routes/PrivateRoute'
import HomePage from './pages/HomePage'
import LoginPage from './pages/LoginPage'
import ProfilePage from './pages/ProfilePage'
import NotFoundPage from './pages/NotFoundPage'
import About from './pages/About/About'
import Contact from './pages/Contact/Contact'
import Jobs from './pages/Jobs/Jobs'
import Profiles from './pages/Profiles/Profiles'
import Settings from './pages/Settings/Settings'
import Signup from './pages/Signup/Signup'
import Home from './pages/Home/Home'
import Layout from './components/Layout/Layout'

function App() {
  return (
    <Layout>
      <Box>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/dashboard" element={<Home />} />
          <Route path="/jobs" element={<Jobs />} />
          <Route path="/profiles" element={<Profiles />} />
          <Route path="/about" element={<About />} />
          <Route path="/contact" element={<Contact />} />
          <Route path="/settings" element={<Settings />} />
          <Route path="/signup" element={<Signup />} />
          <Route path="/login" element={<LoginPage />} />
          <Route
            path="/profile"
            element={
              <PrivateRoute>
                <ProfilePage />
              </PrivateRoute>
            }
          />
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </Box>
    </Layout>
  )
}

export default App
