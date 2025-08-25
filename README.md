# Traffic Simulation

Bu proje Unity DOTS kullanarak geliştirilmiş bir trafik simülasyonu oyun prototipi. Amaç, yol üzerindeki araçların spawn edilmesi, hareket etmesi ve despawn edilerek tekrar pool’a dönmesiyle performanslı bir trafik sistemi oluşturmak.

## Özellikler

- **Araçlar:** Prefab’ler üzerinden spawn ve despawn. Pool sistemi ile performans optimizasyonu.
- **DOTS tabanlı:** Entities, ECS ve Burst kullanıldı.
- **UI:** Aktif araç sayısını gerçek zamanlı olarak gösteriyor.

## Nasıl çalışır?

1. Unity sahnesini aç.
2. Oyun başlatıldığında her spawner belirlenen aralıklarla araç spawn ediyor.
3. Araçlar hedef noktalarına ulaştığında despawn olup pool’a dönüyor.
4. Pool’dan tekrar araçlar spawn ediliyor, böylece sürekli bir trafik akışı sağlanıyor.
5. UI aktif araç sayısını gösteriyor.

## Kurulum

1. Projeyi GitHub’dan klonla veya zip olarak indir.
2. Unity Hub üzerinden aç (Unity 2023.2 veya üstü önerilir).
3. Play tuşuna bas ve sistemi gözlemle.

## Dikkat
- DOTS ve Entities paketleri projede yüklü olmalı.
