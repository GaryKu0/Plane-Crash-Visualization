<template>
  <div class="container text-center py-5">
    <h1 class="display-4 fw-bold mb-3">Plane Crash Data Visualization</h1>
    <p class="lead mb-5">
      Explore Data on plane crashes around the world. Use the interactive map and charts to analyze trends,
      search for specific incidents, and filter by various criteria.
    </p>

    <div class="row justify-content-center">
      <div class="col-md-4 mb-4">
        <div class="card h-100 shadow-sm clickable" @click="$router.push('/map')">
          <div class="card-body">
            <div class="mb-3">
              <i class="bi bi-geo-alt-fill" style="font-size: 2rem;"></i>
            </div>
            <h5 class="card-title">Interactive Map</h5>
            <p class="card-text">View crashes on a map and filter by location</p>
          </div>
        </div>
      </div>

      <div class="col-md-4 mb-4">
        <div class="card h-100 shadow-sm clickable" @click="$router.push('/search')">
          <div class="card-body">
            <div class="mb-3">
              <i class="bi bi-search" style="font-size: 2rem;"></i>
            </div>
            <h5 class="card-title">Advanced Search</h5>
            <p class="card-text">Search for plane crashes by date, location, and more</p>
          </div>
        </div>
      </div>

      <div class="col-md-4 mb-4">
        <div class="card h-100 shadow-sm clickable" @click="$router.push('/trends')">
          <div class="card-body">
            <div class="mb-3">
              <i class="bi bi-graph-up-arrow" style="font-size: 2rem;"></i>
            </div>
            <h5 class="card-title">Trend Analysis</h5>
            <p class="card-text">Analyze statistics and trends over time</p>
          </div>
        </div>
      </div>
    </div>

    <footer class="mt-5 small">
      <span class="me-3">Impressum</span>
      <span class="me-3">Datenschutz</span>
      <span>Disclaimer</span>
    </footer>
  </div>
</template>

<script setup>
// Home page component
</script>

<style scoped>
.clickable {
  cursor: pointer;
  transition: transform 0.2s;
}
.clickable:hover {
  transform: translateY(-4px);
}
</style>
