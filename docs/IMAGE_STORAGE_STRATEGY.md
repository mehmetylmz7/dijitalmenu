# Dijital Menü - Görsel Depolama ve Kalıcılık Stratejisi

Bu doküman, kullanıcılar ve restoranlar tarafından yüklenen kategori ve menü ürün fotoğraflarının (`wwwroot/images/`) kaybolmasını önlemek için uygulanan depolama mimarisini ve gelecekteki bulut seçeneklerini açıklar.

---

## 1. Mimari Durumu ve Çözülen Problem

Uygulama, yüklenen görselleri sunucunun yerel dosya sistemine kaydetmektedir:
- Kategori fotoğrafları: `/app/wwwroot/images/categories/`
- Ürün fotoğrafları: `/app/wwwroot/images/menu-items/`

### Ephemeral Dosya Sistemi Riski:
Docker konteynerleri yeniden oluşturulduğunda veya Render.com'da yeni bir deployment yapıldığında geçici diskteki tüm yeni yüklenen dosyalar silinir. Bu risk aşağıdaki 3 aşamalı mimari ile tamamen çözülmüştür:

---

## 2. Uygulanan Çözüm Aşamaları

### Aşama 1: Yerel & Docker Compose Kalıcılığı (Uygulandı ✅)
`docker-compose.yml` içinde `web_images` volume'u tanımlanmıştır:
```yaml
services:
  web:
    volumes:
      - web_images:/app/wwwroot/images

volumes:
  postgres_data:
  web_images:
```
* **Sonuç:** Konteyner dursa veya imaj yeniden derlense bile görseller Docker volume içinde korunur.

---

### Aşama 2: Render.com Üzerinde Kalıcı Disk Tanımı (Uygulandı ✅)
`render.yaml` içerisine `disk` bloğu eklenmiştir:
```yaml
    disk:
      name: web_images
      mountPath: /app/wwwroot/images
      sizeGB: 1
```
* **Sonuç:** Render Web Service üzerinde `/app/wwwroot/images` dizini 1 GB boyutunda persistent diske bağlanır; deployment sonraları fotoğraflar korunur.

---

### Aşama 3: Storage Servis Mimarisi ve Refactoring (Uygulandı ✅)
Görsel işlemleri doğrudan controller'lar yerine merkezi bir soyutlama üzerinden yürütülmektedir:

1. **`IStorageService` Arayüzü ([dijitalmenu/Services/IStorageService.cs](file:///C:/dijitalmenu/dijitalmenu/Services/IStorageService.cs)):**
   ```csharp
   public interface IStorageService
   {
       bool TrySaveImage(IFormFile? file, string subFolder, out string? fileUrl, out string? errorMessage);
       bool DeleteImage(string? relativeFileUrl);
   }
   ```

2. **`LocalStorageService` ([dijitalmenu/Services/LocalStorageService.cs](file:///C:/dijitalmenu/dijitalmenu/Services/LocalStorageService.cs)):**
   - 5 MB dosya boyutu sınırı.
   - Güvenli dosya uzantıları (`.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`) ve MIME tipi doğrulaması.
   - `Guid` ile benzersiz dosya adı oluşturma.
   - `images/categories` ve `images/menu-items` klasörlerini otomatik oluşturma.
   - Eski/güncellenen görsellerin diskten silinmesi (`DeleteImage`).

3. **Controller Entegrasyonu:**
   - [CategoryController](file:///C:/dijitalmenu/dijitalmenu/Areas/Restaurant/Controllers/CategoryController.cs) ve [MenuItemController](file:///C:/dijitalmenu/dijitalmenu/Areas/Restaurant/Controllers/MenuItemController.cs) `IStorageService` kullanacak şekilde refactor edildi. Mükerrer `TrySavePhoto` kodları temizlendi ve fotoğraf güncellemelerinde yetim kalan eski fotoğrafların otomatik silinmesi sağlandı.

---

## 3. Gelecek Bulut Nesne Depolama (Cloud Object Storage - İsteğe Bağlı)

Uygulama multi-instance / load balancer arkasında yatay ölçeklendiğinde (horizontal scaling):
- `IStorageService` arayüzü sayesinde controller'larda **hiçbir değişiklik yapmadan** yeni bir `CloudinaryStorageService` veya `S3StorageService` yazılarak `Program.cs` içindeki DI kaydını değiştirmek yeterlidir.
