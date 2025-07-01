import { ref, onMounted, onUnmounted } from 'vue'
import { useRoute } from 'vue-router'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import 'leaflet.markercluster/dist/MarkerCluster.css'
import 'leaflet.markercluster/dist/MarkerCluster.Default.css'
import 'leaflet.markercluster/dist/leaflet.markercluster.js'

import { MAP_CONFIG, CLUSTER_CONFIG, createMarkerIcon } from '../utils/mapUtils.js'
import { createPopupContent } from '../utils/popupUtils.js'

export function useMap() {
  const route = useRoute()
  let map = null
  let markerClusterGroup = null

  const initializeMap = () => {
    // Initialize map
    map = L.map('map').setView(MAP_CONFIG.DEFAULT_CENTER, MAP_CONFIG.DEFAULT_ZOOM)

    // Add tile layer
    L.tileLayer(MAP_CONFIG.TILE_URL, {
      attribution: MAP_CONFIG.ATTRIBUTION,
      maxZoom: MAP_CONFIG.MAX_ZOOM
    }).addTo(map)

    // Create marker cluster group
    markerClusterGroup = L.markerClusterGroup(CLUSTER_CONFIG).addTo(map)

    // Check for URL parameters to set initial view
    if (route.query.lat && route.query.lng) {
      const lat = parseFloat(route.query.lat)
      const lng = parseFloat(route.query.lng)
      const zoom = parseInt(route.query.zoom) || MAP_CONFIG.DEFAULT_ZOOM
      map.setView([lat, lng], zoom)
    }
  }

  const loadMarkers = (crashData) => {
    if (!markerClusterGroup || !crashData) return

    // Clear existing markers
    markerClusterGroup.clearLayers()
    
    // Add crash markers
    crashData.forEach(crash => {
      if (!crash.latitude || !crash.longitude) return
      
      const marker = L.marker([crash.latitude, crash.longitude], {
        icon: createMarkerIcon(crash.fatalities || 0)
      })
      
      marker.bindPopup(createPopupContent(crash))
      markerClusterGroup.addLayer(marker)
    })
  }

  const destroyMap = () => {
    if (map) {
      map.remove()
      map = null
      markerClusterGroup = null
    }
  }

  return {
    initializeMap,
    loadMarkers,
    destroyMap,
    getMap: () => map
  }
}
