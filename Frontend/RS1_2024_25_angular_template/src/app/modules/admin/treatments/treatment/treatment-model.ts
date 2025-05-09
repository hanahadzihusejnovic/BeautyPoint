export interface Treatment {
  id: number;
  serviceName: string;
  treatmentDescription: string;
  treatmentPrice: number;
  treatmentCategoryName: string;
  treatmentCategoryId: number;
}

export interface TreatmentResponse {
  totalCount: number;
  pageSize: number;
  treatments: Treatment[];
}

