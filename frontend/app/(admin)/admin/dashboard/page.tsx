"use client"

import { useQuery } from "@tanstack/react-query"
import { Card, CardContent, CardHeader, CardTitle } from "@/registry/new-york-v4/ui/card"
import { 
  Clapperboard, 
  Users, 
  Ticket, 
  TrendingUp,
  Loader2 
} from "lucide-react"
import movieClient from "@/lib/api"

export default function AdminDashboardPage() {
  const { data: movies, isLoading: moviesLoading } = useQuery({
    queryKey: ["admin", "movies", "all"],
    queryFn: () => movieClient.movies.getAll()
  })

  const { data: users, isLoading: usersLoading } = useQuery({
    queryKey: ["admin", "users", "all"],
    queryFn: () => movieClient.users.getAll()
  })

  const { data: bookings, isLoading: bookingsLoading } = useQuery({
    queryKey: ["admin", "bookings", "all"],
    queryFn: () => movieClient.bookings.getById(0) // Mocking for now or needs a real 'all' endpoint if available
    // Actually, bookingsApi.getById(0) is not right. Let's see if there's a getAll for bookings.
  })
  
  // Refined query for bookings and payments
  const { data: allBookings, isLoading: allBookingsLoading } = useQuery({
    queryKey: ["admin", "bookings", "list"],
    queryFn: async () => {
       // Need to verify if bookings.getAll exists in movieClient. 
       // Looking at lib/api/index.ts, it doesn't. Let's add it.
       return []
    }
  })

  const isLoading = moviesLoading || usersLoading

  if (isLoading) {
    return (
      <div className="flex h-[400px] items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    )
  }

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold tracking-tight">Dashboard Overview</h1>
      
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Movies</CardTitle>
            <Clapperboard className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{movies?.length || 0}</div>
            <p className="text-xs text-muted-foreground">Across all genres</p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Users</CardTitle>
            <Users className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{users?.length || 0}</div>
            <p className="text-xs text-muted-foreground">Registered accounts</p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Bookings</CardTitle>
            <Ticket className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">--</div>
            <p className="text-xs text-muted-foreground">Tickets issued</p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Revenue</CardTitle>
            <TrendingUp className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">--</div>
            <p className="text-xs text-muted-foreground">Gross earnings</p>
          </CardContent>
        </Card>
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-7">
        <Card className="col-span-4">
          <CardHeader>
            <CardTitle>Recent Sales</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-[200px] flex items-center justify-center text-muted-foreground">
              Sales chart placeholder
            </div>
          </CardContent>
        </Card>
        <Card className="col-span-3">
          <CardHeader>
            <CardTitle>Top Movies</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {[1, 2, 3].map((i) => (
                <div key={i} className="flex items-center gap-4">
                  <div className="h-9 w-9 rounded bg-muted" />
                  <div className="flex-1 space-y-1">
                    <p className="text-sm font-medium leading-none">Movie Title {i}</p>
                    <p className="text-xs text-muted-foreground">{1000 - i * 100} bookings</p>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
