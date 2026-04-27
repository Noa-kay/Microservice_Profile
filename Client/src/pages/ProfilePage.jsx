import { useEffect, useId, useMemo, useRef, useState } from 'react'
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
import LogoutOutlinedIcon from '@mui/icons-material/LogoutOutlined'
import AddCircleOutlineOutlinedIcon from '@mui/icons-material/AddCircleOutlineOutlined'
import {
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  IconButton,
} from '@mui/material'
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
  updateStudentProfile,
  updateUserFile,
  updateUserSkills,
  uploadFileBinary,
} from '../services/api'
import ProjectList from '../components/ProjectList'

const profileSections = [
  { key: 'personal', label: 'פרופיל', icon: PersonOutlineOutlinedIcon },
  { key: 'skills', label: 'תיק עבודות ורזומה', icon: SchoolOutlinedIcon },
  { key: 'files', label: 'קבצים', icon: FolderOpenOutlinedIcon },
  { key: 'ai', label: 'בוט AI', icon: SmartToyOutlinedIcon },
]

/** שמות מיומנויות נפוצים באנגלית — תצוגה בעברית (השמירה בשרת נשארת לפי הנתונים). */
const SKILL_DISPLAY_HE = Object.freeze({
  javascript: "ג'אווהסקריפט",
  js: "ג'אווהסקריפט",
  typescript: 'טייפסקריפט',
  ts: 'טייפסקריפט',
  react: 'ריאקט',
  angular: 'אנגולר',
  vue: 'Vue.js',
  vuejs: 'Vue.js',
  nodejs: 'Node.js',
  node: 'Node.js',
  html: 'HTML',
  css: 'CSS',
  sass: 'Sass',
  scss: 'SCSS',
  sql: 'SQL',
  postgresql: 'PostgreSQL',
  postgres: 'PostgreSQL',
  mysql: 'MySQL',
  mongodb: 'MongoDB',
  redis: 'Redis',
  docker: 'דוקר',
  kubernetes: 'קוברנטיס',
  aws: 'AWS',
  azure: 'Azure',
  gcp: 'Google Cloud',
  git: 'גיט',
  github: 'גיטהאב',
  csharp: 'סי שארפ',
  'c#': 'סי שארפ',
  dotnet: '.NET',
  php: 'PHP',
  python: 'פייתון',
  java: 'ג׳אווה',
  kotlin: 'קוטלין',
  swift: 'סוויפט',
  go: 'גו',
  golang: 'גו',
  rust: 'ראסט',
  ruby: 'רובי',
  rails: 'Ruby on Rails',
  flutter: 'פלאטר',
  dart: 'דארט',
  figma: 'פיגמה',
  mui: 'Material UI',
  bootstrap: 'בוטסטרפ',
  tailwind: 'טיילווינד CSS',
  tailwindcss: 'טיילווינד CSS',
  excel: 'אקסל',
  powerbi: 'Power BI',
  jquery: 'jQuery',
  webpack: 'Webpack',
  vite: 'Vite',
  'c++': 'סי פלוס פלוס',
  cpp: 'סי פלוס פלוס',
})

