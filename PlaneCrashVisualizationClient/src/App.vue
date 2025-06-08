<script setup>
    import { ref, onMounted } from 'vue'

    const message = ref('Lade...')

    onMounted(async () => {
        const res = await fetch('/api/hello')
        message.value = await res.text()
    })
</script>

<template>
    <h1>{{ message }}</h1>
</template>
