"use client"

import { useState, useEffect, useMemo } from "react"
import { useRouter } from "next/navigation"
import { 
  Search, 
  Calendar, 
  MapPin, 
  Clock, 
  Ticket, 
  QrCode, 
  Copy, 
  CheckCircle2, 
  AlertCircle,
  Armchair,
  History,
  CreditCard,
  XCircle
} from "lucide-react"
import { format, isFuture, isPast } from "date-fns"
import { toast } from "sonner"

// UI Components
import { Input } from "@/registry/new-york-v4/ui/input"
import { Button } from "@/registry/new-york-v4/ui/button"
import { Badge } from "@/registry/new-york-v4/ui/badge"
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/registry/new-york-v4/ui/tabs"
import { Skeleton } from "@/registry/new-york-v4/ui/skeleton"
import { Card } from "@/registry/new-york-v4/ui/card"
import { Separator } from "@/registry/new-york-v4/ui/separator"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/registry/new-york-v4/ui/dialog"

// Hooks & API
import { useAuth } from "@/hooks/use-auth"
import movieClient, { type PaymentHistoryDto } from "@/lib/api"

// --- COMPONENT: TICKET CARD ---
interface TicketCardProps {
  ticket: PaymentHistoryDto;
  onClick: () => void;
  onCancel?: (id: number) => void;
  onPay?: (id: number) => void;
}

