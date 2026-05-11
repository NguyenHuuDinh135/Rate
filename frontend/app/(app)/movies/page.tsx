"use client"

import { useState, useEffect } from "react"
import { Search, Filter, Compass, Star } from "lucide-react"
import { useRouter } from "next/navigation"

// UI
import { Input } from "@/registry/new-york-v4/ui/input"
import { Button } from "@/registry/new-york-v4/ui/button"
import { Badge } from "@/registry/new-york-v4/ui/badge"
import { Skeleton } from "@/registry/new-york-v4/ui/skeleton"

// API
import movieClient, { type MovieDto, type GenreDto } from "@/lib/api"

export default function MoviesPage() {
  const router = useRouter()
  const [movies, setMovies] = useState<MovieDto[]>([])
  const [genres, setGenres] = useState<GenreDto[]>([])
  const [selectedGenre, setSelectedGenre] = useState<number | null>(null)
  const [search, setSearch] = useState("")
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const loadData = async () => {
        try {
            setLoading(true)
            const [mRes, gRes] = await Promise.all([
                movieClient.movies.getAll(),
                movieClient.genres.getAll()
            ])
            setMovies(mRes || [])
            setGenres(gRes || [])
        } catch (error) {
            console.error("Failed to load movies data", error)
        } finally {
            setLoading(false)
        }
    }
    loadData()
  }, [])

  const filteredMovies = movies.filter(m => {
    const matchSearch = m.title.toLowerCase().includes(search.toLowerCase())
    const matchGenre = selectedGenre ? m.genres?.some(g => g.id === selectedGenre) : true
    return matchSearch && matchGenre
  })

  return (
    <div className="min-h-screen bg-background pt-24 px-4 pb-20">
      <div className="container mx-auto max-w-7xl space-y-10">
        
        {/* HERO HEADER */}
        <div className="flex flex-col md:flex-row justify-between items-end gap-6 animate-in slide-in-from-top-4 duration-500">
            <div>
                <h1 className="text-4xl md:text-6xl font-black tracking-tighter uppercase flex items-center gap-3 text-foreground">
                    <Compass className="w-10 h-10 md:w-14 md:h-14 text-primary animate-pulse" />
                    Explore
                </h1>
                <p className="text-muted-foreground mt-2 text-lg">Discover your next cinematic obsession.</p>
            </div>
            
            {/* Search Input */}
            <div className="relative w-full md:w-96 group">
                <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground w-5 h-5 group-focus-within:text-primary transition-colors" />
                <Input 
                    placeholder="Search movies, genres..." 
                    className="pl-12 h-14 rounded-full border-2 border-border focus-visible:border-primary text-base shadow-sm bg-background/50 backdrop-blur-sm"
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                />
            </div>
        </div>

        {/* GENRE TAGS */}
        <div className="flex flex-wrap gap-2 pb-4 border-b border-border/50">
            <Badge 
                variant={selectedGenre === null ? "default" : "outline"} 
                className="cursor-pointer px-5 py-2 text-sm rounded-full h-9 hover:opacity-90 transition-all active:scale-95"
                onClick={() => setSelectedGenre(null)}
            >
                All
            </Badge>
            {loading ? Array.from({length: 5}).map((_, i) => <Skeleton key={i} className="w-20 h-9 rounded-full" />) : 
                genres.map(g => (
                <Badge 
                    key={g.id} 
                    variant={selectedGenre === g.id ? "default" : "secondary"}
                    className={`cursor-pointer px-4 py-2 text-sm rounded-full h-9 transition-all hover:scale-105 active:scale-95 ${selectedGenre === g.id ? 'shadow-md shadow-primary/20' : 'bg-muted/50 hover:bg-muted'}`}
                    onClick={() => setSelectedGenre(g.id)}
                >
                    {g.name}
                </Badge>
            ))}
        </div>

        {/* MOVIE GRID */}
        {loading ? (
            <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-5 gap-6">
                {Array.from({ length: 10 }).map((_, i) => (
                    <div key={i} className="space-y-3">
                        <Skeleton className="aspect-[2/3] w-full rounded-2xl" />
                        <Skeleton className="h-4 w-3/4" />
                        <Skeleton className="h-3 w-1/2" />
                    </div>
                ))}
            </div>
        ) : (
            <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-x-6 gap-y-10 animate-in fade-in duration-700">
                {filteredMovies.map((movie) => (
                    <div 
                        key={movie.id} 
                        onClick={() => router.push(`/movies/${movie.id}`)}
                        className="group cursor-pointer flex flex-col gap-3"
                    >
                        <div className="relative aspect-[2/3] rounded-2xl overflow-hidden shadow-sm border border-border bg-muted transition-all duration-300 group-hover:shadow-2xl group-hover:shadow-primary/10 group-hover:-translate-y-2">
                            <img 
                                src={movie.posterUrl} 
                                alt={movie.title}
                                className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110"
                                loading="lazy"
                            />
                            <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
                            
                            {movie.rating && (
                                <div className="absolute top-3 right-3 bg-black/60 backdrop-blur-md px-2 py-1 rounded-lg flex items-center gap-1 border border-white/10 opacity-0 group-hover:opacity-100 transition-opacity delay-100">
                                    <Star className="w-3 h-3 text-yellow-500 fill-yellow-500" />
                                    <span className="text-xs font-bold text-white">{movie.rating}</span>
                                </div>
                            )}
                        </div>

                        <div>
                            <h3 className="font-bold text-base leading-tight truncate group-hover:text-primary transition-colors text-foreground">{movie.title}</h3>
                            <div className="flex items-center justify-between mt-1 text-xs text-muted-foreground">
                                <span>{movie.year}</span>
                                <span className="truncate max-w-[60%] text-right">{movie.genres?.[0]?.name}</span>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        )}
        
        {/* Empty State */}
        {!loading && filteredMovies.length === 0 && (
            <div className="py-20 text-center space-y-4">
                <div className="inline-flex items-center justify-center w-20 h-20 rounded-full bg-muted">
                    <Search className="w-8 h-8 text-muted-foreground" />
                </div>
                <div>
                    <h3 className="text-xl font-bold text-foreground">No movies found</h3>
                    <p className="text-muted-foreground">Try adjusting your search or filters.</p>
                </div>
                <Button variant="outline" onClick={() => { setSearch(""); setSelectedGenre(null); }}>
                    Clear Filters
                </Button>
            </div>
        )}
      </div>
    </div>
  )
}
