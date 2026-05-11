import { Button } from "@/registry/new-york-v4/ui/button"
import { Star, Film, Award } from "lucide-react"

export default function PersonDetailPage() {
  return (
    <div className="min-h-screen bg-background pt-24 pb-20 px-4 md:px-8">
      <div className="container mx-auto max-w-5xl space-y-12">
        <div className="flex flex-col md:flex-row gap-8 md:gap-12 items-start">
          <div className="w-full md:w-80 shrink-0 aspect-[2/3] bg-muted rounded-3xl overflow-hidden relative shadow-2xl">
             <div className="absolute inset-0 flex items-center justify-center text-muted-foreground">
               <Film className="h-12 w-12 opacity-20" />
             </div>
          </div>
          
          <div className="flex-1 space-y-6">
            <div className="space-y-2">
              <h1 className="text-4xl md:text-6xl font-black tracking-tighter">Robert Downey Jr.</h1>
              <p className="text-xl text-primary font-bold">Actor, Producer</p>
            </div>

            <div className="flex gap-4 text-sm font-medium">
              <div className="flex items-center gap-1">
                <Star className="h-4 w-4 text-yellow-500 fill-yellow-500" />
                <span>Top Rated</span>
              </div>
              <div className="flex items-center gap-1">
                <Award className="h-4 w-4 text-primary" />
                <span>2 Oscars</span>
              </div>
            </div>

            <div className="space-y-4">
              <h2 className="text-xl font-bold">Biography</h2>
              <p className="text-muted-foreground leading-relaxed">
                Robert John Downey Jr. is an American actor and producer. His career has been characterized by critical and popular success in his youth, followed by a period of substance abuse and legal difficulties, before a resurgence of commercial success later in his career.
              </p>
            </div>

            <div className="pt-4 border-t border-dashed">
              <h2 className="text-xl font-bold mb-4">Notable Works</h2>
              <div className="grid grid-cols-2 sm:grid-cols-3 gap-4">
                {[1, 2, 3].map((i) => (
                  <div key={i} className="aspect-video bg-muted rounded-xl flex items-center justify-center text-xs font-bold text-muted-foreground">
                    Movie Poster {i}
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
