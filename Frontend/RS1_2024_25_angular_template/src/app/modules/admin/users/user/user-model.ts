export interface Order {

}

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  dateOfBirth: Date;
  address: string;
  city: string;
  email: string;
  username: string;
  password: string;
  role: string;
  orders: Order[];
}

export interface UserResponse {
  totalCount: number;
  pageSize: number;
  users: User[];
}

