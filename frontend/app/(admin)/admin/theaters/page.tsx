import { Button } from "@/registry/new-york-v4/ui/button"
import { Plus, Search, MapPin } from "lucide-react"
import { Input } from "@/registry/new-york-v4/ui/input"

export default function AdminTheatersPage() {
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Theaters Management</h1>
          <p className="text-muted-foreground">Manage cinema locations, halls, and seating arrangements.</p>
        </div>
        <Button>
          <Plus className="mr-2 h-4 w-4" /> Add Theater
        </Button>
      </div>

      <div className="flex items-center gap-2">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input placeholder="Search theaters..." className="pl-9" />
        </div>
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
        {[1, 2, 3].map((i) => (
          <div key={i} className="rounded-xl border bg-card p-4 shadow-sm hover:shadow-md transition-shadow">
            <div className="flex items-start justify-between">
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10 text-primary">
                <MapPin className="h-5 w-5" />
              </div>
              <Button variant="ghost" size="sm">Edit</Button>
            </div>
            <div className="mt-4">
              <h3 className="font-semibold text-lg">Cinema City {i}</h3>
              <p className="text-sm text-muted-foreground mt-1">123 Movie Lane, Hollywood, CA</p>
              <div className="mt-4 flex gap-2">
                <div className="text-xs bg-muted px-2 py-1 rounded">8 Halls</div>
                <div className="text-xs bg-muted px-2 py-1 rounded">1,200 Seats</div>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
