export default defineNuxtConfig({
  compatibilityDate: '2026-04-01',
  devtools: { enabled: true },
  telemetry: false,
  modules: ['@nuxtjs/tailwindcss'],
  vite: {
    optimizeDeps: {
      include: [
        '@vue/devtools-core',
        '@vue/devtools-kit',
        'msw',
        'msw/browser',
      ]
    }
  }
})
