export interface Reservation {
  id: number;
  userId: string;
  userFirstName: string;
  userLastName: string;
  serviceId: number;
  serviceName: string;
  reservationDate: string;
  reservationStatus: string;
}

export interface ReservationResponse {
  totalCount: number;
  pageSize: number;
  reservations: Reservation[];
}

