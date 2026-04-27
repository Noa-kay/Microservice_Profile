import { Link } from 'react-router-dom'

function NotFoundPage() {
  return (
    <section className="page">
      <h2>Page not found</h2>
      <p>The page you requested does not exist.</p>
      <Link to="/">Back to Home</Link>
    </section>
  )
}

export default NotFoundPage
