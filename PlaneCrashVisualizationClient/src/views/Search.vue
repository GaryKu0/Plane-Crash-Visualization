<template>
  <div class="container py-4">
    <div class="row">
      <div class="col-12">
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb">
            <li class="breadcrumb-item">
              <router-link to="/" class="text-decoration-none">Home</router-link>
            </li>
            <li class="breadcrumb-item active" aria-current="page">Advanced Search</li>
          </ol>
        </nav>
      </div>
    </div>

    <div class="row">
      <div class="col-12">
        <h1 class="h2 mb-4">Advanced Search</h1>
      </div>
    </div>

    <div class="row">
      <div class="col-lg-4">
        <div class="card sticky-top" style="top: 1rem;">
          <div class="card-header">
            <h5 class="card-title mb-0">
              <i class="bi bi-funnel me-2"></i>Search Filters
            </h5>
          </div>
          <div class="card-body">
            <form @submit.prevent="performSearch">
              <!-- Date Filters -->
              <div class="filter-section">
                <h6 class="filter-section-title">
                  <i class="bi bi-calendar3 me-2"></i>Date Range
                </h6>
                <div class="row g-2 mb-3">
                  <div class="col-6">
                    <label for="dateFrom" class="form-label small">From</label>
                    <input type="date" class="form-control form-control-sm" id="dateFrom" v-model="filters.dateFrom">
                  </div>
                  <div class="col-6">
                    <label for="dateTo" class="form-label small">To</label>
                    <input type="date" class="form-control form-control-sm" id="dateTo" v-model="filters.dateTo">
                  </div>
                </div>
              </div>

              <!-- Location and Operator Filters -->
              <div class="filter-section">
                <h6 class="filter-section-title">
                  <i class="bi bi-geo-alt me-2"></i>Location & Operator
                </h6>
                <div class="mb-3">
                  <label for="location" class="form-label small">Location</label>
                  <input type="text" class="form-control form-control-sm" id="location" v-model="filters.location" placeholder="Country, City, or Region">
                </div>
                
                <div class="mb-3">
                  <label for="operator" class="form-label small">Operator</label>
                  <select class="form-select form-select-sm" id="operator" v-model="filters.operator">
                    <option value="">All Operators</option>
                    <option v-for="operator in operators" :key="operator" :value="operator">{{ operator }}</option>
                  </select>
                </div>
                
                <div class="mb-3">
                  <label for="flight" class="form-label small">Flight Number</label>
                  <input type="text" class="form-control form-control-sm" id="flight" v-model="filters.flight" placeholder="e.g., AA1234">
                </div>
                
                <div class="mb-3">
                  <label for="route" class="form-label small">Route</label>
                  <input type="text" class="form-control form-control-sm" id="route" v-model="filters.route" placeholder="e.g., New York - London">
                </div>
              </div>

              <!-- Aircraft Filters -->
              <div class="filter-section">
                <h6 class="filter-section-title">
                  <i class="bi bi-airplane me-2"></i>Aircraft Details
                </h6>
                <div class="mb-3">
                  <label for="manufacturer" class="form-label small">Manufacturer</label>
                  <select class="form-select form-select-sm" id="manufacturer" v-model="filters.manufacturer">
                    <option value="">All Manufacturers</option>
                    <option v-for="manufacturer in manufacturers" :key="manufacturer" :value="manufacturer">{{ manufacturer }}</option>
                  </select>
                </div>
                
                <div class="mb-3">
                  <label for="acModel" class="form-label small">Aircraft Model</label>
                  <input type="text" class="form-control form-control-sm" id="acModel" v-model="filters.acModel" placeholder="e.g., 737-800">
                </div>
                
                <div class="mb-3">
                  <label for="registration" class="form-label small">Registration</label>
                  <input type="text" class="form-control form-control-sm" id="registration" v-model="filters.registration" placeholder="e.g., N12345">
                </div>
                
                <div class="mb-3">
                  <label for="cnLn" class="form-label small">Construction Number</label>
                  <input type="text" class="form-control form-control-sm" id="cnLn" v-model="filters.cnLn" placeholder="c/n or l/n">
                </div>
              </div>

              <!-- Casualties Range Filters -->
              <div class="filter-section">
                <h6 class="filter-section-title">
                  <i class="bi bi-people me-2"></i>People Aboard
                </h6>
                <div class="mb-3">
                  <label class="form-label small">Total Aboard</label>
                  <DualRangeSlider
                    :min="filterConfig.ABOARD_RANGE.MIN"
                    :max="filterConfig.ABOARD_RANGE.MAX"
                    v-model="filters.aboardRange"
                  />
                </div>
                
                <div class="mb-3">
                  <label class="form-label small">Passengers Aboard</label>
                  <DualRangeSlider
                    :min="filterConfig.PASSENGERS_RANGE.MIN"
                    :max="filterConfig.PASSENGERS_RANGE.MAX"
                    v-model="filters.passengersAboardRange"
                  />
                </div>
                
                <div class="mb-3">
                  <label class="form-label small">Crew Aboard</label>
                  <DualRangeSlider
                    :min="filterConfig.CREW_RANGE.MIN"
                    :max="filterConfig.CREW_RANGE.MAX"
                    v-model="filters.crewAboardRange"
                  />
                </div>
              </div>

              <div class="filter-section">
                <h6 class="filter-section-title">
                  <i class="bi bi-exclamation-triangle me-2"></i>Fatalities
                </h6>
                <div class="mb-3">
                  <label class="form-label small">Total Fatalities</label>
                  <DualRangeSlider
                    :min="filterConfig.FATALITIES_RANGE.MIN"
                    :max="filterConfig.FATALITIES_RANGE.MAX"
                    v-model="filters.fatalitiesRange"
                  />
                </div>
                
                <div class="mb-3">
                  <label class="form-label small">Passenger Fatalities</label>
                  <DualRangeSlider
                    :min="filterConfig.PASSENGERS_RANGE.MIN"
                    :max="filterConfig.PASSENGERS_RANGE.MAX"
                    v-model="filters.passengerFatalitiesRange"
                  />
                </div>
                
                <div class="mb-3">
                  <label class="form-label small">Crew Fatalities</label>
                  <DualRangeSlider
                    :min="filterConfig.CREW_RANGE.MIN"
                    :max="filterConfig.CREW_RANGE.MAX"
                    v-model="filters.crewFatalitiesRange"
                  />
                </div>
                
                <div class="mb-3">
                  <label class="form-label small">Ground Fatalities</label>
                  <DualRangeSlider
                    :min="filterConfig.GROUND_RANGE.MIN"
                    :max="filterConfig.GROUND_RANGE.MAX"
                    v-model="filters.groundRange"
                  />
                </div>
              </div>
              
              <div class="d-grid gap-2">
                <button type="submit" class="btn btn-primary" :disabled="loading">
                  <i class="bi bi-search me-2"></i>
                  {{ loading ? 'Searching...' : 'Search' }}
                </button>
                
                <button type="button" class="btn btn-outline-secondary" @click="clearFilters">
                  <i class="bi bi-arrow-clockwise me-2"></i>Clear Filters
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
      
      <div class="col-lg-8">
        <div class="card">
          <div class="card-header d-flex justify-content-between align-items-center">
            <h5 class="card-title mb-0">Search Results</h5>
            <div class="d-flex align-items-center gap-3">
              <span class="badge bg-secondary">{{ searchResults.length }} results</span>
              <div class="dropdown" v-if="searchResults.length > 0">
                <button class="btn btn-outline-secondary btn-sm dropdown-toggle" type="button" data-bs-toggle="dropdown">
                  <i class="bi bi-sort-down me-1"></i>Sort
                </button>
                <ul class="dropdown-menu">
                  <li><a class="dropdown-item" href="#" @click="sortResults('date-desc')">Date (Newest First)</a></li>
                  <li><a class="dropdown-item" href="#" @click="sortResults('date-asc')">Date (Oldest First)</a></li>
                  <li><a class="dropdown-item" href="#" @click="sortResults('fatalities-desc')">Fatalities (High to Low)</a></li>
                  <li><a class="dropdown-item" href="#" @click="sortResults('fatalities-asc')">Fatalities (Low to High)</a></li>
                </ul>
              </div>
            </div>
          </div>
          <div class="card-body">
            <div v-if="loading" class="text-center py-5">
              <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
              </div>
              <p class="mt-3 text-muted">Searching crash database...</p>
            </div>
            
            <div v-else-if="error" class="alert alert-danger" role="alert">
              <i class="bi bi-exclamation-triangle me-2"></i>
              <strong>Error:</strong> {{ error }}
            </div>
            
            <div v-else-if="searchResults.length === 0" class="text-center py-5 text-muted">
              <i class="bi bi-search" style="font-size: 3rem;"></i>
              <h4 class="mt-3">{{ hasSearched ? 'No results found' : 'Ready to search' }}</h4>
              <p>{{ hasSearched ? 'Try adjusting your search criteria to find more results.' : 'Use the filters on the left to search through the crash database.' }}</p>
            </div>
            
            <div v-else>
              <div class="row">
                <div v-for="result in paginatedResults" :key="result.id" class="col-12 mb-3">
                  <div class="card border-start border-4 border-danger h-100">
                    <div class="card-body">
                      <div class="row">
                        <div class="col-md-8">
                          <h6 class="card-title d-flex align-items-center">
                            <i class="bi bi-calendar3 me-2 text-muted"></i>
                            {{ formatDate(result.date) }}
                            <span v-if="result.time" class="ms-2 badge bg-light text-dark">{{ formatTime(result.time) }}</span>
                          </h6>
                          <p class="card-subtitle mb-2 text-muted">
                            <i class="bi bi-geo-alt me-1"></i>{{ result.location || 'Unknown Location' }}
                          </p>
                          
                          <div class="crash-details">
                            <div class="row g-2 mb-2">
                              <div class="col-6" v-if="result.operator_">
                                <small class="text-muted">Operator:</small><br>
                                <strong>{{ result.operator_ }}</strong>
                              </div>
                              <div class="col-6" v-if="result.flight">
                                <small class="text-muted">Flight:</small><br>
                                <strong>{{ result.flight }}</strong>
                              </div>
                            </div>
                            
                            <div class="row g-2 mb-2" v-if="result.manufacturer || result.ac_model">
                              <div class="col-6" v-if="result.manufacturer">
                                <small class="text-muted">Manufacturer:</small><br>
                                <strong>{{ result.manufacturer }}</strong>
                              </div>
                              <div class="col-6" v-if="result.ac_model">
                                <small class="text-muted">Model:</small><br>
                                <strong>{{ result.ac_model }}</strong>
                              </div>
                            </div>
                            
                            <div class="casualties-info mt-3">
                              <div class="row g-2 text-center">
                                <div class="col-3" v-if="result.aboard">
                                  <div class="stat-box bg-light">
                                    <div class="stat-number">{{ result.aboard }}</div>
                                    <div class="stat-label">Aboard</div>
                                  </div>
                                </div>
                                <div class="col-3" v-if="result.fatalities">
                                  <div class="stat-box bg-danger-light">
                                    <div class="stat-number text-danger">{{ result.fatalities }}</div>
                                    <div class="stat-label">Fatalities</div>
                                  </div>
                                </div>
                                <div class="col-3" v-if="result.ground">
                                  <div class="stat-box bg-warning-light">
                                    <div class="stat-number text-warning">{{ result.ground }}</div>
                                    <div class="stat-label">Ground</div>
                                  </div>
                                </div>
                                <div class="col-3" v-if="getSurvivors(result) !== null">
                                  <div class="stat-box bg-success-light">
                                    <div class="stat-number text-success">{{ getSurvivors(result) }}</div>
                                    <div class="stat-label">Survivors</div>
                                  </div>
                                </div>
                              </div>
                            </div>
                            
                            <p class="card-text text-muted small mt-3" v-if="result.summary">
                              {{ truncateText(result.summary, 150) }}
                            </p>
                          </div>
                        </div>
                        <div class="col-md-4 text-end d-flex flex-column justify-content-between">
                          <div class="mb-3" v-if="result.registration || result.route">
                            <small class="text-muted d-block" v-if="result.registration">
                              Registration: <strong>{{ result.registration }}</strong>
                            </small>
                            <small class="text-muted d-block" v-if="result.route">
                              Route: <strong>{{ result.route }}</strong>
                            </small>
                          </div>
                          
                          <div class="action-buttons">
                            <button class="btn btn-outline-primary btn-sm mb-2 w-100" @click="viewDetails(result.id)">
                              <i class="bi bi-eye me-1"></i>View Details
                            </button>
                            <button class="btn btn-outline-secondary btn-sm w-100" @click="showOnMap(result)" :disabled="!result.latitude || !result.longitude">
                              <i class="bi bi-geo-alt me-1"></i>Show on Map
                            </button>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              
              <!-- Pagination -->
              <nav aria-label="Search results pagination" v-if="totalPages > 1">
                <ul class="pagination justify-content-center">
                  <li class="page-item" :class="{ disabled: currentPage === 1 }">
                    <button class="page-link" @click="currentPage = 1" :disabled="currentPage === 1">First</button>
                  </li>
                  <li class="page-item" :class="{ disabled: currentPage === 1 }">
                    <button class="page-link" @click="currentPage--" :disabled="currentPage === 1">Previous</button>
                  </li>
                  <li v-for="page in visiblePages" :key="page" class="page-item" :class="{ active: page === currentPage }">
                    <button class="page-link" @click="currentPage = page">{{ page }}</button>
                  </li>
                  <li class="page-item" :class="{ disabled: currentPage === totalPages }">
                    <button class="page-link" @click="currentPage++" :disabled="currentPage === totalPages">Next</button>
                  </li>
                  <li class="page-item" :class="{ disabled: currentPage === totalPages }">
                    <button class="page-link" @click="currentPage = totalPages" :disabled="currentPage === totalPages">Last</button>
                  </li>
                </ul>
              </nav>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import DualRangeSlider from '../components/DualRangeSlider.vue'
