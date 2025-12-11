# 🏥 Triyaj Sistemi (Triage System)

Hastane acil servislerinde kullanılan profesyonel bir hasta triyaj yönetim sistemi. Bu sistem, hastaların aciliyet seviyelerini değerlendirerek öncelik sırasını belirler ve hasta akışını optimize eder.

## 📋 İçindekiler

- [Özellikler](#özellikler)
- [Teknoloji Stack](#teknoloji-stack)
- [Proje Mimarisi](#proje-mimarisi)
- [Kurulum](#kurulum)
- [Kullanım](#kullanım)
- [API Endpoints](#api-endpoints)
- [Triyaj Seviyeleri](#triyaj-seviyeleri)
- [Veritabanı](#veritabanı)
- [Test](#test)
- [Katkıda Bulunma](#katkıda-bulunma)

## ✨ Özellikler

### 🎯 Ana Özellikler
- **Hasta Kaydı**: Yeni hastaların sisteme kaydedilmesi
- **Otomatik Triyaj Değerlendirmesi**: Vital bulgulara dayalı aciliyet seviyesi belirleme
- **Kuyruk Yönetimi**: Triyaj seviyesine göre otomatik sıralama
- **Gerçek Zamanlı Takip**: Bekleyen hastaların anlık durumu
- **Çok Katmanlı Mimari**: Clean Architecture prensipleriyle geliştirilmiş

### 🏥 Triyaj Sistemi
- **Kırmızı (Red)**: Yaşamsal tehlike - Anında müdahale
- **Sarı (Yellow)**: Acil - 15 dakika içinde müdahale
- **Yeşil (Green)**: Acil olmayan - 30 dakika içinde müdahale
- **Mavi (Blue)**: Rutin - 60 dakika içinde müdahale

### 💻 Teknik Özellikler
- RESTful API desteği
- Entity Framework Core ile veritabanı yönetimi
- Swagger UI ile API dokümantasyonu
- Unit test coverage
- CORS desteği

## 🛠 Teknoloji Stack

### Backend
- **.NET 9.0**: Ana framework
- **ASP.NET Core MVC**: Web framework
- **Entity Framework Core**: ORM
- **SQL Server**: Veritabanı
- **xUnit**: Test framework

### Frontend
- **Bootstrap 5**: UI framework
- **jQuery**: JavaScript library
- **HTML5/CSS3**: Markup ve styling

### Geliştirme Araçları
- **Visual Studio / JetBrains Rider**: IDE
- **Swagger**: API dokümantasyonu
- **Git**: Version control

## 🏗 Proje Mimarisi

Proje Clean Architecture prensipleriyle 4 katmanlı bir mimari kullanmaktadır:

```
Triyaj.Solution/
├── 📁 Triyaj.Domain/          # Domain katmanı - Entity'ler ve Value Object'ler
│   ├── Entities/
│   │   ├── Patient.cs         # Hasta entity'si
│   │   ├── Encounter.cs       # Başvuru entity'si
│   │   └── TriageAssesment.cs # Triyaj değerlendirmesi
│   └── ValueObjects/
│       └── TriageLevel.cs     # Triyaj seviyesi enum
├── 📁 Triyaj.Application/     # Application katmanı - Business logic
│   ├── DTOs/                  # Data Transfer Objects
│   └── Services/              # Business servisler
├── 📁 Triyaj.Infrastructure/  # Infrastructure katmanı - Data access
│   ├── Repositories/          # Repository pattern implementation
│   ├── Migrations/            # EF Core migrations
│   └── Seed/                  # Örnek veriler
├── 📁 Triyaj.API/            # Presentation katmanı - API ve MVC
│   ├── Controllers/           # API ve MVC controller'lar
│   ├── Models/               # View models
│   └── Views/                # Razor views
└── 📁 Triyaj.UnitTests/      # Test projesi
```

### Katman Sorumlulukları

#### 🎯 Domain Layer (Triyaj.Domain)
- Core business entities
- Domain-specific value objects
- Business rules ve constraints

#### ⚙️ Application Layer (Triyaj.Application)
- Business logic implementation
- Data Transfer Objects (DTOs)
- Service interfaces ve implementations

#### 🗄️ Infrastructure Layer (Triyaj.Infrastructure)
- Database context (Entity Framework)
- Repository implementations
- External service integrations

#### 🌐 Presentation Layer (Triyaj.API)
- REST API endpoints
- MVC controllers ve views
- Request/Response models

## 🚀 Kurulum

### Gereksinimler
- **.NET 9.0 SDK** veya üzeri
- **SQL Server** (LocalDB veya Express)
- **Visual Studio 2022** veya **JetBrains Rider**

### Adımlar

1. **Repository'yi klonlayın:**
```bash
git clone https://github.com/asazakk/Triyaj.Solution.git
cd Triyaj.Solution
```

2. **Bağımlılıkları yükleyin:**
```bash
dotnet restore
```

3. **Veritabanı connection string'ini yapılandırın:**
`Triyaj.API/appsettings.json` dosyasında bağlantı dizesini güncelleyin:
```json
{
  "ConnectionStrings": {
    "TriyajDb": "Server=(localdb)\\mssqllocaldb;Database=TriyajDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

4. **Veritabanını oluşturun:**
```bash
dotnet ef database update --project Triyaj.Infrastructure --startup-project Triyaj.API
```

5. **Uygulamayı çalıştırın:**
```bash
dotnet run --project Triyaj.API
```

Uygulama varsayılan olarak `https://localhost:7001` adresinde çalışacaktır.

## 📖 Kullanım

### Web Arayüzü
- Ana sayfa: `https://localhost:7001`
- Hasta listesi: `https://localhost:7001/Encounters`
- Yeni hasta kaydı: `https://localhost:7001/Encounters/Create`

### API Dokümantasyonu
- Swagger UI: `https://localhost:7001/swagger`

### Temel İş Akışı

1. **Hasta Kaydı**: Yeni hasta sisteme kaydedilir
2. **Başvuru Oluşturma**: Hasta için yeni bir başvuru oluşturulur
3. **Triyaj Değerlendirmesi**: Vital bulgular girilerek aciliyet seviyesi belirlenir
4. **Kuyruk Yönetimi**: Hasta triyaj seviyesine göre sıraya eklenir
5. **Durum Takibi**: Hasta durumu gerçek zamanlı olarak takip edilir

## 🔗 API Endpoints

### Hasta Yönetimi
```http
GET    /api/patients           # Tüm hastaları listele
GET    /api/patients/{id}      # Belirli hastayı getir
POST   /api/patients           # Yeni hasta oluştur
```

### Başvuru Yönetimi
```http
GET    /api/encounters         # Tüm başvuruları listele
GET    /api/encounters/{id}    # Belirli başvuruyu getir
GET    /api/encounters/pending # Bekleyen başvuruları listele
POST   /api/encounters         # Yeni başvuru oluştur
POST   /api/encounters/{id}/triage  # Triyaj değerlendirmesi yap
PUT    /api/encounters/{id}/status  # Başvuru durumunu güncelle
```

### Lookup Verileri
```http
GET    /api/lookups/genders    # Cinsiyet listesi
GET    /api/lookups/sources    # Başvuru kaynakları
```

### Örnek API Kullanımı

#### Yeni Başvuru Oluşturma
```http
POST /api/encounters
Content-Type: application/json

{
  "patientId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "source": "Walk-in"
}
```

#### Triyaj Değerlendirmesi
```http
POST /api/encounters/{id}/triage
Content-Type: application/json

{
  "heartRate": 85,
  "systolicBP": 120,
  "diastolicBP": 80,
  "spO2": 98,
  "temperature": 36.5,
  "complaint": "Baş ağrısı"
}
```

## 🏥 Triyaj Seviyeleri

### Değerlendirme Algoritması

Sistem aşağıdaki vital bulgulara göre otomatik triyaj seviyesi belirler:

| Seviye | Renk | Kalp Atışı | Tansiyon | SpO2 | Ateş | Müdahale Süresi |
|--------|------|------------|----------|------|------|-----------------|
| **1** | 🔴 Kırmızı | <50 veya >120 | >180/110 | <90 | >39°C | Anında |
| **2** | 🟡 Sarı | 50-60 veya 100-120 | 160-180/100-110 | 90-95 | 38-39°C | 15 dakika |
| **3** | 🟢 Yeşil | 60-100 | <160/100 | >95 | <38°C | 30 dakika |
| **4** | 🔵 Mavi | Normal aralıkta | Normal aralıkta | Normal | Normal | 60 dakika |

### Triyaj Kuralları
- En yüksek risk seviyesi genel triyaj seviyesini belirler
- Vital bulgular sürekli olarak monitör edilir
- Durum değişikliklerinde triyaj seviyesi güncellenir

## 🗄️ Veritabanı

### Entity İlişkileri
```
Patient (1) ────── (N) Encounter (1) ────── (N) TriageAssessment
```

### Ana Tablolar

#### Patients
- `Id`: Primary key (GUID)
- `NationalId`: TC Kimlik No (Unique)
- `FirstName`, `LastName`: Ad, Soyad
- `BirthDate`: Doğum tarihi
- `Gender`: Cinsiyet
- `Phone`: Telefon

#### Encounters
- `Id`: Primary key (GUID)
- `PatientId`: Hasta referansı
- `ArrivalTime`: Geliş zamanı
- `Source`: Başvuru kaynağı
- `Status`: Durum (Waiting, InProgress, Completed)
- `TriageLevel`: Aciliyet seviyesi
- `QueuePosition`: Kuyruk pozisyonu

#### TriageAssessments
- `Id`: Primary key (GUID)
- `EncounterId`: Başvuru referansı
- `HeartRate`, `SystolicBP`, `DiastolicBP`, `SpO2`, `Temperature`: Vital bulgular
- `Complaint`: Şikayet
- `RecommendedAction`: Önerilen aksiyon
- `CreatedAt`: Değerlendirme zamanı

## 🧪 Test

### Unit Test Çalıştırma
```bash
dotnet test
```

### Test Coverage
Proje aşağıdaki alanlarda test coverage'e sahiptir:
- ✅ Triyaj algoritması testleri
- ✅ Entity validation testleri
- ✅ Service layer testleri
- ✅ Repository pattern testleri

### Test Örneği
```csharp
[Theory]
[InlineData(45, 120, 80, 85, 36.5, "Red")]    // Düşük nabız
[InlineData(85, 190, 115, 98, 36.5, "Red")]   // Yüksek tansiyon
[InlineData(85, 120, 80, 85, 36.5, "Yellow")] // SpO2 düşük
public async Task EvaluateTriage_ShouldReturnCorrectLevel(
    int heartRate, int systolicBP, int diastolicBP, 
    int spO2, double temp, string expected)
{
    // Test implementation...
}
```

## 🔧 Konfigürasyon

### Geliştirme Ortamı
`appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "TriyajDb": "Server=(localdb)\\mssqllocaldb;Database=TriyajDb;Trusted_Connection=true"
  }
}
```

### Üretim Ortamı
Üretim ortamında aşağıdaki ayarlamaları yapın:
- Connection string'i production veritabanına yönlendirin
- CORS politikalarını güvenlik gereksinimlerine göre ayarlayın
- Logging seviyelerini optimize edin

## 🚀 Deployment

### IIS Deployment
```bash
dotnet publish -c Release -o ./publish
```

### Docker (Gelecek sürümlerde)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
COPY ./publish /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "Triyaj.API.dll"]
```

## 🤝 Katkıda Bulunma

1. Bu repository'yi fork edin
2. Feature branch oluşturun (`git checkout -b feature/yeni-ozellik`)
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik: ...'`)
4. Branch'inizi push edin (`git push origin feature/yeni-ozellik`)
5. Pull Request oluşturun

### Geliştirme Kuralları
- Clean Code prensiplerini takip edin
- Unit test yazın
- SOLID prensiplerini uygulayın
- Meaningful commit mesajları yazın

## 📄 Lisans

Bu proje MIT lisansı ile lisanslanmıştır. Detaylar için `LICENSE` dosyasına bakınız.

## 👥 Ekip

- **Geliştirici**: [asazakk](https://github.com/asazakk)

## 📞 İletişim

Herhangi bir sorunuz veya öneriniz için:
- GitHub Issues: [Sorun bildirin](https://github.com/asazakk/Triyaj.Solution/issues)
- Email: [geliştirici emaili]

---

### 🏥 Hasta Güvenliği Uyarısı
> **Önemli**: Bu sistem eğitim ve demonstrasyon amaçlıdır. Gerçek hastane ortamında kullanım öncesi kapsamlı test ve validasyon gereklidir. Tıbbi kararlar için her zaman kalifiye sağlık personeline danışın.

---

**⭐ Projeyi beğendiyseniz yıldız vermeyi unutmayın!**

