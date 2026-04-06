import type { Task } from '../data/tasks.data'

export function mapTask(raw: any): Task {
  return {
    Id: raw.id,
    Title: raw.title,
    Description: raw.description,
    DueDate: raw.dueDate,
    IsCompleted: raw.isCompleted,
    OwnerId: raw.ownerId,
    ExecutorFirstName: raw.executorFirstName,
    ExecutorLastName: raw.executorLastName,
    ExecutorEmail: raw.executorEmail,
    ExecutorPhoto: raw.executorPhoto,
    Priority: raw.priority
  }
}