import { crashApi } from '../services/api.js'
import { FILTER_CONFIG, createFilterConfig } from '../constants/index.js'

const router = useRouter()

// Search filters with comprehensive options
const filters = ref({
  // Date filters
  dateFrom: '',
  dateTo: '',
  
  // Location and operator filters
  location: '',
  operator: '',
  flight: '',
  route: '',
  
  // Aircraft filters
  manufacturer: '',
  acModel: '',
  registration: '',
  cnLn: '',
  
  // Numeric range filters
  aboardRange: { start: FILTER_CONFIG.ABOARD_RANGE.DEFAULT_MIN, end: FILTER_CONFIG.ABOARD_RANGE.DEFAULT_MAX },
  passengersAboardRange: { start: FILTER_CONFIG.PASSENGERS_RANGE.DEFAULT_MIN, end: FILTER_CONFIG.PASSENGERS_RANGE.DEFAULT_MAX },
  crewAboardRange: { start: FILTER_CONFIG.CREW_RANGE.DEFAULT_MIN, end: FILTER_CONFIG.CREW_RANGE.DEFAULT_MAX },
  fatalitiesRange: { start: FILTER_CONFIG.FATALITIES_RANGE.DEFAULT_MIN, end: FILTER_CONFIG.FATALITIES_RANGE.DEFAULT_MAX },
  passengerFatalitiesRange: { start: FILTER_CONFIG.PASSENGERS_RANGE.DEFAULT_MIN, end: FILTER_CONFIG.PASSENGERS_RANGE.DEFAULT_MAX },
  crewFatalitiesRange: { start: FILTER_CONFIG.CREW_RANGE.DEFAULT_MIN, end: FILTER_CONFIG.CREW_RANGE.DEFAULT_MAX },
  groundRange: { start: FILTER_CONFIG.GROUND_RANGE.DEFAULT_MIN, end: FILTER_CONFIG.GROUND_RANGE.DEFAULT_MAX }
})