const TicketCard = ({ ticket, onClick, onCancel, onPay }: TicketCardProps) => {
  const showTime = new Date(ticket.payment_datetime)
  const isUpcoming = isFuture(showTime)
  
  const isPending = (ticket as any).status === 0 || (ticket as any).status === 'Pending'
  const isCancelled = (ticket as any).status === -1 || (ticket as any).status === 'Cancelled'

  const theaterName = (ticket as any).theaterName || "Loading Theater..."
  const seatList = (ticket as any).seats || ["--"]

  const handleCopyId = (e: React.MouseEvent) => {
    e.stopPropagation()
    navigator.clipboard.writeText(ticket.payment_id.toString())
    toast.success("Đã sao chép mã đơn hàng")
  }

  return (
    <div 
      className="group relative flex flex-col md:flex-row bg-card border border-border rounded-2xl overflow-hidden hover:shadow-xl hover:border-primary/50 transition-all duration-300"
    >
      <div className="flex flex-col md:flex-row flex-1 cursor-pointer" onClick={onClick}>
          <div className="md:w-40 h-48 md:h-auto bg-muted relative shrink-0 overflow-hidden">
            {ticket.movie?.poster_url ? (
              <img 
                src={ticket.movie.poster_url} 
                alt={ticket.movie.title} 
                className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110"
              />
            ) : (
              <div className="w-full h-full flex items-center justify-center text-muted-foreground"><Ticket /></div>
            )}
            <div className="absolute inset-0 bg-black/10 group-hover:bg-transparent transition-colors" />
            
            <div className="absolute top-2 left-2 flex flex-col gap-1">
                {isCancelled ? (
                    <Badge variant="destructive">Đã hủy</Badge>
                ) : isPending ? (
                    <Badge className="bg-yellow-500 hover:bg-yellow-600">Chờ thanh toán</Badge>
                ) : (
                    <Badge variant={isUpcoming ? "default" : "secondary"} className={isUpcoming ? "bg-primary text-primary-foreground" : "bg-muted text-muted-foreground"}>
                        {isUpcoming ? "Sắp chiếu" : "Đã xem"}
                    </Badge>
                )}
            </div>
          </div>

          <div className="flex-1 p-5 flex flex-col justify-between relative">
            <div className="md:hidden absolute top-0 left-0 w-full h-4 -mt-2 bg-background rounded-b-xl z-10 border-t border-dashed border-border" />

            <div>
                <div className="flex justify-between items-start gap-2">
                    <h3 className="font-bold text-xl leading-tight group-hover:text-primary transition-colors line-clamp-1 text-foreground">
                        {ticket.movie?.title || "Đang tải tên phim..."}
                    </h3>
                </div>
                
                <div className="mt-4 space-y-2 text-sm text-muted-foreground">
                    <div className="flex items-center gap-2">
                        <Calendar className="w-4 h-4 text-primary" />
                        <span className="font-medium text-foreground">{format(showTime, "EEEE, dd/MM/yyyy")}</span>
                    </div>
                    <div className="flex items-center gap-2">
                        <Clock className="w-4 h-4 text-primary" />
                        <span>{format(showTime, "HH:mm")}</span>
                        <span className="text-border mx-1">|</span>
                        <MapPin className="w-4 h-4 text-primary" />
                        <span className="truncate max-w-[200px]">{theaterName}</span>
                    </div>
                    <div className="flex items-center gap-2">
                        <Armchair className="w-4 h-4 text-primary" />
                        <span className="font-medium text-foreground">Ghế: {seatList.join(", ")}</span>
                    </div>
                </div>
            </div>

            <div className="mt-4 flex items-center gap-4 text-xs text-muted-foreground pt-4 border-t border-dashed">
                <div className="flex items-center gap-1 group/id" onClick={handleCopyId}>
                    <span>Mã đơn:</span>
                    <code className="bg-muted px-1 py-0.5 rounded font-mono text-foreground font-bold group-hover/id:bg-primary/20 transition-colors">
                        #{ticket.payment_id}
                    </code>
                    <Copy className="w-3 h-3 ml-1 opacity-0 group-hover/id:opacity-100 transition-opacity" />
                </div>
                <div className="flex-1 text-right">
                    <span className="font-bold text-lg text-primary">
                        {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(ticket.amount)}
                    </span>
                </div>
            </div>
          </div>
      </div>

      <div className="flex md:flex-col border-t md:border-t-0 md:border-l border-dashed border-border/60 bg-muted/10 relative items-center justify-center p-4 gap-3 shrink-0 md:w-48">
        <div className="hidden md:block absolute -top-3 -left-3 w-6 h-6 bg-background rounded-full border border-border/60" />
        <div className="hidden md:block absolute -bottom-3 -left-3 w-6 h-6 bg-background rounded-full border border-border/60" />

        {isPending && !isCancelled ? (
            <>
                <Button size="sm" className="w-full rounded-full font-bold shadow-md bg-green-600 hover:bg-green-700" onClick={() => onPay && onPay(ticket.payment_id)}>
                    <CreditCard className="w-4 h-4 mr-2" /> Thanh toán
                </Button>
                <Dialog>
                    <DialogTrigger asChild>
                        <Button size="sm" variant="outline" className="w-full rounded-full border-red-200 text-red-600 hover:bg-red-50 hover:text-red-700">
                            <XCircle className="w-4 h-4 mr-2" /> Hủy vé
                        </Button>
                    </DialogTrigger>
                    <DialogContent>
                        <DialogHeader>
                            <DialogTitle>Xác nhận hủy vé?</DialogTitle>
                            <DialogDescription>
                                Bạn có chắc chắn muốn hủy đơn đặt vé #{ticket.payment_id} không? Hành động này không thể hoàn tác.
                            </DialogDescription>
                        </DialogHeader>
                        <DialogFooter>
                            <Button variant="destructive" onClick={() => onCancel && onCancel(ticket.payment_id)}>Xác nhận hủy</Button>
                        </DialogFooter>
                    </DialogContent>
                </Dialog>
            </>
        ) : (
            <>
                <QrCode className="hidden md:block w-20 h-20 text-foreground opacity-80 mb-2" />
                <Button size="sm" variant="secondary" className="w-full rounded-full font-bold shadow-sm" onClick={onClick}>
                    Xem chi tiết
                </Button>
                {isUpcoming && !isCancelled && (
                     <Dialog>
                        <DialogTrigger asChild>
                            <Button size="sm" variant="ghost" className="w-full rounded-full text-xs text-muted-foreground hover:text-destructive">
                                Hủy vé
                            </Button>
                        </DialogTrigger>
                        <DialogContent>
                            <DialogHeader>
                                <DialogTitle>Yêu cầu hoàn vé</DialogTitle>
                                <DialogDescription>
                                    Vé đã thanh toán. Việc hủy vé sẽ tuân theo chính sách hoàn tiền của rạp. Bạn có muốn tiếp tục?
                                </DialogDescription>
                            </DialogHeader>
                            <DialogFooter>
                                <Button variant="destructive" onClick={() => onCancel && onCancel(ticket.payment_id)}>Gửi yêu cầu hủy</Button>
                            </DialogFooter>
                        </DialogContent>
                    </Dialog>
                )}
            </>
        )}
      </div>
    </div>
  )
}

