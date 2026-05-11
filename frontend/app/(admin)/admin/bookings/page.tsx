import { 
  Table, 
  TableBody, 
  TableCell, 
  TableHead, 
  TableHeader, 
  TableRow 
} from "@/registry/new-york-v4/ui/table"
import { Badge } from "@/registry/new-york-v4/ui/badge"

export default function AdminBookingsPage() {
  return (
    <div className="space-y-4">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">Bookings & Transactions</h1>
        <p className="text-muted-foreground">Monitor ticket sales and payment statuses across the platform.</p>
      </div>

      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Booking ID</TableHead>
              <TableHead>Customer</TableHead>
              <TableHead>Movie</TableHead>
              <TableHead>Amount</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Date</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <TableRow>
              <TableCell className="font-mono">#BK-9021</TableCell>
              <TableCell>John Doe</TableCell>
              <TableCell>Avengers: Endgame</TableCell>
              <TableCell>$24.00</TableCell>
              <TableCell>
                <Badge className="bg-green-500/10 text-green-500 hover:bg-green-500/20 border-green-500/20">Completed</Badge>
              </TableCell>
              <TableCell>May 12, 2026</TableCell>
            </TableRow>
            <TableRow>
              <TableCell className="font-mono">#BK-9022</TableCell>
              <TableCell>Jane Smith</TableCell>
              <TableCell>Interstellar</TableCell>
              <TableCell>$15.00</TableCell>
              <TableCell>
                <Badge variant="outline" className="bg-yellow-500/10 text-yellow-500 hover:bg-yellow-500/20 border-yellow-500/20">Pending</Badge>
              </TableCell>
              <TableCell>May 12, 2026</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </div>
    </div>
  )
}
