<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useAuth } from '~/composables/useAuth'
import { useNuxtApp } from '#app'
import debounce from 'lodash.debounce'

definePageMeta({
  middleware: 'auth'
})

const { $fetch } = useNuxtApp()

const { token, isAuthenticated, loadAuth } = useAuth()
const tasks = ref<any[]>([])
const error = ref<any>(null)
const page = ref(1)
const limit = ref(10)
const total = ref(0)

const filter = ref<'all' | 'active' | 'completed'>('all')
const search = ref('')
const loading = ref(false)
const notFound = ref(false)
const sort = ref<'dateAsc' | 'dateDesc' | 'title' | 'priority'>('dateAsc')

const showCreateForm = ref(false)
const editTaskData = ref<any|null>(null)
const form = ref<any>({
  Id: 0,
  Title: '',
  Description: '',
  DueDate: '',
  IsCompleted: false,
  OwnerId: 0,
  ExecutorFirstName: '',
  ExecutorLastName: '',
  ExecutorEmail: '',
  ExecutorPhoto: '/uploads/default.png',
  Priority: 'Обычный'
})

const { user } = useAuth()

function canManage(task) {
  return user.value?.role === 'admin' || task.OwnerId === user.value?.id
}

function formatDate(dateString: string) {
  const date = new Date(dateString)
  return date.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' })
}

async function fetchTasks() {
  if (!token.value) return
  try {
    loading.value = true
    notFound.value = false
    error.value = null
    const data: any = await $fetch('/api/tasks', {
      headers: { Authorization: `Bearer ${token.value}` },
      query: {
        page: page.value.toString(),
        limit: limit.value.toString(),
        status: filter.value,
        search: search.value,
        sort: sort.value,
      },
    })
    tasks.value = data?.data || []
    total.value = data?.total || 0
    if (tasks.value.length === 0) {
      notFound.value = true
    }
  } catch (e: any) {
    error.value = e?.message || 'Ошибка загрузки задач'
  } finally {
    loading.value = false
  }
}

const debouncedFetch = debounce(() => {
  page.value = 1
  fetchTasks()
}, 400)

watch(search, () => {
  debouncedFetch()
})

async function createTask() {
  try {
    const data: any = await $fetch('/api/tasks', {
      method: 'POST',
      body: form.value,
      headers: { Authorization: `Bearer ${token.value}` },
    })
    tasks.value.push(data)
    closeForm()
  } catch (err) {
    error.value = err
  }
}

async function saveEditTask() {
  try {
    const updated: any = await $fetch(`/api/tasks/${form.value.Id}`, {
      method: 'PUT',
      body: form.value,
      headers: { Authorization: `Bearer ${token.value}` },
    })
    const idx = tasks.value.findIndex(t => t.Id === form.value.Id)
    tasks.value[idx] = updated
    closeForm()
  } catch (err) {
    error.value = err
  }
}

function openEditForm(task: any) {
  editTaskData.value = { ...task }
  form.value = { ...task }
}

function closeForm() {
  showCreateForm.value = false
  editTaskData.value = null
  form.value = {
    Id: 0,
    Title: '',
    Description: '',
    DueDate: '',
    IsCompleted: false,
    OwnerId: 0,
    ExecutorFirstName: '',
    ExecutorLastName: '',
    ExecutorEmail: '',
    ExecutorPhoto: '',
    Priority: 'Обычный'
  }
}

async function deleteTask(id: number) {
  try {
    await $fetch(`/api/tasks/${id}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${token.value}` },
    })
    tasks.value = tasks.value.filter(t => t.Id !== id)
  } catch (err) {
    error.value = err
  }
}

function goToPage(p: number) {
  page.value = p
  fetchTasks()
}

function setFilter(value: 'all' | 'active' | 'completed') {
  filter.value = value
  page.value = 1
  fetchTasks()
}

function clearFilters() {
  filter.value = 'all'
  search.value = ''
  sort.value = 'dateAsc'
  page.value = 1
  fetchTasks()
}

function onFileSelected(event) {
  const file = event.target.files[0]
  if (file) {
    form.value.ExecutorPhoto = URL.createObjectURL(file)
  }
}

