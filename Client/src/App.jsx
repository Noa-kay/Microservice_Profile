import { Link, Route, Routes } from 'react-router-dom'
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
import { useAuth } from './context/AuthContext'

function App() {
  const { isAuthenticated } = useAuth()

  return (
    <div className="app-shell">
      <header className="top-nav">
        <h1>Student Profile Client</h1>
        <nav>
          <Link to="/">Home</Link>
          <Link to="/dashboard">Team Home</Link>
          <Link to="/jobs">Jobs</Link>
          <Link to="/profiles">Profiles</Link>
          <Link to="/about">About</Link>
          <Link to="/contact">Contact</Link>
          <Link to="/settings">Settings</Link>
          <Link to="/signup">Signup</Link>
          {!isAuthenticated ? <Link to="/login">Login</Link> : null}
          <Link to="/profile">My Profile</Link>
        </nav>
      </header>

      <main className="page-content">
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
      </main>
    </div>
  )
}

export default App
