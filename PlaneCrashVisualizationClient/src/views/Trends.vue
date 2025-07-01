<template>
  <div class="fullscreen-container">
    <!-- Loading indicator -->
    <div v-if="loading" class="loading-overlay">
      <div class="loading-spinner"></div>
      <div class="mt-3 text-center">Loading trend data...</div>
    </div>

    <div class="container-fluid">
      <div class="content-wrapper">
      <div class="row">
        <div class="col-12">
          <nav aria-label="breadcrumb">
            <ol class="breadcrumb mb-2">
              <li class="breadcrumb-item">
                <router-link to="/" class="text-decoration-none">Home</router-link>
              </li>
              <li class="breadcrumb-item active" aria-current="page">Trend Analysis</li>
            </ol>
          </nav>
        </div>
      </div>

      <div class="row mb-2">
        <div class="col-12">
          <h1 class="h3 mb-2">Trend Analysis</h1>
          <p class="lead text-muted mb-3">Analyze patterns and trends in aviation safety over time</p>
        </div>
      </div>      <!-- Key Statistics Cards -->
      <div class="row mb-3">
        <div class="col-md-3 mb-2">
        <div class="card text-center border-danger">
          <div class="card-body">
            <div class="text-danger mb-2">
              <i class="bi bi-exclamation-triangle-fill" style="font-size: 2rem;"></i>
            </div>
            <h3 class="card-title text-danger">{{ stats.totalCrashes.toLocaleString() }}</h3>
            <p class="card-text text-muted">Total Crashes</p>
          </div>
        </div>
      </div>        <div class="col-md-3 mb-2">
          <div class="card text-center border-dark">
            <div class="card-body">
              <div class="text-dark mb-2">
                <i class="bi bi-people-fill" style="font-size: 2rem;"></i>
              </div>
              <h3 class="card-title text-dark">{{ stats.totalFatalities.toLocaleString() }}</h3>
              <p class="card-text text-muted">Total Fatalities</p>
            </div>
          </div>
        </div>
        
        <div class="col-md-3 mb-2">
          <div class="card text-center border-warning">
            <div class="card-body">
              <div class="text-warning mb-2">
                <i class="bi bi-calendar-year" style="font-size: 2rem;"></i>
              </div>
              <h3 class="card-title text-warning">{{ stats.worstYear }}</h3>
              <p class="card-text text-muted">Worst Year</p>
            </div>
          </div>
        </div>
        
        <div class="col-md-3 mb-2">
        <div class="card text-center border-info">
          <div class="card-body">
            <div class="text-info mb-2">
              <i class="bi bi-airplane" style="font-size: 2rem;"></i>
            </div>
            <h3 class="card-title text-info">{{ stats.commonAircraft }}</h3>
            <p class="card-text text-muted">Most Common Aircraft</p>
          </div>
        </div>
      </div>
    </div>      <!-- Charts Row -->
      <div class="row mb-3">
        <!-- Crashes by Year Chart -->
        <div class="col-lg-12 mb-3">
        <div class="card">
          <div class="card-header">
            <h5 class="card-title mb-3">Crashes by Year</h5>
            <div class="year-range-controls">
              <div class="row align-items-center">
                <div class="col-md-9">
                  <div class="range-slider-container">
                    <input 
                      type="range" 
                      class="form-range range-min" 
                      :min="minAvailableYear" 
                      :max="maxAvailableYear"
                      v-model="selectedMinYear"
                      @input="onYearRangeChange"
                    >
                    <input 
                      type="range" 
                      class="form-range range-max" 
                      :min="minAvailableYear" 
                      :max="maxAvailableYear"
                      v-model="selectedMaxYear"
                      @input="onYearRangeChange"
                    >
                  </div>
                </div>
                <div class="col-md-3">
                  <div class="d-flex flex-column align-items-end">
                    <div class="mb-1">
                      <small class="text-muted">From: </small>
                      <span class="fw-bold">{{ selectedMinYear }}</span>
                    </div>
                    <div>
                      <small class="text-muted">To: </small>
                      <span class="fw-bold">{{ selectedMaxYear }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="card-body">
            <div class="chart-container" style="height: 300px;">
              <canvas ref="yearChart"></canvas>
            </div>
          </div>
        </div>
      </div>
      </div>      <!-- Casualties by Operator Chart -->
      <div class="row mb-3">
        <div class="col-lg-12 mb-3">
        <div class="card">
          <div class="card-header">
            <h5 class="card-title mb-0">Casualties by Operator</h5>
          </div>
          <div class="card-body">
            <div class="chart-container" style="height: 400px;">
              <canvas ref="operatorChart"></canvas>
            </div>
          </div>
        </div>
      </div>
    </div>

      <!-- Crashes by Continent Pie Chart -->
      <div class="row">
        <div class="col-12 mb-2">
        <div class="card">
          <div class="card-header">
            <h5 class="card-title mb-0">Crashes by Continent</h5>
          </div>
          <div class="card-body">
            <div class="row">
              <div class="col-md-8">
                <div class="chart-container" style="height: 400px;">
                  <canvas ref="continentChart"></canvas>
                </div>
              </div>
              <div class="col-md-4">
                <div class="legend mt-3">
                  <h6 class="mb-3">Distribution Summary</h6>
                  <div v-for="continent in continentData" :key="continent.continent" class="d-flex justify-content-between align-items-center mb-2">
                    <span class="small">{{ continent.continent }}</span>
                    <span class="badge bg-secondary">{{ continent.crashCount }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      </div>

      <div class="row mb-3">
        <!-- Most Common Aircraft Models -->
        <div class="col-lg-6 mb-3">
        <div class="card">
          <div class="card-header">
            <h5 class="card-title mb-0">Most Common Aircraft Models</h5>
          </div>
          <div class="card-body">
            <div class="table-responsive">
              <table class="table table-sm">
                <thead>
                  <tr>
                    <th>Rank</th>
                    <th>Aircraft Model</th>
                    <th>Crashes</th>
                    <th>Fatality Rate</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(aircraft, index) in topAircraft.slice(0, 10)" :key="aircraft.aircraftModel">
                    <td>{{ index + 1 }}</td>
                    <td class="text-truncate" style="max-width: 150px;" :title="aircraft.aircraftModel">
                      {{ aircraft.aircraftModel || 'Unknown' }}
                    </td>
                    <td>
                      <span class="badge bg-warning text-dark">{{ aircraft.crashCount }}</span>
                    </td>
                    <td>
                      <span v-if="aircraft.fatalityRate !== null" class="text-muted">
                        {{ aircraft.fatalityRate.toFixed(1) }}%
                      </span>
                      <span v-else class="text-muted">N/A</span>
                    </td>
                  </tr>
                  <tr v-if="topAircraft.length === 0">
                    <td colspan="4" class="text-center text-muted">Loading aircraft data...</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>        <!-- Most Common Manufacturers -->
        <div class="col-lg-6 mb-3">
        <div class="card">
          <div class="card-header">
            <h5 class="card-title mb-0">Most Common Manufacturers</h5>
          </div>
          <div class="card-body">
            <div class="table-responsive">
              <table class="table table-sm">
                <thead>
                  <tr>
                    <th>Rank</th>
                    <th>Manufacturer</th>
                    <th>Crashes</th>
                    <th>Fatality Rate</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(manufacturer, index) in topManufacturers.slice(0, 10)" :key="manufacturer.manufacturerName">
                    <td>{{ index + 1 }}</td>
                    <td class="text-truncate" style="max-width: 150px;" :title="manufacturer.manufacturerName">
                      {{ manufacturer.manufacturerName || 'Unknown' }}
                    </td>
                    <td>
                      <span class="badge bg-info text-dark">{{ manufacturer.crashCount }}</span>
                    </td>
                    <td>
                      <span v-if="manufacturer.fatalityRate !== null" class="text-muted">
                        {{ manufacturer.fatalityRate.toFixed(1) }}%
                      </span>
                      <span v-else class="text-muted">N/A</span>
                    </td>
                  </tr>
                  <tr v-if="topManufacturers.length === 0">
                    <td colspan="4" class="text-center text-muted">Loading manufacturer data...</td>
                  </tr>
                </tbody>
              </table>
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
import { ref, onMounted, watch, onBeforeUnmount } from 'vue'
import axios from 'axios'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  LineController,
  BarElement,
  BarController,
  ArcElement,
  PieController,
  Title,
  Tooltip,
  Legend,
  Filler
} from 'chart.js'

// Register Chart.js components
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  LineController,
  BarElement,
  BarController,
  ArcElement,
  PieController,
  Title,
  Tooltip,
  Legend,
  Filler
)

