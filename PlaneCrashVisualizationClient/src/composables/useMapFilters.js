import { ref, watch } from 'vue'
import { FILTER_CONFIG, createFilterConfig } from '../constants/index.js'

export function useMapFilters(statistics = null) {
  // Create dynamic config based on statistics, fallback to static config
  const config = statistics ? createFilterConfig(statistics) : FILTER_CONFIG
  
  const filters = ref({
    yearRange: { 
      start: config.YEAR_RANGE.DEFAULT_START, 
      end: config.YEAR_RANGE.DEFAULT_END 
    },
    fatalitiesRange: { 
      start: config.FATALITIES_RANGE.DEFAULT_MIN, 
      end: config.FATALITIES_RANGE.DEFAULT_MAX 
    },
    operator: '',
    manufacturer: ''
  })

  // Update filters when statistics change
  const updateFiltersWithStatistics = (newStatistics) => {
    if (newStatistics) {
      const newConfig = createFilterConfig(newStatistics)
      filters.value.yearRange = {
        start: newConfig.YEAR_RANGE.DEFAULT_START,
        end: newConfig.YEAR_RANGE.DEFAULT_END
      }
      filters.value.fatalitiesRange = {
        start: newConfig.FATALITIES_RANGE.DEFAULT_MIN,
        end: newConfig.FATALITIES_RANGE.DEFAULT_MAX
      }
    }
  }

  const validateYearRange = () => {
    const startYear = parseInt(filters.value.yearRange.start)
    const endYear = parseInt(filters.value.yearRange.end)
    
    if (startYear > endYear) {
      filters.value.yearRange.end = filters.value.yearRange.start
    }
  }

  const validateFatalitiesRange = () => {
    const minFatalities = parseInt(filters.value.fatalitiesRange.start)
    const maxFatalities = parseInt(filters.value.fatalitiesRange.end)
    
    if (minFatalities > maxFatalities) {
      filters.value.fatalitiesRange.end = filters.value.fatalitiesRange.start
    }
  }

  const buildApiParams = () => {
    const params = {
      minFatalities: filters.value.fatalitiesRange.start,
      maxFatalities: filters.value.fatalitiesRange.end,
      startYear: filters.value.yearRange.start,
      endYear: filters.value.yearRange.end
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
    updateFiltersWithStatistics,
    validateYearRange,
    validateFatalitiesRange,
    buildApiParams
  }
}
