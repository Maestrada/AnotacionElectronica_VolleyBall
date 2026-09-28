# Anotación Electrónica de Voleibol — León VB

Sistema para administrar competiciones de voleibol y generar un acta electrónica auditable. Está pensado como una API ASP.NET Core consumible desde web, Android o aplicaciones de escritorio; el cliente web incluido sirve como mesa de prueba y administración inicial.

La hoja oficial FIVB se utiliza como referencia visual y funcional. Los datos del partido se almacenan como eventos y proyecciones propias, no como campos dentro de un PDF.

## Capacidades actuales

- Partido en vivo: creación directa o desde calendario, inicio de sets, puntos, deshacer, cierre confirmado de set y de partido.
- Set decisivo: solicitud y confirmación del cambio de cancha al punto configurado (por defecto, 8).
- Bitácora de eventos: puntos, inicio, confirmaciones y cambios de cancha permanecen auditables.
- Catálogos: equipos, jugadores, árbitros, competiciones, reglamentos, fases, recintos, designaciones arbitrales y categorías editables.
- Calendario: programación independiente de juegos y conversión opcional a partido.
- PDF: exportación del acta final en PDF desde los datos propios del partido.
- Frontend: dashboard, marcador, catálogos y configuración visible de la URL de API.

## Arquitectura

| Proyecto | Responsabilidad |
| --- | --- |
| `Domain` | Entidades y reglas de voleibol. |
| `Application` | Casos de uso, DTOs e interfaces. |
| `Infrastructure` | EF Core, SQL Server y migraciones. |
| `API` | REST, SignalR, PDF y configuración de hospedaje. |
| `frontend/...web` | React + Vite para probar la API. |

La API expone SignalR en `/hubs/partidos`. Swagger está disponible en `Development`.

## Puertos fijos por ambiente

Los puertos se definen en archivos versionados de configuración. El frontend lee su URL de API desde `VITE_API_BASE_URL` y además permite modificarla temporalmente desde la interfaz; esa preferencia se guarda en el navegador.

| Ambiente | API HTTP | API HTTPS | Frontend | Configuración API | Configuración web |
| --- | ---: | ---: | ---: | --- | --- |
| Desarrollo | 60153 | 60152 | 5173 | `appsettings.Development.json` | `.env.development` |
| QA | 60253 | — | 5174 | `appsettings.QA.json` | `.env.qa` |
| Producción | 60353 | — | 5175 | `appsettings.Production.json` | `.env.production` |

En QA y Producción se recomienda un proxy inverso que termine HTTPS. El puerto indicado para API continúa siendo el puerto interno fijo de la aplicación.

## Registro físico de aplicación

La API genera archivos diarios en `src/AnotacionElectronica_Leon_VB.API/logs/` con el formato `aplicacion-AAAA-MM-DD.log`. Incluyen solicitudes HTTP, acciones técnicas y excepciones no controladas; no se almacenan cuerpos de petición ni contraseñas.

Todos los servicios de aplicación (`Partido`, `Calendario`, `Equipo`, `Árbitro`, `Competición` y `Reglamento`) pasan por un interceptor centralizado: se registra el inicio, la finalización con duración y cualquier excepción de cada método público. No se registran argumentos ni datos sensibles.

La sección `FileLogging` de `appsettings.json` permite configurar la carpeta, el nivel mínimo y los días de retención. Por defecto se conserva información y errores durante 30 días. Los archivos `.log` se excluyen de Git; la carpeta se mantiene en el proyecto mediante `.gitkeep`.

## Ejecutar en Desarrollo

Requisitos: .NET SDK 9 instalado en el entorno actual, SQL Server y Node.js. El objetivo de despliegue puede migrarse a .NET 8 LTS cuando esté disponible el SDK correspondiente.

```powershell
dotnet restore
dotnet ef database update --project src/AnotacionElectronica_Leon_VB.Infraestructure --startup-project src/AnotacionElectronica_Leon_VB.API
dotnet run --project src/AnotacionElectronica_Leon_VB.API --environment Development
```

En otra terminal:

```powershell
cd frontend/anotacion-electronica-leon-volleyball-web
npm install
npm run dev
```

Abre `http://127.0.0.1:5173`. La interfaz usará `http://localhost:60153/api` automáticamente.

## Ejecutar QA o Producción local

