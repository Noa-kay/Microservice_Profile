import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // When VITE_API_BASE_URL is "/api", the browser hits the dev server; forward to the real API.
      '/api': {
        target: 'http://localhost:5250',
        changeOrigin: true,
      },
    },
  },
})