// API Base URL
const API_BASE_URL = 'http://localhost:5021/api'

// Data and state
const yearRange = ref('10')
const yearChart = ref(null)
const operatorChart = ref(null)
const continentChart = ref(null)
const causeChart = ref(null)
const decadeChart = ref(null)
const loading = ref(false)

// Year range slider controls
const minAvailableYear = ref(1908)
const maxAvailableYear = ref(new Date().getFullYear())
const selectedMinYear = ref(2015)
const selectedMaxYear = ref(new Date().getFullYear())

// Chart.js instances
let yearChartInstance = null
let operatorChartInstance = null
let continentChartInstance = null

// Real data from API
const stats = ref({
  totalCrashes: 0,
  totalFatalities: 0,
  totalAboard: 0,
  worstYear: 0,
  commonAircraft: 'Loading...'
})

const crashesByYear = ref([])
const topAirlines = ref([])
const topAircraft = ref([])
const topManufacturers = ref([])
const continentData = ref([])
const regions = ref([])
const decadeData = ref([])

// Fetch summary statistics
const fetchSummaryStats = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/crashes/summary`)
    // Don't overwrite worstYear if it's already calculated
    const currentWorstYear = stats.value.worstYear
    stats.value = {
      totalCrashes: response.data.totalCrashes,
      totalFatalities: response.data.totalFatalities,
      totalAboard: response.data.totalAboard,
      worstYear: (currentWorstYear && currentWorstYear !== 'Calculating...') ? currentWorstYear : 'Calculating...',
      commonAircraft: 'Calculating...'
    }
  } catch (error) {
    console.error('Error fetching summary stats:', error)
  }
}

// Fetch crashes by year data
const fetchCrashesByYear = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/crashes/by-year`)
    console.log('API Response for by-year:', response.data)
    crashesByYear.value = response.data
    
    // Set available year range based on data
    if (response.data.length > 0) {
      const years = response.data.map(item => item.year).filter(year => year != null)
      minAvailableYear.value = Math.min(...years)
      maxAvailableYear.value = Math.max(...years)
      
      // Set default selection to last 10 years
      selectedMinYear.value = Math.max(minAvailableYear.value, maxAvailableYear.value - 9)
      selectedMaxYear.value = maxAvailableYear.value
      
      // Find worst year (most fatalities)
      const worstYear = response.data.reduce((prev, current) => 
        (prev.totalFatalities > current.totalFatalities) ? prev : current
      )
      
      // Update the stats object properly
      stats.value = {
        ...stats.value,
        worstYear: worstYear.year
      }
      
      console.log('Worst year calculated:', worstYear.year, 'with', worstYear.totalFatalities, 'fatalities')
    }
  } catch (error) {
    console.error('Error fetching crashes by year:', error)
  }
}

