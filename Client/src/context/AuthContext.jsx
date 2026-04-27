import { createContext, useContext, useEffect, useMemo, useState } from 'react'
import { loginRequest, registerRequest } from '../services/api'

const TOKEN_STORAGE_KEY = 'authToken'
const AuthContext = createContext(null)

const parseJwtPayload = (token) => {
  try {
    const payload = token.split('.')[1]
    if (!payload) {
      return null
    }

    const base64 = payload.replace(/-/g, '+').replace(/_/g, '/')
    const normalized = base64.padEnd(Math.ceil(base64.length / 4) * 4, '=')
    return JSON.parse(atob(normalized))
  } catch (error) {
    return null
  }
}

const isTokenValid = (token) => {
  const payload = parseJwtPayload(token)
  if (!payload || !payload.exp) {
    return false
  }

  const nowInSeconds = Math.floor(Date.now() / 1000)
  return payload.exp > nowInSeconds
}

const getUserIdFromPayload = (payload) =>
  payload?.userId ||
  payload?.uid ||
  payload?.sub ||
  payload?.nameid ||
  payload?.[
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
  ] ||
  null

const getTokenFromLoginResponse = (response) =>
  response?.token ||
  response?.accessToken ||
  response?.jwt ||
  response?.data?.token ||
  response?.data?.accessToken ||
  response?.result?.token ||
  null

export function AuthProvider({ children }) {
  const [token, setToken] = useState(null)
  const [user, setUser] = useState(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const storedToken = localStorage.getItem(TOKEN_STORAGE_KEY)
    if (!storedToken || !isTokenValid(storedToken)) {
      localStorage.removeItem(TOKEN_STORAGE_KEY)
      setIsLoading(false)
      return
    }

    const payload = parseJwtPayload(storedToken)
    setToken(storedToken)
    setUser({
      userId: getUserIdFromPayload(payload),
      email: payload?.email || payload?.unique_name || '',
      claims: payload,
    })
    setIsLoading(false)
  }, [])

  const applyAuthToken = (nextToken, fallbackEmail) => {
    if (!nextToken || !isTokenValid(nextToken)) {
      throw new Error('Auth response did not return a valid token.')
    }

    const payload = parseJwtPayload(nextToken)
    localStorage.setItem(TOKEN_STORAGE_KEY, nextToken)
    setToken(nextToken)
    setUser({
      userId: getUserIdFromPayload(payload),
      email: payload?.email || payload?.unique_name || fallbackEmail,
      claims: payload,
    })
  }

  const login = async (email, password) => {
    const data = await loginRequest({ email, password })
    const nextToken = getTokenFromLoginResponse(data)
    applyAuthToken(nextToken, email)
  }

  const register = async (email, password, name) => {
    const data = await registerRequest({ email, password, name })
    const nextToken = getTokenFromLoginResponse(data)
    applyAuthToken(nextToken, email)
  }

  const logout = () => {
    localStorage.removeItem(TOKEN_STORAGE_KEY)
    setToken(null)
    setUser(null)
  }

  const value = useMemo(
    () => ({
      token,
      user,
      isLoading,
      isAuthenticated: Boolean(token && user),
      login,
      register,
      logout,
    }),
    [token, user, isLoading],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider.')
  }

  return context
}