```powershell
# API QA / frontend QA
dotnet run --project src/AnotacionElectronica_Leon_VB.API --environment QA
cd frontend/anotacion-electronica-leon-volleyball-web
npm run dev:qa

# API Producción / compilación web de producción
dotnet run --project src/AnotacionElectronica_Leon_VB.API --environment Production
npm run build:prd
```

Para modificar una dirección sin tocar código, actualiza `Hosting:Urls` en `appsettings.{Ambiente}.json` y `VITE_API_BASE_URL` / `VITE_FRONTEND_PORT` en el `.env` correspondiente. Las variables `VITE_*` se incorporan durante la compilación web; se debe reconstruir el frontend después de cambiarlas.

## Flujo básico del partido

1. Crear un partido: `POST /api/partidos`, o crearlo desde `POST /api/calendario/juegos/{id}/crear-partido`.
2. Iniciar: `POST /api/partidos/{id}/iniciar`.
3. Registrar un rally: `POST /api/anotacion/puntos/registrar`.
4. Confirmar el cambio de cancha: `POST /api/anotacion/partidos/{id}/sets/confirmar-cambio-cancha`.
5. Confirmar el cierre de set: `POST /api/anotacion/partidos/{id}/sets/confirmar-cierre`.
6. Corregir el último punto antes del cierre: `POST /api/anotacion/partidos/{id}/deshacer`.
7. Consultar acta y eventos: `GET /api/partidos/{id}/acta` y `GET /api/anotacion/partidos/{id}/eventos`.
8. Con el partido finalizado, descargar su acta: `GET /api/partidos/{id}/exportaciones/acta.pdf`.

Ejemplo de rally:

```json
{
  "partidoId": "GUID",
  "equipoAnotadorId": "GUID",
  "tipoAccion": "Ataque",
  "jugadorAnotadorId": "GUID"
}
```

## Modelo reglamentario e histórico

Cada partido conserva una instantánea del perfil reglamentario: sets máximos, sets para ganar, puntos de set regular/decisivo, diferencia mínima y punto de cambio de cancha. Por ello los cambios futuros de reglamento o de categorías no alteran un partido histórico ni su reproducción.

Las categorías se administran mediante `/api/categorias`. Al desactivarlas se preserva el historial de equipos y competiciones ya registradas.

## Usuarios, roles, permisos y bitácora

La migración `AgregarSeguridadYBitacora` crea `UsuariosSistema`, `RolesSistema`, `PermisosSistema`, las relaciones muchos-a-muchos y `BitacoraAcciones`. Las contraseñas se almacenan únicamente como hash mediante `PasswordHasher`; nunca se devuelven por la API.

Roles iniciales: `Administrador`, `Organizador`, `Anotador` y `Consulta`. Los permisos incluyen gestión de seguridad, competiciones, calendario, catálogos, anotación, cierre, exportación de actas y consulta de bitácora.

La administración se expone en `/api/seguridad`:

- `GET`/`POST`/`PUT /usuarios` y `PUT /usuarios/{id}/contrasena`.
- `GET`/`POST`/`PUT /roles`.
- `GET`/`POST /permisos`.
- `GET /bitacora?desde=...&hasta=...&usuarioId=...`.

Toda operación `POST`, `PUT`, `PATCH` o `DELETE` queda registrada automáticamente. Mientras se incorpora autenticación JWT, un cliente autorizado puede enviar `X-Usuario-Id: GUID` y opcionalmente `X-Usuario-Nombre` para atribuir su acción; sin esa información queda registrada como `Sistema`.

Un usuario puede vincularse opcionalmente a un árbitro mediante `PUT /api/seguridad/usuarios/{id}/arbitro` con `{ "arbitroId": "GUID" }` (o `null` para desvincularlo). La relación es uno-a-uno: ni el árbitro ni el usuario son obligatorios para el otro. El rol de arbitraje y los roles de seguridad siguen separados deliberadamente; posteriormente se puede definir qué permisos recibe cada tipo de árbitro sin modificar su historial deportivo.

## Validación

```powershell
dotnet build AnotacionElectronica_VolleyBall.sln --no-restore
dotnet test AnotacionElectronica_VolleyBall.sln --no-build --no-restore

cd frontend/anotacion-electronica-leon-volleyball-web
npm run lint
npm run build
```
