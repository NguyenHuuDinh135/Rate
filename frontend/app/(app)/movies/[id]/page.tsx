"use client"

import { useState, useRef, useEffect, useMemo, use } from "react"
import { useParams, useRouter } from "next/navigation"
import { Play, Clock, MapPin, Calendar as CalendarIcon, ChevronLeft, ChevronRight, User, Monitor, CreditCard, Banknote, Loader2, AlertCircle } from "lucide-react"
import gsap from "gsap"
import { useGSAP } from "@gsap/react"
import { toast } from "sonner"
import { format, addDays, subDays, isSameDay, parseISO } from "date-fns"

// Shadcn & UI
import { Button } from "@/registry/new-york-v4/ui/button"
import { Badge } from "@/registry/new-york-v4/ui/badge"
import { Skeleton } from "@/registry/new-york-v4/ui/skeleton"
import { Calendar } from "@/registry/new-york-v4/ui/calendar"
import { Popover, PopoverContent, PopoverTrigger } from "@/registry/new-york-v4/ui/popover"
import { Dialog, DialogContent, DialogTrigger, DialogHeader, DialogTitle, DialogDescription, DialogFooter } from "@/registry/new-york-v4/ui/dialog"
import { Card } from "@/registry/new-york-v4/ui/card"
import { Separator } from "@/registry/new-york-v4/ui/separator"
import { RadioGroup, RadioGroupItem } from "@/registry/new-york-v4/ui/radio-group"
import { Label } from "@/registry/new-york-v4/ui/label"

// Hooks & API
import { useAuth } from "@/hooks/use-auth"
import movieClient, { 
  type MovieDto, type ShowDto, type TheaterDto, type BookedSeatDto, type PersonDto, type PaymentMethod 
} from "@/lib/api"

// --- HELPERS ---
const getEmbedUrl = (url: string) => {
  if (!url) return ""
  const match = url.match(/^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/)
  return (match && match[2].length === 11) ? `https://www.youtube.com/embed/${match[2]}?autoplay=1` : url
}

const formatTimeStr = (timeStr: string) => {
  if (!timeStr) return ""
  if (timeStr.includes("T")) return new Date(timeStr).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', hour12: false })
  const [hours, minutes] = timeStr.split(':')
  return `${hours}:${minutes}`
}

/* ==================== 1. SEAT MAP COMPONENT ==================== */
interface SeatMapProps {
  theater: TheaterDto; bookedSeats: BookedSeatDto[]; selectedSeats: string[]; onToggleSeat: (s: string) => void
}

