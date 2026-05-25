/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/WebUI/Shared/**/*.razor",
    "./src/WebUI/Client/**/*.razor",
    "./src/WebUI/Server/**/*.razor",
    "./src/WebUI/Server/wwwroot/**/*.html"
  ],
  theme: {
    extend: {
      colors: {
        accent: {
          DEFAULT: '#E50914',
          hover: '#B20710',
          light: 'rgba(229, 9, 20, 0.15)',
        }
      }
    },
  },
  plugins: [],
}