// --- MAIN PAGE ---
export default function MyTicketsPage() {
  const router = useRouter()
  const { user, isAuthenticated, isLoading: authLoading } = useAuth()
  
  const [tickets, setTickets] = useState<PaymentHistoryDto[]>([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState("")
  const [activeTab, setActiveTab] = useState("upcoming")

  const enrichTicketData = async (ticket: PaymentHistoryDto) => {
      try {
          const showId = (ticket as any).show_id || (ticket as any).showId
          
          if (showId) {
              const showData = await movieClient.shows.getById(showId)
              
              let theaterName = "Unknown Theater"
              if (showData && showData.theaterId) {
                  const theaterData = await movieClient.theaters.getById(showData.theaterId)
                  theaterName = theaterData?.name || "Unknown Theater"
              }

              return {
                  ...ticket,
                  theaterName: theaterName,
                  showStartTime: showData?.startTime,
                  showEndTime: showData?.endTime,
                  payment_datetime: showData?.date ? `${showData.date.split('T')[0]}T${showData.startTime}` : ticket.payment_datetime
              }
          }
      } catch (err) {
          console.warn(`Failed to enrich ticket #${ticket.payment_id}`, err)
      }
      return ticket
  }

  const fetchTickets = async () => {
    if (!isAuthenticated || !user) return

    try {
      setLoading(true)
      const res = await movieClient.payments.getByUser(user.id)
      // Adapt to ApiResponse structure
      const pData = res as any
      const rawData = pData.data || pData
      
      const enrichedData = await Promise.all((Array.isArray(rawData) ? rawData : []).map(enrichTicketData))

      const sortedData = enrichedData.sort((a: any, b: any) => 
          new Date(b.payment_datetime).getTime() - new Date(a.payment_datetime).getTime()
      )
      
      setTickets(sortedData)
    } catch (error) {
      console.error("Error fetching tickets:", error)
      toast.error("Failed to load tickets")
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    if (!authLoading && !isAuthenticated) {
        router.push("/login")
        return
    }
    fetchTickets()
  }, [isAuthenticated, authLoading])

  const handleCancelTicket = async (id: number) => {
      try {
          toast.success(`Đã gửi yêu cầu hủy vé #${id}`)
          fetchTickets()
      } catch (error) {
          toast.error("Không thể hủy vé. Vui lòng thử lại.")
      }
  }

  const handlePayTicket = (id: number) => {
      toast.info("Chức năng thanh toán lại đang phát triển")
  }

  const filteredTickets = useMemo(() => {
    let filtered = tickets

    if (search.trim()) {
        const lowerSearch = search.toLowerCase()
        filtered = filtered.filter(t => 
            t.movie?.title.toLowerCase().includes(lowerSearch) || 
            t.payment_id.toString().includes(lowerSearch)
        )
    }

    if (activeTab === "upcoming") {
        filtered = filtered.filter(t => isFuture(new Date(t.payment_datetime)))
    } else if (activeTab === "past") {
        filtered = filtered.filter(t => isPast(new Date(t.payment_datetime)))
    }

    return filtered
  }, [tickets, search, activeTab])

  return (
    <div className="min-h-screen bg-background pt-24 pb-20 px-4 md:px-8 transition-colors duration-300">
      <div className="container mx-auto max-w-5xl space-y-8">
        
        <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
            <div>
                <h1 className="text-3xl md:text-4xl font-black tracking-tight flex items-center gap-3 text-foreground">
                    <Ticket className="w-8 h-8 md:w-10 md:h-10 text-primary" />
                    Vé Của Tôi
                </h1>
                <p className="text-muted-foreground mt-1">Quản lý vé đã đặt và lịch sử xem phim.</p>
            </div>
            
            <div className="relative w-full md:w-72">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                <Input 
                    placeholder="Tìm tên phim, mã vé..." 
                    className="pl-9 h-11 bg-card border-border shadow-sm rounded-xl"
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                />
            </div>
        </div>

        <Tabs defaultValue="upcoming" value={activeTab} onValueChange={setActiveTab} className="w-full">
            <TabsList className="grid w-full md:w-[400px] grid-cols-3 h-12 bg-muted/50 p-1 border border-border rounded-xl mb-8">
                <TabsTrigger value="upcoming" className="rounded-lg text-xs font-bold uppercase data-[state=active]:bg-background data-[state=active]:text-primary data-[state=active]:shadow-sm">Sắp Chiếu</TabsTrigger>
                <TabsTrigger value="past" className="rounded-lg text-xs font-bold uppercase data-[state=active]:bg-background data-[state=active]:text-primary data-[state=active]:shadow-sm">Lịch Sử</TabsTrigger>
                <TabsTrigger value="all" className="rounded-lg text-xs font-bold uppercase data-[state=active]:bg-background data-[state=active]:text-primary data-[state=active]:shadow-sm">Tất Cả</TabsTrigger>
            </TabsList>

            <div className="space-y-6 min-h-[400px]">
                {loading ? (
                    Array.from({ length: 3 }).map((_, i) => (
                        <div key={i} className="flex flex-col md:flex-row gap-4 p-4 border rounded-2xl bg-card">
                            <Skeleton className="w-full md:w-32 h-40 md:h-32 rounded-xl" />
                            <div className="flex-1 space-y-3 py-2">
                                <Skeleton className="w-3/4 h-6" />
                                <Skeleton className="w-1/2 h-4" />
                                <Skeleton className="w-1/3 h-4" />
                            </div>
                        </div>
                    ))
                ) : filteredTickets.length > 0 ? (
                    <div className="grid gap-6 animate-in fade-in slide-in-from-bottom-4 duration-500">
                        {filteredTickets.map((ticket) => (
                            <TicketCard 
                                key={ticket.payment_id} 
                                ticket={ticket} 
                                onCancel={handleCancelTicket}
                                onPay={handlePayTicket}
                                onClick={() => router.push(`/booking/success?paymentId=${ticket.payment_id}`)}
                            />
                        ))}
                    </div>
                ) : (
                    <div className="flex flex-col items-center justify-center py-20 border-2 border-dashed border-border rounded-3xl bg-card/50">
                        <div className="w-20 h-20 bg-muted rounded-full flex items-center justify-center mb-4">
                            {activeTab === 'upcoming' ? <Calendar className="w-10 h-10 opacity-30" /> : <History className="w-10 h-10 opacity-30" />}
                        </div>
                        <h3 className="text-xl font-bold text-foreground">Không tìm thấy vé</h3>
                        <p className="text-muted-foreground text-sm mt-1 max-w-xs text-center">
                            {activeTab === 'upcoming' 
                                ? "Bạn chưa có lịch chiếu nào sắp tới. Đặt vé ngay thôi!" 
                                : "Không tìm thấy lịch sử vé nào phù hợp."}
                        </p>
                        {activeTab === 'upcoming' && (
                            <Button className="mt-6 rounded-full font-bold px-8" onClick={() => router.push('/movies')}>
                                Đặt vé ngay
                            </Button>
                        )}
                    </div>
                )}
            </div>
        </Tabs>

      </div>
    </div>
  )
}
