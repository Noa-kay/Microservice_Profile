import axios from 'axios'

export const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || 'http://localhost:5250/api'
export const AUTH_LOGIN_ENDPOINT =
  import.meta.env.VITE_AUTH_LOGIN_ENDPOINT || '/auth/login'
export const AUTH_REGISTER_ENDPOINT =
  import.meta.env.VITE_AUTH_REGISTER_ENDPOINT || '/auth/register'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken')

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})

export const loginRequest = async ({ email, password }) => {
  const response = await api.post(AUTH_LOGIN_ENDPOINT, { email, password })
  return response.data
}

export const registerRequest = async ({ email, password, name }) => {
  const response = await api.post(AUTH_REGISTER_ENDPOINT, {
    email,
    password,
    name,
  })
  return response.data
}

export const getStudentProfile = async (userId) => {
  const response = await api.get(`/student/${userId}`)
  return response.data
}

export const updateStudentProfile = async (profileDto) => {
  const response = await api.put('/student', profileDto)
  return response.data
}

export const getProjectsByUserId = async (userId) => {
  const response = await api.get(`/project/user/${userId}`)
  return response.data
}

/**
 * Multipart POST — use fetch so the browser sets the boundary (axios default JSON header breaks this).
 */
export const addProjectForUser = async (userId, formData) => {
  const token = localStorage.getItem('authToken')
  const base = API_BASE_URL.replace(/\/$/, '')
  const response = await fetch(`${base}/project/user/${userId}`, {
    method: 'POST',
    headers: token ? { Authorization: `Bearer ${token}` } : {},
    body: formData,
  })

  if (!response.ok) {
    const text = await response.text()
    const error = new Error(text || response.statusText || 'Request failed')
    error.response = { data: text, status: response.status }
    throw error
  }

  const contentType = response.headers.get('content-type') || ''
  if (contentType.includes('application/json')) {
    return response.json()
  }

  return null
}

export const deleteProject = async (projectId) => {
  await api.delete(`/project/${projectId}`)
}

export const getPersonalDetails = async (userId) => {
  const response = await api.get(`/personaldetails/${userId}`)
  return response.data
}

export const updatePersonalDetails = async (details) => {
  const response = await api.put('/personaldetails', details)
  return response.data
}

export const getUserSkills = async (userId) => {
  const response = await api.get(`/skills/user/${userId}`)
  return response.data
}

export const updateUserSkills = async (userId, skills) => {
  await api.put(`/skills/user/${userId}`, skills)
}

export const getUserFiles = async (userId) => {
  const response = await api.get(`/files/user/${userId}`)
  return response.data
}

export const saveUserFile = async (fileDto) => {
  await api.post('/files', fileDto)
}

export const updateUserFile = async (documentId, fileDto) => {
  await api.put(`/files/${documentId}`, fileDto)
}

export const deleteUserFile = async (documentId) => {
  await api.delete(`/files/${documentId}`)
}

export const uploadFileBinary = async (file) => {
  const formData = new FormData()
  formData.append('file', file)
  const response = await api.post('/file/upload', formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  })
  return response.data
}

export const initChatSession = async (userId) => {
  const response = await api.post(`/chat/init/${userId}`)
  return response.data
}

export const getChatHistory = async (userId) => {
  const response = await api.get(`/chat/${userId}`)
  return response.data
}

export const sendChatMessage = async (messageDto) => {
  const response = await api.post('/chat/message', messageDto)
  return response.data
}

/** Saves user message and returns [userMessage, assistantMessage] from the server. */
export const sendChatMessageWithReply = async (userMessage, resumeContext) => {
  const response = await api.post('/chat/message/reply', {
    userMessage,
    resumeContext: resumeContext || null,
  })
  return response.data
}

export default api
