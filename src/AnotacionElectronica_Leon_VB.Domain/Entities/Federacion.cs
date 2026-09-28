namespace AnotacionElectronica_Leon_VB.Domain.Entities;
public class Federacion
{ public Guid Id { get; private set; } = Guid.NewGuid(); public string Nombre { get; private set; } = string.Empty; public string? ImagenUrl { get; private set; } private Federacion() { } public Federacion(string nombre, string? imagenUrl = null) { if(string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre es obligatorio."); Nombre=nombre.Trim(); ImagenUrl=imagenUrl?.Trim(); } }