// Search state
const searchResults = ref([])
const loading = ref(false)
const error = ref('')
const hasSearched = ref(false)
const currentPage = ref(1)
const resultsPerPage = 20
const sortBy = ref('date-desc')

// Dropdown data
const operators = ref([])
const manufacturers = ref([])
const statistics = ref(null)

// Create dynamic filter config based on statistics, fallback to static config
const filterConfig = computed(() => {
  return statistics.value ? createFilterConfig(statistics.value) : FILTER_CONFIG
})

// Computed properties
const totalPages = computed(() => Math.ceil(searchResults.value.length / resultsPerPage))
const paginatedResults = computed(() => {
  const start = (currentPage.value - 1) * resultsPerPage
  const end = start + resultsPerPage
  return searchResults.value.slice(start, end)
})

const visiblePages = computed(() => {
  const pages = []
  const start = Math.max(1, currentPage.value - 2)
  const end = Math.min(totalPages.value, currentPage.value + 2)
  
  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  return pages
})

// Methods
const loadDropdownData = async () => {
  try {
    const [operatorData, manufacturerData, statisticsData] = await Promise.all([
      crashApi.getOperators(),
      crashApi.getManufacturers(),
      crashApi.getStatistics()
    ])
    operators.value = operatorData
    manufacturers.value = manufacturerData
    statistics.value = statisticsData
    
    // Update filters with dynamic values after statistics are loaded
    updateFiltersWithStatistics()
  } catch (err) {
    console.error('Failed to load dropdown data:', err)
  }
}

