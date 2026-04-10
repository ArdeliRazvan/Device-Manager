import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DeviceService } from '../../../core/services/device.service';
import { UserService } from '../../../core/services/user.service';
import { AuthService } from '../../../core/services/auth.service';
import { Device } from '../../../core/models/device.model';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-device-detail',
  templateUrl: './device-detail.component.html',
  styleUrls: ['./device-detail.component.scss']
})
export class DeviceDetailComponent implements OnInit {
  device: Device | null = null;
  assignedUser: User | null = null;
  loading = false;
  error = '';
  successMessage = '';
  showDeleteConfirm = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private deviceService: DeviceService,
    private userService: UserService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadDevice(id);
  }

  private loadDevice(id: string): void {
    this.loading = true;
    this.deviceService.getById(id).subscribe({
      next: (device: Device) => {
        this.device = device;
        if (device.assignedUserId) {
          this.userService.getById(device.assignedUserId).subscribe({
            next: (user: User) => { this.assignedUser = user; this.loading = false; },
            error: () => { this.loading = false; }
          });
        } else {
          this.loading = false;
        }
      },
      error: () => { this.error = 'Device not found.'; this.loading = false; }
    });
  }

  canAssign(): boolean {
    return !!this.authService.currentUser && !this.device?.assignedUserId;
  }

  canUnassign(): boolean {
    return this.device?.assignedUserId === this.authService.currentUser?.userId;
  }

  assign(): void {
    this.deviceService.assign(this.device!.id).subscribe({
      next: () => {
        this.successMessage = 'Device assigned to you!';
        this.loadDevice(this.device!.id);
      },
      error: () => { this.error = 'Could not assign device.'; }
    });
  }

  unassign(): void {
    this.deviceService.unassign(this.device!.id).subscribe({
      next: () => {
        this.successMessage = 'Device unassigned.';
        this.assignedUser = null;
        this.device!.assignedUserId = undefined;
      },
      error: () => { this.error = 'Could not unassign device.'; }
    });
  }

  editDevice(): void { this.router.navigate(['/devices', this.device!.id, 'edit']); }
  confirmDelete(): void { this.showDeleteConfirm = true; }
  cancelDelete(): void  { this.showDeleteConfirm = false; }

  deleteDevice(): void {
    this.deviceService.delete(this.device!.id).subscribe({
      next: () => this.router.navigate(['/devices']),
      error: () => { this.error = 'Delete failed.'; this.showDeleteConfirm = false; }
    });
  }

  goBack(): void { this.router.navigate(['/devices']); }
}