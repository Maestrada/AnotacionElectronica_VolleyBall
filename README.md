# Anotación electrónica de voleibol

API para producir un acta electrónica auditable de voleibol. El PDF FIVB indicado por el usuario se usa como referencia visual y funcional; los datos se almacenan como eventos y proyecciones, no como una copia de la planilla.

## Arquitectura

`API ASP.NET Core` expone REST y SignalR. `Application` contiene los casos de uso; `Domain` contiene las reglas del partido y `Infrastructure` persiste con EF Core para SQL Server. Cualquier cliente (web con jQuery, Android o Windows) consume la misma API y se puede suscribir a `/hubs/partidos` para el marcador en vivo.

## Flujo implementado

1. Crear partido: `POST /api/partidos`.
2. Iniciarlo: `POST /api/partidos/{id}/iniciar`; se crea el primer set.
3. Registrar cada punto: `POST /api/anotacion/puntos/registrar`.
4. Al llegar a 25 puntos (15 en el quinto), siempre con dos puntos de diferencia, el set queda pendiente de confirmación. `POST /api/anotacion/partidos/{id}/sets/confirmar-cierre` lo cierra y actualiza el partido.
5. Si hay un error antes de confirmar, `POST /api/anotacion/partidos/{id}/deshacer` revierte el último punto.
6. Iniciar el siguiente set confirmado: `POST /api/partidos/{id}/sets/{numeroSet}/iniciar`.
7. Consultar la proyección del acta: `GET /api/partidos/{id}/acta` y su bitácora: `GET /api/anotacion/partidos/{id}/eventos`.

## Bitácora de eventos

La tabla `EventosPartido` guarda eventos inmutables y secuenciales, con tipo, set, fecha UTC y carga JSON. Actualmente se registran inicio de partido/set, punto, deshacer, set pendiente de confirmación y set confirmado. El marcador y los sets son la proyección de trabajo; la bitácora permite auditoría y será la fuente para reconstruir el acta y agregar tiempos, cambios, tarjetas, incidencias y protestas.

## Reglamentos y sets decisivos

Cada partido contiene una **instantánea** del reglamento: código y versión, máximo de sets, sets necesarios para ganar, puntos de sets regulares y decisivo, diferencia mínima y punto de cambio de cancha. La configuración por defecto es `FIVB-2025-2028` (mejor de cinco), pero `POST /api/partidos` acepta `reglamento` para crear, por ejemplo, fase regular de León: máximo 3 / ganar 2. Los cambios posteriores de reglas no modifican esa instantánea, por lo que el acta y su reproducción histórica permanecen correctas.

En el set decisivo (set 3 de mejor de 3 o set 5 de mejor de 5), al alcanzar uno de los equipos el punto 8, la API deja el set en `pendienteCambioCancha`. No permite un punto posterior hasta llamar `POST /api/anotacion/partidos/{id}/sets/confirmar-cambio-cancha`. La rotación y el marcador no se alteran; se registra el evento de aviso y confirmación.

La siguiente capa de catálogo debe asociar la instantánea a `Competición → Edición → Fase`, y registrar recintos, oficiales, plantillas y designaciones. Un partido siempre conservará además los identificadores y nombres usados en su acta, para que la reproducción no dependa de datos maestros modificados.

## Calendario de juegos

El calendario es una pre-carga independiente. `POST /api/calendario/juegos` programa un juego con código, equipos, fecha/hora, recinto, competición, edición, fase y perfil reglamentario. `GET /api/calendario?desde=...&hasta=...` lo consulta y `POST /api/calendario/juegos/{id}/crear-partido` crea el partido desde esa programación, conservando su reglamento y marcando el juego como convertido.

`POST /api/partidos` sigue disponible para partidos extraordinarios o no planificados; no requiere un juego de calendario.

Ejemplo de punto:

```json
{
  "partidoId": "GUID",
  "equipoAnotadorId": "GUID",
  "tipoAccion": "Ataque",
  "jugadorAnotadorId": "GUID"
}
```

## Próximos módulos del acta

- Eventos inmutables y reversibles: tiempo muerto, sustitución, entrada/salida de líbero, sanción, expulsión, incidencia y protesta.
- Alineación y orden de saque por set, con rotación automática al recuperar el saque.
- Validaciones reglamentarias configurables y confirmaciones de cierre firmadas por anotador, árbitros y capitanes.
- Generador de PDF de la hoja final, diseñado desde datos propios y con revisión de licenciamiento/uso de la plantilla FIVB.
- Autenticación, roles y auditoría antes de uso oficial.

## Requisito de plataforma

La solución existente compila actualmente en .NET 9 porque solo ese SDK está instalado en el entorno. El objetivo de despliegue debe ser .NET 8 LTS: al instalar el SDK/targeting pack 8, se cambiarán los `TargetFramework` y las dependencias de EF Core a la serie 8.
