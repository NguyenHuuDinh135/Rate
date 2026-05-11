"use client"

import { usePathname } from "next/navigation"
import { SidebarTrigger } from "@/registry/new-york-v4/ui/sidebar"
import { Separator } from "@/registry/new-york-v4/ui/separator"
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/registry/new-york-v4/ui/breadcrumb"
import { ModeSwitcher } from "@/components/mode-switcher"

export function AdminHeader() {
  const pathname = usePathname()
  const pathSegments = pathname.split("/").filter(Boolean)
  
  return (
    <header className="flex h-16 shrink-0 items-center gap-2 px-4">
      <div className="flex items-center gap-2">
        <SidebarTrigger className="-ml-1" />
        <Separator orientation="vertical" className="mr-2 h-4" />
        <Breadcrumb>
          <BreadcrumbList>
            <BreadcrumbItem className="hidden md:block">
              <BreadcrumbLink href="/admin/dashboard">
                Admin
              </BreadcrumbLink>
            </BreadcrumbItem>
            {pathSegments.length > 1 && <BreadcrumbSeparator className="hidden md:block" />}
            {pathSegments.length > 1 && (
              <BreadcrumbItem>
                <BreadcrumbPage className="capitalize">
                  {pathSegments[pathSegments.length - 1]}
                </BreadcrumbPage>
              </BreadcrumbItem>
            )}
          </BreadcrumbList>
        </Breadcrumb>
      </div>
      <div className="ml-auto flex items-center gap-2">
        <ModeSwitcher />
      </div>
    </header>
  )
}
