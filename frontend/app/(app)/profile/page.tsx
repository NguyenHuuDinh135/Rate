"use client"

import { useState, useEffect } from "react"
import { useAuth } from "@/hooks/use-auth"
import { Button } from "@/registry/new-york-v4/ui/button"
import { Input } from "@/registry/new-york-v4/ui/input"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/registry/new-york-v4/ui/card"
import { Avatar, AvatarFallback, AvatarImage } from "@/registry/new-york-v4/ui/avatar"
import { toast } from "sonner"
import movieClient from "@/lib/api"

export default function ProfilePage() {
  const { user, refreshAuth } = useAuth()
  const [name, setName] = useState(user?.name || "")
  const [isUpdating, setIsUpdating] = useState(false)

  useEffect(() => {
    if (user) setName(user.name)
  }, [user])

  const handleUpdate = async () => {
    if (!user) return
    
    try {
      setIsUpdating(true)
      await movieClient.users.update({
        id: user.id,
        fullName: name,
        email: user.email
      })
      
      await refreshAuth()
      toast.success("Profile updated successfully")
    } catch (err) {
      toast.error("Failed to update profile")
    } finally {
      setIsUpdating(false)
    }
  }

  return (
    <div className="min-h-screen bg-background pt-24 pb-20 px-4 md:px-8">
      <div className="container mx-auto max-w-2xl space-y-8">
        <div>
          <h1 className="text-3xl font-black tracking-tight">Account Settings</h1>
          <p className="text-muted-foreground mt-1">Manage your personal information and preferences.</p>
        </div>

        <Card className="rounded-3xl border-border shadow-lg overflow-hidden">
          <CardHeader className="bg-muted/30 pb-12">
            <div className="flex items-center gap-6">
              <Avatar className="h-24 w-24 border-4 border-background shadow-xl">
                <AvatarImage src={user?.avatar} />
                <AvatarFallback className="text-2xl font-black">
                  {user?.name?.slice(0, 2).toUpperCase()}
                </AvatarFallback>
              </Avatar>
              <div className="space-y-1">
                <CardTitle className="text-2xl font-bold">{user?.name}</CardTitle>
                <CardDescription>{user?.email}</CardDescription>
              </div>
            </div>
          </CardHeader>
          <CardContent className="pt-8 space-y-6">
            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <label className="text-xs font-bold uppercase tracking-wider text-muted-foreground ml-1">Full Name</label>
                <Input 
                   value={name} 
                   onChange={(e) => setName(e.target.value)}
                   className="rounded-xl h-11" 
                />
              </div>
              <div className="space-y-2">
                <label className="text-xs font-bold uppercase tracking-wider text-muted-foreground ml-1">Email Address</label>
                <Input defaultValue={user?.email} className="rounded-xl h-11" disabled />
              </div>
            </div>

            <div className="pt-6 border-t flex justify-end gap-3">
              <Button 
                className="rounded-full px-8 font-bold"
                onClick={handleUpdate}
                disabled={isUpdating || name === user?.name}
              >
                {isUpdating ? "Saving..." : "Save Changes"}
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
