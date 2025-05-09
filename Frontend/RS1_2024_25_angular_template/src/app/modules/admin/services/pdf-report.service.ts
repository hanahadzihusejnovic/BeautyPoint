import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import {MyConfig} from '../../../my-config';

@Injectable({
  providedIn: 'root'
})
export class PdfReportService {
  private apiUrl = `${MyConfig.api_address}/api/pdf-reports`;

  constructor(private http: HttpClient) {}

  uploadPDF(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post(`${this.apiUrl}/upload`, formData);
  }

  getUploadedPDFs(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/list`);
  }

  downloadPDF(fileName: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/download/${fileName}`, {
      responseType: 'blob'
    });
  }
}
