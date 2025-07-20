using Npgsql;

Console.WriteLine("=== Teste de Conexão PostgreSQL - Múltiplas Portas ===");

var ports = new[] { 5432, 5433, 5434 };
var connectionFound = false;

foreach (var port in ports)
{
    var connectionString = $"Host=localhost;Port={port};Database=postgres;Username=postgres;Password=postgres;Timeout=5;";
    
    Console.WriteLine($"\n🔍 Testando porta {port}...");
    
    try
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        Console.WriteLine($"✅ Conexão bem-sucedida na porta {port}!");
        
        using var command = new NpgsqlCommand("SELECT version();", connection);
        var version = command.ExecuteScalar();
        Console.WriteLine($"📋 Versão: {version}");
        
        Console.WriteLine($"\n🔧 Use esta connection string:");
        Console.WriteLine($"   Host=localhost;Port={port};Database=baitaca_connect;Username=postgres;Password=postgres;");
        
        connectionFound = true;
        break;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Porta {port}: {ex.Message}");
    }
}

if (!connectionFound)
{
    Console.WriteLine("\n❌ Nenhuma conexão PostgreSQL encontrada!");
    Console.WriteLine("\n🛠️ Soluções:");
    Console.WriteLine("1. Instalar PostgreSQL: winget install PostgreSQL.PostgreSQL");
    Console.WriteLine("2. Ou baixar de: https://www.postgresql.org/download/windows/");
    Console.WriteLine("3. Verificar se o serviço está rodando no Gerenciador de Tarefas");
}

Console.WriteLine("\nPressione qualquer tecla para sair...");
Console.ReadKey();