// Fetch top airlines data
const fetchTopAirlines = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/crashes/by-operator?limit=10`)
    topAirlines.value = response.data.map(item => ({
      name: item.operator,
      incidents: item.crashCount,
      fatalities: item.totalFatalities
    }))
  } catch (error) {
    console.error('Error fetching top airlines:', error)
    // Fallback placeholder data
    topAirlines.value = []
  }
}

// Fetch most common aircraft data
const fetchMostCommonAircraft = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/crashes/most-common-aircraft?limit=15`)
    topAircraft.value = response.data.map(item => ({
      aircraftType: item.aircraftType,
      crashCount: item.crashCount,
      totalFatalities: item.totalFatalities,
      totalAboard: item.totalAboard,
      fatalityRate: item.fatalityRate
    }))
    
    // Update common aircraft in stats
    if (topAircraft.value.length > 0) {
      stats.value = {
        ...stats.value,
        commonAircraft: topAircraft.value[0].aircraftType
      }
    }
  } catch (error) {
    console.error('Error fetching aircraft data:', error)
    topAircraft.value = []
  }
}

// Fetch most common manufacturers data
const fetchMostCommonManufacturers = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/crashes/most-common-manufacturers?limit=15`)
    topManufacturers.value = response.data.map(item => ({
      manufacturerName: item.manufacturerName,
      crashCount: item.crashCount,
      totalFatalities: item.totalFatalities,
      totalAboard: item.totalAboard,
      fatalityRate: item.fatalityRate
    }))
  } catch (error) {
    console.error('Error fetching manufacturer data:', error)
    topManufacturers.value = []
  }
}

// Fetch crashes by continent data
const fetchCrashesByContinent = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/crashes/by-continent`)
    continentData.value = response.data
    console.log('Continent data:', continentData.value)
  } catch (error) {
    console.error('Error fetching continent data:', error)
    continentData.value = []
  }
}

