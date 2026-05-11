"use client"

import { Suspense } from "react"
import { useRouter } from "next/navigation"
import { ChevronLeft } from "lucide-react"

import { OtpForm } from "@/app/(auth)/_components/otp-form"
import { Button } from "@/registry/new-york-v4/ui/button"
import { cn } from "@/lib/utils"

export default function OtpPage() {
  const router = useRouter()

  return (
    <div className="bg-background flex min-h-svh flex-col items-center justify-center gap-6 p-6 md:p-10">
      <Button
        variant="ghost"
        className="absolute left-4 top-4 md:left-8 md:top-8"
        onClick={() => router.back()}
      >
        <ChevronLeft className="mr-2 h-4 w-4" />
        Back
      </Button>
      <div className="w-full max-w-sm">
        <Suspense fallback={<div>Loading...</div>}>
          <OtpForm />
        </Suspense>
      </div>
    </div>
  )
}
