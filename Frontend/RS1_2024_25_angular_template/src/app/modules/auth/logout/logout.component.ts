import {Component, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Router} from '@angular/router';
import {MyConfig} from '../../../my-config';
import {MyAuthService} from '../../../services/auth-services/my-auth.service';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css'],
  standalone: false
})
export class LogoutComponent implements OnInit {

  constructor(
    private httpClient: HttpClient,
    private authService: MyAuthService,
    private router: Router
  ) {
  }

  ngOnInit(): void {
    this.logout();
  }

  logout(): void {
    this.handleLogoutSuccessOrError()
  }

  private handleLogoutSuccessOrError(): void {
    this.authService.setLoggedInUser(null);
    setTimeout(() => {
      this.router.navigate(['/login']);
    }, 3000);
  }
}
