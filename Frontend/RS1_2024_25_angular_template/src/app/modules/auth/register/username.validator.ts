import { AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { map, catchError, debounceTime, switchMap, of } from 'rxjs';
import {UserService} from '../../../services/user.service';

export function usernameExistsValidator(userService: UserService): AsyncValidatorFn {
  return (control: AbstractControl) => {
    if (!control.value) return of(null);
    return of(control.value).pipe(
      debounceTime(200),
      switchMap(username =>
        userService.checkUsernameExists(username).pipe(
          map(exists => (exists ? { usernameTaken: true } : null)),
          catchError(() => of(null))
        )
      )
    );
  };
}
