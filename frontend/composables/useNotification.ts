export const useNotification = () => {
  const message = useState<string | null>('notification', () => null)

  function show(msg: string) {
    message.value = msg
    setTimeout(() => {
      message.value = null
    }, 3000)
  }

  return { message, show }
}
