using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class EventoPartido
{
    public Guid Id { get; private set; }
    public Guid PartidoId { get; private set; }
    public Guid? SetId { get; private set; }
    public int Secuencia { get; private set; }
    public TipoEventoPartido Tipo { get; private set; }
    public string DatosJson { get; private set; } = string.Empty;
    public DateTime OcurrioEnUtc { get; private set; }

    private EventoPartido() { }

    public EventoPartido(Guid partidoId, Guid? setId, int secuencia, TipoEventoPartido tipo, string datosJson)
    {
        Id = Guid.NewGuid();
        PartidoId = partidoId;
        SetId = setId;
        Secuencia = secuencia;
        Tipo = tipo;
        DatosJson = datosJson;
        OcurrioEnUtc = DateTime.UtcNow;
    }
}
