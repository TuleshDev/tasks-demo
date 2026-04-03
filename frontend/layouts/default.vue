<template>
  <div>
    <header v-if="$route.path !== '/login'" class="flex justify-end p-4 bg-gray-100">
      <UserMenu />
    </header>
    <main>
      <slot />
    </main>
  </div>
</template>

<script setup>
import UserMenu from '~/components/UserMenu.vue'
</script>
