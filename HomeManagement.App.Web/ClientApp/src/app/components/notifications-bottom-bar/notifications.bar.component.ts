import { Component, OnInit } from "@angular/core";
import { NotificationService } from "src/app/api/main/notification.service";
import { Notification } from "./../../models/notification";
import { MatBottomSheetRef } from "@angular/material/bottom-sheet";

@Component({
    selector: 'notifications-bottom',
    templateUrl: 'notifications.bar.component.html'
})
export class NotificationsBottomBarComponent implements OnInit{

    notifications: Array<Notification> = [];
    constructor(private notificationService: NotificationService,
        private bottomSheetRef: MatBottomSheetRef<NotificationsBottomBarComponent>){
        
    }

    ngOnInit(): void {
        this.loadNotifications();
    }

    loadNotifications(){
        this.notifications.splice(0, this.notifications.length);
        this.notificationService.get().subscribe(notif => {
            notif.forEach(n => {
                this.notifications.push(n);
            });
        });
    }

    openLink(data:any){
        console.log(data);
        this.bottomSheetRef.dismiss();
        event.preventDefault();
    }

    getDay(day:number){
        switch (day) {
            case 1:
                return 1 + 'st';
            case 2:
                return 2 + 'nd';
            case 3:
                return 3 + 'rd';
            default:
                return day + 'th';
        }
    }
}