import {ChangeDetectorRef, Component, Inject, OnInit} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductService} from '../../../services/products-service.service';
import { ProductCategory} from '../../product/product-category-model';
import { Product} from '../../product/product-model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import {MyConfig} from '../../../../../my-config';

@Component({
  selector: 'app-update-product',
  templateUrl: './update-product.component.html',
  standalone:false,
  styleUrls: ['./update-product.component.css']
})
export class UpdateProductComponent implements OnInit {
  productForm!: FormGroup;
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';
  selectedFile: File | null = null;
  categories: any[] = [];
  currentProduct: Product;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private cd: ChangeDetectorRef,
    private dialogRef: MatDialogRef<UpdateProductComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { product: Product }
  ) {
    this.currentProduct = data.product;
  }

  ngOnInit() {
    this.productForm = this.fb.group({
      productName: [this.currentProduct.productName, Validators.required],
      productPrice: [this.currentProduct.productPrice, [Validators.required, Validators.min(0)]],
      volume: [this.currentProduct.volume, Validators.required],
      productCategoryId: [this.currentProduct.productCategoryId, Validators.required],
      productDescription: [this.currentProduct.productDescription, Validators.required],
      imagePath: [this.currentProduct.imagePath]
    });

    this.selectedFile = this.currentProduct.imagePath ? new File([], this.currentProduct.imagePath) : null;

    console.log('Current ImagePath:', this.currentProduct.imagePath);
    this.loadCategories();
    }


  loadCategories(): void {
    this.productService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.cd.detectChanges();
      },
      error: (error) => console.error('Error fetching categories:', error)
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      console.log('Selected file:', this.selectedFile);


      this.productForm.patchValue({
        productImage: this.selectedFile
      });
    }
  }

  submitForm(): void {
    this.successMessage = '';
    this.errorMessage = '';


    if (this.productForm.invalid) {
      this.markFormFieldsTouched();
      this.errorMessage = 'Please fill all required fields.';
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

    if (this.selectedFile) {
      formData.append('productImage', this.selectedFile);
      formData.append('imagePath', this.selectedFile.name);
    } else {
      formData.append('imagePath', this.currentProduct.imagePath);
    }

    formData.forEach((value, key) => {
      console.log(key + ': ' + value);
    });

    console.log('Final FormData before sending:', formData);

    this.productService.updateProduct(this.currentProduct.id, formData).subscribe({
      next: (response) => {
        this.successMessage = 'Product successfully updated!';
        this.resetForm();
      },
      error: (error) => {
        this.errorMessage = 'Error updating product. Please try again.';
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

  getImageUrl(imagePath: string): string {
    return `${MyConfig.api_address}${imagePath}`;
  }

  markFormFieldsTouched(): void {
    Object.values(this.productForm.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  resetForm(): void {
    this.productForm.reset();
    this.selectedFile = null;
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
