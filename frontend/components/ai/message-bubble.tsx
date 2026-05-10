"use client"

import ReactMarkdown from "react-markdown"
import remarkGfm from "remark-gfm"
import { cn } from "@/lib/utils"

interface MessageBubbleProps {
  role: "user" | "assistant" | "system"
  content: string
}

export function MessageBubble({ role, content }: MessageBubbleProps) {
  const isAssistant = role === "assistant"
  const isSystem = role === "system"

  if (isSystem) {
    return (
      <div className="flex justify-center my-4">
        <span className="bg-muted px-3 py-1 rounded-full text-xs text-muted-foreground italic">
          {content}
        </span>
      </div>
    )
  }

  return (
    <div
      className={cn(
        "flex w-full mb-4",
        isAssistant ? "justify-start" : "justify-end"
      )}
    >
      <div
        className={cn(
          "max-w-[80%] px-4 py-2 rounded-2xl shadow-sm",
          isAssistant
            ? "bg-muted text-foreground rounded-tl-none border"
            : "bg-primary text-primary-foreground rounded-tr-none"
        )}
      >
        <div className="prose prose-sm dark:prose-invert max-w-none prose-p:leading-relaxed prose-pre:bg-muted-foreground/10 prose-pre:p-2 prose-code:bg-muted-foreground/10 prose-code:rounded prose-code:px-1">
          <ReactMarkdown remarkPlugins={[remarkGfm]}>{content}</ReactMarkdown>
        </div>
      </div>
    </div>
  )
}
