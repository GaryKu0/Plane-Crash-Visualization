<template>
  <div class="pie-chart-container">
    <div class="card">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h5 class="card-title mb-0">
          <i class="bi bi-pie-chart me-2"></i>
          Crashes by Continent
        </h5>
        <button 
          class="btn btn-sm btn-outline-secondary"
          @click="toggleExpanded"
        >
          <i :class="isExpanded ? 'bi bi-arrows-angle-contract' : 'bi bi-arrows-angle-expand'"></i>
        </button>
      </div>
      <div class="card-body">
        <div v-if="loading" class="text-center p-4">
          <div class="spinner-border spinner-border-sm text-primary me-2" role="status"></div>
          Loading continent data...
        </div>
        <div v-else-if="error" class="alert alert-danger">
          {{ error }}
        </div>
        <div v-else-if="chartData.length === 0" class="text-center text-muted p-4">
          No data available
        </div>
        <div v-else>
          <canvas 
            ref="chartCanvas" 
            :width="chartWidth" 
            :height="chartHeight"
          ></canvas>
          
          <!-- Legend/Summary -->
          <div class="mt-3">
            <div class="row">
              <div v-for="item in chartData" :key="item.continent" class="col-md-6 mb-2">
                <div class="d-flex align-items-center">
                  <div 
                    class="legend-color me-2" 
                    :style="{ backgroundColor: item.color }"
                  ></div>
                  <small class="text-muted">
                    <strong>{{ item.continent }}</strong>: {{ item.crashCount }} crashes
                    <span v-if="item.totalFatalities > 0">
                      ({{ item.totalFatalities }} fatalities)
                    </span>
                  </small>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, watch, computed, nextTick } from 'vue'
import { Chart, registerables } from 'chart.js'

// Register Chart.js components
Chart.register(...registerables)

// Props
const props = defineProps({
  filters: {
    type: Object,
    default: () => ({})
  },
  expanded: {
    type: Boolean,
    default: false
  }
})

// Emits
const emit = defineEmits(['update:expanded'])

// Reactive data
const chartCanvas = ref(null)
const chartInstance = ref(null)
const chartData = ref([])
const loading = ref(true)
const error = ref(null)
const isExpanded = ref(props.expanded)

// Computed properties
const chartWidth = computed(() => isExpanded.value ? 600 : 300)
const chartHeight = computed(() => isExpanded.value ? 400 : 200)

// Continent colors
const continentColors = {
  'North America': '#FF6384',
  'Europe': '#36A2EB',
  'Asia': '#FFCE56',
  'South America': '#4BC0C0',
  'Africa': '#9966FF',
  'Oceania': '#FF9F40',
  'Antarctica': '#C9CBCF'
}

// Methods
const toggleExpanded = () => {
  isExpanded.value = !isExpanded.value
  emit('update:expanded', isExpanded.value)
  // Redraw chart with new size
  nextTick(() => {
    if (chartInstance.value) {
      chartInstance.value.resize()
    }
  })
}

const buildApiUrl = () => {
  const baseUrl = '/api/crashes/by-continent'
  const params = new URLSearchParams()
  
  if (props.filters.startYear) params.append('startYear', props.filters.startYear)
  if (props.filters.endYear) params.append('endYear', props.filters.endYear)
  if (props.filters.operator) params.append('operator_', props.filters.operator)
  if (props.filters.manufacturer) params.append('manufacturer', props.filters.manufacturer)
  
  return `${baseUrl}?${params.toString()}`
}

const fetchData = async () => {
  try {
    loading.value = true
    error.value = null
    
    const response = await fetch(buildApiUrl())
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    
    const data = await response.json()
    chartData.value = data.map(item => ({
      continent: item.continent,
      crashCount: item.crashCount,
      totalFatalities: item.totalFatalities,
      totalAboard: item.totalAboard,
      fatalityRate: item.fatalityRate,
      color: continentColors[item.continent] || '#CCCCCC'
    }))
    
    createChart()
  } catch (err) {
    error.value = `Failed to load continent data: ${err.message}`
    console.error('Error fetching continent data:', err)
  } finally {
    loading.value = false
  }
}

const createChart = () => {
  if (!chartCanvas.value || chartData.value.length === 0) return
  
  // Destroy existing chart
  if (chartInstance.value) {
    chartInstance.value.destroy()
  }
  
  const ctx = chartCanvas.value.getContext('2d')
  
  chartInstance.value = new Chart(ctx, {
    type: 'pie',
    data: {
      labels: chartData.value.map(item => item.continent),
      datasets: [{
        data: chartData.value.map(item => item.crashCount),
        backgroundColor: chartData.value.map(item => item.color),
        borderColor: '#ffffff',
        borderWidth: 2
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: false // We'll use our custom legend
        },
        tooltip: {
          callbacks: {
            label: function(context) {
              const item = chartData.value[context.dataIndex]
              const percentage = ((item.crashCount / chartData.value.reduce((sum, d) => sum + d.crashCount, 0)) * 100).toFixed(1)
              return [
                `${item.continent}: ${item.crashCount} crashes (${percentage}%)`,
                `Fatalities: ${item.totalFatalities}`,
                `Fatality Rate: ${item.fatalityRate.toFixed(1)}%`
              ]
            }
          }
        }
      }
    }
  })
}

// Watchers
watch(() => props.filters, fetchData, { deep: true })
watch(() => props.expanded, (newVal) => {
  isExpanded.value = newVal
})

// Lifecycle
onMounted(() => {
  fetchData()
})

onUnmounted(() => {
  if (chartInstance.value) {
    chartInstance.value.destroy()
  }
})
</script>

<style scoped>
.pie-chart-container {
  position: relative;
}

.legend-color {
  width: 12px;
  height: 12px;
  border-radius: 2px;
  flex-shrink: 0;
}

.card {
  box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
  border: 1px solid rgba(0, 0, 0, 0.125);
}

.card-header {
  background-color: #f8f9fa;
  border-bottom: 1px solid rgba(0, 0, 0, 0.125);
}

canvas {
  max-width: 100%;
  height: auto;
}
</style>
