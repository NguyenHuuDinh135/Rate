// Đăng ký JS Interop vào môi trường window cho các hiệu ứng hoạt họa GSAP
window.animateSeatsEntrance = () => {
    // Đảm bảo GSAP đã sẵn sàng và phần tử tồn tại
    if (typeof gsap !== 'undefined') {
        // Reset trạng thái ban đầu của các ghế
        gsap.set(".seat-btn", { scale: 0, opacity: 0 });
        
        // Hoạt họa zoom-in bounce rực rỡ từ tâm
        gsap.to(".seat-btn", {
            scale: 1,
            opacity: 1,
            duration: 0.45,
            stagger: {
                amount: 0.25,
                grid: "auto",
                from: "center"
            },
            ease: "back.out(1.5)"
        });
    }
};

window.animateHeroText = () => {
    if (typeof gsap !== 'undefined') {
        gsap.fromTo(".hero-animate", 
            { y: 30, opacity: 0 },
            { y: 0, opacity: 1, duration: 0.8, stagger: 0.1, ease: "power3.out", delay: 0.15 }
        );
    }
};

window.rateTheme = {
    key: "rate-theme",
    apply(isDarkMode, animate = false) {
        const root = document.documentElement;
        if (animate && document.startViewTransition) {
            document.startViewTransition(() => {
                root.classList.toggle("dark", isDarkMode);
                root.dataset.theme = isDarkMode ? "dark" : "light";
            });
        } else {
            root.classList.toggle("dark", isDarkMode);
            root.dataset.theme = isDarkMode ? "dark" : "light";
        }
    },
    init(defaultDarkMode) {
        const stored = window.localStorage.getItem(this.key);
        const isDarkMode = stored !== "light";
        this.apply(isDarkMode, false);
        return isDarkMode;
    },
    set(isDarkMode) {
        window.localStorage.setItem(this.key, isDarkMode ? "dark" : "light");
        this.apply(isDarkMode, true);
    }
};

window.rateAuth = {
    focusById(id) {
        const element = document.getElementById(id);
        if (element) {
            element.focus();
        }
    }
};