function SeatMap({ theater, bookedSeats, selectedSeats, onToggleSeat }: SeatMapProps) {
  const containerRef = useRef<HTMLDivElement>(null)

  useGSAP(() => {
    if (!containerRef.current) return
    gsap.fromTo(containerRef.current.querySelectorAll('.seat-btn'), 
      { scale: 0, opacity: 0 },
      { scale: 1, opacity: 1, duration: 0.4, stagger: { amount: 0.2, grid: [theater.numOfRows, theater.seatsPerRow], from: "center" }, ease: "back.out(1.5)" }
    )
  }, { dependencies: [theater], scope: containerRef })

  const renderSeats = () => {
    const rows = []
    for (let i = 0; i < theater.numOfRows; i++) {
      const rowChar = String.fromCharCode(65 + i)
      const seatsInRow = []
      seatsInRow.push(<span key={`l-${i}`} className="w-6 text-center text-[10px] text-muted-foreground font-bold">{rowChar}</span>)
      for (let j = 1; j <= theater.seatsPerRow; j++) {
        const seatCode = `${rowChar}${j}`
        let status = 'available'
        if (theater.missing?.some(s => s.seatRow === rowChar && s.seatNumber === j)) status = 'hidden'
        else if (theater.blocked?.some(s => s.seatRow === rowChar && s.seatNumber === j)) status = 'blocked'
        else if (bookedSeats.some(s => s.seatRow === rowChar && s.seatNumber === j)) status = 'occupied'
        else if (selectedSeats.includes(seatCode)) status = 'selected'

        let seatClass = "w-6 h-6 md:w-8 md:h-8 rounded-t-md rounded-b-sm flex items-center justify-center transition-all duration-200 text-[9px] font-bold border "
        if (status === 'hidden') seatClass += "opacity-0 pointer-events-none border-none"
        else if (status === 'blocked') seatClass += "bg-muted cursor-not-allowed border-transparent opacity-30"
        else if (status === 'occupied') seatClass += "bg-muted text-muted-foreground/50 cursor-not-allowed shadow-none border-transparent"
        else if (status === 'selected') seatClass += "bg-primary text-primary-foreground shadow-lg shadow-primary/40 scale-110 z-10 border-primary"
        else seatClass += "bg-card hover:bg-primary/20 hover:border-primary/50 cursor-pointer text-transparent hover:text-foreground/50 border-border shadow-sm"

        seatsInRow.push(<button key={seatCode} disabled={status !== 'available' && status !== 'selected'} onClick={() => onToggleSeat(seatCode)} className={`seat-btn ${seatClass}`}>{j}</button>)
      }
      seatsInRow.push(<span key={`r-${i}`} className="w-6 text-center text-[10px] text-muted-foreground font-bold">{rowChar}</span>)
      rows.push(<div key={rowChar} className="flex gap-1.5 items-center justify-center mb-1.5">{seatsInRow}</div>)
    }
    return rows
  }

  return (
    <div ref={containerRef} className="relative w-full flex flex-col items-center">
      <div className="w-2/3 mb-10 relative perspective-[500px]">
         <div className="h-2 w-full bg-primary rounded-full shadow-[0_0_30px_10px_rgba(var(--primary),0.4)]" style={{ borderRadius: '50%' }} />
         <div className="absolute top-4 w-full text-center text-xs text-primary font-bold tracking-[0.5em] uppercase opacity-70">SCREEN</div>
      </div>
      <div className="overflow-x-auto max-w-full pb-8 px-4 w-full flex justify-center"><div className="min-w-max">{renderSeats()}</div></div>
      <div className="flex justify-center gap-6 text-xs w-full max-w-2xl border-t border-border pt-6">
        <div className="flex items-center gap-2"><div className="w-3 h-3 rounded bg-card border border-border" /><span className="text-muted-foreground">Available</span></div>
        <div className="flex items-center gap-2"><div className="w-3 h-3 rounded bg-primary shadow-sm" /><span className="text-foreground font-medium">Selected</span></div>
        <div className="flex items-center gap-2"><div className="w-3 h-3 rounded bg-muted" /><span className="text-muted-foreground">Occupied</span></div>
      </div>
    </div>
  )
}

/* ==================== 2. PAYMENT MODAL ==================== */
interface PaymentModalProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    totalAmount: number
    selectedSeats: string[]
    onConfirmPayment: (method: PaymentMethod) => void
    isProcessing: boolean
}

