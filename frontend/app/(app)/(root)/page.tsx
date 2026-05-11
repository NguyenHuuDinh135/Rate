"use client"

import { useState, useRef, useEffect, useMemo } from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import { Play, Info, Star, ChevronLeft, ChevronRight, Sparkles, TrendingUp, Calendar, Ticket } from "lucide-react"
import gsap from "gsap"
import { useGSAP } from "@gsap/react"
import { ScrollTrigger } from "gsap/ScrollTrigger"

// Shadcn UI
import { Button } from "@/registry/new-york-v4/ui/button"
import { Badge } from "@/registry/new-york-v4/ui/badge"
import { Skeleton } from "@/registry/new-york-v4/ui/skeleton"

// API
import movieClient, { type MovieDto } from "@/lib/api"

// Register GSAP
if (typeof window !== "undefined") {
  gsap.registerPlugin(ScrollTrigger)
}

/* ==================== HERO COMPONENT ==================== */
function Hero({ movies }: { movies: MovieDto[] }) {
  const router = useRouter()
  const [currentIndex, setCurrentIndex] = useState(0)
  const containerRef = useRef<HTMLDivElement>(null)
  
  const heroMovies = useMemo(() => movies.slice(0, 5), [movies])
  const currentMovie = heroMovies[currentIndex]

  useEffect(() => {
    if (heroMovies.length === 0) return
    const timer = setInterval(() => {
      setCurrentIndex((prev) => (prev + 1) % heroMovies.length)
    }, 7000)
    return () => clearInterval(timer)
  }, [heroMovies.length, currentIndex])

  useGSAP(() => {
    if (!containerRef.current) return
    
    const elements = containerRef.current.querySelectorAll(".hero-animate")
    
    gsap.fromTo(elements, 
      { y: 30, opacity: 0 },
      { y: 0, opacity: 1, duration: 0.8, stagger: 0.1, ease: "power3.out", delay: 0.1 }
    )
  }, { dependencies: [currentIndex], scope: containerRef })

  if (!currentMovie) return null

  return (
    <div ref={containerRef} className="relative h-[90vh] overflow-hidden bg-background">
      
      {heroMovies.map((item, index) => (
        <div
          key={item.id}
          className={`absolute inset-0 transition-all duration-1000 ease-in-out ${
            index === currentIndex 
              ? "opacity-100 scale-100 z-10" 
              : "opacity-0 scale-110 z-0"
          }`}
        >
          <img 
            src={item.posterUrl} 
            className="h-full w-full object-cover" 
            alt={item.title}
          />
          <div className="absolute inset-0 bg-gradient-to-r from-background via-background/70 to-transparent" />
          <div className="absolute inset-0 bg-gradient-to-t from-background via-transparent to-transparent" />
        </div>
      ))}

      <div className="relative z-20 container h-full flex items-center px-4 md:px-8">
        <div className="max-w-3xl space-y-6">
          
          <div className="hero-animate flex items-center gap-3">
            <Badge className="bg-gradient-to-r from-yellow-500 to-orange-500 text-black border-0 px-4 py-1.5 text-sm font-bold shadow-lg shadow-yellow-500/20">
              <TrendingUp className="w-4 h-4 mr-1" />
              {currentMovie.movieType === 'NowShowing' ? 'ĐANG CHIẾU' : 'SẮP CHIẾU'}
            </Badge>
            <div className="flex items-center gap-2 bg-background/50 backdrop-blur-sm px-3 py-1.5 rounded-full border border-white/10">
              <Star className="w-4 h-4 fill-yellow-500 text-yellow-500" />
              <span className="font-bold text-sm text-white">
                {currentMovie.rating ? currentMovie.rating.toFixed(1) : "TBA"}
              </span>
            </div>
          </div>

          <h1 className="hero-animate text-5xl md:text-7xl font-black leading-[0.95] tracking-tight text-white drop-shadow-2xl">
            {currentMovie.title}
            <span className="block text-3xl md:text-4xl text-gray-300 font-normal mt-2">
              ({currentMovie.year})
            </span>
          </h1>

          <div className="hero-animate flex flex-wrap gap-2">
            {currentMovie.genres?.slice(0, 3).map((genre) => (
              <Badge key={genre.id} variant="outline" className="border-white/20 text-gray-200 backdrop-blur-md bg-white/5">
                {genre.name}
              </Badge>
            ))}
          </div>

          <p className="hero-animate text-base md:text-lg text-gray-300 leading-relaxed line-clamp-3 max-w-2xl drop-shadow-md">
            {currentMovie.summary}
          </p>

          <div className="hero-animate flex flex-wrap gap-3 pt-2">
            <Button 
              size="lg" 
              onClick={() => router.push(`/booking/${currentMovie.id}`)}
              className="bg-primary hover:bg-primary/90 font-bold h-14 px-8 text-base shadow-lg hover:scale-105 transition-transform"
            >
              <Ticket className="w-5 h-5 mr-2 fill-current" />
              Đặt Vé Ngay
            </Button>
            <Button 
              size="lg" 
              variant="outline" 
              onClick={() => router.push(`/movies/${currentMovie.id}`)}
              className="backdrop-blur-sm bg-white/10 border-white/20 text-white hover:bg-white/20 h-14 px-8 font-semibold hover:scale-105 transition-transform"
            >
              <Info className="w-5 h-5 mr-2" />
              Chi Tiết
            </Button>
          </div>
        </div>
      </div>

      <div className="absolute bottom-8 right-8 z-30 hidden lg:flex gap-3">
        {heroMovies.map((item, index) => (
          <button
            key={item.id}
            onClick={() => setCurrentIndex(index)}
            className={`group relative w-20 h-28 rounded-lg overflow-hidden transition-all duration-300 border-2 cursor-pointer ${
              index === currentIndex 
                ? "border-primary scale-110 shadow-xl shadow-primary/30" 
                : "border-transparent opacity-60 hover:opacity-100 hover:scale-105"
            }`}
          >
            <img src={item.posterUrl} alt={item.title} className="w-full h-full object-cover" />
          </button>
        ))}
      </div>

      <div className="absolute bottom-6 left-1/2 -translate-x-1/2 z-30 flex gap-2 lg:hidden">
        {heroMovies.map((_, index) => (
          <button
            key={index}
            onClick={() => setCurrentIndex(index)}
            className={`h-1.5 rounded-full transition-all duration-300 ${
              index === currentIndex ? "w-8 bg-primary shadow-lg" : "w-1.5 bg-white/50"
            }`}
          />
        ))}
      </div>
    </div>
  )
}

