import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import './App.css'

const defaultApiUrl = 'http://localhost:60153/api'

// --- Tipos de Datos ---
export type PerfilReglamento = {
  id?: string
  codigoReglamento: string
  nombre: string
  descripcion?: string
  maximoSets: number
  setsParaGanar: number
  puntosSetRegular: number
  puntosSetDecisivo: number
  diferenciaMinima: number
  puntoCambioCanchaSetDecisivo: number
}

export type Jugador = {
  id: string
  nombre: string
  apellidos: string
  numeroCamiseta: number
  posicion: number
  posicionTexto: string
  esCapitan: boolean
  equipoId: string
  nombreEquipo?: string
}

export type Equipo = {
  id: string
  nombre: string
  nombreEntrenador: string
  nombreAsistente?: string
  categoria: string
  totalJugadores: number
  jugadores: Jugador[]
}

export type Arbitro = {
  id: string
  nombre: string
  apellidos: string
  nombreCompleto: string
  rol: number
  rolTexto: string
  numeroLicencia?: string
  federacion?: string
}

export type Competicion = {
  id: string
  nombre: string
  edicion: string
  categoria: string
  rama: string
  organizador?: string
  sedePrincipal?: string
}

export type JuegoCalendario = {
  id: string
  codigo: string
  competicion?: string
  edicion?: string
  fase?: string
  equipoLocalId: string
  equipoVisitanteId: string
  fechaHoraProgramada: string
  recinto: string
  estado: string
  partidoId?: string
  reglamento: PerfilReglamento
}

export type Partido = {
  id: string
  equipoLocalId: string
  equipoVisitanteId: string
  fechaProgramada: string
  lugar: string
  setsGanadosLocal: number
  setsGanadosVisitante: number
  finalizado: boolean
  equipoGanadorId?: string
  reglamento: PerfilReglamento
}

export type SetActual = {
  id: string
  partidoId: string
  numeroSet: number
  puntosLocal: number
  puntosVisitante: number
  finalizado: boolean
  pendienteConfirmacionCierre: boolean
  pendienteCambioCancha: boolean
  cambioCanchaConfirmado: boolean
  equipoGanadorId?: string
}

type TabType = 'partidos' | 'calendario' | 'equipos' | 'jugadores' | 'arbitros' | 'reglamentos' | 'competiciones'

