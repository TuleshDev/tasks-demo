<template>
  <ClientOnly>
    <div class="flex items-center space-x-4">
      <div v-if="isAuthenticated" class="flex flex-col text-gray-700">
        <span class="font-medium">{{ user?.email }}</span>
      </div>
      <button @click="handleAction"
              class="px-4 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition">
        {{ isAuthenticated ? 'Выйти' : 'Войти' }}
      </button>
    </div>
  </ClientOnly>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useAuth } from '~/composables/useAuth'

const { user, logout, isAuthenticated } = useAuth()
const router = useRouter()

function handleAction() {
  if (isAuthenticated.value) {
    logout()
    router.push('/login')
  } else {
    router.push('/login')
  }
}
</script>
