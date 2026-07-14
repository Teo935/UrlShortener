URL Shortener

Un servizio di URL shortening costruito in C# / ASP.NET Core 8, sviluppato come esercizio di system design e come progetto da portfolio per colloqui tecnici.

Cosa fa


Converte un URL lungo in un codice corto (es. https://localhost:7123/w7e)
Reindirizza automaticamente chi visita il link corto verso l'URL originale
Tiene traccia del numero di click per ogni link


Stack tecnologico

ComponenteSceltaPerchéFrameworkASP.NET Core 8 (Web API, Controllers)API REST pura, nessuna view richiestaDatabaseSQLite + Entity Framework CoreZero setup, file singolo, ideale per sviluppo locale/portfolioCacheIMemoryCache (in-process)Incluso in ASP.NET Core, nessun servizio esterno richiestoTestingxUnitStandard de facto per progetti .NET

Architettura

UrlShortener/
├── Controllers/     → endpoint HTTP, solo orchestrazione
├── Models/          → entità e DTO, nessuna logica
├── Data/            → configurazione EF Core (DbContext, indici)
├── Services/        → logica di business pura (Base62 encoding)
├── Program.cs       → configurazione app e dependency injection
└── appsettings.json → configurazione (connection string, ecc.)

UrlShortener.Tests/
└── Services/        → test unitari su Base62Encoder

La struttura segue il principio di Separation of Concerns: ogni cartella ha una responsabilità isolata, così un cambiamento in un'area (es. cambio di database) ha impatto minimo sul resto del sistema (Single Responsibility Principle).

Design del sistema

Generazione del codice corto

Ogni nuovo URL riceve un ID auto-incrementale dal database, convertito in Base62 (alfabeto 0-9a-zA-Z, 62 simboli). Rispetto a un hash (MD5/SHA troncato), questo approccio:


non genera collisioni
è deterministico e facilmente invertibile (Decode)


Percorso di lettura (redirect) vs scrittura

Il sistema assume un rapporto letture:scritture molto sbilanciato verso le letture (chi clicca un link è molto più frequente di chi ne crea uno nuovo). Questo guida due scelte:


Indice unico sulla colonna ShortCode, per rendere la query di redirect quasi istantanea anche con molte righe
Cache-aside con IMemoryCache: al primo accesso il dato viene letto dal DB e messo in cache (24h); le richieste successive per lo stesso codice non toccano più il database


Nota sulla scalabilità

Il contatore auto-incrementante centralizzato funziona bene in locale/single-server, ma in un sistema distribuito multi-istanza diventerebbe un collo di bottiglia. Un'evoluzione naturale sarebbe l'allocazione di range di ID per server (es. server A: 1-1000, server B: 1001-2000).

Allo stesso modo, l'incremento sincrono di ClickCount ad ogni redirect è un punto di ottimizzazione futura: su alto traffico converrebbe accumulare i click in memoria/coda e scriverli sul DB in batch, invece che ad ogni singola richiesta.

Come avviare il progetto in locale

Prerequisiti


.NET 8 SDK
Visual Studio 2022 (o un IDE equivalente)


Setup


Clona il repository
Apri UrlShortener.sln in Visual Studio
Ripristina il database eseguendo, dalla Package Manager Console:


   Update-Database

(in alternativa, da terminale: dotnet ef database update)
4. Avvia il progetto (F5) — si apre Swagger UI

Provarlo

Creare un link corto:

POST /api/shorten
Content-Type: application/json

{ "url": "https://www.google.com" }

Risposta:

json{ "shortUrl": "https://localhost:7123/w7e" }

Usarlo: apri lo shortUrl ricevuto nel browser → verrai reindirizzato all'URL originale.

Testing

Il progetto include test xUnit su Base62Encoder:


comportamento sui casi limite (es. input 0)
valori noti di encoding
proprietà round-trip (Decode(Encode(x)) == x per ogni x)


Per eseguirli: Test → Test Explorer → Run All Tests in Visual Studio, oppure da terminale:

dotnet test
