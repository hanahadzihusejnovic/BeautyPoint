import { Component, ChangeDetectorRef, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TreatmentService } from '../../services/treatment-service.service';
import { Treatment } from '../treatment/treatment-model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-update-treatment',
  templateUrl: './update-treatment.component.html',
  standalone: false,
  styleUrls: ['./update-treatment.component.css']
})
export class UpdateTreatmentComponent implements OnInit {
  treatmentForm!: FormGroup;
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  currentTreatment: Treatment;
  categories: any[] = [];

  constructor(
    private fb: FormBuilder,
    private treatmentService: TreatmentService,
    private cd: ChangeDetectorRef,
    private dialogRef: MatDialogRef<UpdateTreatmentComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { treatment: Treatment }
  ) {
    this.currentTreatment = data.treatment;
  }

  ngOnInit(): void {
    console.log('Current Treatment:', this.currentTreatment);

    this.treatmentForm = this.fb.group({
      serviceName: [this.currentTreatment.serviceName, Validators.required],
      treatmentDescription: [this.currentTreatment.treatmentDescription, Validators.required],
      treatmentPrice: [this.currentTreatment.treatmentPrice, [Validators.required, Validators.min(0)]],
      treatmentCategoryId: [this.currentTreatment.treatmentCategoryId, Validators.required]
    });

    this.loadCategories();
  }

  loadCategories() {
    this.treatmentService.getTreatmentCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.cd.detectChanges();
      },
      error: (error) => console.error('Error fetching categories:', error)
    });
  }

  submitForm(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.treatmentForm.invalid) {
      this.markFormFieldsTouched();
      this.errorMessage = 'Please fill all required fields.';
      return;
    }

    this.isSubmitting = true;

    const updatedTreatment: Treatment = {
      ...this.currentTreatment,
      ...this.treatmentForm.value
    };

    this.treatmentService.updateTreatment(this.currentTreatment.id, updatedTreatment).subscribe({
      next: () => {
        this.successMessage = 'Treatment successfully updated!';
        this.resetForm();
      },
      error: () => {
        this.errorMessage = 'Error updating treatment. Please try again.';
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  markFormFieldsTouched(): void {
    Object.values(this.treatmentForm.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  resetForm(): void {
    this.treatmentForm.reset();
    this.isSubmitting = false;
    setTimeout(() => {
      this.successMessage = '';
    }, 3000);
    this.dialogRef.close(true);
  }

  closeModal(): void {
    this.dialogRef.close(false);
  }
}
