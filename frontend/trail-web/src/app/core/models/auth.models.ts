export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  username: string;
  email: string;
  password: string;
}

export interface UserProfile {
  id: number;
  username: string;
  email: string;
  role: 'user' | 'guide' | 'admin';
}

export interface AuthResponse {
  token: string;
  user: UserProfile;
}