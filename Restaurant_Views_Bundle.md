# RESTAURANT AREA - TUM VIEW DOSYALARI (TOPLU DOSYA)

Toplam Dosya Sayisi: 16

## Içindekiler Tablosu
- [Account/ChangePassword.cshtml](#account-changepasswordcshtml)
- [Account/Index.cshtml](#account-indexcshtml)
- [AuditLog/Index.cshtml](#auditlog-indexcshtml)
- [Auth/Login.cshtml](#auth-logincshtml)
- [Auth/Register.cshtml](#auth-registercshtml)
- [Builder/Index.cshtml](#builder-indexcshtml)
- [Category/Create.cshtml](#category-createcshtml)
- [Category/Edit.cshtml](#category-editcshtml)
- [Category/Index.cshtml](#category-indexcshtml)
- [Dashboard/Index.cshtml](#dashboard-indexcshtml)
- [MenuItem/Create.cshtml](#menuitem-createcshtml)
- [MenuItem/Edit.cshtml](#menuitem-editcshtml)
- [MenuItem/Index.cshtml](#menuitem-indexcshtml)
- [Shared/_RestaurantLayout.cshtml](#shared-_restaurantlayoutcshtml)
- [_ViewImports.cshtml](#_viewimportscshtml)
- [_ViewStart.cshtml](#_viewstartcshtml)

================================================================================

## Account/ChangePassword.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/Account/ChangePassword.cshtml

`cshtml
@model dijitalmenu.Models.ChangePasswordViewModel
@{
    ViewData["Title"] = "Şifre Değiştir";
}

<div style="max-width: 600px;">
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px;">
        <h3 style="font-size: 1.25rem; font-weight: 700; color: var(--text);">🔒 Şifre Değiştir</h3>
        <a asp-area="Restaurant" asp-controller="Account" asp-action="Index" class="btn btn-secondary btn-sm">
            <span>←</span> Hesabım'a Dön
        </a>
    </div>

    <div class="card">
        <div class="card-header">
            <h5>Güvenli Şifre Güncelleme</h5>
        </div>
        <div class="card-body">
            <form asp-area="Restaurant" asp-controller="Account" asp-action="ChangePassword" method="post">
                <div class="form-group">
                    <label class="form-label" for="currentPassword">Mevcut Şifre</label>
                    <input type="password" id="currentPassword" name="currentPassword" class="form-control" required autocomplete="current-password" placeholder="••••••••••••" />
                </div>

                <div class="form-group">
                    <label class="form-label" for="newPassword">Yeni Şifre</label>
                    <input type="password" id="newPassword" name="newPassword" class="form-control" required minlength="12" autocomplete="new-password" placeholder="••••••••••••" />
                    <small style="color: var(--text-muted); font-size: 0.75rem; display: block; margin-top: 6px;">
                        En az 12 karakter; büyük harf, küçük harf, rakam ve özel karakter içermelidir.
                    </small>
                </div>

                <div class="form-group">
                    <label class="form-label" for="confirmPassword">Yeni Şifre (Tekrar)</label>
                    <input type="password" id="confirmPassword" name="confirmPassword" class="form-control" required minlength="12" autocomplete="new-password" placeholder="••••••••••••" />
                </div>

                <div style="background: var(--surface2); border: 1px solid var(--border); padding: 14px 16px; border-radius: 12px; margin-bottom: 20px; font-size: 0.8rem; color: var(--text-muted);">
                    <p style="font-weight: 600; color: var(--text); margin-bottom: 4px;">📌 Şifre Güvenlik Kuralları:</p>
                    <ul style="padding-left: 20px; line-height: 1.6;">
                        <li>En az 12 karakter uzunluğunda olmalıdır.</li>
                        <li>En az bir büyük harf (A-Z) ve bir küçük harf (a-z) içermelidir.</li>
                        <li>En az bir rakam (0-9) içermelidir.</li>
                        <li>En az bir özel karakter (!, @@, #, $, %, vs.) içermelidir.</li>
                        <li>Mevcut şifrenizle aynı olamaz.</li>
                    </ul>
                </div>

                <div style="display: flex; gap: 10px;">
                    <button type="submit" class="btn btn-primary">
                        <span>🔒</span> Şifreyi Güncelle
                    </button>
                    <a asp-area="Restaurant" asp-controller="Account" asp-action="Index" class="btn btn-secondary">
                        İptal
                    </a>
                </div>
            </form>
        </div>
    </div>
</div>

`

--------------------------------------------------------------------------------

## Account/Index.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/Account/Index.cshtml

`cshtml
@model dijitalmenu.Models.RestaurantAccountViewModel
@{
    ViewData["Title"] = "Hesabım";
}

<div style="display: flex; flex-direction: column; gap: 24px; max-width: 900px;">
    <!-- Başlık ve Hızlı Navigasyon -->
    <div style="display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 12px;">
        <div>
            <h3 style="font-size: 1.25rem; font-weight: 700; color: var(--text);">👤 Hesap ve İşletme Yönetimi</h3>
            <p style="font-size: 0.875rem; color: var(--text-muted); margin-top: 4px;">Kullanıcı profilinizi ve restoran işletme bilgilerinizi buradan yönetebilirsiniz.</p>
        </div>
        <a asp-area="Restaurant" asp-controller="Account" asp-action="ChangePassword" class="btn btn-warning">
            <span>🔒</span> Şifre Değiştir
        </a>
    </div>

    <!-- 1. Profil Bilgileri Kartı -->
    <div class="card">
        <div class="card-header">
            <h5>👤 Profil Bilgileri</h5>
        </div>
        <div class="card-body">
            <form asp-area="Restaurant" asp-controller="Account" asp-action="UpdateProfile" method="post">
                <div class="form-group">
                    <label class="form-label" for="username">Kullanıcı Adı</label>
                    <input type="text" id="username" name="username" class="form-control" value="@Model.Username" required minlength="3" maxlength="50" autocomplete="username" />
                    <small style="color: var(--text-muted); font-size: 0.75rem; display: block; margin-top: 6px;">
                        Kullanıcı adınız giriş yaparken kullanılır. 3-50 karakter arası olmalıdır.
                    </small>
                </div>
                <button type="submit" class="btn btn-primary">
                    <span>💾</span> Profil Bilgilerini Kaydet
                </button>
            </form>
        </div>
    </div>

    <!-- 2. İşletme Bilgileri Kartı -->
    <div class="card">
        <div class="card-header">
            <h5>🏪 İşletme Bilgileri</h5>
        </div>
        <div class="card-body">
            <form asp-area="Restaurant" asp-controller="Account" asp-action="UpdateBusiness" method="post">
                <div class="form-group">
                    <label class="form-label" for="restaurantName">Restoran Adı</label>
                    <input type="text" id="restaurantName" name="restaurantName" class="form-control" value="@Model.RestaurantName" required minlength="2" maxlength="100" />
                </div>

                <div class="form-group">
                    <label class="form-label" for="phone">İletişim Telefonu</label>
                    <input type="tel" id="phone" name="phone" class="form-control" value="@Model.Phone" placeholder="Örn: 0555 123 45 67" maxlength="25" />
                    <small style="color: var(--text-muted); font-size: 0.75rem; display: block; margin-top: 6px;">
                        Müşterilerinizin menü üzerinden görebileceği irtibat numarası.
                    </small>
                </div>

                <div class="form-group">
                    <label class="form-label" for="address">Adres</label>
                    <textarea id="address" name="address" class="form-control" rows="3" placeholder="Restoran açık adresi..." maxlength="500">@Model.Address</textarea>
                </div>

                <div class="form-group">
                    <label class="form-label" for="googleMapsUrl">Google Haritalar (Maps) Bağlantısı</label>
                    <input type="url" id="googleMapsUrl" name="googleMapsUrl" class="form-control" value="@Model.GoogleMapsUrl" placeholder="https://maps.google.com/..." maxlength="2048" />
                    <small style="color: var(--text-muted); font-size: 0.75rem; display: block; margin-top: 6px;">
                        Google Haritalar URL'si veya Paylaş bağlantısı girebilirsiniz.
                    </small>
                </div>

                <div class="form-group">
                    <label class="form-label" for="workingHours">⏰ Çalışma Saatleri</label>
                    <input type="text" id="workingHours" name="workingHours" class="form-control" value="@Model.WorkingHours" placeholder="Örn: Hafta içi: 09:00 - 23:00, Hafta sonu: 10:00 - 00:00" maxlength="200" />
                    <small style="color: var(--text-muted); font-size: 0.75rem; display: block; margin-top: 6px;">
                        Müşterilerinize menüde gösterilecek çalışma gün ve saatleri.
                    </small>
                </div>

                <div class="form-group">
                    <label class="form-label" for="importantNotice">📢 Menü Üstü Önemli Bilgilendirme / Duyuru Notu</label>
                    <textarea id="importantNotice" name="importantNotice" class="form-control" rows="3" placeholder="Örn: Menü fiyatlarımıza KDV dahildir. Masanıza 100 TL servis ücreti (kuver) yansıtılacaktır..." maxlength="1000">@Model.ImportantNotice</textarea>
                    <small style="color: var(--text-muted); font-size: 0.75rem; display: block; margin-top: 6px;">
                        İsteğe bağlıdır. Boş bırakırsanız menünüzde bilgilendirme kutusu gösterilmez.
                    </small>
                </div>

                <button type="submit" class="btn btn-primary">
                    <span>💾</span> İşletme Bilgilerini Kaydet
                </button>
            </form>
        </div>
    </div>

    <!-- 3. Güvenlik Özeti Kartı -->
    <div class="card">
        <div class="card-header">
            <h5>🛡️ Güvenlik</h5>
        </div>
        <div class="card-body" style="display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 16px;">
            <div>
                <p style="font-weight: 600; font-size: 0.9rem; color: var(--text);">Hesap Şifresi</p>
                <p style="font-size: 0.8rem; color: var(--text-muted); margin-top: 2px;">Hesap güvenliğiniz için şifrenizi düzenli aralıklarla güncellemenizi öneririz.</p>
            </div>
            <a asp-area="Restaurant" asp-controller="Account" asp-action="ChangePassword" class="btn btn-secondary">
                <span>🔑</span> Şifremi Değiştir
            </a>
        </div>
    </div>
</div>

`

--------------------------------------------------------------------------------

## AuditLog/Index.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/AuditLog/Index.cshtml

`cshtml
@model List<EntityLayer.Concrete.AuditLog>
@{
    ViewData["Title"] = "İşlem Geçmişi (Audit Logs)";
    Layout = "~/Areas/Restaurant/Views/Shared/_RestaurantLayout.cshtml";
    var filter = ViewBag.Filter as BusinessLayer.Models.AuditLogFilterDto ?? new BusinessLayer.Models.AuditLogFilterDto();
    int currentPage = ViewBag.CurrentPage ?? 1;
    int totalPages = ViewBag.TotalPages ?? 1;
    int totalCount = ViewBag.TotalCount ?? 0;
}

<style>
    .filter-card {
        background: #1e293b;
        border: 1px solid #334155;
        border-radius: 12px;
        padding: 20px;
        margin-bottom: 24px;
    }
    .filter-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
        gap: 14px;
        align-items: flex-end;
    }
    .badge-action {
        display: inline-block;
        padding: 4px 10px;
        border-radius: 6px;
        font-size: 0.75rem;
        font-weight: 600;
    }
    .badge-login-success { background: rgba(34, 197, 94, 0.15); color: #4ade80; }
    .badge-login-failed { background: rgba(255, 77, 109, 0.15); color: #ff4d6d; }
    .badge-logout { background: rgba(148, 163, 184, 0.15); color: #94a3b8; }
    .badge-create { background: rgba(59, 130, 246, 0.15); color: #60a5fa; }
    .badge-update { background: rgba(245, 158, 11, 0.15); color: #fbbf24; }
    .badge-delete { background: rgba(239, 68, 68, 0.15); color: #f87171; }
    .badge-other { background: rgba(108, 99, 255, 0.15); color: #818cf8; }

    /* Modal Styles */
    .modal-backdrop {
        position: fixed;
        top: 0; left: 0; right: 0; bottom: 0;
        background: rgba(0, 0, 0, 0.75);
        backdrop-filter: blur(4px);
        display: none;
        justify-content: center;
        align-items: center;
        z-index: 1000;
        padding: 20px;
    }
    .modal-box {
        background: #1e293b;
        border: 1px solid #334155;
        border-radius: 16px;
        width: 100%;
        max-width: 750px;
        max-height: 90vh;
        overflow-y: auto;
        display: flex;
        flex-direction: column;
        color: #f8fafc;
    }
    .modal-header {
        padding: 18px 24px;
        border-bottom: 1px solid #334155;
        display: flex;
        justify-content: space-between;
        align-items: center;
    }
    .modal-body {
        padding: 24px;
        display: flex;
        flex-direction: column;
        gap: 16px;
    }
    .json-container {
        background: #0f172a;
        border: 1px solid #334155;
        border-radius: 8px;
        padding: 12px;
        font-family: monospace;
        font-size: 0.82rem;
        color: #38bdf8;
        max-height: 200px;
        overflow-y: auto;
        white-space: pre-wrap;
    }
</style>

<div class="d-flex justify-content-between align-items-center mb-4">
    <div>
        <h2 class="h4 font-weight-bold text-white mb-1">📜 Restoran İşlem Geçmişi (Audit Logs)</h2>
        <p class="text-muted small mb-0">Menü düzenlemeleri, giriş/çıkış hareketleri ve restoranınıza ait denetim kayıtları.</p>
    </div>
    <div class="text-muted small">
        Toplam Kayıt: <strong class="text-white">@totalCount</strong>
    </div>
</div>

<!-- Filtre -->
<div class="filter-card">
    <form method="get" asp-area="Restaurant" asp-controller="AuditLog" asp-action="Index">
        <div class="filter-grid">
            <div>
                <label class="small text-muted font-weight-bold">Başlangıç Tarihi</label>
                <input type="datetime-local" name="dateFrom" class="form-control form-control-sm bg-dark text-white border-secondary"
                       value="@(filter.DateFrom.HasValue ? filter.DateFrom.Value.ToString("yyyy-MM-ddTHH:mm") : "")" />
            </div>
            <div>
                <label class="small text-muted font-weight-bold">Bitiş Tarihi</label>
                <input type="datetime-local" name="dateTo" class="form-control form-control-sm bg-dark text-white border-secondary"
                       value="@(filter.DateTo.HasValue ? filter.DateTo.Value.ToString("yyyy-MM-ddTHH:mm") : "")" />
            </div>
            <div>
                <label class="small text-muted font-weight-bold">İşlem Türü</label>
                <select name="action" class="form-control form-control-sm bg-dark text-white border-secondary">
                    <option value="">Tümü</option>
                    <option value="LOGIN_SUCCESS" selected="@(filter.Action == "LOGIN_SUCCESS")">Giriş Başarılı</option>
                    <option value="LOGOUT" selected="@(filter.Action == "LOGOUT")">Çıkış</option>
                    <option value="PASSWORD_CHANGED" selected="@(filter.Action == "PASSWORD_CHANGED")">Şifre Değişimi</option>
                    <option value="MENU_ITEM_CREATED" selected="@(filter.Action == "MENU_ITEM_CREATED")">Ürün Ekleme</option>
                    <option value="MENU_ITEM_UPDATED" selected="@(filter.Action == "MENU_ITEM_UPDATED")">Ürün Güncelleme</option>
                    <option value="MENU_ITEM_DELETED" selected="@(filter.Action == "MENU_ITEM_DELETED")">Ürün Silme</option>
                    <option value="CATEGORY_CREATED" selected="@(filter.Action == "CATEGORY_CREATED")">Kategori Ekleme</option>
                    <option value="CATEGORY_UPDATED" selected="@(filter.Action == "CATEGORY_UPDATED")">Kategori Güncelleme</option>
                    <option value="CATEGORY_DELETED" selected="@(filter.Action == "CATEGORY_DELETED")">Kategori Silme</option>
                    <option value="RESTAURANT_UPDATED" selected="@(filter.Action == "RESTAURANT_UPDATED")">Bilgi Güncelleme</option>
                </select>
            </div>
            <div>
                <label class="small text-muted font-weight-bold">Arama</label>
                <input type="text" name="keyword" class="form-control form-control-sm bg-dark text-white border-secondary" placeholder="Açıklama, IP..." value="@filter.Keyword" />
            </div>
            <div class="d-flex gap-2">
                <button type="submit" class="btn btn-primary btn-sm flex-fill">Filtrele</button>
                <a asp-area="Restaurant" asp-controller="AuditLog" asp-action="Index" class="btn btn-secondary btn-sm" title="Sıfırla">✕</a>
            </div>
        </div>
    </form>
</div>

<!-- Tablo -->
<div class="card bg-dark border-secondary">
    <div class="table-responsive">
        <table class="table table-dark table-hover mb-0">
            <thead>
                <tr class="text-muted small text-uppercase">
                    <th>Tarih</th>
                    <th>İşlem</th>
                    <th>Varlık</th>
                    <th>IP Adresi</th>
                    <th>Açıklama</th>
                    <th style="width:70px; text-align:center;">Detay</th>
                </tr>
            </thead>
            <tbody>
                @if (Model == null || !Model.Any())
                {
                    <tr>
                        <td colspan="6" class="text-center py-5 text-muted">
                            Kayıtlı işlem geçmişi bulunamadı.
                        </td>
                    </tr>
                }
                else
                {
                    foreach (var log in Model)
                    {
                        string badgeClass = "badge-other";
                        if (log.Action == "LOGIN_SUCCESS") badgeClass = "badge-login-success";
                        else if (log.Action == "LOGIN_FAILED") badgeClass = "badge-login-failed";
                        else if (log.Action == "LOGOUT") badgeClass = "badge-logout";
                        else if (log.Action.EndsWith("_CREATED")) badgeClass = "badge-create";
                        else if (log.Action.EndsWith("_UPDATED") || log.Action == "PASSWORD_CHANGED") badgeClass = "badge-update";
                        else if (log.Action.EndsWith("_DELETED")) badgeClass = "badge-delete";

                        <tr>
                            <td class="small text-muted" style="font-family:monospace;">
                                @log.CreatedAt.ToString("dd.MM.yyyy HH:mm:ss")
                            </td>
                            <td>
                                <span class="badge-action @badgeClass">@log.Action</span>
                            </td>
                            <td class="small">
                                @(log.EntityType ?? "-") @(log.EntityId.HasValue ? $"#{log.EntityId}" : "")
                            </td>
                            <td class="small text-muted" style="font-family:monospace;">
                                @(log.IpAddress ?? "-")
                            </td>
                            <td class="small text-truncate" style="max-width:280px;" title="@log.Description">
                                @(log.Description ?? "-")
                            </td>
                            <td class="text-center">
                                <button type="button" class="btn btn-xs btn-outline-info" onclick="openDetailModal('@log.Id')">
                                     İncele
                                </button>
                            </td>
                        </tr>
                    }
                }
            </tbody>
        </table>
    </div>
</div>

<!-- Modal -->
<div id="detailModal" class="modal-backdrop">
    <div class="modal-box">
        <div class="modal-header">
            <h5 id="modalTitle" class="mb-0 font-weight-bold">İşlem Detayı</h5>
            <button type="button" class="close text-white" onclick="closeDetailModal()">&times;</button>
        </div>
        <div class="modal-body">
            <div class="row g-3 small mb-3">
                <div class="col-6">
                    <span class="text-muted d-block">İşlem:</span>
                    <strong id="mAction"></strong>
                </div>
                <div class="col-6">
                    <span class="text-muted d-block">Tarih:</span>
                    <span id="mCreatedAt"></span>
                </div>
                <div class="col-6">
                    <span class="text-muted d-block">Varlık:</span>
                    <span id="mEntity"></span>
                </div>
                <div class="col-6">
                    <span class="text-muted d-block">IP Adresi:</span>
                    <span id="mIp"></span>
                </div>
                <div class="col-12">
                    <span class="text-muted d-block">Açıklama:</span>
                    <span id="mDesc"></span>
                </div>
            </div>

            <div id="oldValuesBlock" style="display:none;">
                <h6 class="small font-weight-bold text-danger mb-1">Önceki Değerler (Old Values)</h6>
                <div id="mOldValues" class="json-container"></div>
            </div>

            <div id="newValuesBlock" style="display:none;">
                <h6 class="small font-weight-bold text-success mb-1">Yeni Değerler (New Values)</h6>
                <div id="mNewValues" class="json-container"></div>
            </div>
        </div>
    </div>
</div>

<script>
    function openDetailModal(id) {
        fetch('@Url.Action("Details", "AuditLog", new { area = "Restaurant" })/' + id)
            .then(res => res.json())
            .then(data => {
                document.getElementById('modalTitle').innerText = 'İşlem #' + data.id + ' — ' + data.action;
                document.getElementById('mAction').innerText = data.action;
                document.getElementById('mCreatedAt').innerText = data.createdAt;
                document.getElementById('mEntity').innerText = data.entityType ? data.entityType + (data.entityId ? ' #' + data.entityId : '') : '-';
                document.getElementById('mIp').innerText = data.ipAddress || '-';
                document.getElementById('mDesc').innerText = data.description || '-';

                var oldBlock = document.getElementById('oldValuesBlock');
                var newBlock = document.getElementById('newValuesBlock');

                if (data.oldValues) {
                    try {
                        var parsed = JSON.parse(data.oldValues);
                        document.getElementById('mOldValues').innerText = JSON.stringify(parsed, null, 2);
                    } catch(e) {
                        document.getElementById('mOldValues').innerText = data.oldValues;
                    }
                    oldBlock.style.display = 'block';
                } else {
                    oldBlock.style.display = 'none';
                }

                if (data.newValues) {
                    try {
                        var parsed = JSON.parse(data.newValues);
                        document.getElementById('mNewValues').innerText = JSON.stringify(parsed, null, 2);
                    } catch(e) {
                        document.getElementById('mNewValues').innerText = data.newValues;
                    }
                    newBlock.style.display = 'block';
                } else {
                    newBlock.style.display = 'none';
                }

                document.getElementById('detailModal').style.display = 'flex';
            })
            .catch(err => alert('Detay yüklenemedi.'));
    }

    function closeDetailModal() {
        document.getElementById('detailModal').style.display = 'none';
    }

    window.onclick = function(event) {
        var modal = document.getElementById('detailModal');
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    };
</script>

`

--------------------------------------------------------------------------------

## Auth/Login.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/Auth/Login.cshtml

`cshtml
@{
    Layout = null;
}
<!DOCTYPE html>
<html lang="tr">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Restoran Girişi — Dijital Menü</title>

    <!-- Google Fonts: Sora & Inter -->
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&family=Sora:wght@600;700;800&display=swap" rel="stylesheet" />

    <style>
        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

        :root {
            --primary: #10B981;
            --primary-dark: #006c49;
            --primary-light: #4edea3;
            --charcoal: #111827;
            --bg: #f8f9fa;
            --surface: #ffffff;
            --text: #191c1d;
            --text-secondary: #575e70;
            --border: #E5E7EB;
            --shadow-lg: 0 20px 48px rgba(17, 24, 39, 0.08);
            --radius-md: 14px;
            --radius-lg: 24px;
            --font-display: 'Sora', sans-serif;
            --font-body: 'Inter', sans-serif;
        }

        body {
            font-family: var(--font-body);
            background: var(--bg);
            color: var(--text);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 24px;
            position: relative;
            overflow-x: hidden;
        }

        .ambient-glow {
            position: absolute;
            top: 20%; left: 50%;
            transform: translateX(-50%);
            width: 500px; height: 500px;
            background: radial-gradient(circle, rgba(16, 185, 129, 0.12) 0%, rgba(248, 249, 250, 0) 70%);
            border-radius: 50%;
            pointer-events: none;
            z-index: 0;
        }

        .login-card {
            position: relative;
            z-index: 10;
            background: var(--surface);
            border: 1px solid var(--border);
            border-radius: var(--radius-lg);
            padding: 44px 40px;
            width: 100%;
            max-width: 440px;
            box-shadow: var(--shadow-lg);
        }

        .logo-header {
            text-align: center;
            margin-bottom: 32px;
        }

        .logo-icon {
            width: 52px;
            height: 52px;
            border-radius: 14px;
            background: linear-gradient(135deg, var(--primary), var(--primary-dark));
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 1.8rem;
            color: #ffffff;
            box-shadow: 0 4px 14px rgba(16, 185, 129, 0.3);
            margin-bottom: 16px;
        }

        .logo-header h1 {
            font-family: var(--font-display);
            font-size: 1.5rem;
            font-weight: 700;
            color: var(--charcoal);
        }

        .logo-header p {
            font-size: 0.875rem;
            color: var(--text-secondary);
            margin-top: 4px;
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-label {
            display: block;
            font-size: 0.875rem;
            font-weight: 600;
            color: var(--charcoal);
            margin-bottom: 8px;
        }

        .form-control {
            width: 100%;
            padding: 12px 16px;
            background: #ffffff;
            border: 1px solid var(--border);
            border-radius: 12px;
            color: var(--text);
            font-size: 0.9rem;
            font-family: inherit;
            outline: none;
            transition: all 0.2s ease;
        }

        .form-control:focus {
            border-color: var(--primary);
            box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.15);
        }

        .btn-submit {
            width: 100%;
            padding: 14px;
            background: var(--primary);
            border: none;
            border-radius: 12px;
            color: #ffffff;
            font-size: 1rem;
            font-weight: 700;
            font-family: inherit;
            cursor: pointer;
            margin-top: 8px;
            box-shadow: 0 4px 14px rgba(16, 185, 129, 0.25);
            transition: all 0.2s ease;
        }

        .btn-submit:hover {
            background: var(--primary-dark);
            transform: translateY(-1px);
            box-shadow: 0 6px 18px rgba(16, 185, 129, 0.35);
        }

        .alert-error {
            background: rgba(239, 68, 68, 0.08);
            border: 1px solid rgba(239, 68, 68, 0.25);
            border-radius: 10px;
            padding: 12px 16px;
            color: #b91c1c;
            font-size: 0.875rem;
            font-weight: 500;
            margin-bottom: 20px;
            animation: fadeIn 0.25s ease;
        }

        .footer-link {
            text-align: center;
            margin-top: 24px;
            font-size: 0.875rem;
            color: var(--text-secondary);
        }

        .footer-link a {
            color: var(--primary-dark);
            font-weight: 600;
            text-decoration: none;
        }

        .footer-link a:hover {
            text-decoration: underline;
        }

        @@keyframes fadeIn {
            from { opacity: 0; transform: translateY(-4px); }
            to { opacity: 1; transform: translateY(0); }
        }

        @@media (max-width: 480px) {
            body { padding: 16px 12px; }
            .login-card { padding: 28px 20px; border-radius: 18px; }
            .logo-header h1 { font-size: 1.3rem; }
        }
    </style>
</head>
<body>
    <div class="ambient-glow"></div>

    <div class="login-card">
        <div class="logo-header">
            <div class="logo-icon">🍃</div>
            <h1>Restoran Paneli</h1>
            <p>Yönetim hesabınıza giriş yapın</p>
        </div>

        @if (ViewBag.Error != null)
        {
            <div class="alert-error">⚠️ @ViewBag.Error</div>
        }

        <form method="post" asp-area="Restaurant" asp-controller="Auth" asp-action="Login">
            <div class="form-group">
                <label class="form-label" for="username">Kullanıcı Adı</label>
                <input type="text" id="username" name="username" class="form-control" placeholder="kullaniciadi" required autofocus autocomplete="username" />
            </div>

            <div class="form-group">
                <label class="form-label" for="password">Şifre</label>
                <input type="password" id="password" name="password" class="form-control" placeholder="••••••••" required autocomplete="current-password" />
            </div>

            <button type="submit" class="btn-submit">Giriş Yap →</button>
        </form>

        <div class="footer-link">
            Henüz hesabınız yok mu?
            <a asp-area="Restaurant" asp-controller="Auth" asp-action="Register">Ücretsiz Kayıt Olun</a>
        </div>
    </div>
</body>
</html>

`

--------------------------------------------------------------------------------

## Auth/Register.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/Auth/Register.cshtml

`cshtml
@{
    Layout = null;
    var themes = ViewBag.Themes as List<EntityLayer.Concrete.Theme> ?? new();
}
<!DOCTYPE html>
<html lang="tr">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Restoran Kaydı — Dijital Menü</title>

    <!-- Google Fonts: Sora & Inter -->
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&family=Sora:wght@600;700;800&display=swap" rel="stylesheet" />

    <style>
        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

        :root {
            --primary: #10B981;
            --primary-dark: #006c49;
            --primary-light: #4edea3;
            --charcoal: #111827;
            --bg: #f8f9fa;
            --surface: #ffffff;
            --surface-low: #f3f4f5;
            --text: #191c1d;
            --text-secondary: #575e70;
            --border: #E5E7EB;
            --shadow-lg: 0 20px 48px rgba(17, 24, 39, 0.08);
            --radius-md: 14px;
            --radius-lg: 24px;
            --font-display: 'Sora', sans-serif;
            --font-body: 'Inter', sans-serif;
        }

        body {
            font-family: var(--font-body);
            background: var(--bg);
            color: var(--text);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 32px 24px;
            position: relative;
            overflow-x: hidden;
        }

        .ambient-glow {
            position: absolute;
            top: 15%; left: 50%;
            transform: translateX(-50%);
            width: 600px; height: 600px;
            background: radial-gradient(circle, rgba(16, 185, 129, 0.12) 0%, rgba(248, 249, 250, 0) 70%);
            border-radius: 50%;
            pointer-events: none;
            z-index: 0;
        }

        .register-card {
            position: relative;
            z-index: 10;
            background: var(--surface);
            border: 1px solid var(--border);
            border-radius: var(--radius-lg);
            padding: 44px 40px;
            width: 100%;
            max-width: 520px;
            box-shadow: var(--shadow-lg);
        }

        .logo-header {
            text-align: center;
            margin-bottom: 28px;
        }

        .logo-icon {
            width: 52px;
            height: 52px;
            border-radius: 14px;
            background: linear-gradient(135deg, var(--primary), var(--primary-dark));
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 1.8rem;
            color: #ffffff;
            box-shadow: 0 4px 14px rgba(16, 185, 129, 0.3);
            margin-bottom: 14px;
        }

        .logo-header h1 {
            font-family: var(--font-display);
            font-size: 1.5rem;
            font-weight: 700;
            color: var(--charcoal);
        }

        .logo-header p {
            font-size: 0.875rem;
            color: var(--text-secondary);
            margin-top: 4px;
        }

        .section-divider {
            font-size: 0.72rem;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.08em;
            color: var(--text-secondary);
            margin: 24px 0 14px;
            padding-top: 14px;
            border-top: 1px solid var(--border);
        }

        .form-group {
            margin-bottom: 18px;
        }

        .form-label {
            display: block;
            font-size: 0.875rem;
            font-weight: 600;
            color: var(--charcoal);
            margin-bottom: 8px;
        }

        .form-control {
            width: 100%;
            padding: 11px 16px;
            background: #ffffff;
            border: 1px solid var(--border);
            border-radius: 12px;
            color: var(--text);
            font-size: 0.9rem;
            font-family: inherit;
            outline: none;
            transition: all 0.2s ease;
        }

        .form-control:focus {
            border-color: var(--primary);
            box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.15);
        }

        .theme-label-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 10px;
        }

        .theme-grid {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 10px;
            max-height: 280px;
            overflow-y: auto;
            padding-right: 4px;
        }

        .theme-option {
            border: 2px solid var(--border);
            border-radius: 12px;
            overflow: hidden;
            cursor: pointer;
            transition: all 0.2s ease;
            position: relative;
            background: var(--surface-low);
        }

        .theme-option:hover {
            border-color: #cbd5e1;
            transform: translateY(-2px);
        }

        .theme-option.selected {
            border-color: var(--primary);
            box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.2);
        }

        .theme-swatch {
            height: 48px;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            padding: 0 10px;
        }

        .swatch-dot {
            width: 10px; height: 10px;
            border-radius: 50%;
            flex-shrink: 0;
        }

        .swatch-bar {
            height: 5px; border-radius: 3px;
            flex: 1; opacity: 0.7;
        }

        .theme-option-info {
            padding: 7px 10px;
            border-top: 1px solid var(--border);
            background: #ffffff;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .theme-option-name {
            font-size: 0.75rem;
            font-weight: 600;
            color: var(--charcoal);
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .theme-check {
            width: 18px; height: 18px;
            border-radius: 50%;
            border: 1px solid var(--border);
            display: flex; align-items: center; justify-content: center;
            font-size: 0.65rem;
            color: transparent;
            transition: all 0.2s ease;
        }

        .theme-option.selected .theme-check {
            background: var(--primary);
            border-color: var(--primary);
            color: #ffffff;
        }

        .btn-submit {
            width: 100%;
            padding: 14px;
            background: var(--primary);
            border: none;
            border-radius: 12px;
            color: #ffffff;
            font-size: 1rem;
            font-weight: 700;
            font-family: inherit;
            cursor: pointer;
            margin-top: 12px;
            box-shadow: 0 4px 14px rgba(16, 185, 129, 0.25);
            transition: all 0.2s ease;
        }

        .btn-submit:hover {
            background: var(--primary-dark);
            transform: translateY(-1px);
            box-shadow: 0 6px 18px rgba(16, 185, 129, 0.35);
        }

        .alert-error {
            background: rgba(239, 68, 68, 0.08);
            border: 1px solid rgba(239, 68, 68, 0.25);
            border-radius: 10px;
            padding: 12px 16px;
            color: #b91c1c;
            font-size: 0.875rem;
            font-weight: 500;
            margin-bottom: 20px;
            animation: fadeIn 0.25s ease;
        }

        .password-hint {
            display: flex;
            align-items: center;
            gap: 6px;
            font-size: 0.75rem;
            color: var(--text-secondary);
            margin-top: 6px;
            line-height: 1.4;
        }

        .footer-link {
            text-align: center;
            margin-top: 24px;
            font-size: 0.875rem;
            color: var(--text-secondary);
        }

        .footer-link a {
            color: var(--primary-dark);
            font-weight: 600;
            text-decoration: none;
        }

        .footer-link a:hover {
            text-decoration: underline;
        }

        @@keyframes fadeIn {
            from { opacity: 0; transform: translateY(-4px); }
            to { opacity: 1; transform: translateY(0); }
        }

        @@media (max-width: 540px) {
            body { padding: 16px 12px; }
            .register-card { padding: 28px 18px; border-radius: 18px; }
            .theme-grid { grid-template-columns: 1fr; max-height: 220px; }
            .logo-header h1 { font-size: 1.3rem; }
        }
    </style>
</head>
<body>
    <div class="ambient-glow"></div>

    <div class="register-card">
        <div class="logo-header">
            <div class="logo-icon">🍃</div>
            <h1>Ücretsiz Kayıt Olun</h1>
            <p>Restoranınız için dijital menünüzü dakikalar içinde kurun</p>
        </div>

        @if (ViewBag.Error != null)
        {
            <div class="alert-error">⚠️ @ViewBag.Error</div>
        }

        <form method="post" asp-area="Restaurant" asp-controller="Auth" asp-action="Register" id="registerForm">
            <div class="section-divider">🏪 Restoran Bilgileri</div>

            <div class="form-group">
                <label class="form-label" for="restaurantName">Restoran Adı</label>
                <input type="text" id="restaurantName" name="restaurantName" class="form-control"
                       placeholder="Örn: Lezzet Köşesi" required autofocus />
            </div>

            <!-- Tema Seçimi -->
            <div class="form-group">
                <div class="theme-label-row">
                    <label class="form-label">🎨 Başlangıç Teması</label>
                    <span style="font-size:0.75rem; color:var(--text-secondary);">Daha sonra değiştirebilirsiniz</span>
                </div>

                <input type="hidden" name="themeId" id="selectedThemeId" value="@(themes.FirstOrDefault()?.Id ?? 0)" />

                @if (themes.Any())
                {
                    <div class="theme-grid">
                        @foreach (var theme in themes)
                        {
                            var isFirst = theme.Id == (themes.FirstOrDefault()?.Id ?? 0);
                            <div class="theme-option @(isFirst ? "selected" : "")"
                                 id="opt-@theme.Id"
                                 onclick="selectTheme(@theme.Id)">
                                <div class="theme-swatch" style="background: @theme.BackgroundColor;">
                                    <div class="swatch-dot" style="background: @theme.PrimaryColor;"></div>
                                    <div class="swatch-bar" style="background: @theme.PrimaryColor;"></div>
                                    <div class="swatch-dot" style="background: @theme.SecondaryColor; width:7px; height:7px;"></div>
                                </div>
                                <div class="theme-option-info">
                                    <span class="theme-option-name">@theme.Name</span>
                                    <div class="theme-check" id="chk-@theme.Id">✓</div>
                                </div>
                            </div>
                        }
                    </div>
                }
                else
                {
                    <p style="color:var(--text-secondary); font-size:0.85rem; padding: 8px 0;">
                        Varsayılan tema atanacaktır.
                    </p>
                }
            </div>

            <div class="section-divider">🔐 Giriş Bilgileri</div>

            <div class="form-group">
                <label class="form-label" for="username">Kullanıcı Adı</label>
                <input type="text" id="username" name="username" class="form-control"
                       placeholder="kullanici_adiniz" required autocomplete="username" />
            </div>

            <div class="form-group">
                <label class="form-label" for="password">Şifre</label>
                <input type="password" id="password" name="password" class="form-control"
                       placeholder="••••••••" required autocomplete="new-password" />
                <div class="password-hint">
                    <span>ℹ️</span> En az 12 karakter; büyük, küçük harf, rakam ve özel karakter içermelidir.
                </div>
            </div>

            <button type="submit" class="btn-submit" id="submitBtn">
                Restoranımı Kur →
            </button>
        </form>

        <div class="footer-link">
            Zaten hesabınız var mı?
            <a asp-area="Restaurant" asp-controller="Auth" asp-action="Login">Giriş yapın</a>
        </div>
    </div>

    <script>
        function selectTheme(id) {
            document.getElementById('selectedThemeId').value = id;
            document.querySelectorAll('.theme-option').forEach(el => el.classList.remove('selected'));
            document.getElementById('opt-' + id).classList.add('selected');
        }
    </script>
</body>
</html>

`

--------------------------------------------------------------------------------

## Builder/Index.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/Builder/Index.cshtml

`cshtml
@using EntityLayer.Concrete
@{
    ViewData["Title"] = "Canli Menu Olusturucu";
    Layout = "~/Areas/Restaurant/Views/Shared/_RestaurantLayout.cshtml";
    var restaurant = ViewBag.Restaurant as Restaurant;
    var categories = ViewBag.Categories as List<Category>;
    var menuItems = ViewBag.MenuItems as List<MenuItem>;
    var themes = ViewBag.Themes as List<Theme>;
    var suggestions = ViewBag.Suggestions as List<string>;
    var currentThemeId = restaurant?.ThemeId ?? 0;
}

@section Styles {
<style>
    .content { padding: 0 !important; overflow: hidden; }

    .builder-shell {
        display: flex;
        height: calc(100vh - 65px);
        overflow: hidden;
        background: #0d1117;
    }

    /* LEFT PANEL */
    .builder-panel {
        width: 420px;
        min-width: 320px;
        background: #161b22;
        border-right: 1px solid #30363d;
        display: flex;
        flex-direction: column;
        overflow: hidden;
    }

    .panel-header {
        padding: 20px 24px 16px;
        border-bottom: 1px solid #30363d;
        background: #161b22;
        flex-shrink: 0;
    }

    .panel-header h2 {
        font-size: 1rem;
        font-weight: 700;
        color: #e6edf3;
        margin: 0 0 2px;
        display: flex;
        align-items: center;
        gap: 8px;
    }

    .panel-header p {
        font-size: 0.75rem;
        color: #7d8590;
        margin: 0;
    }

    .panel-body {
        flex: 1;
        overflow-y: auto;
        padding: 16px;
        scrollbar-width: thin;
        scrollbar-color: #30363d transparent;
    }

    .panel-body::-webkit-scrollbar { width: 4px; }
    .panel-body::-webkit-scrollbar-track { background: transparent; }
    .panel-body::-webkit-scrollbar-thumb { background: #30363d; border-radius: 4px; }

    .section-card {
        background: #1c2128;
        border: 1px solid #30363d;
        border-radius: 12px;
        margin-bottom: 10px;
        overflow: hidden;
        transition: border-color 0.2s;
    }

    .section-card.open { border-color: rgba(34,197,94,0.25); }

    .section-trigger {
        width: 100%;
        padding: 14px 16px;
        background: transparent;
        border: none;
        cursor: pointer;
        display: flex;
        align-items: center;
        gap: 12px;
        color: #e6edf3;
        text-align: left;
        transition: background 0.15s;
    }

    .section-trigger:hover { background: #21262d; }

    .step-badge {
        width: 24px;
        height: 24px;
        border-radius: 50%;
        background: rgba(34,197,94,0.12);
        color: #4ade80;
        font-size: 0.7rem;
        font-weight: 700;
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
        border: 1px solid rgba(34,197,94,0.25);
    }

    .step-title {
        font-size: 0.875rem;
        font-weight: 600;
        flex: 1;
    }

    .step-desc {
        font-size: 0.72rem;
        color: #7d8590;
        font-weight: 400;
        margin-top: 1px;
    }

    .chevron {
        color: #7d8590;
        font-size: 0.75rem;
        transition: transform 0.25s ease;
        flex-shrink: 0;
    }

    .section-card.open .chevron { transform: rotate(180deg); }

    .section-body {
        display: none;
        padding: 4px 16px 16px;
        border-top: 1px solid #30363d;
        background: #161b22;
    }

    .section-body.open {
        display: block;
        animation: slideDown 0.2s ease;
    }

    @@keyframes slideDown {
        from { opacity: 0; transform: translateY(-6px); }
        to   { opacity: 1; transform: translateY(0); }
    }

    .theme-grid {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 8px;
        margin-top: 12px;
    }

    .theme-tile {
        border: 1.5px solid #30363d;
        border-radius: 10px;
        padding: 10px;
        cursor: pointer;
        transition: all 0.2s;
        background: #21262d;
        position: relative;
        overflow: hidden;
    }

    .theme-tile:hover {
        border-color: #4ade80;
        transform: translateY(-2px);
        box-shadow: 0 4px 16px rgba(34,197,94,0.12);
    }

    .theme-tile.active {
        border-color: #22c55e;
        background: #1a2e1a;
        box-shadow: 0 0 0 3px rgba(34,197,94,0.15);
    }

    .theme-palette {
        display: flex;
        height: 18px;
        border-radius: 5px;
        overflow: hidden;
        margin-bottom: 8px;
        border: 1px solid rgba(255,255,255,0.07);
    }

    .theme-palette span { flex: 1; }

    .theme-tile-name {
        font-size: 0.75rem;
        font-weight: 600;
        color: #c9d1d9;
        line-height: 1.3;
    }

    .theme-tile-badge {
        font-size: 0.62rem;
        color: #7d8590;
        margin-top: 2px;
    }

    .check-icon {
        position: absolute;
        top: 6px;
        right: 6px;
        color: #22c55e;
        font-size: 0.8rem;
        opacity: 0;
        transition: opacity 0.15s;
    }

    .theme-tile.active .check-icon { opacity: 1; }

    .builder-input {
        width: 100%;
        padding: 9px 12px;
        background: #21262d;
        border: 1px solid #30363d;
        border-radius: 8px;
        color: #e6edf3;
        font-size: 0.8rem;
        font-family: inherit;
        transition: border-color 0.2s;
        outline: none;
        resize: none;
    }

    .builder-input:focus { border-color: #22c55e; }
    .builder-input::placeholder { color: #4d5566; }

    .builder-label {
        font-size: 0.72rem;
        font-weight: 600;
        color: #8b949e;
        text-transform: uppercase;
        letter-spacing: 0.05em;
        margin-bottom: 6px;
        margin-top: 12px;
        display: block;
    }

    .btn-builder-primary {
        display: inline-flex;
        align-items: center;
        gap: 6px;
        padding: 9px 16px;
        background: #22c55e;
        color: #0d1117;
        border: none;
        border-radius: 8px;
        font-size: 0.8rem;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s;
        font-family: inherit;
        width: 100%;
        justify-content: center;
        margin-top: 12px;
    }

    .btn-builder-primary:hover {
        background: #4ade80;
        transform: translateY(-1px);
    }

    .pill-row {
        display: flex;
        flex-wrap: wrap;
        gap: 6px;
        margin-top: 10px;
        margin-bottom: 4px;
    }

    .suggestion-pill {
        padding: 4px 10px;
        background: #21262d;
        border: 1px solid #30363d;
        border-radius: 20px;
        color: #8b949e;
        font-size: 0.72rem;
        cursor: pointer;
        transition: all 0.15s;
        font-family: inherit;
    }

    .suggestion-pill:hover {
        border-color: #4ade80;
        color: #4ade80;
        background: #1a2e1a;
    }

    /* ─── Sortable Lists ─── */
    .sortable-list {
        display: flex;
        flex-direction: column;
        gap: 6px;
        margin-top: 10px;
        margin-bottom: 6px;
    }

    .sortable-item {
        display: flex;
        align-items: center;
        justify-content: space-between;
        background: #21262d;
        border: 1px solid #30363d;
        border-radius: 8px;
        padding: 8px 10px;
        font-size: 0.78rem;
        color: #e6edf3;
        transition: all 0.2s ease;
        user-select: none;
    }

    .sortable-item:hover {
        border-color: #484f58;
        background: #262c36;
    }

    .sortable-item.dragging {
        opacity: 0.35;
        border-color: #22c55e;
        background: #16261c;
    }

    .sortable-item.drag-over {
        border-top: 2px solid #22c55e;
        transform: translateY(-2px);
    }

    .drag-handle {
        cursor: grab;
        color: #7d8590;
        font-size: 1rem;
        margin-right: 8px;
        display: inline-flex;
        align-items: center;
        transition: color 0.15s;
    }

    .drag-handle:hover, .sortable-item:hover .drag-handle {
        color: #4ade80;
    }

    .drag-handle:active {
        cursor: grabbing;
    }

    .item-actions-group {
        display: flex;
        align-items: center;
        gap: 4px;
    }

    .sort-btn {
        background: #161b22;
        border: 1px solid #30363d;
        color: #8b949e;
        border-radius: 4px;
        width: 26px;
        height: 26px;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        font-size: 0.7rem;
        cursor: pointer;
        transition: all 0.15s;
    }

    .sort-btn:hover:not(:disabled) {
        color: #4ade80;
        border-color: #22c55e;
        background: #21262d;
    }

    .sort-btn:disabled {
        opacity: 0.25;
        cursor: not-allowed;
    }

    .delete-item-btn {
        background: #161b22;
        border: 1px solid #30363d;
        color: #f85149;
        border-radius: 4px;
        width: 26px;
        height: 26px;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        font-size: 0.75rem;
        cursor: pointer;
        transition: all 0.15s;
        margin-left: 2px;
    }

    .delete-item-btn:hover {
        background: rgba(248, 81, 73, 0.15);
        border-color: #f85149;
        color: #ff7b72;
    }

    .builder-toast {
        position: fixed;
        bottom: 24px;
        left: 50%;
        transform: translateX(-50%) translateY(60px);
        background: #1c2128;
        border: 1px solid #30363d;
        border-radius: 10px;
        padding: 10px 20px;
        font-size: 0.8rem;
        color: #e6edf3;
        display: flex;
        align-items: center;
        gap: 8px;
        z-index: 9999;
        transition: transform 0.3s cubic-bezier(0.34,1.56,0.64,1);
        box-shadow: 0 8px 24px rgba(0,0,0,0.5);
        max-width: 90vw;
        box-sizing: border-box;
    }

    .builder-toast.show { transform: translateX(-50%) translateY(0); }
    .toast-icon { color: #4ade80; font-size: 1rem; }

    /* RIGHT PANEL */
    .preview-panel {
        flex: 1;
        background: #13161c;
        display: flex;
        flex-direction: column;
        overflow: hidden;
    }

    .preview-toolbar {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 12px 24px;
        background: #161b22;
        border-bottom: 1px solid #30363d;
        flex-shrink: 0;
        gap: 16px;
    }

    .preview-toolbar-left {
        display: flex;
        align-items: center;
        gap: 12px;
    }

    .preview-label {
        font-size: 0.75rem;
        font-weight: 600;
        color: #8b949e;
        text-transform: uppercase;
        letter-spacing: 0.06em;
    }

    .view-toggle {
        display: flex;
        background: #21262d;
        border: 1px solid #30363d;
        border-radius: 8px;
        overflow: hidden;
    }

    .view-btn {
        padding: 6px 14px;
        background: transparent;
        border: none;
        color: #7d8590;
        font-size: 0.8rem;
        cursor: pointer;
        display: flex;
        align-items: center;
        gap: 6px;
        transition: all 0.15s;
        font-family: inherit;
        white-space: nowrap;
    }

    .view-btn.active {
        background: #30363d;
        color: #e6edf3;
    }

    .view-btn:hover:not(.active) { color: #c9d1d9; }

    .preview-actions {
        display: flex;
        align-items: center;
        gap: 8px;
    }

    .btn-preview-open {
        display: inline-flex;
        align-items: center;
        gap: 6px;
        padding: 7px 14px;
        background: #21262d;
        border: 1px solid #30363d;
        border-radius: 8px;
        color: #8b949e;
        font-size: 0.78rem;
        text-decoration: none;
        transition: all 0.15s;
    }

    .btn-preview-open:hover {
        background: #30363d;
        color: #e6edf3;
    }

    .preview-live-badge {
        display: flex;
        align-items: center;
        gap: 6px;
        font-size: 0.72rem;
        color: #4ade80;
        font-weight: 600;
    }

    .live-dot {
        width: 6px;
        height: 6px;
        background: #22c55e;
        border-radius: 50%;
        animation: pulseLive 2s infinite;
    }

    @@keyframes pulseLive {
        0%, 100% { opacity: 1; box-shadow: 0 0 0 0 rgba(34,197,94,0.4); }
        50% { box-shadow: 0 0 0 5px rgba(34,197,94,0); }
    }

    .preview-stage {
        flex: 1;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 28px;
        overflow: hidden;
        position: relative;
    }

    .preview-stage::before {
        content: '';
        position: absolute;
        inset: 0;
        background:
            radial-gradient(ellipse at 30% 20%, rgba(34,197,94,0.04) 0%, transparent 50%),
            radial-gradient(ellipse at 80% 80%, rgba(59,130,246,0.03) 0%, transparent 50%);
        pointer-events: none;
    }

    .desktop-frame {
        width: 100%;
        max-width: 860px;
        height: 100%;
        max-height: 640px;
        background: #0d1117;
        border-radius: 12px;
        border: 1px solid #30363d;
        overflow: hidden;
        box-shadow: 0 20px 60px rgba(0,0,0,0.5);
        position: relative;
        display: flex;
        flex-direction: column;
    }

    .desktop-chrome {
        background: #21262d;
        border-bottom: 1px solid #30363d;
        padding: 10px 14px;
        display: flex;
        align-items: center;
        gap: 10px;
        flex-shrink: 0;
    }

    .chrome-dots {
        display: flex;
        gap: 5px;
    }

    .chrome-dots span {
        width: 10px;
        height: 10px;
        border-radius: 50%;
    }

    .chrome-dots span:nth-child(1) { background: #f85149; }
    .chrome-dots span:nth-child(2) { background: #e3b341; }
    .chrome-dots span:nth-child(3) { background: #3fb950; }

    .chrome-bar {
        flex: 1;
        background: #30363d;
        border-radius: 6px;
        padding: 4px 10px;
        font-size: 0.7rem;
        color: #7d8590;
        display: flex;
        align-items: center;
        gap: 6px;
    }

    .desktop-frame iframe {
        flex: 1;
        width: 100%;
        border: none;
        background: #fff;
    }

    .mobile-frame-wrap {
        display: flex;
        align-items: center;
        justify-content: center;
        height: 100%;
    }

    .phone-outer {
        width: 290px;
        background: #1c2128;
        border-radius: 44px;
        padding: 14px;
        box-shadow:
            0 0 0 1px #30363d,
            0 20px 60px rgba(0,0,0,0.6),
            inset 0 1px 0 rgba(255,255,255,0.05);
        position: relative;
    }

    .phone-inner {
        background: #0d1117;
        border-radius: 32px;
        overflow: hidden;
        position: relative;
    }

    .phone-notch-bar {
        position: absolute;
        top: 0;
        left: 50%;
        transform: translateX(-50%);
        width: 110px;
        height: 28px;
        background: #1c2128;
        border-radius: 0 0 18px 18px;
        z-index: 10;
    }

    .phone-inner iframe {
        width: 100%;
        height: 560px;
        border: none;
        display: block;
    }

    .phone-side-btn {
        position: absolute;
        right: -3px;
        top: 100px;
        width: 3px;
        height: 50px;
        background: #30363d;
        border-radius: 0 3px 3px 0;
    }

    .preview-loading {
        position: absolute;
        inset: 0;
        background: rgba(13,17,23,0.6);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 5;
        opacity: 0;
        pointer-events: none;
        transition: opacity 0.2s;
        border-radius: inherit;
    }

    .preview-loading.visible { opacity: 1; pointer-events: auto; }

    .spinner {
        width: 28px;
        height: 28px;
        border: 2px solid #30363d;
        border-top-color: #22c55e;
        border-radius: 50%;
        animation: spin 0.6s linear infinite;
    }

    @@keyframes spin { to { transform: rotate(360deg); } }

    select.builder-input {
        appearance: none;
        background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='%237d8590' stroke-width='2'%3E%3Cpath d='m6 9 6 6 6-6'/%3E%3C/svg%3E");
        background-repeat: no-repeat;
        background-position: right 12px center;
        padding-right: 32px;
    }

    select.builder-input option {
        background: #21262d;
        color: #e6edf3;
    }

    @@media (max-width: 992px) {
        .builder-shell {
            flex-direction: column;
            height: auto;
            overflow-y: visible;
        }
        .builder-panel {
            width: 100%;
            min-width: 100%;
            height: auto;
            border-right: none;
            border-bottom: 1px solid #30363d;
        }
        .preview-panel {
            min-height: 700px;
        }
        .preview-toolbar {
            flex-wrap: wrap;
            gap: 10px;
            padding: 10px 16px;
        }
        .preview-stage {
            padding: 16px 8px;
        }
    }

    @@media (max-width: 480px) {
        .theme-grid {
            grid-template-columns: 1fr;
        }
        .phone-outer {
            width: 100%;
            max-width: 320px;
            padding: 10px;
        }
    }
</style>
}

<div class="builder-shell">

    <!-- LEFT PANEL -->
    <div class="builder-panel">
        <div class="panel-header">
            <h2>&#10024; Menu Olusturucu</h2>
            <p>Degisiklikler sagda aninda yansiyor</p>
        </div>

        <div class="panel-body">

            <!-- 1. Tema -->
            <div class="section-card open" id="sec-theme">
                <button class="section-trigger" onclick="toggleSection('sec-theme')">
                    <span class="step-badge">1</span>
                    <span>
                        <div class="step-title">Sablon Secimi</div>
                        <div class="step-desc">Gorunum ve renk paleti</div>
                    </span>
                    <span class="chevron">&#9660;</span>
                </button>
                <div class="section-body open" id="body-theme">
                    <div class="theme-grid">
                        @foreach (var t in themes)
                        {
                            var isSelected = t.Id == currentThemeId;
                            var layoutLabel = t.Layout == LayoutType.List ? "Liste" :
                                             t.Layout == LayoutType.Grid ? "Grid" : "Gorselli";
                            <div class="theme-tile @(isSelected ? "active" : "")"
                                 id="theme-@t.Id"
                                 onclick="selectTheme(@t.Id)">
                                <span class="check-icon">&#10003;</span>
                                <div class="theme-palette">
                                    <span style="background:@t.BackgroundColor"></span>
                                    <span style="background:@t.PrimaryColor"></span>
                                    <span style="background:@t.SecondaryColor"></span>
                                </div>
                                <div class="theme-tile-name">@t.Name</div>
                                <div class="theme-tile-badge">@layoutLabel</div>
                            </div>
                        }
                    </div>
                </div>
            </div>

            <!-- 2. Kategoriler -->
            <div class="section-card" id="sec-cat">
                <button class="section-trigger" onclick="toggleSection('sec-cat')">
                    <span class="step-badge">2</span>
                    <span>
                        <div class="step-title">Kategori Ekle &amp; Sırala</div>
                        <div class="step-desc">Kategorileri düzenleyin ve sürükleyerek sıralayın</div>
                    </span>
                    <span class="chevron">&#9660;</span>
                </button>
                <div class="section-body" id="body-cat">
                    @if (suggestions != null && suggestions.Any())
                    {
                        <label class="builder-label" style="margin-top:12px;">&#128161; Öneriler</label>
                        <div class="pill-row" id="suggestion-pills">
                            @foreach (var sug in suggestions)
                            {
                                <button class="suggestion-pill" onclick="quickAddCategory('@sug')">+ @sug</button>
                            }
                        </div>
                    }
                    else
                    {
                        <div id="suggestion-pills" style="margin-top:12px;"></div>
                    }

                    <label class="builder-label">Yeni Kategori Adı</label>
                    <div style="display:flex;gap:6px;">
                        <input type="text" id="categoryName" class="builder-input"
                               placeholder="Örnek: Başlangıçlar, Tatlılar..."
                               onkeydown="if(event.key==='Enter'){event.preventDefault();addCategoryFromInput()}" />
                        <button onclick="addCategoryFromInput()"
                                style="padding:9px 14px;background:#22c55e;border:none;border-radius:8px;color:#0d1117;font-weight:700;cursor:pointer;font-size:1rem;flex-shrink:0;transition:background 0.15s;"
                                onmouseenter="this.style.background='#4ade80'"
                                onmouseleave="this.style.background='#22c55e'">+</button>
                    </div>

                    <label class="builder-label" style="margin-top:16px;display:flex;justify-content:space-between;align-items:center;">
                        <span>Mevcut Kategoriler</span>
                        <span style="font-size:0.65rem;color:#7d8590;text-transform:none;">Sürükle-bırak veya oklarla sıralayın</span>
                    </label>
                    <div class="sortable-list" id="category-sortable-list">
                        <!-- Javascript ile doldurulacak -->
                    </div>
                </div>
            </div>

            <!-- 3. Urun -->
            <div class="section-card" id="sec-item">
                <button class="section-trigger" onclick="toggleSection('sec-item')">
                    <span class="step-badge">3</span>
                    <span>
                        <div class="step-title">Ürün Ekle &amp; Sırala</div>
                        <div class="step-desc">Yemek, içecek ekleyin ve sıralayın</div>
                    </span>
                    <span class="chevron">&#9660;</span>
                </button>
                <div class="section-body" id="body-item">
                    <label class="builder-label">Ürün Adı</label>
                    <input type="text" id="itemName" class="builder-input"
                           placeholder="Örnek: Urfa Kebabı, Fırın Sütlaç" />

                    <label class="builder-label">Kategori</label>
                    <select id="itemCategory" class="builder-input" onchange="syncBuilderItemFilter(this.value)">
                        <option value="">Kategori seçin...</option>
                        @foreach (var cat in categories)
                        {
                            <option value="@cat.Id">@cat.Name</option>
                        }
                    </select>

                    <label class="builder-label">Açıklama</label>
                    <textarea id="itemDescription" class="builder-input" rows="2"
                              placeholder="Örnek: 200gr kuzu eti, közlenmiş biber eşliğinde..."></textarea>

                    <label class="builder-label">Fiyat (TL)</label>
                    <input type="number" id="itemPrice" class="builder-input"
                           step="0.01" min="0" placeholder="0.00" />

                    <button class="btn-builder-primary" onclick="submitMenuItemFn()">
                        &#10003; Menüye Ekle
                    </button>

                    <div style="margin-top:20px;border-top:1px solid #30363d;padding-top:12px;">
                        <label class="builder-label" style="display:flex;justify-content:space-between;align-items:center;margin-top:0;">
                            <span>Ürünleri Sırala &amp; Yönet</span>
                            <span style="font-size:0.65rem;color:#7d8590;text-transform:none;">Kategoriye göre listele &amp; sırala</span>
                        </label>
                        
                        <select id="builderItemFilterSelect" class="builder-input" onchange="renderBuilderItemsList(this.value)" style="margin-top:6px;margin-bottom:8px;">
                            @foreach (var cat in categories)
                            {
                                <option value="@cat.Id">@cat.Name (@menuItems.Count(mi => mi.CategoryId == cat.Id) Ürün)</option>
                            }
                        </select>

                        <div class="sortable-list" id="item-sortable-list">
                            <!-- Javascript ile kategori bazlı doldurulacak -->
                        </div>
                    </div>
                </div>
            </div>

            <!-- 4. Harita & Konum & Instagram Ayarları -->
            <div class="section-card" id="sec-location">
                <button class="section-trigger" onclick="toggleSection('sec-location')">
                    <span class="step-badge">4</span>
                    <span>
                        <div class="step-title">📍 Harita, Konum &amp; Sosyal Medya</div>
                        <div class="step-desc">Google Haritalar, Instagram ve iletişim</div>
                    </span>
                    <span class="chevron">&#9660;</span>
                </button>
                <div class="section-body" id="body-location">
                    <label class="builder-label">Google Haritalar Bağlantısı (URL veya Embed Kodu)</label>
                    <input type="text" id="googleMapsUrl" class="builder-input" value="@restaurant?.GoogleMapsUrl"
                           placeholder="https://maps.google.com/?q=... veya <iframe src='...'></iframe>" />

                    <label class="builder-label">📸 Instagram Profili veya Bağlantısı</label>
                    <div style="display:flex;gap:6px;">
                        <input type="text" id="restaurantInstagram" class="builder-input" value="@restaurant?.InstagramUrl"
                               placeholder="Örn: @@kocaoglurestoran veya https://instagram.com/kocaoglurestoran" />
                        @if (!string.IsNullOrWhiteSpace(restaurant?.InstagramUrl))
                        {
                            var igTestUrl = restaurant.InstagramUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                                ? restaurant.InstagramUrl
                                : $"https://instagram.com/{restaurant.InstagramUrl.TrimStart('@')}";
                            <a href="@igTestUrl" target="_blank" rel="noopener noreferrer" 
                               style="padding:8px 12px;background:linear-gradient(45deg,#f09433,#e6683c,#dc2743,#cc2366,#bc1888);color:white;border-radius:8px;text-decoration:none;display:inline-flex;align-items:center;justify-content:center;font-size:0.8rem;flex-shrink:0;" title="Instagram'da Gör">
                                <i class="fa-brands fa-instagram"></i>
                            </a>
                        }
                    </div>
                    <p style="font-size:0.7rem;color:#7d8590;margin-top:3px;margin-bottom:8px;">💡 <em>Menünüzün sağ altında ziyaretçilerin tıklayabileceği Instagram butonu açılır.</em></p>

                    <label class="builder-label">Adres</label>
                    <input type="text" id="restaurantAddress" class="builder-input" value="@restaurant?.Address"
                           placeholder="Örn: Atatürk Caddesi No:123, Hatay" />

                    <label class="builder-label">Telefon</label>
                    <input type="text" id="restaurantPhone" class="builder-input" value="@restaurant?.Phone"
                           placeholder="Örn: +90 555 000 00 00" />

                    <label class="builder-label">Çalışma Saatleri</label>
                    <input type="text" id="restaurantWorkingHours" class="builder-input" value="@restaurant?.WorkingHours"
                           placeholder="Örn: Hafta içi: 09:00 - 23:00, Hafta sonu: 10:00 - 00:00" />

                    <button class="btn-builder-primary" onclick="submitLocationSettings()">
                        💾 Harita, İletişim &amp; Instagram Bilgilerini Kaydet
                    </button>
                </div>
            </div>

            <!-- 5. Önemli Bilgilendirme & Duyuru Notu -->
            <div class="section-card" id="sec-notice">
                <button class="section-trigger" onclick="toggleSection('sec-notice')">
                    <span class="step-badge">5</span>
                    <span>
                        <div class="step-title">📢 Duyuru &amp; Bilgilendirme Notu</div>
                        <div class="step-desc">Kuver, KDV veya servis bilgisi (İsteğe bağlı)</div>
                    </span>
                    <span class="chevron">&#9660;</span>
                </button>
                <div class="section-body" id="body-notice">
                    <label class="builder-label">Menü Üstü Bilgilendirme Notu</label>
                    <textarea id="restaurantNotice" class="builder-input" rows="3"
                              placeholder="Örn: Menü fiyatlarımıza KDV dahildir. Masanıza kişi başı 100 TL servis ücreti (kuver) yansıtılacaktır."
                              style="resize: vertical; min-height: 80px; font-family: inherit;">@restaurant?.ImportantNotice</textarea>
                    <p style="font-size: 0.72rem; color: #7d8590; margin-top: 4px; margin-bottom: 12px;">
                        💡 <em>Boş bırakırsanız menünüzde bilgilendirme kutusu gösterilmez.</em>
                    </p>

                    <button class="btn-builder-primary" onclick="submitNoticeSettings()">
                        💾 Bilgilendirme Notunu Kaydet
                    </button>
                </div>
            </div>

        </div>
    </div>

    <!-- RIGHT PANEL - PREVIEW -->
    <div class="preview-panel">
        <div class="preview-toolbar">
            <div class="preview-toolbar-left">
                <span class="preview-label">Onizleme</span>
                <div class="view-toggle">
                    <button class="view-btn active" id="btn-desktop" onclick="switchView('desktop')">
                        &#128421; Masaustu
                    </button>
                    <button class="view-btn" id="btn-mobile" onclick="switchView('mobile')">
                        &#128241; Mobil
                    </button>
                </div>
            </div>

            <div class="preview-live-badge">
                <span class="live-dot"></span>
                Canli
            </div>

            <div class="preview-actions">
                <a href="/Home/Menu/@restaurant?.Id" target="_blank" class="btn-preview-open">
                    &#8599; Yeni Sekmede Ac
                </a>
            </div>
        </div>

        <div class="preview-stage" id="preview-stage">

            <!-- Desktop View -->
            <div class="desktop-frame" id="view-desktop">
                <div class="desktop-chrome">
                    <div class="chrome-dots">
                        <span></span><span></span><span></span>
                    </div>
                    <div class="chrome-bar">
                        &#128274; localhost:5218/Home/Menu/@restaurant?.Id
                    </div>
                </div>
                <div class="preview-loading" id="loading-desktop">
                    <div class="spinner"></div>
                </div>
                <iframe id="previewIframe-desktop"
                        src="/Home/Menu/@restaurant?.Id?previewThemeId=@currentThemeId"
                        onload="hideLoading('desktop')">
                </iframe>
            </div>

            <!-- Mobile View -->
            <div class="mobile-frame-wrap" id="view-mobile" style="display:none;">
                <div class="phone-outer">
                    <div class="phone-inner">
                        <div class="phone-notch-bar"></div>
                        <div class="preview-loading" id="loading-mobile">
                            <div class="spinner"></div>
                        </div>
                        <iframe id="previewIframe-mobile"
                                src="/Home/Menu/@restaurant?.Id?previewThemeId=@currentThemeId"
                                onload="hideLoading('mobile')">
                        </iframe>
                    </div>
                    <div class="phone-side-btn"></div>
                </div>
            </div>

        </div>
    </div>
</div>

<!-- Toast -->
<div class="builder-toast" id="toast">
    <span class="toast-icon">&#10003;</span>
    <span id="toast-msg">Islem basarili!</span>
</div>

@Html.AntiForgeryToken()

@section Scripts {
<script>
    const restaurantId  = '@restaurant?.Id';
    let currentThemeId  = '@currentThemeId';
    let currentViewMode = 'desktop';

    // Modelden gelen kategoriler ve menü öğeleri
    let builderCategories = @Html.Raw(System.Text.Json.JsonSerializer.Serialize(categories.Select(c => new { id = c.Id, name = c.Name, displayOrder = c.DisplayOrder })));
    let builderMenuItems = @Html.Raw(System.Text.Json.JsonSerializer.Serialize(menuItems.Select(mi => new { id = mi.Id, name = mi.Name, description = mi.Description, price = mi.Price, categoryId = mi.CategoryId, displayOrder = mi.DisplayOrder })));

    function postForm(url, params) {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        const body = new URLSearchParams(params);
        if (token) body.append('__RequestVerificationToken', token);
        return fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body
        });
    }

    function postJson(url, data) {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        const headers = { 'Content-Type': 'application/json' };
        if (token) headers['RequestVerificationToken'] = token;
        return fetch(url, {
            method: 'POST',
            headers,
            body: JSON.stringify(data)
        });
    }

    function toggleSection(id) {
        const card = document.getElementById(id);
        const bodyId = 'body-' + id.replace('sec-', '');
        const body = document.getElementById(bodyId);
        const isOpen = card.classList.contains('open');
        card.classList.toggle('open', !isOpen);
        body.classList.toggle('open', !isOpen);
    }

    function switchView(mode) {
        currentViewMode = mode;
        document.getElementById('view-desktop').style.display = mode === 'desktop' ? 'flex' : 'none';
        document.getElementById('view-mobile').style.display  = mode === 'mobile'  ? 'flex' : 'none';
        document.getElementById('btn-desktop').classList.toggle('active', mode === 'desktop');
        document.getElementById('btn-mobile').classList.toggle('active',  mode === 'mobile');
    }

    function showLoading() {
        document.getElementById('loading-desktop').classList.add('visible');
        document.getElementById('loading-mobile').classList.add('visible');
    }

    function hideLoading(which) {
        document.getElementById('loading-' + which).classList.remove('visible');
    }

    function showToast(msg) {
        const t = document.getElementById('toast');
        document.getElementById('toast-msg').textContent = msg;
        t.classList.add('show');
        setTimeout(() => t.classList.remove('show'), 2800);
    }

    function refreshPreview() {
        const ts = new Date().getTime();
        const url = `/Home/Menu/${restaurantId}?previewThemeId=${currentThemeId}&t=${ts}`;
        showLoading();
        document.getElementById('previewIframe-desktop').src = url;
        document.getElementById('previewIframe-mobile').src  = url;
    }

    function selectTheme(themeId) {
        document.querySelectorAll('.theme-tile').forEach(t => t.classList.remove('active'));
        document.getElementById('theme-' + themeId).classList.add('active');
        currentThemeId = themeId;

        postForm('/Restaurant/Builder/SelectTheme', { themeId })
        .then(r => r.json())
        .then(d => {
            if (d.success) { refreshPreview(); showToast('Tema güncellendi!'); }
            else alert(d.message || 'Tema seçilirken hata oluştu.');
        })
        .catch(e => console.error(e));
    }

    // ─── KATEGORİ YÖNETİMİ & SIRALAMA ───
    function renderCategorySortableList() {
        const container = document.getElementById('category-sortable-list');
        if (!container) return;

        if (builderCategories.length === 0) {
            container.innerHTML = '<div style="font-size:0.75rem;color:#7d8590;text-align:center;padding:12px;border:1px dashed #30363d;border-radius:8px;">Henüz kategori eklenmedi.</div>';
            return;
        }

        container.innerHTML = '';
        builderCategories.forEach((cat, index) => {
            const catItemCount = builderMenuItems.filter(mi => mi.categoryId === cat.id).length;
            const itemDiv = document.createElement('div');
            itemDiv.className = 'sortable-item';
            itemDiv.draggable = true;
            itemDiv.dataset.id = cat.id;
            itemDiv.dataset.index = index;

            itemDiv.innerHTML = `
                <div style="display:flex;align-items:center;min-width:0;flex:1;">
                    <span class="drag-handle" title="Sıralamak için sürükleyin">⠿</span>
                    <span style="font-weight:600;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;color:#e6edf3;flex:1;min-width:0;">${escapeHtml(cat.name)}</span>
                    <span style="font-size:0.65rem;background:#161b22;color:#8b949e;padding:2px 6px;border-radius:10px;margin-left:6px;border:1px solid #30363d;flex-shrink:0;">
                        ${catItemCount} Ürün
                    </span>
                </div>
                <div class="item-actions-group">
                    <button type="button" class="sort-btn" onclick="moveCategory(${index}, -1)" title="Yukarı Taşı" ${index === 0 ? 'disabled' : ''}>▲</button>
                    <button type="button" class="sort-btn" onclick="moveCategory(${index}, 1)" title="Aşağı Taşı" ${index === builderCategories.length - 1 ? 'disabled' : ''}>▼</button>
                    <button type="button" class="delete-item-btn" onclick="deleteCategoryFn(${cat.id}, '${escapeHtml(cat.name)}')" title="Kategoriyi Sil">✕</button>
                </div>
            `;
            container.appendChild(itemDiv);
        });

        initCategoryDragAndDrop();
        updateCategoryDropdowns();
    }

    function initCategoryDragAndDrop() {
        const list = document.getElementById('category-sortable-list');
        const items = list.querySelectorAll('.sortable-item');
        let dragSrcIndex = null;

        items.forEach(item => {
            item.addEventListener('dragstart', function(e) {
                dragSrcIndex = parseInt(this.dataset.index);
                this.classList.add('dragging');
                e.dataTransfer.effectAllowed = 'move';
                e.dataTransfer.setData('text/plain', dragSrcIndex);
            });

            item.addEventListener('dragover', function(e) {
                e.preventDefault();
                e.dataTransfer.dropEffect = 'move';
                this.classList.add('drag-over');
            });

            item.addEventListener('dragleave', function() {
                this.classList.remove('drag-over');
            });

            item.addEventListener('drop', function(e) {
                e.preventDefault();
                this.classList.remove('drag-over');
                const targetIndex = parseInt(this.dataset.index);
                if (dragSrcIndex !== null && dragSrcIndex !== targetIndex) {
                    const movedItem = builderCategories.splice(dragSrcIndex, 1)[0];
                    builderCategories.splice(targetIndex, 0, movedItem);
                    renderCategorySortableList();
                    saveCategoryOrder();
                }
            });

            item.addEventListener('dragend', function() {
                this.classList.remove('dragging');
                items.forEach(i => i.classList.remove('drag-over'));
            });
        });
    }

    function moveCategory(index, direction) {
        const targetIndex = index + direction;
        if (targetIndex < 0 || targetIndex >= builderCategories.length) return;
        const temp = builderCategories[index];
        builderCategories[index] = builderCategories[targetIndex];
        builderCategories[targetIndex] = temp;
        renderCategorySortableList();
        saveCategoryOrder();
    }

    function saveCategoryOrder() {
        const categoryIds = builderCategories.map(c => c.id);
        postJson('/Restaurant/Builder/UpdateCategoryOrder', categoryIds)
        .then(r => r.json())
        .then(d => {
            if (d.success) {
                refreshPreview();
                showToast('🔄 Kategori sırası güncellendi!');
            }
        })
        .catch(e => console.error(e));
    }

    function deleteCategoryFn(id, name) {
        if (!confirm(`"${name}" kategorisini ve bu kategoriye ait tüm ürünleri silmek istediğinizden emin misiniz?`)) return;

        postForm('/Restaurant/Builder/DeleteCategory', { id })
        .then(r => r.json())
        .then(d => {
            if (d.success) {
                builderCategories = builderCategories.filter(c => c.id !== id);
                builderMenuItems = builderMenuItems.filter(mi => mi.categoryId !== id);
                renderCategorySortableList();
                renderBuilderItemsList();
                refreshPreview();
                showToast(`"${name}" kategorisi silindi.`);
            } else {
                alert(d.message || 'Silme işleminde hata oluştu.');
            }
        })
        .catch(e => console.error(e));
    }

    function updateCategoryDropdowns() {
        const itemCatSelect = document.getElementById('itemCategory');
        const filterSelect = document.getElementById('builderItemFilterSelect');

        if (itemCatSelect) {
            const currentVal = itemCatSelect.value;
            itemCatSelect.innerHTML = '<option value="">Kategori seçin...</option>';
            builderCategories.forEach(c => {
                const opt = document.createElement('option');
                opt.value = c.id;
                opt.textContent = c.name;
                if (c.id == currentVal) opt.selected = true;
                itemCatSelect.appendChild(opt);
            });
        }

        if (filterSelect) {
            const currentFilterVal = filterSelect.value;
            filterSelect.innerHTML = '';
            builderCategories.forEach(c => {
                const count = builderMenuItems.filter(mi => mi.categoryId === c.id).length;
                const opt = document.createElement('option');
                opt.value = c.id;
                opt.textContent = `${c.name} (${count} Ürün)`;
                if (c.id == currentFilterVal) opt.selected = true;
                filterSelect.appendChild(opt);
            });
        }
    }

    function addCategoryFromInput() {
        const input = document.getElementById('categoryName');
        const name  = input.value.trim();
        if (!name) return;
        addCategory(name);
        input.value = '';
    }

    function quickAddCategory(name) { addCategory(name); }

    function addCategory(name) {
        postForm('/Restaurant/Builder/AddCategory', { name })
        .then(r => r.json())
        .then(d => {
            if (d.success) {
                builderCategories.push({ id: d.category.id, name: d.category.name, displayOrder: builderCategories.length });
                renderCategorySortableList();
                renderBuilderItemsList(d.category.id);
                updateSuggestions(d.suggestions);
                refreshPreview();
                showToast('"' + name + '" kategorisi eklendi!');
                const sec = document.getElementById('sec-item');
                const body = document.getElementById('body-item');
                if (!sec.classList.contains('open')) {
                    sec.classList.add('open');
                    body.classList.add('open');
                }
            } else {
                alert(d.message || 'Kategori eklenirken hata oluştu.');
            }
        })
        .catch(e => console.error(e));
    }

    function updateSuggestions(suggestions) {
        const container = document.getElementById('suggestion-pills');
        if (!container) return;
        container.innerHTML = '';
        if (suggestions && suggestions.length > 0) {
            suggestions.forEach(sug => {
                const btn = document.createElement('button');
                btn.className = 'suggestion-pill';
                btn.textContent = '+ ' + sug;
                btn.onclick = () => quickAddCategory(sug);
                container.appendChild(btn);
            });
        }
    }

    // ─── ÜRÜN YÖNETİMİ & SIRALAMA ───
    function syncBuilderItemFilter(catId) {
        const filterSelect = document.getElementById('builderItemFilterSelect');
        if (filterSelect && catId) {
            filterSelect.value = catId;
            renderBuilderItemsList(catId);
        }
    }

    function renderBuilderItemsList(selectedCatId) {
        const filterSelect = document.getElementById('builderItemFilterSelect');
        const container = document.getElementById('item-sortable-list');
        if (!container) return;

        let catId = selectedCatId;
        if (!catId && filterSelect) {
            catId = filterSelect.value;
        }
        if (!catId && builderCategories.length > 0) {
            catId = builderCategories[0].id;
            if (filterSelect) filterSelect.value = catId;
        }

        if (!catId) {
            container.innerHTML = '<div style="font-size:0.75rem;color:#7d8590;text-align:center;padding:12px;border:1px dashed #30363d;border-radius:8px;">Önce bir kategori ekleyin.</div>';
            return;
        }

        const catItems = builderMenuItems.filter(mi => mi.categoryId == catId);

        if (catItems.length === 0) {
            container.innerHTML = '<div style="font-size:0.75rem;color:#7d8590;text-align:center;padding:12px;border:1px dashed #30363d;border-radius:8px;">Bu kategoride henüz ürün bulunmuyor.</div>';
            return;
        }

        container.innerHTML = '';
        catItems.forEach((item, index) => {
            const itemDiv = document.createElement('div');
            itemDiv.className = 'sortable-item';
            itemDiv.draggable = true;
            itemDiv.dataset.id = item.id;
            itemDiv.dataset.index = index;
            itemDiv.dataset.catId = catId;

            itemDiv.innerHTML = `
                <div style="display:flex;align-items:center;min-width:0;flex:1;">
                    <span class="drag-handle" title="Sıralamak için sürükleyin">⠿</span>
                    <span style="font-weight:600;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;color:#e6edf3;flex:1;min-width:0;">${escapeHtml(item.name)}</span>
                    <span style="font-size:0.68rem;color:#4ade80;font-weight:700;margin-left:6px;background:#16261c;padding:2px 6px;border-radius:6px;border:1px solid #22c55e44;flex-shrink:0;">
                        ${parseFloat(item.price).toFixed(2)} TL
                    </span>
                </div>
                <div class="item-actions-group">
                    <button type="button" class="sort-btn" onclick="moveItem(${index}, -1)" title="Yukarı Taşı" ${index === 0 ? 'disabled' : ''}>▲</button>
                    <button type="button" class="sort-btn" onclick="moveItem(${index}, 1)" title="Aşağı Taşı" ${index === catItems.length - 1 ? 'disabled' : ''}>▼</button>
                    <button type="button" class="delete-item-btn" onclick="deleteMenuItemFn(${item.id}, '${escapeHtml(item.name)}')" title="Ürünü Sil">✕</button>
                </div>
            `;
            container.appendChild(itemDiv);
        });

        initItemDragAndDrop(catId);
    }

    function initItemDragAndDrop(catId) {
        const list = document.getElementById('item-sortable-list');
        const items = list.querySelectorAll('.sortable-item');
        let dragSrcIndex = null;

        items.forEach(item => {
            item.addEventListener('dragstart', function(e) {
                dragSrcIndex = parseInt(this.dataset.index);
                this.classList.add('dragging');
                e.dataTransfer.effectAllowed = 'move';
                e.dataTransfer.setData('text/plain', dragSrcIndex);
            });

            item.addEventListener('dragover', function(e) {
                e.preventDefault();
                e.dataTransfer.dropEffect = 'move';
                this.classList.add('drag-over');
            });

            item.addEventListener('dragleave', function() {
                this.classList.remove('drag-over');
            });

            item.addEventListener('drop', function(e) {
                e.preventDefault();
                this.classList.remove('drag-over');
                const targetIndex = parseInt(this.dataset.index);
                if (dragSrcIndex !== null && dragSrcIndex !== targetIndex) {
                    const catItems = builderMenuItems.filter(mi => mi.categoryId == catId);
                    const moved = catItems.splice(dragSrcIndex, 1)[0];
                    catItems.splice(targetIndex, 0, moved);

                    // builderMenuItems listesini güncelle
                    builderMenuItems = builderMenuItems.filter(mi => mi.categoryId != catId).concat(catItems);

                    renderBuilderItemsList(catId);
                    saveMenuItemOrder(catId);
                }
            });

            item.addEventListener('dragend', function() {
                this.classList.remove('dragging');
                items.forEach(i => i.classList.remove('drag-over'));
            });
        });
    }

    function moveItem(index, direction) {
        const filterSelect = document.getElementById('builderItemFilterSelect');
        const catId = filterSelect ? filterSelect.value : null;
        if (!catId) return;

        const catItems = builderMenuItems.filter(mi => mi.categoryId == catId);
        const targetIndex = index + direction;
        if (targetIndex < 0 || targetIndex >= catItems.length) return;

        const temp = catItems[index];
        catItems[index] = catItems[targetIndex];
        catItems[targetIndex] = temp;

        builderMenuItems = builderMenuItems.filter(mi => mi.categoryId != catId).concat(catItems);

        renderBuilderItemsList(catId);
        saveMenuItemOrder(catId);
    }

    function saveMenuItemOrder(catId) {
        const catItems = builderMenuItems.filter(mi => mi.categoryId == catId);
        const itemIds = catItems.map(i => i.id);

        postJson('/Restaurant/Builder/UpdateMenuItemOrder', itemIds)
        .then(r => r.json())
        .then(d => {
            if (d.success) {
                refreshPreview();
                showToast('🔄 Ürün sırası güncellendi!');
            }
        })
        .catch(e => console.error(e));
    }

    function deleteMenuItemFn(id, name) {
        if (!confirm(`"${name}" ürününü silmek istediğinizden emin misiniz?`)) return;

        postForm('/Restaurant/Builder/DeleteMenuItem', { id })
        .then(r => r.json())
        .then(d => {
            if (d.success) {
                builderMenuItems = builderMenuItems.filter(mi => mi.id !== id);
                renderCategorySortableList();
                renderBuilderItemsList();
                refreshPreview();
                showToast(`"${name}" ürünü silindi.`);
            } else {
                alert(d.message || 'Silme işleminde hata oluştu.');
            }
        })
        .catch(e => console.error(e));
    }

    function submitMenuItemFn() {
        const name        = document.getElementById('itemName').value.trim();
        const description = document.getElementById('itemDescription').value.trim();
        const price       = parseFloat(document.getElementById('itemPrice').value);
        const categoryId  = parseInt(document.getElementById('itemCategory').value);

        if (!name || isNaN(price) || isNaN(categoryId)) {
            showToast('Lütfen tüm zorunlu alanları doldurun.');
            return;
        }

        postForm('/Restaurant/Builder/AddMenuItem', { name, description, price, categoryId })
        .then(r => r.json())
        .then(d => {
            if (d.success) {
                document.getElementById('itemName').value        = '';
                document.getElementById('itemDescription').value = '';
                document.getElementById('itemPrice').value       = '';

                builderMenuItems.push({
                    id: d.item.id,
                    name: d.item.name,
                    description: d.item.description,
                    price: d.item.price,
                    categoryId: d.item.categoryId,
                    displayOrder: d.item.displayOrder
                });

                renderCategorySortableList();
                renderBuilderItemsList(categoryId);
                refreshPreview();
                showToast('"' + name + '" menüye eklendi!');
            } else {
                alert(d.message || 'Ürün eklenirken hata oluştu.');
            }
        })
        .catch(e => console.error(e));
    }

    // ─── HARİTA, KONUM & INSTAGRAM ───
    function submitLocationSettings() {
        const googleMapsUrl = document.getElementById('googleMapsUrl').value.trim();
        const instagramUrl = document.getElementById('restaurantInstagram').value.trim();
        const address = document.getElementById('restaurantAddress').value.trim();
        const phone = document.getElementById('restaurantPhone').value.trim();
        const workingHours = document.getElementById('restaurantWorkingHours').value.trim();

        postForm('/Restaurant/Builder/UpdateLocation', { googleMapsUrl, address, phone, workingHours, instagramUrl })
        .then(r => r.json())
        .then(d => {
            if (d.success) {
                refreshPreview();
                showToast('📍 Harita, iletişim ve Instagram bilgileri kaydedildi!');
            } else {
                alert(d.message || 'Hata oluştu.');
            }
        })
        .catch(e => console.error(e));
    }

    function submitNoticeSettings() {
        const notice = document.getElementById('restaurantNotice').value.trim();

        postForm('/Restaurant/Builder/UpdateNotice', { notice })
        .then(r => r.json())
        .then(d => {
            if (d.success) {
                refreshPreview();
                showToast('📢 Bilgilendirme notu kaydedildi!');
            } else {
                alert(d.message || 'Hata oluştu.');
            }
        })
        .catch(e => console.error(e));
    }

    function escapeHtml(str) {
        if (!str) return '';
        return str.replace(/&/g, "&amp;")
                  .replace(/</g, "&lt;")
                  .replace(/>/g, "&gt;")
                  .replace(/"/g, "&quot;")
                  .replace(/'/g, "&#039;");
    }

    // Sayfa yüklendiğinde listeleri render et
    document.addEventListener('DOMContentLoaded', () => {
        renderCategorySortableList();
        renderBuilderItemsList();
    });
</script>
}

`

--------------------------------------------------------------------------------

## Category/Create.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/Category/Create.cshtml

`cshtml
@{
    ViewData["Title"] = "Yeni Kategori";
}
<div class="card" style="max-width:560px;">
    <div class="card-header"><h5>🏷️ Yeni Kategori Ekle</h5></div>
    <div class="card-body">
        @if (ViewBag.Error != null)
        {
            <div class="alert-error">⚠️ @ViewBag.Error</div>
        }
        <form method="post" asp-action="Create" enctype="multipart/form-data">
            <div class="form-group">
                <label class="form-label">Kategori Adı</label>
                <input type="text" name="name" class="form-control" placeholder="Örn: Ana Yemekler, Tatlılar, İçecekler" required autofocus />
            </div>
            <div class="form-group">
                <label class="form-label">Kategori Görseli (İsteğe Bağlı)</label>
                <input type="file" name="photoFile" class="form-control" accept="image/jpeg,image/png,image/webp,image/gif" />
                <small class="text-muted">Desteklenen formatlar: JPG, PNG, WEBP, GIF. Maksimum: 5MB.</small>
            </div>
            <div style="display:flex;gap:10px;margin-top:8px;">
                <button type="submit" class="btn btn-primary">💾 Kaydet</button>
                <a asp-action="Index" class="btn btn-secondary">İptal</a>
            </div>
        </form>
    </div>
</div>

`

--------------------------------------------------------------------------------

## Category/Edit.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/Category/Edit.cshtml

`cshtml
@model EntityLayer.Concrete.Category
@{
    ViewData["Title"] = "Kategori Düzenle";
}
<div class="card" style="max-width:560px;">
    <div class="card-header"><h5>✏️ Kategori Düzenle</h5></div>
    <div class="card-body">
        <form method="post" asp-action="Edit" enctype="multipart/form-data">
            <input type="hidden" name="id" value="@Model.Id" />
            <div class="form-group">
                <label class="form-label">Kategori Adı</label>
                <input type="text" name="name" value="@Model.Name" class="form-control" required autofocus />
            </div>
            
            @if (!string.IsNullOrEmpty(Model.ImageUrl))
            {
                <div class="form-group">
                    <label class="form-label">Mevcut Görsel</label>
                    <div>
                        <img src="@Model.ImageUrl" alt="@Model.Name" style="max-width: 200px; max-height: 200px; object-fit: cover; border-radius: 8px; border: 1px solid #ddd;" />
                    </div>
                </div>
            }

            <div class="form-group">
                <label class="form-label">Kategori Görseli (İsteğe Bağlı - Yeni görsel seçilmezse mevcut görsel korunur)</label>
                <input type="file" name="photoFile" class="form-control" accept="image/jpeg,image/png,image/webp,image/gif" />
                <small class="text-muted">Desteklenen formatlar: JPG, PNG, WEBP, GIF. Maksimum: 5MB.</small>
            </div>
            <div style="display:flex;gap:10px;margin-top:8px;">
                <button type="submit" class="btn btn-primary">💾 Güncelle</button>
                <a asp-action="Index" class="btn btn-secondary">İptal</a>
            </div>
        </form>
    </div>
</div>

`

--------------------------------------------------------------------------------

## Category/Index.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/Category/Index.cshtml

`cshtml
@model List<EntityLayer.Concrete.Category>
@{
    ViewData["Title"] = "Kategoriler";
}

<div class="card">
    <div class="card-header">
        <h5>🏷️ Kategorilerim</h5>
        <a asp-action="Create" class="btn btn-primary btn-sm">➕ Yeni Kategori</a>
    </div>
    @if (!Model.Any())
    {
        <div style="padding:48px;text-align:center;color:#7d8590;">
            <div style="font-size:3rem;margin-bottom:12px;">🏷️</div>
            <p style="font-size:.9rem;">Henüz kategori eklenmemiş. İlk kategorinizi ekleyin!</p>
        </div>
    }
    else
    {
        <table>
            <thead>
                <tr>
                    <th>#</th>
                    <th>Görsel</th>
                    <th>Kategori Adı</th>
                    <th>İşlemler</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var item in Model)
                {
                    <tr>
                        <td><span class="badge badge-green">@item.Id</span></td>
                        <td>
                            @if (!string.IsNullOrEmpty(item.ImageUrl))
                            {
                                <img src="@item.ImageUrl" alt="@item.Name" style="width:40px;height:40px;object-fit:cover;border-radius:4px;" />
                            }
                            else
                            {
                                <div style="width:40px;height:40px;background:#e1e4e8;border-radius:4px;display:flex;align-items:center;justify-content:center;color:#6a737d;font-size:12px;">Yok</div>
                            }
                        </td>
                        <td>@item.Name</td>
                        <td style="display:flex;gap:8px;">
                            <a asp-action="Edit" asp-route-id="@item.Id" class="btn btn-sm btn-warning">✏️ Düzenle</a>
                            <partial name="~/Views/Shared/_DeleteButton.cshtml" model="item.Id" />
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

`

--------------------------------------------------------------------------------

## Dashboard/Index.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/Dashboard/Index.cshtml

`cshtml
@{
    ViewData["Title"] = "Dashboard";
}

<div class="stat-grid">
    <div class="stat-card stat-green">
        <div class="stat-icon">🏷️</div>
        <div class="stat-label">Toplam Kategori</div>
        <div class="stat-value">@ViewBag.CategoryCount</div>
    </div>
    <div class="stat-card stat-blue">
        <div class="stat-icon">🍕</div>
        <div class="stat-label">Toplam Ürün</div>
        <div class="stat-value">@ViewBag.MenuItemCount</div>
    </div>
</div>

<div class="card">
    <div class="card-header">
        <h5>🚀 Hızlı Menü İşlemleri</h5>
    </div>
    <div class="card-body" style="display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 12px;">
        <a asp-area="Restaurant" asp-controller="Category" asp-action="Create" class="btn btn-primary" style="justify-content: center;">
            <span>➕</span> Yeni Kategori
        </a>
        <a asp-area="Restaurant" asp-controller="MenuItem" asp-action="Create" class="btn btn-primary" style="justify-content: center;">
            <span>➕</span> Yeni Ürün
        </a>
        <a asp-area="Restaurant" asp-controller="Category" asp-action="Index" class="btn btn-secondary" style="justify-content: center;">
            <span>🏷️</span> Kategorileri Yönet
        </a>
        <a asp-area="Restaurant" asp-controller="MenuItem" asp-action="Index" class="btn btn-secondary" style="justify-content: center;">
            <span>🍕</span> Ürünleri Yönet
        </a>
        <a asp-area="Restaurant" asp-controller="Builder" asp-action="Index" class="btn btn-secondary" style="justify-content: center;">
            <span>✨</span> Menü Oluşturucu
        </a>
    </div>
</div>

<div class="card">
    <div class="card-header">
        <h5>📱 QR Kod ve Canlı Menü Bağlantınız</h5>
        <span style="background: rgba(16, 185, 129, 0.12); color: var(--primary-dark); padding: 4px 12px; border-radius: var(--radius-full); font-size: 0.75rem; font-weight: 700;">
            İnternet Erişimi Aktif
        </span>
    </div>
    <div class="card-body" style="text-align: center; display: flex; flex-direction: column; align-items: center; gap: 20px;">
        @if (ViewBag.QrCodeImage != null)
        {
            <div style="background: #ffffff; padding: 16px; border-radius: var(--radius-lg); border: 1px solid var(--border); box-shadow: var(--shadow-sm); display: inline-block;">
                <img src="@ViewBag.QrCodeImage" alt="Restoran QR Kodu" width="220" height="220" style="max-width: 220px; width: 100%; height: auto; aspect-ratio: 1/1; border-radius: 12px; display: block;" />
            </div>

            <div style="background: var(--surface-low); padding: 18px 24px; border-radius: var(--radius-md); border: 1px solid var(--border); max-width: 650px; width: 100%;">
                <span style="color: var(--text-secondary); font-size: 0.8rem; font-weight: 700; text-transform: uppercase; letter-spacing: 0.05em; display: block; margin-bottom: 8px;">
                    Müşterilerinizin Erişeceği Canlı Menü Linki:
                </span>
                <div style="display: flex; align-items: center; justify-content: center; gap: 12px; flex-wrap: wrap;">
                    <a href="@ViewBag.MenuUrl" target="_blank" rel="noopener" style="color: var(--primary-dark); font-weight: 700; font-size: 1.05rem; word-break: break-all; text-decoration: none;">
                        @ViewBag.MenuUrl
                    </a>
                    <button type="button" id="btnCopyMenuUrl" onclick="navigator.clipboard.writeText('@ViewBag.MenuUrl'); const el=this; el.innerHTML='<span>✓</span> Kopyalandı!'; setTimeout(()=>el.innerHTML='<span>📋</span> Kopyala', 2000);" class="btn btn-secondary btn-sm">
                        <span>📋</span> Kopyala
                    </button>
                </div>
            </div>

            <div style="display: flex; gap: 12px; justify-content: center; flex-wrap: wrap;">
                <a href="@ViewBag.QrCodeImage" download="Menu_QRCode.png" class="btn btn-primary" style="padding: 11px 24px;">
                    <span>⬇️</span> QR Kodu İndir (PNG)
                </a>
                <a href="@ViewBag.PublicQrImageUrl" target="_blank" rel="noopener" class="btn btn-secondary" style="padding: 11px 20px;">
                    <span>🌐</span> Doğrudan QR Görseli
                </a>
            </div>

            <div style="background: rgba(16, 185, 129, 0.06); border: 1px solid rgba(16, 185, 129, 0.2); color: var(--primary-dark); padding: 14px 20px; border-radius: var(--radius-md); font-size: 0.85rem; max-width: 650px; width: 100%; display: flex; align-items: center; gap: 10px; text-align: left;">
                <span style="font-size: 1.4rem; flex-shrink: 0;">🔒</span>
                <span><strong>Sabit QR Garantisi:</strong> Menünüzdeki ürünler, fiyatlar veya tasarım değişse bile bu QR kodunuz asla değişmez. Masalardaki baskıları yenilemenize gerek kalmaz.</span>
            </div>
        }
        else
        {
            <p style="color: var(--text-muted); font-size: 0.9rem;">QR Kod oluşturulamadı. Lütfen restoran bilgilerinizi güncelleyin.</p>
        }
    </div>
</div>

`

--------------------------------------------------------------------------------

## MenuItem/Create.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/MenuItem/Create.cshtml

`cshtml
@{
    ViewData["Title"] = "Yeni Ürün";
    var categories = ViewBag.Categories as List<EntityLayer.Concrete.Category>;
    var defaultAllergens = ViewBag.Allergens as IReadOnlyList<string> ?? dijitalmenu.Helpers.AllergenHelper.DefaultAllergens;
}
<div class="card" style="max-width:680px;">
    <div class="card-header"><h5>🍕 Yeni Ürün Ekle</h5></div>
    <div class="card-body">
        <form method="post" asp-action="Create" enctype="multipart/form-data">
            <div class="form-group">
                <label class="form-label">Ürün Adı</label>
                <input type="text" name="Name" class="form-control" placeholder="Örn: Margarita Pizza" required autofocus />
            </div>
            <div class="form-group">
                <label class="form-label">Açıklama <span style="color:#7d8590;font-weight:400;">(opsiyonel)</span></label>
                <textarea name="Description" class="form-control" rows="3" placeholder="Ürün hakkında kısa bir açıklama..."></textarea>
            </div>
            <div style="display:grid;grid-template-columns:1fr 1fr;gap:16px;">
                <div class="form-group">
                    <label class="form-label">Fiyat (₺)</label>
                    <input type="number" name="Price" class="form-control" placeholder="0.00" step="0.01" min="0" required />
                </div>
                <div class="form-group">
                    <label class="form-label">🔥 Kalori (kcal)</label>
                    <input type="number" name="Calories" class="form-control" placeholder="Örn: 450" min="0" max="10000" />
                    <small style="color:#7d8590;display:block;margin-top:4px;">Opsiyonel (1 porsiyon için enerji)</small>
                </div>
            </div>
            <div class="form-group">
                <label class="form-label">Kategori</label>
                <select name="CategoryId" class="form-control">
                    @foreach (var cat in categories ?? new List<EntityLayer.Concrete.Category>())
                    {
                        <option value="@cat.Id">@cat.Name</option>
                    }
                </select>
            </div>

            <!-- Alerjenler (Checkbox Seçimi) -->
            <div class="form-group" style="margin-top:16px;">
                <label class="form-label" style="display:flex;align-items:center;gap:6px;">
                    <span>⚠️</span>
                    <span>Alerjenler</span>
                    <span style="color:#7d8590;font-weight:400;font-size:12px;">(Ürünün içerdiği alerjenleri işaretleyin)</span>
                </label>
                <div style="display:grid;grid-template-columns:repeat(auto-fill, minmax(180px, 1fr));gap:8px;padding:12px;background:#f8f9fa;border:1px solid #E5E7EB;border-radius:8px;max-height:220px;overflow-y:auto;">
                    @foreach (var allergen in defaultAllergens)
                    {
                        <label style="display:flex;align-items:center;gap:8px;font-size:13px;cursor:pointer;margin:0;user-select:none;">
                            <input type="checkbox" name="SelectedAllergens" value="@allergen" style="width:16px;height:16px;accent-color:#10B981;cursor:pointer;" />
                            <span>@allergen</span>
                        </label>
                    }
                </div>
            </div>

            <div class="form-group" style="margin-top:16px;">
                <label class="form-label">📸 Ürün Fotoğrafı Yükle <span style="color:#7d8590;font-weight:400;">(Dosya Seç)</span></label>
                <input type="file" name="photoFile" class="form-control" accept="image/*" />
            </div>
            <div class="form-group">
                <label class="form-label">🔗 veya Fotoğraf Bağlantısı (URL)</label>
                <input type="text" name="ImageUrl" class="form-control" placeholder="https://example.com/gorsel.jpg" />
            </div>
            <div style="display:flex;gap:10px;margin-top:20px;">
                <button type="submit" class="btn btn-primary">💾 Kaydet</button>
                <a asp-action="Index" class="btn btn-secondary">İptal</a>
            </div>
        </form>
    </div>
</div>

`

--------------------------------------------------------------------------------

## MenuItem/Edit.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/MenuItem/Edit.cshtml

`cshtml
@model EntityLayer.Concrete.MenuItem
@{
    ViewData["Title"] = "Ürün Düzenle";
    var categories = ViewBag.Categories as List<EntityLayer.Concrete.Category>;
    var defaultAllergens = ViewBag.Allergens as IReadOnlyList<string> ?? dijitalmenu.Helpers.AllergenHelper.DefaultAllergens;
    var selectedAllergens = ViewBag.SelectedAllergens as HashSet<string> ?? dijitalmenu.Helpers.AllergenHelper.ParseAllergens(Model.Allergens);
}
<div class="card" style="max-width:680px;">
    <div class="card-header"><h5>✏️ Ürün Düzenle</h5></div>
    <div class="card-body">
        <form method="post" asp-action="Edit" enctype="multipart/form-data">
            <input type="hidden" name="Id" value="@Model.Id" />
            <div class="form-group">
                <label class="form-label">Ürün Adı</label>
                <input type="text" name="Name" value="@Model.Name" class="form-control" required autofocus />
            </div>
            <div class="form-group">
                <label class="form-label">Açıklama</label>
                <textarea name="Description" class="form-control" rows="3">@Model.Description</textarea>
            </div>
            <div style="display:grid;grid-template-columns:1fr 1fr;gap:16px;">
                <div class="form-group">
                    <label class="form-label">Fiyat (₺)</label>
                    <input type="number" name="Price" value="@Model.Price" class="form-control" step="0.01" min="0" required />
                </div>
                <div class="form-group">
                    <label class="form-label">🔥 Kalori (kcal)</label>
                    <input type="number" name="Calories" value="@(Model.Calories.HasValue ? Model.Calories.Value.ToString() : "")" class="form-control" placeholder="Örn: 450" min="0" max="10000" />
                    <small style="color:#7d8590;display:block;margin-top:4px;">Opsiyonel (1 porsiyon için enerji)</small>
                </div>
            </div>
            <div class="form-group">
                <label class="form-label">Kategori</label>
                <select name="CategoryId" class="form-control">
                    @foreach (var cat in categories ?? new List<EntityLayer.Concrete.Category>())
                    {
                        if (cat.Id == Model.CategoryId)
                        {
                            <option value="@cat.Id" selected>@cat.Name</option>
                        }
                        else
                        {
                            <option value="@cat.Id">@cat.Name</option>
                        }
                    }
                </select>
            </div>

            <!-- Alerjenler (Checkbox Seçimi) -->
            <div class="form-group" style="margin-top:16px;">
                <label class="form-label" style="display:flex;align-items:center;gap:6px;">
                    <span>⚠️</span>
                    <span>Alerjenler</span>
                    <span style="color:#7d8590;font-weight:400;font-size:12px;">(Ürünün içerdiği alerjenleri işaretleyin)</span>
                </label>
                <div style="display:grid;grid-template-columns:repeat(auto-fill, minmax(180px, 1fr));gap:8px;padding:12px;background:#f8f9fa;border:1px solid #E5E7EB;border-radius:8px;max-height:220px;overflow-y:auto;">
                    @foreach (var allergen in defaultAllergens)
                    {
                        var isChecked = selectedAllergens.Contains(allergen);
                        <label style="display:flex;align-items:center;gap:8px;font-size:13px;cursor:pointer;margin:0;user-select:none;">
                            <input type="checkbox" name="SelectedAllergens" value="@allergen" @(isChecked ? "checked" : "") style="width:16px;height:16px;accent-color:#10B981;cursor:pointer;" />
                            <span>@allergen</span>
                        </label>
                    }
                </div>
            </div>

            @if (!string.IsNullOrWhiteSpace(Model.ImageUrl))
            {
                <div class="form-group" style="margin-top:16px;">
                    <label class="form-label">Mevcut Fotoğraf</label>
                    <div style="margin-bottom:8px;">
                        <img src="@Model.ImageUrl" alt="@Model.Name" style="max-height:120px;border-radius:8px;object-fit:cover;" />
                    </div>
                </div>
            }
            <div class="form-group">
                <label class="form-label">📸 Yeni Fotoğraf Yükle <span style="color:#7d8590;font-weight:400;">(Dosya Seç)</span></label>
                <input type="file" name="photoFile" class="form-control" accept="image/*" />
            </div>
            <div class="form-group">
                <label class="form-label">🔗 veya Fotoğraf Bağlantısı (URL)</label>
                <input type="text" name="ImageUrl" value="@Model.ImageUrl" class="form-control" placeholder="https://example.com/gorsel.jpg" />
            </div>
            <div style="display:flex;gap:10px;margin-top:20px;">
                <button type="submit" class="btn btn-primary">💾 Güncelle</button>
                <a asp-action="Index" class="btn btn-secondary">İptal</a>
            </div>
        </form>
    </div>
</div>

`

--------------------------------------------------------------------------------

## MenuItem/Index.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/MenuItem/Index.cshtml

`cshtml
@model List<EntityLayer.Concrete.MenuItem>
@{
    ViewData["Title"] = "Ürünlerim";
    var categories = ViewBag.Categories as List<EntityLayer.Concrete.Category>;
}

<div class="card">
    <div class="card-header">
        <h5>🍕 Ürün Listesi</h5>
        <a asp-action="Create" class="btn btn-primary btn-sm">➕ Yeni Ürün</a>
    </div>
    @if (!Model.Any())
    {
        <div style="padding:48px;text-align:center;color:#7d8590;">
            <div style="font-size:3rem;margin-bottom:12px;">🍕</div>
            <p style="font-size:.9rem;">Henüz ürün eklenmemiş. Önce bir kategori, sonra ürünlerinizi ekleyin.</p>
            <a asp-area="Restaurant" asp-controller="Category" asp-action="Create"
               style="display:inline-block;margin-top:16px;color:#4ade80;font-size:.875rem;">+ Kategori Ekle</a>
        </div>
    }
    else
    {
        <table>
            <thead>
                <tr>
                    <th>#</th>
                    <th>Görsel</th>
                    <th>Ürün Adı</th>
                    <th>Kategori</th>
                    <th>Fiyat</th>
                    <th>İşlemler</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var item in Model)
                {
                    var catName = categories?.FirstOrDefault(c => c.Id == item.CategoryId)?.Name ?? "-";
                    var imgUrl = !string.IsNullOrWhiteSpace(item.ImageUrl) ? item.ImageUrl : "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&q=80&w=100";
                    <tr>
                        <td><span class="badge badge-green">@item.Id</span></td>
                        <td>
                            <img src="@imgUrl" alt="@item.Name" style="width:44px;height:44px;border-radius:8px;object-fit:cover;" onerror="this.src='https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&q=80&w=100'" />
                        </td>
                        <td>@item.Name</td>
                        <td style="color:#7d8590;">@catName</td>
                        <td style="color:#4ade80;font-weight:600;">@item.Price.ToString("N2") ₺</td>
                        <td style="display:flex;gap:8px;">
                            <a asp-action="Edit" asp-route-id="@item.Id" class="btn btn-sm btn-warning">✏️ Düzenle</a>
                            <partial name="~/Views/Shared/_DeleteButton.cshtml" model="item.Id" />
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

`

--------------------------------------------------------------------------------

## Shared/_RestaurantLayout.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/Shared/_RestaurantLayout.cshtml

`cshtml
<!DOCTYPE html>
<html lang="tr">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] — Restoran Paneli</title>

    <!-- Google Fonts: Sora & Inter -->
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&family=Sora:wght@600;700;800&display=swap" rel="stylesheet" />

    <!-- Material Symbols -->
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@24,400,0..1,0&display=swap" rel="stylesheet" />

    <style>
        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

        :root {
            --sidebar-w: 260px;
            --primary: #10B981;
            --primary-dark: #006c49;
            --primary-light: #4edea3;
            --primary-dim: rgba(16, 185, 129, 0.12);
            --charcoal: #111827;
            --charcoal-light: #1f2937;
            --bg: #f8f9fa;
            --surface: #ffffff;
            --surface-low: #f3f4f5;
            --surface-container: #edeeef;
            --text: #191c1d;
            --text-secondary: #575e70;
            --text-muted: #7d8590;
            --border: #E5E7EB;
            --border-light: #f0f1f2;
            --danger: #ef4444;
            --warning: #f59e0b;
            --shadow-sm: 0 1px 3px rgba(0, 0, 0, 0.05);
            --shadow-md: 0 4px 20px rgba(0, 0, 0, 0.05);
            --radius-sm: 8px;
            --radius-md: 14px;
            --radius-lg: 20px;
            --radius-xl: 28px;
            --radius-full: 9999px;
            --font-display: 'Sora', sans-serif;
            --font-body: 'Inter', sans-serif;
        }

        body {
            font-family: var(--font-body);
            background: var(--bg);
            color: var(--text);
            display: flex;
            min-height: 100vh;
            -webkit-font-smoothing: antialiased;
        }

        /* ─── Sidebar ─── */
        .sidebar {
            width: var(--sidebar-w);
            background: var(--charcoal);
            border-right: 1px solid var(--charcoal-light);
            display: flex;
            flex-direction: column;
            position: fixed;
            top: 0; left: 0; bottom: 0;
            z-index: 100;
            color: #ffffff;
        }

        .sidebar-brand {
            padding: 24px 20px;
            border-bottom: 1px solid rgba(255, 255, 255, 0.08);
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .brand-icon-box {
            width: 40px;
            height: 40px;
            border-radius: var(--radius-sm);
            background: linear-gradient(135deg, var(--primary), var(--primary-dark));
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.25rem;
            color: #ffffff;
            box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
            flex-shrink: 0;
        }

        .brand-text-wrap {
            overflow: hidden;
        }

        .brand-title {
            font-family: var(--font-display);
            font-size: 1.05rem;
            font-weight: 700;
            color: #ffffff;
            line-height: 1.2;
        }

        .brand-subtitle {
            font-size: 0.75rem;
            color: var(--text-muted);
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
            display: block;
            margin-top: 2px;
        }

        .sidebar-nav {
            padding: 20px 14px;
            flex: 1;
            overflow-y: auto;
            display: flex;
            flex-direction: column;
            gap: 4px;
        }

        .nav-section-label {
            font-size: 0.7rem;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.08em;
            color: #6b7280;
            padding: 14px 12px 6px;
        }

        .sidebar-nav-link {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 10px 14px;
            border-radius: var(--radius-sm);
            color: #9ca3af;
            text-decoration: none;
            font-size: 0.875rem;
            font-weight: 500;
            transition: all 0.15s ease;
        }

        .sidebar-nav-link:hover {
            background: rgba(255, 255, 255, 0.06);
            color: #ffffff;
        }

        .sidebar-nav-link.active {
            background: var(--primary);
            color: #ffffff;
            font-weight: 600;
            box-shadow: 0 4px 12px rgba(16, 185, 129, 0.28);
        }

        .sidebar-nav-link .icon {
            font-size: 1.2rem;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .sidebar-footer {
            padding: 18px 16px;
            border-top: 1px solid rgba(255, 255, 255, 0.08);
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .public-menu-link {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 9px 14px;
            background: rgba(16, 185, 129, 0.12);
            border: 1px solid rgba(16, 185, 129, 0.25);
            border-radius: var(--radius-sm);
            color: var(--primary-light);
            text-decoration: none;
            font-size: 0.8rem;
            font-weight: 600;
            transition: all 0.2s ease;
        }

        .public-menu-link:hover {
            background: rgba(16, 185, 129, 0.22);
            color: #ffffff;
        }

        .btn-logout {
            display: flex;
            align-items: center;
            gap: 10px;
            width: 100%;
            padding: 9px 14px;
            background: transparent;
            border: none;
            border-radius: var(--radius-sm);
            color: #9ca3af;
            font-size: 0.85rem;
            font-weight: 500;
            cursor: pointer;
            text-align: left;
            transition: all 0.2s ease;
        }

        .btn-logout:hover {
            background: rgba(239, 68, 68, 0.12);
            color: #f87171;
        }

        /* ─── Main Content Area ─── */
        .main-wrapper {
            margin-left: var(--sidebar-w);
            flex: 1;
            display: flex;
            flex-direction: column;
            min-height: 100vh;
        }

        .topbar {
            height: 72px;
            background: #ffffff;
            border-bottom: 1px solid var(--border);
            padding: 0 32px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            position: sticky;
            top: 0;
            z-index: 50;
        }

        .topbar-title {
            font-family: var(--font-display);
            font-size: 1.15rem;
            font-weight: 700;
            color: var(--charcoal);
        }

        .topbar-user {
            display: flex;
            align-items: center;
            gap: 10px;
            background: var(--surface-low);
            padding: 6px 14px 6px 6px;
            border-radius: var(--radius-full);
            border: 1px solid var(--border);
        }

        .topbar-user .avatar {
            width: 32px;
            height: 32px;
            border-radius: 50%;
            background: var(--primary);
            color: #ffffff;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
            font-size: 0.85rem;
        }

        .topbar-user span {
            font-size: 0.875rem;
            font-weight: 600;
            color: var(--charcoal);
        }

        .content {
            padding: 32px;
            flex: 1;
        }

        /* ─── Modern Card System ─── */
        .card {
            background: var(--surface);
            border: 1px solid var(--border);
            border-radius: var(--radius-lg);
            overflow: hidden;
            box-shadow: var(--shadow-sm);
            margin-bottom: 24px;
        }

        .card-header {
            padding: 20px 24px;
            border-bottom: 1px solid var(--border);
            display: flex;
            align-items: center;
            justify-content: space-between;
            background: #ffffff;
        }

        .card-header h5 {
            font-family: var(--font-display);
            font-size: 1rem;
            font-weight: 700;
            color: var(--charcoal);
        }

        .card-body {
            padding: 24px;
        }

        /* ─── Modern Buttons ─── */
        .btn {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 9px 18px;
            border-radius: var(--radius-sm);
            font-size: 0.875rem;
            font-weight: 600;
            border: none;
            cursor: pointer;
            text-decoration: none;
            transition: all 0.2s ease;
        }

        .btn-primary {
            background: var(--primary);
            color: #ffffff;
            box-shadow: 0 2px 8px rgba(16, 185, 129, 0.2);
        }

        .btn-primary:hover {
            background: var(--primary-dark);
            transform: translateY(-1px);
            box-shadow: 0 4px 14px rgba(16, 185, 129, 0.3);
            color: #ffffff;
        }

        .btn-secondary {
            background: var(--surface-low);
            color: var(--charcoal);
            border: 1px solid var(--border);
        }

        .btn-secondary:hover {
            background: #e5e7eb;
        }

        .btn-warning {
            background: rgba(245, 158, 11, 0.12);
            color: #b45309;
            border: 1px solid rgba(245, 158, 11, 0.25);
        }

        .btn-warning:hover {
            background: rgba(245, 158, 11, 0.2);
        }

        .btn-danger {
            background: rgba(239, 68, 68, 0.12);
            color: #b91c1c;
            border: 1px solid rgba(239, 68, 68, 0.25);
        }

        .btn-danger:hover {
            background: rgba(239, 68, 68, 0.2);
        }

        .btn-sm {
            padding: 6px 12px;
            font-size: 0.8rem;
            border-radius: 6px;
        }

        /* ─── Forms ─── */
        .form-group {
            margin-bottom: 20px;
        }

        .form-label {
            display: block;
            font-size: 0.875rem;
            font-weight: 600;
            margin-bottom: 8px;
            color: var(--charcoal);
        }

        .form-control {
            width: 100%;
            padding: 11px 16px;
            background: #ffffff;
            border: 1px solid var(--border);
            border-radius: var(--radius-sm);
            color: var(--text);
            font-size: 0.9rem;
            font-family: inherit;
            transition: all 0.2s ease;
            outline: none;
        }

        .form-control:focus {
            border-color: var(--primary);
            box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.15);
        }

        /* ─── Tables ─── */
        table {
            width: 100%;
            border-collapse: collapse;
            font-size: 0.875rem;
        }

        thead th {
            padding: 12px 16px;
            text-align: left;
            font-size: 0.75rem;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            color: var(--text-secondary);
            border-bottom: 1px solid var(--border);
            background: var(--surface-low);
        }

        tbody tr {
            border-bottom: 1px solid var(--border-light);
            transition: background 0.15s ease;
        }

        tbody tr:hover {
            background: var(--surface-low);
        }

        tbody td {
            padding: 14px 16px;
            color: var(--charcoal);
        }

        tbody tr:last-child {
            border-bottom: none;
        }

        /* ─── Stat Cards ─── */
        .stat-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 20px;
            margin-bottom: 28px;
        }

        .stat-card {
            background: #ffffff;
            border: 1px solid var(--border);
            border-radius: var(--radius-lg);
            padding: 24px;
            box-shadow: var(--shadow-sm);
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

        .stat-icon {
            font-size: 1.6rem;
        }

        .stat-label {
            font-size: 0.8rem;
            color: var(--text-secondary);
            font-weight: 600;
        }

        .stat-value {
            font-family: var(--font-display);
            font-size: 2.2rem;
            font-weight: 800;
            color: var(--charcoal);
        }

        .stat-green .stat-value { color: var(--primary); }
        .stat-blue .stat-value { color: #0284c7; }

        /* ─── Alerts ─── */
        .alert-error {
            background: rgba(239, 68, 68, 0.08);
            border: 1px solid rgba(239, 68, 68, 0.25);
            border-radius: var(--radius-sm);
            padding: 12px 18px;
            color: #b91c1c;
            font-size: 0.875rem;
            font-weight: 500;
            margin-bottom: 20px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .alert-info {
            background: var(--primary-dim);
            border: 1px solid rgba(16, 185, 129, 0.3);
            border-radius: var(--radius-sm);
            padding: 12px 18px;
            color: var(--primary-dark);
            font-size: 0.875rem;
            font-weight: 600;
            margin-bottom: 20px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        @@media (max-width: 768px) {
            .sidebar { display: none; }
            .main-wrapper { margin-left: 0; }
            .content { padding: 16px; }
            .topbar { padding: 0 16px; }
        }
    </style>
    @await RenderSectionAsync("Styles", required: false)
</head>
<body>

<aside class="sidebar">
    <div class="sidebar-brand">
        <div class="brand-icon-box">🍃</div>
        <div class="brand-text-wrap">
            <div class="brand-title">Restoran Paneli</div>
            <span class="brand-subtitle">@(ViewBag.RestaurantUsername ?? "Kullanıcı")</span>
        </div>
    </div>

    <nav class="sidebar-nav">
        <div class="nav-section-label">Genel</div>
        <a class="sidebar-nav-link @(ViewContext.RouteData.Values["controller"]?.ToString() == "Dashboard" ? "active" : "")"
           asp-area="Restaurant" asp-controller="Dashboard" asp-action="Index">
            <span class="icon">📊</span> Dashboard
        </a>

        <div class="nav-section-label">Menü Yönetimi</div>
        <a class="sidebar-nav-link @(ViewContext.RouteData.Values["controller"]?.ToString() == "Category" ? "active" : "")"
           asp-area="Restaurant" asp-controller="Category" asp-action="Index">
            <span class="icon">🏷️</span> Kategoriler
        </a>
        <a class="sidebar-nav-link @(ViewContext.RouteData.Values["controller"]?.ToString() == "MenuItem" ? "active" : "")"
           asp-area="Restaurant" asp-controller="MenuItem" asp-action="Index">
            <span class="icon">🍕</span> Ürünler
        </a>
        <a class="sidebar-nav-link @(ViewContext.RouteData.Values["controller"]?.ToString() == "Builder" ? "active" : "")"
           asp-area="Restaurant" asp-controller="Builder" asp-action="Index">
            <span class="icon">✨</span> Menü Oluşturucu
        </a>

        <div class="nav-section-label">Hesap</div>
        <a class="sidebar-nav-link @(ViewContext.RouteData.Values["controller"]?.ToString() == "Account" ? "active" : "")"
           asp-area="Restaurant" asp-controller="Account" asp-action="Index">
            <span class="icon">👤</span> Hesabım
        </a>
        <a class="sidebar-nav-link @(ViewContext.RouteData.Values["controller"]?.ToString() == "AuditLog" ? "active" : "")"
           asp-area="Restaurant" asp-controller="AuditLog" asp-action="Index">
            <span class="icon">📜</span> İşlem Geçmişi
        </a>
    </nav>

    <div class="sidebar-footer">
        @{
            var restaurantId = Context.Session.GetString("RestaurantId");
        }
        @if (!string.IsNullOrEmpty(restaurantId))
        {
            <a class="public-menu-link" href="/Home/Menu/@restaurantId" target="_blank" rel="noopener">
                <span>🌐</span> Menümü Görüntüle
            </a>
        }
        <form asp-area="Restaurant" asp-controller="Auth" asp-action="Logout" method="post" style="display:inline">
            <button type="submit" class="btn-logout">
                <span>🚪</span> Çıkış Yap
            </button>
        </form>
    </div>
</aside>

<div class="main-wrapper">
    <header class="topbar">
        <div class="topbar-title">@(ViewData["Title"] ?? "Dashboard")</div>
        <div class="topbar-user">
            <div class="avatar">@(ViewBag.RestaurantUsername?.ToString()?.Substring(0, 1)?.ToUpper() ?? "R")</div>
            <span>@(ViewBag.RestaurantUsername ?? "Kullanıcı")</span>
        </div>
    </header>

    <main class="content">
        @if (TempData["Error"] != null)
        {
            <div class="alert-error">
                <span>⚠️</span>
                <span>@TempData["Error"]</span>
            </div>
        }
        @if (TempData["Success"] != null)
        {
            <div class="alert-info">
                <span>✅</span>
                <span>@TempData["Success"]</span>
            </div>
        }
        @RenderBody()
    </main>
</div>

@await RenderSectionAsync("Scripts", required: false)
</body>
</html>

`

--------------------------------------------------------------------------------

## _ViewImports.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/_ViewImports.cshtml

`cshtml
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@using EntityLayer.Concrete

`

--------------------------------------------------------------------------------

## _ViewStart.cshtml
**Dosya Yolu:** Areas/Restaurant/Views/_ViewStart.cshtml

`cshtml
@{
    Layout = "_RestaurantLayout";
}

`

--------------------------------------------------------------------------------

