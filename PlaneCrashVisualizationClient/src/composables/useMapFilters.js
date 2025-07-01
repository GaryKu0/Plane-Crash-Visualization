import { ref } from 'vue'
import { FILTER_CONFIG } from '../constants/index.js'

export function useMapFilters() {
  const filters = ref({
    yearStart: FILTER_CONFIG.YEAR_RANGE.DEFAULT_START,
    yearEnd: FILTER_CONFIG.YEAR_RANGE.DEFAULT_END,
    minFatalities: FILTER_CONFIG.FATALITIES_RANGE.DEFAULT_MIN,
    maxFatalities: FILTER_CONFIG.FATALITIES_RANGE.DEFAULT_MAX,
    operator: '',
    manufacturer: ''
  })

  const validateYearRange = () => {
    const startYear = parseInt(filters.value.yearStart)
    const endYear = parseInt(filters.value.yearEnd)
    
    if (startYear > endYear) {
      filters.value.yearEnd = filters.value.yearStart
    }
  }

  const validateFatalitiesRange = () => {
    const minFatalities = parseInt(filters.value.minFatalities)
    const maxFatalities = parseInt(filters.value.maxFatalities)
    
    if (minFatalities > maxFatalities) {
      filters.value.maxFatalities = filters.value.minFatalities
    }
  }

  const buildApiParams = () => {
    const params = {
      minFatalities: filters.value.minFatalities,
      maxFatalities: filters.value.maxFatalities,
      startYear: filters.value.yearStart,
      endYear: filters.value.yearEnd
    }

    if (filters.value.operator) {
      params.operator_ = filters.value.operator
    }

    if (filters.value.manufacturer) {
      params.manufacturer = filters.value.manufacturer
    }

    return params
  }

  return {
    filters,
    validateYearRange,
    validateFatalitiesRange,
    buildApiParams
  }
}