function skillNameForDisplay(rawName, fallbackSkillId) {
  const trimmed =
    rawName != null && String(rawName).trim() !== '' ? String(rawName).trim() : ''
  if (trimmed) {
    const lower = trimmed.toLowerCase()
    const compact = lower.replace(/\s+/g, '').replace(/\./g, '')
    return SKILL_DISPLAY_HE[lower] || SKILL_DISPLAY_HE[compact] || trimmed
  }
  return fallbackSkillId
    ? `מיומנות (${String(fallbackSkillId).slice(0, 8)})`
    : 'מיומנות'
}

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
  const [manualSkillInput, setManualSkillInput] = useState('')
  const [manualSkills, setManualSkills] = useState([])
  const [error, setError] = useState('')
  const [successMessage, setSuccessMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [confirmDelete, setConfirmDelete] = useState(null)
  const [isConfirmDeleting, setIsConfirmDeleting] = useState(false)
  const navigate = useNavigate()
  const portfolioCoverInputId = useId()
  const profileFilesUploadId = useId()

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
          setError('לא ניתן לזהות את המשתמש מהאסימון. נסו להתחבר מחדש.')
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
            'לא ניתן להתחיל שיחה. ודאו שהשרת פועל ורעננו את הדף.',
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
              'טעינת הפרופיל נכשלה.',
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
      showSuccess('הפרטים האישיים נשמרו בהצלחה.')
    } catch (requestError) {
      setError(requestError?.response?.data || 'שמירת הפרטים האישיים נכשלה.')
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

  const addManualSkill = () => {
    const value = manualSkillInput.trim()
    if (!value) {
      return
    }
    const duplicateInManual = manualSkills.some(
      (item) => item.toLowerCase() === value.toLowerCase(),
    )
    if (duplicateInManual) {
      return
    }
    setManualSkills((previous) => [...previous, value])
    setManualSkillInput('')
  }

  const removeManualSkill = (name) => {
    setManualSkills((previous) => previous.filter((item) => item !== name))
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
      const selectedSkillNames = userSkills
        .filter((skill) => skill.isSelected)
        .map((skill) => {
          const idx = userSkills.findIndex((s) => s.skillId === skill.skillId)
          return profile?.skills?.[idx] || ''
        })
        .filter(Boolean)

      const selectedSkills = userSkills
        .filter((skill) => skill.isSelected)
        .map((skill) => ({
          userId: user.userId,
          skillId: skill.skillId,
          yearsOfExperience: Number(skill.yearsOfExperience) || 0,
        }))

      await updateUserSkills(user.userId, selectedSkills)
      if (manualSkills.length > 0 || selectedSkillNames.length > 0) {
        await updateStudentProfile({
          userId: user.userId,
          name: personalForm.name || '',
          email: personalForm.email || '',
          phone: personalForm.phone || '',
          address: personalForm.address || '',
          bio: personalForm.bio || '',
          skills: Array.from(new Set([...selectedSkillNames, ...manualSkills])),
        })
      }
      const [nextProfile, nextUserSkills] = await Promise.all([
        getStudentProfile(user.userId).catch(() => null),
        getUserSkills(user.userId).catch(() => []),
      ])
      setProfile(nextProfile || profile)
      setUserSkills(
        Array.isArray(nextUserSkills)
          ? nextUserSkills.map((skill) => ({
              ...skill,
              isSelected: true,
            }))
          : [],
      )
      setManualSkills([])
      showSuccess('המיומנויות נשמרו בהצלחה.')
    } catch (requestError) {
      setError(requestError?.response?.data || 'שמירת המיומנויות נכשלה.')
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
      showSuccess('הקובץ הועלה בהצלחה.')
    } catch (requestError) {
      setError(requestError?.response?.data || 'העלאת הקובץ נכשלה.')
    } finally {
      setIsUploadingFile(false)
      event.target.value = ''
    }
  }

  const handleDeleteFile = (documentId) => {
    setConfirmDelete({ type: 'file', id: documentId })
  }

  const handleCloseConfirmDelete = () => {
    if (!isConfirmDeleting) {
      setConfirmDelete(null)
    }
  }

  const handleConfirmDelete = async () => {
    if (!confirmDelete) {
      return
    }
    setIsConfirmDeleting(true)
    setError('')
    try {
      if (confirmDelete.type === 'file') {
        await deleteUserFile(confirmDelete.id)
      await reloadFiles()
        showSuccess('הקובץ נמחק.')
      } else {
        await deleteProject(confirmDelete.id)
        await reloadProjects()
        showSuccess('הפרויקט הוסר.')
      }
      setConfirmDelete(null)
    } catch (requestError) {
      setError(
        requestError?.response?.data ||
          (confirmDelete.type === 'file'
            ? 'מחיקת הקובץ נכשלה.'
            : 'מחיקת הפרויקט נכשלה.'),
      )
    } finally {
      setIsConfirmDeleting(false)
    }
  }

  const handleEditFile = async (file) => {
    const nextName = window.prompt('שם קובץ חדש', file.fileName)
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
      showSuccess('שם הקובץ עודכן.')
    } catch (requestError) {
      setError(requestError?.response?.data || 'עדכון הקובץ נכשל.')
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
        setError('לא ניתן להתחיל שיחה. ודאו שהשרת פועל ורעננו את הדף.')
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
      setError(requestError?.response?.data || 'שליחת ההודעה נכשלה.')
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
      setError('נדרשת כותרת לפרויקט.')
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
      showSuccess('הפרויקט נוסף לתיק.')
    } catch (requestError) {
      setError(
        requestError?.response?.data ||
          requestError?.message ||
          'הוספת הפרויקט נכשלה.',
      )
    } finally {
      setIsSavingProject(false)
    }
  }

  const handleDeleteProject = (projectId) => {
    setConfirmDelete({ type: 'project', id: projectId })
  }

  const resumePreviewText = useMemo(() => {
    const selectedSkills = userSkills.filter((skill) => skill.isSelected)
    const skillLines =
      selectedSkills.length > 0
        ? selectedSkills
            .map((skill) => {
              const idx = userSkills.findIndex((s) => s.skillId === skill.skillId)
              const raw =
                idx >= 0 && profile?.skills?.[idx] ? profile.skills[idx] : ''
              const label = skillNameForDisplay(raw, skill.skillId)
              return `- ${label} (${skill.yearsOfExperience} שנות ניסיון)`
            })
            .join('\n')
        : '- לא נבחרו מיומנויות'

    return [
      `שם מלא: ${personalForm.name || '-'}`,
      `דוא״ל: ${personalForm.email || '-'}`,
      `טלפון: ${personalForm.phone || '-'}`,
      `כתובת: ${personalForm.address || '-'}`,
      '',
      'אודות',
      personalForm.bio || '-',
      '',
      'מיומנויות',
      skillLines,
      '',
      'פרויקטים',
      ...(projects.length > 0
        ? projects.map(
            (project) =>
              `- ${project.title || project.projectName || 'פרויקט ללא כותרת'}`,
          )
        : ['- אין פרויקטים עדיין']),
    ].join('\n')
  }, [personalForm, userSkills, projects, profile?.skills])

  const handleCopyResumePreview = async () => {
    try {
      await navigator.clipboard.writeText(resumePreviewText)
      showSuccess('הטקסט הועתק — ניתן להדביק בצ׳אט או במסמך.')
    } catch {
      setError('לא ניתן להעתיק ללוח.')
    }
  }

  return (
    <section className="profile-area page" dir="rtl" lang="he">
      <div className="profile-area__header">
        <div className="profile-area__header-logout-wrap">
          <Button
            type="button"
            variant="contained"
            size="medium"
            startIcon={<LogoutOutlinedIcon />}
            onClick={handleLogout}
            sx={{
              fontWeight: 800,
              px: 2.5,
              '& .MuiButton-startIcon': { mr: 0, ml: 1 },
            }}
          >
            יציאה מהמערכת
          </Button>
        </div>
        <div className="profile-area__header-center">
          <h2 className="profile-area__title">אזור אישי</h2>
          <p className="profile-area__subtitle">
            ניהול פרופיל, תיק עבודות ומיומנויות, קבצים וסטודיו AI לקורות חיים.
          </p>
        </div>
      </div>

      {isLoading ? <p>טוען פרופיל…</p> : null}
      {error ? <div className="status-card status-card--error">{error}</div> : null}
      {successMessage ? <div className="status-card status-card--success">{successMessage}</div> : null}

      <div className="profile-area__content">
        <aside className="profile-sidebar">
          {profileSections.map((section) => {
            const Icon = section.icon
            const active = selectedSection === section.key
            return (
              <Button
                key={section.key}
                type="button"
                variant={active ? 'contained' : 'outlined'}
                onClick={() => setSelectedSection(section.key)}
                startIcon={<Icon fontSize="small" />}
                sx={{
                  flex: '1 1 190px',
                  minHeight: 48,
                  borderRadius: 2,
                  py: 1,
                  '& .MuiButton-startIcon': { mr: 0, ml: 1 },
                }}
              >
                {section.label}
              </Button>
            )
          })}
        </aside>

        <div className="profile-panel">
          {selectedSection === 'personal' ? (
            <div className="profile-card">
              <h3>פרטים אישיים</h3>
              <div className="profile-form-grid">
                <label>
                  שם מלא
                  <input value={personalForm.name} onChange={handlePersonalChange('name')} />
                </label>
                <label>
                  דוא״ל
                  <input
                    type="email"
                    value={personalForm.email}
                    onChange={handlePersonalChange('email')}
                    dir="ltr"
                  />
                </label>
                <label>
                  טלפון
                  <input value={personalForm.phone} onChange={handlePersonalChange('phone')} />
                </label>
                <label>
                  כתובת
                  <input value={personalForm.address} onChange={handlePersonalChange('address')} />
                </label>
              </div>
              <label className="profile-form-textarea">
                אודות (ביוגרפיה)
                <textarea value={personalForm.bio} onChange={handlePersonalChange('bio')} rows={5} />
              </label>
              <div className="profile-save-wrap">
                <Button
                  type="button"
                  variant="contained"
                  size="large"
                  startIcon={<SaveOutlinedIcon />}
                onClick={handleSavePersonalDetails}
                disabled={isSavingDetails}
                  sx={{
                    minWidth: 280,
                    fontWeight: 800,
                    py: 1.25,
                    px: 3,
                    borderRadius: 2,
                    '& .MuiButton-startIcon': { mr: 0, ml: 1 },
                  }}
                >
                  שמירת שינויים בפרופיל
                </Button>
              </div>
            </div>
          ) : null}

          {selectedSection === 'skills' ? (
            <div className="profile-card skills-portfolio-card">
              <h3>תיק עבודות ומיומנויות</h3>
              <p className="muted-text">
                למעלה: רשימת הפרויקטים שלך. מתחתיה: טופס להוספת פרויקט. בסוף העמוד: מיומנויות —
                שמרו לאחר עדכון.
              </p>

              <div className="portfolio-layout" id="my-portfolio-heading">
                <section className="portfolio-projects-panel" aria-labelledby="portfolio-list-title">
                  <div className="portfolio-projects-panel__head">
                    <h4 className="portfolio-projects-panel__title" id="portfolio-list-title">
                      הפרויקטים שלך
                    </h4>
                    <p className="muted-text portfolio-projects-panel__subtitle">
                      {projects.length === 0
                        ? 'עדיין אין פרויקטים — ניתן להוסיף מהטופס למטה.'
                        : `${projects.length} פרויקטים בתיק`}
                    </p>
                    </div>
                  <ProjectList projects={projects} onDelete={handleDeleteProject} />
                </section>

                <section className="portfolio-add-panel" aria-labelledby="portfolio-add-title">
                  <div className="portfolio-add-panel__header">
                    <h4 className="portfolio-add-panel__title" id="portfolio-add-title">
                      הוספת פרויקט לתיק
                </h4>
                    <p className="portfolio-add-panel__hint muted-text">
                      מלאו את הפרטים ושמרו — הפרויקט יתווסף לרשימה למעלה.
                    </p>
                  </div>
                  <div className="portfolio-form-grid">
                    <label className="portfolio-field portfolio-field--span-2">
                      כותרת הפרויקט
                    <input
                      value={projectForm.title}
                      onChange={handleProjectFieldChange('title')}
                        placeholder="למשל: אתר תדמית לעסק"
                    />
                  </label>
                    <label className="portfolio-field">
                      שם קצר (אופציונלי)
                    <input
                      value={projectForm.projectName}
                      onChange={handleProjectFieldChange('projectName')}
                        placeholder="שם תצוגה"
                    />
                  </label>
                    <label className="portfolio-field">
                      קישור GitHub
                    <input
                      value={projectForm.gitHubLink}
                      onChange={handleProjectFieldChange('gitHubLink')}
                      placeholder="https://github.com/..."
                        dir="ltr"
                    />
                  </label>
                    <label className="portfolio-field portfolio-field--span-2">
                      כתובת תמונת כיסוי (URL)
                    <input
                      value={projectForm.projectsImages}
                      onChange={handleProjectFieldChange('projectsImages')}
                        placeholder="/uploads/... או קישור מלא"
                        dir="ltr"
                    />
                  </label>
                </div>
                  <label className="profile-form-textarea portfolio-field--full">
                    תיאור
                  <textarea
                    value={projectForm.description}
                    onChange={handleProjectFieldChange('description')}
                    rows={4}
                      placeholder="מה בניתם? אילו טכנולוגיות? תוצאה מרכזית?"
                  />
                </label>
                  <div className="portfolio-upload-row portfolio-upload-row--stack">
                    <span className="portfolio-upload-caption">או העלאת תמונת כיסוי מהמחשב</span>
                  <input
                      id={portfolioCoverInputId}
                    type="file"
                    accept="image/*"
                      style={{ display: 'none' }}
                    onChange={(event) => {
                      setProjectCoverFile(event.target.files?.[0] || null)
                    }}
                  />
                    <Button
                      component="label"
                      variant="outlined"
                      htmlFor={portfolioCoverInputId}
                      startIcon={<UploadFileOutlinedIcon />}
                      sx={{
                        alignSelf: 'flex-start',
                        py: 1.25,
                        px: 2,
                        '& .MuiButton-startIcon': { mr: 0, ml: 1 },
                      }}
                    >
                      בחירת קובץ תמונה
                    </Button>
                    {projectCoverFile ? (
                      <span className="portfolio-file-pill">{projectCoverFile.name}</span>
                    ) : null}
                  </div>
                  <div className="portfolio-form-actions">
                    <Button
                  type="button"
                      variant="contained"
                      size="large"
                      startIcon={<SaveOutlinedIcon />}
                  onClick={handleAddProject}
                  disabled={isSavingProject}
                      sx={{
                        fontWeight: 800,
                        py: 1.25,
                        px: 3,
                        '& .MuiButton-startIcon': { mr: 0, ml: 1 },
                      }}
                    >
                      שמירת פרויקט חדש
                    </Button>
                  </div>
                </section>
              </div>

              <hr className="portfolio-divider" aria-hidden="true" />

              <div className="skills-portfolio-block">
                <h4 className="skills-portfolio-subtitle">מיומנויות</h4>
                <p className="muted-text skills-portfolio-hint">
                  סמנו מיומנויות, עדכנו שנות ניסיון והוסיפו חדשות לפי הצורך. לחצו על &quot;שמירת
                  מיומנויות&quot; כדי לעדכן בשרת.
                </p>
                {userSkills.length === 0 ? (
                  <p>עדיין לא שויכו מיומנויות לחשבון.</p>
                ) : null}
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
                          {skillNameForDisplay(profile?.skills?.[index], skill.skillId)}
                        </span>
                      </label>
                      <label className="skills-row__years">
                        שנות ניסיון
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
                <div className="skills-add-row">
                  <input
                    value={manualSkillInput}
                    onChange={(event) => setManualSkillInput(event.target.value)}
                    placeholder="הוספת מיומנות (למשל: ריאקט, SQL, פייתון)"
                  />
                  <Button
                    type="button"
                    variant="outlined"
                    startIcon={<AddCircleOutlineOutlinedIcon />}
                    onClick={addManualSkill}
                    sx={{ flexShrink: 0, '& .MuiButton-startIcon': { mr: 0, ml: 1 } }}
                  >
                    הוספת מיומנות
                  </Button>
                </div>
                {manualSkills.length > 0 ? (
                  <div className="manual-skills-list">
                    {manualSkills.map((name) => (
                      <Chip
                        key={name}
                        label={skillNameForDisplay(name, null)}
                        onDelete={() => removeManualSkill(name)}
                        variant="outlined"
                      />
                    ))}
                  </div>
                ) : null}
                <Button
                  type="button"
                  variant="contained"
                  size="large"
                  startIcon={<SaveOutlinedIcon />}
                  onClick={handleSaveSkills}
                  disabled={isSavingSkills}
                  sx={{
                    mt: 1,
                    fontWeight: 800,
                    py: 1.25,
                    px: 3,
                    '& .MuiButton-startIcon': { mr: 0, ml: 1 },
                  }}
                >
                  שמירת מיומנויות בשרת
                </Button>
              </div>
            </div>
          ) : null}

          {selectedSection === 'files' ? (
            <div className="profile-card profile-card--files">
              <div className="files-header">
                <div>
                  <h3>קבצים</h3>
                  <p className="muted-text files-intro">העלאה, הורדה וניהול מסמכים המקושרים לחשבון.</p>
                </div>
                <input
                  id={profileFilesUploadId}
                  type="file"
                  style={{ display: 'none' }}
                  onChange={handleUploadNewFile}
                  disabled={isUploadingFile}
                />
                <Button
                  component="label"
                  variant="contained"
                  htmlFor={profileFilesUploadId}
                  disabled={isUploadingFile}
                  startIcon={<UploadFileOutlinedIcon />}
                  sx={{
                    fontWeight: 800,
                    py: 1.25,
                    px: 2.5,
                    '& .MuiButton-startIcon': { mr: 0, ml: 1 },
                  }}
                >
                  {isUploadingFile ? 'מעלה קובץ…' : 'בחירת קובץ להעלאה'}
                </Button>
              </div>
              <div className="files-table-wrap">
                <table className="files-table">
                  <thead>
                    <tr>
                      <th>שם קובץ</th>
                      <th>תאריך</th>
                      <th>סוג / גודל</th>
                      <th>פעולות</th>
                    </tr>
                  </thead>
                  <tbody>
                    {files.map((file) => (
                      <tr key={file.id}>
                        <td>{file.fileName}</td>
                        <td>{new Date(file.uploadDate).toLocaleDateString('he-IL')}</td>
                        <td>{file.fileType || '-'}</td>
                        <td className="file-actions">
                          <Button
                            type="button"
                            size="small"
                            variant="outlined"
                            onClick={() => handleDownloadFile(file.filePath)}
                          >
                            הורדת קובץ
                          </Button>
                          <Button
                            type="button"
                            size="small"
                            variant="outlined"
                            onClick={() => handleEditFile(file)}
                          >
                            שינוי שם קובץ
                          </Button>
                          <Button
                            type="button"
                            size="small"
                            variant="contained"
                            onClick={() => handleDeleteFile(file.id)}
                            sx={{ fontWeight: 800 }}
                          >
                            מחיקת קובץ
                          </Button>
                        </td>
                      </tr>
                    ))}
                    {files.length === 0 ? (
                      <tr>
                        <td colSpan={4}>אין קבצים עדיין.</td>
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
                    <h3 className="ai-chat-header__title">סטודיו AI</h3>
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
                        <div
                          className={
                            message.isAssistant ? 'ai-chat-bubble ai-chat-bubble--assistant' : 'ai-chat-bubble ai-chat-bubble--user'
                          }
                        >
                          <p className="ai-chat-bubble__text">{message.messageContent}</p>
                        </div>
                        <div className="ai-chat-bubble-meta">
                          {message.isAssistant ? 'עוזר' : 'את/ה'} ·{' '}
                          {new Date(message.date).toLocaleString()}
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
                  <IconButton
                    type="button"
                    color="primary"
                    onClick={handleSendMessage}
                    disabled={isSubmittingMessage || !chatDraft.trim()}
                    title="שליחת הודעה"
                    aria-label="שלח הודעה"
                    sx={{ width: 46, height: 46, flexShrink: 0 }}
                  >
                    {isSubmittingMessage ? (
                      <span className="ai-chat-send__spinner" />
                    ) : (
                      <SendIcon sx={{ fontSize: 24 }} />
                    )}
                  </IconButton>
                </div>
              </div>

              <div className="resume-preview-panel">
                <div className="resume-preview-header">
                  <h3>סיכום קורות חיים (מהפרופיל שלך)</h3>
                  <Button
                    type="button"
                    variant="outlined"
                    onClick={handleCopyResumePreview}
                    sx={{ fontWeight: 800 }}
                  >
                    העתקת סיכום ללוח
                  </Button>
                </div>
                <p className="muted-text">
                  תמונת מצב בטקסט פשוט של הנתונים מהפרופיל. ניתן להדביק בצ׳אט למטה או לייצא
                  למסמך.
                </p>
                <pre className="resume-preview">{resumePreviewText}</pre>
              </div>
            </div>
          ) : null}
        </div>
      </div>

      <Dialog
        open={Boolean(confirmDelete)}
        onClose={handleCloseConfirmDelete}
        aria-labelledby="confirm-delete-title"
        aria-describedby="confirm-delete-desc"
        dir="rtl"
      >
        <DialogTitle id="confirm-delete-title">
          {confirmDelete?.type === 'file' ? 'מחיקת קובץ' : 'מחיקת פרויקט'}
        </DialogTitle>
        <DialogContent>
          <DialogContentText id="confirm-delete-desc">
            {confirmDelete?.type === 'file'
              ? 'האם למחוק את הקובץ? לא ניתן לשחזר לאחר המחיקה.'
              : 'למחוק את הפרויקט? פעולה זו אינה ניתנת לביטול.'}
          </DialogContentText>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 2, gap: 1 }}>
          <Button
            type="button"
            onClick={handleCloseConfirmDelete}
            disabled={isConfirmDeleting}
            color="inherit"
          >
            ביטול
          </Button>
          <Button
            type="button"
            onClick={handleConfirmDelete}
            disabled={isConfirmDeleting}
            variant="contained"
            color="error"
            autoFocus
          >
            {isConfirmDeleting ? 'מוחק…' : 'מחק'}
          </Button>
        </DialogActions>
      </Dialog>
    </section>
  )
}

export default ProfilePage