// Process geographic data (simplified - based on location analysis)
const processGeographicData = () => {
  // This is a simplified version - ideally you'd have a backend endpoint for this
  regions.value = [
    { name: 'North America', count: Math.floor(stats.value.totalCrashes * 0.25), percentage: 25, color: 'bg-primary' },
    { name: 'Europe', count: Math.floor(stats.value.totalCrashes * 0.22), percentage: 22, color: 'bg-success' },
    { name: 'Asia', count: Math.floor(stats.value.totalCrashes * 0.19), percentage: 19, color: 'bg-warning' },
    { name: 'South America', count: Math.floor(stats.value.totalCrashes * 0.15), percentage: 15, color: 'bg-info' },
    { name: 'Africa', count: Math.floor(stats.value.totalCrashes * 0.13), percentage: 13, color: 'bg-danger' },
    { name: 'Oceania', count: Math.floor(stats.value.totalCrashes * 0.06), percentage: 6, color: 'bg-secondary' }
  ]
}

// Process decade data from yearly data
const processDecadeData = () => {
  const decades = {}
  crashesByYear.value.forEach(yearData => {
    const decade = Math.floor(yearData.year / 10) * 10
    if (!decades[decade]) {
      decades[decade] = { decade, crashes: 0, fatalities: 0 }
    }
    decades[decade].crashes += yearData.crashCount
    decades[decade].fatalities += yearData.totalFatalities
  })
  
  decadeData.value = Object.values(decades).sort((a, b) => a.decade - b.decade)
}

// Handle year range changes
const onYearRangeChange = () => {
  // Ensure min is not greater than max
  if (parseInt(selectedMinYear.value) > parseInt(selectedMaxYear.value)) {
    selectedMinYear.value = selectedMaxYear.value
  }
  
  // Update the visual range indicator
  updateRangeSliderStyles()
  
  // Debounce chart update - only update the year chart with shorter delay
  clearTimeout(window.yearRangeTimeout)
  window.yearRangeTimeout = setTimeout(() => {
    initializeYearChart()
  }, 100) // Reduced from 300ms to 100ms for better responsiveness
}

