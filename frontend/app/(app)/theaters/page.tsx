import { MapPin, Phone, Info } from "lucide-react"
import { Button } from "@/registry/new-york-v4/ui/button"

export default function TheatersPage() {
  return (
    <div className="min-h-screen bg-background pt-24 pb-20 px-4 md:px-8">
      <div className="container mx-auto max-w-6xl space-y-12">
        <div className="text-center space-y-4">
          <h1 className="text-4xl md:text-5xl font-black tracking-tighter">Our Cinemas</h1>
          <p className="text-muted-foreground text-lg max-w-2xl mx-auto">
            Find the nearest cinema location and enjoy the ultimate movie experience with state-of-the-art technology.
          </p>
        </div>

        <div className="grid gap-8 md:grid-cols-2 lg:grid-cols-3">
          {[1, 2, 3, 4, 5, 6].map((i) => (
            <div key={i} className="group rounded-3xl border bg-card overflow-hidden hover:shadow-2xl transition-all duration-500 hover:-translate-y-2">
              <div className="h-48 bg-muted relative">
                <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent" />
                <div className="absolute bottom-4 left-4 flex items-center gap-2 text-white">
                  <MapPin className="h-4 w-4" />
                  <span className="text-sm font-medium">District {i}, Saigon</span>
                </div>
              </div>
              <div className="p-6 space-y-4">
                <h3 className="text-xl font-bold group-hover:text-primary transition-colors">Grand Cinema Complex {i}</h3>
                <div className="space-y-2 text-sm text-muted-foreground">
                  <div className="flex items-center gap-2">
                    <Phone className="h-4 w-4" />
                    <span>+84 123 456 78{i}</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Info className="h-4 w-4" />
                    <span>IMAX, 4DX, Gold Class available</span>
                  </div>
                </div>
                <Button className="w-full rounded-full font-bold">View Showtimes</Button>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}
