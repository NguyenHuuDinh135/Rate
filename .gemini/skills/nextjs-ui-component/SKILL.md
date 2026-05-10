---
name: nextjs-ui-component
description: Scaffold UI components for Next.js using Tailwind CSS v4 and Radix UI. Use this when creating new reusable components in frontend/components or frontend/registry.
---

# Next.js UI Component

Skill này giúp tạo các UI component chuẩn theo hệ thống `new-york-v4` của dự án Rate.

## Quy tắc thiết kế

1. **Tailwind v4**: Sử dụng các utility classes hiện đại. Lưu ý các class như `focus-visible:ring-ring/50`.
2. **Radix UI**: Sử dụng các primitive từ Radix UI (VD: `Slot` từ `@radix-ui/react-slot`).
3. **CVA**: Sử dụng `class-variance-authority` để quản lý variants và sizes.
4. **'use client'**: Chỉ thêm directive này nếu component có interactive logic hoặc sử dụng React hooks.
5. **Icon**: Sử dụng `lucide-react` hoặc `hugeicons`.

## Template Ví dụ

```tsx
import * as React from "react"
import { cva, type VariantProps } from "class-variance-authority"
import { Slot } from "@radix-ui/react-slot"
import { cn } from "@/lib/utils"

const componentVariants = cva(
  "inline-flex items-center justify-center transition-all",
  {
    variants: {
      variant: {
        default: "bg-primary text-primary-foreground",
        outline: "border border-input bg-background",
      },
      size: {
        default: "h-9 px-4",
        sm: "h-8 px-3",
      },
    },
    defaultVariants: {
      variant: "default",
      size: "default",
    },
  }
)

interface MyComponentProps extends React.ComponentProps<"div">, VariantProps<typeof componentVariants> {
  asChild?: boolean
}

function MyComponent({ className, variant, size, asChild = false, ...props }: MyComponentProps) {
  const Comp = asChild ? Slot : "div"
  return (
    <Comp
      className={cn(componentVariants({ variant, size, className }))}
      {...props}
    />
  )
}

export { MyComponent, componentVariants }
```
