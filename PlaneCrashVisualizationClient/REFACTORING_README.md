# MapView Component Refactoring Documentation

## Overview

The MapView component has been completely refactored to improve maintainability, readability, and code organization. The monolithic component has been broken down into smaller, reusable components and utility functions following Vue 3 best practices.

## Refactoring Benefits

### 1. **Separation of Concerns**
- Business logic separated into composables
- UI components are focused on presentation
- Utilities handle data formatting and map configuration

### 2. **Reusability**
- Components can be reused across different views
- Composables can be shared between components
- Utilities are framework-agnostic

### 3. **Maintainability**
- Smaller, focused files are easier to debug
- Clear dependency structure
- Centralized configuration management

### 4. **Error Handling**
- Comprehensive error handling with custom ApiError class
- User-friendly error messages
- Graceful degradation on API failures

### 5. **Performance**
- Optimized watchers and debounced API calls
- Proper cleanup on component unmount
- Efficient marker management

## File Structure

```
src/
├── components/
│   ├── DualRangeSlider.vue     # Reusable dual range slider
│   ├── LoadingOverlay.vue      # Loading indicator component
│   ├── MapControls.vue         # Map filter controls
│   └── MapLegend.vue           # Map legend component
├── composables/
│   ├── useCrashData.js         # Data fetching and management
│   ├── useMap.js               # Map initialization and marker management
│   └── useMapFilters.js        # Filter state and validation
├── constants/
│   └── index.js                # Centralized configuration
├── services/
│   └── api.js                  # API service layer
├── utils/
│   ├── mapUtils.js             # Map-related utilities
│   └── popupUtils.js           # Popup content formatting
└── views/
    └── MapView.vue             # Main view component (refactored)
```

## Components

### DualRangeSlider.vue
A reusable component for dual-range input controls.

**Props:**
- `modelValue`: Object with `start` and `end` properties
- `min`: Minimum value (default: 0)
- `max`: Maximum value (default: 100)

**Events:**
- `update:modelValue`: Emitted when range changes
- `change`: Emitted with debounced changes

### LoadingOverlay.vue
A loading indicator with optional message display.

**Props:**
- `show`: Boolean to control visibility
- `message`: Optional loading message

### MapControls.vue
Filter controls for the map view.

**Props:**
- `filters`: Current filter values
- `operators`: Array of available operators
- `manufacturers`: Array of available manufacturers

**Events:**
- `update:filters`: Emitted when filters change

### MapLegend.vue
Legend component showing fatality severity levels.

**Features:**
- Dynamic legend items based on configuration
- Consistent styling with map markers

## Composables

### useMapFilters.js
Manages filter state and validation.

**Returns:**
- `filters`: Reactive filter object
- `validateYearRange()`: Ensures valid year range
- `validateFatalitiesRange()`: Ensures valid fatalities range
- `buildApiParams()`: Converts filters to API parameters

### useMap.js
Handles map initialization and marker management.

**Returns:**
- `initializeMap()`: Initializes Leaflet map
- `loadMarkers(data)`: Loads crash markers on map
- `destroyMap()`: Cleanup function
- `getMap()`: Returns map instance

### useCrashData.js
Manages data fetching and API interactions.

**Returns:**
- `crashData`: Reactive crash data array
- `operators`: Available operators
- `manufacturers`: Available manufacturers
- `loading`: Loading state
- `error`: Error state
- `fetchCrashData(params)`: Fetch crash data with filters
- `initializeData()`: Initial data loading

## Utilities

### mapUtils.js
Map-related constants and utility functions.

**Exports:**
- `FATALITY_THRESHOLDS`: Severity level thresholds
- `MARKER_COLORS`: Color scheme for markers
- `MARKER_SIZES`: Size configuration for markers
- `createMarkerIcon(fatalities)`: Creates Leaflet marker icons
- `createClusterIcon(cluster)`: Creates cluster icons
- `MAP_CONFIG`: Map configuration constants
- `CLUSTER_CONFIG`: Cluster configuration

### popupUtils.js
Popup content formatting utilities.

**Exports:**
- `formatDate(dateString)`: Safe date formatting
- `formatValue(value, default)`: Safe value formatting
- `formatNumber(value, default)`: Safe number formatting
- `createPopupContent(crash)`: Creates HTML popup content

## Services

### api.js
Centralized API service with error handling.

**Features:**
- Custom `ApiError` class for structured error handling
- Consistent error logging
- Automatic data sorting for dropdowns
- Configuration-based endpoints

## Constants

### index.js
Centralized configuration management.

**Exports:**
- `API_CONFIG`: API URLs and endpoints
- `FILTER_CONFIG`: Filter default values and ranges
- `UI_CONFIG`: UI behavior configuration
- Re-exports from other utility files

## Usage Example

```vue
<template>
  <MapView />
</template>

<script setup>
import MapView from '@/views/MapView.vue'
</script>
```

## Migration Guide

### Before (Monolithic)
- Single 591-line file
- Mixed concerns (data, UI, map logic)
- Inline API calls
- Hardcoded configuration
- Limited error handling

### After (Modular)
- 10+ focused files under 100 lines each
- Separated concerns with clear responsibilities
- Service layer for API interactions
- Centralized configuration
- Comprehensive error handling

## Best Practices Implemented

1. **Composition API**: Leverages Vue 3's Composition API for better logic reuse
2. **Single Responsibility**: Each file has a single, clear purpose
3. **Error Boundaries**: Proper error handling at each layer
4. **Configuration Management**: Centralized constants for easy maintenance
5. **Performance Optimization**: Debounced API calls and efficient updates
6. **Accessibility**: Proper ARIA labels and semantic HTML
7. **Type Safety**: Consistent parameter validation and prop types

## Testing Considerations

The refactored structure makes testing much easier:

- **Components**: Can be tested in isolation with mocked props
- **Composables**: Pure functions that can be unit tested
- **Utilities**: Framework-agnostic functions easy to test
- **Services**: API layer can be mocked for integration tests

## Future Improvements

1. **TypeScript**: Add TypeScript for better type safety
2. **Unit Tests**: Add comprehensive test coverage
3. **Storybook**: Document components in Storybook
4. **Performance**: Add virtual scrolling for large datasets
5. **Accessibility**: Enhanced keyboard navigation and screen reader support
6. **Internationalization**: Add i18n support for multi-language
7. **Theme Support**: Add dark/light theme toggle
8. **Error Recovery**: Add retry mechanisms for failed API calls
