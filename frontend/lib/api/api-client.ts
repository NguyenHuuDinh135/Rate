// ============================================
// API Client with Auth Interceptors
// ============================================

import { API_CONFIG, AUTH_CONFIG, HTTP_STATUS } from "@/lib/constants"
import type { ApiError, ApiResponse, AuthTokens } from "@/types/auth"

type RequestMethod = "GET" | "POST" | "PUT" | "DELETE" | "PATCH"

interface RequestOptions {
  method?: RequestMethod
  headers?: Record<string, string>
  body?: unknown
  credentials?: RequestCredentials
  skipAuth?: boolean
}

// Token storage utilities (in-memory for security)
let accessToken: string | null = null
let refreshPromise: Promise<AuthTokens | null> | null = null

export const tokenStorage = {
  getAccessToken: () => accessToken,
  setAccessToken: (token: string | null) => {
    accessToken = token
  },
  clear: () => {
    accessToken = null
  },
}

// Parse API error response
async function parseError(response: Response): Promise<ApiError> {
  try {
    const data = await response.json()
    return {
      message: data.message || "An error occurred",
      code: data.code,
      status: response.status,
      errors: data.errors,
    }
  } catch {
    return {
      message: response.statusText || "Network error",
      status: response.status,
    }
  }
}

// Refresh access token
async function refreshAccessToken(): Promise<AuthTokens | null> {
  try {
    const response = await fetch(
      `${API_CONFIG.BASE_URL}${AUTH_CONFIG.ENDPOINTS.REFRESH}`,
      {
        method: "POST",
        credentials: "include", // Include httpOnly refresh token cookie
        headers: {
          "Content-Type": "application/json",
        },
      }
    )

    if (!response.ok) {
      tokenStorage.clear()
      return null
    }

    const data: ApiResponse<AuthTokens> = await response.json()
    tokenStorage.setAccessToken(data.data.accessToken)
    return data.data
  } catch {
    tokenStorage.clear()
    return null
  }
}

// Main API client
export async function apiClient<T>(
  endpoint: string,
  options: RequestOptions = {}
): Promise<T> {
  const {
    method = "GET",
    headers = {},
    body,
    credentials = "include",
    skipAuth = false,
  } = options

  const url = endpoint.startsWith("http")
    ? endpoint
    : `${API_CONFIG.BASE_URL}${endpoint}`

  const requestHeaders: Record<string, string> = {
    "Content-Type": "application/json",
    ...headers,
  }

  // Add auth header if we have a token
  if (!skipAuth && accessToken) {
    requestHeaders["Authorization"] = `Bearer ${accessToken}`
  }

  const config: RequestInit = {
    method,
    headers: requestHeaders,
    credentials,
  }

  if (body && method !== "GET") {
    config.body = JSON.stringify(body)
  }

  let response = await fetch(url, config)

  // Handle 401 - Try to refresh token
  if (response.status === HTTP_STATUS.UNAUTHORIZED && !skipAuth) {
    // Use existing refresh promise or create new one (prevent race conditions)
    if (!refreshPromise) {
      refreshPromise = refreshAccessToken()
    }

    const newTokens = await refreshPromise
    refreshPromise = null

    if (newTokens) {
      // Retry original request with new token
      requestHeaders["Authorization"] = `Bearer ${newTokens.accessToken}`
      response = await fetch(url, {
        ...config,
        headers: requestHeaders,
      })
    } else {
      // Refresh failed - throw error to trigger logout
      const error: ApiError = {
        message: "Session expired. Please login again.",
        status: HTTP_STATUS.UNAUTHORIZED,
        code: "SESSION_EXPIRED",
      }
      throw error
    }
  }

  if (!response.ok) {
    const error = await parseError(response)
    throw error
  }

  // Handle empty responses
  const contentType = response.headers.get("content-type")
  if (!contentType || !contentType.includes("application/json")) {
    return {} as T
  }

  return response.json()
}

// Convenience methods
export const api = {
  get: <T>(endpoint: string, options?: Omit<RequestOptions, "method" | "body">) =>
    apiClient<T>(endpoint, { ...options, method: "GET" }),

  post: <T>(endpoint: string, body?: unknown, options?: Omit<RequestOptions, "method">) =>
    apiClient<T>(endpoint, { ...options, method: "POST", body }),

  put: <T>(endpoint: string, body?: unknown, options?: Omit<RequestOptions, "method">) =>
    apiClient<T>(endpoint, { ...options, method: "PUT", body }),

  patch: <T>(endpoint: string, body?: unknown, options?: Omit<RequestOptions, "method">) =>
    apiClient<T>(endpoint, { ...options, method: "PATCH", body }),

  delete: <T>(endpoint: string, options?: Omit<RequestOptions, "method">) =>
    apiClient<T>(endpoint, { ...options, method: "DELETE" }),
}

export default api
