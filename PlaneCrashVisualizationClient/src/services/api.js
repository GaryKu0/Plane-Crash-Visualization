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
  }
}

export { ApiError }
