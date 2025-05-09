import { Component, OnInit } from '@angular/core';
import { UsersService } from '../../services/users-service.service';
import { User } from './user-model';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import {CreatePaymentComponent} from '../../payments/create-payment/create-payment.component';
import {MatDialog} from '@angular/material/dialog';
import {CreateUserComponent} from '../create-user/create-user.component';
import {UpdateUserComponent} from '../update-user/update-user.component';
import {MyDialogConfirmComponent} from '../../../shared/dialogs/my-dialog-confirm/my-dialog-confirm.component';
import { Location } from '@angular/common';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
  standalone: false
})
export class UsersComponent implements OnInit {
  users: User[] = [];
  filteredUsers: User[] = [];
  isLoading: boolean = false;
  error: string | null = null;
  searchText: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';
  sortedColumn: string = '';
  sortBy: keyof User = 'firstName';
  showSortDropdown: boolean = false;

  currentPage: number = 1;
  pageSize: number = 10;
  totalItems: number = 0;

  activeUserEmail: string | null = null;

  constructor(
    private usersService: UsersService,
    private dialog: MatDialog,
    private location: Location
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.usersService.getUsers(this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        console.log('Users Response:', response);

        this.users = response.users;
        this.users.forEach(user => {
          user.role = this.getUserRole(user.email);
        });

        this.filteredUsers = [...this.users];
        console.log(this.filteredUsers);

        this.totalItems = response.totalCount;
        this.isLoading = false;
        console.log("Loaded users:", this.users);

      },
      error: (err) => {
        this.error = 'Error loading users';
        this.isLoading = false;
      }
    });

  }

  getUserRole(email: string): string {
    if (email.endsWith('@adminBeautyPoint.com')) {
      return 'Admin';
    } else if (email.endsWith('@employeeBeautyPoint.com')) {
      return 'Employee';
    } else {
      return 'Client';
    }
  }

  onSearchChange(): void {
    this.filteredUsers = this.users.filter(user =>
      user.firstName.toLowerCase().startsWith(this.searchText.toLowerCase())
    );
  }

  toggleSortDropdown(): void {
    this.showSortDropdown = !this.showSortDropdown;
  }

  onSortChange(): void {
    this.sortUsers(this.sortBy);
  }

  sortUsers(column: keyof User): void {
    const direction = this.sortDirection === 'asc' ? 1 : -1;

    this.filteredUsers.sort((a, b) => {
      if (a[column] < b[column]) return -1 * direction;
      if (a[column] > b[column]) return 1 * direction;
      return 0;
    });
    this.sortedColumn = column;
  }

  toggleActions(userEmail: string) {
    this.activeUserEmail = this.activeUserEmail === userEmail ? null : userEmail;
  }

  updateUser(user: User) {
    const dialogRef = this.dialog.open(UpdateUserComponent, {
      width: '500px',
      data: { user }
    });

    dialogRef.afterClosed().subscribe(updated => {
      if (updated) {
        this.loadUsers();
      }
    });
  }

  deleteUser(id: string) {

    console.log("ID koji se šalje:", id);

    const dialogRef = this.dialog.open(MyDialogConfirmComponent, {
      width: '400px',
      data: {
        title: 'Delete confirmation',
        message: 'Are you sure you want to delete this user?',
        confirmButtonText: 'Delete'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.usersService.deleteUser(id).subscribe(() => {
          this.users = this.users.filter(u => u.id !== id);
          this.loadUsers();
        });
      }
    });
  }
  generatePDF(): void {
    const doc = new jsPDF();

    doc.setFontSize(16);
    doc.text('User Profiles Report', 14, 20);

    const tableData = this.filteredUsers.map(user => [
      user.firstName,
      user.lastName,
      user.email,
      user.city,
      user.role
    ]);

    autoTable(doc, {
      startY: 30,
      head: [['First Name', 'Last Name', 'Email', 'City', 'Role']],
      body: tableData
    });

    doc.save('user_profiles_report.pdf');
  }

  nextPage(): void {
    if (this.currentPage * this.pageSize < this.totalItems) {
      this.currentPage++;
      this.loadUsers();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadUsers();
    }
  }

  openCreateUserModal(): void {
    const dialogRef = this.dialog.open(CreateUserComponent, {
      width: '500px',
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((userCreated) => {
      if (userCreated) {
        this.loadUsers();
      }
    });
  }

  goBack(): void {
    this.location.back();
  }
}
