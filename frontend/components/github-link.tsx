"use client"

import * as React from "react"
import Link from "next/link"

import { siteConfig } from "@/lib/config"
import { Icons } from "@/components/icons"
import { Button } from "@/registry/new-york-v4/ui/button"
import { Skeleton } from "@/registry/new-york-v4/ui/skeleton"

export function GitHubLink() {
  return (
    <Button asChild size="sm" variant="ghost" className="h-8 shadow-none">
      <Link href={siteConfig.links.github} target="_blank" rel="noreferrer">
        <Icons.gitHub />
        <React.Suspense fallback={<Skeleton className="h-4 w-[42px]" />}>
          <StarsCount />
        </React.Suspense>
      </Link>
    </Button>
  )
}

export function StarsCount() {
  const [stars, setStars] = React.useState<string | null>(null)

  React.useEffect(() => {
    fetch("https://api.github.com/repos/NguyenHuuDinh135/Rate")
      .then((res) => res.json())
      .then((json) => {
        const formattedCount =
          json.stargazers_count >= 1000
            ? `${Math.round(json.stargazers_count / 1000)}k`
            : json.stargazers_count?.toLocaleString() || "0"
        setStars(formattedCount)
      })
      .catch(() => setStars("0"))
  }, [])

  if (stars === null) {
    return <Skeleton className="h-4 w-[42px]" />
  }

  return (
    <span className="text-muted-foreground w-fit text-xs tabular-nums">
      {stars}
    </span>
  )
}
