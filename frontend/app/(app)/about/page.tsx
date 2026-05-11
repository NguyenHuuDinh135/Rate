import { Film, Users, Award, Clock, Star, Heart, ShieldCheck, Zap } from "lucide-react"
import { Button } from "@/registry/new-york-v4/ui/button"

export default function AboutPage() {
  return (
    <div className="min-h-screen bg-background pt-24 pb-20 px-4 md:px-8 overflow-hidden">
      <div className="container mx-auto max-w-6xl space-y-24">
        
        {/* HERO SECTION */}
        <section className="relative py-12 text-center space-y-6">
          <div className="absolute top-0 left-1/2 -translate-x-1/2 w-[500px] h-[500px] bg-primary/10 rounded-full blur-3xl -z-10 animate-pulse" />
          <h1 className="text-5xl md:text-7xl font-black tracking-tighter uppercase italic">
            Behind the <span className="text-primary">Screeen</span>
          </h1>
          <p className="text-muted-foreground text-xl max-w-3xl mx-auto font-medium leading-relaxed">
            Rate Cinema is more than just a place to watch movies. It&apos;s a destination where technology meets emotion, 
            creating unforgettable memories for every cinephile.
          </p>
          <div className="flex flex-wrap justify-center gap-4 pt-4">
            <Button size="lg" className="rounded-full px-8 font-bold text-lg h-14">
              Explore Movies
            </Button>
            <Button size="lg" variant="outline" className="rounded-full px-8 font-bold text-lg h-14">
              Our Theaters
            </Button>
          </div>
        </section>

        {/* MISSION & VISION */}
        <section className="grid md:grid-cols-2 gap-12 items-center">
          <div className="space-y-6">
            <div className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-primary/10 text-primary font-bold text-sm uppercase tracking-wider">
              <Star className="w-4 h-4" /> Our Story
            </div>
            <h2 className="text-4xl font-bold tracking-tight">Redefining the Cinematic Experience Since 2024</h2>
            <p className="text-muted-foreground text-lg leading-relaxed">
              Founded with a passion for storytelling and cutting-edge technology, Rate Cinema began as a single-screen theater in the heart of the city. 
              Today, we are proud to be one of the leading cinema chains, pushing the boundaries of what a movie-going experience can be.
            </p>
            <p className="text-muted-foreground text-lg leading-relaxed">
              We believe that every film deserves the best possible presentation. That&apos;s why we invest in the latest IMAX, 4DX, and Dolby Atmos technologies 
              to ensure that every frame and every sound is delivered with absolute precision.
            </p>
          </div>
          <div className="relative group">
            <div className="aspect-video bg-muted rounded-3xl overflow-hidden shadow-2xl transition-transform duration-500 group-hover:scale-[1.02]">
                {/* Placeholder for an image or video */}
                <div className="w-full h-full flex items-center justify-center bg-gradient-to-br from-primary/20 to-secondary/20">
                    <Film className="w-24 h-24 text-primary/40" />
                </div>
            </div>
            <div className="absolute -bottom-6 -right-6 bg-card border p-6 rounded-2xl shadow-xl hidden lg:block animate-in slide-in-from-right-10">
                <div className="flex items-center gap-4">
                    <div className="w-12 h-12 rounded-full bg-primary flex items-center justify-center text-primary-foreground">
                        <Users className="w-6 h-6" />
                    </div>
                    <div>
                        <div className="text-2xl font-black">5M+</div>
                        <div className="text-xs text-muted-foreground uppercase font-bold tracking-widest">Happy Viewers</div>
                    </div>
                </div>
            </div>
          </div>
        </section>

        {/* FEATURES / STATS */}
        <section className="grid grid-cols-2 md:grid-cols-4 gap-6">
          {[
            { icon: ShieldCheck, label: "Safety First", desc: "Highest hygiene standards" },
            { icon: Zap, label: "Tech Forward", desc: "4K Laser & Atmos Sound" },
            { icon: Heart, label: "Premium Comfort", desc: "Recliner seats in all halls" },
            { icon: Award, label: "Best Service", desc: "Award-winning hospitality" }
          ].map((item, i) => (
            <div key={i} className="p-8 rounded-3xl border bg-card/50 backdrop-blur-sm text-center space-y-4 hover:border-primary/50 transition-colors group">
              <div className="w-16 h-16 mx-auto rounded-2xl bg-muted flex items-center justify-center group-hover:bg-primary/10 transition-colors">
                <item.icon className="w-8 h-8 text-primary" />
              </div>
              <div className="space-y-1">
                <h3 className="font-bold text-lg">{item.label}</h3>
                <p className="text-sm text-muted-foreground">{item.desc}</p>
              </div>
            </div>
          ))}
        </section>

        {/* CORE VALUES */}
        <section className="space-y-12">
            <div className="text-center space-y-4">
                <h2 className="text-4xl font-black tracking-tight uppercase">Why Choose <span className="text-primary">Rate</span>?</h2>
                <p className="text-muted-foreground text-lg max-w-2xl mx-auto">
                    We are committed to excellence in every aspect of your visit, from the moment you book your ticket to the final credits.
                </p>
            </div>
            
            <div className="grid md:grid-cols-3 gap-8">
                <div className="space-y-4 p-8 rounded-3xl border bg-gradient-to-b from-background to-muted/20">
                    <Clock className="w-10 h-10 text-primary" />
                    <h3 className="text-xl font-bold">Convenience</h3>
                    <p className="text-muted-foreground">
                        Book tickets in seconds with our seamless online platform and mobile app. No more long queues.
                    </p>
                </div>
                <div className="space-y-4 p-8 rounded-3xl border bg-gradient-to-b from-background to-muted/20">
                    <Users className="w-10 h-10 text-primary" />
                    <h3 className="text-xl font-bold">Community</h3>
                    <p className="text-muted-foreground">
                        Join our loyalty program and become part of a community of movie lovers with exclusive perks.
                    </p>
                </div>
                <div className="space-y-4 p-8 rounded-3xl border bg-gradient-to-b from-background to-muted/20">
                    <Award className="w-10 h-10 text-primary" />
                    <h3 className="text-xl font-bold">Innovation</h3>
                    <p className="text-muted-foreground">
                        We are constantly upgrading our facilities to provide the most immersive experience possible.
                    </p>
                </div>
            </div>
        </section>

        {/* CTA SECTION */}
        <section className="relative rounded-[40px] bg-primary p-12 md:p-24 text-center text-primary-foreground overflow-hidden">
            <div className="absolute top-0 left-0 w-full h-full opacity-10 pointer-events-none">
                <div className="absolute top-10 left-10 w-64 h-64 border-8 border-white rounded-full" />
                <div className="absolute bottom-10 right-10 w-96 h-96 border-8 border-white rounded-full" />
            </div>
            <div className="relative z-10 space-y-8">
                <h2 className="text-4xl md:text-6xl font-black tracking-tighter uppercase italic">Ready for the Show?</h2>
                <p className="text-xl opacity-90 max-w-2xl mx-auto font-medium">
                    Check out the latest blockbusters and reserve your favorite seats now. 
                    Experience the magic of cinema like never before.
                </p>
                <Button size="lg" variant="secondary" className="rounded-full px-12 font-bold text-lg h-16 shadow-2xl hover:scale-105 transition-transform">
                    Book Your Tickets Now
                </Button>
            </div>
        </section>

      </div>
    </div>
  )
}
