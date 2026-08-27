using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class AlineacionSet
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid SetId { get; private set; }
    public Guid EquipoId { get; private set; }
    
    // Formación inicial registrada en la hoja de alineación
    public Guid PosicionInicial1Id { get; private set; } // Jugador en Posición I
    public Guid PosicionInicial2Id { get; private set; } // Jugador en Posición II
    public Guid PosicionInicial3Id { get; private set; } // Jugador en Posición III
    public Guid PosicionInicial4Id { get; private set; } // Jugador en Posición IV
    public Guid PosicionInicial5Id { get; private set; } // Jugador en Posición V
    public Guid PosicionInicial6Id { get; private set; } // Jugador en Posición VI

    // Posiciones dinámicas actuales en cancha (I = 1, II = 2, ..., VI = 6)
    // Key: PosicionCancha (1 a 6), Value: JugadorId actual en cancha
    private readonly Dictionary<PosicionCancha, Guid> _posicionesActuales = new();
    public IReadOnlyDictionary<PosicionCancha, Guid> PosicionesActuales => _posicionesActuales;

    // Estado del Líbero: JugadorId original al que el líbero está sustituyendo temporalmente
    private readonly Dictionary<PosicionCancha, Guid> _jugadorReemplazadoPorLibero = new();

    public int SustitucionesRealizadas { get; private set; } = 0;
    public const int MaxSustitucionesPorSet = 6;

    private readonly List<SustitucionReglamentaria> _sustituciones = new();
    public IReadOnlyCollection<SustitucionReglamentaria> Sustituciones => _sustituciones.AsReadOnly();

    protected AlineacionSet() { }

    public AlineacionSet(
        Guid setId,
        Guid equipoId,
        Guid p1, Guid p2, Guid p3, Guid p4, Guid p5, Guid p6)
    {
        SetId = setId;
        EquipoId = equipoId;
        PosicionInicial1Id = p1;
        PosicionInicial2Id = p2;
        PosicionInicial3Id = p3;
        PosicionInicial4Id = p4;
        PosicionInicial5Id = p5;
        PosicionInicial6Id = p6;

        _posicionesActuales[PosicionCancha.I] = p1;
        _posicionesActuales[PosicionCancha.II] = p2;
        _posicionesActuales[PosicionCancha.III] = p3;
        _posicionesActuales[PosicionCancha.IV] = p4;
        _posicionesActuales[PosicionCancha.V] = p5;
        _posicionesActuales[PosicionCancha.VI] = p6;
    }

    /// <summary>
    /// Ejecuta la rotación oficial en el sentido de las agujas del reloj:
    /// Pos I -> VI, VI -> V, V -> IV, IV -> III, III -> II, II -> I
    /// El jugador que pasa a Posición I toma el turno de saque.
    /// </summary>
    public void RotarSentidoHorario()
    {
        var jugadorPos1 = _posicionesActuales[PosicionCancha.I];

        _posicionesActuales[PosicionCancha.I] = _posicionesActuales[PosicionCancha.II];
        _posicionesActuales[PosicionCancha.II] = _posicionesActuales[PosicionCancha.III];
        _posicionesActuales[PosicionCancha.III] = _posicionesActuales[PosicionCancha.IV];
        _posicionesActuales[PosicionCancha.IV] = _posicionesActuales[PosicionCancha.V];
        _posicionesActuales[PosicionCancha.V] = _posicionesActuales[PosicionCancha.VI];
        _posicionesActuales[PosicionCancha.VI] = jugadorPos1;

        // Si el líbero rotó a la red (Posición IV, III o II), debe salir automáticamente
        VerificarSalidaObligatoriaLiberoDeLaRed();
    }

    public Guid ObtenerJugadorAlSaque() => _posicionesActuales[PosicionCancha.I];

    /// <summary>
    /// Sustitución formal FIVB (máximo 6 por set, solo jugadores de campo)
    /// </summary>
    public void RealizarSustitucionReglamentaria(
        Guid jugadorSaleId,
        Guid jugadorEntraId,
        int puntosLocal,
        int puntosVisitante)
    {
        if (SustitucionesRealizadas >= MaxSustitucionesPorSet)
            throw new InvalidOperationException($"Límite alcanzado: el equipo ya utilizó las {MaxSustitucionesPorSet} sustituciones permitidas.");

        var posicionEncontrada = _posicionesActuales.FirstOrDefault(x => x.Value == jugadorSaleId);
        if (posicionEncontrada.Key == 0)
            throw new InvalidOperationException("El jugador que sale no está actualmente en la cancha.");

        _posicionesActuales[posicionEncontrada.Key] = jugadorEntraId;
        SustitucionesRealizadas++;

        _sustituciones.Add(new SustitucionReglamentaria(
            Id,
            jugadorSaleId,
            jugadorEntraId,
            posicionEncontrada.Key,
            puntosLocal,
            puntosVisitante
        ));
    }

    /// <summary>
    /// Reemplazo de Líbero: No cuenta como sustitución regular.
    /// Solo permitido en posiciones de zaga (I, VI, V).
    /// </summary>
    public void EntrarLibero(Guid liberoId, PosicionCancha posicion)
    {
        if (posicion is not (PosicionCancha.I or PosicionCancha.VI or PosicionCancha.V))
            throw new InvalidOperationException("El líbero únicamente puede jugar en posiciones de zaga (1, 6 o 5).");

        var jugadorOriginal = _posicionesActuales[posicion];
        _jugadorReemplazadoPorLibero[posicion] = jugadorOriginal;
        _posicionesActuales[posicion] = liberoId;
    }

    public void SalirLibero(PosicionCancha posicion)
    {
        if (_jugadorReemplazadoPorLibero.TryGetValue(posicion, out var jugadorOriginalId))
        {
            _posicionesActuales[posicion] = jugadorOriginalId;
            _jugadorReemplazadoPorLibero.Remove(posicion);
        }
    }

    private void VerificarSalidaObligatoriaLiberoDeLaRed()
    {
        foreach (var posDelantera in new[] { PosicionCancha.IV, PosicionCancha.III, PosicionCancha.II })
        {
            if (_jugadorReemplazadoPorLibero.ContainsKey(posDelantera))
            {
                SalirLibero(posDelantera);
            }
        }
    }
}