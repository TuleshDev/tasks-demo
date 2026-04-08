import { ref } from 'vue'

const token = ref<string | null>(null)
const isAuthenticated = ref(false)
const user = ref<{ id: number; email: string; role: 'admin' | 'user' } | null>(null)
const rememberMe = ref(true)

async function setAuth(newToken: string, email?: string) {
  token.value = newToken
  isAuthenticated.value = true

  if (email) {
    const config = useRuntimeConfig()

    const res = await fetch(`${config.public.apiUrl}/api/users/by-email?email=${encodeURIComponent(email)}`)
    if (res.ok) {
      const u = await res.json()
      user.value = u
      if (process.client) {
        const safeUser = { id: user.value.id, email: user.value.email, role: user.value.role }
        if (rememberMe.value) {
          localStorage.setItem('user', JSON.stringify(safeUser))
          localStorage.setItem('rememberMe', 'true')
        } else {
          sessionStorage.setItem('user', JSON.stringify(safeUser))
          sessionStorage.setItem('rememberMe', 'false')
        }
      }
    }
  }

  if (process.client) {
    if (rememberMe.value) {
      localStorage.setItem('auth_token', newToken)
      localStorage.setItem('rememberMe', 'true')
    } else {
      sessionStorage.setItem('auth_token', newToken)
      sessionStorage.setItem('rememberMe', 'false')
    }
  }
}

function loadAuth() {
  if (process.client) {
    const savedToken = localStorage.getItem('auth_token') || sessionStorage.getItem('auth_token')
    const savedUser = localStorage.getItem('user') || sessionStorage.getItem('user')
    const savedRemember = localStorage.getItem('rememberMe') || sessionStorage.getItem('rememberMe')

    if (savedToken) {
      token.value = savedToken
      isAuthenticated.value = true
    }
    if (savedUser) {
      user.value = JSON.parse(savedUser)
    }
    if (savedRemember) {
      rememberMe.value = savedRemember === 'true'
    }
  }
}

function clearAuth() {
  token.value = null
  isAuthenticated.value = false
  user.value = null
  if (process.client) {
    localStorage.removeItem('auth_token')
    localStorage.removeItem('user')
    localStorage.removeItem('rememberMe')
    sessionStorage.removeItem('auth_token')
    sessionStorage.removeItem('user')
    sessionStorage.removeItem('rememberMe')
  }
}

function logout() {
  clearAuth()
}

export function useAuth() {
  return { token, isAuthenticated, user, rememberMe, setAuth, loadAuth, clearAuth, logout }
}
