import { Component, OnInit } from '@angular/core';
import { AdminDashboardService, DashboardStats } from '../admin-dashboard-service/admin-dashboard-service.component';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css'],
  standalone: false
})
export class AdminDashboardComponent implements OnInit {
  stats: DashboardStats | null = null;
  isLoading = true;
  error = '';
  isDarkMode = false;

  constructor(private adminDashboardService: AdminDashboardService) {}

  ngOnInit(): void {
    this.loadDashboardStats();
  }

  loadDashboardStats(): void {
    this.adminDashboardService.getDashboardStats().subscribe({
      next: (data) => {
        this.stats = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Failed to load dashboard statistics.';
        this.isLoading = false;
        console.error(err);
      },
    });
  }

  toggleTheme(): void {
    this.isDarkMode = !this.isDarkMode;
    const theme = this.isDarkMode ? 'dark' : 'light';
    console.log(`Switching to ${theme} mode`);

    const dashboardElement = document.querySelector('.admin-dashboard');
    if (dashboardElement) {
      dashboardElement.setAttribute('data-theme', theme);
    }
  }
}
