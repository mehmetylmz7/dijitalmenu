import React from 'react';
import { MenuItem, Category } from '../../types';
import { DesignProps } from './types';
import { LayoutGrid, Sliders, LogOut, Compass } from 'lucide-react';

export const BentoGrid: React.FC<DesignProps> = ({
  menuItems,
  scrollToCategory,
  onAdminLoginClick,
  isAdmin,
  onLogout,
}) => {
  return (
    <div className="bg-[#f8fafc] text-[#0f172a] min-h-screen font-sans selection:bg-amber-600 selection:text-white">
      {/* Modern Block Header */}
      <header className="max-w-6xl mx-auto px-6 pt-16 pb-8">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="md:col-span-2 bg-gradient-to-br from-stone-900 to-stone-850 text-white p-10 rounded-[2.5rem] flex flex-col justify-between shadow-sm">
            <div className="flex items-center gap-2 text-amber-400 font-bold tracking-widest text-[10px] uppercase">
              <Compass className="w-4 h-4 animate-spin-slow" /> Kocaoğlu Explorer
            </div>
            <div className="mt-8">
              <h1 className="text-4xl md:text-6xl font-serif font-black mb-3">
                Kocaoğlu Dijital Menü
              </h1>
              <p className="text-stone-300 text-sm md:text-base max-w-xl">
                Bento-grid düzeni ile hazırlanmış, kategorize edilmiş taze ve tescilli Anadolu gastronomi seçkisi.
              </p>
            </div>
          </div>
          <div className="bg-amber-100 p-10 rounded-[2.5rem] flex flex-col justify-between border border-amber-200">
            <span className="text-[10px] font-bold tracking-widest text-amber-800 uppercase">
              ADRES &amp; SAAT
            </span>
            <div>
              <h4 className="text-xl font-bold font-serif text-amber-900 mb-2">Hatay Şubesi</h4>
              <p className="text-sm text-amber-800 leading-relaxed">
                Atatürk Caddesi, No: 123 <br />
                Hatay, Türkiye <br />
                Hergün: 10:00 - 00:00
              </p>
            </div>
          </div>
        </div>
      </header>

      {/* Grid Categories Selector Bar */}
      <nav className="sticky top-0 z-30 bg-white/95 backdrop-blur-md border-b border-stone-200 shadow-sm overflow-x-auto no-scrollbar">
        <div className="max-w-6xl mx-auto flex py-4 px-6 gap-3 min-w-max">
          {Object.values(Category).map((cat) => (
            <button
              key={cat}
              onClick={() => scrollToCategory(cat)}
              className="px-5 py-2.5 bg-stone-100 hover:bg-amber-600 hover:text-white text-stone-700 font-bold text-xs rounded-2xl tracking-wide uppercase transition-all duration-300"
            >
              {cat}
            </button>
          ))}
        </div>
      </nav>

      {/* Main Bento Blocks Grid */}
      <main className="max-w-6xl mx-auto px-6 py-12">
        {Object.values(Category).map((cat) => {
          const items = menuItems.filter((i) => i.category === cat);
          if (items.length === 0) return null;

          return (
            <section key={cat} id={cat} className="mb-20 scroll-mt-24">
              <div className="flex items-center gap-4 mb-8">
                <LayoutGrid className="w-5 h-5 text-amber-600" />
                <h3 className="text-2xl font-serif font-black text-stone-800 uppercase tracking-tight">
                  {cat}
                </h3>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {items.map((item, index) => {
                  const isLarge = index % 4 === 0;
                  return (
                    <div
                      key={item.id}
                      className={`bg-white border border-stone-200/80 rounded-[2rem] overflow-hidden hover:border-amber-500/40 hover:shadow-xl transition-all duration-300 flex flex-col ${
                        isLarge ? 'md:col-span-2' : ''
                      }`}
                    >
                      <div className={`relative overflow-hidden bg-stone-50 ${isLarge ? 'h-72' : 'h-52'}`}>
                        <img
                          src={item.imageUrl}
                          alt={item.name}
                          className="w-full h-full object-cover"
                          onError={(e) => {
                            e.currentTarget.src =
                              'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&q=80&w=400';
                          }}
                        />
                        {item.isSpecial && (
                          <div className="absolute top-4 left-4 bg-amber-600 text-white text-[9px] uppercase font-bold tracking-widest px-3 py-1 rounded-xl">
                            Şefin İmzası
                          </div>
                        )}
                      </div>
                      <div className="p-8 flex flex-col flex-1 justify-between">
                        <div>
                          <div className="flex justify-between items-start gap-4 mb-2">
                            <h4 className="text-xl font-bold font-serif text-stone-950">
                              {item.name}
                            </h4>
                            <span className="text-lg font-black text-amber-700 whitespace-nowrap">
                              {item.price}
                            </span>
                          </div>
                          <p className="text-stone-500 text-sm leading-relaxed">
                            {item.description}
                          </p>
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            </section>
          );
        })}

        {/* Notes Bento Box */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mt-16">
          <div className="md:col-span-2 bg-stone-900 text-stone-100 p-10 rounded-[2.5rem] flex flex-col justify-between">
            <h4 className="text-amber-400 font-bold uppercase tracking-wider text-xs mb-4">
              SERVİS NOTU
            </h4>
            <p className="text-2xl font-serif font-medium leading-relaxed text-white mb-6">
              "Kendi alkolünü yanında getiren misafirlerimizden kişi başı{' '}
              <span className="text-amber-400 font-bold underline">100 TL</span> servis ücreti
              alınmaktadır."
            </p>
            <p className="text-stone-500 text-xs">
              * Tüm ürünlerimiz yerel üreticilerden doğrudan temin edilmektedir.
            </p>
          </div>
          <div className="bg-stone-200 p-10 rounded-[2.5rem] flex flex-col justify-between border border-stone-300">
            <span className="text-[10px] font-bold tracking-widest text-stone-500 uppercase">
              REZERVASYON
            </span>
            <div>
              <p className="text-sm font-semibold text-stone-700">
                En taze Hatay mezelerini yerinde tatmak için masanızı ayırtın.
              </p>
              <p className="text-lg font-serif font-bold text-stone-900 mt-4">
                +90 555 000 00 00
              </p>
            </div>
          </div>
        </div>
      </main>

      {/* Footer */}
      <footer className="bg-white border-t border-stone-200 py-12 px-6 text-xs text-stone-500">
        <div className="max-w-6xl mx-auto flex flex-col md:flex-row justify-between items-center gap-6 text-center md:text-left">
          <div className="font-serif text-lg font-bold text-stone-800">Kocaoğlu Restoran</div>
          <div>© {new Date().getFullYear()} KOCAOĞLU MEZE. HER HAKKI SAKLIDIR.</div>
          <div>
            {isAdmin ? (
              <button
                onClick={onLogout}
                className="hover:text-stone-900 transition-colors flex items-center gap-2 font-bold"
              >
                <LogOut className="w-3.5 h-3.5" /> Güvenli Çıkış
              </button>
            ) : (
              <button
                onClick={onAdminLoginClick}
                className="hover:text-stone-900 transition-colors flex items-center gap-2 font-bold"
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
