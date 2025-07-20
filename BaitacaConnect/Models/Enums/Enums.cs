namespace BaitacaConnect.Models.Enums
{
    public enum TipoUsuario
    {
        Visitante,
        Administrador,
        Funcionario,
        Guia
    }

    public enum StatusReserva
    {
        Ativa,
        Cancelada,
        Concluida,
        EmAndamento,
        NaoCompareceu
    }

    public enum DificuldadeTrilha
    {
        Facil,
        Moderada,
        Dificil,
        Extrema
    }

    public enum TipoFaunaFlora
    {
        Fauna,
        Flora
    }

    public enum TipoPontoInteresse
    {
        Mirante,
        Cachoeira,
        Formacao,
        Fauna,
        Flora,
        Historico,
        Descanso,
        Perigo
    }

    public enum SeveridadeAlerta
    {
        Baixa,
        Media,
        Alta,
        Critica
    }
}
