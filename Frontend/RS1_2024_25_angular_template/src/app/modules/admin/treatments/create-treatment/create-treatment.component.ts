import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TreatmentService } from '../../services/treatment-service.service';
import { TreatmentCategory } from '../treatment/treatment-category-model';
import { ChangeDetectorRef } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-create-treatment',
  templateUrl: './create-treatment.component.html',
  standalone: false,
  styleUrls: ['./create-treatment.component.css']
})
export class CreateTreatmentComponent implements OnInit {
  treatmentForm!: FormGroup;
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  categories: TreatmentCategory[] = [];

  constructor(
    private fb: FormBuilder,
    private treatmentService: TreatmentService,
    private cd: ChangeDetectorRef,
    private dialogRef: MatDialogRef<CreateTreatmentComponent>
  ) {}

  ngOnInit() {
    this.treatmentForm = this.fb.group({
      serviceName: ['', Validators.required],
      treatmentPrice: ['', [Validators.required, Validators.min(0)]],
      treatmentDescription: ['', Validators.required],
      treatmentCategoryId: ['', Validators.required]
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

  submitForm() {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.treatmentForm.invalid) {
      this.markFormFieldsTouched();
      this.errorMessage = 'Please fill all required fields.';
      return;
    }

    this.isSubmitting = true;

    const treatmentData = {
      serviceName: this.treatmentForm.value.serviceName,
      treatmentPrice: this.treatmentForm.value.treatmentPrice,
      treatmentDescription: this.treatmentForm.value.treatmentDescription,
      treatmentCategoryId: parseInt(this.treatmentForm.value.treatmentCategoryId, 10),
      treatmentCategoryName: this.getCategoryNameById(this.treatmentForm.value.treatmentCategoryId)
    };

    console.log('JSON to be sent:', treatmentData);

    this.treatmentService.createTreatment(treatmentData).subscribe({
      next: () => {
        this.successMessage = 'Treatment successfully created!';
        this.resetForm();
      },
      error: () => {
        this.errorMessage = 'Error creating treatment. Please try again.';
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  getCategoryNameById(categoryId: any): string {
    const category = this.categories.find(c => c.id == categoryId);
    return category ? category.categoryName : 'Unknown Category';
  }

  markFormFieldsTouched() {
    Object.values(this.treatmentForm.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  resetForm() {
    this.treatmentForm.reset();
    this.isSubmitting = false;
    setTimeout(() => {
      this.successMessage = '';
    }, 3000);
    this.dialogRef.close(true);
  }

  closeModal() {
    this.dialogRef.close(false);
  }
}
