import { Component, OnInit } from '@angular/core';
import { PaymentsService } from '../../services/payments-service.service';
import { Payment, PaymentResponse } from './payment-model';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { PdfReportService } from '../../services/pdf-report.service';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import {MyDialogConfirmComponent} from '../../../shared/dialogs/my-dialog-confirm/my-dialog-confirm.component';
import {CreateReservationComponent} from '../../reservations/create-reservation/create-reservation.component';
import {Reservation} from '../../reservations/reservation/reservation-model';
import {UpdateReservationComponent} from '../../reservations/update-reservation/update-reservation.component';
import {MatDialog} from '@angular/material/dialog';
import {CreatePaymentComponent} from '../create-payment/create-payment.component';
import {UpdateTreatmentComponent} from '../../treatments/update-treatment/update-treatment.component';
import {UpdatePaymentComponent} from '../update-payment/update-payment.component';
import { Location } from '@angular/common';


@Component({
  selector: 'app-payments',
  templateUrl: './payments.component.html',
  styleUrls: ['./payments.component.css'],
  standalone: false
})
export class PaymentsComponent implements OnInit {
  payments: Payment[] = [];
  filteredPayments: Payment[] = [];
  isLoading = true;
  error = '';
  searchText: string = '';
  sortBy: keyof Payment = 'orderId';
  sortDirection: 'asc' | 'desc' = 'asc';
  showSortDropdown: boolean = false;
  sortedColumn: string = '';

  pdfUrl: SafeResourceUrl | null = null;
  uploadedFileName: string = '';

  selectedFile: File | null = null;
  uploadedFiles: string[] = [];

  openDropdown: string | null = null;

  currentPage: number = 1;
  pageSize: number = 10;
  totalItems: number = 0;

  constructor(private paymentsService: PaymentsService, private sanitizer: DomSanitizer, private pdfReportService: PdfReportService,
              private dialog: MatDialog, private location: Location) {}

  ngOnInit(): void {
    this.loadPayments();
    this.fetchUploadedPDFs();
  }

