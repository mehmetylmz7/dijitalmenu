import React from 'react';
import { MenuItem, Category } from '../../types';
import { DesignProps } from './types';
import { GlassWater, Sliders, LogOut, Award } from 'lucide-react';

export const MidnightBistro: React.FC<DesignProps> = ({
  menuItems,
  scrollToCategory,
  onAdminLoginClick,
  isAdmin,
  onLogout,
}) => {
  return (
    <div className="bg-[#0b0908] text-[#f4f2f0] min-h-screen font-sans selection:bg-amber-500 selection:text-black">
      {/* Cinematic Dark Hero */}
      <header className="relative h-[45vh] flex items-center justify-center overflow-hidden border-b border-amber-500/20">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_center,rgba(194,155,90,0.15),transparent_70%)]"></div>
        <img
          src="https://images.unsplash.com/photo-1514362545857-3bc16c4c7d1b?auto=format&fit=crop&q=80&w=1200"
          className="absolute inset-0 w-full h-full object-cover opacity-20 mix-blend-luminosity"
          alt="Luxury Restaurant Background"
        />
        <div className="relative z-10 text-center px-4">
          <span className="text-amber-500 text-xs tracking-[0.5em] uppercase font-bold mb-4 block">
            PREMIUM EXPERIENCE
          </span>
          <h1 className="text-5xl md:text-7xl font-serif text-white mb-3 tracking-wide">
            KOCAOĞLU
          </h1>
          <p className="text-amber-400 text-sm md:text-base tracking-[0.3em] font-light uppercase">
            BİSTRO &amp; GOURMET
          </p>
        </div>
      </header>

      {/* Floating Category Bar */}
      <div className="sticky top-0 z-30 bg-[#0b0908]/90 backdrop-blur-md border-b border-stone-800 shadow-2xl">
        <div className="max-w-6xl mx-auto flex justify-center py-5 px-6 gap-6 md:gap-10 overflow-x-auto no-scrollbar min-w-max">
          {Object.values(Category).map((cat) => (
            <button
              key={cat}
              onClick={() => scrollToCategory(cat)}
              className="text-stone-400 hover:text-amber-400 font-medium text-xs md:text-sm tracking-widest uppercase transition-all duration-300 hover:scale-105"
            >
              {cat}
            </button>
          ))}
        </div>
      </div>

      <div className="max-w-6xl mx-auto px-6 py-16">
        {Object.values(Category).map((cat) => {
          const items = menuItems.filter((i) => i.category === cat);
          if (items.length === 0) return null;

          return (
            <section key={cat} id={cat} className="mb-24 scroll-mt-28">
              <div className="flex items-center gap-6 mb-12">
                <h2 className="text-2xl md:text-3xl font-serif tracking-wide text-white uppercase">
                  {cat}
                </h2>
                <div className="h-[1px] bg-gradient-to-r from-amber-500/30 to-transparent flex-1"></div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                {items.map((item) => (
                  <div
                    key={item.id}
                    className="group bg-stone-900/40 rounded-2xl border border-stone-800/80 hover:border-amber-500/30 overflow-hidden transition-all duration-300 flex flex-col hover:shadow-[0_0_30px_rgba(245,158,11,0.05)]"
                  >
                    <div className="relative h-56 overflow-hidden">
                      <img
                        src={item.imageUrl}
                        alt={item.name}
                        className="w-full h-full object-cover opacity-80 group-hover:opacity-100 group-hover:scale-105 transition-all duration-500"
                        onError={(e) => {
                          e.currentTarget.src =
                            'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&q=80&w=400';
                        }}
                      />
                      {item.isSpecial && (
                        <div className="absolute top-4 left-4 bg-gradient-to-r from-amber-500 to-amber-600 text-black text-[9px] uppercase font-bold tracking-widest px-3 py-1 rounded-full shadow-lg flex items-center gap-1">
                          <Award className="w-3 h-3" /> Özel Lezzet
                        </div>
                      )}
                    </div>
                    <div className="p-6 flex flex-col flex-1 bg-gradient-to-b from-stone-900/20 to-stone-950/40">
                      <div className="flex justify-between items-start gap-4 mb-3">
                        <h3 className="text-lg font-bold text-white group-hover:text-amber-400 transition-colors">
                          {item.name}
                        </h3>
                        <span className="text-base font-bold text-amber-500 whitespace-nowrap">
                          {item.price}
                        </span>
                      </div>
                      <p className="text-stone-400 text-xs leading-relaxed line-clamp-2">
                        {item.description}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          );
        })}

        {/* Notes */}
        <div className="mt-20 p-10 bg-stone-900/30 rounded-3xl border border-amber-500/10 shadow-2xl relative overflow-hidden text-center max-w-4xl mx-auto">
          <div className="absolute inset-0 bg-[radial-gradient(circle_at_top,rgba(245,158,11,0.03),transparent_60%)]"></div>
          <div className="relative z-10 max-w-2xl mx-auto">
            <GlassWater className="w-8 h-8 text-amber-500 mx-auto mb-4 opacity-80" />
            <h3 className="text-amber-500 font-bold uppercase tracking-widest text-xs mb-3">
              SERVİS NOTU
            </h3>
            <p className="text-xl md:text-2xl font-serif leading-relaxed mb-4 text-stone-200">
              "Kendi alkolünü yanında getiren misafirlerimizden kişi başı{' '}
              <span className="text-amber-400 font-bold">100 TL</span> servis ücreti
              alınmaktadır."
            </p>
            <p className="text-stone-500 text-xs italic">
              Menü fiyatları mevsimsel hammadde tedariğine göre değişiklik gösterebilir.
            </p>
          </div>
        </div>
      </div>

      {/* Dark Luxury Footer */}
      <footer className="bg-stone-950 text-stone-500 py-16 px-6 border-t border-stone-900 text-xs">
        <div className="max-w-6xl mx-auto grid grid-cols-1 md:grid-cols-4 gap-12 mb-12">
          <div>
            <h5 className="text-white font-serif text-lg mb-4">KOCAOĞLU</h5>
            <p className="text-stone-600 leading-relaxed">
              Özel reçetelerle hazırlanan zengin menümüz ve büyüleyici ambiyansımızla sizleri ağırlamaktan mutluluk duyuyoruz.
            </p>
          </div>
          <div>
            <h5 className="text-white uppercase tracking-widest mb-4">REZERVASYON</h5>
            <p>+90 555 000 00 00</p>
            <p>info@kocaoglurestoran.com</p>
          </div>
          <div>
            <h5 className="text-white uppercase tracking-widest mb-4">KONUM</h5>
            <p>Atatürk Caddesi, No: 123</p>
            <p>Hatay, Türkiye</p>
          </div>
          <div>
            <h5 className="text-white uppercase tracking-widest mb-4">SAATLER</h5>
            <p>Hafta içi: 11:00 - 23:00</p>
            <p>Hafta sonu: 10:00 - 00:00</p>
          </div>
        </div>
        <div className="max-w-6xl mx-auto border-t border-stone-900/80 pt-8 flex flex-col md:flex-row justify-between items-center gap-4 text-[10px] uppercase tracking-wider">
          <p>© {new Date().getFullYear()} KOCAOĞLU RESTORAN. TÜM HAKLARI SAKLIDIR.</p>
          <div>
            {isAdmin ? (
              <button
                onClick={onLogout}
                className="hover:text-amber-400 transition-colors flex items-center gap-2"
              >
                <LogOut className="w-3.5 h-3.5" /> Güvenli Çıkış
              </button>
            ) : (
              <button
                onClick={onAdminLoginClick}
                className="hover:text-amber-400 transition-colors flex items-center gap-2"
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
