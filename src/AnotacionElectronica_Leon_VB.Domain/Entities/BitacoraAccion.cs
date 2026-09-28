namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class BitacoraAccion
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid? UsuarioId { get; private set; }
    public string Usuario { get; private set; } = "Sistema";
    public string Accion { get; private set; } = string.Empty;
    public string MetodoHttp { get; private set; } = string.Empty;
    public string Ruta { get; private set; } = string.Empty;
    public int CodigoRespuesta { get; private set; }
    public string? DireccionIp { get; private set; }
    public string? DetalleJson { get; private set; }
    public DateTime FechaUtc { get; private set; } = DateTime.UtcNow;
    private BitacoraAccion() { }
    public BitacoraAccion(Guid? usuarioId, string usuario, string accion, string metodoHttp, string ruta, int codigoRespuesta, string? direccionIp, string? detalleJson)
    {
        UsuarioId = usuarioId; Usuario = string.IsNullOrWhiteSpace(usuario) ? "Sistema" : usuario.Trim();
        Accion = accion.Trim(); MetodoHttp = metodoHttp.Trim(); Ruta = ruta.Trim(); CodigoRespuesta = codigoRespuesta;
        DireccionIp = direccionIp; DetalleJson = detalleJson; FechaUtc = DateTime.UtcNow;
    }
}
