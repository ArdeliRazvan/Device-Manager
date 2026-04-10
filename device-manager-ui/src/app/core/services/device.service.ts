import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Device, DeviceCreateDto, DeviceUpdateDto } from '../models/device.model';

@Injectable({ providedIn: 'root' })
export class DeviceService {
  private readonly apiUrl = '/api/devices';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Device[]> {
    return this.http.get<Device[]>(this.apiUrl);
  }

  getById(id: string): Observable<Device> {
    return this.http.get<Device>(`${this.apiUrl}/${id}`);
  }

  create(dto: DeviceCreateDto): Observable<Device> {
    return this.http.post<Device>(this.apiUrl, dto);
  }

  update(id: string, dto: DeviceUpdateDto): Observable<Device> {
    return this.http.put<Device>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  assign(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/assign`, {});
  }

  unassign(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/unassign`, {});
  }
}