"use client"

import { useState, useEffect, useRef } from "react"
import { useChat } from "@ai-sdk/react"
import { Send, Sparkles, Plus, History, Trash2 } from "lucide-react"
import { MessageBubble } from "@/components/ai/message-bubble"
import { Button } from "@/registry/new-york-v4/ui/button"
import { Input } from "@/registry/new-york-v4/ui/input"
import { ScrollArea } from "@/registry/new-york-v4/ui/scroll-area"
import { API_CONFIG } from "@/lib/constants"
import { cn } from "@/lib/utils"

export default function AiChatPage() {
  const [sessionId, setSessionId] = useState<number | null>(null)
  const [isLoadingSession, setIsLoadingSession] = useState(false)
  const scrollRef = useRef<HTMLDivElement>(null)

  const { messages, input, handleInputChange, handleSubmit, setMessages, isLoading, error } = useChat({
    api: `${API_CONFIG.BASE_URL}/ai/chat`,
    body: {
      sessionId: sessionId,
    },
    onFinish: () => {
        // Option to handle post-chat logic
    }
  })

  // Create or load session on mount
  useEffect(() => {
    async function initSession() {
      setIsLoadingSession(true)
      try {
        const response = await fetch(`${API_CONFIG.BASE_URL}/ai/session/create`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ title: "New Conversation" }),
        })
        const data = await response.json()
        setSessionId(data.id)
      } catch (err) {
        console.error("Failed to create AI session", err)
      } finally {
        setIsLoadingSession(false)
      }
    }

    if (!sessionId) {
      initSession()
    }
  }, [sessionId])

  // Scroll to bottom when messages change
  useEffect(() => {
    if (scrollRef.current) {
      scrollRef.current.scrollTop = scrollRef.current.scrollHeight
    }
  }, [messages])

  const handleNewChat = async () => {
    setMessages([])
    setSessionId(null) // This will trigger a new session creation
  }

  return (
    <div className="flex h-[calc(100vh-8rem)] flex-col gap-4 p-4 lg:flex-row">
      {/* Sidebar - Desktop Only */}
      <div className="hidden w-64 flex-col gap-4 lg:flex">
        <Button 
          variant="outline" 
          className="w-full justify-start gap-2 border-dashed"
          onClick={handleNewChat}
        >
          <Plus className="h-4 w-4" />
          Hội thoại mới
        </Button>
        <div className="flex flex-1 flex-col gap-2 overflow-y-auto">
          <div className="text-xs font-semibold text-muted-foreground uppercase px-2 mb-2 tracking-wider">
            Lịch sử chat
          </div>
          {/* Recent sessions would go here */}
          <div className="flex items-center gap-2 rounded-lg px-2 py-1.5 text-sm bg-accent text-accent-foreground">
            <History className="h-4 w-4" />
            <span className="truncate">Hội thoại hiện tại</span>
          </div>
        </div>
      </div>

      {/* Main Chat Area */}
      <div className="flex flex-1 flex-col rounded-xl border bg-card shadow-sm overflow-hidden">
        {/* Chat Header */}
        <div className="flex items-center justify-between border-b px-4 py-3">
          <div className="flex items-center gap-2">
            <div className="rounded-full bg-primary/10 p-1.5">
              <Sparkles className="h-4 w-4 text-primary" />
            </div>
            <div>
              <h2 className="text-sm font-semibold">Rate AI Assistant</h2>
              <div className="flex items-center gap-1.5">
                <span className="h-1.5 w-1.5 rounded-full bg-green-500" />
                <span className="text-[10px] text-muted-foreground">Sẵn sàng hỗ trợ</span>
              </div>
            </div>
          </div>
          <Button variant="ghost" size="icon" onClick={handleNewChat}>
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>

        {/* Messages */}
        <ScrollArea ref={scrollRef} className="flex-1 p-4">
          {messages.length === 0 ? (
            <div className="flex h-full flex-col items-center justify-center gap-4 text-center">
              <div className="rounded-full bg-primary/5 p-6 animate-pulse">
                <Sparkles className="h-12 w-12 text-primary/40" />
              </div>
              <div className="max-w-sm space-y-2">
                <h3 className="text-lg font-medium">Xin chào! Tôi có thể giúp gì cho bạn?</h3>
                <p className="text-sm text-muted-foreground">
                  Tôi có thể giúp bạn tìm phim, xem lịch chiếu rạp hoặc gợi ý phim hay cho tối nay.
                </p>
              </div>
              <div className="mt-4 grid grid-cols-1 gap-2 sm:grid-cols-2">
                {[
                  "Phim nào đang hot nhất?",
                  "Lịch chiếu phim Godzilla hôm nay",
                  "Tìm phim hành động hay",
                  "Rạp nào gần đây nhất?"
                ].map((suggestion) => (
                  <Button 
                    key={suggestion} 
                    variant="outline" 
                    className="h-auto py-2 text-xs text-left justify-start"
                    onClick={() => {
                        // This is a bit hacky for useChat, usually you'd use append
                        const event = { target: { value: suggestion } } as any;
                        handleInputChange(event);
                    }}
                  >
                    {suggestion}
                  </Button>
                ))}
              </div>
            </div>
          ) : (
            messages.map((m) => (
              <MessageBubble key={m.id} role={m.role as any} content={m.content} />
            ))
          )}
          {isLoading && (
            <div className="flex justify-start mb-4">
               <div className="bg-muted px-4 py-2 rounded-2xl rounded-tl-none border">
                  <span className="flex gap-1">
                    <span className="h-1.5 w-1.5 rounded-full bg-primary/40 animate-bounce" />
                    <span className="h-1.5 w-1.5 rounded-full bg-primary/40 animate-bounce [animation-delay:0.2s]" />
                    <span className="h-1.5 w-1.5 rounded-full bg-primary/40 animate-bounce [animation-delay:0.4s]" />
                  </span>
               </div>
            </div>
          )}
          {error && (
            <div className="flex justify-center my-4">
                <div className="bg-destructive/10 text-destructive text-xs px-4 py-2 rounded-lg border border-destructive/20">
                    Đã có lỗi xảy ra. Vui lòng thử lại sau.
                </div>
            </div>
          )}
        </ScrollArea>

        {/* Input Area */}
        <div className="p-4 pt-0">
          <form
            onSubmit={handleSubmit}
            className="relative flex items-center rounded-lg border bg-background px-3 py-1.5 focus-within:ring-1 focus-within:ring-primary"
          >
            <Input
              value={input}
              onChange={handleInputChange}
              placeholder="Hỏi AI về phim và lịch chiếu..."
              className="border-0 focus-visible:ring-0 px-0 h-10 shadow-none"
              disabled={isLoading || isLoadingSession}
            />
            <Button 
              type="submit" 
              size="icon" 
              disabled={!input.trim() || isLoading || isLoadingSession}
              className="h-8 w-8 shrink-0 rounded-md"
            >
              <Send className="h-4 w-4" />
            </Button>
          </form>
          <p className="mt-2 text-[10px] text-center text-muted-foreground">
            AI có thể nhầm lẫn. Hãy kiểm tra lại thông tin quan trọng.
          </p>
        </div>
      </div>
    </div>
  )
}
