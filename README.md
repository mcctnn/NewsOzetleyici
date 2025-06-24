📰 Akıllı Haber Özetleyici
Kullanıcıların haber URL'lerini girerek otomatik özetler alabileceği ve kategorilere göre sınıflandırabileceği modern web uygulaması.
🚀 Özellikler

URL Bazlı Haber Özetleme: Kullanıcılar herhangi bir haber URL'si girerek otomatik özet alabilir
AI Destekli Özetleme: Hugging Face Transformers modelleri ile güçlü metin özetleme
Akıllı Kategori Sınıflandırma: Haberleri konularına göre otomatik kategorilere ayırır
Modern Web Arayüzü: Angular ile geliştirilmiş responsive kullanıcı arayüzü
RESTful API: .NET Core Web API ile güçlü backend
Yerel Veritabanı: SQL Server LocalDB ile hızlı veri depolama

🛠️ Teknoloji Stack
Backend

.NET 9 Web API - RESTful API geliştirme
Entity Framework Core - ORM ve veritabanı işlemleri
SQL Server LocalDB - Geliştirme ortamı veritabanı
HtmlAgilityPack - Web scraping ve HTML parsing

AI & ML

Hugging Face Transformers API - Metin özetleme modelleri
Natural Language Processing - Kategori sınıflandırma

📁 Proje Yapısı
NewsOzetleyici/
│
├── Backend/
│   ├── NewsOzetleyici.API/          # Web API katmanı
│   ├── NewsOzetleyici.Core/         # İş mantığı ve domain modelleri
│   ├── NewsOzetleyici.Data/         # Veri erişim katmanı
│   └── NewsOzetleyici.Services/     # Servis implementasyonları
│
├── README.md
└── .gitignore
