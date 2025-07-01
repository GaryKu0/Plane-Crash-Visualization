export function formatDate(dateString) {
  if (!dateString) return 'Unknown Date'
  try {
    return new Date(dateString).toLocaleDateString()
  } catch {
    return 'Invalid Date'
  }
}

export function formatValue(value, defaultValue = 'Unknown') {
  return value || defaultValue
}

export function formatNumber(value, defaultValue = 0) {
  return value || defaultValue
}

export function createPopupContent(crash) {
  return `
    <div style="min-width: 300px; max-width: 400px;">
      <h6 class="text-danger mb-2">${formatDate(crash.date)}</h6>
      
      <div class="row mb-2">
        <div class="col-6">
          <small><strong>Time:</strong></small><br>
          <span>${formatValue(crash.time)}</span>
        </div>
        <div class="col-6">
          <small><strong>Location:</strong></small><br>
          <span>${formatValue(crash.location)}</span>
        </div>
      </div>
      
      <div class="row mb-2">
        <div class="col-6">
          <small><strong>Operator:</strong></small><br>
          <span>${formatValue(crash.operator_)}</span>
        </div>
        <div class="col-6">
          <small><strong>Flight:</strong></small><br>
          <span>${formatValue(crash.flight)}</span>
        </div>
      </div>
      
      <div class="mb-2">
        <small><strong>Route:</strong></small><br>
        <span>${formatValue(crash.route)}</span>
      </div>
      
      <div class="row mb-2">
        <div class="col-6">
          <small><strong>Aircraft Type:</strong></small><br>
          <span>${formatValue(crash.ac_Type)}</span>
        </div>
        <div class="col-6">
          <small><strong>Registration:</strong></small><br>
          <span>${formatValue(crash.registration)}</span>
        </div>
      </div>
      
      <div class="mb-2">
        <small><strong>Cn/ln:</strong></small><br>
        <span>${formatValue(crash.cn_ln)}</span>
      </div>
      
      <div class="row mb-2">
        <div class="col-4">
          <small><strong>Aboard:</strong></small><br>
          <span class="badge bg-primary">${formatNumber(crash.aboard)}</span>
        </div>
        <div class="col-4">
          <small><strong>Crew:</strong></small><br>
          <span class="badge bg-info">${formatNumber(crash.aboardCrew)}</span>
        </div>
        <div class="col-4">
          <small><strong>Passengers:</strong></small><br>
          <span class="badge bg-info">${formatNumber(crash.aboardPassengers)}</span>
        </div>
      </div>
      
      <div class="row mb-2">
        <div class="col-4">
          <small><strong>Fatalities:</strong></small><br>
          <span class="badge bg-danger">${formatNumber(crash.fatalities)}</span>
        </div>
        <div class="col-4">
          <small><strong>Crew:</strong></small><br>
          <span class="badge bg-warning text-dark">${formatNumber(crash.fatalitiesCrew)}</span>
        </div>
        <div class="col-4">
          <small><strong>Passengers:</strong></small><br>
          <span class="badge bg-warning text-dark">${formatNumber(crash.fatalitiesPassengers)}</span>
        </div>
      </div>
      
      <div class="mb-2">
        <small><strong>Ground Fatalities:</strong></small><br>
        <span class="badge bg-secondary">${formatNumber(crash.ground)}</span>
      </div>
      
      ${crash.summary ? `
      <div class="mb-2">
        <small><strong>Summary:</strong></small><br>
        <small class="text-muted">${crash.summary}</small>
      </div>
      ` : ''}
    </div>
  `
}
