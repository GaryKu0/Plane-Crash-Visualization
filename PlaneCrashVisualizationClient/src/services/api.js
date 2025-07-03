import axios from 'axios'
import { API_CONFIG } from '../constants/index.js'

const { BASE_URL, ENDPOINTS } = API_CONFIG

class ApiError extends Error {
  constructor(message, status) {
    super(message)
    this.status = status
    this.name = 'ApiError'
  }
}

const handleApiError = (error, context) => {
  const message = error.response?.data?.message || error.message || 'An unknown error occurred'
  const status = error.response?.status || 500
  
  console.error(`Error ${context}:`, {
    message,
    status,
    url: error.config?.url
  })
  
  throw new ApiError(`Failed to ${context}: ${message}`, status)
}

export const crashApi = {
  async getOperators() {
    try {
      const response = await axios.get(`${BASE_URL}${ENDPOINTS.OPERATORS}`)
      return response.data.sort()
    } catch (error) {
      handleApiError(error, 'fetch operators')
    }
  },

  async getManufacturers() {
    try {
      const response = await axios.get(`${BASE_URL}${ENDPOINTS.MANUFACTURERS}`)
      return response.data.sort()
    } catch (error) {
      handleApiError(error, 'fetch manufacturers')
    }
  },

  async getMapData(params) {
    try {
      const response = await axios.get(`${BASE_URL}${ENDPOINTS.MAP_DATA}`, { params })
      return response.data
    } catch (error) {
      handleApiError(error, 'fetch crash data')
    }
  },

  async getStatistics() {
    try {
      const response = await axios.get(`${BASE_URL}${ENDPOINTS.STATISTICS}`)
      return response.data
    } catch (error) {
      handleApiError(error, 'fetch statistics')
    }
  },

  async searchCrashes(searchParams) {
    try {
      // Convert search parameters to API query parameters
      const params = {}
      
      if (searchParams.dateFrom) {
        params.dateFrom = searchParams.dateFrom
      }
      
      if (searchParams.dateTo) {
        params.dateTo = searchParams.dateTo
      }
      
      if (searchParams.location) {
        params.location = searchParams.location
      }
      
      if (searchParams.operator_) {
        params.operator_ = searchParams.operator_
      }
      
      if (searchParams.flight) {
        params.flight = searchParams.flight
      }
      
      if (searchParams.route) {
        params.route = searchParams.route
      }
      
      if (searchParams.registration) {
        params.registration = searchParams.registration
      }
      
      if (searchParams.cn_ln) {
        params.cn_ln = searchParams.cn_ln
      }
      
      if (searchParams.manufacturer) {
        params.manufacturer = searchParams.manufacturer
      }
      
      if (searchParams.acModel) {
        params.acModel = searchParams.acModel
      }
      
      // Numeric range parameters
      if (searchParams.minAboard !== undefined) {
        params.minAboard = searchParams.minAboard
      }
      if (searchParams.maxAboard !== undefined) {
        params.maxAboard = searchParams.maxAboard
      }
      if (searchParams.minFatalities !== undefined) {
        params.minFatalities = searchParams.minFatalities
      }
      if (searchParams.maxFatalities !== undefined) {
        params.maxFatalities = searchParams.maxFatalities
      }
      if (searchParams.minGround !== undefined) {
        params.minGround = searchParams.minGround
      }
      if (searchParams.maxGround !== undefined) {
        params.maxGround = searchParams.maxGround
      }
      if (searchParams.minAboardCrew !== undefined) {
        params.minAboardCrew = searchParams.minAboardCrew
      }
      if (searchParams.maxAboardCrew !== undefined) {
        params.maxAboardCrew = searchParams.maxAboardCrew
      }
      if (searchParams.minAboardPassengers !== undefined) {
        params.minAboardPassengers = searchParams.minAboardPassengers
      }
      if (searchParams.maxAboardPassengers !== undefined) {
        params.maxAboardPassengers = searchParams.maxAboardPassengers
      }
      if (searchParams.minFatalitiesCrew !== undefined) {
        params.minFatalitiesCrew = searchParams.minFatalitiesCrew
      }
      if (searchParams.maxFatalitiesCrew !== undefined) {
        params.maxFatalitiesCrew = searchParams.maxFatalitiesCrew
      }
      if (searchParams.minFatalitiesPassengers !== undefined) {
        params.minFatalitiesPassengers = searchParams.minFatalitiesPassengers
      }
      if (searchParams.maxFatalitiesPassengers !== undefined) {
        params.maxFatalitiesPassengers = searchParams.maxFatalitiesPassengers
      }
      
      if (searchParams.limit) {
        params.limit = searchParams.limit
      }

      const response = await axios.get(`${BASE_URL}${ENDPOINTS.SEARCH}`, { params })
      return response.data
    } catch (error) {
      handleApiError(error, 'search crashes')
    }
  },

  async getCrashById(id) {
    try {
      const response = await axios.get(`${BASE_URL}/crashes/${id}`)
      return response.data
    } catch (error) {
      handleApiError(error, 'fetch crash details')
    }
  },

  async getSummary() {
    try {
      const response = await axios.get(`${BASE_URL}/crashes/summary`)
      return response.data
    } catch (error) {
      handleApiError(error, 'fetch summary data')
    }
  }
}

export { ApiError }
