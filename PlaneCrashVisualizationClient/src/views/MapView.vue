<template>
  <div class="map-container">
    <!-- Map Controls -->
    <MapControls
      :filters="filters"
      :operators="operators"
      :manufacturers="manufacturers"
      @update:filters="handleFiltersUpdate"
    />

    <!-- Map -->
    <div id="map"></div>

    <!-- Loading indicator -->
    <LoadingOverlay 
      :show="loading" 
      :message="loadingMessage" 
    />

    <!-- Legend -->
    <MapLegend />

    <!-- Error Toast -->
    <div v-if="error" class="error-toast">
      <div class="alert alert-danger alert-dismissible fade show" role="alert">
        {{ error }}
        <button type="button" class="btn-close" @click="error = null"></button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, onUnmounted, watch, ref, computed } from 'vue'
import { useMapFilters, useMap, useCrashData } from '../composables/index.js'
import MapControls from '../components/MapControls.vue'
import MapLegend from '../components/MapLegend.vue'
import LoadingOverlay from '../components/LoadingOverlay.vue'

// Composables
const { filters, buildApiParams } = useMapFilters()
const { initializeMap, loadMarkers, destroyMap } = useMap()
const { 
  crashData, 
  operators, 
  manufacturers, 
  loading, 
  error, 
  fetchCrashData, 
  initializeData 
} = useCrashData()

// Local state
const loadingMessage = computed(() => {
  if (loading.value) {
    return 'Loading crash data...'
  }
  return ''
})

// Debounced filter application
let filterTimeout = null
const debouncedFetchData = () => {
  clearTimeout(filterTimeout)
  filterTimeout = setTimeout(async () => {
    const params = buildApiParams()
    await fetchCrashData(params)
  }, 500)
}

// Event handlers
const handleFiltersUpdate = (newFilters) => {
  filters.value = newFilters
}

// Lifecycle
onMounted(async () => {
  initializeMap()
  await initializeData()
  
  // Initial data fetch
  const params = buildApiParams()
  await fetchCrashData(params)
})

onUnmounted(() => {
  destroyMap()
  if (filterTimeout) {
    clearTimeout(filterTimeout)
  }
})

// Watchers
watch(filters, debouncedFetchData, { deep: true })

watch(crashData, (newData) => {
  if (newData && newData.length > 0) {
    loadMarkers(newData)
  }
}, { immediate: true })
</script>

<style scoped>
.map-container {
  position: fixed;
  top: 60px; /* Start below the navbar */
  left: 0;
  right: 0;
  bottom: 0;
  height: calc(100vh - 60px); /* Full height minus navbar */
  width: 100vw;
  margin: 0;
  padding: 0;
  overflow: hidden; /* Prevent scrolling */
}

#map {
  height: 100%;
  width: 100%;
}

.error-toast {
  position: absolute;
  top: 20px;
  right: 20px;
  z-index: 2000;
  max-width: 400px;
}

/* Deep selectors for Leaflet popup styling */
:deep(.leaflet-popup-content-wrapper) {
  border-radius: 8px;
}

:deep(.leaflet-popup-content) {
  margin: 12px;
}

/* Custom marker cluster styles */
:deep(.marker-cluster-small) {
  background-color: rgba(181, 226, 140, 0.6);
}
:deep(.marker-cluster-small div) {
  background-color: rgba(110, 204, 57, 0.6);
}

:deep(.marker-cluster-medium) {
  background-color: rgba(241, 211, 87, 0.6);
}
:deep(.marker-cluster-medium div) {
  background-color: rgba(240, 194, 12, 0.6);
}

:deep(.marker-cluster-large) {
  background-color: rgba(253, 156, 115, 0.6);
}
:deep(.marker-cluster-large div) {
  background-color: rgba(241, 128, 23, 0.6);
}
</style>
