import React from 'react';
import { MenuItem, Category } from '../../types';
import { DesignProps } from './types';
import { Coffee, Sliders, LogOut, Sun } from 'lucide-react';

export const SunsetCafe: React.FC<DesignProps> = ({
  menuItems,
  scrollToCategory,
  onAdminLoginClick,
  isAdmin,
  onLogout,
}) => {
  return (
    <div className="bg-gradient-to-br from-[#fffdfa] via-[#fffbf7] to-[#fffcfc] text-[#4d3a34] min-h-screen font-sans selection:bg-[#ff793f] selection:text-white">
      {/* Sunlit warm Header */}
      <header className="relative pt-24 pb-20 px-6 text-center border-b border-[#f3e1d3]">
        <div className="absolute inset-0 bg-[radial-gradient(#ff793f_1px,transparent_1px)] [background-size:32px_32px] opacity-[0.03]"></div>
        <div className="max-w-xl mx-auto relative z-10">
          <div className="w-12 h-12 bg-[#ffe8d6] text-[#e15f41] rounded-full flex items-center justify-center mx-auto mb-4 shadow-sm">
            <Coffee className="w-6 h-6" />
          </div>
          <h1 className="text-4xl md:text-5xl font-black text-[#4d3a34] uppercase tracking-tight">
            Kocaoğlu Cafe &amp; Mutfak
          </h1>
          <p className="text-[#ff793f] text-xs uppercase tracking-[0.25em] font-bold mt-2">
            Günlük Sıcak &amp; Taze Sohbet Noktası
          </p>
        </div>
      </header>

      {/* Warm Categories Bar */}
      <nav className="sticky top-0 z-30 bg-white/90 backdrop-blur-md border-b border-[#f3e1d3] overflow-x-auto no-scrollbar">
        <div className="max-w-4xl mx-auto flex justify-center py-4 px-6 gap-3 md:gap-5 min-w-max">
          {Object.values(Category).map((cat) => (
            <button
              key={cat}
              onClick={() => scrollToCategory(cat)}
              className="px-5 py-2.5 bg-[#ffe8d6]/50 hover:bg-[#ff793f] text-[#4d3a34] hover:text-white font-bold text-xs rounded-full tracking-wide uppercase transition-all duration-300"
            >
              {cat}
            </button>
          ))}
        </div>
      </nav>

      <main className="max-w-5xl mx-auto px-6 py-16">
        {Object.values(Category).map((cat) => {
          const items = menuItems.filter((i) => i.category === cat);
          if (items.length === 0) return null;

          return (
            <section key={cat} id={cat} className="mb-20 scroll-mt-24">
              <div className="flex items-center gap-4 mb-10">
                <h3 className="text-2xl md:text-3xl font-black text-[#4d3a34] uppercase tracking-tight shrink-0">
                  {cat}
                </h3>
                <div className="h-[2px] bg-gradient-to-r from-[#f3e1d3] to-transparent w-full"></div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                {items.map((item) => (
                  <div
                    key={item.id}
                    className="bg-white rounded-[2.25rem] overflow-hidden border border-[#f3e1d3] shadow-sm hover:shadow-xl hover:-translate-y-1 transition-all duration-300 flex flex-col"
                  >
                    <div className="relative h-56 overflow-hidden bg-[#fffdfa]">
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
                        <div className="absolute top-4 left-4 bg-[#ff793f] text-white text-[9px] uppercase font-bold tracking-widest px-3 py-1 rounded-full shadow-md">
                          En Çok Satan
                        </div>
                      )}
                    </div>
                    <div className="p-7 flex flex-col flex-1">
                      <div className="flex justify-between items-start gap-4 mb-3">
                        <h4 className="text-lg font-bold text-[#4d3a34] leading-tight">
                          {item.name}
                        </h4>
                        <span className="text-base font-black text-[#e15f41] whitespace-nowrap">
                          {item.price}
                        </span>
                      </div>
                      <p className="text-stone-500 text-xs leading-relaxed line-clamp-2">
                        {item.description}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          );
        })}

        {/* Notes Block */}
        <div className="mt-16 p-8 md:p-12 bg-gradient-to-br from-[#ff793f] to-[#e15f41] rounded-[2.5rem] text-white shadow-lg relative overflow-hidden">
          <div className="absolute right-0 bottom-0 p-8 opacity-10">
            <Sun className="w-56 h-56" />
          </div>
          <div className="relative z-10 max-w-xl">
            <span className="text-amber-100 font-bold uppercase tracking-widest text-[10px] block mb-2">
              KÜÇÜK BİR NOT
            </span>
            <p className="text-xl md:text-2xl font-semibold leading-relaxed mb-4">
              "Kendi alkolünü yanında getiren misafirlerimizden kişi başı{' '}
              <span className="bg-white text-[#e15f41] px-2 py-0.5 rounded-xl font-bold">
                100 TL
              </span>{' '}
              servis ücreti alınmaktadır."
            </p>
            <p className="text-amber-100/80 text-xs italic">
              * Kahvelerimiz taze kavrulmuş çekirdeklerden öğütülmektedir.
            </p>
          </div>
        </div>
      </main>

      {/* Sunset Footer */}
      <footer className="bg-[#3a2c27] text-[#9c847c] py-16 px-6 text-xs border-t border-[#f3e1d3]">
        <div className="max-w-5xl mx-auto grid grid-cols-1 md:grid-cols-4 gap-12 mb-12">
          <div>
            <h5 className="text-white font-bold text-base mb-4">KOCAOĞLU CAFE</h5>
            <p className="text-[#846b63] leading-relaxed">
              Taptaze fırın ürünleri, zengin Hatay mezeleri ve güler yüzümüzle günün her saati buradayız.
            </p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-wider mb-4">İLETİŞİM</h5>
            <p>Tel: +90 555 000 00 00</p>
            <p>E-posta: merhaba@kocaoglucafe.com</p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-wider mb-4">ADRESİMİZ</h5>
            <p>Atatürk Caddesi, No: 123</p>
            <p>Hatay, Türkiye</p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-wider mb-4">SAATLERİMİZ</h5>
            <p>H. İçi: 11:00 - 23:00</p>
            <p>H. Sonu: 10:00 - 00:00</p>
          </div>
        </div>
        <div className="max-w-5xl mx-auto border-t border-[#4d3a34] pt-8 flex flex-col md:flex-row justify-between items-center gap-4 text-[10px] uppercase tracking-wider">
          <p>© {new Date().getFullYear()} KOCAOĞLU CAFE. MUTLU GÜNLER DİLERİZ.</p>
          <div>
            {isAdmin ? (
              <button
                onClick={onLogout}
                className="hover:text-white transition-colors flex items-center gap-2"
              >
                <LogOut className="w-3.5 h-3.5" /> ÇIKIŞ
              </button>
            ) : (
              <button
                onClick={onAdminLoginClick}
                className="hover:text-white transition-colors flex items-center gap-2"
              >
                <Sliders className="w-3.5 h-3.5" /> YÖNETİM
              </button>
            )}
          </div>
        </div>
      </footer>
    </div>
  );
};
