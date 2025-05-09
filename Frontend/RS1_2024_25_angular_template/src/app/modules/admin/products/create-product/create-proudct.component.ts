import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductService } from '../../services/products-service.service';
import { ProductCategory } from '../product/product-category-model';
import { ChangeDetectorRef } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-create-product',
  templateUrl: './create-product.component.html',
  standalone:false,
  styleUrls: ['./create-product.component.css']
})
export class CreateProductComponent implements OnInit {
  productForm!: FormGroup;
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  selectedFile: File | null = null;
  categories: ProductCategory[] = [];

  constructor(private fb: FormBuilder, private productService: ProductService,
              private cd: ChangeDetectorRef, private dialogRef: MatDialogRef<CreateProductComponent>) {}

  ngOnInit() {
    this.productForm = this.fb.group({
      productName: ['', Validators.required],
      productPrice: ['', [Validators.required, Validators.min(0)]],
      volume: ['', Validators.required],
      productCategoryId: ['', Validators.required],
      productDescription: ['', Validators.required],
    });

    this.loadCategories();
  }

  loadCategories() {
    this.productService.getCategories().subscribe({
      next: (categories) => {
        console.log('Fetched categories:', categories);
        this.categories = categories;
        this.cd.detectChanges();
      },
      error: (error) => console.error('Error fetching categories:', error)
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      console.log('Selected file:', this.selectedFile);
    }
  }

  submitForm() {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.productForm.invalid || !this.selectedFile) {
      this.markFormFieldsTouched();
      this.errorMessage = 'Please fill all required fields and upload an image.';
      return;
    }

    this.isSubmitting = true;

    const formData = new FormData();
    formData.append('productName', this.productForm.value.productName);
    formData.append('productPrice', this.productForm.value.productPrice);
    formData.append('productDescription', this.productForm.value.productDescription);
    formData.append('productCategoryId', this.productForm.value.productCategoryId);
    formData.append('productCategoryName', this.getCategoryNameById(this.productForm.value.productCategoryId));
    formData.append('volume', this.productForm.value.volume);
    formData.append('productImage', this.selectedFile);
    formData.append('imagePath', this.selectedFile.name);

    console.log('Final FormData before sending:', formData);

    this.productService.createProduct(formData).subscribe({
      next: (response) => {
        console.log('Product created successfully:', response);
        this.successMessage = 'Product successfully created!';
        this.resetForm();
      },
      error: (error) => {
        console.error('Error creating product:', error);
        this.errorMessage = 'Error creating product. Please try again.';
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  getCategoryNameById(categoryId: any): string {
    const category = this.categories.find(c => c.id == categoryId);
    return category ? category.productCategoryName : 'Unknown Category';
  }

  markFormFieldsTouched() {
    Object.values(this.productForm.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  resetForm() {
    this.productForm.reset();
    this.selectedFile = null;
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
