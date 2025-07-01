<template>
  <div 
    class="dual-range-container"
    :style="{
      '--range-start': ((modelValue.start - min) / (max - min)) * 100,
      '--range-end': ((modelValue.end - min) / (max - min)) * 100
    }"
  >
    <input 
      type="range" 
      class="form-range range-min" 
      :min="min" 
      :max="max" 
      :value="modelValue.start"
      @input="updateStart"
      @focus="bringToFront('min')"
      @mousedown="bringToFront('min')"
    >
    <input 
      type="range" 
      class="form-range range-max" 
      :min="min" 
      :max="max" 
      :value="modelValue.end"
      @input="updateEnd"
      @focus="bringToFront('max')"
      @mousedown="bringToFront('max')"
    >
  </div>
  <div class="d-flex justify-content-between mt-2">
    <small class="text-muted">{{ modelValue.start }}</small>
    <small class="text-muted">-</small>
    <small class="text-muted">{{ modelValue.end }}</small>
  </div>
</template>

<script setup>
import { defineEmits, defineProps, ref, nextTick } from 'vue'

const props = defineProps({
  modelValue: {
    type: Object,
    required: true,
    validator: (value) => {
      return value && typeof value.start !== 'undefined' && typeof value.end !== 'undefined'
    }
  },
  min: {
    type: [Number, String],
    default: 0
  },
  max: {
    type: [Number, String],
    default: 100
  }
})

const emit = defineEmits(['update:modelValue', 'change'])

const bringToFront = async (slider) => {
  // Use nextTick to ensure DOM is updated
  await nextTick()
  
  // Find the container
  const container = document.querySelector('.dual-range-container')
  if (!container) return
  
  const minSlider = container.querySelector('.range-min')
  const maxSlider = container.querySelector('.range-max')
  
  if (slider === 'min') {
    minSlider.style.zIndex = '4'
    maxSlider.style.zIndex = '3'
  } else {
    maxSlider.style.zIndex = '4'
    minSlider.style.zIndex = '3'
  }
}

const updateStart = (event) => {
  const start = parseInt(event.target.value)
  let end = parseInt(props.modelValue.end)
  
  // Ensure start doesn't exceed end
  if (start > end) {
    end = start
  }
  
  const newValue = { start, end }
  emit('update:modelValue', newValue)
  emit('change', newValue)
}

const updateEnd = (event) => {
  const end = parseInt(event.target.value)
  let start = parseInt(props.modelValue.start)
  
  // Ensure end doesn't go below start
  if (end < start) {
    start = end
  }
  
  const newValue = { start, end }
  emit('update:modelValue', newValue)
  emit('change', newValue)
}
</script>

<style scoped>
.dual-range-container {
  position: relative;
  height: 24px;
  margin: 10px 0;
}

.dual-range-container input[type="range"] {
  position: absolute;
  width: 100%;
  height: 24px;
  top: 0;
  left: 0;
  appearance: none;
  background: transparent;
  pointer-events: none;
}

.dual-range-container input[type="range"].range-min {
  z-index: 2;
}

.dual-range-container input[type="range"].range-max {
  z-index: 3;
}

/* When min slider is focused, bring it to front */
.dual-range-container input[type="range"].range-min:focus {
  z-index: 4;
}

.dual-range-container input[type="range"]::-webkit-slider-thumb {
  appearance: none;
  height: 20px;
  width: 20px;
  border-radius: 50%;
  background: #007bff;
  cursor: pointer;
  pointer-events: all;
  border: 2px solid white;
  box-shadow: 0 2px 6px rgba(0,0,0,0.2);
  position: relative;
  transition: all 0.15s ease;
}

.dual-range-container input[type="range"]::-webkit-slider-thumb:hover {
  background: #0056b3;
  transform: scale(1.1);
}

.dual-range-container input[type="range"]::-webkit-slider-thumb:active {
  background: #004085;
  transform: scale(1.2);
}

.dual-range-container input[type="range"]::-moz-range-thumb {
  height: 20px;
  width: 20px;
  border-radius: 50%;
  background: #007bff;
  cursor: pointer;
  pointer-events: all;
  border: 2px solid white;
  box-shadow: 0 2px 6px rgba(0,0,0,0.2);
  position: relative;
  transition: all 0.15s ease;
}

.dual-range-container input[type="range"]::-moz-range-thumb:hover {
  background: #0056b3;
  transform: scale(1.1);
}

.dual-range-container input[type="range"]::-moz-range-thumb:active {
  background: #004085;
  transform: scale(1.2);
}

.dual-range-container::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  height: 6px;
  background: #e9ecef;
  border-radius: 3px;
  transform: translateY(-50%);
  z-index: 1;
}

.dual-range-container input[type="range"]::-webkit-slider-track {
  appearance: none;
  background: transparent;
}

.dual-range-container input[type="range"]::-moz-range-track {
  background: transparent;
}

/* Add a visual indicator for the selected range */
.dual-range-container::after {
  content: '';
  position: absolute;
  top: 50%;
  height: 6px;
  background: #007bff;
  border-radius: 3px;
  transform: translateY(-50%);
  z-index: 1;
  left: calc(var(--range-start, 0) * 1%);
  width: calc((var(--range-end, 100) - var(--range-start, 0)) * 1%);
  transition: all 0.15s ease;
}
</style>