const updateFiltersWithStatistics = () => {
  if (!statistics.value) return
  
  const config = createFilterConfig(statistics.value)
  
  // Update filters to use dynamic min/max values
  filters.value = {
    ...filters.value,
    aboardRange: { start: config.ABOARD_RANGE.DEFAULT_MIN, end: config.ABOARD_RANGE.DEFAULT_MAX },
    passengersAboardRange: { start: config.PASSENGERS_RANGE.DEFAULT_MIN, end: config.PASSENGERS_RANGE.DEFAULT_MAX },
    crewAboardRange: { start: config.CREW_RANGE.DEFAULT_MIN, end: config.CREW_RANGE.DEFAULT_MAX },
    fatalitiesRange: { start: config.FATALITIES_RANGE.DEFAULT_MIN, end: config.FATALITIES_RANGE.DEFAULT_MAX },
    passengerFatalitiesRange: { start: config.PASSENGERS_RANGE.DEFAULT_MIN, end: config.PASSENGERS_RANGE.DEFAULT_MAX },
    crewFatalitiesRange: { start: config.CREW_RANGE.DEFAULT_MIN, end: config.CREW_RANGE.DEFAULT_MAX },
    groundRange: { start: config.GROUND_RANGE.DEFAULT_MIN, end: config.GROUND_RANGE.DEFAULT_MAX }
  }
}

