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
    return <p>No projects yet.</p>
  }

  return (
    <div className="projects-grid">
      {projects.map((project) => {
        const imageUrl = toAbsoluteImageUrl(project.projectsImages)
        return (
          <article key={project.id} className="project-card">
            {imageUrl ? (
              <img
                className="project-image"
                src={imageUrl}
                alt={project.title || project.projectName || 'Project image'}
              />
            ) : (
              <div className="project-image project-image--placeholder">
                No image
              </div>
            )}

            <h4>{project.title || project.projectName || 'Untitled Project'}</h4>
            <p>{project.description || 'No description.'}</p>
            {project.gitHubLink ? (
              <p className="project-github">
                <a href={project.gitHubLink} target="_blank" rel="noreferrer">
                  GitHub
                </a>
              </p>
            ) : null}
            {typeof onDelete === 'function' ? (
              <button
                type="button"
                className="project-card__delete"
                onClick={() => onDelete(project.id)}
              >
                Delete
              </button>
            ) : null}
          </article>
        )
      })}
    </div>
  )
}

export default ProjectList