function PaymentModal({ open, onOpenChange, totalAmount, selectedSeats, onConfirmPayment, isProcessing }: PaymentModalProps) {
    const [method, setMethod] = useState<PaymentMethod>('Card')

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="sm:max-w-md bg-card border-border">
                <DialogHeader>
                    <DialogTitle>Checkout</DialogTitle>
                    <DialogDescription>Select your preferred payment method.</DialogDescription>
                </DialogHeader>
                <div className="grid gap-6 py-4">
                    <div className="bg-muted/50 p-4 rounded-lg flex justify-between items-center border border-border">
                        <div><p className="text-sm text-muted-foreground">Total Amount</p><p className="text-2xl font-bold text-foreground">{totalAmount.toLocaleString()} đ</p></div>
                        <Badge variant="outline" className="bg-background">{selectedSeats.length} Seats</Badge>
                    </div>
                    <RadioGroup defaultValue="Card" onValueChange={(v) => setMethod(v as PaymentMethod)} className="grid grid-cols-1 gap-3">
                        <div>
                            <RadioGroupItem value="Card" id="card" className="peer sr-only" />
                            <Label htmlFor="card" className="flex items-center justify-between rounded-md border-2 border-muted bg-popover p-4 hover:bg-accent hover:text-accent-foreground peer-data-[state=checked]:border-primary [&:has([data-state=checked])]:border-primary cursor-pointer transition-all">
                                <div className="flex items-center gap-3"><CreditCard className="w-5 h-5 text-primary" /><div className="space-y-1"><p className="text-sm font-medium leading-none">Credit Card</p><p className="text-xs text-muted-foreground">Visa, Mastercard</p></div></div>
                            </Label>
                        </div>
                        <div>
                            <RadioGroupItem value="Cash" id="cash" className="peer sr-only" />
                            <Label htmlFor="cash" className="flex items-center justify-between rounded-md border-2 border-muted bg-popover p-4 hover:bg-accent hover:text-accent-foreground peer-data-[state=checked]:border-primary [&:has([data-state=checked])]:border-primary cursor-pointer transition-all">
                                <div className="flex items-center gap-3"><Banknote className="w-5 h-5 text-green-500" /><div className="space-y-1"><p className="text-sm font-medium leading-none">Cash</p><p className="text-xs text-muted-foreground">Pay at counter</p></div></div>
                            </Label>
                        </div>
                    </RadioGroup>
                </div>
                <DialogFooter>
                    <Button variant="outline" onClick={() => onOpenChange(false)} disabled={isProcessing}>Cancel</Button>
                    <Button onClick={() => onConfirmPayment(method)} disabled={isProcessing} className="w-full sm:w-auto">
                        {isProcessing ? <><Loader2 className="mr-2 h-4 w-4 animate-spin" /> Processing...</> : "Confirm Payment"}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}

/* ==================== 3. BOOKING SUMMARY ==================== */
function BookingSummary({ selectedSeats, pricePerTicket, movieTitle, showTime, onCheckout }: any) {
  const total = selectedSeats.length * pricePerTicket
  return (
    <div className="space-y-6">
        <div className="space-y-1"><h3 className="text-xl font-bold tracking-wide uppercase">Your Order</h3><p className="text-muted-foreground text-xs">Review your tickets</p></div>
        <Card className="p-6 space-y-4 bg-card border-border">
            <div className="flex gap-4">
                <div className="h-16 w-12 bg-muted rounded overflow-hidden shrink-0"><div className="w-full h-full bg-gradient-to-br from-muted to-muted-foreground/20" /></div>
                <div className="space-y-1 overflow-hidden"><h4 className="font-bold truncate text-foreground">{movieTitle}</h4><p className="text-xs text-muted-foreground flex items-center gap-1"><Clock className="w-3 h-3"/> {showTime}</p></div>
            </div>
            <Separator className="my-4" />
            <div className="space-y-2 text-sm">
                <div className="flex justify-between text-muted-foreground"><span>Tickets ({selectedSeats.length})</span><span className="text-foreground font-medium">{selectedSeats.length > 0 ? selectedSeats.join(', ') : '-'}</span></div>
                <div className="flex justify-between text-muted-foreground"><span>Price</span><span className="text-foreground font-medium">{pricePerTicket.toLocaleString()} đ</span></div>
            </div>
        </Card>
        <div className="bg-primary text-primary-foreground rounded-xl p-6 shadow-xl relative overflow-hidden">
            <div className="absolute top-0 right-0 w-32 h-32 bg-white/10 rounded-full blur-3xl -mr-10 -mt-10" />
            <div className="relative z-10">
                <div className="flex justify-between items-end mb-6"><div><p className="text-primary-foreground/80 text-xs font-medium uppercase mb-1">Total Payment</p><p className="text-3xl font-black">{total.toLocaleString()} đ</p></div></div>
                <Button className="w-full bg-background/20 hover:bg-background/30 text-primary-foreground font-bold h-12 border border-primary-foreground/10 backdrop-blur-md transition-all" onClick={onCheckout} disabled={selectedSeats.length === 0}>CONFIRM & PAY</Button>
            </div>
        </div>
    </div>
  )
}

