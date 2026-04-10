import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DeviceService } from '../../../core/services/device.service';
import { Device } from '../../../core/models/device.model';

@Component({
  selector: 'app-device-form',
  templateUrl: './device-form.component.html',
  styleUrls: ['./device-form.component.scss'],
  standalone: false
})
export class DeviceFormComponent implements OnInit {
  form!: FormGroup;
  isEditMode = false;
  deviceId: string | null = null;
  loading = false;
  submitting = false;
  error = '';
  successMessage = '';

  readonly deviceTypes = ['phone', 'tablet'];
  readonly ramOptions = [1, 2, 4, 6, 8, 12, 16, 32, 64];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private deviceService: DeviceService
  ) {}

  ngOnInit(): void {
    this.buildForm();

    this.deviceId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.deviceId && this.deviceId !== 'new';

    if (this.isEditMode) {
      this.loadDevice(this.deviceId!);
    }
  }

  private buildForm(): void {
    this.form = this.fb.group({
      name:         ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      manufacturer: ['', Validators.required],
      type:         ['phone', Validators.required],
      os:           ['', Validators.required],
      osVersion:    ['', Validators.required],
      processor:    ['', Validators.required],
      ram:          [8, [Validators.required, Validators.min(1), Validators.max(64)]],
      description:  ['']
    });
  }

  private loadDevice(id: string): void {
    this.loading = true;
    this.deviceService.getById(id).subscribe({
      next: (device: Device) => {
        // Populăm formularul cu datele existente
        this.form.patchValue({
          name:         device.name,
          manufacturer: device.manufacturer,
          type:         device.type,
          os:           device.os,
          osVersion:    device.osVersion,
          processor:    device.processor,
          ram:          device.ram,
          description:  device.description
        });
        this.loading = false;
      },
      error: () => {
        this.error = 'Nu s-au putut încărca datele dispozitivului.';
        this.loading = false;
      }
    });
  }

  // Getter rapid pentru câmpuri — folosit în template pentru validare
  get f() { return this.form.controls; }

  isFieldInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  getFieldError(field: string): string {
    const control = this.form.get(field);
    if (!control || !control.errors) return '';

    if (control.errors['required'])   return 'This field is required.';
    if (control.errors['minlength'])  return `Minimum ${control.errors['minlength'].requiredLength} characters.`;
    if (control.errors['maxlength'])  return `Maximum ${control.errors['maxlength'].requiredLength} characters.`;
    if (control.errors['min'])        return `Minimum value is ${control.errors['min'].min}.`;
    if (control.errors['max'])        return `Maximum value is ${control.errors['max'].max}.`;
    if (control.errors['duplicate'])  return 'A device with this name already exists.';

    return 'Invalid value.';
  }

  onSubmit(): void {
    // Marchăm toate câmpurile ca touched pentru a afișa erorile
    this.form.markAllAsTouched();

    if (this.form.invalid) return;

    this.submitting = true;
    this.error = '';

    const formValue = this.form.value;

    if (this.isEditMode) {
      this.deviceService.update(this.deviceId!, formValue).subscribe({
        next: () => {
          this.successMessage = 'Device updated successfully!';
          this.submitting = false;
          setTimeout(() => this.router.navigate(['/devices', this.deviceId]), 1000);
        },
        error: (err) => {
          this.error = err.status === 409
            ? 'A device with this name already exists.'
            : 'Update failed. Please try again.';
          this.submitting = false;
        }
      });
    } else {
      this.deviceService.create(formValue).subscribe({
        next: (created) => {
          this.successMessage = 'Device created successfully!';
          this.submitting = false;
          setTimeout(() => this.router.navigate(['/devices', created.id]), 1000);
        },
        error: (err) => {
          if (err.status === 409) {
            // Setăm eroarea direct pe câmpul name
            this.form.get('name')?.setErrors({ duplicate: true });
          } else if (err.status === 400) {
            this.error = 'Please check all fields and try again.';
          } else {
            this.error = 'Creation failed. Please try again.';
          }
          this.submitting = false;
        }
      });
    }
  }

  cancel(): void {
    if (this.isEditMode) {
      this.router.navigate(['/devices', this.deviceId]);
    } else {
      this.router.navigate(['/devices']);
    }
  }
}