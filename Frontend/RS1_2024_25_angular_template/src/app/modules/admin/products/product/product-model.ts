export interface Product {
  id: number;
  productName: string;
  productPrice: number;
  productDescription: string;
  productCategoryId: number;
  productCategoryName: string;
  imagePath: string;
  volume: number;
}

export interface ProductResponse {
  totalCount: number;
  pageSize: number;
  products: Product[];
}