/* ==================== 4. MAIN PAGE ==================== */
export default function MovieDetailPage() {
  const params = useParams()
  const id = params?.id as string
  const router = useRouter()
  const { user } = useAuth()

  // --- STATE ---
  const [movie, setMovie] = useState<MovieDto | null>(null)
  const [persons, setPersons] = useState<PersonDto[]>([])
  
  // Data Logic
  const [allShows, setAllShows] = useState<ShowDto[]>([]) 
  const [datesWithShows, setDatesWithShows] = useState<string[]>([])
  
  const [selectedDate, setSelectedDate] = useState<Date>(new Date("2025-12-18")) 
  const [viewStartDate, setViewStartDate] = useState<Date>(new Date("2025-12-18"))

  const [selectedShow, setSelectedShow] = useState<ShowDto | null>(null)
  const [theater, setTheater] = useState<TheaterDto | null>(null)
  const [bookedSeats, setBookedSeats] = useState<BookedSeatDto[]>([])
  const [selectedSeats, setSelectedSeats] = useState<string[]>([])
  
  const [loading, setLoading] = useState(true)
  const [isProcessing, setIsProcessing] = useState(false)
  const [trailerOpen, setTrailerOpen] = useState(false)
  const [isPaymentOpen, setIsPaymentOpen] = useState(false)

  // 1. Fetch Data
  useEffect(() => {
    if (!id) return
    const fetchData = async () => {
      try {
        setLoading(true)
        
        const [movieRes, personRes, showsRes] = await Promise.all([
          movieClient.movies.getById(Number(id)),
          movieClient.movies.getPersons(Number(id)).catch(err => {
            console.warn("Failed to load cast:", err);
            return { roles: [] }; 
          }),
          movieClient.shows.getFiltered("") // Get all shows to extract dates, or we can filter by movie
        ])
        
        setMovie(movieRes)
        
        // Handle cast
        const pData = personRes as any
        if (Array.isArray(pData)) setPersons(pData)
        else if (pData?.roles) setPersons(pData.roles)
        else setPersons([]) 
        
        // Handle shows and dates
        const showsData = (showsRes as any).data || showsRes
        if (Array.isArray(showsData)) {
           // Filter shows for this movie
           const movieShows = showsData.filter((s: any) => s.movieId === Number(id))
           setAllShows(movieShows)
           
           // Extract unique dates
           const dates = Array.from(new Set(movieShows.map((s: any) => s.date.split('T')[0])))
           setDatesWithShows(dates)
           
           if (dates.length > 0) {
             const firstDate = parseISO(dates[0])
             setSelectedDate(firstDate)
             setViewStartDate(firstDate)
           }
        }
      } catch (error) { 
          console.error("Critical error loading movie details:", error); 
          toast.error("Failed to load movie details") 
      } finally { 
          setLoading(false) 
      }
    }
    fetchData()
  }, [id])

  // 2. Filter Shows
  const currentShows = useMemo(() => {
    const dateKey = format(selectedDate, "yyyy-MM-dd")
    return allShows
        .filter((s: any) => s.date.startsWith(dateKey))
        .sort((a, b) => a.startTime.localeCompare(b.startTime))
  }, [allShows, selectedDate])

  // 3. Fetch Theater & Bookings
  useEffect(() => {
    if (!selectedShow) return
    const fetchData = async () => {
      try {
        const [theaterRes, bookedRes] = await Promise.all([
          movieClient.theaters.getById(selectedShow.theaterId),
          movieClient.bookings.getBookedSeats(selectedShow.id)
        ])
        setTheater(theaterRes)
        setBookedSeats(bookedRes || [])
        setSelectedSeats([])
      } catch { toast.error("Error loading seat map") }
    }
    fetchData()
  }, [selectedShow])

  // Handlers
  const handleToggleSeat = (code: string) => {
    setSelectedSeats(prev => prev.includes(code) ? prev.filter(s => s !== code) : [...prev, code])
  }

  const handleCheckoutClick = () => {
    if (!selectedShow || selectedSeats.length === 0) return toast.warning("Please select seats first")
    setIsPaymentOpen(true)
  }

  // Handle Payment
  const handleConfirmPayment = async (method: PaymentMethod) => {
    if (!selectedShow || !user) return toast.error("Please login to book tickets")

    try {
      setIsProcessing(true)
      const currentUserId = user.id
      const pricePerTicket = 40000
      const totalAmount = selectedSeats.length * pricePerTicket

      const bookingPromises = selectedSeats.map(seat => {
        const row = seat.charAt(0)
        const num = parseInt(seat.substring(1))
        return movieClient.bookings.create({
            userId: currentUserId,
            showId: selectedShow.id,
            seatRow: row,
            seatNumber: num,
            price: pricePerTicket,
            status: 1 
        })
      })

      const bookingIds = await Promise.all(bookingPromises)

      const paymentRes = await movieClient.payments.create({
          amount: totalAmount,
          payment_datetime: new Date().toISOString(),
          payment_method: String(method).toLowerCase(),
          user_id: currentUserId,
          show_id: selectedShow.id,
          bookings: bookingIds
      })

      const newPaymentId = paymentRes 

      setIsPaymentOpen(false)
      toast.success("Payment Successful!")
      
      const bookingIdsStr = bookingIds.join(',')
      setTimeout(() => router.push(`/booking/success?paymentId=${newPaymentId}&bookingIds=${bookingIdsStr}`), 1000)
    } catch (error) { 
      console.error("Transaction Failed", error)
      toast.error("Transaction Failed") 
    } finally { 
      setIsProcessing(false) 
    }
  }

  const handlePrevWeek = () => setViewStartDate(prev => subDays(prev, 7))
  const handleNextWeek = () => setViewStartDate(prev => addDays(prev, 7))

  if (loading || !movie) return <div className="min-h-screen bg-background flex items-center justify-center"><Skeleton className="h-10 w-10 rounded-full" /></div>

  const dateList = Array.from({ length: 7 }).map((_, i) => addDays(viewStartDate, i))

  return (
    <div className="min-h-screen bg-background text-foreground font-sans pb-20 transition-colors duration-300">
      
      <div className="relative w-full h-[60vh] lg:h-[70vh]">
        <div className="absolute inset-0 z-0">
            <img src={movie.posterUrl} className="w-full h-full object-cover opacity-30 dark:opacity-20 blur-3xl" alt="bg" />
            <div className="absolute inset-0 bg-gradient-to-t from-background via-background/60 to-transparent" />
            <div className="absolute inset-0 bg-gradient-to-r from-background via-background/50 to-transparent" />
        </div>
        <div className="relative z-10 container h-full flex items-center pt-12">
            <div className="flex flex-col md:flex-row gap-8 lg:gap-16 items-start md:items-end w-full">
                <div className="shrink-0 group relative mx-auto md:mx-0">
                    <img src={movie.posterUrl} alt={movie.title} className="relative w-52 md:w-64 lg:w-80 rounded-xl shadow-2xl shadow-black/20 dark:shadow-black/50 border border-border group-hover:scale-[1.02] transition-transform duration-500"/>
                </div>
                <div className="flex-1 space-y-6 text-center md:text-left w-full">
                    <div className="space-y-2">
                        <h1 className="text-4xl md:text-6xl font-black tracking-tighter leading-none text-foreground">{movie.title}</h1>
                        <div className="flex flex-wrap items-center justify-center md:justify-start gap-3 text-sm font-medium text-muted-foreground">
                            {movie.genres?.map(g => <Badge key={g.id} variant="outline" className="border-border bg-background/50 backdrop-blur">{g.name}</Badge>)}
                            <span>•</span><span>{movie.year}</span>
                            <span>•</span><span className="flex items-center gap-1 text-primary"><Monitor className="w-3 h-3"/> IMAX</span>
                        </div>
                    </div>
                    <p className="text-muted-foreground max-w-2xl text-sm md:text-base leading-relaxed line-clamp-3 md:line-clamp-4">{movie.summary}</p>
                    {persons.length > 0 && (
                        <div className="flex flex-wrap items-center justify-center md:justify-start gap-3">
                            {persons.slice(0, 4).map(person => (
                                <div key={person.id} onClick={() => router.push(`/person/${person.id}`)} className="group flex items-center gap-2 pr-3 rounded-full border border-border bg-card/50 hover:bg-accent hover:border-primary/30 transition-all cursor-pointer">
                                    <div className="w-8 h-8 rounded-full overflow-hidden bg-muted">{person.pictureUrl ? <img src={person.pictureUrl} className="w-full h-full object-cover" alt="" /> : <User className="w-4 h-4 m-2 text-muted-foreground" />}</div>
                                    <div className="text-xs font-medium text-muted-foreground group-hover:text-foreground">{person.fullName}</div>
                                </div>
                            ))}
                        </div>
                    )}
                    <div className="pt-4"><Dialog open={trailerOpen} onOpenChange={setTrailerOpen}><DialogTrigger asChild><Button size="lg" className="rounded-full font-bold shadow-lg hover:scale-105 transition-transform"><Play className="w-4 h-4 mr-2 fill-current" /> WATCH TRAILER</Button></DialogTrigger><DialogContent className="sm:max-w-[900px] bg-black border-border p-0 overflow-hidden"><div className="aspect-video w-full">{movie.trailerUrl && <iframe src={getEmbedUrl(movie.trailerUrl)} className="w-full h-full" allowFullScreen allow="autoplay" />}</div></DialogContent></Dialog></div>
                </div>
            </div>
        </div>
      </div>

      <div className="sticky top-0 z-50 bg-background/80 backdrop-blur-md border-y border-border shadow-sm">
        <div className="container py-4">
            <div className="flex flex-col lg:flex-row items-start lg:items-center gap-6">
                <div className="flex items-center gap-2 w-full lg:w-auto">
                    <Button variant="ghost" size="icon" onClick={handlePrevWeek} className="shrink-0 rounded-full h-10 w-10"><ChevronLeft className="h-5 w-5" /></Button>
                    <div className="flex items-center gap-3 overflow-x-auto no-scrollbar pb-2 lg:pb-0 px-2">
                        {dateList.map((day) => {
                            const dateStr = format(day, "yyyy-MM-dd")
                            const isActive = isSameDay(day, selectedDate)
                            const hasShow = datesWithShows.includes(dateStr)
                            return (
                                <button key={dateStr} onClick={() => { setSelectedDate(day); setSelectedShow(null); }} disabled={!hasShow} className={`flex flex-col items-center justify-center min-w-[3.5rem] h-14 rounded-2xl transition-all duration-300 border ${isActive ? "bg-primary border-primary text-primary-foreground shadow-md scale-105" : hasShow ? "bg-card border-border text-foreground hover:bg-accent" : "bg-muted/30 border-transparent text-muted-foreground/30 cursor-not-allowed"}`}>
                                    <span className="text-[10px] font-bold uppercase tracking-wider opacity-70">{format(day, "EEE")}</span>
                                    <span className="text-lg font-bold">{format(day, "dd")}</span>
                                </button>
                            )
                        })}
                    </div>
                    <Button variant="ghost" size="icon" onClick={handleNextWeek} className="shrink-0 rounded-full h-10 w-10"><ChevronRight className="h-5 w-5" /></Button>
                    <div className="h-8 w-px bg-border mx-2 hidden sm:block" />
                    <Popover>
                        <PopoverTrigger asChild><Button variant="outline" className="h-10 w-10 rounded-full p-0 shrink-0 border-border bg-card hover:bg-accent"><CalendarIcon className="h-5 w-5 text-primary" /></Button></PopoverTrigger>
                        <PopoverContent className="w-auto p-0" align="start">
                            <Calendar mode="single" selected={selectedDate} onSelect={(d) => { if(d) { setSelectedDate(d); setViewStartDate(d); setSelectedShow(null); } }} disabled={(date) => !datesWithShows.includes(format(date, "yyyy-MM-dd"))} initialFocus />
                        </PopoverContent>
                    </Popover>
                </div>
                <div className="hidden lg:block h-10 w-px bg-border mx-4" />
                <div className="flex-1 w-full overflow-x-auto pb-2 lg:pb-0">
                    <div className="flex flex-col gap-2">
                        <span className="text-xs text-muted-foreground font-bold uppercase tracking-widest flex items-center gap-2"><Clock className="w-3 h-3"/> Showtimes ({format(selectedDate, "MMM dd")})</span>
                        {currentShows.length > 0 ? (
                            <div className="flex gap-3">
                                {currentShows.map(show => {
                                    const timeStr = `${formatTimeStr(show.startTime)} - ${formatTimeStr(show.endTime)}`
                                    const isSelected = selectedShow?.id === show.id
                                    return (
                                        <button key={show.id} onClick={() => setSelectedShow(show)} className={`px-5 py-2.5 rounded-xl border text-sm font-semibold transition-all whitespace-nowrap flex items-center gap-2 ${isSelected ? "bg-foreground text-background border-foreground shadow-md scale-105" : "bg-card border-border text-muted-foreground hover:border-primary hover:text-foreground"}`}>
                                            {timeStr}
                                            <span className={`text-[9px] font-bold px-1.5 py-0.5 rounded uppercase ${isSelected ? "bg-background/20 text-background" : "bg-muted text-muted-foreground"}`}>{show.type === 'ThreeD' ? '3D' : '2D'}</span>
                                        </button>
                                    )
                                })}
                            </div>
                        ) : <div className="flex items-center gap-2 text-sm text-muted-foreground italic py-2 border border-dashed border-border rounded-lg px-4 bg-muted/30 w-fit"><AlertCircle className="w-4 h-4" /> No showtimes available for this date.</div>}
                    </div>
                </div>
            </div>
        </div>
      </div>

      <div className="container py-12">
        {!selectedShow ? (
            <div className="flex flex-col items-center justify-center py-32 text-muted-foreground space-y-6 animate-in fade-in zoom-in-95">
                <Monitor className="w-24 h-24 opacity-10" />
                <div className="text-center"><h3 className="text-lg font-medium">Ready to watch?</h3><p className="text-sm">Select a showtime to view seats.</p></div>
            </div>
        ) : (
            theater ? (
                <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 lg:gap-16 animate-in fade-in slide-in-from-bottom-8 duration-700">
                    <div className="lg:col-span-4 order-2 lg:order-1 h-fit sticky top-24">
                        <BookingSummary 
                            selectedSeats={selectedSeats} pricePerTicket={40000} movieTitle={movie.title} 
                            showTime={`${format(selectedDate, "EEE, dd MMM")} • ${formatTimeStr(selectedShow.startTime)}`} 
                            onCheckout={handleCheckoutClick}
                        />
                    </div>
                    <div className="lg:col-span-8 order-1 lg:order-2">
                        <Card className="rounded-[2rem] p-6 lg:p-10 shadow-lg relative overflow-hidden bg-card border-border">
                            <div className="absolute top-0 left-1/2 -translate-x-1/2 w-1/2 h-2 bg-primary blur-[60px] opacity-40" />
                            <div className="flex items-center gap-2 mb-8 text-sm text-muted-foreground justify-center border-b border-border pb-4">
                                <MapPin className="w-4 h-4 text-primary" /> <span className="font-medium text-foreground">{theater.name}</span><span className="mx-2">•</span><span>{selectedShow.type === 'ThreeD' ? '3D Experience' : 'Standard 2D'}</span>
                            </div>
                            <SeatMap theater={theater} bookedSeats={bookedSeats} selectedSeats={selectedSeats} onToggleSeat={handleToggleSeat} />
                        </Card>
                    </div>
                </div>
            ) : <div className="flex justify-center py-32"><div className="w-12 h-12 border-4 border-primary border-t-transparent rounded-full animate-spin" /></div>
        )}
      </div>

      <PaymentModal 
        open={isPaymentOpen} 
        onOpenChange={setIsPaymentOpen}
        totalAmount={selectedSeats.length * 40000}
        selectedSeats={selectedSeats}
        onConfirmPayment={handleConfirmPayment}
        isProcessing={isProcessing}
      />
    </div>
  )
}
