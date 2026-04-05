<template>
  <div class="flex items-center justify-center min-h-screen bg-gray-100">
    <div class="w-full max-w-md bg-white rounded-lg shadow-lg p-8">
      <div v-if="loading" class="global-spinner">
        <div class="spinner"></div>
        <p>Вход в систему...</p>
      </div>
      <h2 class="text-2xl font-bold text-center mb-6">Вход</h2>
      <form @submit.prevent="handleLogin" class="space-y-4">
        <div>
          <label class="block text-gray-700 mb-1">Логин</label>
          <input v-model="email" type="text" required
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
      <div v-if="error" class="error">
        <p>{{ error }}</p>
        <button @click="login">Повторить</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '~/composables/useAuth'
import { useNuxtApp } from '#app'

const email = ref('')
const password = ref('')
const rememberMe = ref(false)
const loading = ref(false)
const error = ref<any>(null)
const router = useRouter()
const { setAuth } = useAuth()

async function handleLogin() {
  loading.value = true
  error.value = null
  try {
    const data = await $fetch('/api/auth/login', {
      method: 'POST',
      body: {
        email: email.value,
        password: password.value,
      },
    })

    if (data?.token) {
      const token = data.token

      const res = await $fetch(`/api/users/by-email?email=${encodeURIComponent(email.value)}`)
      if (res) {
        setAuth(token, res.email)

        if (rememberMe.value) {
          localStorage.setItem('user', JSON.stringify(res))
        } else {
          sessionStorage.setItem('user', JSON.stringify(res))
        }

        router.push('/')
      }
    }
  } catch (e: any) {
    error.value = e?.data?.error || 'Ошибка авторизации'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.global-spinner {
  position: fixed;
  inset: 0;
  background: rgba(255,255,255,0.8);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  z-index: 9999;
}
.spinner {
  border: 4px solid #ccc;
  border-top: 4px solid #333;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
}
@keyframes spin {
  to { transform: rotate(360deg); }
}
.error {
  color: red;
  margin-top: 1rem;
}
</style>
