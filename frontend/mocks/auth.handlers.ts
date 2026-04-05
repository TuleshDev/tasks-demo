import { http, HttpResponse } from 'msw'
import { users } from '../data/users.data'

export const authHandlers = [
  http.post('/api/auth/login', async ({ request }) => {
    const { email, password } = await request.json()
    const user = users.find(u => u.email === email && u.password === password)

    if (!user) {
      return HttpResponse.json({ message: 'Invalid credentials' }, { status: 401 })
    }

    return HttpResponse.json({
      token: String(user.id),
      user: { id: user.id, email: user.email, role: user.role },
    })
  }),

  http.get('/api/users/by-email', ({ request }) => {
    const url = new URL(request.url)
    const email = url.searchParams.get('email')

    if (!email) {
      return HttpResponse.json({ error: 'Email is required' }, { status: 400 })
    }

    const user = users.find(u => u.email === email)

    if (!user) {
      return HttpResponse.json({ error: 'User not found' }, { status: 404 })
    }

    return HttpResponse.json(user)
  }),

  http.get('/api/auth/me', ({ request }) => {
    const authHeader = request.headers.get('authorization')
    if (!authHeader) return HttpResponse.json(null, { status: 401 })

    const token = authHeader.replace('Bearer ', '')
    const user = users.find(u => u.id === Number(token))
    if (!user) return HttpResponse.json(null, { status: 401 })

    return HttpResponse.json(user)
  })
]
