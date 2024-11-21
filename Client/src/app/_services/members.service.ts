import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/Member';
import { AccountsService } from './accounts.service';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

private http=inject(HttpClient);
baseUrl=environment.apiUrl;
private accountService = inject(AccountsService); 
getMembers(){
  return this.http.get<Member[]>(this.baseUrl+"User");
}
getMember(username:string){
  return this.http.get<Member>(this.baseUrl+"User/"+username);
}

}