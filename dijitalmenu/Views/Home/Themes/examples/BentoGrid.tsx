import React from 'react';
import { MenuItem, Category } from '../../types';
import { DesignProps } from './types';
import { FileText, Sliders, LogOut, Printer } from 'lucide-react';

export const VintageNewspaper: React.FC<DesignProps> = ({
  menuItems,
  scrollToCategory,
  onAdminLoginClick,
  isAdmin,
  onLogout,
}) => {
  return (
    <div className="bg-[#f4eedb] text-[#1c1813] min-h-screen font-serif p-4 md:p-8 selection:bg-[#5c4033] selection:text-white">
      {/* Outer vintage border container */}
      <div className="max-w-5xl mx-auto border-4 border-double border-[#3e2e1e] p-6 md:p-12 bg-[#f9f5e8] shadow-md">
        {/* Newspaper Header */}
        <header className="border-b-4 border-black pb-6 mb-10 text-center">
          <div className="flex justify-between border-b border-black/30 pb-2 mb-4 text-[11px] font-sans uppercase tracking-[0.15em]">
            <span>SAYI: 45 // 1978'DEN BERİ</span>
            <span>FİYAT: 10 KURUŞ</span>
            <span>HATAY, TÜRKİYE</span>
          </div>
          <h1 className="text-5xl md:text-7xl font-serif font-black uppercase tracking-tight leading-none text-[#2d1e10]">
            Kocaoğlu Postası
          </h1>
          <div className="h-[2px] bg-black my-4"></div>
          <p className="font-sans text-xs uppercase tracking-[0.4em] font-black italic">
            GELENEKSEL LEZZETLERİN EN GÜVENİLİR VE TESCİLLİ GAZETESİ
          </p>
        </header>

        {/* Vintage Column Categorization Bar */}
        <nav className="border-b-4 border-black py-2 mb-12 overflow-x-auto no-scrollbar">
          <div className="flex justify-around gap-4 min-w-max font-sans text-xs uppercase tracking-widest font-black">
            {Object.values(Category).map((cat) => (
              <button
                key={cat}
                onClick={() => scrollToCategory(cat)}
                className="hover:underline transition-all"
              >
                * {cat} *
              </button>
            ))}
          </div>
        </nav>

        {/* Traditional dual-column paper list */}
        <main className="space-y-16">
          {Object.values(Category).map((cat) => {
            const items = menuItems.filter((i) => i.category === cat);
            if (items.length === 0) return null;

            return (
              <section key={cat} id={cat} className="scroll-mt-24">
                <div className="text-center mb-8 border-b-2 border-black/80 pb-2">
                  <h3 className="text-3xl font-serif font-bold italic uppercase tracking-wider">
                    {cat} Bölümü
                  </h3>
                  <p className="font-sans text-[10px] tracking-widest text-stone-500 uppercase mt-1">
                    [ GÜNLÜK OLARAK TAZE HAZIRLANMAKTADIR ]
                  </p>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-x-12 gap-y-10">
                  {items.map((item) => (
                    <div key={item.id} className="flex flex-col justify-between group">
                      <div>
                        {/* Title ............. Price style row */}
                        <div className="flex justify-between items-baseline gap-2 mb-1">
                          <h4 className="text-lg font-bold font-serif text-[#2d1e10] group-hover:underline">
                            {item.name}
                          </h4>
                          {/* Dotted separator */}
                          <div className="flex-1 border-b border-dashed border-[#8d7c66] mx-2"></div>
                          <span className="font-mono text-sm font-bold shrink-0">
                            {item.price}
                          </span>
                        </div>
                        <p className="text-[#4e4337] text-sm font-sans leading-relaxed">
                          {item.description}
                        </p>
                      </div>
                      {item.isSpecial && (
                        <div className="mt-2 text-[9px] font-sans font-bold uppercase tracking-widest text-[#5c4033] italic">
                          :: Tavsiye Olunur ::
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              </section>
            );
          })}

          {/* Newspaper Note Ad Block */}
          <div className="border-4 border-black p-8 text-center bg-[#f4eedb] max-w-2xl mx-auto my-16">
            <span className="font-sans text-xs font-black uppercase tracking-[0.3em] block mb-2">
              - RESMİ TEBLİĞAT -
            </span>
            <p className="text-xl md:text-2xl font-serif italic text-black leading-relaxed mb-4">
              "Kendi alkolünü yanında getiren kıymetli misafirlerimizden kişi başı{' '}
              <span className="underline font-bold">100 TL</span> servis ücreti tahsil edilmektedir."
            </p>
            <p className="font-sans text-[10px] uppercase tracking-widest text-stone-600">
              * Tesisimizde hijyen kuralları en üst mertebede uygulanmaktadır.
            </p>
          </div>
        </main>

        {/* Vintage Footer */}
        <footer className="border-t-4 border-black pt-10 mt-16 text-center text-xs font-sans text-stone-600">
          <p className="font-bold uppercase tracking-widest mb-4">
            KOCAOĞLU RESTORAN ve MAHDUMLARI
          </p>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8 text-[11px]">
            <div>İRTİBAT: +90 555 000 00 00</div>
            <div>ADRES: Atatürk Cad. No: 123 Hatay</div>
            <div>SAATLER: 11:00 - 23:00 / 10:00 - 00:00</div>
          </div>
          <div className="border-t border-black/30 pt-6 flex flex-col md:flex-row justify-between items-center gap-4 text-[10px] uppercase">
            <span>© {new Date().getFullYear()} KOCAOĞLU POSTASI. HER HAKKI MAHFUZDUR.</span>
            <div>
              {isAdmin ? (
                <button
                  onClick={onLogout}
                  className="hover:underline transition-colors flex items-center gap-1 font-bold"
                >
                  <LogOut className="w-3.5 h-3.5" /> GÜVENLİ ÇIKIŞ
                </button>
              ) : (
                <button
                  onClick={onAdminLoginClick}
                  className="hover:underline transition-colors flex items-center gap-1 font-bold"
                >
                  <Sliders className="w-3.5 h-3.5" /> YÖNETİCİ GİRİŞİ
                </button>
              )}
            </div>
          </div>
        </footer>
      </div>
    </div>
  );
};
