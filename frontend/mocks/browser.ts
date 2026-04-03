import { setupWorker } from 'msw/browser'
import { authHandlers } from './auth.handlers'
import { taskHandlers } from './tasks.handlers'

const handlers = [
  ...authHandlers,
  ...taskHandlers,
]

export const worker = setupWorker(...handlers)
