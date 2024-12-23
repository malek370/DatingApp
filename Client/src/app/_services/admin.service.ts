import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { UserRoles } from '../_models/UsersRoles';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl=environment.apiUrl;
  private http=inject(HttpClient);

  getUserWithRoles(){
    return this.http.get<UserRoles[]>(this.baseUrl+"admin/users-with-roles");
  }
  updateUserRolse(username:string,roles:string[]){
    return this.http.post<string[]>(this.baseUrl+"admin/edit-roles/"+username+"?roles="+roles,
      {});
  }
}