import { ref } from 'vue'

const token = ref<string | null>(null)
const isAuthenticated = ref(false)
const user = ref<{ username: string } | null>(null)

function setAuth(newToken: string, username?: string) {
  token.value = newToken
  isAuthenticated.value = true
  if (username) {
    user.value = { username }
    if (process.client) {
      localStorage.setItem('user', JSON.stringify(user.value))
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
