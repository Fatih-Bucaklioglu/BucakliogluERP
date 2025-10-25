using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using BucakliogluERP.Models;

namespace BucakliogluERP.Services
{
    public class DataService
    {
        private readonly string _dataPath;

        public DataService()
        {
            // Uygulama verileri için klasör
            string appDataPath = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData);
            _dataPath = Path.Combine(appDataPath, "BucakliogluERP");

            // Klasör yoksa oluştur
            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }
        }

        // Veri kaydetme
        public void SaveData<T>(string fileName, T data)
        {
            try
            {
                string filePath = Path.Combine(_dataPath, fileName);
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Veri kaydetme hatası: {ex.Message}");
            }
        }

        // Veri yükleme
        public T LoadData<T>(string fileName) where T : new()
        {
            try
            {
                string filePath = Path.Combine(_dataPath, fileName);

                if (!File.Exists(filePath))
                {
                    return new T();
                }

                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Veri yükleme hatası: {ex.Message}");
            }
        }

        // Siparişleri kaydet
        public void SaveSiparisler(List<Siparis> siparisler)
        {
            SaveData("siparisler.json", siparisler);
        }

        // Siparişleri yükle
        public List<Siparis> LoadSiparisler()
        {
            return LoadData<List<Siparis>>("siparisler.json") ?? new List<Siparis>();
        }

        // Cari hesapları kaydet
        public void SaveCariHesaplar(List<CariHesap> cariHesaplar)
        {
            SaveData("carihesaplar.json", cariHesaplar);
        }

        // Cari hesapları yükle
        public List<CariHesap> LoadCariHesaplar()
        {
            return LoadData<List<CariHesap>>("carihesaplar.json") ?? new List<CariHesap>();
        }

        // Müşterileri kaydet
        public void SaveMusteriler(List<Musteri> musteriler)
        {
            SaveData("musteriler.json", musteriler);
        }

        // Müşterileri yükle
        public List<Musteri> LoadMusteriler()
        {
            return LoadData<List<Musteri>>("musteriler.json") ?? new List<Musteri>();
        }
    }
}