import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DeviceService } from '../../../core/services/device.service';
import { UserService } from '../../../core/services/user.service';
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
  users: Map<string, string> = new Map(); // id -> nume user
  loading = false;
  error = '';
  deleteConfirmId: string | null = null; // id-ul device-ului în așteptare de confirmare

  constructor(
    private deviceService: DeviceService,
    private userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    this.error = '';

    // Încarc users întâi, apoi devices
    this.userService.getAll().subscribe({
      next: (users: User[]) => {
        users.forEach(u => this.users.set(u.id, u.name));
        this.loadDevices();
      },
      error: () => {
        // Dacă users eșuează, încarc devices oricum
        this.loadDevices();
      }
    });
  }

  private loadDevices(): void {
    this.deviceService.getAll().subscribe({
      next: (devices: Device[]) => {
        // Atașez numele userului la fiecare device
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
    event.stopPropagation(); // nu naviga la detalii
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