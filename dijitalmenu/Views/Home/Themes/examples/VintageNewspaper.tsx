import React from 'react';
import { MenuItem, Category } from '../../types';
import { DesignProps } from './types';
import { Eye, Sliders, LogOut, Sun } from 'lucide-react';

export const OrientalJade: React.FC<DesignProps> = ({
  menuItems,
  scrollToCategory,
  onAdminLoginClick,
  isAdmin,
  onLogout,
}) => {
  return (
    <div className="bg-[#022312] text-[#e6f4ea] min-h-screen font-serif selection:bg-[#d4af37] selection:text-black">
      {/* Zen Centered Header */}
      <header className="relative pt-24 pb-20 px-6 text-center border-b border-[#1b4332]">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_top,rgba(212,175,55,0.05),transparent_60%)]"></div>
        <div className="max-w-2xl mx-auto relative z-10">
          <div className="w-12 h-12 border-2 border-[#d4af37]/30 rounded-full flex items-center justify-center mx-auto mb-6 text-[#d4af37]">
            <Sun className="w-5 h-5 animate-spin-slow" />
          </div>
          <h1 className="text-4xl md:text-5xl font-serif text-[#d4af37] uppercase tracking-[0.2em] font-medium">
            KOCAOĞLU
          </h1>
          <div className="h-[1px] w-32 bg-[#d4af37]/40 mx-auto my-4"></div>
          <p className="text-emerald-300 text-xs uppercase tracking-[0.4em] font-sans font-semibold">
            DOĞU &amp; BATI LEZZET KÖPRÜSÜ
          </p>
        </div>
      </header>

      {/* Serene Navigation Bar */}
      <nav className="sticky top-0 z-30 bg-[#022312]/95 backdrop-blur-md border-b border-[#1b4332] overflow-x-auto no-scrollbar">
        <div className="max-w-4xl mx-auto flex justify-center py-4 px-6 gap-6 md:gap-10 min-w-max font-sans text-xs uppercase tracking-widest text-emerald-300">
          {Object.values(Category).map((cat) => (
            <button
              key={cat}
              onClick={() => scrollToCategory(cat)}
              className="hover:text-[#d4af37] transition-colors relative py-1"
            >
              {cat}
            </button>
          ))}
        </div>
      </nav>

      {/* Content */}
      <main className="max-w-4xl mx-auto px-6 py-16">
        {Object.values(Category).map((cat) => {
          const items = menuItems.filter((i) => i.category === cat);
          if (items.length === 0) return null;

          return (
            <section key={cat} id={cat} className="mb-24 scroll-mt-24">
              <div className="text-center mb-12">
                <span className="text-[#d4af37] text-xs font-sans font-bold uppercase tracking-[0.3em] block mb-2">
                  HAZIRLANIŞI ÖZEL
                </span>
                <h3 className="text-2xl md:text-3xl font-serif text-white tracking-wide">
                  {cat}
                </h3>
                <div className="h-[1.5px] w-12 bg-emerald-500/30 mx-auto mt-4"></div>
              </div>

              <div className="space-y-12">
                {items.map((item) => (
                  <div
                    key={item.id}
                    className="flex flex-col md:flex-row gap-6 p-4 rounded-xl border border-transparent hover:border-[#1b4332] hover:bg-[#042f1a]/30 transition-all duration-300"
                  >
                    <div className="w-full md:w-36 h-36 shrink-0 rounded-lg overflow-hidden border border-[#1b4332] bg-[#022312]">
                      <img
                        src={item.imageUrl}
                        alt={item.name}
                        className="w-full h-full object-cover opacity-80"
                        onError={(e) => {
                          e.currentTarget.src =
                            'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&q=80&w=400';
                        }}
                      />
                    </div>
                    <div className="flex-1 flex flex-col justify-between">
                      <div>
                        <div className="flex justify-between items-baseline gap-4 mb-2">
                          <h4 className="text-xl font-bold text-white tracking-wide">
                            {item.name}
                          </h4>
                          <span className="font-sans text-base font-bold text-[#d4af37]">
                            {item.price}
                          </span>
                        </div>
                        <p className="text-stone-300 text-sm font-sans font-light leading-relaxed">
                          {item.description}
                        </p>
                      </div>
                      {item.isSpecial && (
                        <div className="mt-3">
                          <span className="font-sans text-[9px] uppercase tracking-widest text-[#d4af37] border border-[#d4af37]/30 px-2.5 py-0.5 rounded">
                            Tavsiye Edilen Reçete
                          </span>
                        </div>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            </section>
          );
        })}

        {/* Notes */}
        <div className="border border-[#1b4332] p-8 md:p-12 bg-[#042f1a]/50 rounded-2xl relative max-w-2xl mx-auto text-center mt-20">
          <span className="font-sans text-[10px] uppercase tracking-[0.3em] text-[#d4af37] block mb-3">
            BİLGİLENDİRME
          </span>
          <p className="text-xl md:text-2xl font-serif text-[#e6f4ea] leading-relaxed mb-4">
            "Kendi alkolünü yanında getiren misafirlerimizden kişi başı{' '}
            <span className="text-[#d4af37] font-bold">100 TL</span> servis ücreti
            alınmaktadır."
          </p>
          <p className="font-sans text-[10px] italic text-stone-500">
            * Menü fiyatlarımız ve içeriklerimiz taze hammaddelere göre güncellenebilmektedir.
          </p>
        </div>
      </main>

      {/* Serene Footer */}
      <footer className="border-t border-[#1b4332] bg-[#01140a] text-stone-500 py-16 px-6 font-sans text-xs">
        <div className="max-w-4xl mx-auto grid grid-cols-1 md:grid-cols-3 gap-12 mb-12 text-center md:text-left">
          <div>
            <h5 className="text-[#d4af37] font-serif text-lg mb-4">Kocaoğlu Meclisi</h5>
            <p className="font-light text-stone-600 leading-relaxed">
              Anadolu'nun bereketli topraklarından süzülen geleneksel lezzetleri şükranla masanıza sunuyoruz.
            </p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-widest mb-4">MÜRACAAT</h5>
            <p>Telefon: +90 555 000 00 00</p>
            <p>E-Posta: meclis@kocaoglu.com</p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-widest mb-4">ÇALIŞMA GÜNLERİ</h5>
            <p>Pazartesi - Cuma: 11:00 - 23:00</p>
            <p>Cumartesi - Pazar: 10:00 - 00:00</p>
          </div>
        </div>
        <div className="max-w-4xl mx-auto border-t border-stone-900 pt-8 flex flex-col md:flex-row justify-between items-center gap-4 text-[10px] tracking-wider text-stone-600">
          <p>© {new Date().getFullYear()} KOCAOĞLU RESTORAN. ESENLİKLER DİLERİZ.</p>
          <div>
            {isAdmin ? (
              <button
                onClick={onLogout}
                className="hover:text-white transition-colors flex items-center gap-2 uppercase"
              >
                <LogOut className="w-3.5 h-3.5" /> Çıkış Yap
              </button>
            ) : (
              <button
                onClick={onAdminLoginClick}
                className="hover:text-white transition-colors flex items-center gap-2 uppercase"
              >
                <Sliders className="w-3.5 h-3.5" /> Yönetim Kapısı
              </button>
            )}
          </div>
        </div>
      </footer>
    </div>
  );
};
