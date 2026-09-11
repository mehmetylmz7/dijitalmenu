# Dijital Menü - Dağıtım ve Ortam Değişkenleri Rehberi

Bu rehber, Dijital Menü uygulamasının yerel Docker ortamında ve canlı bulut platformlarında (özellikle Render.com) hatasız çalışması için gereken adımları ve ortam değişkenlerini özetler.

---

## 1. Ortam Değişkenleri Referansı (Environment Variables)

| Değişken Adı | Açıklama | Örnek / Varsayılan | Zorunlu mu? |
| :--- | :--- | :--- | :--- |
| `ASPNETCORE_ENVIRONMENT` | Çalışma ortamı | `Production` veya `Development` | Evet |
| `ConnectionStrings__PostgresConnection` | PostgreSQL bağlantı dizesi | `Host=...;Port=5432;Database=...;Username=...;Password=...` | Canlıda Evet |
| `ConnectionStrings__DefaultConnection` | Yedek bağlantı dizesi anahtarı | (Yukarıdaki ile aynı) | Tercihen |
| `Seed__AdminUsername` | Sistem yöneticisi kullanıcı adı | `admin` | Hayır (Atlanırsa seed yapılmaz, uyarılır) |
| `Seed__AdminPassword` | Sistem yöneticisi parolası | `GucluSifre123!` | Seed istendiğinde Evet |
| `AppUrl` | Uygulama ana alan adı / URL'i | `https://menum.onrender.com` | QR kod & Link üretimi için |
| `PORT` | Dinamik HTTP portu (Render vb. otomatik sağlar) | `8080` veya platform portu | Otomatik |

---

## 2. Render.com Dağıtım Adımları

1. **GitHub Repository Bağlantısı:**
   - Render Dashboard $\rightarrow$ **New Web Service** $\rightarrow$ Proje reposunu seçin.
   - Environment: `Docker` (veya repo kökündeki `render.yaml` ile Blueprint olarak oluşturun).

2. **PostgreSQL Veritabanı Oluşturma:**
   - Render Dashboard $\rightarrow$ **New PostgreSQL** oluşturun.
   - Oluşturulan DB'nin **Internal Database URL** bilgisini alın.

3. **Environment Variables Ekleme:**
   - Render Service $\rightarrow$ **Environment** sekmesine gidin.
   - Aşağıdaki değişkenleri tanımlayın:
     ```env
     ASPNETCORE_ENVIRONMENT=Production
     ConnectionStrings__PostgresConnection=Host=dpg-...;Port=5432;Database=...;Username=...;Password=...
     ConnectionStrings__DefaultConnection=Host=dpg-...;Port=5432;Database=...;Username=...;Password=...
     Seed__AdminUsername=admin
     Seed__AdminPassword=GucluSifre123!
     ```

4. **Kalıcı Disk (Opsiyonel ama Önerilen):**
   - Disk sekmesinden `/app/wwwroot/images` dizinine mount edilecek bir disk ekleyin (Restoran ve ürün fotoğraflarının deployment sonraları silinmemesi için).

---

## 3. Yerel Docker Compose ile Çalıştırma

Kök dizindeki `.env` dosyasını hazırlayın:
```bash
cp .env.example .env
# .env içindeki şifreleri düzenleyin
```

Ardından konteynerleri başlatın:
```bash
docker compose up -d --build
```
- Web: `http://localhost:8080`
- PostgreSQL: `localhost:5432`
- Yüklenen resimler `web_images` Docker volume'unda güvenle saklanır.
