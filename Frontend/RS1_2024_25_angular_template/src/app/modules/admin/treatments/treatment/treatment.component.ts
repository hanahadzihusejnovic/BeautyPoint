import { Component, OnInit } from '@angular/core';
import { TreatmentService } from '../../services/treatment-service.service';
import { Treatment } from './treatment-model';
import { CreateTreatmentComponent } from '../create-treatment/create-treatment.component';
import { UpdateTreatmentComponent } from '../update-treatment/update-treatment.component';
import { MatDialog } from '@angular/material/dialog';
import {MyDialogConfirmComponent} from '../../../shared/dialogs/my-dialog-confirm/my-dialog-confirm.component';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import { Location } from '@angular/common';

@Component({
  selector: 'app-treatment',
  templateUrl: './treatment.component.html',
  standalone: false,
  styleUrls: ['./treatment.component.css'],
})
export class TreatmentComponent implements OnInit {
  treatments: Treatment[] = [];
  searchQuery: string = '';
  activeTreatmentId: number | null = null;

  currentPage: number = 1;
  pageSize: number = 10;
  totalItems: number = 0;

  constructor(private treatmentService: TreatmentService, private dialog: MatDialog, private location: Location) {}

  ngOnInit() {
    this.loadTreatments();
  }

  loadTreatments(): void {
    this.treatmentService.getTreatments(this.currentPage, this.pageSize).subscribe(response => {
      console.log('Treatments Response:', response);

      this.treatments = response.treatments;
      this.totalItems = response.totalCount;
    });
  }


  filterTreatments(): Treatment[] {
    return this.treatments.filter(
      (treatment) =>
        treatment.serviceName.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        treatment.treatmentCategoryName.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  toggleActions(treatmentId: number): void {
    this.activeTreatmentId = this.activeTreatmentId === treatmentId ? null : treatmentId;
  }

  deleteTreatment(id: number): void {
    const dialogRef = this.dialog.open(MyDialogConfirmComponent, {
      width: '400px',
      data: {
        title: 'Delete confirmation',
        message: 'Are you sure you want to delete this treatment?',
        confirmButtonText: 'Delete'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.treatmentService.deleteTreatment(id).subscribe(() => {
          this.treatments = this.treatments.filter(t => t.id !== id);
        });
      }
    });
  }

  openCreateTreatmentModal(): void {
    const dialogRef = this.dialog.open(CreateTreatmentComponent, {
      width: '400px',
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadTreatments();
      }
    });
  }

  openUpdateTreatmentModal(treatment: Treatment): void {
    const dialogRef = this.dialog.open(UpdateTreatmentComponent, {
      width: '500px',
      data: { treatment }
    });

    dialogRef.afterClosed().subscribe(updated => {
      if (updated) {
        this.loadTreatments();
      }
    });
  }

  nextPage(): void {
    if (this.currentPage * this.pageSize < this.totalItems) {
      this.currentPage++;
      this.loadTreatments();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadTreatments();
    }
  }

  generatePDF(): void {
    const doc = new jsPDF();

    doc.setFontSize(16);
    doc.text('Treatments Report', 14, 20);

    const tableData = this.filterTreatments().map(treatment => [
      treatment.serviceName,
      treatment.treatmentDescription,
      treatment.treatmentPrice,
      treatment.treatmentCategoryName
    ]);

    autoTable(doc, {
      startY: 30,
      head: [['Service Name', 'Description', 'Price', 'Category']],
      body: tableData
    });

    doc.save('treatment_report.pdf');
  }

  goBack(): void {
    this.location.back();
  }
}
