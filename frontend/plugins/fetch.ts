export default defineNuxtPlugin((nuxtApp) => {
  const customFetch = $fetch.create({
    onResponseError({ response }) {
      if (response.status === 401) {
        localStorage.removeItem('auth_token')

        const { show } = useNotification()
        show('Сессия истекла, войдите снова')

        const router = useRouter()
        router.push('/login')
      }
    }
  })

  nuxtApp.provide('fetch', customFetch)
})
