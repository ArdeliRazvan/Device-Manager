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
  users: User[] = []; 
  selectedUserId: string = ''; 
  isAssigning: boolean = false;
  loading: boolean = false;
  error: string = '';
  successMessage: string = '';
  showDeleteConfirm: boolean = false;

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
    if (this.authService.isAdmin) {
      this.loadUsers();
    }
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
          this.assignedUser = null;
          this.loading = false;
        }
      },
      error: () => { this.error = 'Device not found.'; this.loading = false; }
    });
  }

  private loadUsers(): void {
    this.userService.getAll().subscribe({
      next: (data) => this.users = data,
      error: () => console.error('Error loading users')
    });
  }

  startAssign(): void {
    this.isAssigning = true;
  }

  cancelAssign(): void {
    this.isAssigning = false;
    this.selectedUserId = '';
  }

  confirmAssign(): void {
    if (!this.selectedUserId || !this.device) return;
    this.loading = true;
    this.deviceService.assign(this.device.id, this.selectedUserId).subscribe({
      next: () => {
        this.successMessage = 'Assigned successfully!';
        this.isAssigning = false;
        this.selectedUserId = '';
        this.loadDevice(this.device!.id);
        setTimeout(() => this.successMessage = '', 3000);
      },
      error: () => {
        this.error = 'Assignment failed.';
        this.loading = false;
      }
    });
  }

  unassign(): void {
    if (!this.device || !confirm('Are you sure?')) return;
    this.loading = true;
    this.deviceService.unassign(this.device.id).subscribe({
      next: () => {
        this.successMessage = 'Unassigned successfully.';
        this.loadDevice(this.device!.id);
        setTimeout(() => this.successMessage = '', 3000);
      },
      error: () => { this.error = 'Unassignment failed.'; this.loading = false; }
    });
  }

  canAssign(): boolean {
    return this.authService.isAdmin && !this.device?.assignedUserId;
  }

  canUnassign(): boolean {
    if (!this.device?.assignedUserId) return false;
    return this.authService.isAdmin || this.device.assignedUserId === this.authService.currentUser?.userId;
  }

  editDevice(): void { this.router.navigate(['/devices', this.device!.id, 'edit']); }
  confirmDelete(): void { this.showDeleteConfirm = true; }
  cancelDelete(): void { this.showDeleteConfirm = false; }
  deleteDevice(): void {
    this.deviceService.delete(this.device!.id).subscribe({
      next: () => this.router.navigate(['/devices']),
      error: () => { this.error = 'Delete failed.'; this.showDeleteConfirm = false; }
    });
  }
  goBack(): void { this.router.navigate(['/devices']); }
}