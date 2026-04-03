import { useAuth } from '~/composables/useAuth'

export default defineNuxtRouteMiddleware(() => {
  if (process.client) {
    const { loadAuth, isAuthenticated } = useAuth()
    loadAuth()
    if (!isAuthenticated.value) {
      return navigateTo('/login')
    }
  }
})