watch([filter, sort], () => {
  page.value = 1
  fetchTasks()
})

onMounted(async () => {
  loadAuth()
  if (isAuthenticated.value && token.value) {
    await fetchTasks()
  }
})
</script>

<template>
  <div class="min-h-screen bg-gray-100 p-6">
    <div class="w-full bg-white rounded-lg shadow-lg p-8">
      <div class="flex justify-between items-center mb-4">
        <h1 class="text-3xl font-bold text-left">Список задач</h1>
        <button @click="showCreateForm = true"
                class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700">
          + Добавить задачу
        </button>
      </div>

      <div class="flex flex-wrap items-center space-x-2 mb-6">
        <button @click="setFilter('all')" :class="['px-3 py-1 rounded border', filter==='all' ? 'bg-gray-300 border-gray-400' : 'bg-gray-100 hover:bg-gray-200']">Все задачи</button>
        <button @click="setFilter('active')" :class="['px-3 py-1 rounded border', filter==='active' ? 'bg-gray-300 border-gray-400' : 'bg-gray-100 hover:bg-gray-200']">Активные</button>
        <button @click="setFilter('completed')" :class="['px-3 py-1 rounded border', filter==='completed' ? 'bg-gray-300 border-gray-400' : 'bg-gray-100 hover:bg-gray-200']">Выполненные</button>

        <input v-model="search" type="text" placeholder="Поиск задач..." class="px-3 py-1 border rounded flex-1" />
        <button @click="clearFilters" class="px-3 py-1 bg-gray-200 rounded hover:bg-gray-300">Очистить</button>
        <select v-model="sort" class="px-3 py-1 border rounded">
          <option value="dateAsc">По дате (возрастание)</option>
          <option value="dateDesc">По дате (убывание)</option>
          <option value="title">По названию</option>
          <option value="priority">По приоритету</option>
        </select>
      </div>
      <div v-if="error" class="error">
        <p>{{ error }}</p>
        <button @click="fetchTasks">Повторить</button>
      </div>
      <div v-else>
        <div v-if="loading" class="flex justify-center py-4">
          <div class="w-8 h-8 border-4 border-blue-500 border-t-transparent rounded-full animate-spin"></div>
        </div>
        <div v-else-if="notFound">Результаты не найдены</div>
        <div class="overflow-x-auto">
          <table class="w-full text-left border border-gray-200 rounded-lg">
            <thead>
              <tr class="bg-gray-200 text-gray-700">
                <th class="px-4 py-2">Название</th>
                <th class="px-4 py-2">Описание</th>
                <th class="px-4 py-2">Приоритет</th>
                <th class="px-4 py-2">Дата</th>
                <th class="px-4 py-2">Исполнитель</th>
                <th class="px-4 py-2">Статус</th>
                <th class="px-4 py-2">Действия</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="task in tasks" :key="task.Id" class="hover:bg-gray-50 transition">
                <td class="px-4 py-2 font-medium text-gray-800">{{ task.Title }}</td>
                <td class="px-4 py-2 text-gray-600">{{ task.Description }}</td>
                <td class="px-4 py-2">
                  <span :class="['px-2 py-1 rounded text-sm font-semibold', task.Priority === 'Обычный' ? 'bg-gray-200 text-gray-700' : 'bg-red-500 text-white']">
                    {{ task.Priority }}
                  </span>
                </td>
                <td class="px-4 py-2 text-gray-600">{{ formatDate(task.DueDate) }}</td>
                <td class="px-4 py-2 text-gray-600">
                  <div class="flex items-center space-x-3">
                    <img v-if="task.ExecutorPhoto" :src="task.ExecutorPhoto" alt="Фото" class="w-20 h-20 rounded-full object-cover border-2 border-gray-300" />
                    <div>
                      <span>{{ task.ExecutorFirstName }} {{ task.ExecutorLastName }}</span>
                      <div class="text-sm text-gray-500">{{ task.ExecutorEmail }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-4 py-2">
                  <span :class="['px-2 py-1 rounded text-sm font-semibold', task.IsCompleted ? 'bg-green-500 text-white' : 'bg-yellow-400 text-gray-800']">
                    {{ task.IsCompleted ? 'Выполнено' : 'В процессе' }}
                  </span>
                </td>
                <td class="px-4 py-2 space-x-2">
                  <button v-if="canManage(task)"
                          @click="openEditForm(task)"
                          class="px-3 py-1 bg-blue-600 text-white rounded hover:bg-blue-700">Редактировать</button>
                  <button v-if="canManage(task)"
                          @click="deleteTask(task.Id)"
                          class="px-3 py-1 bg-red-600 text-white rounded hover:bg-red-700">Удалить</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="flex justify-center items-center space-x-2 mt-6">
          <button @click="goToPage(1)" :disabled="page===1" class="px-3 py-1 bg-gray-300 rounded disabled:opacity-50 hover:bg-gray-400">&lt;-</button>
          <button v-for="p in Math.ceil(total/limit)" :key="p" @click="goToPage(p)" :class="['px-3 py-1 rounded transition', page===p ? 'bg-blue-600 text-white' : 'bg-gray-200 hover:bg-gray-300']">{{ p }}</button>
          <button @click="goToPage(Math.ceil(total/limit))" :disabled="page===Math.ceil(total/limit)" class="px-3 py-1 bg-gray-300 rounded disabled:opacity-50 hover:bg-gray-400">-&gt;</button>
        </div>
      </div>
    </div>
    <div v-if="showCreateForm || editTaskData" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
      <div class="bg-white rounded-lg shadow-lg p-8 w-full max-w-3xl">
        <h2 class="text-xl font-bold mb-4">{{ editTaskData ? 'Редактировать задачу' : 'Создать задачу' }}</h2>
        <form @submit.prevent="editTaskData ? saveEditTask() : createTask()">
          <div class="grid grid-cols-3 gap-4 mb-6">
            <div class="flex flex-col items-start col-span-1">
              <img v-if="form.ExecutorPhoto"
                   :src="form.ExecutorPhoto"
                   alt="Фото исполнителя"
                   class="w-20 h-20 rounded-full object-cover border-2 border-gray-300 mb-4" />
              <input type="file"
                     accept=".png,.jpg,.jpeg,*"
                     class="hidden"
                     ref="fileInput"
                     @change="onFileSelected" />
              <button type="button"
                      @click="$refs.fileInput.click()"
                      class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
                Загрузить
              </button>
            </div>
            <div class="flex flex-col space-y-3 col-span-2">
              <input v-model="form.ExecutorFirstName"
                     type="text"
                     placeholder="Имя исполнителя"
                     class="w-full px-3 py-2 border rounded"
                     required />
              <input v-model="form.ExecutorLastName"
                     type="text"
                     placeholder="Фамилия исполнителя"
                     class="w-full px-3 py-2 border rounded"
                     required />
              <input v-model="form.ExecutorEmail"
                     type="email"
                     placeholder="Email исполнителя"
                     class="w-full px-3 py-2 border rounded"
                     required />
            </div>
          </div>
          <input v-model="form.Title"
                 type="text"
                 placeholder="Название"
                 class="w-full mb-2 px-3 py-2 border rounded"
                 required />
          <textarea v-model="form.Description"
                    placeholder="Описание"
                    class="w-full mb-4 px-3 py-4 border rounded h-36"
                    required></textarea>
          <input v-model="form.DueDate"
                 type="date"
                 class="w-full mb-2 px-3 py-2 border rounded"
                 required />
          <select v-model="form.Priority"
                  class="w-full mb-2 px-3 py-2 border rounded">
            <option value="Обычный">Обычный</option>
            <option value="Важно">Важно</option>
          </select>
          <div class="flex items-center mb-4">
            <input v-model="form.IsCompleted" type="checkbox" class="mr-2" />
            <label>Задача выполнена</label>
          </div>
          <div class="flex justify-end space-x-2">
            <button type="button" @click="closeForm"
                    class="px-4 py-2 bg-gray-300 rounded hover:bg-gray-400">
              Отмена
            </button>
            <button type="submit"
                    class="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700">
              {{ editTaskData ? 'Сохранить' : 'Создать' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.error {
  color: red;
  padding: 1rem;
}
</style>
