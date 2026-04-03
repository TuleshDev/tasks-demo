import { http, HttpResponse } from 'msw'
import { tasks } from '../data/tasks.data'
import type { Task } from '../data/tasks.data'
import { getUserFromAuthHeader } from '../utils/auth'

let lastId = tasks.length

export const taskHandlers = [
  http.get('/api/tasks', ({ request }) => {
    const user = getUserFromAuthHeader(request.headers.get('authorization'))
    if (!user) return HttpResponse.json(null, { status: 401 })

    const url = new URL(request.url)
    const status = url.searchParams.get('status') || 'all'
    const search = url.searchParams.get('search') || ''
    const sort = url.searchParams.get('sort') || ''
    const page = parseInt(url.searchParams.get('page') || '1', 10)
    const limit = parseInt(url.searchParams.get('limit') || '10', 10)

    let filtered = tasks

    if (status === 'active') {
      filtered = filtered.filter(t => !t.IsCompleted)
    } else if (status === 'completed') {
      filtered = filtered.filter(t => t.IsCompleted)
    }

    if (search) {
      const s = search.toLowerCase()
      filtered = filtered.filter(
        t =>
          t.Title.toLowerCase().includes(s) ||
          t.Description.toLowerCase().includes(s)
      )
    }

    if (sort === 'dateAsc') {
      filtered = filtered.sort((a, b) => new Date(a.DueDate).getTime() - new Date(b.DueDate).getTime())
    } else if (sort === 'dateDesc') {
      filtered = filtered.sort((a, b) => new Date(b.DueDate).getTime() - new Date(a.DueDate).getTime())
    } else if (sort === 'title') {
      filtered = filtered.sort((a, b) => a.Title.localeCompare(b.Title))
    } else if (sort === 'priority') {
      filtered = filtered.sort((a, b) => a.Priority.localeCompare(b.Priority))
    }

    const total = filtered.length
    const start = (page - 1) * limit
    const end = start + limit
    const paginated = filtered.slice(start, end).map(task => ({
      ...task,
      ExecutorPhoto: task.ExecutorPhoto || '/uploads/default.png'
    }))

    return HttpResponse.json({ data: paginated, total, page, limit })
  }),

  http.post('/api/tasks', async ({ request }) => {
    const user = getUserFromAuthHeader(request.headers.get('authorization'))
    if (!user) return HttpResponse.json(null, { status: 401 })

    const body = await request.json()
    const task: Task = {
      Id: ++lastId,
      Title: body.Title,
      Description: body.Description || '',
      DueDate: new Date(body.DueDate).toISOString().split('T')[0],
      Priority: body.Priority || 'Обычный',
      IsCompleted: false,
      OwnerId: user.id,
      ExecutorFirstName: body.ExecutorFirstName || '',
      ExecutorLastName: body.ExecutorLastName || '',
      ExecutorEmail: body.ExecutorEmail || '',
      ExecutorPhoto: body.ExecutorPhoto || '/uploads/default.png',
    }

    tasks.push(task)

    const url = new URL(request.url)
    const status = url.searchParams.get('status') || 'all'
    const search = url.searchParams.get('search') || ''
    const sort = url.searchParams.get('sort') || ''
    const page = parseInt(url.searchParams.get('page') || '1', 10)
    const limit = parseInt(url.searchParams.get('limit') || '10', 10)

    let filtered = tasks

    if (status === 'active') {
      filtered = filtered.filter(t => !t.IsCompleted)
    } else if (status === 'completed') {
      filtered = filtered.filter(t => t.IsCompleted)
    }

    if (search) {
      const s = search.toLowerCase()
      filtered = filtered.filter(
        t =>
          t.Title.toLowerCase().includes(s) ||
          t.Description.toLowerCase().includes(s)
      )
    }

    if (sort === 'dateAsc') {
      filtered = filtered.sort((a, b) => new Date(a.DueDate).getTime() - new Date(b.DueDate).getTime())
    } else if (sort === 'dateDesc') {
      filtered = filtered.sort((a, b) => new Date(b.DueDate).getTime() - new Date(a.DueDate).getTime())
    } else if (sort === 'title') {
      filtered = filtered.sort((a, b) => a.Title.localeCompare(b.Title))
    } else if (sort === 'priority') {
      filtered = filtered.sort((a, b) => a.Priority.localeCompare(b.Priority))
    }

    const index = filtered.findIndex(t => t.Id === task.Id)
    let pageForNewTask = null
    if (index !== -1) {
      pageForNewTask = Math.floor(index / limit) + 1
    }

    return HttpResponse.json({ 
      ...task, 
      DueDate: new Date(task.DueDate).toISOString().split('T')[0], 
      pageForNewTask, 
      page, 
      limit 
    }, { status: 201 })
  }),

  http.put('/api/tasks/:id', async ({ params, request }) => {
    const body = await request.json()
    const id = Number(params.id)
    const index = tasks.findIndex(t => t.Id === id)
    if (index !== -1) {
      tasks[index] = { 
        ...tasks[index], 
        ...body, 
        DueDate: new Date(body.DueDate).toISOString().split('T')[0] 
      }
      return HttpResponse.json(tasks[index])
    }
    return HttpResponse.json({ error: 'Task not found' }, { status: 404 })
  }),

  http.delete('/api/tasks/:id', ({ params }) => {
    const id = Number(params.id)
    const index = tasks.findIndex(t => t.Id === id)
    if (index !== -1) {
      tasks.splice(index, 1)
      return HttpResponse.json({ success: true })
    }
    return HttpResponse.json({ error: 'Task not found' }, { status: 404 })
  }),
]
