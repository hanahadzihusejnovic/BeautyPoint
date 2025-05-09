import {Reservation} from './reservation-model';

export interface User {
  id:string;
  firstName: string;
  lastName: string;
  email: string;
  reservations: Reservation[];
}
