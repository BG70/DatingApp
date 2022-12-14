import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  module: any = {}

  constructor(public accountService: AccountService, private router: Router
    , private toastr: ToastrService) { }

  ngOnInit(): void {

  }

  login()
  {
    //console.log(this.module);
    this.accountService.login(this.module).subscribe(response =>{
      //console.log(response);
      this.router.navigateByUrl('/members');
    });
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
