<template>
  <div class="map-legend">
    <div class="card">
      <div class="card-body p-2">
        <h6 class="card-title mb-2 small">Legend</h6>
        <div 
          v-for="item in legendItems" 
          :key="item.class"
          class="legend-item"
        >
          <div :class="['legend-marker', item.class]"></div>
          <small>{{ item.label }}</small>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { FATALITY_THRESHOLDS } from '../utils/mapUtils.js'

const legendItems = computed(() => [
  {
    class: 'low',
    label: `1-${FATALITY_THRESHOLDS.LOW} Fatalities`
  },
  {
    class: 'medium', 
    label: `${FATALITY_THRESHOLDS.LOW + 1}-${FATALITY_THRESHOLDS.HIGH} Fatalities`
  },
  {
    class: 'high',
    label: `${FATALITY_THRESHOLDS.HIGH + 1}+ Fatalities`
  }
])
</script>

<style scoped>
.map-legend {
  position: absolute;
  bottom: 20px;
  right: 20px;
  z-index: 1000;
}

.legend-item {
  display: flex;
  align-items: center;
  margin-bottom: 5px;
}

.legend-marker {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  margin-right: 8px;
  border: 1px solid white;
  box-shadow: 0 1px 2px rgba(0,0,0,0.3);
}

.legend-marker.low {
  background-color: #28a745;
}

.legend-marker.medium {
  background-color: #ffc107;
}

.legend-marker.high {
  background-color: #dc3545;
}

.card {
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  border: none;
}
</style>
