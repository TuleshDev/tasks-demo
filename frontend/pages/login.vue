<template>
  <div class="flex items-center justify-center min-h-screen bg-gray-100">
    <div class="w-full max-w-md bg-white rounded-lg shadow-lg p-8">
      <h2 class="text-2xl font-bold text-center mb-6">Вход</h2>
      <form @submit.prevent="handleLogin" class="space-y-4">
        <div>
          <label class="block text-gray-700 mb-1">Логин</label>
          <input v-model="username" type="text" required
                 class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"/>
        </div>
        <div>
          <label class="block text-gray-700 mb-1">Пароль</label>
          <input v-model="password" type="password" required
                 class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"/>
        </div>
        <div class="flex items-center">
          <input v-model="rememberMe" type="checkbox"
                 class="h-4 w-4 text-blue-600 border-gray-300 rounded"/>
          <label class="ml-2 text-gray-700">Запомнить меня</label>
        </div>
        <button type="submit"
                class="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition">
          Войти
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '~/composables/useAuth'

const username = ref('')
const password = ref('')
const rememberMe = ref(false)
const router = useRouter()
const { setAuth } = useAuth()

async function handleLogin() {
  const { data } = await useFetch('/api/auth/login', {
    method: 'POST',
    body: {
      email: username.value,
      password: password.value,
    },
    server: false,
  })

  if (data.value?.token) {
    const token = data.value.token
    const userData = { username: username.value }
    setAuth(token)
    if (rememberMe.value) {
      localStorage.setItem('user', JSON.stringify(userData))
    } else {
      sessionStorage.setItem('user', JSON.stringify(userData))
    }
    router.push('/')
  }
}
</script>
