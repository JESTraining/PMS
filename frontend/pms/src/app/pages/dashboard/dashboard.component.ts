import { Component, inject, PLATFORM_ID, signal, afterNextRender, OnInit } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ChartComponent } from 'ng-apexcharts';
import type {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexFill,
  ApexGrid,
  ApexLegend,
  ApexPlotOptions,
  ApexStroke,
  ApexTooltip,
  ApexXAxis,
  ApexNonAxisChartSeries,
  ApexMarkers
} from 'ng-apexcharts';

import { AnalyticsService } from '../../services/analytics/analytics.service';
import { DashboardSummary } from '../../models/analytics/dashboard-summary.model';

type ChartTab = 'daily' | 'weekly' | 'monthly';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, ChartComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly platformId = inject(PLATFORM_ID);
  userName = '';

  /** Apex runs only in the browser (avoids SSR/prerender issues). */
  chartsReady = signal(false);

  chartTab = signal<ChartTab>('daily');

  appointmentDays = signal<{ dow: string, day: number, active: boolean }[]>([]);
  private allAppointments: any[] = [];
  appointmentsList = signal<any[]>([]);
  flatCalendarCells: { cell: number | null, id: string }[] = [];
  currentMonthName = '';

  /** Sparklines — Cliniva-style KPI row */

  sparkXaxis: ApexXAxis = {
    type: 'category',
    categories: [],
    labels: { show: false },
    axisBorder: { show: false },
    axisTicks: { show: false }
  };

  sparkMarkers: ApexMarkers = {
    size: 0,
    strokeColors: '#fff',
    strokeWidth: 2,
    hover: {
      size: 5,
    }
  };

  sparkTooltipAppointments: ApexTooltip = {
    custom: ({ series, seriesIndex, dataPointIndex }) => {
      const date = this.sparkXaxis.categories[dataPointIndex] || '';
      const val = series[seriesIndex][dataPointIndex];
      return `<div class="bg-white px-2 py-1 text-xs border border-gray-200 rounded shadow font-medium text-gray-700">
                ${date}: <span class="text-violet-600 font-bold">${val} Appointments</span>
              </div>`;
    }
  };

  sparkTooltipEncounters: ApexTooltip = {
    custom: ({ series, seriesIndex, dataPointIndex }) => {
      const date = this.sparkXaxis.categories[dataPointIndex] || '';
      const val = series[seriesIndex][dataPointIndex];
      return `<div class="bg-white px-2 py-1 text-xs border border-gray-200 rounded shadow font-medium text-gray-700">
                ${date}: <span class="text-orange-600 font-bold">${val} Encounters</span>
              </div>`;
    }
  };

  sparkTooltipPatients: ApexTooltip = {
    custom: ({ series, seriesIndex, dataPointIndex }) => {
      const date = this.sparkXaxis.categories[dataPointIndex] || '';
      const val = series[seriesIndex][dataPointIndex];
      return `<div class="bg-white px-2 py-1 text-xs border border-gray-200 rounded shadow font-medium text-gray-700">
                ${date}: <span class="text-green-600 font-bold">${val} New Patients</span>
              </div>`;
    }
  };

  sparkTooltipEarning: ApexTooltip = {
    custom: ({ series, seriesIndex, dataPointIndex }) => {
      const date = this.sparkXaxis.categories[dataPointIndex] || '';
      const val = series[seriesIndex][dataPointIndex];
      return `<div class="bg-white px-2 py-1 text-xs border border-gray-200 rounded shadow font-medium text-gray-700">
                ${date}: <span class="text-blue-600 font-bold">$${val.toLocaleString()}</span>
              </div>`;
    }
  };

  sparkAppointments: ApexAxisChartSeries = [
    { name: 'a', data: [12, 18, 14, 22, 19, 28, 32, 26, 35, 40, 38, 45] }
  ];
  sparkOperations: ApexAxisChartSeries = [
    { name: 'o', data: [4, 6, 5, 8, 7, 9, 11, 10, 12, 14, 13, 15] }
  ];
  sparkPatients: ApexAxisChartSeries = [
    { name: 'p', data: [8, 10, 14, 11, 16, 18, 22, 20, 24, 28, 26, 30] }
  ];
  sparkEarning: ApexAxisChartSeries = [
    { name: 'e', data: [120, 190, 160, 210, 240, 280, 260, 300, 320, 340, 360, 380] }
  ];

  sparkChartBase: ApexChart = {
    type: 'area',
    height: 64,
    sparkline: { enabled: true },
    toolbar: { show: false },
    animations: { enabled: true }
  };

  sparkStroke: ApexStroke = { curve: 'smooth', width: 2 };
  sparkDataLabels: ApexDataLabels = { enabled: false };
  sparkTooltip: ApexTooltip = {
    fixed: { enabled: false },
    x: { show: false },
    marker: { show: false }
  };

  sparkFillPurple: ApexFill = {
    type: 'gradient',
    gradient: {
      shadeIntensity: 1,
      opacityFrom: 0.45,
      opacityTo: 0.05,
      stops: [0, 90, 100],
      colorStops: [
        { offset: 0, color: '#7c3aed', opacity: 1 },
        { offset: 100, color: '#7c3aed', opacity: 0.1 }
      ]
    }
  };

  sparkFillOrange: ApexFill = {
    type: 'gradient',
    gradient: {
      shadeIntensity: 1,
      opacityFrom: 0.45,
      opacityTo: 0.05,
      stops: [0, 90, 100],
      colorStops: [
        { offset: 0, color: '#ea580c', opacity: 1 },
        { offset: 100, color: '#ea580c', opacity: 0.1 }
      ]
    }
  };

  sparkFillGreen: ApexFill = {
    type: 'gradient',
    gradient: {
      shadeIntensity: 1,
      opacityFrom: 0.45,
      opacityTo: 0.05,
      stops: [0, 90, 100],
      colorStops: [
        { offset: 0, color: '#16a34a', opacity: 1 },
        { offset: 100, color: '#16a34a', opacity: 0.1 }
      ]
    }
  };

  sparkFillBlue: ApexFill = {
    type: 'gradient',
    gradient: {
      shadeIntensity: 1,
      opacityFrom: 0.45,
      opacityTo: 0.05,
      stops: [0, 90, 100],
      colorStops: [
        { offset: 0, color: '#2563eb', opacity: 1 },
        { offset: 100, color: '#2563eb', opacity: 0.1 }
      ]
    }
  };

  sparkColorsPurple = ['#7c3aed'];
  sparkColorsOrange = ['#ea580c'];
  sparkColorsGreen = ['#16a34a'];
  sparkColorsBlue = ['#2563eb'];

  /** Donut — Patient chart */
  patientSeries: ApexNonAxisChartSeries = [22, 15, 20, 18];
  patientLabels = ['Dengue', 'Typhoid', 'Malaria', 'Cold'];
  patientChart: ApexChart = {
    type: 'donut',
    height: 280,
    width: '100%',
    fontFamily: 'Roboto, system-ui, sans-serif',
    toolbar: { show: false }
  };
  patientColors = ['#8b5cf6', '#6366f1', '#f59e0b', '#22c55e'];
  patientPlot: ApexPlotOptions = {
    pie: {
      donut: {
        size: '72%',
        labels: {
          show: true,
          name: { show: true, fontSize: '14px' },
          value: { fontSize: '22px', fontWeight: 600 },
          total: {
            show: true,
            showAlways: true,
            label: 'Total People',
            fontSize: '13px',
            color: '#6b7280',
            formatter: () => '75'
          }
        }
      }
    }
  };
  patientLegend: ApexLegend = {
    position: 'bottom',
    fontSize: '12px',
    markers: { size: 6, strokeWidth: 0 },
    itemMargin: { horizontal: 10, vertical: 4 }
  };
  patientDataLabels: ApexDataLabels = { enabled: false };

  /** Hospital survey — line */
  surveySeries: ApexAxisChartSeries = [
    { name: 'New Patients', data: [28, 35, 42, 38, 48, 52, 45, 58, 62, 55, 60, 68] },
    { name: 'Old Patients', data: [45, 42, 48, 44, 50, 46, 52, 49, 54, 50, 56, 53] }
  ];
  surveyChart: ApexChart = {
    type: 'line',
    height: 300,
    width: '100%',
    fontFamily: 'Roboto, system-ui, sans-serif',
    toolbar: { show: false },
    zoom: { enabled: false }
  };
  surveyStroke: ApexStroke = { curve: 'smooth', width: 3 };
  surveyColors = ['#3b82f6', '#fb923c'];
  surveyXaxis: ApexXAxis = {
    categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    labels: { style: { colors: '#9ca3af', fontSize: '11px' } },
    axisBorder: { show: false },
    axisTicks: { show: false }
  };
  surveyGrid: ApexGrid = {
    borderColor: '#f3f4f6',
    strokeDashArray: 4,
    padding: { top: 8, right: 8 }
  };
  surveyLegend: ApexLegend = {
    position: 'top',
    horizontalAlign: 'right',
    fontSize: '12px',
    markers: { size: 5 }
  };
  surveyTooltip: ApexTooltip = { theme: 'light', x: { show: true } };

  constructor() {
    if (isPlatformBrowser(this.platformId)) {
      this.userName = localStorage.getItem('pms_user_name') || 'User';
    }
    afterNextRender(() => this.chartsReady.set(true));
  }

  setChartTab(tab: ChartTab): void {
    this.chartTab.set(tab);
    const sets: Record<ChartTab, number[]> = {
      daily: [22, 15, 20, 18],
      weekly: [18, 20, 24, 16],
      monthly: [30, 18, 22, 14]
    };
    this.patientSeries = [...sets[tab]];
  }
  
  onSelectDay(selectedDay: number) {
    this.appointmentDays.update(days => days.map(d => ({
      ...d,
      active: d.day === selectedDay
    })));
    this.filterAppointmentsByDay(selectedDay);
  }

  // Analytics data
  private analyticsService = inject(AnalyticsService);
  summary = signal<DashboardSummary | null>(null);

  ngOnInit() {
    this.generateWeekStrip();
    this.generateCalendar();
    this.loadDashboardData();
  }

  generateWeekStrip() {
    const today = new Date();
    const days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
    const startOfWeek = new Date(today);
    startOfWeek.setDate(today.getDate() - today.getDay()); // Ir al domingo

    const arr = Array.from({ length: 7 }).map((_, i) => {
      const d = new Date(startOfWeek);
      d.setDate(startOfWeek.getDate() + i);
      return {
        dow: days[d.getDay()],
        day: d.getDate(),
        active: d.getDate() === today.getDate() && d.getMonth() === today.getMonth()
      };
    });
    this.appointmentDays.set(arr);
  }

  generateCalendar() {
    const today = new Date();
    this.currentMonthName = today.toLocaleString('default', { month: 'long', year: 'numeric' }); // "May 2026"
    
    const year = today.getFullYear();
    const month = today.getMonth();
    const firstDay = new Date(year, month, 1).getDay();
    const daysInMonth = new Date(year, month + 1, 0).getDate();

    const cells: { cell: number | null, id: string }[] = [];
    let dayCounter = 1;

    for (let i = 0; i < 42; i++) {
      if (i < firstDay || dayCounter > daysInMonth) {
        cells.push({ cell: null, id: `empty-${i}` });
      } else {
        cells.push({ cell: dayCounter, id: `day-${dayCounter}` });
        dayCounter++;
      }
    }
    this.flatCalendarCells = cells;
  }

  isToday(day: number): boolean {
    const today = new Date();
    return day === today.getDate();
  }

  getDayData(day: number) {
    if (!this.summary() || !this.summary()?.calendarActiveDays) return null;
    return this.summary()?.calendarActiveDays.find(d => d.day === day) || null;
  }

    loadDashboardData() {
    this.analyticsService.getSummary().subscribe(data => {
      this.summary.set(data);
      this.allAppointments = data.monthlyAppointments; 
      const today = new Date().getDate();
      this.filterAppointmentsByDay(today);
      this.updatePatientChart(data);
    });
    

    this.analyticsService.getSparklines().subscribe(data => {
      this.sparkXaxis = { ...this.sparkXaxis, categories: data.dates };
      this.sparkAppointments = [{ name: 'Appointments', data: data.appointmentsHistory }];
      this.sparkOperations = [{ name: 'Encounters', data: data.encountersHistory }];
      this.sparkPatients = [{ name: 'New Patients', data: data.patientsHistory }];
      this.sparkEarning = [{ name: 'Earning', data: data.earningsHistory }];
    });
  }

  private filterAppointmentsByDay(day: number) {
    const filtered = this.allAppointments.filter(a => {
      const apptDate = new Date(a.fullDate);
      return apptDate.getDate() === day;
    });
    this.appointmentsList.set(filtered);
  }

  updatePatientChart(data: DashboardSummary) {
    if (data.topConditions && data.topConditions.length > 0) {
      this.patientSeries = data.topConditions.map(c => c.patientCount);
      this.patientLabels = data.topConditions.map(c => c.conditionName);
    } else {
      this.patientSeries = [100]; 
      this.patientLabels = ['Conditions data not available'];
      this.patientColors = ['#575758'];
    }
  }
}
