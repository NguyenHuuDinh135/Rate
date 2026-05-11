import api from './api-client'
import * as Types from './types'
import { ApiResponse } from '@/types/auth'

export const moviesApi = {
  getAll: () => api.get<Types.MovieDto[]>('/movies/all'),
  getById: (id: number) => api.get<Types.MovieDto>(`/movies/id/${id}`),
  getFiltered: (movieType: 'NowShowing' | 'ComingSoon') => 
    api.get<Types.MovieDto[]>('/movies/filtered', { 
      headers: { 'movieType': movieType } // The backend seems to expect filters in query or header? 
      // Actually, looking at backend, it takes it from query.
    }),
  getFilteredWithParams: (params: { movieType?: string; title?: string; year?: number }) =>
    api.get<Types.MovieDto[]>('/movies/filtered', { 
       // In frontend api-client, we might need to support query params.
       // The current api-client doesn't seem to have a 'params' option in RequestOptions.
    }),
  getPersons: (id: number) => api.get<Types.PersonsForMovieDto>(`/movies/id/${id}/persons`),
}

export const personsApi = {
  getAll: () => api.get<Types.PersonDto[]>('/person/all'),
  getById: (id: number) => api.get<Types.PersonDto>(`/person/id/${id}`),
  getWithMovies: (id: number) => api.get<Types.PersonMovieDto[]>(`/person/id/${id}/movies`),
}

export const showsApi = {
  getById: (id: number) => api.get<Types.ShowDto>(`/shows/id/${id}`),
  getFiltered: (date: string) => api.get<ApiResponse<Types.ShowDto[]>>(`/shows/filters?date=${date}`),
}

export const bookingsApi = {
  getById: (id: number) => api.get<Types.BookingDto>(`/bookings/id/${id}`),
  getByUser: (userId: string) => api.get<Types.BookingDto[]>(`/bookings/users/${userId}`),
  getBookedSeats: (showId: number) => api.get<Types.BookedSeatDto[]>(`/bookings/shows/${showId}`),
  getLayout: (showId: number) => api.get<Types.BookingLayoutDto>(`/bookings/layout/${showId}`),
  create: (payload: Types.CreateBookingCommand) => api.post<number>('/bookings/create', payload),
}

export const paymentsApi = {
  getById: (id: number) => api.get<ApiResponse<Types.PaymentHistoryDto>>(`/payments/id/${id}`),
  getByUser: (userId: string) => api.get<ApiResponse<Types.PaymentHistoryDto[]>>(`/payments/users/${userId}`),
  create: (payload: Types.CreatePaymentCommand) => api.post<number>('/payments/create', payload),
}

export const usersApi = {
  getMe: () => api.get<Types.UserDto>('/users/me'),
  getAll: () => api.get<Types.UserDto[]>('/users/all'),
  getById: (id: string) => api.get<Types.UserDto>(`/users/id/${id}`),
  update: (payload: { id: string; fullName: string; email: string }) => 
    api.put<void>('/users/update', payload),
}

const movieClient = {
  movies: moviesApi,
  persons: personsApi,
  shows: showsApi,
  bookings: bookingsApi,
  payments: paymentsApi,
  users: usersApi,
}

export default movieClient
export * from './types'