  loadPayments(): void {
    this.isLoading = true;
    this.paymentsService.getPayments(this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        console.log('Payments Response:', response);

        this.payments = response.payments.map(payment => {
          if (payment.paymentDate) {
            payment.paymentDate = payment.paymentDate.split('.')[0];
          }
          return payment;
        });

        this.filteredPayments = [...this.payments];
        this.totalItems = response.totalCount;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = 'Failed to load payments data.';
        this.isLoading = false;
        console.error(err);
      },
    });
  }


  onSearchChange(): void {
    const lowerCaseSearchText = this.searchText.toLowerCase();
    this.filteredPayments = this.payments.filter(payment =>
      payment.orderId.toString().startsWith(lowerCaseSearchText) ||
      payment.amount.toString().startsWith(lowerCaseSearchText) ||
      payment.paymentStatus.toLowerCase().startsWith(lowerCaseSearchText)
    );
  }

  toggleSortDropdown(): void {
    this.showSortDropdown = !this.showSortDropdown;
  }

  onSortChange(): void {
    this.sortPayments(this.sortBy);
  }

  sortPayments(column: keyof Payment): void {
    const direction = this.sortDirection === 'asc' ? 1 : -1;
    this.filteredPayments = [...this.payments].sort((a, b) => {
      const aValue = a[column];
      const bValue = b[column];

      let comparison = 0;

      if (column === 'paymentDate') {
        const dateA = new Date(aValue);
        const dateB = new Date(bValue);
        comparison = dateA.getTime() < dateB.getTime() ? -1 : dateA.getTime() > dateB.getTime() ? 1 : 0;
      } else if (typeof aValue === 'string' && typeof bValue === 'string') {
        comparison = aValue.toLowerCase() < bValue.toLowerCase() ? -1 : aValue.toLowerCase() > bValue.toLowerCase() ? 1 : 0;
      } else if (typeof aValue === 'number' && typeof bValue === 'number') {
        comparison = aValue < bValue ? -1 : aValue > bValue ? 1 : 0;
      }

      return comparison * direction;
    });
    this.sortedColumn = column;
  }

  generatePDF(): void {
    const doc = new jsPDF();
    doc.setFontSize(16);
    doc.text('Payments Report', 14, 20);

    const tableData = this.filteredPayments.map(payment => [
      payment.orderId,
      payment.paymentDate,
      `${payment.amount} BAM`,
      payment.paymentMethod,
      payment.paymentStatus,
      payment.transactionId
    ]);

    autoTable(doc, {
      startY: 30,
      head: [['Order ID', 'Payment Date', 'Amount', 'Method', 'Status', 'Transaction ID']],
      body: tableData
    });

    doc.save('payments_report.pdf');
  }

  onFileSelect(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      this.loadPdf(file);
      this.selectedFile = file;
    }
  }

  onFileDrop(event: DragEvent): void {
    event.preventDefault();
    if (event.dataTransfer?.files.length) {
      const file = event.dataTransfer.files[0];
      this.loadPdf(file);
      this.selectedFile = file;
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    (event.currentTarget as HTMLElement).classList.add('drag-over');
  }

  onDragLeave(): void {
    document.querySelector('.drag-drop-container')?.classList.remove('drag-over');
  }

  loadPdf(file: File): void {
    if (file.type === 'application/pdf') {
      const reader = new FileReader();
      reader.onload = (e) => {
        const unsafeUrl = e.target?.result as string;
        this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(unsafeUrl);
        this.uploadedFileName = file.name;
      };
      reader.readAsDataURL(file);
    } else {
      alert('Only PDF files are allowed!');
    }
  }


  uploadPDF(): void {
    if (!this.selectedFile) {
      alert('Please select a PDF file first.');
      return;
    }

    this.pdfReportService.uploadPDF(this.selectedFile).subscribe({
      next: (response) => {
        console.log('Upload successful:', response);
        alert('File uploaded successfully!');

        this.fetchUploadedPDFs();
      },
      error: (error) => {
        console.error('Upload failed:', error);
        alert('Upload failed. Please try again.');
      }
    });
  }

  fetchUploadedPDFs(): void {
    this.pdfReportService.getUploadedPDFs().subscribe({
      next: (files:any) => {
        console.log('Fetched PDF list:', files);

        this.uploadedFiles = files.$values || [];

        console.log('Processed PDF list:', this.uploadedFiles);
      },
      error: (error) => {
        console.error('Failed to fetch uploaded PDFs:', error);
      }
    });
  }

  downloadPdf(fileName: string): void {
    this.pdfReportService.downloadPDF(fileName).subscribe({
      next: (fileBlob) => {
        const blob = new Blob([fileBlob], { type: 'application/pdf' });
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = fileName;
        link.click();
        window.URL.revokeObjectURL(link.href);
      },
      error: (error) => {
        console.error('Failed to download PDF:', error);
        alert('Failed to download file. Please try again.');
      }
    });
  }

  toggleActions(orderId: string) {
    this.openDropdown = this.openDropdown === orderId ? null : orderId;
  }

  editPayment(payment: Payment) {
    const dialogRef = this.dialog.open(UpdatePaymentComponent, {
      width: '500px',
      data: { payment }
    });

    dialogRef.afterClosed().subscribe(updated => {
      if (updated) {
        this.loadPayments();
      }
    });
  }

  deletePayment(id: number) {
    const dialogRef = this.dialog.open(MyDialogConfirmComponent, {
      width: '400px',
      data: {
        title: 'Delete confirmation',
        message: 'Are you sure you want to delete this payment?',
        confirmButtonText: 'Delete'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.paymentsService.deletePayment(id).subscribe(() => {
          this.payments = this.payments.filter(p => p.id !== id);
          this.loadPayments();
        });
      }
    });
  }

  nextPage(): void {
    if (this.currentPage * this.pageSize < this.totalItems) {
      this.currentPage++;
      this.loadPayments();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadPayments();
    }
  }

  openCreatePaymentModal(): void {
    const dialogRef = this.dialog.open(CreatePaymentComponent, {
      width: '500px',
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((paymentCreated) => {
      if (paymentCreated) {
        this.loadPayments();
      }
    });
  }

  goBack(): void {
    this.location.back();
  }

}
