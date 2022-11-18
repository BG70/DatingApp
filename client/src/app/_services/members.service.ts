import { HttpClient, HttpParams } from '@angular/common/http'; // , HttpHeaders
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
//import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';

// const httpOptions = {
//   headers: new HttpHeaders({
//     Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user'))?.token 
//   })
// }

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user: User;    
  userParams: UserParams;


  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })     
   }

   getUserParams(){
    return this.userParams;
   }

   setUserParams(params: UserParams){
    this.userParams = params;
   }

   resetUserParams(){
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }   

  getMembers(userParams: UserParams) { // page?: number, itemsPerPage?: number
    // if(this.members.length>0) return of(this.members); // אם שלפנו לא נשלוף שוב
    // return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
    //   map(members => {
    //     this.members = members;
    //     return members;
    //   })
    // ); // , httpOptions

    var response = this.memberCache.get(Object.values(userParams).join('-')); // האם יש לנו תשובה בCache
    if(response){
      return of(response);
    }

    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);    
    params = params.append('orderBy', userParams.orderBy);    

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params)
      .pipe(map(response => { // Cache חלק הזה שומר מערך ב
        this.memberCache.set(Object.values(userParams).join('-'),response);
        return response;
      }))
  }

 
 
  getMember(username: string) {
    //const member = this.members.find(x => x.username === username);
    //if(member !== undefined) return of(member);

    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), []) // מאחד קאש של כל השליפות-מערכים למערך אחד
      .find((member: Member) => member.username === username);

    if(member){
      return of(member);
    }
    return this.http.get<Member>(this.baseUrl + 'users/' + username); // , httpOptions
  }

  updateMember(member: Member){
   // return this.http.put(this.baseUrl + 'users', member);
   return this.http.put(this.baseUrl + 'users', member).pipe(
    map(() =>{ // כי השרות לא  מחזיר כלום
      const index = this.members.indexOf(member); // מצא אותו במערך שלנונ
      this.members[index] = member;       
    }) 
   )
  }

  private getPaginatedResult<T>(url, params) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      })
    );
  }   

  private getPaginationHeaders(pageNumber: number, pageSize: number){
    let params = new HttpParams();
    
    params = params.append('pageNumber', pageNumber.toString()); // from quaryString
    params = params.append('pageSize', pageSize.toString());   // from quaryString

    return params;
  }


}
