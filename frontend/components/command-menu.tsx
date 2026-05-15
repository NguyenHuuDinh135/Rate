"use client"

import * as React from "react"
import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { Search, Film, User, Calendar, Star, Loader2 } from "lucide-react"

import { 
  Command, 
  CommandEmpty, 
  CommandGroup, 
  CommandInput, 
  CommandItem, 
  CommandList 
} from "@/registry/new-york-v4/ui/command"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/registry/new-york-v4/ui/dialog"
import { Button } from "@/registry/new-york-v4/ui/button"
import { cn } from "@/lib/utils"
import movieClient, { type MovieDto } from "@/lib/api"
import { Badge } from "@/registry/new-york-v4/ui/badge"

export function CommandMenu() {
  const router = useRouter()
  const [open, setOpen] = useState(false)
  const [query, setQuery] = useState("")
  const [results, setResults] = useState<MovieDto[]>([])
  const [isLoading, setIsLoading] = useState(false)

  React.useEffect(() => {
    const down = (e: KeyboardEvent) => {
      if ((e.key === "k" && (e.metaKey || e.ctrlKey)) || e.key === "/") {
        if (
          (e.target instanceof HTMLElement && e.target.isContentEditable) ||
          e.target instanceof HTMLInputElement ||
          e.target instanceof HTMLTextAreaElement ||
          e.target instanceof HTMLSelectElement
        ) {
          return
        }

        e.preventDefault()
        setOpen((open) => !open)
      }
    }

    document.addEventListener("keydown", down)
    return () => document.removeEventListener("keydown", down)
  }, [])

  useEffect(() => {
    if (!query) {
      setResults([])
      return
    }

    const searchMovies = async () => {
      setIsLoading(true)
      try {
        // Use the search API which uses Elasticsearch on the backend
        const res = await movieClient.movies.search(query)
        setResults(res || [])
      } catch (error) {
        console.error("Search failed:", error)
        setResults([])
      } finally {
        setIsLoading(false)
      }
    }

    const timer = setTimeout(searchMovies, 300)
    return () => clearTimeout(timer)
  }, [query])

  const onSelect = (id: number) => {
    setOpen(false)
    router.push(`/movies/${id}`)
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button
          variant="secondary"
          className={cn(
            "bg-surface text-foreground dark:bg-card relative h-8 w-full justify-start pl-3 font-medium shadow-none sm:pr-12 md:w-48 lg:w-56 xl:w-64"
          )}
          onClick={() => setOpen(true)}
        >
          <Search className="mr-2 h-4 w-4 text-muted-foreground" />
          <span className="hidden lg:inline-flex">Search for a movie...</span>
          <span className="inline-flex lg:hidden">Search...</span>
          <div className="absolute top-1.5 right-1.5 hidden gap-1 sm:flex">
            <kbd className="pointer-events-none inline-flex h-5 select-none items-center gap-1 rounded border bg-muted px-1.5 font-mono text-[10px] font-medium text-muted-foreground opacity-100">
              <span className="text-xs">⌘</span>K
            </kbd>
          </div>
        </Button>
      </DialogTrigger>

      <DialogContent
        showCloseButton={false}
        className="max-w-2xl overflow-hidden p-0 shadow-2xl"
      >
        <DialogHeader className="sr-only">
          <DialogTitle>Search</DialogTitle>
          <DialogDescription>Search for movies...</DialogDescription>
        </DialogHeader>
        <Command className="rounded-none border-none">
          <CommandInput 
            placeholder="Type movie title..." 
            value={query}
            onValueChange={setQuery}
          />
          <CommandList className="max-h-[400px]">
            {isLoading && (
              <div className="flex items-center justify-center py-6">
                <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
              </div>
            )}
            
            {!isLoading && results.length === 0 && query.length >= 1 && (
              <CommandEmpty>No movies found for "{query}".</CommandEmpty>
            )}

            {!isLoading && results.length > 0 && (
              <CommandGroup heading="Movies">
                {results.map((movie) => (
                  <CommandItem
                    key={movie.id}
                    value={movie.title}
                    onSelect={() => onSelect(movie.id)}
                    className="flex items-center gap-3 p-2 cursor-pointer"
                  >
                    <div className="h-12 w-8 shrink-0 overflow-hidden rounded bg-muted">
                      {movie.posterUrl && (
                        <img 
                          src={movie.posterUrl} 
                          alt={movie.title} 
                          className="h-full w-full object-cover"
                        />
                      )}
                    </div>
                    <div className="flex flex-1 flex-col gap-0.5">
                      <span className="font-bold text-sm">{movie.title}</span>
                      <div className="flex items-center gap-2 text-xs text-muted-foreground">
                        <span className="flex items-center gap-1">
                          <Calendar className="h-3 w-3" />
                          {movie.year}
                        </span>
                        {movie.rating && (
                          <span className="flex items-center gap-1 text-yellow-500">
                            <Star className="h-3 w-3 fill-current" />
                            {movie.rating.toFixed(1)}
                          </span>
                        )}
                        <Badge variant="secondary" className="text-[10px] h-4 px-1 leading-none uppercase">
                          {movie.movieType}
                        </Badge>
                      </div>
                    </div>
                  </CommandItem>
                ))}
              </CommandGroup>
            )}
          </CommandList>
        </Command>
      </DialogContent>
    </Dialog>
  )
}
