<template>
  <div class="map-controls">
    <div class="card">
      <div class="card-body p-4">
        <h6 class="card-title mb-4">Map Filters</h6>
        
        <div class="mb-4">
          <label class="form-label small">Year Range</label>
          <DualRangeSlider
            v-model="yearRange"
            :min="filterConfig.YEAR_RANGE.MIN"
            :max="filterConfig.YEAR_RANGE.MAX"
            @change="handleYearRangeChange"
          />
        </div>
        
        <div class="mb-4">
          <label class="form-label small">Fatalities Range</label>
          <DualRangeSlider
            v-model="fatalitiesRange"
            :min="filterConfig.FATALITIES_RANGE.MIN"
            :max="filterConfig.FATALITIES_RANGE.MAX"
            @change="handleFatalitiesRangeChange"
          />
        </div>
        
        <div class="mb-4">
          <label class="form-label small">Operator</label>
          <select 
            class="form-select form-select-sm" 
            :value="filters.operator"
            @change="updateFilter('operator', $event.target.value)"
          >
            <option value="">All Operators</option>
            <option v-for="operator in operators" :key="operator" :value="operator">
              {{ operator }}
            </option>
          </select>
        </div>
        
        <div class="mb-4">
          <label class="form-label small">Manufacturer</label>
          <select 
            class="form-select form-select-sm" 
            :value="filters.manufacturer"
            @change="updateFilter('manufacturer', $event.target.value)"
          >
            <option value="">All Manufacturers</option>
            <option v-for="manufacturer in manufacturers" :key="manufacturer" :value="manufacturer">
              {{ manufacturer }}
            </option>
          </select>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, defineProps, defineEmits } from 'vue'
import { FILTER_CONFIG } from '../constants/index.js'
import DualRangeSlider from './DualRangeSlider.vue'

const props = defineProps({
  filters: {
    type: Object,
    required: true
  },
  operators: {
    type: Array,
    default: () => []
  },
  manufacturers: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['update:filters'])

// Make FILTER_CONFIG available to template
const filterConfig = FILTER_CONFIG

const yearRange = computed({
  get: () => ({
    start: props.filters.yearStart,
    end: props.filters.yearEnd
  }),
  set: (value) => {
    updateFilter('yearStart', value.start)
    updateFilter('yearEnd', value.end)
  }
})

const fatalitiesRange = computed({
  get: () => ({
    start: props.filters.minFatalities,
    end: props.filters.maxFatalities
  }),
  set: (value) => {
    updateFilter('minFatalities', value.start)
    updateFilter('maxFatalities', value.end)
  }
})

const updateFilter = (key, value) => {
  emit('update:filters', {
    ...props.filters,
    [key]: value
  })
}

const handleYearRangeChange = (value) => {
  yearRange.value = value
}

const handleFatalitiesRangeChange = (value) => {
  fatalitiesRange.value = value
}
</script>

<style scoped>
.map-controls {
  position: absolute;
  top: 80px;
  left: 20px;
  z-index: 1060;
  width: 300px;
}

.card {
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  border: none;
}
</style>
