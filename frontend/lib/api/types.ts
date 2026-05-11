// ==========================================
// SHARED DTOs & TYPES (Migrated from moviereservation.client)
// ==========================================

export type MovieType = 'ComingSoon' | 'NowShowing' | 'Removed' | number
export type PaymentMethod = 'Cash' | 'Card' | 'Cod' | number | string
export type ShowType = 'ThreeD' | 'TwoD' | number
export type BookingStatus = 'Reserved' | 'Paid' | 'Cancelled' | number

export interface GenreDto {
  id: number
  name: string
}

export interface MovieDto {
  id: number
  title: string
  summary: string
  year: number
  rating?: number | null
  trailerUrl: string
  posterUrl: string
  movieType: MovieType
  genres: GenreDto[]
}

export interface FilteredMoviesDto {
  movies: MovieDto[]
  totalCount: number
}

export interface PersonMovieDto {
  id: number
  title: string
  posterUrl: string
  year: number
  role?: string
  rating?: string | number 
  summary?: string        
  trailerUrl?: string
}

export interface PersonDto {
  id: number
  fullName: string
  age?: number            
  dateOfBirth?: string
  biography?: string
  placeOfBirth?: string
  pictureUrl?: string
  role?: string
  movies?: any[]
}

export interface PersonsForMovieDto {
  movieId: number
  roles: PersonDto[] 
}

export interface TheaterSeatDto {
  seatRow: string
  seatNumber: number
}

export interface TheaterDto {
  id: number
  name: string
  numOfRows: number
  seatsPerRow: number
  type: string | number
  missing: TheaterSeatDto[]
  blocked: TheaterSeatDto[]
}

export interface ShowDto {
  id: number
  startTime: string
  endTime: string
  date: string
  movieId: number
  theaterId: number
  status: string | number
  type: ShowType
}

export interface BookingDto {
  id: number
  userId?: string
  user_id?: string
  showId?: number
  show_id?: number
  seatRow?: string
  seat_row?: string
  seatNumber?: number
  seat_number?: number
  price: number
  status: BookingStatus
  booking_datetime?: string
}

export interface BookedSeatDto {
  seatRow: string
  seatNumber: number
  isBooked?: boolean
}

export interface CreateBookingCommand {
  userId: string
  showId: number
  seatRow: string
  seatNumber: number
  price: number
  status?: number
}

export interface UpdateBookingCommand {
  id: number
  status: number
}

export interface CreatePaymentCommand {
  amount: number
  payment_datetime: string
  payment_method: string
  user_id: number | string
  show_id: number
  bookings: number[]
}

export interface UpdatePaymentCommand {
  id: number
  amount: number
  paymentMethod: PaymentMethod
}

export interface PaymentHistoryDto {
  payment_id: number
  amount: number
  payment_datetime: string
  payment_method: string
  user_id?: string
  show_id?: number
  movie?: {
    title: string
    poster_url: string
  }
}
export interface UserDto {
  id: string
  name: string
  email: string
  role?: string | null
}

export interface BookingLayoutDto {
  showId: number
  theaterId: number
  theaterName: string
  numOfRows: number
  seatsPerRow: number
  seats: { seatRow: string; seatNumber: number; type: number }[]
  bookedSeats: string[]
}

export type PaymentDto = PaymentHistoryDto
