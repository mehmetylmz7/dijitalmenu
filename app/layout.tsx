import type { Metadata } from 'next'
import './globals.css'

export const metadata: Metadata = { title: 'Dijital Menü — Masanızın yeni dijital deneyimi', description: 'Restoranınız için zarif, hızlı ve yönetilebilir QR menü.' }
export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) { return <html lang="tr"><body>{children}</body></html> }
