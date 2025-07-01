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
      <div class="col-md-4">
        <div class="card">
          <div class="card-header">
            <h5 class="card-title mb-0">Search Filters</h5>
          </div>
          <div class="card-body">
            <form @submit.prevent="performSearch">
              <div class="mb-3">
                <label for="dateFrom" class="form-label">Date From</label>
                <input type="date" class="form-control" id="dateFrom" v-model="filters.dateFrom">
              </div>
              
              <div class="mb-3">
                <label for="dateTo" class="form-label">Date To</label>
                <input type="date" class="form-control" id="dateTo" v-model="filters.dateTo">
              </div>
              
              <div class="mb-3">
                <label for="location" class="form-label">Location</label>
                <input type="text" class="form-control" id="location" v-model="filters.location" placeholder="Country, City, or Region">
              </div>
              
              <div class="mb-3">
                <label for="airline" class="form-label">Airline</label>
                <input type="text" class="form-control" id="airline" v-model="filters.airline" placeholder="Airline name">
              </div>
              
              <div class="mb-3">
                <label for="aircraft" class="form-label">Aircraft Type</label>
                <input type="text" class="form-control" id="aircraft" v-model="filters.aircraft" placeholder="e.g., Boeing 737">
              </div>
              
              <div class="mb-3">
                <label for="minFatalities" class="form-label">Minimum Fatalities</label>
                <input type="number" class="form-control" id="minFatalities" v-model="filters.minFatalities" min="0">
              </div>
              
              <button type="submit" class="btn btn-primary w-100">
                <i class="bi bi-search me-2"></i>Search
              </button>
              
              <button type="button" class="btn btn-outline-secondary w-100 mt-2" @click="clearFilters">
                <i class="bi bi-arrow-clockwise me-2"></i>Clear Filters
              </button>
            </form>
          </div>
        </div>
      </div>
      
      <div class="col-md-8">
        <div class="card">
          <div class="card-header d-flex justify-content-between align-items-center">
            <h5 class="card-title mb-0">Search Results</h5>
            <span class="badge bg-secondary">{{ searchResults.length }} results</span>
          </div>
          <div class="card-body">
            <div v-if="loading" class="text-center py-5">
              <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading...</span>
              </div>
              <p class="mt-2">Searching...</p>
            </div>
            
            <div v-else-if="searchResults.length === 0" class="text-center py-5 text-muted">
              <i class="bi bi-search" style="font-size: 3rem;"></i>
              <p class="mt-3">{{ hasSearched ? 'No results found. Try adjusting your search criteria.' : 'Use the filters to search for plane crash data.' }}</p>
            </div>
            
            <div v-else>
              <div class="row">
                <div v-for="result in paginatedResults" :key="result.id" class="col-12 mb-3">
                  <div class="card border-start border-4 border-danger">
                    <div class="card-body">
                      <div class="row">
                        <div class="col-md-8">
                          <h6 class="card-title">{{ result.date }} - {{ result.location }}</h6>
                          <p class="card-text">
                            <strong>Airline:</strong> {{ result.airline || 'Unknown' }}<br>
                            <strong>Aircraft:</strong> {{ result.aircraft || 'Unknown' }}<br>
                            <strong>Fatalities:</strong> {{ result.fatalities || 'Unknown' }}
                          </p>
                          <p class="card-text text-muted small">{{ result.summary }}</p>
                        </div>
                        <div class="col-md-4 text-end">
                          <button class="btn btn-outline-primary btn-sm" @click="viewDetails(result.id)">
                            <i class="bi bi-eye me-1"></i>View Details
                          </button>
                          <button class="btn btn-outline-secondary btn-sm ms-2" @click="showOnMap(result)">
                            <i class="bi bi-geo-alt me-1"></i>Show on Map
                          </button>
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
import { ref, computed, watch } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// Search filters
const filters = ref({
  dateFrom: '',
  dateTo: '',
  location: '',
  airline: '',
  aircraft: '',
  minFatalities: ''
})

// Search state
const searchResults = ref([])
const loading = ref(false)
const hasSearched = ref(false)
const currentPage = ref(1)
const resultsPerPage = 10

// Mock data for demonstration
const mockResults = [
  {
    id: 1,
    date: '2023-01-15',
    location: 'Nepal, Pokhara',
    airline: 'Yeti Airlines',
    aircraft: 'ATR 72-500',
    fatalities: 72,
    summary: 'ATR 72 crashed during approach to Pokhara Airport in Nepal.'
  },
  {
    id: 2,
    date: '2022-03-21',
    location: 'China, Guangxi',
    airline: 'China Eastern Airlines',
    aircraft: 'Boeing 737-800',
    fatalities: 132,
    summary: 'Boeing 737-800 crashed in mountainous terrain during cruise flight.'
  },
  {
    id: 3,
    date: '2020-01-08',
    location: 'Iran, Tehran',
    airline: 'Ukraine International Airlines',
    aircraft: 'Boeing 737-800',
    fatalities: 176,
    summary: 'Boeing 737-800 shot down shortly after takeoff from Tehran airport.'
  }
]

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
const performSearch = async () => {
  loading.value = true
  hasSearched.value = true
  currentPage.value = 1
  
  // Simulate API call
  await new Promise(resolve => setTimeout(resolve, 1000))
  
  // Filter mock results based on criteria
  let results = [...mockResults]
  
  if (filters.value.location) {
    results = results.filter(r => r.location.toLowerCase().includes(filters.value.location.toLowerCase()))
  }
  
  if (filters.value.airline) {
    results = results.filter(r => r.airline.toLowerCase().includes(filters.value.airline.toLowerCase()))
  }
  
  if (filters.value.aircraft) {
    results = results.filter(r => r.aircraft.toLowerCase().includes(filters.value.aircraft.toLowerCase()))
  }
  
  if (filters.value.minFatalities) {
    results = results.filter(r => r.fatalities >= parseInt(filters.value.minFatalities))
  }
  
  searchResults.value = results
  loading.value = false
}

const clearFilters = () => {
  filters.value = {
    dateFrom: '',
    dateTo: '',
    location: '',
    airline: '',
    aircraft: '',
    minFatalities: ''
  }
  searchResults.value = []
  hasSearched.value = false
  currentPage.value = 1
}

const viewDetails = (id) => {
  // Navigate to detail view (would need to be implemented)
  console.log('View details for crash ID:', id)
}

const showOnMap = (result) => {
  // Navigate to map with specific location
  router.push({
    path: '/map',
    query: {
      lat: result.lat || 28.2096, // Default coordinates for demonstration
      lng: result.lng || 83.9856,
      zoom: 10
    }
  })
}

// Watch for URL parameters (if coming from other views)
watch(() => router.currentRoute.value.query, (newQuery) => {
  if (newQuery.search) {
    filters.value.location = newQuery.search
    performSearch()
  }
}, { immediate: true })
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
</style>
