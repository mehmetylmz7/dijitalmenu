import React from 'react';
import { MenuItem, Category } from '../../types';
import { DesignProps } from './types';
import { Anchor, Sliders, LogOut, Sailboat } from 'lucide-react';

export const CoastalAegean: React.FC<DesignProps> = ({
  menuItems,
  scrollToCategory,
  onAdminLoginClick,
  isAdmin,
  onLogout,
}) => {
  return (
    <div className="bg-[#fcfcff] text-[#1e293b] min-h-screen font-sans selection:bg-[#0284c7] selection:text-white">
      {/* Breeze Seaside Header */}
      <header className="relative pt-24 pb-20 px-6 text-center bg-gradient-to-b from-[#e0f2fe] to-[#fcfcff] overflow-hidden border-b-4 border-[#0284c7]/20">
        <div className="absolute inset-0 bg-[radial-gradient(#0284c7_1px,transparent_1px)] [background-size:24px_24px] opacity-10"></div>
        <div className="max-w-xl mx-auto relative z-10">
          <Anchor className="w-10 h-10 text-[#0284c7] mx-auto mb-4 animate-bounce" />
          <h1 className="text-4xl md:text-5xl font-serif font-black text-[#0369a1] uppercase tracking-wide">
            KOCAOĞLU MEZE &amp; BALIK
          </h1>
          <p className="text-[#0ea5e9] text-xs uppercase tracking-[0.3em] font-bold mt-2">
            Akdeniz &amp; Ege Rüzgarları
          </p>
        </div>
      </header>

      {/* Aegean Navigation Bar */}
      <nav className="sticky top-0 z-30 bg-white/95 backdrop-blur-md border-b-2 border-[#e2e8f0] shadow-sm overflow-x-auto no-scrollbar">
        <div className="max-w-4xl mx-auto flex justify-center py-4 px-6 gap-3 md:gap-6 min-w-max">
          {Object.values(Category).map((cat) => (
            <button
              key={cat}
              onClick={() => scrollToCategory(cat)}
              className="px-4 py-2 border-2 border-[#0284c7]/20 hover:border-[#0284c7] text-[#0369a1] hover:bg-[#f0f9ff] font-bold text-xs md:text-sm rounded-full tracking-wider uppercase transition-all duration-300"
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
              <div className="text-center mb-10">
                <h3 className="text-2xl md:text-3xl font-serif font-bold text-[#0369a1] uppercase tracking-wide inline-block relative py-2 px-6">
                  {cat}
                  <span className="absolute bottom-0 left-0 right-0 h-[2px] bg-[#0284c7]"></span>
                </h3>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                {items.map((item) => (
                  <div
                    key={item.id}
                    className="bg-white rounded-3xl overflow-hidden border border-[#e2e8f0] shadow-sm hover:shadow-lg hover:border-[#0284c7]/40 transition-all duration-300 flex flex-col"
                  >
                    <div className="relative h-56 overflow-hidden bg-sky-50">
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
                        <div className="absolute top-4 left-4 bg-sky-600 text-white text-[9px] uppercase font-bold tracking-widest px-3 py-1 rounded-full shadow-lg">
                          Günlük Meze
                        </div>
                      )}
                    </div>
                    <div className="p-6 flex flex-col flex-1">
                      <div className="flex justify-between items-start gap-4 mb-3">
                        <h4 className="text-lg font-serif font-bold text-[#1e293b] leading-tight">
                          {item.name}
                        </h4>
                        <span className="text-base font-black text-[#0284c7] whitespace-nowrap">
                          {item.price}
                        </span>
                      </div>
                      <p className="text-[#64748b] text-xs leading-relaxed line-clamp-2">
                        {item.description}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          );
        })}

        {/* Note Section */}
        <div className="mt-16 p-8 md:p-12 bg-gradient-to-br from-[#0369a1] to-[#0284c7] rounded-3xl text-white shadow-xl relative overflow-hidden">
          <div className="absolute right-0 bottom-0 p-8 opacity-10">
            <Sailboat className="w-48 h-48" />
          </div>
          <div className="relative z-10 max-w-xl">
            <span className="text-[#e0f2fe] font-bold uppercase tracking-widest text-[10px] block mb-2">
              Liman Bilgilendirmesi
            </span>
            <p className="text-xl md:text-2xl font-serif leading-relaxed mb-4">
              "Kendi alkolünü yanında getiren misafirlerimizden kişi başı{' '}
              <span className="text-[#e0f2fe] font-bold bg-white/10 px-2 py-0.5 rounded-lg">
                100 TL
              </span>{' '}
              servis ücreti alınmaktadır."
            </p>
            <p className="text-sky-100 text-xs italic">
              * Deniz ürünlerimiz Hatay kıyılarından taze olarak günlük temin edilmektedir.
            </p>
          </div>
        </div>
      </main>

      {/* Aegean Footer */}
      <footer className="bg-[#0f172a] text-[#94a3b8] py-16 px-6 text-xs">
        <div className="max-w-5xl mx-auto grid grid-cols-1 md:grid-cols-4 gap-12 mb-12">
          <div>
            <h5 className="text-white font-serif text-lg mb-4">KOCAOĞLU TAHT</h5>
            <p className="text-[#64748b] leading-relaxed">
              Tarihi lezzetleri, Ege ve Akdeniz'in taze deniz esintileri ile harmanlayarak şık masalara sunuyoruz.
            </p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-wider mb-4">REZERVASYON</h5>
            <p>Tel: +90 555 000 00 00</p>
            <p>E-posta: liman@kocaoglu.com</p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-wider mb-4">LİMAN</h5>
            <p>Atatürk Caddesi, No: 123</p>
            <p>Hatay, Türkiye</p>
          </div>
          <div>
            <h5 className="text-white font-bold uppercase tracking-wider mb-4">AÇILIŞ-KAPANIŞ</h5>
            <p>H. İçi: 11:00 - 23:00</p>
            <p>H. Sonu: 10:00 - 00:00</p>
          </div>
        </div>
        <div className="max-w-5xl mx-auto border-t border-slate-800 pt-8 flex flex-col md:flex-row justify-between items-center gap-4 text-[10px] uppercase tracking-wider">
          <p>© {new Date().getFullYear()} KOCAOĞLU BALIK. SELAMETLE.</p>
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