function MovieCard({ movie, index = 0 }: { movie: MovieDto; index?: number }) {
  const [imageLoaded, setImageLoaded] = useState(false)
  const cardRef = useRef<HTMLDivElement>(null)
  const router = useRouter()

  useGSAP(() => {
    if (!cardRef.current) return
    
    const delayTime = (index % 12) * 0.05

    gsap.fromTo(cardRef.current, 
      { opacity: 0, y: 50, scale: 0.9 },
      { 
        opacity: 1, 
        y: 0, 
        scale: 1, 
        duration: 0.6, 
        delay: delayTime,
        ease: "back.out(1.2)",
        scrollTrigger: {
          trigger: cardRef.current,
          start: "top 95%",
          toggleActions: "play none none none"
        }
      }
    )
  }, { scope: cardRef })

  return (
    <div 
      ref={cardRef} 
      className="group cursor-pointer opacity-0"
      onClick={() => router.push(`/movies/${movie.id}`)}
    >
      <div className="relative aspect-[2/3] overflow-hidden rounded-xl bg-muted border border-border/50 shadow-lg hover:shadow-2xl transition-all duration-500">
        {!imageLoaded && <Skeleton className="absolute inset-0" />}
        <img 
          src={movie.posterUrl} 
          className={`h-full w-full object-cover transition-all duration-700 ${
            imageLoaded ? 'opacity-100 group-hover:scale-110 group-hover:rotate-1' : 'opacity-0'
          }`}
          loading="lazy"
          alt={movie.title}
          onLoad={() => setImageLoaded(true)}
        />
        
        <div className="absolute top-3 left-3 bg-black/70 backdrop-blur-md text-white text-xs font-bold px-2.5 py-1 rounded-full flex items-center gap-1 border border-white/10">
          <Star className="w-3 h-3 fill-yellow-500 text-yellow-500" />
          {movie.rating ? movie.rating.toFixed(1) : "TBA"}
        </div>

        <div className="absolute inset-0 bg-black/60 opacity-0 group-hover:opacity-100 transition-all duration-300 flex items-center justify-center backdrop-blur-[2px]">
          <Button size="sm" className="rounded-full h-12 w-12 p-0 bg-primary hover:bg-primary/90 shadow-lg transform scale-0 group-hover:scale-100 transition-transform duration-300">
            <Play className="w-5 h-5 ml-1 fill-current" />
          </Button>
        </div>
      </div>

      <div className="mt-3 space-y-1">
        <h3 className="text-sm font-bold truncate group-hover:text-primary transition-colors">
          {movie.title}
        </h3>
        <p className="text-xs text-muted-foreground truncate flex items-center gap-1.5">
          {movie.genres?.[0]?.name || "Phim rạp"} 
          <span className="text-[10px] opacity-50">•</span>
          <span className="flex items-center gap-0.5">
            <Calendar className="w-3 h-3" />
            {movie.year}
          </span>
        </p>
      </div>
    </div>
  )
}

