"use client"

import { useQuery } from "@tanstack/react-query"
import { Button } from "@/registry/new-york-v4/ui/button"
import { Plus, CalendarDays, Loader2, Search } from "lucide-react"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/registry/new-york-v4/ui/table"
import { Input } from "@/registry/new-york-v4/ui/input"
import movieClient from "@/lib/api"
import { format } from "date-fns"

export default function AdminShowsPage() {
  const { data: shows, isLoading } = useQuery({
    queryKey: ["admin", "shows", "all"],
    queryFn: () => movieClient.shows.getFiltered("") // Or getAll if available
  })

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Showtimes Management</h1>
          <p className="text-muted-foreground">Schedule movie screenings across different theaters and halls.</p>
        </div>
        <Button>
          <Plus className="mr-2 h-4 w-4" /> Create Showtime
        </Button>
      </div>

      <div className="flex items-center gap-2">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input placeholder="Search showtimes..." className="pl-9" />
        </div>
      </div>

      <div className="rounded-md border bg-card">
        {isLoading ? (
          <div className="flex h-40 items-center justify-center">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
          </div>
        ) : shows && (shows as any).data && (shows as any).data.length > 0 ? (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Movie</TableHead>
                <TableHead>Theater</TableHead>
                <TableHead>Date</TableHead>
                <TableHead>Time</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {(shows as any).data.map((show: any) => (
                <TableRow key={show.id}>
                  <TableCell className="font-medium">{show.movieTitle}</TableCell>
                  <TableCell>{show.theaterName}</TableCell>
                  <TableCell>{format(new Date(show.date), "dd MMM yyyy")}</TableCell>
                  <TableCell>{show.startTime}</TableCell>
                  <TableCell className="text-right">
                    <Button variant="ghost" size="sm">Edit</Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        ) : (
          <div className="p-20 flex flex-col items-center justify-center text-center">
            <div className="h-12 w-12 rounded-full bg-muted flex items-center justify-center mb-4">
              <CalendarDays className="h-6 w-6 text-muted-foreground" />
            </div>
            <h3 className="text-lg font-semibold">No showtimes scheduled</h3>
            <p className="text-sm text-muted-foreground mt-1 max-w-xs">
              Start by selecting a movie and a theater to create your first screening schedule.
            </p>
            <Button className="mt-4" variant="outline">Schedule Now</Button>
          </div>
        )}
      </div>
    </div>
  )
}
