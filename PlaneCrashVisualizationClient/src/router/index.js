import { createRouter, createWebHistory } from 'vue-router';
import Home from '../views/Home.vue';
import MapView from '../views/MapView.vue';
import Search from '../views/Search.vue';
import Trends from '../views/Trends.vue';
import CrashDetail from '../views/CrashDetail.vue';

const routes = [
  { path: '/', name: 'Home', component: Home },
  { path: '/map', name: 'Map', component: MapView },
  { path: '/search', name: 'Search', component: Search },
  { path: '/trends', name: 'Trends', component: Trends },
  { path: '/crash/:id', name: 'CrashDetail', component: CrashDetail }
];

export default createRouter({
  history: createWebHistory(),
  routes
});