function MovieSection({ title, movies, icon }: { title: string; movies: MovieDto[]; icon?: React.ReactNode }) {
  const ref = useRef<HTMLDivElement>(null)
  const containerRef = useRef<HTMLDivElement>(null)
  const [canScrollLeft, setCanScrollLeft] = useState(false)
  const [canScrollRight, setCanScrollRight] = useState(true)

  useGSAP(() => {
    if (!containerRef.current) return
    gsap.from(containerRef.current, {
      opacity: 0,
      y: 60,
      duration: 1,
      ease: "power3.out",
      scrollTrigger: {
        trigger: containerRef.current,
        start: "top 85%",
      }
    })
  }, { scope: containerRef })

  const checkScroll = () => {
    if (ref.current) {
      const { scrollLeft, scrollWidth, clientWidth } = ref.current
      setCanScrollLeft(scrollLeft > 10)
      setCanScrollRight(scrollLeft < scrollWidth - clientWidth - 10)
    }
  }

  const scroll = (dir: "left" | "right") => {
    if (!ref.current) return
    const scrollAmount = ref.current.clientWidth * 0.8
    ref.current.scrollBy({ 
      left: dir === "left" ? -scrollAmount : scrollAmount, 
      behavior: "smooth" 
    })
    setTimeout(checkScroll, 300)
  }

  useEffect(() => {
    checkScroll()
    window.addEventListener('resize', checkScroll)
    return () => window.removeEventListener('resize', checkScroll)
  }, [movies])

  if (movies.length === 0) return null

  return (
    <div ref={containerRef} className="space-y-5">
      <div className="flex justify-between items-center">
        <div className="flex items-center gap-3">
          {icon}
          <h2 className="text-2xl md:text-3xl font-bold bg-gradient-to-r from-foreground to-foreground/70 bg-clip-text text-transparent">
            {title}
          </h2>
        </div>
        <div className="flex gap-2">
          <Button size="icon" variant="outline" onClick={() => scroll("left")} disabled={!canScrollLeft} className="rounded-full">
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <Button size="icon" variant="outline" onClick={() => scroll("right")} disabled={!canScrollRight} className="rounded-full">
            <ChevronRight className="h-4 w-4" />
          </Button>
        </div>
      </div>

      <div 
        ref={ref} 
        onScroll={checkScroll}
        className="flex gap-5 overflow-x-auto no-scrollbar snap-x scroll-smooth pb-4 px-1"
        style={{ scrollbarWidth: "none", msOverflowStyle: "none" }}
      >
        {movies.map((m, i) => (
          <div key={m.id} className="w-44 md:w-52 flex-none snap-start">
            <MovieCard movie={m} index={i} />
          </div>
        ))}
      </div>
    </div>
  )
}