const performSearch = async () => {
  loading.value = true
  error.value = ''
  hasSearched.value = true
  currentPage.value = 1
  
  try {
    // Build search parameters
    const searchParams = {
      limit: 1000, // Get more results for client-side pagination
    }
    
    // Add date filters
    if (filters.value.dateFrom) {
      searchParams.dateFrom = filters.value.dateFrom
    }
    if (filters.value.dateTo) {
      searchParams.dateTo = filters.value.dateTo
    }
    
    // Add text filters
    if (filters.value.location) {
      searchParams.location = filters.value.location
    }
    if (filters.value.operator) {
      searchParams.operator_ = filters.value.operator
    }
    if (filters.value.flight) {
      searchParams.flight = filters.value.flight
    }
    if (filters.value.route) {
      searchParams.route = filters.value.route
    }
    if (filters.value.manufacturer) {
      searchParams.manufacturer = filters.value.manufacturer
    }
    if (filters.value.acModel) {
      searchParams.acModel = filters.value.acModel
    }
    if (filters.value.registration) {
      searchParams.registration = filters.value.registration
    }
    if (filters.value.cnLn) {
      searchParams.cn_ln = filters.value.cnLn
    }
    
    // Add range filters
    const config = filterConfig.value
    
    if (filters.value.aboardRange.start > config.ABOARD_RANGE.DEFAULT_MIN) {
      searchParams.minAboard = filters.value.aboardRange.start
    }
    if (filters.value.aboardRange.end < config.ABOARD_RANGE.DEFAULT_MAX) {
      searchParams.maxAboard = filters.value.aboardRange.end
    }
    
    if (filters.value.passengersAboardRange.start > config.PASSENGERS_RANGE.DEFAULT_MIN) {
      searchParams.minAboardPassengers = filters.value.passengersAboardRange.start
    }
    if (filters.value.passengersAboardRange.end < config.PASSENGERS_RANGE.DEFAULT_MAX) {
      searchParams.maxAboardPassengers = filters.value.passengersAboardRange.end
    }
    
    if (filters.value.crewAboardRange.start > config.CREW_RANGE.DEFAULT_MIN) {
      searchParams.minAboardCrew = filters.value.crewAboardRange.start
    }
    if (filters.value.crewAboardRange.end < config.CREW_RANGE.DEFAULT_MAX) {
      searchParams.maxAboardCrew = filters.value.crewAboardRange.end
    }
    
    if (filters.value.fatalitiesRange.start > config.FATALITIES_RANGE.DEFAULT_MIN) {
      searchParams.minFatalities = filters.value.fatalitiesRange.start
    }
    if (filters.value.fatalitiesRange.end < config.FATALITIES_RANGE.DEFAULT_MAX) {
      searchParams.maxFatalities = filters.value.fatalitiesRange.end
    }
    
    if (filters.value.passengerFatalitiesRange.start > config.PASSENGERS_RANGE.DEFAULT_MIN) {
      searchParams.minFatalitiesPassengers = filters.value.passengerFatalitiesRange.start
    }
    if (filters.value.passengerFatalitiesRange.end < config.PASSENGERS_RANGE.DEFAULT_MAX) {
      searchParams.maxFatalitiesPassengers = filters.value.passengerFatalitiesRange.end
    }
    
    if (filters.value.crewFatalitiesRange.start > config.CREW_RANGE.DEFAULT_MIN) {
      searchParams.minFatalitiesCrew = filters.value.crewFatalitiesRange.start
    }
    if (filters.value.crewFatalitiesRange.end < config.CREW_RANGE.DEFAULT_MAX) {
      searchParams.maxFatalitiesCrew = filters.value.crewFatalitiesRange.end
    }
    
    if (filters.value.groundRange.start > config.GROUND_RANGE.DEFAULT_MIN) {
      searchParams.minGround = filters.value.groundRange.start
    }
    if (filters.value.groundRange.end < config.GROUND_RANGE.DEFAULT_MAX) {
      searchParams.maxGround = filters.value.groundRange.end
    }
    
    const results = await crashApi.searchCrashes(searchParams)
    searchResults.value = results
    applySorting()
  } catch (err) {
    error.value = err.message || 'Failed to perform search'
    searchResults.value = []
  } finally {
    loading.value = false
  }
}

