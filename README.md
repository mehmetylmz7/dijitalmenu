# Dijital Menü

Restoranların tema seçerek dijital menü oluşturmasını sağlayan .NET 8 ASP.NET Core MVC uygulaması.

## Gereksinimler

- .NET SDK 8
- SQL Server veya SQL Express

## Yerel kurulum

1. `dijitalmenu/appsettings.Development.json` içindeki `DefaultConnection` değerini kendi SQL Server örneğinize göre ayarlayın.
2. `dotnet restore`
3. `dotnet run --project dijitalmenu`

Uygulama başlangıcında migration'lar otomatik uygulanır. Bu nedenle uygulama hesabının şema değiştirme yetkisi olmalıdır. Production ortamında migration'ları ayrı bir deployment adımı olarak çalıştırmak tercih edilmelidir.

## Güvenlik ve yapılandırma

- Production'da `Seed:AdminUsername` ve `Seed:AdminPassword` değerlerini güvenli environment variable veya secret store üzerinden verin; varsayılan değerleri kullanmayın.
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

## Migration kontrolü

`AddDataIntegrityConstraints` migration'ı unique index ve alan uzunluğu kuralları eklemeden önce mevcut veriyi doğrular. Hata verirse, bildirilen duplicate veya aşırı uzun kayıtlar düzeltilmeli; migration yeniden çalıştırılmalıdır. Migration veriyi sessizce kırpmaz veya birleştirmez.
