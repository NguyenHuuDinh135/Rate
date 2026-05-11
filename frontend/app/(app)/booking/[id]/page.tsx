"use client"

import { useState, useMemo } from "react"
import { useParams, useRouter } from "next/navigation"
import { useQuery, useMutation } from "@tanstack/react-query"
import { Button } from "@/registry/new-york-v4/ui/button"
import { Armchair, ChevronLeft, CreditCard, Loader2 } from "lucide-react"
import { toast } from "sonner"
import movieClient from "@/lib/api"
import { useAuth } from "@/hooks/use-auth"

export default function BookingPage() {
  const params = useParams()
  const router = useRouter()
  const { user } = useAuth()
  const showId = parseInt(params.id as string)
  
  const [selectedSeats, setSelectedSeats] = useState<string[]>([])

  const { data: layout, isLoading: layoutLoading } = useQuery({
    queryKey: ["booking", "layout", showId],
    queryFn: () => movieClient.bookings.getLayout(showId),
    enabled: !!showId
  })

  const { data: movie, isLoading: movieLoading } = useQuery({
    queryKey: ["movies", layout?.showId],
    queryFn: async () => {
       if (!layout) return null
       // We need to get the movie ID from the show first.
       // The layout DTO doesn't have movieId. Let's get the show details too.
       const show = await movieClient.shows.getById(showId)
       return movieClient.movies.getById(show.movieId)
    },
    enabled: !!layout
  })

  const bookingMutation = useMutation({
    mutationFn: async () => {
      if (!user || !layout) return
      
      const bookings = selectedSeats.map(seatId => {
         const row = seatId.match(/[A-Z]+/)?.[0] || ""
         const num = parseInt(seatId.match(/\d+/)?.[0] || "0")
         return movieClient.bookings.create({
           userId: user.id,
           showId: showId,
           seatRow: row,
           seatNumber: num,
           price: 12.0 // Pricing logic
         })
      })
      
      return Promise.all(bookings)
    },
    onSuccess: () => {
      toast.success("Booking request sent!")
      router.push("/booking/success")
    },
    onError: () => {
      toast.error("Booking failed. Seats might have been taken.")
    }
  })

  const toggleSeat = (id: string) => {
    setSelectedSeats(prev => 
      prev.includes(id) ? prev.filter(s => s !== id) : [...prev, id]
    )
  }

  const isLoading = layoutLoading || movieLoading

  if (isLoading) {
    return (
      <div className="flex h-svh items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    )
  }

  const rows = Array.from({ length: layout?.numOfRows || 0 }, (_, i) => String.fromCharCode(65 + i))
  const seatNums = Array.from({ length: layout?.seatsPerRow || 0 }, (_, i) => i + 1)

  return (
    <div className="min-h-screen bg-background pt-24 pb-20 px-4 md:px-8">
      <div className="container mx-auto max-w-4xl">
        <Button 
          variant="ghost" 
          className="mb-6 -ml-4" 
          onClick={() => router.back()}
        >
          <ChevronLeft className="mr-2 h-4 w-4" /> Back to Movie
        </Button>

        <div className="grid gap-8 lg:grid-cols-3">
          <div className="lg:col-span-2 space-y-8">
            <div className="rounded-3xl border bg-card p-8">
              <div className="flex flex-col items-center gap-12">
                {/* Screen */}
                <div className="w-full">
                  <div className="h-1 w-full bg-primary rounded-full shadow-[0_4px_20px_rgba(var(--primary),0.5)]" />
                  <p className="text-center text-xs text-muted-foreground mt-2 uppercase tracking-widest">Screen ({layout?.theaterName})</p>
                </div>

                {/* Seats Grid */}
                <div className="flex flex-col gap-2">
                  {rows.map(row => (
                    <div key={row} className="flex gap-2">
                      <div className="w-6 flex items-center justify-center text-xs font-bold text-muted-foreground">{row}</div>
                      {seatNums.map(num => {
                        const id = `${row}${num}`
                        const isBooked = layout?.bookedSeats.includes(id)
                        const isSelected = selectedSeats.includes(id)

                        return (
                          <button
                            key={id}
                            disabled={isBooked}
                            onClick={() => toggleSeat(id)}
                            className={`
                              h-7 w-7 rounded-t-lg transition-all flex items-center justify-center text-[9px] font-bold
                              ${isBooked 
                                ? "bg-muted-foreground/20 text-transparent cursor-not-allowed" 
                                : isSelected 
                                  ? "bg-primary text-primary-foreground scale-110 shadow-lg" 
                                  : "bg-muted text-muted-foreground hover:bg-muted-foreground/20"}
                            `}
                          >
                            {num}
                          </button>
                        )
                      })}
                    </div>
                  ))}
                </div>

                {/* Legend */}
                <div className="flex flex-wrap justify-center gap-6 text-xs font-medium uppercase tracking-wider">
                  <div className="flex items-center gap-2">
                    <div className="h-4 w-4 rounded-t-sm bg-muted" />
                    <span className="text-muted-foreground">Available</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <div className="h-4 w-4 rounded-t-sm bg-primary" />
                    <span className="text-primary">Selected</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <div className="h-4 w-4 rounded-t-sm bg-muted-foreground/20" />
                    <span className="text-muted-foreground/50">Occupied</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="space-y-6">
            <div className="rounded-3xl border bg-card p-6 sticky top-28">
              <h2 className="text-xl font-bold mb-4">Booking Summary</h2>
              <div className="space-y-4">
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Movie</span>
                  <span className="font-medium truncate max-w-[150px]">{movie?.title}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Theater</span>
                  <span className="font-medium">{layout?.theaterName}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Seats</span>
                  <span className="font-medium text-primary">
                    {selectedSeats.length > 0 ? selectedSeats.sort().join(", ") : "None selected"}
                  </span>
                </div>
                <div className="border-t pt-4 mt-4">
                  <div className="flex justify-between items-center">
                    <span className="font-bold">Total Price</span>
                    <span className="text-2xl font-black text-primary">
                      ${(selectedSeats.length * 12).toFixed(2)}
                    </span>
                  </div>
                </div>
                <Button 
                  className="w-full rounded-full h-12 font-bold mt-4 shadow-xl shadow-primary/20" 
                  disabled={selectedSeats.length === 0 || bookingMutation.isPending}
                  onClick={() => bookingMutation.mutate()}
                >
                  {bookingMutation.isPending ? (
                    <Loader2 className="h-4 w-4 animate-spin" />
                  ) : (
                    <>
                      <CreditCard className="mr-2 h-4 w-4" /> Checkout Now
                    </>
                  )}
                </Button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
