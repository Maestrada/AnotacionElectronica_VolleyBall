namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class Rotacion
{
    public Guid Id { get; private set; }
    public Guid SetId { get; private set; }
    public Guid EquipoId { get; private set; }

    // Posiciones en la cancha oficiales (1 al 6)
    public Guid Posicion1Id { get; private set; } // Saque
    public Guid Posicion2Id { get; private set; }
    public Guid Posicion3Id { get; private set; }
    public Guid Posicion4Id { get; private set; }
    public Guid Posicion5Id { get; private set; }
    public Guid Posicion6Id { get; private set; }

    private Rotacion() { }

    public Rotacion(Guid setId, Guid equipoId, Guid p1, Guid p2, Guid p3, Guid p4, Guid p5, Guid p6)
    {
        Id = Guid.NewGuid();
        SetId = setId;
        EquipoId = equipoId;
        Posicion1Id = p1;
        Posicion2Id = p2;
        Posicion3Id = p3;
        Posicion4Id = p4;
        Posicion5Id = p5;
        Posicion6Id = p6;
    }
}