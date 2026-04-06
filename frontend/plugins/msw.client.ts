export default async () => {
  const config = useRuntimeConfig()

  if (config.public.useMsw) {
    const { worker } = await import('~/mocks/browser')

    await worker.start({
      onUnhandledRequest: 'bypass',
    })

    console.log('MSW запущен')
  } else {
    console.log('MSW отключён, работаем с реальным API')
  }
}
