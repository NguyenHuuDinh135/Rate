// ============================================
// Auth & API Constants
// ============================================

export const AUTH_CONFIG = {
  // Cookie names
  ACCESS_TOKEN_KEY: "access_token",
  REFRESH_TOKEN_KEY: "refresh_token",
  USER_KEY: "user_data",

  // Token expiry
  ACCESS_TOKEN_EXPIRY: 15 * 60 * 1000, // 15 minutes
  REFRESH_TOKEN_EXPIRY: 7 * 24 * 60 * 60 * 1000, // 7 days

  // API endpoints
  ENDPOINTS: {
    LOGIN: "/auth/login",
    LOGOUT: "/auth/logout",
    REGISTER: "/auth/register",
    REFRESH: "/auth/refresh",
    ME: "/auth/me",
    FORGOT_PASSWORD: "/auth/forgot-password",
    RESET_PASSWORD: "/auth/reset-password",
  },

  // Route paths
  ROUTES: {
    LOGIN: "/login",
    SIGNUP: "/signup",
    DASHBOARD: "/dashboard",
    HOME: "/",
  },

  // Protected route prefixes
  PROTECTED_ROUTES: ["/dashboard", "/settings", "/profile"],
  PUBLIC_ROUTES: ["/login", "/signup", "/forgot-password"],
} as const

export const API_CONFIG = {
  /**
   * Backend API Base URL
   * Set NEXT_PUBLIC_API_URL in .env.local
   */
  BASE_URL: process.env.NEXT_PUBLIC_API_URL || "/api",
  TIMEOUT: 30000, // 30 seconds
} as const

export const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  INTERNAL_ERROR: 500,
} as const
