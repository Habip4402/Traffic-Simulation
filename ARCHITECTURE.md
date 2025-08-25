# Traffic Simulation - Architecture Overview

## Sistemler

### 1. CarSpawnerSystem
- Her spawner, belirlenen interval ile araç spawn eder.
- Spawn edilen araçlar pool’dan alınır ya da yoksa yeni instantiate edilir.
- Araç spawn edildiğinde `VehicleData` ve `LocalTransform` ile konum ve hareket bilgisi atanır.
- `ActiveCount` pool sayısını günceller.

### 2. PoolCountUpdateSystem
- Despawn edilen araçlara `ReturnedToPoolTag` eklenir.
- Bu sistem, tag’li araçları bulur ve ilgili pool’un `ActiveCount` değerini düşürür.
- EntityCommandBuffer kullanılarak güvenli şekilde component kaldırır.

### 3. VehicleMovementSystem
- Araçları yollar boyunca hareket ettirir.
- `VehicleData` içerisindeki `Speed` ve `RoadT` parametrelerini kullanır.
- Hedef noktaya ulaştığında despawn tetiklenir.

### 4. TrafficLightSystem
- Tüm lambaların state’lerini günceller (Red/Green).
- Başlangıçta sıralamaya göre timer gecikmesi uygulanır.
- DOTS ECS kullanarak performanslı şekilde çalışır.

### 5. CarCountUI
- Sahnedeki aktif araç sayısını toplar.
- Unity UI (TextMeshPro) üzerinden oyuncuya gösterir.

## Data Components

- **CarSpawnerData:** Spawner’ın spawn interval ve timer bilgisi.
- **CarPoolData:** Pool’un maksimum ve aktif araç sayısı.
- **VehicleData:** Araçların hareket bilgisi (hız, konum, hedef vs.)
- **ReturnedToPoolTag:** Araç pool’a dönmüşse işaretlemek için.
- **SpawnedBy:** Hangi spawner tarafından spawn edildiğini tutar.

## Flow

1. CarSpawnerSystem → Araç spawn.
2. VehicleMovementSystem → Araçları hareket ettir.
3. Araç hedefe ulaşınca → ReturnedToPoolTag ekle.
4. PoolCountUpdateSystem → Pool count azalt.
5. CarCountUI → UI güncelle.
6. TrafficLightSystem → Lambaları yönet.
