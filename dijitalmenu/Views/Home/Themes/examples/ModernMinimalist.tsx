import React from 'react';
import { MenuItem, Category } from '../../types';
import { DesignProps } from './types';
import { ArrowUpRight, Sliders, LogOut, Info } from 'lucide-react';

export const ModernMinimalist: React.FC<DesignProps> = ({
  menuItems,
  scrollToCategory,
  onAdminLoginClick,
  isAdmin,
  onLogout,
}) => {
  return (
    <div className="bg-[#fcfcfc] text-[#111111] min-h-screen font-sans selection:bg-black selection:text-white">
      {/* Structural Minimal Header */}
      <header className="border-b border-black py-16 px-6">
        <div className="max-w-6xl mx-auto flex flex-col md:flex-row justify-between items-start md:items-end gap-6">
          <div>
            <span className="text-xs uppercase tracking-[0.3em] font-mono block mb-2 text-stone-500">
              EST. 1978 / HATAY
            </span>
            <h1 className="text-6xl md:text-8xl font-black tracking-tight uppercase leading-none">
              KOCAOĞLU
            </h1>
            <h2 className="text-3xl md:text-4xl font-light uppercase tracking-widest text-stone-600 mt-1">
              RESTORAN
            </h2>
          </div>
          <div className="text-left md:text-right font-mono text-xs text-stone-600">
            <p>TEL: +90 555 000 00 00</p>
            <p>ATATURK CAD. NO: 123</p>
            <p>TURKIYE</p>
          </div>
        </div>
      </header>

      {/* Categories Bar */}
      <nav className="sticky top-0 z-30 bg-white/95 backdrop-blur-sm border-b border-black overflow-x-auto no-scrollbar">
        <div className="max-w-6xl mx-auto flex py-5 px-6 gap-6 md:gap-10 min-w-max font-mono text-xs uppercase tracking-wider">
          {Object.values(Category).map((cat, index) => (
            <button
              key={cat}
              onClick={() => scrollToCategory(cat)}
              className="hover:text-amber-800 transition-colors flex items-center gap-1 group font-bold"
            >
              <span className="text-[10px] text-stone-400 font-normal">0{index + 1}.</span>
              {cat}
              <ArrowUpRight className="w-3 h-3 opacity-0 group-hover:opacity-100 transition-opacity" />
            </button>
          ))}
        </div>
      </nav>

      {/* Main Content */}
      <main className="max-w-6xl mx-auto px-6 py-16">
        {Object.values(Category).map((cat) => {
          const items = menuItems.filter((i) => i.category === cat);
          if (items.length === 0) return null;

          return (
            <section key={cat} id={cat} className="mb-24 scroll-mt-28">
              <div className="border-b border-black pb-4 mb-10 flex justify-between items-end">
                <h3 className="text-3xl font-black uppercase tracking-tight">{cat}</h3>
                <span className="font-mono text-xs text-stone-500">
                  ({items.length} ÜRÜN)
                </span>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-x-8 gap-y-12">
                {items.map((item) => (
                  <div
                    key={item.id}
                    className="border border-black bg-white group hover:shadow-[8px_8px_0px_0px_rgba(0,0,0,1)] transition-all duration-200 flex flex-col h-full"
                  >
                    <div className="relative h-64 border-b border-black overflow-hidden bg-stone-100">
                      <img
                        src={item.imageUrl}
                        alt={item.name}
                        className="w-full h-full object-cover filter grayscale group-hover:grayscale-0 transition-all duration-300"
                        onError={(e) => {
                          e.currentTarget.src =
                            'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&q=80&w=400';
                        }}
                      />
                      {item.isSpecial && (
                        <span className="absolute top-4 left-4 bg-black text-white font-mono text-[9px] uppercase tracking-widest px-3 py-1 font-bold">
                          ÖZEL TARİF
                        </span>
                      )}
                    </div>

                    <div className="p-6 flex flex-col flex-1">
                      <div className="flex justify-between items-baseline gap-4 mb-3">
                        <h4 className="text-lg font-bold uppercase tracking-tight">
                          {item.name}
                        </h4>
                        <span className="font-mono font-bold text-sm shrink-0">
                          {item.price}
                        </span>
                      </div>
                      <p className="text-stone-600 text-sm font-light leading-relaxed mb-6 flex-1">
                        {item.description}
                      </p>
                      <div className="border-t border-stone-200 pt-4 font-mono text-[10px] text-stone-500 flex justify-between uppercase">
                        <span>KOCAOĞLU KITCHEN</span>
                        <span>0{item.id}</span>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          );
        })}

        {/* Note Section */}
        <div className="border-2 border-black p-8 md:p-12 bg-white hover:shadow-[12px_12px_0px_0px_rgba(0,0,0,1)] transition-all duration-200 relative">
          <div className="flex items-start gap-4">
            <Info className="w-6 h-6 shrink-0 text-black mt-1" />
            <div>
              <span className="font-mono text-xs uppercase tracking-widest text-stone-500 block mb-2">
                SERVİS BİLGİLENDİRMESİ
              </span>
              <p className="text-2xl font-bold uppercase tracking-tight mb-4">
                "Kendi alkolünü yanında getiren misafirlerimizden kişi başı{' '}
                <span className="bg-black text-white px-2 py-0.5">100 TL</span> servis ücreti
                alınmaktadır."
              </p>
              <p className="text-stone-500 font-mono text-xs">
                * Menü fiyatları ve içerikleri mevsimsel olarak değişebilir.
              </p>
            </div>
          </div>
        </div>
      </main>

      {/* Footer */}
      <footer className="border-t border-black bg-black text-stone-400 py-16 px-6 font-mono text-xs mt-12">
        <div className="max-w-6xl mx-auto grid grid-cols-1 md:grid-cols-4 gap-12 mb-12">
          <div>
            <h5 className="text-white font-bold uppercase tracking-widest mb-4">KOCAOĞLU</h5>
            <p className="font-light text-stone-500 leading-relaxed">
              Geleneksel lezzetleri modern sunumlarla buluşturan tescilli lezzet durağı.
            </p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-widest mb-4">İLETİŞİM</h5>
            <p>T: +90 555 000 00 00</p>
            <p>E: info@kocaoglurestoran.com</p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-widest mb-4">ADRES</h5>
            <p>Atatürk Caddesi, No: 123</p>
            <p>Hatay, Türkiye</p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-widest mb-4">SAATLER</h5>
            <p>H. İçi: 11:00 - 23:00</p>
            <p>H. Sonu: 10:00 - 00:00</p>
          </div>
        </div>
        <div className="max-w-6xl mx-auto border-t border-stone-900 pt-8 flex flex-col md:flex-row justify-between items-center gap-4 text-[10px]">
          <p>© {new Date().getFullYear()} KOCAOĞLU RESTORAN. ALL RIGHTS RESERVED.</p>
          <div>
            {isAdmin ? (
              <button
                onClick={onLogout}
                className="hover:text-white transition-colors flex items-center gap-2 font-bold uppercase"
              >
                <LogOut className="w-3.5 h-3.5" /> Güvenli Çıkış
              </button>
            ) : (
              <button
                onClick={onAdminLoginClick}
                className="hover:text-white transition-colors flex items-center gap-2 font-bold uppercase"
              >
                <Sliders className="w-3.5 h-3.5" /> Yönetici Paneli
              </button>
            )}
          </div>
        </div>
      </footer>
    </div>
  );
};
