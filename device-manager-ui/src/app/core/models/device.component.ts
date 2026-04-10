export interface Device {
  id: string;
  name: string;
  manufacturer: string;
  type: 'phone' | 'tablet';
  os: string;
  osVersion: string;
  processor: string;
  ram: number;
  description: string;
  assignedUserId?: string;
  assignedUserName?: string; // populat în frontend prin join cu users
  createdAt: Date;
  updatedAt: Date;
}

export interface DeviceCreateDto {
  name: string;
  manufacturer: string;
  type: string;
  os: string;
  osVersion: string;
  processor: string;
  ram: number;
  description: string;
}

export interface DeviceUpdateDto {
  name?: string;
  manufacturer?: string;
  type?: string;
  os?: string;
  osVersion?: string;
  processor?: string;
  ram?: number;
  description?: string;
}