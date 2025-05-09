import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ReservationService } from '../../services/reservations-service.service';
import { Reservation } from './reservation-model';
import { User} from './user-model';
import { Service} from './service-model';
import { CreateReservationComponent } from '../create-reservation/create-reservation.component';
import {UpdateReservationComponent} from '../update-reservation/update-reservation.component';
import {MyDialogConfirmComponent} from '../../../shared/dialogs/my-dialog-confirm/my-dialog-confirm.component';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import { Location } from '@angular/common';

@Component({
  selector: 'app-reservation',
  templateUrl: './reservation.component.html',
  standalone: false,
  styleUrls: ['./reservation.component.css'],
})
export class ReservationComponent implements OnInit {
  reservations: Reservation[] = [];
  users: User[] = [];
  services: Service[] = [];
  searchQuery: string = '';
  activeReservationId: number | null = null;

  currentPage: number = 1;
  pageSize: number = 10;
  totalItems: number = 0;

  constructor(
    private reservationService: ReservationService,
    private dialog: MatDialog,
    private location: Location
  ) {}

  ngOnInit() {
    this.loadReservations();
    this.loadUsers();
    this.loadServices();
  }

  loadReservations(): void {
    this.reservationService.getReservations(this.currentPage, this.pageSize).subscribe(response => {
      console.log('Reservations Response:', response); // Debugging

      this.reservations = response.reservations;
      this.totalItems = response.totalCount;
    });
  }

  loadUsers(): void {
    this.reservationService.getUsers().subscribe((data) => {
      console.log('Users received:', data);
      this.users = data;
    });
  }

  loadServices(): void {
    this.reservationService.getServices().subscribe((data) => {
      console.log('Services received:', data);
      this.services = data;
    });
  }

  filterReservations(): Reservation[] {
    return this.reservations.filter(
      (reservation) =>
        reservation.userFirstName.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        reservation.userLastName.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        reservation.serviceName.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  toggleActions(reservationId: number): void {
    this.activeReservationId = this.activeReservationId === reservationId ? null : reservationId;
  }

  deleteReservation(id: number): void {
    const dialogRef = this.dialog.open(MyDialogConfirmComponent, {
      width: '400px',
      data: {
        title: 'Delete confirmation',
        message: 'Are you sure you want to delete this reservation?',
        confirmButtonText: 'Delete'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.reservationService.deleteReservation(id).subscribe(() => {
          this.reservations = this.reservations.filter(r => r.id !== id);
        });
      }
    });
  }

  openCreateReservationModal(): void {
    const dialogRef = this.dialog.open(CreateReservationComponent, {
      width: '500px',
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((reservationCreated) => {
      if (reservationCreated) {
        this.loadReservations();
      }
    });
  }

  openUpdateReservationModal(reservation: Reservation): void {
    const dialogRef = this.dialog.open(UpdateReservationComponent, {
      width: '500px',
      data: { reservation }
    });

    dialogRef.afterClosed().subscribe((updated) => {
      if (updated) {
        this.loadReservations();
      }
    });
  }

  nextPage(): void {
    if (this.currentPage * this.pageSize < this.totalItems) {
      this.currentPage++;
      this.loadReservations();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadReservations();
    }
  }

  generatePDF(): void {
    const doc = new jsPDF();

    doc.setFontSize(16);
    doc.text('Reservation Report', 14, 20);

    const tableData = this.filterReservations().map(reservation => [
      reservation.userFirstName + reservation.userLastName,
      reservation.serviceName,
      reservation.reservationDate,
      reservation.reservationStatus
    ]);

    autoTable(doc, {
      startY: 30,
        head: [['User Name', 'Service', 'Reservation Date', 'Status']],
      body: tableData
    });

    doc.save('reservation_report.pdf');
  }

  goBack(): void {
    this.location.back();
  }
}
