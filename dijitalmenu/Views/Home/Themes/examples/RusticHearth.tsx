import React from 'react';
import { MenuItem, Category } from '../../types';
import { DesignProps } from './types';
import { Terminal, Sliders, LogOut, Radio } from 'lucide-react';

export const CyberDiner: React.FC<DesignProps> = ({
  menuItems,
  scrollToCategory,
  onAdminLoginClick,
  isAdmin,
  onLogout,
}) => {
  return (
    <div className="bg-[#030008] text-[#e0e0ff] min-h-screen font-mono selection:bg-[#00f0ff] selection:text-black">
      {/* Sci-Fi Cyber Grid Header */}
      <header className="relative pt-16 pb-12 px-6 border-b border-[#00f0ff]/30 overflow-hidden bg-[linear-gradient(rgba(0,240,255,0.03)_1px,transparent_1px),linear-gradient(90deg,rgba(0,240,255,0.03)_1px,transparent_1px)] bg-[size:24px_24px]">
        <div className="absolute top-4 right-6 flex items-center gap-2 text-xs text-[#00f0ff]">
          <span className="w-2 h-2 rounded-full bg-emerald-500 animate-ping"></span>
          <span>SYSTEM_ONLINE // V3.0</span>
        </div>
        <div className="max-w-4xl mx-auto text-center relative z-10">
          <h1 className="text-4xl md:text-6xl font-black uppercase text-[#00f0ff] tracking-tighter drop-shadow-[0_0_12px_rgba(0,240,255,0.4)]">
            KOCAOĞLU // RESTORAN
          </h1>
          <p className="text-[#ff007f] text-xs uppercase tracking-[0.4em] font-bold mt-2 drop-shadow-[0_0_8px_rgba(255,0,127,0.4)]">
            :: GELENEKSEL_LEZZET_MATRISI ::
          </p>
        </div>
      </header>

      {/* Cyberpunk Navigation Panel */}
      <nav className="sticky top-0 z-30 bg-[#030008]/90 backdrop-blur-md border-b border-[#00f0ff]/20 overflow-x-auto no-scrollbar">
        <div className="max-w-4xl mx-auto flex justify-center py-4 px-6 gap-6 min-w-max">
          {Object.values(Category).map((cat) => (
            <button
              key={cat}
              onClick={() => scrollToCategory(cat)}
              className="text-[#00f0ff]/70 hover:text-[#00f0ff] hover:bg-[#00f0ff]/10 px-4 py-2 border border-[#00f0ff]/20 hover:border-[#00f0ff] transition-all duration-300 text-xs font-bold uppercase"
            >
              [ {cat} ]
            </button>
          ))}
        </div>
      </nav>

      {/* Content */}
      <main className="max-w-6xl mx-auto px-6 py-16">
        {Object.values(Category).map((cat) => {
          const items = menuItems.filter((i) => i.category === cat);
          if (items.length === 0) return null;

          return (
            <section key={cat} id={cat} className="mb-24 scroll-mt-28">
              <div className="mb-10 flex items-center justify-between border-b border-[#00f0ff]/10 pb-2">
                <h3 className="text-xl md:text-2xl font-black text-[#00f0ff] tracking-tight uppercase flex items-center gap-2">
                  <Terminal className="w-5 h-5 text-[#ff007f]" /> {cat}
                </h3>
                <span className="text-xs text-[#00f0ff]/50">SEC_ID: {cat.toUpperCase()}_DB</span>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                {items.map((item) => (
                  <div
                    key={item.id}
                    className="border border-[#00f0ff]/20 hover:border-[#00f0ff] bg-[#0c071a]/50 hover:bg-[#0c071a] hover:shadow-[0_0_20px_rgba(0,240,255,0.15)] transition-all duration-300 flex flex-col h-full relative group"
                  >
                    <div className="relative h-56 overflow-hidden border-b border-[#00f0ff]/20">
                      <img
                        src={item.imageUrl}
                        alt={item.name}
                        className="w-full h-full object-cover group-hover:scale-105 transition-all duration-500"
                        onError={(e) => {
                          e.currentTarget.src =
                            'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&q=80&w=400';
                        }}
                      />
                      {item.isSpecial && (
                        <div className="absolute top-4 left-4 bg-[#ff007f] text-white text-[9px] uppercase font-bold tracking-widest px-3 py-1 border border-white">
                          CRITICAL_DISC
                        </div>
                      )}
                    </div>
                    <div className="p-6 flex flex-col flex-1">
                      <div className="flex justify-between items-start gap-4 mb-3">
                        <h4 className="text-base font-bold text-white group-hover:text-[#00f0ff] transition-colors uppercase">
                          {item.name}
                        </h4>
                        <span className="text-base font-bold text-[#ff007f] drop-shadow-[0_0_8px_rgba(255,0,127,0.3)] shrink-0">
                          [{item.price}]
                        </span>
                      </div>
                      <p className="text-[#a0a0cf] text-xs leading-relaxed mb-6 flex-1">
                        {item.description}
                      </p>
                      <div className="border-t border-[#00f0ff]/10 pt-4 flex justify-between text-[9px] text-[#00f0ff]/40">
                        <span>SYS_ID: 0{item.id}</span>
                        <span>STATUS: AVAILABLE</span>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          );
        })}

        {/* Warning Section */}
        <div className="border border-[#ff007f]/40 p-8 md:p-12 bg-[#ff007f]/5 hover:bg-[#ff007f]/10 transition-all duration-300 relative">
          <div className="absolute -top-3 left-6 bg-[#030008] px-3 text-[#ff007f] text-xs font-bold uppercase flex items-center gap-1.5">
            <Radio className="w-3.5 h-3.5 animate-pulse" /> SYSTEM_ALERT
          </div>
          <p className="text-xl md:text-2xl font-black uppercase tracking-tight mb-4 text-[#00f0ff] leading-normal">
            "Kendi alkolünü yanında getiren misafirlerimizden kişi başı{' '}
            <span className="text-[#ff007f] font-bold bg-[#ff007f]/10 border border-[#ff007f]/30 px-2 py-0.5">
              100 TL
            </span>{' '}
            servis ücreti alınmaktadır."
          </p>
          <p className="text-[#a0a0cf] text-xs">
            &gt; TÜM FİYAT MATRİSLERİMİZ GÜNCEL KDV DAHLİDİR. DEĞİŞİKLİK GÖSTEREBİLİR.
          </p>
        </div>
      </main>

      {/* Cyber Footer */}
      <footer className="border-t border-[#00f0ff]/20 bg-[#06040f] py-16 px-6 text-xs text-[#a0a0cf]">
        <div className="max-w-6xl mx-auto grid grid-cols-1 md:grid-cols-4 gap-12 mb-12">
          <div>
            <h5 className="text-[#00f0ff] font-bold uppercase tracking-widest mb-4">
              // KOCAOĞLU
            </h5>
            <p className="leading-relaxed">
              Cyber-gastronomi ilkelerine dayalı geleneksel Antakya tarifleri, optimize edilmiş süreçlerle sunulur.
            </p>
          </div>
          <div>
            <h5 className="text-[#ff007f] font-bold uppercase tracking-widest mb-4">
              // DATALINK
            </h5>
            <p>TEL: +90 555 000 00 00</p>
            <p>LOG: info@kocaoglurestoran.com</p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-widest mb-4">
              // TERMINAL
            </h5>
            <p>Atatürk Caddesi, No: 123</p>
            <p>Hatay, Türkiye</p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-widest mb-4">
              // HOURS
            </h5>
            <p>SEC_01: 11:00 - 23:00</p>
            <p>SEC_02: 10:00 - 00:00</p>
          </div>
        </div>
        <div className="max-w-6xl mx-auto border-t border-[#00f0ff]/10 pt-8 flex flex-col md:flex-row justify-between items-center gap-4 text-[9px] uppercase tracking-wider">
          <p>© {new Date().getFullYear()} KOCAOĞLU.SYS - ALL RIGHTS RESERVED.</p>
          <div>
            {isAdmin ? (
              <button
                onClick={onLogout}
                className="hover:text-white transition-colors flex items-center gap-2 text-[#ff007f]"
              >
                <LogOut className="w-3.5 h-3.5" /> LOG_OUT
              </button>
            ) : (
              <button
                onClick={onAdminLoginClick}
                className="hover:text-white transition-colors flex items-center gap-2 text-[#00f0ff]"
              >
                <Sliders className="w-3.5 h-3.5" /> ADMIN_CONSOLE
              </button>
            )}
          </div>
        </div>
      </footer>
    </div>
  );
};
