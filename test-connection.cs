using Npgsql;

try
{
    var connectionString = "Host=localhost;Port=5432;Database=postgres;Username=1234;Password=1234;";
    
    using var connection = new NpgsqlConnection(connectionString);
    connection.Open();
    
    Console.WriteLine("✅ Conexão com PostgreSQL bem-sucedida!");
    
    // Tentar listar os bancos de dados
    using var command = new NpgsqlCommand("SELECT datname FROM pg_database WHERE datistemplate = false;", connection);
    using var reader = command.ExecuteReader();
    
    Console.WriteLine("\n📋 Bancos de dados disponíveis:");
    while (reader.Read())
    {
        Console.WriteLine($"  - {reader.GetString(0)}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro na conexão: {ex.Message}");
    
    // Tentar com o usuário postgres padrão
    Console.WriteLine("\n🔄 Tentando com usuário 'postgres'...");
    
    try
    {
        var connectionString2 = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=1234;";
        
        using var connection2 = new NpgsqlConnection(connectionString2);
        connection2.Open();
        
        Console.WriteLine("✅ Conexão com usuário 'postgres' bem-sucedida!");
    }
    catch (Exception ex2)
    {
        Console.WriteLine($"❌ Também falhou com postgres: {ex2.Message}");
        Console.WriteLine("\n💡 Sugestões:");
        Console.WriteLine("1. Verifique se o PostgreSQL está rodando");
        Console.WriteLine("2. Confirme as credenciais corretas");
        Console.WriteLine("3. Tente acessar via pgAdmin ou outro cliente");
    }
}

Console.WriteLine("\nPressione qualquer tecla para continuar...");
Console.ReadKey();