const clearFilters = () => {
  const config = filterConfig.value
  filters.value = {
    dateFrom: '',
    dateTo: '',
    location: '',
    operator: '',
    flight: '',
    route: '',
    manufacturer: '',
    acModel: '',
    registration: '',
    cnLn: '',
    aboardRange: { start: config.ABOARD_RANGE.DEFAULT_MIN, end: config.ABOARD_RANGE.DEFAULT_MAX },
    passengersAboardRange: { start: config.PASSENGERS_RANGE.DEFAULT_MIN, end: config.PASSENGERS_RANGE.DEFAULT_MAX },
    crewAboardRange: { start: config.CREW_RANGE.DEFAULT_MIN, end: config.CREW_RANGE.DEFAULT_MAX },
    fatalitiesRange: { start: config.FATALITIES_RANGE.DEFAULT_MIN, end: config.FATALITIES_RANGE.DEFAULT_MAX },
    passengerFatalitiesRange: { start: config.PASSENGERS_RANGE.DEFAULT_MIN, end: config.PASSENGERS_RANGE.DEFAULT_MAX },
    crewFatalitiesRange: { start: config.CREW_RANGE.DEFAULT_MIN, end: config.CREW_RANGE.DEFAULT_MAX },
    groundRange: { start: config.GROUND_RANGE.DEFAULT_MIN, end: config.GROUND_RANGE.DEFAULT_MAX }
  }
  searchResults.value = []
  hasSearched.value = false
  currentPage.value = 1
  error.value = ''
}

const sortResults = (sortType) => {
  sortBy.value = sortType
  applySorting()
}

const applySorting = () => {
  const results = [...searchResults.value]
  
  switch (sortBy.value) {
    case 'date-desc':
      results.sort((a, b) => new Date(b.date || 0) - new Date(a.date || 0))
      break
    case 'date-asc':
      results.sort((a, b) => new Date(a.date || 0) - new Date(b.date || 0))
      break
    case 'fatalities-desc':
      results.sort((a, b) => (b.fatalities || 0) - (a.fatalities || 0))
      break
    case 'fatalities-asc':
      results.sort((a, b) => (a.fatalities || 0) - (b.fatalities || 0))
      break
  }
  
  searchResults.value = results
}

const viewDetails = (id) => {
  // Navigate to detail view - implement this route in your router
  router.push(`/crash/${id}`)
}

const showOnMap = (result) => {
  if (result.latitude && result.longitude) {
    router.push({
      path: '/map',
      query: {
        lat: result.latitude,
        lng: result.longitude,
        zoom: 10,
        highlight: result.id
      }
    })
  }
}

