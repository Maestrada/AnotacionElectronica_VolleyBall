using AnotacionElectronica_Leon_VB.Domain.Enums;
namespace AnotacionElectronica_Leon_VB.Domain.Entities;
public class DesignacionArbitral
{ public Guid Id { get; private set; } = Guid.NewGuid(); public Guid PartidoId { get; private set; } public Guid ArbitroId { get; private set; } public RolArbitro Rol { get; private set; } public string NombreArbitro { get; private set; } = string.Empty; private DesignacionArbitral() { } public DesignacionArbitral(Guid partidoId, Arbitro arbitro, RolArbitro rol) { if (partidoId == Guid.Empty) throw new ArgumentException("El partido es obligatorio."); PartidoId = partidoId; ArbitroId = arbitro.Id; Rol = rol; NombreArbitro = $"{arbitro.Nombre} {arbitro.Apellidos}"; } }
