// API Configuration
export const API_CONFIG = {
  BASE_URL: 'http://localhost:5021/api',
  ENDPOINTS: {
    OPERATORS: '/crashes/operators',
    MANUFACTURERS: '/crashes/manufacturers',
    MAP_DATA: '/crashes/map-data'
  }
}

// Filter Configuration
export const FILTER_CONFIG = {
  YEAR_RANGE: {
    MIN: 1920,
    MAX: 2025,
    DEFAULT_START: 1920,
    DEFAULT_END: 2025
  },
  FATALITIES_RANGE: {
    MIN: 0,
    MAX: 1000,
    DEFAULT_MIN: 0,
    DEFAULT_MAX: 1000
  }
}

// UI Configuration
export const UI_CONFIG = {
  DEBOUNCE_DELAY: 500,
  LOADING_MESSAGES: {
    FETCHING_DATA: 'Loading crash data...',
    FETCHING_OPERATORS: 'Loading operators...',
    FETCHING_MANUFACTURERS: 'Loading manufacturers...'
  }
}

// Map Configuration - re-exported from mapUtils for convenience
export { MAP_CONFIG, CLUSTER_CONFIG, FATALITY_THRESHOLDS, MARKER_COLORS, MARKER_SIZES } from '../utils/mapUtils.js'
