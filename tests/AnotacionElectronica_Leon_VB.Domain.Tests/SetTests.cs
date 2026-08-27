using AnotacionElectronica_Leon_VB.Domain.Entities;
using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Domain.Tests;

public class SetTests
{
    [Fact]
    public void ResultadoReglamentario_QuedaPendienteHastaQueSeConfirma_YPuedeDeshacerse()
    {
        var local = Guid.NewGuid();
        var visitante = Guid.NewGuid();
        var set = new Set(Guid.NewGuid(), 1);

        for (var punto = 0; punto < 23; punto++)
        {
            set.AnotarPunto(local, local, visitante, TipoAccionPunto.Ataque);
            set.AnotarPunto(visitante, local, visitante, TipoAccionPunto.Ataque);
        }
        set.AnotarPunto(local, local, visitante, TipoAccionPunto.Ataque);
        set.AnotarPunto(local, local, visitante, TipoAccionPunto.Ataque);

        Assert.Equal(25, set.PuntosLocal);
        Assert.Equal(23, set.PuntosVisitante);
        Assert.True(set.PendienteConfirmacionCierre);
        Assert.False(set.Finalizado);

        set.DeshacerUltimoPunto();

        Assert.Equal(24, set.PuntosLocal);
        Assert.False(set.PendienteConfirmacionCierre);

        set.AnotarPunto(local, local, visitante, TipoAccionPunto.Ataque);
        set.ConfirmarCierre(local, visitante);

        Assert.True(set.Finalizado);
        Assert.Equal(local, set.EquipoGanadorId);
    }

    [Fact]
    public void SetDecisivo_ActivaYConfirmaCambioDeCanchaAlLlegarAOcho()
    {
        var local = Guid.NewGuid();
        var visitante = Guid.NewGuid();
        var set = new Set(Guid.NewGuid(), 3, esSetDecisivo: true, puntosParaGanar: 15);

        for (var punto = 0; punto < 5; punto++)
        {
            set.AnotarPunto(local, local, visitante, TipoAccionPunto.Ataque);
            set.AnotarPunto(visitante, local, visitante, TipoAccionPunto.Ataque);
        }
        set.AnotarPunto(local, local, visitante, TipoAccionPunto.Ataque);
        set.AnotarPunto(local, local, visitante, TipoAccionPunto.Ataque);
        set.AnotarPunto(local, local, visitante, TipoAccionPunto.Ataque);

        Assert.True(set.PendienteCambioCancha);
        Assert.Equal(8, set.PuntosLocal);
        Assert.Equal(5, set.PuntosVisitante);

        set.ConfirmarCambioCancha();

        Assert.True(set.CambioCanchaConfirmado);
        Assert.False(set.PendienteCambioCancha);
    }
}
