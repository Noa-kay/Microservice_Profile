import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { Stack, Typography } from '@mui/material'
import PageShell from '../../components/PageShell/PageShell'
import { useAuth } from '../../context/AuthContext'

export default function Signup() {
  const { register, isAuthenticated } = useAuth()
  const navigate = useNavigate()
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    if (isAuthenticated) {
      navigate('/profile', { replace: true })
    }
  }, [isAuthenticated, navigate])

  const handleSubmit = async (event) => {
    event.preventDefault()
    setError('')
    setIsSubmitting(true)

    try {
      await register(email, password, name || undefined)
      navigate('/profile', { replace: true })
    } catch (requestError) {
      setError(
        requestError?.response?.data?.message ||
          requestError?.response?.data ||
          requestError.message ||
          'Registration failed.',
      )
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <PageShell>
      <Stack spacing={2}>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 700 }}>
          הרשמה
        </Typography>
        <Typography variant="body1" color="text.secondary">
          צור חשבון חדש. אחרי ההרשמה תועברי לאזור האישי.
        </Typography>

        <form className="login-form" onSubmit={handleSubmit}>
          <label>
            שם (אופציונלי)
            <input
              type="text"
              value={name}
              onChange={(event) => setName(event.target.value)}
              placeholder="שם מלא"
            />
          </label>
          <label>
            אימייל
            <input
              type="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              placeholder="you@example.com"
              required
            />
          </label>
          <label>
            סיסמה (לפחות 6 תווים)
            <input
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              minLength={6}
              required
            />
          </label>
          {error ? <p className="error-text">{error}</p> : null}
          <button type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'נרשמים...' : 'הרשמה'}
          </button>
        </form>
      </Stack>
    </PageShell>
  )
}
