<template>
  <div class="container py-4">
    <div class="row">
      <div class="col-12">
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb">
            <li class="breadcrumb-item">
              <router-link to="/" class="text-decoration-none">Home</router-link>
            </li>
            <li class="breadcrumb-item">
              <router-link to="/search" class="text-decoration-none">Search</router-link>
            </li>
            <li class="breadcrumb-item active" aria-current="page">Crash Details</li>
          </ol>
        </nav>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
      <p class="mt-3 text-muted">Loading crash details...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="alert alert-danger" role="alert">
      <i class="bi bi-exclamation-triangle me-2"></i>
      <strong>Error:</strong> {{ error }}
      <div class="mt-3">
        <router-link to="/search" class="btn btn-outline-primary">
          <i class="bi bi-arrow-left me-2"></i>Back to Search
        </router-link>
      </div>
    </div>

    <!-- Crash Details -->
    <div v-else-if="crash" class="row">
      <div class="col-12">
        <div class="d-flex justify-content-between align-items-center mb-4">
          <h1 class="h2 mb-0">Crash Details</h1>
          <div class="d-flex gap-2">
            <button 
              class="btn btn-outline-primary" 
              @click="showOnMap"
              :disabled="!(crash.latitude || crash.Latitude) || !(crash.longitude || crash.Longitude)"
            >
              <i class="bi bi-geo-alt me-2"></i>Show on Map
            </button>
            <router-link to="/search" class="btn btn-outline-secondary">
              <i class="bi bi-arrow-left me-2"></i>Back to Search
            </router-link>
          </div>
        </div>
      </div>
    </div>

    <div v-if="crash" class="row">
      <!-- Main Information Card -->
      <div class="col-lg-8 mb-4">
        <div class="card border-start border-4 border-red">
          <div class="card-header">
            <h5 class="card-title mb-0 text-red">
              <i class="bi bi-calendar3 me-2"></i>
              {{ formatDate(crash.date || crash.Date) }}
              <span v-if="crash.time || crash.Time" class="ms-2 badge bg-light text-dark">{{ formatTime(crash.time || crash.Time) }}</span>
            </h5>
          </div>
          <div class="card-body">
            <div class="row g-4">
              <!-- Basic Information -->
              <div class="col-md-6">
                <h6 class="text-muted text-uppercase fw-bold mb-3">
                  <i class="bi bi-info-circle me-2"></i>Basic Information
                </h6>
                <div class="info-group">
                  <div class="info-item" v-if="crash.location || crash.Location">
                    <label>Location:</label>
                    <span>{{ crash.location || crash.Location }}</span>
                  </div>
                  <div class="info-item" v-if="(crash.latitude || crash.Latitude) && (crash.longitude || crash.Longitude)">
                    <label>Coordinates:</label>
                    <span>{{ (crash.latitude || crash.Latitude).toFixed(6) }}, {{ (crash.longitude || crash.Longitude).toFixed(6) }}</span>
                  </div>
                  <div class="info-item" v-if="crash.operator || crash.Operator">
                    <label>Operator:</label>
                    <span>{{ crash.operator || crash.Operator }}</span>
                  </div>
                  <div class="info-item" v-if="crash.flight || crash.Flight">
                    <label>Flight:</label>
                    <span>{{ crash.flight || crash.Flight }}</span>
                  </div>
                  <div class="info-item" v-if="crash.route || crash.Route">
                    <label>Route:</label>
                    <span>{{ crash.route || crash.Route }}</span>
                  </div>
                </div>
              </div>

              <!-- Aircraft Information -->
              <div class="col-md-6">
                <h6 class="text-muted text-uppercase fw-bold mb-3">
                  <i class="bi bi-airplane me-2"></i>Aircraft Information
                </h6>
                <div class="info-group">
                  <div class="info-item" v-if="crash.manufacturer || crash.Manufacturer">
                    <label>Manufacturer:</label>
                    <span>{{ crash.manufacturer || crash.Manufacturer }}</span>
                  </div>
                  <div class="info-item" v-if="crash.aC_Model || crash.AC_Model || crash.acModel || crash.ac_Model">
                    <label>Model:</label>
                    <span>{{ crash.aC_Model || crash.AC_Model || crash.acModel || crash.ac_Model }}</span>
                  </div>
                  <div class="info-item" v-if="crash.registration || crash.Registration">
                    <label>Registration:</label>
                    <span>{{ crash.registration || crash.Registration }}</span>
                  </div>
                  <div class="info-item" v-if="crash.cn_ln || crash.Cn_ln">
                    <label>Construction Number:</label>
                    <span>{{ crash.cn_ln || crash.Cn_ln }}</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Summary -->
            <div v-if="crash.summary || crash.Summary" class="mt-4">
              <h6 class="text-muted text-uppercase fw-bold mb-3">
                <i class="bi bi-file-text me-2"></i>Summary
              </h6>
              <div class="summary-text p-3 bg-light rounded">
                {{ crash.summary || crash.Summary }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Casualties Statistics -->
      <div class="col-lg-4 mb-4">
        <div class="card h-100">
          <div class="card-header">
            <h5 class="card-title mb-0 text-red">
              <i class="bi bi-people me-2"></i>Casualties & Statistics
            </h5>
          </div>
          <div class="card-body">
            <!-- People Aboard -->
            <div class="stat-section mb-4">
              <h6 class="text-muted text-uppercase fw-bold mb-3">People Aboard</h6>
              <div class="stat-grid">
                <div class="stat-box bg-primary-light" v-if="crash.aboard || crash.Aboard">
                  <div class="stat-number text-primary">{{ crash.aboard || crash.Aboard }}</div>
                  <div class="stat-label">Total Aboard</div>
                </div>
                <div class="stat-box bg-info-light" v-if="crash.aboardPassengers || crash.AboardPassengers">
                  <div class="stat-number text-info">{{ crash.aboardPassengers || crash.AboardPassengers }}</div>
                  <div class="stat-label">Passengers</div>
                </div>
                <div class="stat-box bg-secondary-light" v-if="crash.aboardCrew || crash.AboardCrew">
                  <div class="stat-number text-secondary">{{ crash.aboardCrew || crash.AboardCrew }}</div>
                  <div class="stat-label">Crew</div>
                </div>
              </div>
            </div>

            <!-- Fatalities -->
            <div class="stat-section mb-4">
              <h6 class="text-muted text-uppercase fw-bold mb-3">Fatalities</h6>
              <div class="stat-grid">
                <div class="stat-box bg-danger-light" v-if="crash.fatalities !== null || crash.Fatalities !== null">
                  <div class="stat-number text-danger">{{ crash.fatalities || crash.Fatalities || 0 }}</div>
                  <div class="stat-label">Total Fatalities</div>
                </div>
                <div class="stat-box bg-danger-light" v-if="crash.fatalitiesPassengers !== null || crash.FatalitiesPassengers !== null">
                  <div class="stat-number text-danger">{{ crash.fatalitiesPassengers || crash.FatalitiesPassengers || 0 }}</div>
                  <div class="stat-label">Passenger Fatalities</div>
                </div>
                <div class="stat-box bg-danger-light" v-if="crash.fatalitiesCrew !== null || crash.FatalitiesCrew !== null">
                  <div class="stat-number text-danger">{{ crash.fatalitiesCrew || crash.FatalitiesCrew || 0 }}</div>
                  <div class="stat-label">Crew Fatalities</div>
                </div>
                <div class="stat-box bg-warning-light" v-if="crash.ground !== null || crash.Ground !== null">
                  <div class="stat-number text-warning">{{ crash.ground || crash.Ground || 0 }}</div>
                  <div class="stat-label">Ground Fatalities</div>
                </div>
              </div>
            </div>

            <!-- Survivors -->
            <div class="stat-section" v-if="getSurvivors() !== null">
              <h6 class="text-muted text-uppercase fw-bold mb-3">Survivors</h6>
              <div class="stat-grid">
                <div class="stat-box bg-success-light">
                  <div class="stat-number text-success">{{ getSurvivors() }}</div>
                  <div class="stat-label">Total Survivors</div>
                </div>
                <div class="stat-box bg-success-light" v-if="getPassengerSurvivors() !== null">
                  <div class="stat-number text-success">{{ getPassengerSurvivors() }}</div>
                  <div class="stat-label">Passenger Survivors</div>
                </div>
                <div class="stat-box bg-success-light" v-if="getCrewSurvivors() !== null">
                  <div class="stat-number text-success">{{ getCrewSurvivors() }}</div>
                  <div class="stat-label">Crew Survivors</div>
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
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { crashApi } from '../services/api.js'

const route = useRoute()
const router = useRouter()

const crash = ref(null)
const loading = ref(true)
const error = ref('')

const loadCrashDetails = async () => {
  try {
    loading.value = true
    error.value = ''
    
    const crashId = parseInt(route.params.id)
    if (!crashId) {
      throw new Error('Invalid crash ID')
    }
    
    const crashData = await crashApi.getCrashById(crashId)
    crash.value = crashData
  } catch (err) {
    error.value = err.message || 'Failed to load crash details'
    console.error('Error loading crash details:', err)
  } finally {
    loading.value = false
  }
}

const showOnMap = () => {
  if (crash.value && (crash.value.latitude || crash.value.Latitude) && (crash.value.longitude || crash.value.Longitude)) {
    router.push({
      path: '/map',
      query: {
        lat: crash.value.latitude || crash.value.Latitude,
        lng: crash.value.longitude || crash.value.Longitude,
        zoom: 12,
        highlight: crash.value.id || crash.value.Id,
        openPopup: 'true'
      }
    })
  }
}

const getSurvivors = () => {
  const aboard = crash.value?.aboard || crash.value?.Aboard
  const fatalities = crash.value?.fatalities || crash.value?.Fatalities
  if (aboard !== null && fatalities !== null) {
    return Math.max(0, aboard - fatalities)
  }
  return null
}

const getPassengerSurvivors = () => {
  const aboardPassengers = crash.value?.aboardPassengers || crash.value?.AboardPassengers
  const fatalitiesPassengers = crash.value?.fatalitiesPassengers || crash.value?.FatalitiesPassengers
  if (aboardPassengers !== null && fatalitiesPassengers !== null) {
    return Math.max(0, aboardPassengers - fatalitiesPassengers)
  }
  return null
}

const getCrewSurvivors = () => {
  const aboardCrew = crash.value?.aboardCrew || crash.value?.AboardCrew
  const fatalitiesCrew = crash.value?.fatalitiesCrew || crash.value?.FatalitiesCrew
  if (aboardCrew !== null && fatalitiesCrew !== null) {
    return Math.max(0, aboardCrew - fatalitiesCrew)
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
  const timeStr = time.toString().padStart(4, '0')
  return `${timeStr.slice(0, 2)}:${timeStr.slice(2)}`
}

onMounted(() => {
  loadCrashDetails()
})
</script>

<style scoped>
.border-start {
  border-left-width: 4px !important;
}

.info-group {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.info-item label {
  font-size: 0.875rem;
  font-weight: 600;
  color: #6c757d;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.info-item span {
  font-size: 1rem;
  color: #212529;
  word-break: break-word;
}

.stat-section {
  border-bottom: 1px solid #e9ecef;
  padding-bottom: 1rem;
}

.stat-section:last-child {
  border-bottom: none;
  padding-bottom: 0;
}

.stat-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 0.75rem;
}

.stat-box {
  padding: 1rem;
  border-radius: 0.5rem;
  text-align: center;
  border: 1px solid rgba(0, 0, 0, 0.1);
}

.stat-number {
  font-size: 1.5rem;
  font-weight: 700;
  line-height: 1;
  margin-bottom: 0.25rem;
}

.stat-label {
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  opacity: 0.8;
}

.summary-text {
  line-height: 1.6;
  font-size: 0.95rem;
}

/* Background colors for stat boxes */
.bg-primary-light {
  background-color: rgba(13, 110, 253, 0.1);
}

.bg-info-light {
  background-color: rgba(13, 202, 240, 0.1);
}

.bg-secondary-light {
  background-color: rgba(108, 117, 125, 0.1);
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

/* Card styling */
.border-red {
  border-color: #dc3545 !important;
}

.text-red {
  color: #dc3545 !important;
}

.card-title {
  font-weight: 600;
}

.card-title .bi {
  color: inherit;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .stat-grid {
    grid-template-columns: 1fr;
  }
  
  .d-flex.gap-2 {
    flex-direction: column;
    gap: 0.5rem !important;
  }
  
  .d-flex.gap-2 .btn {
    width: 100%;
  }
}

/* Hover effects */
.btn:hover {
  transform: translateY(-1px);
  transition: transform 0.1s ease-in-out;
}

.card:hover {
  box-shadow: 0 4px 8px rgba(0,0,0,0.1);
  transition: box-shadow 0.2s ease-in-out;
}

/* Bootstrap Icons */
.bi {
  color: #6c757d;
}

h6 .bi {
  color: #6c757d;
}
</style>
