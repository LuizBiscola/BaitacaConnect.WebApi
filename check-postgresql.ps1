
# Verificar conexão na porta 5432
Write-Host "`nVerificando conectividade na porta 5432..." -ForegroundColor Yellow

try {
    $connection = Test-NetConnection -ComputerName localhost -Port 5432 -WarningAction SilentlyContinue
    if ($connection.TcpTestSucceeded) {
        Write-Host "✅ Porta 5432 está acessível" -ForegroundColor Green
    } else {
        Write-Host "❌ Porta 5432 não está acessível" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Erro ao testar porta: $($_.Exception.Message)" -ForegroundColor Red
}

# Sugestões de configuração
Write-Host "`n=== Configurações Sugeridas ===" -ForegroundColor Green
Write-Host "📝 Connection String atual:" -ForegroundColor Yellow
Write-Host "   Host=localhost;Database=baitaca_connect_dev;Username=postgres;Password=postgres;Port=5432" -ForegroundColor White

Write-Host "`n🔧 Se PostgreSQL estiver em porta diferente, atualize no appsettings.Development.json" -ForegroundColor Yellow
Write-Host "🔑 Se usuário/senha forem diferentes, atualize as credenciais" -ForegroundColor Yellow

Write-Host "`n=== Próximos Passos ===" -ForegroundColor Green
Write-Host "1. Certifique-se que PostgreSQL está instalado e rodando" -ForegroundColor White
Write-Host "2. Execute: dotnet ef database update" -ForegroundColor White
Write-Host "3. Se der erro, verifique as credenciais na connection string" -ForegroundColor White
