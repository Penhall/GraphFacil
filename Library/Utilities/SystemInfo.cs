// D:\PROJETOS\GraphFacil\Library\Utilities\SystemInfo.cs
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System;

﻿// D:\PROJETOS\GraphFacil\Library\Utilities\SystemInfo.cs
namespace LotoLibrary.Utilities
{
    /// <summary>
    /// Informações do sistema para diagnósticos
    /// </summary>
    public static class SystemInfo
    {
        public static string GetSystemDiagnostics()
        {
            var info = "=== DIAGNÓSTICO DO SISTEMA ===\n\n";

            try
            {
                // Informações da aplicação
                var assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                var location = assembly.Location;

                info += $"📱 APLICAÇÃO:\n";
                info += $"   Versão: {version}\n";
                info += $"   Localização: {Path.GetDirectoryName(location)}\n";
                info += $"   Data de Build: {File.GetLastWriteTime(location)}\n\n";

                // Informações do sistema
                info += $"💻 SISTEMA:\n";
                info += $"   OS: {Environment.OSVersion}\n";
                info += $"   .NET: {Environment.Version}\n";
                info += $"   Arquitetura: {Environment.Is64BitProcess} (64-bit Process)\n";
                info += $"   Máquina: {Environment.MachineName}\n";
                info += $"   Usuário: {Environment.UserName}\n";
                info += $"   Diretório Atual: {Environment.CurrentDirectory}\n\n";

                // Informações de memória
                var process = Process.GetCurrentProcess();
                info += $"🧠 MEMÓRIA:\n";
                info += $"   Uso Atual: {process.WorkingSet64 / 1024 / 1024:N0} MB\n";
                info += $"   Memória Privada: {process.PrivateMemorySize64 / 1024 / 1024:N0} MB\n";
                info += $"   GC Total: {GC.GetTotalMemory(false) / 1024 / 1024:N0} MB\n\n";

                // Informações de performance
                info += $"⚡ PERFORMANCE:\n";
                info += $"   Processors: {Environment.ProcessorCount}\n";
                info += $"   Tempo Ativo: {TimeSpan.FromMilliseconds(Environment.TickCount):hh\\:mm\\:ss}\n";
                info += $"   Threads Ativas: {process.Threads.Count}\n\n";

                // Verificação de arquivos críticos
                info += $"📁 ARQUIVOS CRÍTICOS:\n";
                var criticalFiles = new[]
                {
                    "Lotofacil.json",
                    "PercentuaisSS.json",
                    "PercentuaisNS.json",
                    "ContagemSS.json",
                    "ContagemNS.json"
                };

                foreach (var file in criticalFiles)
                {
                    var exists = File.Exists(file);
                    var status = exists ? "✅" : "❌";
                    var size = exists ? $"({new FileInfo(file).Length / 1024:N0} KB)" : "(Não encontrado)";
                    info += $"   {status} {file} {size}\n";
                }

                info += "\n";

                // Status dos componentes
                info += $"🔧 COMPONENTES:\n";
                info += $"   Infra.arLoto: {(Infra.arLoto?.Count ?? 0)} sorteios\n";
                info += $"   Infra.arGeral: {(Infra.arGeral?.Count ?? 0)} combinações\n";
                info += $"   Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";

            }
            catch (Exception ex)
            {
                info += $"❌ ERRO AO COLETAR INFORMAÇÕES: {ex.Message}\n";
            }

            return info;
        }

        public static void SaveDiagnosticsToFile(string filename = null)
        {
            try
            {
                filename ??= $"SystemDiagnostics_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var diagnostics = GetSystemDiagnostics();
                File.WriteAllText(filename, diagnostics);

                Debug.WriteLine($"Diagnósticos salvos em: {filename}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao salvar diagnósticos: {ex.Message}");
            }
        }

        public static bool IsSystemHealthy()
        {
            try
            {
                // Verificações básicas de saúde do sistema
                var hasData = Infra.arLoto?.Count > 0;
                var hasMemory = GC.GetTotalMemory(false) < 500 * 1024 * 1024; // < 500MB
                var hasFiles = File.Exists("Lotofacil.json");

                return hasData && hasMemory && hasFiles;
            }
            catch
            {
                return false;
            }
        }
    }
}
