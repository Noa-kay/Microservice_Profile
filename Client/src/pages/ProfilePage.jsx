import { useEffect, useMemo, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import PersonOutlineOutlinedIcon from '@mui/icons-material/PersonOutlineOutlined'
import SchoolOutlinedIcon from '@mui/icons-material/SchoolOutlined'
import FolderOpenOutlinedIcon from '@mui/icons-material/FolderOpenOutlined'
import SmartToyOutlinedIcon from '@mui/icons-material/SmartToyOutlined'
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined'
import SendIcon from '@mui/icons-material/Send'
import SmartToyIcon from '@mui/icons-material/SmartToy'
import PersonIcon from '@mui/icons-material/Person'
import UploadFileOutlinedIcon from '@mui/icons-material/UploadFileOutlined'
import { useAuth } from '../context/AuthContext'
import {
  API_BASE_URL,
  addProjectForUser,
  deleteProject,
  deleteUserFile,
  getChatHistory,
  getPersonalDetails,
  getProjectsByUserId,
  getStudentProfile,
  getUserFiles,
  getUserSkills,
  initChatSession,
  saveUserFile,
  sendChatMessageWithReply,
  updatePersonalDetails,
  updateUserFile,
  updateUserSkills,
  uploadFileBinary,
} from '../services/api'
import ProjectList from '../components/ProjectList'

const profileSections = [
  { key: 'personal', label: 'Profile', icon: PersonOutlineOutlinedIcon },
  { key: 'skills', label: 'Skills & Portfolio', icon: SchoolOutlinedIcon },
  { key: 'files', label: 'Files', icon: FolderOpenOutlinedIcon },
  { key: 'ai', label: 'AI Studio', icon: SmartToyOutlinedIcon },
]

const buildApiOrigin = () => API_BASE_URL.replace(/\/api\/?$/, '')

/** Map API message row (camelCase or PascalCase) to UI shape */
const normalizeOneChatMessage = (m, historyId) => ({
  id: m.id ?? m.Id,
  date: m.date ?? m.Date,
  senderId: m.senderId ?? m.SenderId ?? null,
  messageContent: m.messageContent ?? m.MessageContent,
  chatHistoryId: m.chatHistoryId ?? m.ChatHistoryId ?? historyId,
  isAssistant: Boolean(m.isAssistant ?? m.IsAssistant),
})

const normalizeChatMessagesFromServer = (rawList, historyId) => {
  if (!Array.isArray(rawList)) {
    return []
  }
  return rawList
    .filter((m) => m && (m.messageContent ?? m.MessageContent))
    .map((m) => normalizeOneChatMessage(m, historyId))
}

const emptyPersonalForm = {
  id: '',
  userId: '',
  name: '',
  email: '',
  phone: '',
  address: '',
  bio: '',
}

const emptyProjectForm = {
  title: '',
  projectName: '',
  description: '',
  gitHubLink: '',
  projectsImages: '',
}

function ProfilePage() {
  const { user, logout } = useAuth()
  const [selectedSection, setSelectedSection] = useState('personal')
  const [profile, setProfile] = useState(null)
  const [personalForm, setPersonalForm] = useState(emptyPersonalForm)
  const [userSkills, setUserSkills] = useState([])
  const [files, setFiles] = useState([])
  const [chatHistoryId, setChatHistoryId] = useState('')
  const [chatMessages, setChatMessages] = useState([])
  const [chatDraft, setChatDraft] = useState('')
  const [isSubmittingMessage, setIsSubmittingMessage] = useState(false)
  const [isSavingDetails, setIsSavingDetails] = useState(false)
  const [isSavingSkills, setIsSavingSkills] = useState(false)
  const [isUploadingFile, setIsUploadingFile] = useState(false)
  const [projectForm, setProjectForm] = useState(emptyProjectForm)
  const [projectCoverFile, setProjectCoverFile] = useState(null)
  const [isSavingProject, setIsSavingProject] = useState(false)
  const [projects, setProjects] = useState([])
  const [error, setError] = useState('')
  const [successMessage, setSuccessMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const navigate = useNavigate()

  useEffect(() => {
    if (selectedSection === 'projects') {
      setSelectedSection('skills')
    }
  }, [selectedSection])

  const chatStorageKey = useMemo(
    () => (user?.userId ? `profile-chat-${user.userId}` : ''),
    [user?.userId],
  )

  const aiChatThreadRef = useRef(null)

  useEffect(() => {
    if (selectedSection !== 'ai') {
      return
    }
    const node = aiChatThreadRef.current
    if (node) {
      node.scrollTop = node.scrollHeight
    }
  }, [chatMessages, selectedSection])

  const hydrateChatMessages = (nextHistoryId) => {
    if (!chatStorageKey) {
      return []
    }

    try {
      const raw = localStorage.getItem(chatStorageKey)
      if (!raw) {
        return []
      }

      const parsed = JSON.parse(raw)
      if (!Array.isArray(parsed)) {
        return []
      }

      return parsed
        .filter((item) => item && item.chatHistoryId === nextHistoryId && item.messageContent)
        .map((item) => ({
          ...item,
          isAssistant: Boolean(item.isAssistant),
        }))
    } catch (storageError) {
      return []
    }
  }

  const persistChatMessages = (messages) => {
    if (!chatStorageKey) {
      return
    }
    localStorage.setItem(chatStorageKey, JSON.stringify(messages))
  }

  const reloadProjects = async () => {
    if (!user?.userId) {
      return
    }
    const list = await getProjectsByUserId(user.userId)
    setProjects(Array.isArray(list) ? list : [])
  }

  useEffect(() => {
    let isMounted = true

    const loadProfileArea = async () => {
      if (!user?.userId) {
        if (isMounted) {
          setError('Could not extract user id from token claims.')
          setIsLoading(false)
        }
        return
      }

      const userIdStr = String(user.userId).trim()

      // Chat must not depend on the rest of the profile loading — otherwise any failed
      // request blocks session init and Send becomes a silent no-op (!chatHistoryId).
      let existingHistory = null
      try {
        existingHistory =
          (await getChatHistory(userIdStr).catch(() => null)) ||
          (await initChatSession(userIdStr))
      } catch {
        existingHistory = null
      }

      if (isMounted) {
        const historyId = existingHistory?.id ?? existingHistory?.Id ?? ''
        const fromServer = normalizeChatMessagesFromServer(
          existingHistory?.messages ?? existingHistory?.Messages,
          historyId,
        )
        const fromStorage = hydrateChatMessages(historyId)
        const initialChatMessages =
          fromServer.length > 0 ? fromServer : fromStorage
        setChatHistoryId(historyId)
        setChatMessages(initialChatMessages)
        if (fromServer.length > 0) {
          persistChatMessages(initialChatMessages)
        }
        if (!historyId) {
          setError(
            'Chat session could not be started. Check that the API is running, then refresh.',
          )
        }
      }

      try {
        const personalDetailsResponse = await getPersonalDetails(userIdStr).catch(
          () => null,
        )
        const [
          userSkillsResponse,
          filesResponse,
          projectsResponse,
          profileResponse,
        ] = await Promise.all([
          getUserSkills(userIdStr).catch(() => []),
          getUserFiles(userIdStr).catch(() => []),
          getProjectsByUserId(userIdStr).catch(() => []),
          getStudentProfile(userIdStr).catch(() => null),
        ])

        if (isMounted) {
          setProfile(profileResponse || null)
          setPersonalForm({
            id: personalDetailsResponse?.id || '',
            userId: personalDetailsResponse?.userId || userIdStr,
            name: personalDetailsResponse?.name || '',
            email: personalDetailsResponse?.email || user.email || '',
            phone: personalDetailsResponse?.phone || '',
            address: personalDetailsResponse?.address || '',
            bio: personalDetailsResponse?.bio || '',
          })
          setUserSkills(
            Array.isArray(userSkillsResponse)
              ? userSkillsResponse.map((skill) => ({
                  ...skill,
                  isSelected: true,
                }))
              : [],
          )
          setFiles(Array.isArray(filesResponse) ? filesResponse : [])
          setProjects(Array.isArray(projectsResponse) ? projectsResponse : [])
        }
      } catch (error) {
        if (isMounted) {
          setError(
            error?.response?.data?.message ||
              error?.response?.data ||
              'Failed to load student profile.',
          )
        }
      } finally {
        if (isMounted) {
          setIsLoading(false)
        }
      }
    }

    loadProfileArea()

    return () => {
      isMounted = false
    }
  }, [user?.userId, user?.email, chatStorageKey])

  const handleLogout = () => {
    logout()
    navigate('/login', { replace: true })
  }

  const showSuccess = (message) => {
    setSuccessMessage(message)
    setTimeout(() => {
      setSuccessMessage('')
    }, 2200)
  }

  const handlePersonalChange = (field) => (event) => {
    setPersonalForm((previous) => ({
      ...previous,
      [field]: event.target.value,
    }))
  }

  const handleSavePersonalDetails = async () => {
    if (!user?.userId) {
      return
    }

    setIsSavingDetails(true)
    setError('')
    try {
      const payload = {
        ...personalForm,
        id: personalForm.id || crypto.randomUUID(),
        userId: personalForm.userId || user.userId,
      }
      const updated = await updatePersonalDetails(payload)
      setPersonalForm({
        id: updated.id,
        userId: updated.userId,
        name: updated.name || '',
        email: updated.email || '',
        phone: updated.phone || '',
        address: updated.address || '',
        bio: updated.bio || '',
      })
      showSuccess('Profile updated successfully.')
    } catch (requestError) {
      setError(requestError?.response?.data || 'Failed to update profile details.')
    } finally {
      setIsSavingDetails(false)
    }
  }

  const toggleSkill = (skillId) => {
    setUserSkills((previous) =>
      previous.map((skill) =>
        skill.skillId === skillId
          ? {
              ...skill,
              isSelected: !skill.isSelected,
            }
          : skill,
      ),
    )
  }

  const updateYears = (skillId, years) => {
    setUserSkills((previous) =>
      previous.map((skill) =>
        skill.skillId === skillId
          ? {
              ...skill,
              yearsOfExperience: Number(years) || 0,
            }
          : skill,
      ),
    )
  }

  const handleSaveSkills = async () => {
    if (!user?.userId) {
      return
    }

    setIsSavingSkills(true)
    setError('')
    try {
      const selectedSkills = userSkills
        .filter((skill) => skill.isSelected)
        .map((skill) => ({
          userId: user.userId,
          skillId: skill.skillId,
          yearsOfExperience: Number(skill.yearsOfExperience) || 0,
        }))

      await updateUserSkills(user.userId, selectedSkills)
      showSuccess('Skills saved successfully.')
    } catch (requestError) {
      setError(requestError?.response?.data || 'Failed to save skills.')
    } finally {
      setIsSavingSkills(false)
    }
  }

  const resolveFileType = (file) => {
    if (file.type) {
      return file.type
    }
    const extension = file.name.split('.').pop()
    return extension ? `.${extension}` : 'unknown'
  }

  const reloadFiles = async () => {
    if (!user?.userId) {
      return
    }
    const nextFiles = await getUserFiles(user.userId)
    setFiles(Array.isArray(nextFiles) ? nextFiles : [])
  }

  const handleUploadNewFile = async (event) => {
    const file = event.target.files?.[0]
    if (!file || !user?.userId) {
      return
    }

    setIsUploadingFile(true)
    setError('')
    try {
      const uploadResponse = await uploadFileBinary(file)
      await saveUserFile({
        id: crypto.randomUUID(),
        userId: user.userId,
        fileName: file.name,
        fileType: resolveFileType(file),
        uploadDate: new Date().toISOString(),
        filePath: uploadResponse?.url || null,
      })
      await reloadFiles()
      showSuccess('File uploaded successfully.')
    } catch (requestError) {
      setError(requestError?.response?.data || 'Failed to upload file.')
    } finally {
      setIsUploadingFile(false)
      event.target.value = ''
    }
  }

  const handleDeleteFile = async (documentId) => {
    setError('')
    try {
      await deleteUserFile(documentId)
      await reloadFiles()
      showSuccess('File deleted.')
    } catch (requestError) {
      setError(requestError?.response?.data || 'Failed to delete file.')
    }
  }

  const handleEditFile = async (file) => {
    const nextName = window.prompt('New file name', file.fileName)
    if (!nextName || nextName.trim() === '' || nextName === file.fileName) {
      return
    }

    setError('')
    try {
      await updateUserFile(file.id, {
        ...file,
        fileName: nextName.trim(),
      })
      await reloadFiles()
      showSuccess('File updated.')
    } catch (requestError) {
      setError(requestError?.response?.data || 'Failed to update file.')
    }
  }

  const handleDownloadFile = (filePath) => {
    if (!filePath) {
      return
    }
    const absoluteUrl = filePath.startsWith('http')
      ? filePath
      : `${buildApiOrigin()}${filePath}`
    window.open(absoluteUrl, '_blank', 'noopener,noreferrer')
  }

  const handleSendMessage = async () => {
    const text = chatDraft.trim()
    const uid = user?.userId != null ? String(user.userId).trim() : ''
    if (!text || !uid) {
      return
    }

    setIsSubmittingMessage(true)
    setError('')
    try {
      let hid = chatHistoryId
      if (!hid) {
        const history =
          (await getChatHistory(uid).catch(() => null)) ||
          (await initChatSession(uid))
        hid = history?.id ?? history?.Id ?? ''
        if (hid) {
          setChatHistoryId(hid)
        }
      }
      if (!hid) {
        setError('Could not start a chat session. Is the API running? Try refreshing the page.')
        return
      }

      const payload = {
        id: crypto.randomUUID(),
        date: new Date().toISOString(),
        senderId: uid,
        messageContent: text,
        chatHistoryId: hid,
      }

      const savedList = await sendChatMessageWithReply(payload, resumePreviewText)
      const list = Array.isArray(savedList) ? savedList : []
      const normalized = list.map((row) => normalizeOneChatMessage(row, hid))
      const nextMessages = [...chatMessages, ...normalized]
      setChatMessages(nextMessages)
      persistChatMessages(nextMessages)
      setChatDraft('')
    } catch (requestError) {
      setError(requestError?.response?.data || 'Failed to send message.')
    } finally {
      setIsSubmittingMessage(false)
    }
  }

  const handleProjectFieldChange = (field) => (event) => {
    setProjectForm((previous) => ({
      ...previous,
      [field]: event.target.value,
    }))
  }

  const handleAddProject = async () => {
    if (!user?.userId) {
      return
    }

    const title = projectForm.title.trim()
    if (!title) {
      setError('Project title is required.')
      return
    }

    setIsSavingProject(true)
    setError('')
    try {
      let imagePath = projectForm.projectsImages.trim() || null
      if (projectCoverFile) {
        const uploadResponse = await uploadFileBinary(projectCoverFile)
        imagePath = uploadResponse?.url || imagePath
      }

      const formData = new FormData()
      formData.append('Title', title)
      formData.append(
        'ProjectName',
        projectForm.projectName.trim() || title,
      )
      formData.append('Description', projectForm.description || '')
      formData.append('GitHubLink', projectForm.gitHubLink || '')
      if (imagePath) {
        formData.append('ProjectsImages', imagePath)
      }

      await addProjectForUser(user.userId, formData)
      setProjectForm(emptyProjectForm)
      setProjectCoverFile(null)
      await reloadProjects()
      showSuccess('Project added to your profile.')
    } catch (requestError) {
      setError(
        requestError?.response?.data ||
          requestError?.message ||
          'Failed to add project.',
      )
    } finally {
      setIsSavingProject(false)
    }
  }

  const handleDeleteProject = async (projectId) => {
    if (!window.confirm('Delete this project?')) {
      return
    }
    setError('')
    try {
      await deleteProject(projectId)
      await reloadProjects()
      showSuccess('Project removed.')
    } catch (requestError) {
      setError(requestError?.response?.data || 'Failed to delete project.')
    }
  }

  const resumePreviewText = useMemo(() => {
    const selectedSkills = userSkills.filter((skill) => skill.isSelected)
    const skillLines =
      selectedSkills.length > 0
        ? selectedSkills
            .map((skill) => {
              const idx = userSkills.findIndex((s) => s.skillId === skill.skillId)
              const label =
                idx >= 0 && profile?.skills?.[idx]
                  ? profile.skills[idx]
                  : String(skill.skillId).slice(0, 8)
              return `- ${label} (${skill.yearsOfExperience}y)`
            })
            .join('\n')
        : '- No skills selected yet'

    return [
      `Full Name: ${personalForm.name || '-'}`,
      `Email: ${personalForm.email || '-'}`,
      `Phone: ${personalForm.phone || '-'}`,
      `Address: ${personalForm.address || '-'}`,
      '',
      'Bio',
      personalForm.bio || '-',
      '',
      'Skills',
      skillLines,
      '',
      'Projects',
      ...(projects.length > 0
        ? projects.map((project) => `- ${project.title || project.projectName || 'Untitled Project'}`)
        : ['- No projects yet']),
    ].join('\n')
  }, [personalForm, userSkills, projects, profile?.skills])

  const handleCopyResumePreview = async () => {
    try {
      await navigator.clipboard.writeText(resumePreviewText)
      showSuccess('Resume text copied — paste it into the chat or an AI tool.')
    } catch {
      setError('Could not copy to clipboard.')
    }
  }

  return (
    <section className="profile-area page">
      <div className="profile-area__header">
        <div>
          <h2>Personal Area</h2>
          <p>Manage your profile, skills &amp; portfolio, files and AI resume studio.</p>
        </div>
        <button className="btn-primary" onClick={handleLogout}>
          Logout
        </button>
      </div>

      {isLoading ? <p>Loading profile...</p> : null}
      {error ? <p className="error-text">{error}</p> : null}
      {successMessage ? <p className="success-text">{successMessage}</p> : null}

      <div className="profile-area__content">
        <aside className="profile-sidebar">
          {profileSections.map((section) => {
            const Icon = section.icon
            return (
              <button
                key={section.key}
                className={
                  selectedSection === section.key
                    ? 'profile-sidebar__item is-active'
                    : 'profile-sidebar__item'
                }
                onClick={() => setSelectedSection(section.key)}
              >
                <Icon fontSize="small" />
                <span>{section.label}</span>
              </button>
            )
          })}
        </aside>

        <div className="profile-panel">
          {selectedSection === 'personal' ? (
            <div className="profile-card">
              <h3>Personal Details</h3>
              <div className="profile-form-grid">
                <label>
                  Full Name
                  <input value={personalForm.name} onChange={handlePersonalChange('name')} />
                </label>
                <label>
                  Email
                  <input value={personalForm.email} onChange={handlePersonalChange('email')} />
                </label>
                <label>
                  Phone
                  <input value={personalForm.phone} onChange={handlePersonalChange('phone')} />
                </label>
                <label>
                  Address
                  <input value={personalForm.address} onChange={handlePersonalChange('address')} />
                </label>
              </div>
              <label className="profile-form-textarea">
                Biography
                <textarea value={personalForm.bio} onChange={handlePersonalChange('bio')} rows={5} />
              </label>
              <button
                className="btn-primary"
                onClick={handleSavePersonalDetails}
                disabled={isSavingDetails}
              >
                <SaveOutlinedIcon fontSize="small" /> Save Changes
              </button>
            </div>
          ) : null}

          {selectedSection === 'skills' ? (
            <div className="profile-card skills-portfolio-card">
              <h3>Skills &amp; Portfolio</h3>
              <p className="muted-text">
                Show what you know (skills) and what you have built (portfolio projects) in one place.
              </p>

              <div className="skills-portfolio-block">
                <h4 className="skills-portfolio-subtitle">Skills</h4>
                <p className="muted-text skills-portfolio-hint">
                  Loaded from your <code>skillToUser</code> relation; save with PUT.
                </p>
                {userSkills.length === 0 ? <p>No skills assigned yet.</p> : null}
                <div className="skills-table">
                  {userSkills.map((skill, index) => (
                    <div className="skills-row" key={skill.skillId}>
                      <label className="skills-row__name">
                        <input
                          type="checkbox"
                          checked={skill.isSelected}
                          onChange={() => toggleSkill(skill.skillId)}
                        />
                        <span>
                          {profile?.skills?.[index] || `Skill ${skill.skillId.slice(0, 8)}`}
                        </span>
                      </label>
                      <label className="skills-row__years">
                        Years
                        <input
                          type="number"
                          min="0"
                          value={skill.yearsOfExperience}
                          onChange={(event) => updateYears(skill.skillId, event.target.value)}
                        />
                      </label>
                    </div>
                  ))}
                </div>
                <button className="btn-primary" onClick={handleSaveSkills} disabled={isSavingSkills}>
                  <SaveOutlinedIcon fontSize="small" /> Save Skills
                </button>
              </div>

              <hr className="portfolio-divider" aria-hidden="true" />

              <div className="skills-portfolio-block portfolio-block">
                <h4 className="skills-portfolio-subtitle" id="my-portfolio-heading">
                  My Portfolio
                </h4>
                <p className="muted-text skills-portfolio-hint">
                  Add projects with{' '}
                  <code>POST /api/project/user/:userId</code> (JWT required).
                </p>
                <div className="profile-form-grid">
                  <label>
                    Title
                    <input
                      value={projectForm.title}
                      onChange={handleProjectFieldChange('title')}
                      placeholder="e.g. Portfolio website"
                    />
                  </label>
                  <label>
                    Project name (short)
                    <input
                      value={projectForm.projectName}
                      onChange={handleProjectFieldChange('projectName')}
                      placeholder="Optional display name"
                    />
                  </label>
                  <label>
                    GitHub link
                    <input
                      value={projectForm.gitHubLink}
                      onChange={handleProjectFieldChange('gitHubLink')}
                      placeholder="https://github.com/..."
                    />
                  </label>
                  <label>
                    Cover image URL
                    <input
                      value={projectForm.projectsImages}
                      onChange={handleProjectFieldChange('projectsImages')}
                      placeholder="/uploads/... or full URL"
                    />
                  </label>
                </div>
                <label className="profile-form-textarea">
                  Description
                  <textarea
                    value={projectForm.description}
                    onChange={handleProjectFieldChange('description')}
                    rows={4}
                    placeholder="What did you build?"
                  />
                </label>
                <label className="profile-form-cover">
                  Or upload cover image
                  <input
                    type="file"
                    accept="image/*"
                    onChange={(event) => {
                      setProjectCoverFile(event.target.files?.[0] || null)
                    }}
                  />
                </label>
                <button
                  className="btn-primary"
                  type="button"
                  onClick={handleAddProject}
                  disabled={isSavingProject}
                >
                  <SaveOutlinedIcon fontSize="small" /> Add project
                </button>

                <div className="projects-section projects-section--inline" aria-labelledby="my-portfolio-heading">
                  <h3 className="projects-section__list-title">Your projects</h3>
                  <ProjectList projects={projects} onDelete={handleDeleteProject} />
                </div>
              </div>
            </div>
          ) : null}

          {selectedSection === 'files' ? (
            <div className="profile-card">
              <div className="files-header">
                <h3>Files</h3>
                <label className="btn-primary btn-upload">
                  <UploadFileOutlinedIcon fontSize="small" /> Upload New File
                  <input type="file" onChange={handleUploadNewFile} disabled={isUploadingFile} />
                </label>
              </div>
              <div className="files-table-wrap">
                <table className="files-table">
                  <thead>
                    <tr>
                      <th>File Name</th>
                      <th>Date</th>
                      <th>Size</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {files.map((file) => (
                      <tr key={file.id}>
                        <td>{file.fileName}</td>
                        <td>{new Date(file.uploadDate).toLocaleDateString()}</td>
                        <td>{file.fileType || '-'}</td>
                        <td className="file-actions">
                          <button onClick={() => handleDownloadFile(file.filePath)}>Download</button>
                          <button onClick={() => handleEditFile(file)}>Edit</button>
                          <button onClick={() => handleDeleteFile(file.id)}>Delete</button>
                        </td>
                      </tr>
                    ))}
                    {files.length === 0 ? (
                      <tr>
                        <td colSpan={4}>No files yet.</td>
                      </tr>
                    ) : null}
                  </tbody>
                </table>
              </div>
            </div>
          ) : null}

          {selectedSection === 'ai' ? (
            <div className="profile-card ai-grid">
              <div className="ai-chat-panel">
                <header className="ai-chat-header">
                  <div className="ai-chat-header__avatar" aria-hidden="true">
                    <SmartToyOutlinedIcon />
                  </div>
                  <div className="ai-chat-header__text">
                    <h3 className="ai-chat-header__title">AI Studio</h3>
                    <p className="ai-chat-header__subtitle">
                      עוזר קריירה — הודעות נשמרות בשרת. ללא מפתח OpenAI בשרת תקבל הסבר איך להפעיל;
                      עם מפתח — תשובות אוטומטיות (הקשר מסיכום הקו״ח מצורף אוטומטית לשליחה).
                    </p>
                  </div>
                </header>

                <div className="ai-chat-thread" ref={aiChatThreadRef} role="log" aria-live="polite">
                  {chatMessages.length === 0 ? (
                    <p className="ai-chat-empty muted-text">
                      אין הודעות עדיין. כתוב למטה כדי להתחיל שיחה.
                    </p>
                  ) : null}
                  {chatMessages.map((message) => (
                    <div
                      key={message.id}
                      className={
                        message.isAssistant
                          ? 'ai-chat-row ai-chat-row--assistant'
                          : 'ai-chat-row ai-chat-row--user'
                      }
                    >
                      <div className="ai-chat-row__avatar" aria-hidden="true">
                        {message.isAssistant ? (
                          <SmartToyIcon fontSize="small" />
                        ) : (
                          <PersonIcon fontSize="small" />
                        )}
                      </div>
                      <div className="ai-chat-bubble-wrap">
                        <div className="ai-chat-bubble-meta">
                          {message.isAssistant ? 'עוזר' : 'את/ה'} ·{' '}
                          {new Date(message.date).toLocaleString()}
                        </div>
                        <div
                          className={
                            message.isAssistant ? 'ai-chat-bubble ai-chat-bubble--assistant' : 'ai-chat-bubble ai-chat-bubble--user'
                          }
                        >
                          <p className="ai-chat-bubble__text">{message.messageContent}</p>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>

                <div className="ai-chat-composer">
                  <textarea
                    className="ai-chat-input"
                    rows={2}
                    value={chatDraft}
                    onChange={(event) => setChatDraft(event.target.value)}
                    onKeyDown={(event) => {
                      if (event.key === 'Enter' && !event.shiftKey) {
                        event.preventDefault()
                        handleSendMessage()
                      }
                    }}
                    placeholder="כתוב הודעה… (Enter לשליחה, Shift+Enter לשורה חדשה)"
                    disabled={isSubmittingMessage}
                  />
                  <button
                    type="button"
                    className="ai-chat-send"
                    onClick={handleSendMessage}
                    disabled={isSubmittingMessage || !chatDraft.trim()}
                    title="שלח"
                    aria-label="שלח הודעה"
                  >
                    {isSubmittingMessage ? (
                      <span className="ai-chat-send__spinner" />
                    ) : (
                      <SendIcon sx={{ fontSize: 20 }} />
                    )}
                  </button>
                </div>
              </div>

              <div>
                <div className="resume-preview-header">
                  <h3>Resume summary (from your profile)</h3>
                  <button type="button" className="btn-secondary" onClick={handleCopyResumePreview}>
                    Copy text
                  </button>
                </div>
                <p className="muted-text">
                  Plain-text snapshot of your profile data. Copy and paste into the chat or export to
                  PDF elsewhere if needed.
                </p>
                <pre className="resume-preview">{resumePreviewText}</pre>
              </div>
            </div>
          ) : null}
        </div>
      </div>
    </section>
  )
}

export default ProfilePage
