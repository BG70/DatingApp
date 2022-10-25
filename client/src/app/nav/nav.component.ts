import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable } from 'rxjs';
import { User } from '../_models/user';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  module: any = {}

  constructor(public accountService: AccountService) { }

  ngOnInit(): void {

  }

  login()
  {
    //console.log(this.module);
    this.accountService.login(this.module).subscribe(response =>{
      console.log(response);
    }, error => {
      console.log(error);
    });
  }

  logout(){
    this.accountService.logout();
  }

}
