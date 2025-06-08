import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
    plugins: [vue()],
    server: {
        proxy: {
            '/api': {
                target: 'https://localhost:7042', // dein ASP.NET Core-Backend
                changeOrigin: true,
                secure: false, // weil HTTPS mit self-signed Zertifikat
            }
        }
    }
})