async function request<T>(apiUrl: string, path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${apiUrl}${path}`, {
    headers: { 'Content-Type': 'application/json', ...options?.headers },
    ...options
  })
  if (!response.ok) {
    const body = (await response.json().catch(() => null)) as { mensaje?: string } | null
    throw new Error(body?.mensaje ?? `Error HTTP ${response.status}`)
  }
  return response.json() as Promise<T>
}

export default function App() {
  const [apiUrl, setApiUrl] = useState(localStorage.getItem('apiUrl') ?? defaultApiUrl)
  const [tabActiva, setTabActiva] = useState<TabType>('partidos')
  const [mensaje, setMensaje] = useState<{ texto: string; tipo: 'info' | 'exito' | 'error' }>({
    texto: 'Sistema de anotación listo. Conecta la API o administra los catálogos.',
    tipo: 'info'
  })

  // Datos maestros
  const [equipos, setEquipos] = useState<Equipo[]>([])
  const [jugadores, setJugadores] = useState<Jugador[]>([])
  const [arbitros, setArbitros] = useState<Arbitro[]>([])
  const [competiciones, setCompeticiones] = useState<Competicion[]>([])
  const [reglamentos, setReglamentos] = useState<PerfilReglamento[]>([])
  const [juegosCalendario, setJuegosCalendario] = useState<JuegoCalendario[]>([])

  // Estado del Partido y Marcador
  const [partidoActual, setPartidoActual] = useState<Partido | null>(null)
  const [setActual, setSetActual] = useState<SetActual | null>(null)
  const [accionPunto, setAccionPunto] = useState<'Ataque' | 'Bloqueo' | 'Saque' | 'Error'>('Ataque')
  const [jugadorAnotadorId, setJugadorAnotadorId] = useState<string>('')

  // Formularios de Creación
  // 1. Partido Directo
  const [partidoLocalId, setPartidoLocalId] = useState('')
  const [partidoVisitanteId, setPartidoVisitanteId] = useState('')
  const [partidoLugar, setPartidoLugar] = useState('Polideportivo León')
  const [partidoReglamentoCodigo, setPartidoReglamentoCodigo] = useState('')

  // 2. Equipo
  const [formEquipoNombre, setFormEquipoNombre] = useState('')
  const [formEquipoEntrenador, setFormEquipoEntrenador] = useState('')
  const [formEquipoAsistente, setFormEquipoAsistente] = useState('')
  const [formEquipoCategoria, setFormEquipoCategoria] = useState('Mayor')

  // 3. Jugador
  const [formJugadorEquipoId, setFormJugadorEquipoId] = useState('')
  const [formJugadorNombre, setFormJugadorNombre] = useState('')
  const [formJugadorApellidos, setFormJugadorApellidos] = useState('')
  const [formJugadorCamiseta, setFormJugadorCamiseta] = useState<number | ''>('')
  const [formJugadorPosicion, setFormJugadorPosicion] = useState<number>(2) // 2: Rematador/Punta
  const [formJugadorCapitan, setFormJugadorCapitan] = useState(false)
  const [filtroEquipoJugadores, setFiltroEquipoJugadores] = useState('')

  // 4. Árbitro
  const [formArbitroNombre, setFormArbitroNombre] = useState('')
  const [formArbitroApellidos, setFormArbitroApellidos] = useState('')
  const [formArbitroRol, setFormArbitroRol] = useState<number>(1) // 1: Primer Árbitro
  const [formArbitroLicencia, setFormArbitroLicencia] = useState('')
  const [formArbitroFederacion, setFormArbitroFederacion] = useState('Asociación Estatal de Voleibol de Guanajuato')

  // 5. Reglamento
  const [formRegCodigo, setFormRegCodigo] = useState('')
  const [formRegNombre, setFormRegNombre] = useState('')
  const [formRegDesc, setFormRegDesc] = useState('')
  const [formRegMaxSets, setFormRegMaxSets] = useState(5)
  const [formRegSetsGanar, setFormRegSetsGanar] = useState(3)
  const [formRegPtsRegular, setFormRegPtsRegular] = useState(25)
  const [formRegPtsDecisivo, setFormRegPtsDecisivo] = useState(15)
  const [formRegDifMin, setFormRegDifMin] = useState(2)
  const [formRegPtsCambio, setFormRegPtsCambio] = useState(8)

  // 6. Competición
  const [formCompNombre, setFormCompNombre] = useState('')
  const [formCompEdicion, setFormCompEdicion] = useState('Temporada 2026')
  const [formCompCategoria, setFormCompCategoria] = useState('Mayor')
  const [formCompRama, setFormCompRama] = useState('Femenil')
  const [formCompOrganizador, setFormCompOrganizador] = useState('Comisión Municipal del Deporte')
  const [formCompSede, setFormCompSede] = useState('Polideportivo León')

  // 7. Calendario
  const [formCalCodigo, setFormCalCodigo] = useState('')
  const [formCalCompId, setFormCalCompId] = useState('')
  const [formCalFase, setFormCalFase] = useState('Fase Regular - Jornada 1')
  const [formCalLocalId, setFormCalLocalId] = useState('')
  const [formCalVisitanteId, setFormCalVisitanteId] = useState('')
  const [formCalFechaHora, setFormCalFechaHora] = useState(new Date().toISOString().slice(0, 16))
  const [formCalRecinto, setFormCalRecinto] = useState('Cancha Central Polideportivo')
  const [formCalRegCodigo, setFormCalRegCodigo] = useState('')

  // Carga general de datos
  const cargarTodosLosDatos = async () => {
    try {
      const [eqs, jugs, arbs, comps, regs, cals] = await Promise.all([
        request<Equipo[]>(apiUrl, '/equipos').catch(() => []),
        request<Jugador[]>(apiUrl, '/jugadores').catch(() => []),
        request<Arbitro[]>(apiUrl, '/arbitros').catch(() => []),
        request<Competicion[]>(apiUrl, '/competiciones').catch(() => []),
        request<PerfilReglamento[]>(apiUrl, '/reglamentos').catch(() => []),
        request<JuegoCalendario[]>(apiUrl, '/calendario').catch(() => [])
      ])

      setEquipos(eqs)
      setJugadores(jugs)
      setArbitros(arbs)
      setCompeticiones(comps)
      setReglamentos(regs)
      setJuegosCalendario(cals)

      if (eqs.length > 0 && !formJugadorEquipoId) setFormJugadorEquipoId(eqs[0].id)
      if (eqs.length > 0 && !partidoLocalId) setPartidoLocalId(eqs[0].id)
      if (eqs.length > 1 && !partidoVisitanteId) setPartidoVisitanteId(eqs[1].id)
      if (eqs.length > 0 && !formCalLocalId) setFormCalLocalId(eqs[0].id)
      if (eqs.length > 1 && !formCalVisitanteId) setFormCalVisitanteId(eqs[1].id)
      if (comps.length > 0 && !formCalCompId) setFormCalCompId(comps[0].id)
      if (regs.length > 0 && !partidoReglamentoCodigo) setPartidoReglamentoCodigo(regs[0].codigoReglamento)
      if (regs.length > 0 && !formCalRegCodigo) setFormCalRegCodigo(regs[0].codigoReglamento)
    } catch (err) {
      setMensaje({ texto: (err as Error).message, tipo: 'error' })
    }
  }

  useEffect(() => {
    void cargarTodosLosDatos()
  }, [apiUrl])

  const guardarApiUrl = (e: FormEvent) => {
    e.preventDefault()
    localStorage.setItem('apiUrl', apiUrl)
    setMensaje({ texto: 'URL de API actualizada y reconectando...', tipo: 'exito' })
    void cargarTodosLosDatos()
  }

  // --- Handlers de Creación ---

  // 1. Guardar Equipo
  const handleSubmitEquipo = async (e: FormEvent) => {
    e.preventDefault()
    try {
      const nuevo = await request<Equipo>(apiUrl, '/equipos', {
        method: 'POST',
        body: JSON.stringify({
          nombre: formEquipoNombre,
          nombreEntrenador: formEquipoEntrenador,
          nombreAsistente: formEquipoAsistente || null,
          categoria: formEquipoCategoria
        })
      })
      setMensaje({ texto: `¡Equipo '${nuevo.nombre}' registrado con éxito!`, tipo: 'exito' })
      setFormEquipoNombre('')
      setFormEquipoEntrenador('')
      setFormEquipoAsistente('')
      void cargarTodosLosDatos()
    } catch (err) {
      setMensaje({ texto: (err as Error).message, tipo: 'error' })
    }
  }

  // 2. Guardar Jugador
  const handleSubmitJugador = async (e: FormEvent) => {
    e.preventDefault()
    if (!formJugadorEquipoId) {
      setMensaje({ texto: 'Debes seleccionar un equipo para el jugador.', tipo: 'error' })
      return
    }
    try {
      const nuevo = await request<Jugador>(apiUrl, '/jugadores', {
        method: 'POST',
        body: JSON.stringify({
          nombre: formJugadorNombre,
          apellidos: formJugadorApellidos,
          numeroCamiseta: Number(formJugadorCamiseta),
          posicion: Number(formJugadorPosicion),
          esCapitan: formJugadorCapitan,
          equipoId: formJugadorEquipoId
        })
      })
      setMensaje({ texto: `¡Jugador #${nuevo.numeroCamiseta} ${nuevo.nombre} ${nuevo.apellidos} registrado con éxito!`, tipo: 'exito' })
      setFormJugadorNombre('')
      setFormJugadorApellidos('')
      setFormJugadorCamiseta('')
      setFormJugadorCapitan(false)
      void cargarTodosLosDatos()
    } catch (err) {
      setMensaje({ texto: (err as Error).message, tipo: 'error' })
    }
  }

  // 3. Guardar Árbitro
  const handleSubmitArbitro = async (e: FormEvent) => {
    e.preventDefault()
    try {
      const nuevo = await request<Arbitro>(apiUrl, '/arbitros', {
        method: 'POST',
        body: JSON.stringify({
          nombre: formArbitroNombre,
          apellidos: formArbitroApellidos,
          rol: Number(formArbitroRol),
          numeroLicencia: formArbitroLicencia || null,
          federacion: formArbitroFederacion || null
        })
      })
      setMensaje({ texto: `¡Oficial ${nuevo.nombreCompleto} (${nuevo.rolTexto}) registrado con éxito!`, tipo: 'exito' })
      setFormArbitroNombre('')
      setFormArbitroApellidos('')
      setFormArbitroLicencia('')
      void cargarTodosLosDatos()
    } catch (err) {
      setMensaje({ texto: (err as Error).message, tipo: 'error' })
    }
  }

  // 4. Guardar Reglamento
  const handleSubmitReglamento = async (e: FormEvent) => {
    e.preventDefault()
    try {
      const nuevo = await request<PerfilReglamento>(apiUrl, '/reglamentos', {
        method: 'POST',
        body: JSON.stringify({
          codigoReglamento: formRegCodigo,
          nombre: formRegNombre,
          descripcion: formRegDesc || null,
          maximoSets: Number(formRegMaxSets),
          setsParaGanar: Number(formRegSetsGanar),
          puntosSetRegular: Number(formRegPtsRegular),
          puntosSetDecisivo: Number(formRegPtsDecisivo),
          diferenciaMinima: Number(formRegDifMin),
          puntoCambioCanchaSetDecisivo: Number(formRegPtsCambio)
        })
      })
      setMensaje({ texto: `¡Reglamento '${nuevo.nombre}' (${nuevo.codigoReglamento}) registrado con éxito!`, tipo: 'exito' })
      setFormRegCodigo('')
      setFormRegNombre('')
      setFormRegDesc('')
      void cargarTodosLosDatos()
    } catch (err) {
      setMensaje({ texto: (err as Error).message, tipo: 'error' })
    }
  }

  // Preset helper para reglamento
  const aplicarPresetReglamento = (tipo: 'FIVB' | 'LEON' | 'RAPIDO') => {
    if (tipo === 'FIVB') {
      setFormRegCodigo('FIVB-OFICIAL-5SETS')
      setFormRegNombre('FIVB Oficial (Mejor de 5 Sets)')
      setFormRegDesc('Reglamento internacional oficial. Sets regulares a 25 puntos y set 5 a 15 puntos.')
      setFormRegMaxSets(5)
      setFormRegSetsGanar(3)
      setFormRegPtsRegular(25)
      setFormRegPtsDecisivo(15)
      setFormRegDifMin(2)
      setFormRegPtsCambio(8)
    } else if (tipo === 'LEON') {
      setFormRegCodigo('LEON-REGULAR-3SETS')
      setFormRegNombre('Liga León Regular (Mejor de 3 Sets)')
      setFormRegDesc('Formato de fase regular de liga local: gana 2 de 3 sets (25 pts reg / 15 dec).')
      setFormRegMaxSets(3)
      setFormRegSetsGanar(2)
      setFormRegPtsRegular(25)
      setFormRegPtsDecisivo(15)
      setFormRegDifMin(2)
      setFormRegPtsCambio(8)
    } else if (tipo === 'RAPIDO') {
      setFormRegCodigo('TORNEO-RAPIDO-21PTS')
      setFormRegNombre('Torneo Rápido Relámpago (Sets a 21 pts)')
      setFormRegDesc('Sets regulares a 21 puntos para agilizar torneos de un solo día.')
      setFormRegMaxSets(3)
      setFormRegSetsGanar(2)
      setFormRegPtsRegular(21)
      setFormRegPtsDecisivo(15)
      setFormRegDifMin(2)
      setFormRegPtsCambio(8)
    }
  }

  // 5. Guardar Competición
  const handleSubmitCompeticion = async (e: FormEvent) => {
    e.preventDefault()
    try {
      const nuevo = await request<Competicion>(apiUrl, '/competiciones', {
        method: 'POST',
        body: JSON.stringify({
          nombre: formCompNombre,
          edicion: formCompEdicion,
          categoria: formCompCategoria,
          rama: formCompRama,
          organizador: formCompOrganizador || null,
          sedePrincipal: formCompSede || null
        })
      })
      setMensaje({ texto: `¡Competición '${nuevo.nombre} (${nuevo.edicion})' registrada con éxito!`, tipo: 'exito' })
      setFormCompNombre('')
      void cargarTodosLosDatos()
    } catch (err) {
      setMensaje({ texto: (err as Error).message, tipo: 'error' })
    }
  }

  // 6. Guardar Juego en Calendario
  const handleSubmitCalendario = async (e: FormEvent) => {
    e.preventDefault()
    if (formCalLocalId === formCalVisitanteId) {
      setMensaje({ texto: 'El equipo local y el equipo visitante no pueden ser el mismo.', tipo: 'error' })
      return
    }

    const regSeleccionado = reglamentos.find(r => r.codigoReglamento === formCalRegCodigo)
    const compSeleccionada = competiciones.find(c => c.id === formCalCompId)

    try {
      const nuevo = await request<JuegoCalendario>(apiUrl, '/calendario/juegos', {
        method: 'POST',
        body: JSON.stringify({
          codigo: formCalCodigo || `JUEGO-${Date.now().toString().slice(-4)}`,
          equipoLocalId: formCalLocalId,
          equipoVisitanteId: formCalVisitanteId,
          fechaHoraProgramada: new Date(formCalFechaHora).toISOString(),
          recinto: formCalRecinto,
          competicion: compSeleccionada ? compSeleccionada.nombre : 'Liga León',
          edicion: compSeleccionada ? compSeleccionada.edicion : '2026',
          fase: formCalFase,
          reglamento: regSeleccionado
            ? {
                codigoReglamento: regSeleccionado.codigoReglamento,
                maximoSets: regSeleccionado.maximoSets,
                setsParaGanar: regSeleccionado.setsParaGanar,
                puntosSetRegular: regSeleccionado.puntosSetRegular,
                puntosSetDecisivo: regSeleccionado.puntosSetDecisivo,
                diferenciaMinima: regSeleccionado.diferenciaMinima,
                puntoCambioCanchaSetDecisivo: regSeleccionado.puntoCambioCanchaSetDecisivo
              }
            : null
        })
      })
      setMensaje({ texto: `¡Juego programado '${nuevo.codigo}' guardado en el calendario!`, tipo: 'exito' })
      setFormCalCodigo('')
      void cargarTodosLosDatos()
    } catch (err) {
      setMensaje({ texto: (err as Error).message, tipo: 'error' })
    }
  }

  // 7. Crear e Iniciar Partido
  const handleCrearPartido = async (e: FormEvent) => {
    e.preventDefault()
    if (partidoLocalId === partidoVisitanteId) {
      setMensaje({ texto: 'El equipo local y visitante no pueden ser el mismo.', tipo: 'error' })
      return
    }

    const reg = reglamentos.find(r => r.codigoReglamento === partidoReglamentoCodigo)

    try {
      const nuevo = await request<Partido>(apiUrl, '/partidos', {
        method: 'POST',
        body: JSON.stringify({
          equipoLocalId: partidoLocalId,
          equipoVisitanteId: partidoVisitanteId,
          fechaProgramada: new Date().toISOString(),
          lugar: partidoLugar,
          reglamento: reg
            ? {
                codigoReglamento: reg.codigoReglamento,
                maximoSets: reg.maximoSets,
                setsParaGanar: reg.setsParaGanar,
                puntosSetRegular: reg.puntosSetRegular,
                puntosSetDecisivo: reg.puntosSetDecisivo,
                diferenciaMinima: reg.diferenciaMinima,
                puntoCambioCanchaSetDecisivo: reg.puntoCambioCanchaSetDecisivo
              }
            : null
        })
      })
      setPartidoActual(nuevo)
      setSetActual(null)
      setMensaje({ texto: 'Partido creado. Haz clic en "Iniciar Partido" para abrir el Set 1.', tipo: 'exito' })
    } catch (err) {
      setMensaje({ texto: (err as Error).message, tipo: 'error' })
    }
  }

  const handleCrearDesdeCalendario = async (juegoId: string) => {
    try {
      const nuevo = await request<Partido>(apiUrl, `/calendario/juegos/${juegoId}/crear-partido`, { method: 'POST' })
      setPartidoActual(nuevo)
      setSetActual(null)
      setTabActiva('partidos')
      setMensaje({ texto: 'Partido creado desde el calendario. ¡Listo para iniciar!', tipo: 'exito' })
      void cargarTodosLosDatos()
    } catch (err) {
      setMensaje({ texto: (err as Error).message, tipo: 'error' })
    }
  }

  const iniciarPartido = async () => {
    if (!partidoActual) return
    try {
      await request<unknown>(apiUrl, `/partidos/${partidoActual.id}/iniciar`, { method: 'POST' })
      await recargarPartido(partidoActual.id)
      setMensaje({ texto: '¡Partido y Set 1 iniciados con éxito!', tipo: 'exito' })
    } catch (err) {
      setMensaje({ texto: (err as Error).message, tipo: 'error' })
    }
  }

  const recargarPartido = async (id: string) => {
    const detalle = await request<Partido>(apiUrl, `/partidos/${id}`)
    const acta = await request<{ sets: SetActual[] }>(apiUrl, `/partidos/${id}/acta`)
    setPartidoActual(detalle)
    setSetActual(acta.sets.at(-1) ?? null)
  }

  const anotarPunto = async (equipoId: string) => {
    if (!partidoActual) return
    try {
      await request<unknown>(apiUrl, '/anotacion/puntos/registrar', {
        method: 'POST',
        body: JSON.stringify({
          partidoId: partidoActual.id,
          equipoAnotadorId: equipoId,
          tipoAccion: accionPunto,
          jugadorAnotadorId: jugadorAnotadorId || null
        })
      })
      await recargarPartido(partidoActual.id)
      setJugadorAnotadorId('')
    } catch (err) {
      setMensaje({ texto: (err as Error).message, tipo: 'error' })
    }
  }

  const ejecutarAccionPartido = async (path: string, mensajeExito: string) => {
    if (!partidoActual) return
    try {
      await request<unknown>(apiUrl, path, { method: 'POST' })
      await recargarPartido(partidoActual.id)
      setMensaje({ texto: mensajeExito, tipo: 'exito' })
    } catch (err) {
      setMensaje({ texto: (err as Error).message, tipo: 'error' })
    }
  }

  // Nombres de equipos actuales en partido
  const equipoLocalInfo = equipos.find(e => e.id === partidoActual?.equipoLocalId)
  const equipoVisitanteInfo = equipos.find(e => e.id === partidoActual?.equipoVisitanteId)

  return (
    <main className="app-container">
      {/* Header Principal */}
      <header className="main-header">
        <div className="header-title">
          <span className="eyebrow">SISTEMA OFICIAL DE ANOTACIÓN ELECTRÓNICA</span>
          <h1>Voleibol Sala · León VB</h1>
        </div>
        <div className="header-actions">
          <span className={`status-badge ${partidoActual ? 'active' : ''}`}>
            {partidoActual ? (partidoActual.finalizado ? '🏁 PARTIDO FINALIZADO' : '⚡ PARTIDO EN CURSO') : '⚪ MODO ADMINISTRACIÓN'}
          </span>
        </div>
      </header>

      {/* Barra de Notificación / Mensajes */}
      <div className={`notification-bar ${mensaje.tipo}`} role="status">
        <span className="notif-icon">
          {mensaje.tipo === 'exito' ? '✅' : mensaje.tipo === 'error' ? '❌' : 'ℹ️'}
        </span>
        <span className="notif-text">{mensaje.texto}</span>
      </div>

      {/* Barra de Configuración de API */}
      <section className="connection-bar panel">
        <form onSubmit={guardarApiUrl} className="connection-form">
          <label className="inline-label">
            <span>URL de Backend API:</span>
            <input
              type="text"
              value={apiUrl}
              onChange={e => setApiUrl(e.target.value)}
              placeholder="http://localhost:60153/api"
            />
          </label>
          <button type="submit" className="btn-secondary">Reconectar / Refrescar Datos</button>
        </form>
      </section>

      {/* Navegación por Pestañas */}
      <nav className="tabs-nav" aria-label="Secciones del sistema">
        <button
          className={`tab-btn ${tabActiva === 'partidos' ? 'active' : ''}`}
          onClick={() => setTabActiva('partidos')}
        >
          🏆 Marcador & Partido
        </button>
        <button
          className={`tab-btn ${tabActiva === 'calendario' ? 'active' : ''}`}
          onClick={() => setTabActiva('calendario')}
        >
          📅 Calendario ({juegosCalendario.length})
        </button>
        <button
          className={`tab-btn ${tabActiva === 'equipos' ? 'active' : ''}`}
          onClick={() => setTabActiva('equipos')}
        >
          🛡️ Equipos ({equipos.length})
        </button>
        <button
          className={`tab-btn ${tabActiva === 'jugadores' ? 'active' : ''}`}
          onClick={() => setTabActiva('jugadores')}
        >
          👤 Jugadores ({jugadores.length})
        </button>
        <button
          className={`tab-btn ${tabActiva === 'arbitros' ? 'active' : ''}`}
          onClick={() => setTabActiva('arbitros')}
        >
          ⚖️ Árbitros ({arbitros.length})
        </button>
        <button
          className={`tab-btn ${tabActiva === 'reglamentos' ? 'active' : ''}`}
          onClick={() => setTabActiva('reglamentos')}
        >
          📋 Reglamentos ({reglamentos.length})
        </button>
        <button
          className={`tab-btn ${tabActiva === 'competiciones' ? 'active' : ''}`}
          onClick={() => setTabActiva('competiciones')}
        >
          🏅 Competiciones ({competiciones.length})
        </button>
      </nav>

      {/* CONTENIDO DE PESTAÑAS */}

      {/* 1. TAB: PARTIDOS & MARCADOR */}
      {tabActiva === 'partidos' && (
        <section className="tab-content">
          {!partidoActual ? (
            <div className="grid-2-cols">
              <form className="panel form" onSubmit={handleCrearPartido}>
                <h2>Iniciar Nuevo Partido Directo</h2>
                <p className="form-subtitle">Selecciona los equipos y el reglamento para abrir la mesa de control.</p>

                <div className="form-row">
                  <label>
                    Equipo Local *
                    <select
                      required
                      value={partidoLocalId}
                      onChange={e => setPartidoLocalId(e.target.value)}
                    >
                      <option value="">-- Seleccionar Equipo Local --</option>
                      {equipos.map(eq => (
                        <option key={eq.id} value={eq.id}>{eq.nombre} ({eq.categoria})</option>
                      ))}
                    </select>
                  </label>

                  <label>
                    Equipo Visitante *
                    <select
                      required
                      value={partidoVisitanteId}
                      onChange={e => setPartidoVisitanteId(e.target.value)}
                    >
                      <option value="">-- Seleccionar Equipo Visitante --</option>
                      {equipos.map(eq => (
                        <option key={eq.id} value={eq.id}>{eq.nombre} ({eq.categoria})</option>
                      ))}
                    </select>
                  </label>
                </div>

                <div className="form-row">
                  <label>
                    Reglamento del Partido *
                    <select
                      value={partidoReglamentoCodigo}
                      onChange={e => setPartidoReglamentoCodigo(e.target.value)}
                    >
                      {reglamentos.map(reg => (
                        <option key={reg.codigoReglamento} value={reg.codigoReglamento}>
                          {reg.nombre} ({reg.codigoReglamento})
                        </option>
                      ))}
                    </select>
                  </label>

                  <label>
                    Sede / Recinto *
                    <input
                      type="text"
                      required
                      value={partidoLugar}
                      onChange={e => setPartidoLugar(e.target.value)}
                    />
                  </label>
                </div>

                {equipos.length < 2 && (
                  <p className="hint-warning">
                    ⚠️ Se necesitan al menos 2 equipos registrados. Crea equipos en la pestaña <strong>Equipos</strong>.
                  </p>
                )}

                <button type="submit" className="primary btn-large" disabled={equipos.length < 2}>
                  Crear Partido y Abrir Mesa
                </button>
              </form>

              <div className="panel">
                <h2>Juegos Programados en Calendario</h2>
                <p className="form-subtitle">Inicia directamente un partido desde la programación oficial.</p>
                {juegosCalendario.length === 0 ? (
                  <div className="empty-state">No hay juegos programados en el calendario.</div>
                ) : (
                  <ul className="custom-list">
                    {juegosCalendario.map(j => {
                      const eqLoc = equipos.find(e => e.id === j.equipoLocalId)?.nombre ?? 'Local'
                      const eqVis = equipos.find(e => e.id === j.equipoVisitanteId)?.nombre ?? 'Visitante'
                      return (
                        <li key={j.id} className="list-card-item">
                          <div>
                            <div className="item-title">
                              <strong>{j.codigo}</strong> · <span className="team-vs">{eqLoc} vs {eqVis}</span>
                            </div>
                            <small className="item-meta">
                              {new Date(j.fechaHoraProgramada).toLocaleString()} · {j.recinto} · {j.competicion} ({j.fase})
                            </small>
                          </div>
                          <button
                            className="btn-secondary"
                            disabled={j.estado !== 'Programado' && j.estado !== 'Reprogramado'}
                            onClick={() => void handleCrearDesdeCalendario(j.id)}
                          >
                            {j.estado === 'ConvertidoEnPartido' ? 'Iniciado' : 'Iniciar Partido'}
                          </button>
                        </li>
                      )
                    })}
                  </ul>
                )}
              </div>
            </div>
          ) : (
            <div className="scoreboard-view panel">
              <div className="scoreboard-header">
                <button className="link-btn" onClick={() => { setPartidoActual(null); setSetActual(null) }}>
                  ← Salir al Menú de Partidos
                </button>
                <div className="scoreboard-match-info">
                  <span className="match-venue">📍 {partidoActual.lugar}</span>
                  <span className="match-rules">
                    📜 {partidoActual.reglamento.codigoReglamento} (Mejor de {partidoActual.reglamento.maximoSets} · Gana {partidoActual.reglamento.setsParaGanar})
                  </span>
                </div>
              </div>

              {!setActual ? (
                <div className="empty-state scoreboard-prematch">
                  <h2>Partido Listo para Iniciar</h2>
                  <p>Equipos: <strong>{equipoLocalInfo?.nombre ?? 'Local'}</strong> vs <strong>{equipoVisitanteInfo?.nombre ?? 'Visitante'}</strong></p>
                  <button className="primary btn-large" onClick={() => void iniciarPartido()}>
                    ▶️ Iniciar Partido (Set 1)
                  </button>
                </div>
              ) : (
                <div className="active-match-scoreboard">
                  {/* Encabezado del Set */}
                  <div className="set-header">
                    <span className="set-title">SET {setActual.numeroSet}</span>
                    {setActual.finalizado && <span className="badge badge-success">SET FINALIZADO</span>}
                    {setActual.pendienteCambioCancha && <span className="badge badge-warning">CAMBIO DE CANCHA PENDIENTE</span>}
                    {setActual.pendienteConfirmacionCierre && <span className="badge badge-info">CIERRE DE SET PENDIENTE</span>}
                  </div>

                  {/* Marcador Principal */}
                  <div className="score-board-grid">
                    <div className="team-score-card local">
                      <span className="team-role">LOCAL</span>
                      <h3 className="team-name">{equipoLocalInfo?.nombre ?? 'Local'}</h3>
                      <span className="sets-won">Sets ganados: {partidoActual.setsGanadosLocal}</span>
                      <div className="score-digit">{setActual.puntosLocal}</div>
                      <button
                        className="btn-point primary"
                        disabled={setActual.finalizado || setActual.pendienteCambioCancha || setActual.pendienteConfirmacionCierre}
                        onClick={() => void anotarPunto(partidoActual.equipoLocalId)}
                      >
                        +1 Punto {equipoLocalInfo?.nombre ?? 'Local'}
                      </button>
                    </div>

                    <div className="score-divider">
                      <span>VS</span>
                    </div>

                    <div className="team-score-card visitor">
                      <span className="team-role">VISITANTE</span>
                      <h3 className="team-name">{equipoVisitanteInfo?.nombre ?? 'Visitante'}</h3>
                      <span className="sets-won">Sets ganados: {partidoActual.setsGanadosVisitante}</span>
                      <div className="score-digit">{setActual.puntosVisitante}</div>
                      <button
                        className="btn-point primary"
                        disabled={setActual.finalizado || setActual.pendienteCambioCancha || setActual.pendienteConfirmacionCierre}
                        onClick={() => void anotarPunto(partidoActual.equipoVisitanteId)}
                      >
                        +1 Punto {equipoVisitanteInfo?.nombre ?? 'Visitante'}
                      </button>
                    </div>
                  </div>

                  {/* Opciones de Anotación Avanzada */}
                  {!setActual.finalizado && (
                    <div className="point-details-bar panel-sub">
                      <label className="inline-select">
                        <span>Tipo de Acción:</span>
                        <select
                          value={accionPunto}
                          onChange={e => setAccionPunto(e.target.value as 'Ataque' | 'Bloqueo' | 'Saque' | 'Error')}
                        >
                          <option value="Ataque">Ataque</option>
                          <option value="Bloqueo">Bloqueo</option>
                          <option value="Saque">Saque As</option>
                          <option value="Error">Error del Adversario</option>
                        </select>
                      </label>

                      <label className="inline-select">
                        <span>Jugador Anotador (Opcional):</span>
                        <select
                          value={jugadorAnotadorId}
                          onChange={e => setJugadorAnotadorId(e.target.value)}
                        >
                          <option value="">-- Sin registrar jugador específico --</option>
                          <optgroup label={equipoLocalInfo?.nombre ?? 'Local'}>
                            {jugadores.filter(j => j.equipoId === partidoActual.equipoLocalId).map(j => (
                              <option key={j.id} value={j.id}>#{j.numeroCamiseta} {j.nombre} {j.apellidos} ({j.posicionTexto})</option>
                            ))}
                          </optgroup>
                          <optgroup label={equipoVisitanteInfo?.nombre ?? 'Visitante'}>
                            {jugadores.filter(j => j.equipoId === partidoActual.equipoVisitanteId).map(j => (
                              <option key={j.id} value={j.id}>#{j.numeroCamiseta} {j.nombre} {j.apellidos} ({j.posicionTexto})</option>
                            ))}
                          </optgroup>
                        </select>
                      </label>
                    </div>
                  )}

                  {/* Barra de Acciones y Arbitraje */}
                  <div className="match-actions-bar">
                    {!setActual.finalizado && (
                      <>
                        <button
                          className="btn-secondary"
                          onClick={() => void ejecutarAccionPartido(`/anotacion/partidos/${partidoActual.id}/deshacer`, 'Último punto revertido.')}
                        >
                          ↶ Deshacer Último Punto
                        </button>

                        {setActual.pendienteCambioCancha && (
                          <button
                            className="warning btn-large"
                            onClick={() => void ejecutarAccionPartido(`/anotacion/partidos/${partidoActual.id}/sets/confirmar-cambio-cancha`, 'Cambio de cancha confirmado.')}
                          >
                            ⚠️ Confirmar Cambio de Cancha (Punto 8)
                          </button>
                        )}

                        {setActual.pendienteConfirmacionCierre && (
                          <button
                            className="primary btn-large"
                            onClick={() => void ejecutarAccionPartido(`/anotacion/partidos/${partidoActual.id}/sets/confirmar-cierre`, 'Set cerrado y confirmado.')}
                          >
                            ✅ Confirmar Cierre del Set {setActual.numeroSet}
                          </button>
                        )}
                      </>
                    )}

                    {setActual.finalizado && !partidoActual.finalizado && (
                      <button
                        className="primary btn-large"
                        onClick={() => void ejecutarAccionPartido(`/partidos/${partidoActual.id}/sets/${setActual.numeroSet + 1}/iniciar`, `Set ${setActual.numeroSet + 1} iniciado.`)}
                      >
                        ▶️ Iniciar Set {setActual.numeroSet + 1}
                      </button>
                    )}

                    {partidoActual.finalizado && (
                      <div className="winner-banner">
                        🎉 ¡Partido Finalizado! Ganador:{' '}
                        <strong>
                          {partidoActual.equipoGanadorId === partidoActual.equipoLocalId
                            ? equipoLocalInfo?.nombre
                            : equipoVisitanteInfo?.nombre}
                        </strong>
                      </div>
                    )}
                  </div>
                </div>
              )}
            </div>
          )}
        </section>
      )}

      {/* 2. TAB: CALENDARIO DE JUEGOS */}
      {tabActiva === 'calendario' && (
        <section className="tab-content grid-2-cols">
          <form className="panel form" onSubmit={handleSubmitCalendario}>
            <h2>Programar Juego en Calendario</h2>
            <p className="form-subtitle">Planifica encuentros oficiales de torneos o ligas.</p>

            <div className="form-row">
              <label>
                Código del Juego *
                <input
                  type="text"
                  required
                  placeholder="Ej: J01-FEM-2026"
                  value={formCalCodigo}
                  onChange={e => setFormCalCodigo(e.target.value)}
                />
              </label>

              <label>
                Competición / Torneo
                <select value={formCalCompId} onChange={e => setFormCalCompId(e.target.value)}>
                  <option value="">-- Seleccionar Competición --</option>
                  {competiciones.map(c => (
                    <option key={c.id} value={c.id}>{c.nombre} ({c.edicion} - {c.categoria})</option>
                  ))}
                </select>
              </label>
            </div>

            <div className="form-row">
              <label>
                Fase / Jornada
                <input
                  type="text"
                  placeholder="Ej: Fase de Grupos - Fecha 1"
                  value={formCalFase}
                  onChange={e => setFormCalFase(e.target.value)}
                />
              </label>

              <label>
                Reglamento Aplicable
                <select value={formCalRegCodigo} onChange={e => setFormCalRegCodigo(e.target.value)}>
                  {reglamentos.map(r => (
                    <option key={r.codigoReglamento} value={r.codigoReglamento}>{r.nombre}</option>
                  ))}
                </select>
              </label>
            </div>

            <div className="form-row">
              <label>
                Equipo Local *
                <select required value={formCalLocalId} onChange={e => setFormCalLocalId(e.target.value)}>
                  <option value="">-- Seleccionar Local --</option>
                  {equipos.map(e => (
                    <option key={e.id} value={e.id}>{e.nombre}</option>
                  ))}
                </select>
              </label>

              <label>
                Equipo Visitante *
                <select required value={formCalVisitanteId} onChange={e => setFormCalVisitanteId(e.target.value)}>
                  <option value="">-- Seleccionar Visitante --</option>
                  {equipos.map(e => (
                    <option key={e.id} value={e.id}>{e.nombre}</option>
                  ))}
                </select>
              </label>
            </div>

            <div className="form-row">
              <label>
                Fecha y Hora Programada *
                <input
                  type="datetime-local"
                  required
                  value={formCalFechaHora}
                  onChange={e => setFormCalFechaHora(e.target.value)}
                />
              </label>

              <label>
                Recinto / Cancha *
                <input
                  type="text"
                  required
                  placeholder="Ej: Cancha Central Polideportivo"
                  value={formCalRecinto}
                  onChange={e => setFormCalRecinto(e.target.value)}
                />
              </label>
            </div>

            <button type="submit" className="primary">Programar Juego</button>
          </form>

          <div className="panel">
            <h2>Calendario Oficial ({juegosCalendario.length})</h2>
            {juegosCalendario.length === 0 ? (
              <div className="empty-state">No hay juegos programados. Completa el formulario para agendar uno.</div>
            ) : (
              <div className="table-responsive">
                <table className="data-table">
                  <thead>
                    <tr>
                      <th>Código</th>
                      <th>Encuentro</th>
                      <th>Fecha / Hora</th>
                      <th>Recinto</th>
                      <th>Estado</th>
                      <th>Acción</th>
                    </tr>
                  </thead>
                  <tbody>
                    {juegosCalendario.map(j => {
                      const local = equipos.find(e => e.id === j.equipoLocalId)?.nombre ?? 'Local'
                      const visit = equipos.find(e => e.id === j.equipoVisitanteId)?.nombre ?? 'Visitante'
                      return (
                        <tr key={j.id}>
                          <td><strong>{j.codigo}</strong></td>
                          <td>{local} vs {visit}</td>
                          <td>{new Date(j.fechaHoraProgramada).toLocaleString([], { dateStyle: 'short', timeStyle: 'short' })}</td>
                          <td>{j.recinto}</td>
                          <td>
                            <span className={`badge ${j.estado === 'ConvertidoEnPartido' ? 'badge-info' : 'badge-success'}`}>
                              {j.estado}
                            </span>
                          </td>
                          <td>
                            <button
                              className="btn-sm primary"
                              disabled={j.estado === 'ConvertidoEnPartido'}
                              onClick={() => void handleCrearDesdeCalendario(j.id)}
                            >
                              Iniciar
                            </button>
                          </td>
                        </tr>
                      )
                    })}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        </section>
      )}

      {/* 3. TAB: EQUIPOS */}
      {tabActiva === 'equipos' && (
        <section className="tab-content grid-2-cols">
          <form className="panel form" onSubmit={handleSubmitEquipo}>
            <h2>Registrar Nuevo Equipo</h2>
            <p className="form-subtitle">Ingresa la información básica y cuerpo técnico del club.</p>

            <label>
              Nombre del Equipo *
              <input
                type="text"
                required
                placeholder="Ej: Panteras de León"
                value={formEquipoNombre}
                onChange={e => setFormEquipoNombre(e.target.value)}
              />
            </label>

            <div className="form-row">
              <label>
                Nombre del Entrenador *
                <input
                  type="text"
                  required
                  placeholder="Ej: Roberto Gómez"
                  value={formEquipoEntrenador}
                  onChange={e => setFormEquipoEntrenador(e.target.value)}
                />
              </label>

              <label>
                Nombre del Asistente Técnico
                <input
                  type="text"
                  placeholder="Ej: Carlos Vega (Opcional)"
                  value={formEquipoAsistente}
                  onChange={e => setFormEquipoAsistente(e.target.value)}
                />
              </label>
            </div>

            <label>
              Categoría *
              <select value={formEquipoCategoria} onChange={e => setFormEquipoCategoria(e.target.value)}>
                <option value="Mayor">Mayor / Libre</option>
                <option value="U19">Juvenil Mayor (U19)</option>
                <option value="U17">Juvenil Menor (U17)</option>
                <option value="U15">Infantil Mayor (U15)</option>
                <option value="Master">Master (+35)</option>
              </select>
            </label>

            <button type="submit" className="primary">Guardar Equipo</button>
          </form>

          <div className="panel">
            <h2>Equipos Registrados ({equipos.length})</h2>
            {equipos.length === 0 ? (
              <div className="empty-state">No hay equipos registrados aún.</div>
            ) : (
              <div className="cards-grid">
                {equipos.map(eq => (
                  <div key={eq.id} className="item-card">
                    <div className="card-header">
                      <h3>{eq.nombre}</h3>
                      <span className="badge badge-info">{eq.categoria}</span>
                    </div>
                    <p className="card-detail">👨‍🏫 <strong>DT:</strong> {eq.nombreEntrenador}</p>
                    {eq.nombreAsistente && <p className="card-detail">📋 <strong>AT:</strong> {eq.nombreAsistente}</p>}
                    <p className="card-detail">👥 <strong>Plantilla:</strong> {eq.totalJugadores} jugador(es)</p>
                    <button
                      className="btn-sm btn-secondary"
                      onClick={() => {
                        setFiltroEquipoJugadores(eq.id)
                        setFormJugadorEquipoId(eq.id)
                        setTabActiva('jugadores')
                      }}
                    >
                      Ver / Agregar Jugadores →
                    </button>
                  </div>
                ))}
              </div>
            )}
          </div>
        </section>
      )}

      {/* 4. TAB: JUGADORES */}
      {tabActiva === 'jugadores' && (
        <section className="tab-content grid-2-cols">
          <form className="panel form" onSubmit={handleSubmitJugador}>
            <h2>Registrar Nuevo Jugador</h2>
            <p className="form-subtitle">Asigna dorsales, posiciones tácticas y capitanía.</p>

            <label>
              Equipo de Pertenencia *
              <select
                required
                value={formJugadorEquipoId}
                onChange={e => setFormJugadorEquipoId(e.target.value)}
              >
                <option value="">-- Seleccionar Equipo --</option>
                {equipos.map(eq => (
                  <option key={eq.id} value={eq.id}>{eq.nombre} ({eq.categoria})</option>
                ))}
              </select>
            </label>

            <div className="form-row">
              <label>
                Nombre *
                <input
                  type="text"
                  required
                  placeholder="Ej: Alejandro"
                  value={formJugadorNombre}
                  onChange={e => setFormJugadorNombre(e.target.value)}
                />
              </label>

              <label>
                Apellidos *
                <input
                  type="text"
                  required
                  placeholder="Ej: Silva Méndez"
                  value={formJugadorApellidos}
                  onChange={e => setFormJugadorApellidos(e.target.value)}
                />
              </label>
            </div>

            <div className="form-row">
              <label>
                Número de Camiseta (Dorsal) *
                <input
                  type="number"
                  min="1"
                  max="99"
                  required
                  placeholder="Ej: 7"
                  value={formJugadorCamiseta}
                  onChange={e => setFormJugadorCamiseta(e.target.value ? Number(e.target.value) : '')}
                />
              </label>

              <label>
                Posición Táctica *
                <select
                  value={formJugadorPosicion}
                  onChange={e => setFormJugadorPosicion(Number(e.target.value))}
                >
                  <option value={1}>Colocador / Armador</option>
                  <option value={2}>Rematador / Punta Receptor</option>
                  <option value={3}>Central / Bloqueador</option>
                  <option value={4}>Opuesto</option>
                  <option value={5}>Líbero (Defensivo)</option>
                </select>
              </label>
            </div>

            <label className="checkbox-label">
              <input
                type="checkbox"
                checked={formJugadorCapitan}
                onChange={e => setFormJugadorCapitan(e.target.checked)}
              />
              <span>👑 Es el Capitán del Equipo (Cap)</span>
            </label>

            <button type="submit" className="primary" disabled={equipos.length === 0}>
              Guardar Jugador
            </button>
          </form>

          <div className="panel">
            <div className="panel-header-filter">
              <h2>Plantillas de Jugadores ({jugadores.length})</h2>
              <select
                value={filtroEquipoJugadores}
                onChange={e => setFiltroEquipoJugadores(e.target.value)}
                className="filter-select"
              >
                <option value="">-- Todos los equipos --</option>
                {equipos.map(eq => (
                  <option key={eq.id} value={eq.id}>{eq.nombre}</option>
                ))}
              </select>
            </div>

            {jugadores.length === 0 ? (
              <div className="empty-state">No hay jugadores registrados. Agrega jugadores usando el formulario.</div>
            ) : (
              <div className="table-responsive">
                <table className="data-table">
                  <thead>
                    <tr>
                      <th>Dorsal</th>
                      <th>Nombre Completo</th>
                      <th>Equipo</th>
                      <th>Posición</th>
                      <th>Rol</th>
                    </tr>
                  </thead>
                  <tbody>
                    {jugadores
                      .filter(j => !filtroEquipoJugadores || j.equipoId === filtroEquipoJugadores)
                      .map(j => (
                        <tr key={j.id}>
                          <td><span className="jersey-badge">#{j.numeroCamiseta}</span></td>
                          <td><strong>{j.nombre} {j.apellidos}</strong></td>
                          <td>{j.nombreEquipo ?? equipos.find(e => e.id === j.equipoId)?.nombre}</td>
                          <td><span className={`badge ${j.posicion === 5 ? 'badge-warning' : 'badge-info'}`}>{j.posicionTexto}</span></td>
                          <td>{j.esCapitan ? <span className="captain-badge">👑 Capitán</span> : 'Jugador'}</td>
                        </tr>
                      ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        </section>
      )}

      {/* 5. TAB: ÁRBITROS */}
      {tabActiva === 'arbitros' && (
        <section className="tab-content grid-2-cols">
          <form className="panel form" onSubmit={handleSubmitArbitro}>
            <h2>Registrar Árbitro u Oficial de Mesa</h2>
            <p className="form-subtitle">Registra el cuerpo arbitral calificado para los partidos.</p>

            <div className="form-row">
              <label>
                Nombre *
                <input
                  type="text"
                  required
                  placeholder="Ej: Daniel"
                  value={formArbitroNombre}
                  onChange={e => setFormArbitroNombre(e.target.value)}
                />
              </label>

              <label>
                Apellidos *
                <input
                  type="text"
                  required
                  placeholder="Ej: Navarro Castillo"
                  value={formArbitroApellidos}
                  onChange={e => setFormArbitroApellidos(e.target.value)}
                />
              </label>
            </div>

            <label>
              Rol / Función Arbitral *
              <select value={formArbitroRol} onChange={e => setFormArbitroRol(Number(e.target.value))}>
                <option value={1}>1.º Árbitro (Principal en Silla)</option>
                <option value={2}>2.º Árbitro (Asistente en Piso)</option>
                <option value={3}>Anotador Oficial de Acta</option>
                <option value={4}>Asistente de Anotador (Control de Líbero)</option>
                <option value={5}>Juez de Línea</option>
              </select>
            </label>

            <div className="form-row">
              <label>
                Número de Licencia / Credencial
                <input
                  type="text"
                  placeholder="Ej: FED-VB-2026-44"
                  value={formArbitroLicencia}
                  onChange={e => setFormArbitroLicencia(e.target.value)}
                />
              </label>

              <label>
                Federación / Asociación
                <input
                  type="text"
                  value={formArbitroFederacion}
                  onChange={e => setFormArbitroFederacion(e.target.value)}
                />
              </label>
            </div>

            <button type="submit" className="primary">Registrar Árbitro / Oficial</button>
          </form>

          <div className="panel">
            <h2>Colegio de Árbitros y Oficiales ({arbitros.length})</h2>
            {arbitros.length === 0 ? (
              <div className="empty-state">No hay árbitros registrados.</div>
            ) : (
              <div className="cards-grid">
                {arbitros.map(a => (
                  <div key={a.id} className="item-card">
                    <div className="card-header">
                      <h3>{a.nombreCompleto}</h3>
                      <span className="badge badge-success">{a.rolTexto}</span>
                    </div>
                    <p className="card-detail">📜 <strong>Licencia:</strong> {a.numeroLicencia ?? 'N/D'}</p>
                    <p className="card-detail">🏛️ <strong>Asociación:</strong> {a.federacion ?? 'N/D'}</p>
                  </div>
                ))}
              </div>
            )}
          </div>
        </section>
      )}

      {/* 6. TAB: REGLAMENTOS */}
      {tabActiva === 'reglamentos' && (
        <section className="tab-content grid-2-cols">
          <form className="panel form" onSubmit={handleSubmitReglamento}>
            <h2>Crear Perfil de Reglamento</h2>
            <p className="form-subtitle">Configura reglas personalizadas para ligas, torneos o fases de copa.</p>

            <div className="preset-buttons-bar">
              <span>Plantillas Rápidas:</span>
              <button type="button" className="btn-sm btn-secondary" onClick={() => aplicarPresetReglamento('FIVB')}>FIVB Oficial (5 Sets)</button>
              <button type="button" className="btn-sm btn-secondary" onClick={() => aplicarPresetReglamento('LEON')}>León Regular (3 Sets)</button>
              <button type="button" className="btn-sm btn-secondary" onClick={() => aplicarPresetReglamento('RAPIDO')}>Rápido (21 Pts)</button>
            </div>

            <div className="form-row">
              <label>
                Código del Reglamento *
                <input
                  type="text"
                  required
                  placeholder="Ej: LEON-PLAYOFFS-5SETS"
                  value={formRegCodigo}
                  onChange={e => setFormRegCodigo(e.target.value)}
                />
              </label>

              <label>
                Nombre Descriptivo *
                <input
                  type="text"
                  required
                  placeholder="Ej: Playoffs León (Mejor de 5)"
                  value={formRegNombre}
                  onChange={e => setFormRegNombre(e.target.value)}
                />
              </label>
            </div>

            <label>
              Descripción
              <input
                type="text"
                placeholder="Ej: Aplica para semifinales y finales de copa."
                value={formRegDesc}
                onChange={e => setFormRegDesc(e.target.value)}
              />
            </label>

            <div className="form-row">
              <label>
                Máximo de Sets *
                <input
                  type="number"
                  min="1"
                  max="5"
                  required
                  value={formRegMaxSets}
                  onChange={e => setFormRegMaxSets(Number(e.target.value))}
                />
              </label>

              <label>
                Sets para Ganar *
                <input
                  type="number"
                  min="1"
                  max="3"
                  required
                  value={formRegSetsGanar}
                  onChange={e => setFormRegSetsGanar(Number(e.target.value))}
                />
              </label>
            </div>

            <div className="form-row">
              <label>
                Puntos Set Regular *
                <input
                  type="number"
                  min="10"
                  max="35"
                  required
                  value={formRegPtsRegular}
                  onChange={e => setFormRegPtsRegular(Number(e.target.value))}
                />
              </label>

              <label>
                Puntos Set Decisivo *
                <input
                  type="number"
                  min="5"
                  max="25"
                  required
                  value={formRegPtsDecisivo}
                  onChange={e => setFormRegPtsDecisivo(Number(e.target.value))}
                />
              </label>
            </div>

            <div className="form-row">
              <label>
                Diferencia Mínima *
                <input
                  type="number"
                  min="1"
                  max="5"
                  required
                  value={formRegDifMin}
                  onChange={e => setFormRegDifMin(Number(e.target.value))}
                />
              </label>

              <label>
                Punto Cambio Cancha Decisivo *
                <input
                  type="number"
                  min="1"
                  max="15"
                  required
                  value={formRegPtsCambio}
                  onChange={e => setFormRegPtsCambio(Number(e.target.value))}
                />
              </label>
            </div>

            <button type="submit" className="primary">Guardar Perfil Reglamentario</button>
          </form>

          <div className="panel">
            <h2>Perfiles Reglamentarios Disponibles ({reglamentos.length})</h2>
            <div className="cards-grid">
              {reglamentos.map(r => (
                <div key={r.codigoReglamento} className="item-card">
                  <div className="card-header">
                    <h3>{r.nombre}</h3>
                    <span className="badge badge-info">{r.codigoReglamento}</span>
                  </div>
                  {r.descripcion && <p className="card-detail">📝 {r.descripcion}</p>}
                  <ul className="specs-list">
                    <li><strong>Formato:</strong> Mejor de {r.maximoSets} (gana {r.setsParaGanar})</li>
                    <li><strong>Puntos Regular:</strong> {r.puntosSetRegular} pts (dif. min {r.diferenciaMinima})</li>
                    <li><strong>Puntos Decisivo:</strong> {r.puntosSetDecisivo} pts (cambio cancha al punto {r.puntoCambioCanchaSetDecisivo})</li>
                  </ul>
                </div>
              ))}
            </div>
          </div>
        </section>
      )}

      {/* 7. TAB: COMPETICIONES */}
      {tabActiva === 'competiciones' && (
        <section className="tab-content grid-2-cols">
          <form className="panel form" onSubmit={handleSubmitCompeticion}>
            <h2>Registrar Competición o Torneo</h2>
            <p className="form-subtitle">Organiza los campeonatos, ligas y copas activas.</p>

            <div className="form-row">
              <label>
                Nombre del Torneo / Liga *
                <input
                  type="text"
                  required
                  placeholder="Ej: Liga Municipal de Voleibol León"
                  value={formCompNombre}
                  onChange={e => setFormCompNombre(e.target.value)}
                />
              </label>

              <label>
                Edición / Temporada *
                <input
                  type="text"
                  required
                  placeholder="Ej: Clausura 2026"
                  value={formCompEdicion}
                  onChange={e => setFormCompEdicion(e.target.value)}
                />
              </label>
            </div>

            <div className="form-row">
              <label>
                Categoría *
                <select value={formCompCategoria} onChange={e => setFormCompCategoria(e.target.value)}>
                  <option value="Mayor">Mayor / Libre</option>
                  <option value="U19">Juvenil Mayor (U19)</option>
                  <option value="U17">Juvenil Menor (U17)</option>
                  <option value="U15">Infantil Mayor (U15)</option>
                  <option value="Master">Master</option>
                </select>
              </label>

              <label>
                Rama *
                <select value={formCompRama} onChange={e => setFormCompRama(e.target.value)}>
                  <option value="Femenil">Femenil</option>
                  <option value="Varonil">Varonil</option>
                  <option value="Mixto">Mixto</option>
                </select>
              </label>
            </div>

            <div className="form-row">
              <label>
                Organizador
                <input
                  type="text"
                  placeholder="Ej: COMUDE León"
                  value={formCompOrganizador}
                  onChange={e => setFormCompOrganizador(e.target.value)}
                />
              </label>

              <label>
                Sede Principal
                <input
                  type="text"
                  placeholder="Ej: Polideportivo León 1"
                  value={formCompSede}
                  onChange={e => setFormCompSede(e.target.value)}
                />
              </label>
            </div>

            <button type="submit" className="primary">Registrar Competición</button>
          </form>

          <div className="panel">
            <h2>Competiciones Registradas ({competiciones.length})</h2>
            {competiciones.length === 0 ? (
              <div className="empty-state">No hay competiciones registradas.</div>
            ) : (
              <div className="cards-grid">
                {competiciones.map(c => (
                  <div key={c.id} className="item-card">
                    <div className="card-header">
                      <h3>{c.nombre}</h3>
                      <span className="badge badge-success">{c.rama}</span>
                    </div>
                    <p className="card-detail">🏆 <strong>Edición:</strong> {c.edicion}</p>
                    <p className="card-detail">🏷️ <strong>Categoría:</strong> {c.categoria}</p>
                    {c.organizador && <p className="card-detail">🏛️ <strong>Organizador:</strong> {c.organizador}</p>}
                    {c.sedePrincipal && <p className="card-detail">📍 <strong>Sede:</strong> {c.sedePrincipal}</p>}
                  </div>
                ))}
              </div>
            )}
          </div>
        </section>
      )}
    </main>
  )
}
