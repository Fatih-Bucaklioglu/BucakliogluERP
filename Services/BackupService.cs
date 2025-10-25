using System;
using System.IO;
using System.IO.Compression;
using Timers = System.Timers;

namespace BucakliogluERP.Services
{
    public class BackupService
    {
        private readonly Timers.Timer _timer;
        private readonly string _dataPath;
        private readonly string _backupPath;

        public BackupService()
        {
            _dataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "BucakliogluERP");

            _backupPath = Path.Combine(_dataPath, "Backups");

            if (!Directory.Exists(_backupPath))
                Directory.CreateDirectory(_backupPath);

            _timer = new Timers.Timer(60000);
            _timer.Elapsed += Timer_Elapsed;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void Timer_Elapsed(object? sender, Timers.ElapsedEventArgs e)
        {
            // Devamı aynı...
            var now = DateTime.Now;

            // Saat 23:00 ise yedek al
            if (now.Hour == 23 && now.Minute == 0)
            {
                CreateBackup();
            }

            // Eski yedekleri temizle (30 günden eski)
            CleanOldBackups();
        }

        public void CreateBackup()
        {
            try
            {
                string backupFileName = $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
                string backupFilePath = Path.Combine(_backupPath, backupFileName);

                // Geçici klasör
                string tempPath = Path.Combine(Path.GetTempPath(), "BucakliogluERP_Backup");
                if (Directory.Exists(tempPath))
                    Directory.Delete(tempPath, true);

                Directory.CreateDirectory(tempPath);

                // Tüm JSON dosyalarını kopyala
                foreach (var file in Directory.GetFiles(_dataPath, "*.json"))
                {
                    File.Copy(file, Path.Combine(tempPath, Path.GetFileName(file)));
                }

                // ZIP oluştur
                if (File.Exists(backupFilePath))
                    File.Delete(backupFilePath);

                ZipFile.CreateFromDirectory(tempPath, backupFilePath);
                Directory.Delete(tempPath, true);

                System.Diagnostics.Debug.WriteLine($"Yedek oluşturuldu: {backupFileName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Yedekleme hatası: {ex.Message}");
            }
        }

        private void CleanOldBackups()
        {
            try
            {
                var files = Directory.GetFiles(_backupPath, "Backup_*.zip");

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);

                    // 30 günden eski dosyaları sil
                    if ((DateTime.Now - fileInfo.CreationTime).TotalDays > 30)
                    {
                        File.Delete(file);
                        System.Diagnostics.Debug.WriteLine($"Eski yedek silindi: {fileInfo.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eski yedek temizleme hatası: {ex.Message}");
            }
        }
    }
}