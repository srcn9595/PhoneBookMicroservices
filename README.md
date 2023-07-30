# Telefon Rehberi Microservices Uygulaması

Bu proje, bir telefon rehberi uygulamasının microservices mimarisi ile geliştirilmiş bir örneğidir. Uygulama, kişileri oluşturma, kaldırma ve listeleme, kişiye iletişim bilgisi ekleme ve kaldırma, kişi detaylarını getirme ve konuma göre istatistikler çıkartan raporlar oluşturma işlevlerini sağlar.

## Kurulum

Bu projeyi çalıştırmak için öncelikle bilgisayarınızda .NET Core'un kurulu olması gerekmektedir. Ayrıca, projenin veritabanı olarak Postgres veya MongoDB kullanmaktadır, bu yüzden bu veritabanlarından birinin kurulu olması gerekmektedir.

1. Projeyi klonlayın veya indirin:

  git clone https://github.com/srcn9595/PhoneBookMicroservices.git

2. Proje dizinine gidin:

  cd PhoneBookMicroservices

3. Projeyi build edin:

  dotnet build

4. Projeyi çalıştırın:

  dotnet run


## Kullanım

Proje, aşağıdaki endpointlere sahiptir:

- `GET /api/contacts`: Tüm kişileri listeler.
- `POST /api/contacts`: Yeni bir kişi oluşturur.
- `DELETE /api/contacts/{id}`: Belirtilen ID'ye sahip kişiyi siler.
- `POST /api/contacts/{id}/contact-info`: Belirtilen ID'ye sahip kişiye yeni bir iletişim bilgisi ekler.
- `DELETE /api/contacts/{id}/contact-info/{infoId}`: Belirtilen ID'ye sahip kişiden belirtilen ID'ye sahip iletişim bilgisini siler.
- `GET /api/contacts/{id}`: Belirtilen ID'ye sahip kişinin detaylarını getirir.
- `POST /api/reports`: Yeni bir rapor talebi oluşturur.
- `GET /api/reports`: Tüm raporları listeler.
- `GET /api/reports/{id}`: Belirtilen ID'ye sahip raporun detaylarını getirir.

Her bir endpoint için gerekli parametreler ve dönen yanıtlar hakkında daha fazla bilgi için API belgelendirmesine bakınız.

Veritabanı olarak Postgre SQL kullanımı:
Bu projede, veritabanı olarak Postgre SQL kullanılmaktadır. Postgre SQL, güçlü ve açık kaynaklı bir ilişkisel veritabanı yönetim sistemidir. Proje, verilerin depolanması ve yönetilmesi için Postgre SQL veritabanı ile etkileşim kurmaktadır.

Veritabanı bağlantı ayarları:
Projeyi çalıştırmadan önce, Postgre SQL veritabanınızı kurduğunuzdan emin olun ve bağlantı ayarlarınızı doğru şekilde yapılandırın. Veritabanı bağlantı ayarları, `appsettings.json` dosyasında yapılandırılmaktadır. Lütfen aşağıdaki alanları kendi veritabanı bilgilerinizle güncelleyin:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=<server_adresi>;Database=<veritabanı_adi>;User Id=<kullanici_adi>;Password=<parola>;"
}
```
Veritabanınızı oluşturmak için migration işlemini gerçekleştirmeyi unutmayın. Aşağıdaki komutları proje dizininde çalıştırarak migration işlemini yapabilirsiniz:

dotnet ef migrations add InitialMigration
dotnet ef database update


