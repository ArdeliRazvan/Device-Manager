export interface RegisterDto {
  name: string;
  email: string;
  password: string;
  location: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  userId: string;
  name: string;
  email: string;
  role: string;
}

export interface CurrentUser {
  userId: string;
  name: string;
  email: string;
  role: string;
}