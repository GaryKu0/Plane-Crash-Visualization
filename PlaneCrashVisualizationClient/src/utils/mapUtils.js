import L from 'leaflet'

export const FATALITY_THRESHOLDS = {
  LOW: 50,
  HIGH: 200
}

export const MARKER_COLORS = {
  LOW: '#28a745',
  MEDIUM: '#ffc107',
  HIGH: '#dc3545'
}

export const MARKER_SIZES = {
  LOW: 14,
  MEDIUM: 18,
  HIGH: 22
}

export function createMarkerIcon(fatalities) {
  let color = MARKER_COLORS.LOW
  let size = MARKER_SIZES.LOW
  
  if (fatalities > FATALITY_THRESHOLDS.HIGH) {
    color = MARKER_COLORS.HIGH
    size = MARKER_SIZES.HIGH
  } else if (fatalities > FATALITY_THRESHOLDS.LOW) {
    color = MARKER_COLORS.MEDIUM
    size = MARKER_SIZES.MEDIUM
  }
  
  return L.divIcon({
    className: 'custom-marker',
    html: `<div style="background-color: ${color}; width: ${size}px; height: ${size}px; border-radius: 50%; border: 2px solid white; box-shadow: 0 2px 4px rgba(0,0,0,0.3);"></div>`,
    iconSize: [size, size],
    iconAnchor: [size/2, size/2]
  })
}

export function createClusterIcon(cluster) {
  const childCount = cluster.getChildCount()
  let clusterClass = 'marker-cluster-small'
  
  if (childCount >= 100) {
    clusterClass = 'marker-cluster-large'
  } else if (childCount >= 10) {
    clusterClass = 'marker-cluster-medium'
  }
  
  return new L.DivIcon({ 
    html: `<div><span>${childCount}</span></div>`, 
    className: `marker-cluster ${clusterClass}`, 
    iconSize: new L.Point(40, 40) 
  })
}

export const MAP_CONFIG = {
  DEFAULT_CENTER: [39.8283, -98.5795],
  DEFAULT_ZOOM: 4,
  MAX_ZOOM: 18,
  TILE_URL: 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
  ATTRIBUTION: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}

export const CLUSTER_CONFIG = {
  maxClusterRadius: 80,
  spiderfyOnMaxZoom: true,
  showCoverageOnHover: false,
  zoomToBoundsOnClick: true,
  iconCreateFunction: createClusterIcon
}
