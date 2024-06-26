import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()], 
  server: {
    open: '/home', 
    proxy: {
      "/api": {
        target: "http://localhost:5242/",  
        changeOrigin: true,
        secure: false,
        ws: true,    
      },
    },
  },
  // some other configuration
}
)
