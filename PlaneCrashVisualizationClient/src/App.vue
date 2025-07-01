<script setup>
import { ref, onMounted } from 'vue'


const error = ref(null)

</script>

<template>
  <div id="app">
    <!-- Navigation Header -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary fixed-top">
      <div class="container-fluid px-4">
        <router-link class="navbar-brand fw-bold" to="/">
          <i class="bi bi-airplane me-2"></i>
          Plane Crash Visualization
        </router-link>
        
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav ms-auto">
            <li class="nav-item">
              <router-link class="nav-link" to="/" active-class="active">
                <i class="bi bi-house me-1"></i>Home
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/map" active-class="active">
                <i class="bi bi-geo-alt me-1"></i>Map
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/search" active-class="active">
                <i class="bi bi-search me-1"></i>Search
              </router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/trends" active-class="active">
                <i class="bi bi-graph-up-arrow me-1"></i>Trends
              </router-link>
            </li>
          </ul>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main>
      <router-view />
    </main>

    <!-- API Status (for development) -->
    <div v-if="error" class="position-fixed bottom-0 end-0 m-3">
      <div class="alert alert-warning alert-dismissible fade show" role="alert">
        <i class="bi bi-exclamation-triangle me-2"></i>
        <small>Development Mode: {{ error }}</small>
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'App'
}
</script>

<style scoped>
.navbar-brand {
  font-size: 1.5rem;
}

.navbar {
  min-height: 60px;
  z-index: 1050;
}

.nav-link.active {
  font-weight: 600;
  background-color: rgba(255, 255, 255, 0.1);
  border-radius: 0.375rem;
}

.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.05);
  border-radius: 0.375rem;
}

#app {
  min-height: 100vh;
}

main {
  position: relative;
  z-index: 1;
}
</style>
