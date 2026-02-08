"use client"

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react"
import { useRouter } from "next/navigation"
import { toast } from "sonner"

import { AUTH_CONFIG } from "@/lib/constants"
import {
  getCurrentUser,
  loginApi,
  logoutApi,
  refreshAuth,
  registerApi,
} from "@/lib/api/auth"
import { tokenStorage } from "@/lib/api/api-client"
import type {
  AuthContextType,
  AuthState,
  LoginCredentials,
  RegisterCredentials,
  User,
  ApiError,
} from "@/types/auth"

// ============================================
// Auth Context
// ============================================

const AuthContext = createContext<AuthContextType | undefined>(undefined)

const initialState: AuthState = {
  user: null,
  isAuthenticated: false,
  isLoading: true,
  error: null,
}

interface AuthProviderProps {
  children: ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const router = useRouter()
  const [state, setState] = useState<AuthState>(initialState)

  // ============================================
  // Initialize Auth State (check existing session)
  // ============================================
  useEffect(() => {
    const initAuth = async () => {
      try {
        // Try to refresh the session using httpOnly cookie
        const authData = await refreshAuth()

        if (authData) {
          setState({
            user: authData.user,
            isAuthenticated: true,
            isLoading: false,
            error: null,
          })
        } else {
          setState({
            ...initialState,
            isLoading: false,
          })
        }
      } catch {
        setState({
          ...initialState,
          isLoading: false,
        })
      }
    }

    initAuth()
  }, [])

  // ============================================
  // Login
  // ============================================
  const login = useCallback(
    async (credentials: LoginCredentials) => {
      setState((prev) => ({ ...prev, isLoading: true, error: null }))

      try {
        const authData = await loginApi(credentials)

        setState({
          user: authData.user,
          isAuthenticated: true,
          isLoading: false,
          error: null,
        })

        toast.success("Login successful!", {
          description: `Welcome ${authData.user.name}`,
        })

        // Redirect to home page
        router.push(AUTH_CONFIG.ROUTES.HOME)
      } catch (err) {
        const apiError = err as ApiError
        setState((prev) => ({
          ...prev,
          isLoading: false,
          error: apiError.message || "Login failed",
        }))

        toast.error("Login failed", {
          description: apiError.message,
        })

        throw err
      }
    },
    [router]
  )

  // ============================================
  // Register
  // ============================================
  const register = useCallback(
    async (credentials: RegisterCredentials) => {
      setState((prev) => ({ ...prev, isLoading: true, error: null }))

      try {
        const authData = await registerApi(credentials)

        setState({
          user: authData.user,
          isAuthenticated: true,
          isLoading: false,
          error: null,
        })

        toast.success("Registration successful!", {
          description: "Your account has been created.",
        })

        router.push(AUTH_CONFIG.ROUTES.HOME)
      } catch (err) {
        const apiError = err as ApiError
        setState((prev) => ({
          ...prev,
          isLoading: false,
          error: apiError.message || "Registration failed",
        }))

        toast.error("Registration failed", {
          description: apiError.message,
        })

        throw err
      }
    },
    [router]
  )

  // ============================================
  // Logout
  // ============================================
  const logout = useCallback(async () => {
    setState((prev) => ({ ...prev, isLoading: true }))

    try {
      await logoutApi()
    } catch {
      // Continue logout even if server request fails
    } finally {
      // Clear all auth state
      tokenStorage.clear()
      setState({
        ...initialState,
        isLoading: false,
      })

      toast.success("Signed out successfully")
      router.push(AUTH_CONFIG.ROUTES.LOGIN)
    }
  }, [router])

  // ============================================
  // Refresh Auth
  // ============================================
  const refreshAuthState = useCallback(async () => {
    try {
      const authData = await refreshAuth()

      if (authData) {
        setState({
          user: authData.user,
          isAuthenticated: true,
          isLoading: false,
          error: null,
        })
      } else {
        setState({
          ...initialState,
          isLoading: false,
        })
      }
    } catch {
      setState({
        ...initialState,
        isLoading: false,
      })
    }
  }, [])

  // ============================================
  // Clear Error
  // ============================================
  const clearError = useCallback(() => {
    setState((prev) => ({ ...prev, error: null }))
  }, [])

  // ============================================
  // Context Value
  // ============================================
  const value = useMemo<AuthContextType>(
    () => ({
      ...state,
      login,
      logout,
      register,
      refreshAuth: refreshAuthState,
      clearError,
    }),
    [state, login, logout, register, refreshAuthState, clearError]
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

// ============================================
// useAuth Hook
// ============================================
export function useAuth() {
  const context = useContext(AuthContext)

  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider")
  }

  return context
}

// ============================================
// Auth Guard Component
// ============================================
interface AuthGuardProps {
  children: ReactNode
  fallback?: ReactNode
}

export function AuthGuard({ children, fallback }: AuthGuardProps) {
  const { isAuthenticated, isLoading } = useAuth()
  const router = useRouter()

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      router.push(AUTH_CONFIG.ROUTES.LOGIN)
    }
  }, [isAuthenticated, isLoading, router])

  if (isLoading) {
    return fallback || <AuthLoadingSkeleton />
  }

  if (!isAuthenticated) {
    return null
  }

  return <>{children}</>
}

// ============================================
// Guest Guard (for login/signup pages)
// ============================================
export function GuestGuard({ children }: { children: ReactNode }) {
  const { isAuthenticated, isLoading } = useAuth()
  const router = useRouter()

  useEffect(() => {
    if (!isLoading && isAuthenticated) {
      router.push(AUTH_CONFIG.ROUTES.DASHBOARD)
    }
  }, [isAuthenticated, isLoading, router])

  if (isLoading) {
    return <AuthLoadingSkeleton />
  }

  if (isAuthenticated) {
    return null
  }

  return <>{children}</>
}

// ============================================
// Loading Skeleton
// ============================================
function AuthLoadingSkeleton() {
  return (
    <div className="flex min-h-svh items-center justify-center">
      <div className="flex flex-col items-center gap-4">
        <div className="h-10 w-10 animate-spin rounded-full border-4 border-primary border-t-transparent" />
        <p className="text-sm text-muted-foreground">Loading...</p>
      </div>
    </div>
  )
}
