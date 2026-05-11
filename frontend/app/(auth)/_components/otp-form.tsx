"use client"

import * as React from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { useSearchParams } from "next/navigation"

import { useAuth } from "@/hooks/use-auth"
import { cn } from "@/lib/utils"
import { otpSchema, type OtpFormData } from "@/lib/validations/auth"
import { Button } from "@/registry/new-york-v4/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/registry/new-york-v4/ui/card"
import {
  InputOTP,
  InputOTPGroup,
  InputOTPSeparator,
  InputOTPSlot,
} from "@/registry/new-york-v4/ui/input-otp"
import { Icons } from "@/components/icons"

export function OtpForm({
  email,
  className,
  ...props
}: React.ComponentProps<"div"> & { email?: string }) {
  const { verifyOtp, isLoading } = useAuth()
  const searchParams = useSearchParams()
  const resolvedEmail = email || searchParams.get("email") || ""

  const {
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<OtpFormData>({
    resolver: zodResolver(otpSchema),
    defaultValues: {
      otp: "",
    },
  })

  const otpValue = watch("otp")

  const onSubmit = async (data: OtpFormData) => {
    try {
      await verifyOtp(resolvedEmail, data.otp)
    } catch (error) {
      console.error("OTP Verification Error:", error)
    }
  }

  return (
    <div className={cn("flex flex-col gap-6", className)} {...props}>
      <Card>
        <CardHeader className="text-center">
          <CardTitle className="text-xl">Enter OTP Code</CardTitle>
          <CardDescription>
            Please enter the 6-digit code sent to {resolvedEmail || "your email"}.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid gap-6">
              <div className="flex items-center justify-center">
                <InputOTP
                  maxLength={6}
                  autoFocus
                  value={otpValue}
                  onChange={(value) => setValue("otp", value, { shouldValidate: true })}
                >
                  <InputOTPGroup>
                    <InputOTPSlot index={0} />
                    <InputOTPSlot index={1} />
                    <InputOTPSlot index={2} />
                  </InputOTPGroup>
                  <InputOTPSeparator />
                  <InputOTPGroup>
                    <InputOTPSlot index={3} />
                    <InputOTPSlot index={4} />
                    <InputOTPSlot index={5} />
                  </InputOTPGroup>
                </InputOTP>
              </div>

              {errors.otp && (
                <p className="px-1 text-center text-xs text-red-600">
                  {errors.otp.message}
                </p>
              )}

              <Button 
                type="submit" 
                className="w-full" 
                disabled={isLoading || otpValue?.length !== 6}
              >
                {isLoading && <Icons.spinner className="mr-2 h-4 w-4 animate-spin" />}
                Confirm
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
