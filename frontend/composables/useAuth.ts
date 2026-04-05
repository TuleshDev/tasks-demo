import { ref } from 'vue'

const token = ref<string | null>(null)
const isAuthenticated = ref(false)
const user = ref<{ id: number; email: string; role: 'admin' | 'user' } | null>(null)

async function setAuth(newToken: string, email?: string) {
  token.value = newToken
  isAuthenticated.value = true

  if (email) {
    const res = await fetch(`/api/users/by-email?email=${encodeURIComponent(email)}`)
    if (res.ok) {
      const u = await res.json()
      user.value = u
      if (process.client) {
        localStorage.setItem('user', JSON.stringify(user.value))
      }
    }
  }

  if (process.client) {
    localStorage.setItem('auth_token', newToken)
  }
}

function loadAuth() {
  if (process.client) {
    const savedToken = localStorage.getItem('auth_token')
    const savedUser = localStorage.getItem('user')
    if (savedToken) {
      token.value = savedToken
      isAuthenticated.value = true
    }
    if (savedUser) {
      user.value = JSON.parse(savedUser)
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
  }
}

function logout() {
  clearAuth()
}

export function useAuth() {
  return { token, isAuthenticated, user, setAuth, loadAuth, clearAuth, logout }
}
