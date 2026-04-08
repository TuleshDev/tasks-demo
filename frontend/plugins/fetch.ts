export default defineNuxtPlugin((nuxtApp) => {
  const customFetch = $fetch.create({
    onResponseError({ response }) {
      if (response.status === 401) {
        localStorage.removeItem('auth_token')
        sessionStorage.removeItem('auth_token')

        const { show } = useNotification()
        show('Сессия истекла, войдите снова', { type: 'warning' })

        const router = useRouter()
        router.push('/login')
      } else if (response.status === 403) {
        const { show } = useNotification()
        show('У вас нет прав для выполнения этого действия', { type: 'error' })
      }
    }
  })

  nuxtApp.provide('fetch', customFetch)
})
