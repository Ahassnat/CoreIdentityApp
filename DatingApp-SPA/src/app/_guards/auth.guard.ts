import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private toastr: ToastrService,
             private router: Router) {}
  canActivate(next: ActivatedRouteSnapshot):  boolean {
    const roles = next.firstChild.data['roles'] as Array<string>; // if activate the roles can be 'Admin' or 'Modarator'
    if (roles) {
      const match = this.authService.roleMatch(roles);
      if (match) {
        return true;
      } else {
        this.router.navigate(['members']);
        this.toastr.error('you Are not Authorise to access this area', 'Authrize Alert');
      }
    }

    if (this.authService.loggedIn()) {
      return true;
    }
    this.toastr.error('you shall not Pass !!', 'Error Log');
    this.router.navigate(['/home']);
    return false;
  }
}
