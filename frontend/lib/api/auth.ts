// ============================================
// Auth API Functions
// ============================================

import { AUTH_CONFIG } from "@/lib/constants"
import type {
  ApiResponse,
  AuthResponse,
  LoginCredentials,
  RegisterCredentials,
  User,
} from "@/types/auth"
import { api, tokenStorage } from "./api-client"

/**
 * Login user with email and password
 */
export async function loginApi(
  credentials: LoginCredentials
): Promise<AuthResponse> {
  const response = await api.post<ApiResponse<AuthResponse>>(
    AUTH_CONFIG.ENDPOINTS.LOGIN,
    credentials,
    { skipAuth: true }
  )

  // Store access token in memory
  tokenStorage.setAccessToken(response.data.tokens.accessToken)

  return response.data
}

/**
 * Register new user
 */
export async function registerApi(
  credentials: RegisterCredentials
): Promise<AuthResponse> {
  const response = await api.post<ApiResponse<AuthResponse>>(
    AUTH_CONFIG.ENDPOINTS.REGISTER,
    credentials,
    { skipAuth: true }
  )

  // Store access token in memory
  tokenStorage.setAccessToken(response.data.tokens.accessToken)

  return response.data
}

/**
 * Logout user - clears tokens on server and client
 */
export async function logoutApi(): Promise<void> {
  try {
    await api.post(AUTH_CONFIG.ENDPOINTS.LOGOUT)
  } finally {
    // Always clear local token even if server request fails
    tokenStorage.clear()
  }
}

/**
 * Get current user info
 */
export async function getCurrentUser(): Promise<User> {
  const response = await api.get<ApiResponse<User>>(AUTH_CONFIG.ENDPOINTS.ME)
  return response.data
}

/**
 * Refresh authentication - called on app init
 */
export async function refreshAuth(): Promise<AuthResponse | null> {
  try {
    const response = await api.post<ApiResponse<AuthResponse>>(
      AUTH_CONFIG.ENDPOINTS.REFRESH,
      undefined,
      { skipAuth: true }
    )

    tokenStorage.setAccessToken(response.data.tokens.accessToken)
    return response.data
  } catch {
    tokenStorage.clear()
    return null
  }
}

/**
 * Request password reset
 */
export async function forgotPasswordApi(email: string): Promise<void> {
  await api.post(
    AUTH_CONFIG.ENDPOINTS.FORGOT_PASSWORD,
    { email },
    { skipAuth: true }
  )
}

/**
 * Reset password with token
 */
export async function resetPasswordApi(
  token: string,
  password: string
): Promise<void> {
  await api.post(
    AUTH_CONFIG.ENDPOINTS.RESET_PASSWORD,
    { token, password },
    { skipAuth: true }
  )
}