// Update range slider visual styles
const updateRangeSliderStyles = () => {
  const min = parseInt(selectedMinYear.value)
  const max = parseInt(selectedMaxYear.value)
  const rangeMin = parseInt(minAvailableYear.value)
  const rangeMax = parseInt(maxAvailableYear.value)
  
  const leftPercent = ((min - rangeMin) / (rangeMax - rangeMin)) * 100
  const rightPercent = ((rangeMax - max) / (rangeMax - rangeMin)) * 100
  
  // Update CSS custom properties for the active range
  const container = document.querySelector('.range-slider-container')
  if (container) {
    container.style.setProperty('--range-min', `${leftPercent}%`)
    container.style.setProperty('--range-max', `${rightPercent}%`)
  }
}

// Initialize year chart only
const initializeYearChart = () => {
  console.log('Initializing year chart...')
  
  // Year Chart with Chart.js
  if (yearChart.value && crashesByYear.value.length > 0) {
    let filteredData = crashesByYear.value
    
    // Filter out any items without year property
    filteredData = filteredData.filter(item => item.year != null)
    
    // Filter by selected year range
    filteredData = filteredData.filter(item => 
      item.year >= parseInt(selectedMinYear.value) && 
      item.year <= parseInt(selectedMaxYear.value)
    ).sort((a, b) => a.year - b.year)
    
    console.log('Filtered data for chart:', filteredData)
    
    if (filteredData.length === 0) {
      console.log('No data available for chart')
      return
    }
    
    // If chart already exists, update data instead of recreating
    if (yearChartInstance) {
      yearChartInstance.data.labels = filteredData.map(item => item.year?.toString() || 'Unknown')
      yearChartInstance.data.datasets[0].data = filteredData.map(item => item.crashCount || 0)
      yearChartInstance.data.datasets[1].data = filteredData.map(item => item.totalFatalities || 0)
      yearChartInstance.update('none') // 'none' for no animation
      return
    }
    
    const ctx = yearChart.value.getContext('2d')
    
    yearChartInstance = new ChartJS(ctx, {
      type: 'line',
      data: {
        labels: filteredData.map(item => item.year?.toString() || 'Unknown'),
        datasets: [
          {
            label: 'Number of Crashes',
            data: filteredData.map(item => item.crashCount || 0),
            borderColor: 'rgb(220, 53, 69)',
            backgroundColor: 'rgba(220, 53, 69, 0.1)',
            borderWidth: 2,
            fill: true,
            tension: 0.1,
            pointBackgroundColor: 'rgb(220, 53, 69)',
            pointBorderColor: '#fff',
            pointBorderWidth: 2,
            pointRadius: 4
          },
          {
            label: 'Total Fatalities',
            data: filteredData.map(item => item.totalFatalities || 0),
            borderColor: 'rgb(108, 117, 125)',
            backgroundColor: 'rgba(108, 117, 125, 0.1)',
            borderWidth: 2,
            fill: false,
            tension: 0.1,
            pointBackgroundColor: 'rgb(108, 117, 125)',
            pointBorderColor: '#fff',
            pointBorderWidth: 2,
            pointRadius: 4,
            yAxisID: 'y1'
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        animation: {
          duration: 200 // Faster animation
        },
        interaction: {
          mode: 'index',
          intersect: false,
        },
        plugins: {
          title: {
            display: false
          },
          legend: {
            position: 'top',
            labels: {
              usePointStyle: true,
              padding: 20
            }
          },
          tooltip: {
            backgroundColor: 'rgba(0, 0, 0, 0.8)',
            titleColor: '#fff',
            bodyColor: '#fff',
            borderColor: 'rgba(255, 255, 255, 0.1)',
            borderWidth: 1,
            callbacks: {
              label: function(context) {
                if (context.datasetIndex === 0) {
                  return `Crashes: ${context.parsed.y}`
                } else {
                  return `Fatalities: ${context.parsed.y.toLocaleString()}`
                }
              }
            }
          }
        },
        scales: {
          x: {
            display: true,
            title: {
              display: true,
              text: 'Year'
            },
            grid: {
              display: false
            }
          },
          y: {
            type: 'linear',
            display: true,
            position: 'left',
            title: {
              display: true,
              text: 'Number of Crashes'
            },
            grid: {
              color: 'rgba(0, 0, 0, 0.1)'
            }
          },
          y1: {
            type: 'linear',
            display: true,
            position: 'right',
            title: {
              display: true,
              text: 'Total Fatalities'
            },
            grid: {
              drawOnChartArea: false,
            },
            ticks: {
              callback: function(value) {
                return value.toLocaleString()
              }
            }
          }
        }
      }
    })
  }
}

// Initialize operator chart only
const initializeOperatorChart = () => {
  console.log('Initializing operator chart...')
  
  // Destroy existing operator chart instance
  if (operatorChartInstance) {
    operatorChartInstance.destroy()
    operatorChartInstance = null
  }
  
  // Operator Casualties Bar Chart
  if (operatorChart.value && topAirlines.value.length > 0) {
    const ctx = operatorChart.value.getContext('2d')
    
    // Take top 10 operators by fatalities
    const sortedOperators = [...topAirlines.value]
      .sort((a, b) => b.fatalities - a.fatalities)
      .slice(0, 10)
    
    operatorChartInstance = new ChartJS(ctx, {
      type: 'bar',
      data: {
        labels: sortedOperators.map(item => {
          // Truncate long operator names
          const name = item.name || 'Unknown'
          return name.length > 15 ? name.substring(0, 15) + '...' : name
        }),
        datasets: [{
          label: 'Total Casualties',
          data: sortedOperators.map(item => item.fatalities || 0),
          backgroundColor: 'rgba(220, 53, 69, 0.8)',
          borderColor: 'rgb(220, 53, 69)',
          borderWidth: 1
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          title: {
            display: false
          },
          legend: {
            display: false
          },
          tooltip: {
            backgroundColor: 'rgba(0, 0, 0, 0.8)',
            titleColor: '#fff',
            bodyColor: '#fff',
            borderColor: 'rgba(255, 255, 255, 0.1)',
            borderWidth: 1,
            callbacks: {
              title: function(tooltipItems) {
                const index = tooltipItems[0].dataIndex
                return sortedOperators[index].name || 'Unknown'
              },
              label: function(context) {
                return `Casualties: ${context.parsed.y.toLocaleString()}`
              }
            }
          }
        },
        scales: {
          x: {
            display: true,
            title: {
              display: true,
              text: 'Operator'
            },
            ticks: {
              maxRotation: 45,
              minRotation: 45
            }
          },
          y: {
            display: true,
            title: {
              display: true,
              text: 'Total Casualties'
            },
            ticks: {
              callback: function(value) {
                return value.toLocaleString()
              }
            }
          }
        }
      }
    })
  }
}

// Initialize continent pie chart
const initializeContinentChart = () => {
  console.log('Initializing continent chart...')
  
  // Destroy existing continent chart instance
  if (continentChartInstance) {
    continentChartInstance.destroy()
    continentChartInstance = null
  }
  
  // Continent Pie Chart
  if (continentChart.value && continentData.value.length > 0) {
    const ctx = continentChart.value.getContext('2d')
    
    // Define colors for each continent
    const colors = [
      '#FF6384', // Red-pink
      '#36A2EB', // Blue
      '#FFCE56', // Yellow
      '#4BC0C0', // Teal
      '#9966FF', // Purple
      '#FF9F40', // Orange
      '#FF6384'  // Red-pink (fallback)
    ]
    
    continentChartInstance = new ChartJS(ctx, {
      type: 'pie',
      data: {
        labels: continentData.value.map(item => item.continent || 'Unknown'),
        datasets: [{
          data: continentData.value.map(item => item.crashCount || 0),
          backgroundColor: colors.slice(0, continentData.value.length),
          borderColor: colors.slice(0, continentData.value.length).map(color => color),
          borderWidth: 2
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'bottom',
            labels: {
              padding: 20,
              usePointStyle: true
            }
          },
          tooltip: {
            backgroundColor: 'rgba(0, 0, 0, 0.8)',
            titleColor: '#fff',
            bodyColor: '#fff',
            callbacks: {
              label: function(context) {
                const label = context.label || ''
                const value = context.parsed || 0
                const total = context.dataset.data.reduce((a, b) => a + b, 0)
                const percentage = ((value / total) * 100).toFixed(1)
                return `${label}: ${value} crashes (${percentage}%)`
              }
            }
          }
        }
      }
    })
  }
}

// Initialize other charts (cause and decade charts)
const initializeOtherCharts = () => {
  // Cause Chart - simplified placeholder
  if (causeChart.value) {
    const causeHtml = `
      <div class="h-100 d-flex flex-column justify-content-center text-center">
        <h6 class="mb-3">Primary Causes</h6>
        <div class="mb-2"><span class="badge bg-danger me-2">35%</span> Pilot Error</div>
        <div class="mb-2"><span class="badge bg-warning me-2">25%</span> Mechanical</div>
        <div class="mb-2"><span class="badge bg-info me-2">20%</span> Weather</div>
        <div class="mb-2"><span class="badge bg-secondary me-2">20%</span> Other</div>
        <small class="text-muted mt-2">Based on analysis patterns</small>
      </div>
    `
    causeChart.value.innerHTML = causeHtml
  }
  
  // Decade Chart
  if (decadeChart.value && decadeData.value.length > 0) {
    const decadeHtml = `
      <div class="h-100 d-flex flex-column justify-content-center">
        <h6 class="text-center mb-3">Fatalities by Decade</h6>
        <div class="row text-center">
          ${decadeData.value.slice(-4).map(item => `
            <div class="col">
              <div class="text-danger fw-bold">${item.fatalities.toLocaleString()}</div>
              <small class="text-muted">${item.decade}s</small>
            </div>
          `).join('')}
        </div>
        <small class="text-muted text-center mt-2">Chart.js integration needed for full visualization</small>
      </div>
    `
    decadeChart.value.innerHTML = decadeHtml
  }
}

// Initialize all charts (used on initial load)
const initializeCharts = () => {
  console.log('Initializing all charts...')
  initializeYearChart()
  initializeOperatorChart()
  initializeContinentChart()
  initializeOtherCharts()
}

// Fetch all data
const fetchAllData = async () => {
  loading.value = true
  try {
    await Promise.all([
      fetchSummaryStats(),
      fetchCrashesByYear(),
      fetchTopAirlines(),
      fetchMostCommonAircraft(),
      fetchMostCommonManufacturers(),
      fetchCrashesByContinent()
    ])
    
    // Process derived data
    processGeographicData()
    processDecadeData()
    
    // Initialize charts after data is loaded
    setTimeout(() => {
      initializeCharts()
    }, 100)
  } catch (error) {
    console.error('Error fetching data:', error)
  } finally {
    loading.value = false
  }
}

// Watch for year range changes
watch([selectedMinYear, selectedMaxYear], () => {
  console.log('Year range changed to:', selectedMinYear.value, '-', selectedMaxYear.value)
  updateRangeSliderStyles()
  initializeYearChart()
})

onMounted(() => {
  fetchAllData()
  // Initialize range slider styles after a short delay to ensure DOM is ready
  setTimeout(() => {
    updateRangeSliderStyles()
  }, 200)
})

onBeforeUnmount(() => {
  // Clean up chart instances
  if (yearChartInstance) {
    yearChartInstance.destroy()
    yearChartInstance = null
  }
  if (operatorChartInstance) {
    operatorChartInstance.destroy()
    operatorChartInstance = null
  }
  if (continentChartInstance) {
    continentChartInstance.destroy()
    continentChartInstance = null
  }
})
</script>

<style scoped>
.fullscreen-container {
  position: fixed !important;
  top: 60px !important;
  left: 0 !important;
  right: 0 !important;
  bottom: 0 !important;
  width: 100vw !important;
  height: calc(100vh - 60px) !important;
  margin: 0 !important;
  padding: 0 !important;
  overflow-x: hidden;
  overflow-y: auto;
}

.container-fluid {
  margin: 0 !important;
  padding: 0 !important;
  max-width: none !important;
  width: 100% !important;
}

.content-wrapper {
  padding: 15px;
  margin: 0 auto;
  max-width: 1400px;
}

/* Ensure rows and columns have no extra spacing */
.row {
  margin-left: 0 !important;
  margin-right: 0 !important;
}

.col-12, .col-md-3, .col-lg-4, .col-lg-6, .col-lg-8 {
  padding-left: 4px !important;
  padding-right: 4px !important;
}

/* Remove any Bootstrap container gutters */
.container-fluid .row {
  --bs-gutter-x: 0;
}

/* Force proper positioning */
.fullscreen-container * {
  box-sizing: border-box;
}

/* Override any inherited margins or paddings */
.breadcrumb {
  margin-left: 8px !important;
}

.card {
  margin-left: 0 !important;
  margin-right: 0 !important;
}

.chart-container {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f8f9fa;
  border-radius: 0.375rem;
}

.progress-stacked {
  height: 20px;
}

.legend .badge {
  border-radius: 50%;
}

.card-title {
  font-weight: 600;
}

.btn-check:checked + .btn-outline-primary {
  background-color: #0d6efd;
  border-color: #0d6efd;
  color: white;
}

.table th {
  border-top: none;
  font-weight: 600;
  color: #495057;
}

.list-unstyled li {
  display: flex;
  align-items: center;
}

/* Loading indicator */
.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255, 255, 255, 0.9);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  z-index: 2000;
}

.loading-spinner {
  border: 4px solid #f3f3f3;
  border-top: 4px solid #007bff;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Year Range Slider Styles */
.year-range-controls {
  margin-bottom: 1rem;
}

.range-slider-container {
  position: relative;
  height: 40px;
  margin: 10px 0;
  padding: 0;
}

.range-slider-container input[type="range"] {
  position: absolute;
  width: 100%;
  height: 6px;
  background: transparent;
  pointer-events: none;
  -webkit-appearance: none;
  appearance: none;
  top: 50%;
  transform: translateY(-50%);
  left: 0;
  margin: 0;
  padding: 0;
  border: none;
  outline: none;
}

/* Webkit browsers (Chrome, Safari, etc.) */
.range-slider-container input[type="range"]::-webkit-slider-track {
  width: 100%;
  height: 6px;
  background: transparent;
  border: none;
  outline: none;
  margin: 0;
  padding: 0;
}

.range-slider-container input[type="range"]::-webkit-slider-thumb {
  pointer-events: all;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  border: 2px solid #0d6efd;
  background: #fff;
  cursor: pointer;
  -webkit-appearance: none;
  appearance: none;
  box-shadow: 0 2px 4px rgba(0,0,0,0.2);
  position: relative;
  z-index: 3;
  margin: 0;
}

/* Firefox */
.range-slider-container input[type="range"]::-moz-range-track {
  width: 100%;
  height: 6px;
  background: transparent;
  border: none;
  outline: none;
  margin: 0;
  padding: 0;
}

.range-slider-container input[type="range"]::-moz-range-thumb {
  pointer-events: all;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  border: 2px solid #0d6efd;
  background: #fff;
  cursor: pointer;
  box-shadow: 0 2px 4px rgba(0,0,0,0.2);
  -moz-appearance: none;
  margin: 0;
}

/* Layer order for sliders */
.range-slider-container .range-min {
  z-index: 1;
}

.range-slider-container .range-max {
  z-index: 2;
}

/* Background track */
.range-slider-container::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  height: 6px;
  background: #e9ecef;
  border-radius: 3px;
  transform: translateY(-50%);
  z-index: 0;
}

/* Active range between sliders */
.range-slider-container::after {
  content: '';
  position: absolute;
  top: 50%;
  height: 6px;
  background: #0d6efd;
  border-radius: 3px;
  transform: translateY(-50%);
  z-index: 0;
  left: var(--range-min);
  right: var(--range-max);
}
</style>
