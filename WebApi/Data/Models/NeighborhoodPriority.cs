namespace Api.Data.Models;

// Espelha o enum do Postgres: CREATE TYPE priority AS ENUM ('ALTA', 'MÉDIA', 'BAIXA')
// Os nomes dos membros DEVEM ser identicos aos valores do banco.
public enum NeighborhoodPriority
{
    ALTA,
    MÉDIA,
    BAIXA
}