function LoadingState() {
  return (
    <div className="bg-background min-h-screen">
      <div className="relative h-[90vh] overflow-hidden">
        <Skeleton className="absolute inset-0" />
      </div>
      <div className="container py-12 space-y-12 px-4 md:px-8">
        {[1, 2].map((section) => (
          <div key={section} className="space-y-5">
            <Skeleton className="h-10 w-64" />
            <div className="flex gap-5 overflow-hidden">
              {[1, 2, 3, 4, 5, 6].map((i) => (
                <Skeleton key={i} className="h-72 w-44 md:w-52 rounded-xl flex-none" />
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default function IndexPage() {
  const [visibleCount, setVisibleCount] = useState(12) 
  const [nowShowing, setNowShowing] = useState<MovieDto[]>([])
  const [comingSoon, setComingSoon] = useState<MovieDto[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true)
        // Adjusting to what our movieClient expects or what the backend actually returns
        // MovieEndpoints.cs says GetFiltered takes title, type, year as query params.
        const [nowRes, comingRes] = await Promise.all([
          movieClient.movies.getFiltered("NowShowing"),
          movieClient.movies.getFiltered("ComingSoon")
        ])

        setNowShowing(nowRes || [])
        setComingSoon(comingRes || [])
      } catch (error) {
        console.error("Failed to load movies", error)
      } finally {
        setLoading(false)
      }
    }

    fetchData()
  }, [])

  if (loading) return <LoadingState />

  const allMovies = [...nowShowing, ...comingSoon]
  const discoveryMovies = allMovies.slice(0, visibleCount)

  return (
    <div className="bg-background min-h-screen pb-20">
      
      <Hero movies={nowShowing} />

      <div className="container py-16 space-y-20 px-4 md:px-8">
        
        <MovieSection 
          title="Phim đang chiếu" 
          movies={nowShowing}
          icon={<Play className="w-7 h-7 text-primary fill-primary/20" />}
        />

        <MovieSection 
          title="Phim sắp chiếu" 
          movies={comingSoon}
          icon={<Calendar className="w-7 h-7 text-primary" />}
        />

        <div className="space-y-8">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <Sparkles className="w-7 h-7 text-primary" />
              <h2 className="text-2xl md:text-3xl font-bold">Khám phá</h2>
            </div>
            <Badge variant="outline" className="text-sm px-4 py-1.5 border-primary/30 text-primary">
              {allMovies.length} phim
            </Badge>
          </div>
          
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 gap-5 md:gap-6">
            {discoveryMovies.map((m, i) => (
              <MovieCard key={`grid-${m.id}`} movie={m} index={i} />
            ))}
          </div>

          {visibleCount < allMovies.length && (
            <div className="flex justify-center pt-6">
              <Button 
                variant="outline" 
                size="lg" 
                onClick={() => setVisibleCount(v => v + 12)}
                className="min-w-[180px] h-12 font-semibold hover:bg-primary hover:text-primary-foreground hover:border-primary transition-all"
              >
                Tải thêm phim
              </Button>
            </div>
          )}
        </div>
      </div>

      <style jsx global>{`
        .scrollbar-hide::-webkit-scrollbar { display: none; }
        .scrollbar-hide { -ms-overflow-style: none; scrollbar-width: none; }
      `}</style>
    </div>
  )
}
