import { Button } from '@mui/material'
import { API_BASE_URL } from '../services/api'

const toAbsoluteImageUrl = (imagePath) => {
  if (!imagePath) {
    return null
  }

  if (imagePath.startsWith('http://') || imagePath.startsWith('https://')) {
    return imagePath
  }

  const apiOrigin = API_BASE_URL.replace('/api', '')
  const normalizedPath = imagePath.startsWith('/') ? imagePath : `/${imagePath}`
  return `${apiOrigin}${normalizedPath}`
}

function ProjectList({ projects, onDelete }) {
  if (!Array.isArray(projects) || projects.length === 0) {
    return (
      <div className="projects-empty" role="status">
        <p className="projects-empty__text">אין פרויקטים להצגה.</p>
      </div>
    )
  }

  return (
    <div className="projects-grid">
      {projects.map((project) => {
        const imageUrl = toAbsoluteImageUrl(project.projectsImages)
        return (
          <article key={project.id} className="project-card">
            <div className="project-card__media">
              {imageUrl ? (
                <img
                  className="project-image"
                  src={imageUrl}
                  alt={project.title || project.projectName || 'תמונת פרויקט'}
                />
              ) : (
                <div className="project-image project-image--placeholder">אין תמונה</div>
              )}
            </div>
            <div className="project-card__body">
              <h4 className="project-card__title">
                {project.title || project.projectName || 'פרויקט ללא כותרת'}
              </h4>
              <p className="project-card__desc">
                {project.description || 'אין תיאור.'}
              </p>
              {project.gitHubLink ? (
                <p className="project-github">
                  <a href={project.gitHubLink} target="_blank" rel="noreferrer">
                    קישור ל-GitHub
                  </a>
                </p>
              ) : null}
            </div>
            {typeof onDelete === 'function' ? (
              <div className="project-card__actions">
                <Button
                  type="button"
                  fullWidth
                  variant="contained"
                  onClick={() => onDelete(project.id)}
                  sx={{ fontWeight: 800, py: 1 }}
                >
                  מחיקת פרויקט
                </Button>
              </div>
            ) : null}
          </article>
        )
      })}
    </div>
  )
}

export default ProjectList
