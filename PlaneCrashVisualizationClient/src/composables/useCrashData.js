import { ref, watch } from 'vue'
import { crashApi, ApiError } from '../services/api.js'

export function useCrashData() {
  const crashData = ref([])
  const operators = ref([])
  const manufacturers = ref([])
  const statistics = ref(null)
  const loading = ref(false)
  const error = ref(null)

  const fetchOperators = async () => {
    try {
      operators.value = await crashApi.getOperators()
    } catch (err) {
      console.error('Failed to fetch operators:', err)
      error.value = 'Failed to load operators'
    }
  }

  const fetchManufacturers = async () => {
    try {
      manufacturers.value = await crashApi.getManufacturers()
    } catch (err) {
      console.error('Failed to fetch manufacturers:', err)
      error.value = 'Failed to load manufacturers'
    }
  }

  const fetchStatistics = async () => {
    try {
      statistics.value = await crashApi.getStatistics()
    } catch (err) {
      console.error('Failed to fetch statistics:', err)
      error.value = 'Failed to load statistics'
    }
  }

  const fetchCrashData = async (filterParams) => {
    try {
      loading.value = true
      error.value = null
      crashData.value = await crashApi.getMapData(filterParams)
    } catch (err) {
      console.error('Failed to fetch crash data:', err)
      error.value = 'Failed to load crash data'
      crashData.value = []
    } finally {
      loading.value = false
    }
  }

  const initializeData = async () => {
    await Promise.all([
      fetchOperators(),
      fetchManufacturers(),
      fetchStatistics()
    ])
  }

  return {
    crashData,
    operators,
    manufacturers,
    statistics,
    loading,
    error,
    fetchCrashData,
    initializeData
  }
}
