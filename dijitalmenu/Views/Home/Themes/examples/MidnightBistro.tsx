import React from 'react';
import { MenuItem, Category } from '../../types';
import { DesignProps } from './types';
import { Soup, Sliders, LogOut, Flame } from 'lucide-react';

export const RusticHearth: React.FC<DesignProps> = ({
  menuItems,
  scrollToCategory,
  onAdminLoginClick,
  isAdmin,
  onLogout,
}) => {
  return (
    <div className="bg-[#f9f3eb] text-[#3e342f] min-h-screen font-sans selection:bg-[#d07b5c] selection:text-white">
      {/* Warm Organic Header */}
      <header className="relative pt-20 pb-16 px-6 text-center bg-[#f0e3d4] border-b-2 border-[#dfccb7]">
        <div className="max-w-xl mx-auto">
          <div className="inline-flex p-3 bg-[#e8dac7] text-[#c0603d] rounded-2xl mb-4">
            <Flame className="w-6 h-6 animate-pulse" />
          </div>
          <h1 className="text-4xl md:text-5xl font-serif font-black text-[#5c3e31] tracking-tight">
            Kocaoğlu Ocağı
          </h1>
          <p className="text-[#c0603d] text-xs uppercase tracking-[0.25em] font-bold mt-2">
            Odun Ateşinde Geleneksel Lezzetler
          </p>
        </div>
      </header>

      {/* Earthy Categories Bar */}
      <nav className="sticky top-0 z-30 bg-[#f9f3eb]/95 backdrop-blur-md border-b-2 border-[#dfccb7] overflow-x-auto no-scrollbar">
        <div className="max-w-4xl mx-auto flex justify-center py-4 px-6 gap-4 md:gap-8 min-w-max">
          {Object.values(Category).map((cat) => (
            <button
              key={cat}
              onClick={() => scrollToCategory(cat)}
              className="px-4 py-2 bg-[#f0e3d4]/40 hover:bg-[#c0603d] hover:text-white rounded-xl text-xs md:text-sm font-bold tracking-wider uppercase transition-all duration-200"
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
              <div className="flex items-center gap-4 mb-8">
                <h3 className="text-2xl md:text-3xl font-serif font-bold text-[#5c3e31] shrink-0">
                  {cat}
                </h3>
                <div className="h-0.5 bg-dashed border-t border-[#dfccb7] w-full"></div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                {items.map((item) => (
                  <div
                    key={item.id}
                    className="bg-[#f0e3d4]/30 rounded-[2rem] p-5 border border-[#dfccb7]/60 hover:border-[#c0603d] transition-all duration-300 flex flex-col"
                  >
                    <div className="relative h-48 rounded-[1.5rem] overflow-hidden mb-4 bg-stone-100">
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
                        <span className="absolute top-3 left-3 bg-[#c0603d] text-white text-[9px] uppercase font-bold tracking-widest px-3 py-1 rounded-xl shadow-md">
                          Fırından Yeni
                        </span>
                      )}
                    </div>
                    <div className="flex-1 flex flex-col justify-between">
                      <div>
                        <div className="flex justify-between items-baseline gap-4 mb-2">
                          <h4 className="text-lg font-serif font-bold text-[#5c3e31]">
                            {item.name}
                          </h4>
                          <span className="text-base font-black text-[#c0603d]">
                            {item.price}
                          </span>
                        </div>
                        <p className="text-[#605249] text-xs leading-relaxed line-clamp-2">
                          {item.description}
                        </p>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          );
        })}

        {/* Note Section */}
        <div className="mt-16 p-8 md:p-10 bg-[#5c3e31] rounded-[2.5rem] text-white shadow-xl relative overflow-hidden">
          <div className="absolute -right-12 -bottom-12 opacity-5 text-stone-200">
            <Soup className="w-64 h-64" />
          </div>
          <div className="relative z-10 max-w-xl">
            <h4 className="text-[#df9e84] font-bold uppercase tracking-wider text-xs mb-3">
              Köyümüzden Sofranıza
            </h4>
            <p className="text-xl md:text-2xl font-serif leading-relaxed mb-4">
              "Kendi alkolünü yanında getiren misafirlerimizden kişi başı{' '}
              <span className="text-[#df9e84] font-bold">100 TL</span> servis ücreti
              alınmaktadır."
            </p>
            <p className="text-[#dfccb7] text-xs italic">
              * El yapımı ekmeklerimiz ve yayık tereyağımız ikramımızdır.
            </p>
          </div>
        </div>
      </main>

      {/* Rustic Footer */}
      <footer className="bg-[#483025] text-[#b39e95] py-16 px-6 text-xs">
        <div className="max-w-5xl mx-auto grid grid-cols-1 md:grid-cols-4 gap-12 mb-12">
          <div>
            <h5 className="text-white font-serif text-lg mb-4">KOCAOĞLU OCAĞI</h5>
            <p className="text-[#968075] leading-relaxed">
              Kuşaktan kuşağa aktarılan sırlarımızla, Hatay'ın nefis odun fırını lezzetlerini sunuyoruz.
            </p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-wider mb-4">OCAK BAŞI</h5>
            <p>Tel: +90 555 000 00 00</p>
            <p>E-posta: ocak@kocaoglu.com</p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-wider mb-4">ADRESİMİZ</h5>
            <p>Atatürk Caddesi, No: 123</p>
            <p>Hatay, Türkiye</p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-wider mb-4">KAPILARIMIZ</h5>
            <p>H. İçi: 11:00 - 23:00</p>
            <p>H. Sonu: 10:00 - 00:00</p>
          </div>
        </div>
        <div className="max-w-5xl mx-auto border-t border-[#5c3e31] pt-8 flex flex-col md:flex-row justify-between items-center gap-4 text-[10px] uppercase tracking-wider">
          <p>© {new Date().getFullYear()} KOCAOĞLU RESTORAN. AFİYET OLSUN.</p>
          <div>
            {isAdmin ? (
              <button
                onClick={onLogout}
                className="hover:text-white transition-colors flex items-center gap-2"
              >
                <LogOut className="w-3.5 h-3.5" /> Güvenli Çıkış
              </button>
            ) : (
              <button
                onClick={onAdminLoginClick}
                className="hover:text-white transition-colors flex items-center gap-2"
              >
                <Sliders className="w-3.5 h-3.5" /> Yönetici Girişi
              </button>
            )}
          </div>
        </div>
      </footer>
    </div>
  );
};
