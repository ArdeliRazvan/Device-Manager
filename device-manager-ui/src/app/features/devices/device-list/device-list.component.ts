import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DeviceService } from '../../../core/services/device.service';
import { UserService } from '../../../core/services/user.service';
import { AuthService } from '../../../core/services/auth.service'; // Importă AuthService
import { Device } from '../../../core/models/device.model';
import { User } from '../../../core/models/user.model';

interface DeviceWithUser extends Device {
  assignedUserName?: string;
}

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.scss'],
  standalone: false
})
export class DeviceListComponent implements OnInit {
  devices: Device[] = [];
  users: Map<string, string> = new Map(); 
  loading = false;
  error = '';
  deleteConfirmId: string | null = null; 

  constructor(
    private deviceService: DeviceService,
    private userService: UserService,
    public authService: AuthService, // Schimbat în public pentru HTML
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    this.error = '';

    this.userService.getAll().subscribe({
      next: (users: User[]) => {
        users.forEach(u => this.users.set(u.id, u.name));
        this.loadDevices();
      },
      error: () => {
        this.loadDevices();
      }
    });
  }

  private loadDevices(): void {
    this.deviceService.getAll().subscribe({
      next: (devices: Device[]) => {
        this.devices = devices.map(d => ({
          ...d,
          assignedUserName: d.assignedUserId
            ? (this.users.get(d.assignedUserId) ?? 'Unknown')
            : undefined
        }));
        this.loading = false;
      },
      error: () => {
        this.error = 'Nu s-au putut încărca dispozitivele.';
        this.loading = false;
      }
    });
  }

  viewDetails(id: string): void {
    this.router.navigate(['/devices', id]);
  }

  editDevice(id: string, event: Event): void {
    event.stopPropagation(); 
    this.router.navigate(['/devices', id, 'edit']);
  }

  confirmDelete(id: string, event: Event): void {
    event.stopPropagation();
    this.deleteConfirmId = id;
  }

  cancelDelete(): void {
    this.deleteConfirmId = null;
  }

  deleteDevice(id: string): void {
    // Verificare suplimentară de securitate și în Frontend
    if (!this.authService.isAdmin) {
      this.error = 'Doar administratorii pot șterge dispozitive.';
      return;
    }

    this.deviceService.delete(id).subscribe({
      next: () => {
        this.devices = this.devices.filter(d => d.id !== id);
        this.deleteConfirmId = null;
      },
      error: () => {
        this.error = 'Ștergerea a eșuat. Încearcă din nou.';
        this.deleteConfirmId = null;
      }
    });
  }

  getTypeBadgeClass(type: string): string {
    return type === 'phone' ? 'badge-phone' : 'badge-tablet';
  }
}