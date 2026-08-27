namespace AnotacionElectronica_Leon_VB.Application.DTOs;

public record CrearPerfilReglamentoDto(
    string CodigoReglamento,
    string Nombre,
    string? Descripcion,
    int MaximoSets,
    int SetsParaGanar,
    int PuntosSetRegular,
    int PuntosSetDecisivo,
    int DiferenciaMinima,
    int PuntoCambioCanchaSetDecisivo);

public record PerfilReglamentoDto(
    Guid Id,
    string CodigoReglamento,
    string Nombre,
    string? Descripcion,
    int MaximoSets,
    int SetsParaGanar,
    int PuntosSetRegular,
    int PuntosSetDecisivo,
    int DiferenciaMinima,
    int PuntoCambioCanchaSetDecisivo);
