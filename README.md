# Dijital Menü

Restoranların tema seçerek dijital menü oluşturmasını sağlayan .NET 8 ASP.NET Core MVC uygulaması.

## Gereksinimler

- .NET SDK 8
- Docker Desktop (PostgreSQL veritabanı container'ı için) veya PostgreSQL 15+

## Docker ile Hızlı Başlatma

Projeyi veritabanı dahil tüm servisleriyle Docker üzerinde çalıştırmak için:

```bash
docker compose up --build -d
```

Uygulama `http://localhost:8080` adresinde, PostgreSQL ise `localhost:5432` portunda çalışacaktır.

## Yerel Geliştirme (Visual Studio / CLI)

1. Sadece PostgreSQL container'ını başlatın:
   ```bash
   docker compose up postgres -d
   ```
2. `dotnet restore`
3. `dotnet run --project dijitalmenu`

Uygulama başlangıcında migration'lar otomatik olarak PostgreSQL üzerine uygulanır.

## Güvenlik ve yapılandırma

- Production'da `POSTGRES_PASSWORD`, `Seed:AdminUsername` ve `Seed:AdminPassword` değerlerini güvenli environment variable üzerinden verin; varsayılan değerleri kullanmayın.
- Yeni restoran parolası en az 12 karakter olmalı; büyük/küçük harf, rakam ve özel karakter içermelidir.
- Ürün görselleri yalnızca JPG, PNG, GIF veya WebP formatında ve en fazla 5 MB olabilir.
- Harita bağlantıları HTTPS üzerinden Google Maps alan adlarıyla sınırlıdır.

## Veri modeli

`Restaurant -> Menu -> Category -> MenuItem` zincirinde silme işlemleri cascade çalışır. Restoran silindiğinde ilişkili kullanıcı, menü, kategori ve ürünler de silinir; panel silme onayları bu etkiyi açıkça belirtir.

## Builder JSON endpoint'leri

Restaurant oturumu ve antiforgery token gerektirir:

- `POST /Restaurant/Builder/SelectTheme`
- `POST /Restaurant/Builder/AddCategory`
- `POST /Restaurant/Builder/AddMenuItem`
- `POST /Restaurant/Builder/UpdateLocation`
