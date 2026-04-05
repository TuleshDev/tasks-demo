export async function getUserFromAuthHeader(auth?: string) {
  if (!auth) return null
  try {
    const user = await $fetch('/api/auth/me', {
      headers: { Authorization: auth }
    })
    return user
  } catch {
    return null
  }
}