const getSurvivors = (result) => {
  if (result.aboard && result.fatalities !== undefined) {
    return Math.max(0, result.aboard - result.fatalities)
  }
  return null
}

const formatDate = (dateString) => {
  if (!dateString) return 'Unknown Date'
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

const formatTime = (time) => {
  if (!time) return ''
  // Assuming time is in HHMM format
  const timeStr = time.toString().padStart(4, '0')
  return `${timeStr.slice(0, 2)}:${timeStr.slice(2)}`
}

const truncateText = (text, maxLength) => {
  if (!text) return ''
  if (text.length <= maxLength) return text
  return text.substring(0, maxLength) + '...'
}

// Watch for URL parameters (if coming from other views)
watch(() => router.currentRoute.value.query, (newQuery) => {
  if (newQuery.search) {
    filters.value.location = newQuery.search
    performSearch()
  }
}, { immediate: true })

// Load dropdown data on mount
onMounted(() => {
  loadDropdownData()
})
</script>

<style scoped>
.border-start {
  border-left-width: 4px !important;
}

.card-title {
  color: #dc3545;
  font-weight: 600;
}

.pagination .page-link {
  border-color: #dc3545;
  color: #dc3545;
}

.pagination .page-item.active .page-link {
  background-color: #dc3545;
  border-color: #dc3545;
}

.pagination .page-link:hover {
  background-color: #f8d7da;
  border-color: #dc3545;
  color: #dc3545;
}

.filter-section {
  margin-bottom: 2rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid #e9ecef;
}

.filter-section:last-child {
  border-bottom: none;
  margin-bottom: 0;
}

.filter-section-title {
  color: #495057;
  font-size: 0.9rem;
  font-weight: 600;
  margin-bottom: 1rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.crash-details {
  font-size: 0.9rem;
}

.stat-box {
  padding: 0.5rem;
  border-radius: 0.375rem;
  text-align: center;
}

.stat-number {
  font-size: 1.25rem;
  font-weight: 700;
  line-height: 1;
}

.stat-label {
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  opacity: 0.8;
  margin-top: 0.25rem;
}

.bg-danger-light {
  background-color: rgba(220, 53, 69, 0.1);
}

.bg-warning-light {
  background-color: rgba(255, 193, 7, 0.1);
}

.bg-success-light {
  background-color: rgba(25, 135, 84, 0.1);
}

.casualties-info {
  background: #f8f9fa;
  border-radius: 0.375rem;
  padding: 1rem;
}

.action-buttons .btn {
  font-size: 0.875rem;
}

.sticky-top {
  position: sticky;
  z-index: 100;
}

.card-body {
  max-height: 80vh;
  overflow-y: auto;
}

/* Custom scrollbar for filter card */
.card-body::-webkit-scrollbar {
  width: 6px;
}

.card-body::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 3px;
}

.card-body::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
}

.card-body::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

/* Form controls styling */
.form-control-sm:focus,
.form-select-sm:focus {
  border-color: #dc3545;
  box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25);
}

/* Bootstrap Icons styling */
.bi {
  color: #6c757d;
}

.card-title .bi {
  color: #dc3545;
}

/* Responsive adjustments */
@media (max-width: 991.98px) {
  .sticky-top {
    position: relative;
  }
  
  .card-body {
    max-height: none;
  }
}

/* Animation for loading */
@keyframes pulse {
  0% { opacity: 1; }
  50% { opacity: 0.5; }
  100% { opacity: 1; }
}

.loading {
  animation: pulse 1.5s ease-in-out infinite;
}

/* Hover effects */
.card:hover {
  transform: translateY(-2px);
  transition: transform 0.2s ease-in-out;
  box-shadow: 0 4px 8px rgba(0,0,0,0.1);
}

.btn:hover {
  transform: translateY(-1px);
  transition: transform 0.1s ease-in-out;
}

/* Custom badges */
.badge {
  font-size: 0.75rem;
  padding: 0.375rem 0.75rem;
}

/* Typography improvements */
.small {
  font-size: 0.875rem;
}

.card-subtitle {
  font-size: 0.9rem;
}

/* Enhanced form styling */
.form-label {
  font-weight: 500;
  margin-bottom: 0.5rem;
}

.form-control, .form-select {
  border-radius: 0.375rem;
  border-color: #ced4da;
  transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
}

/* Alert styling */
.alert {
  border-radius: 0.5rem;
  border: none;
}
</style>
