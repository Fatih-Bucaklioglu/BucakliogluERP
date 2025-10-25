using System;
using System.Collections.Generic;
using System.Linq;
using BucakliogluERP.Models;

namespace BucakliogluERP.Helpers
{
    public static class AramaHelper
    {
        // Fuzzy search (benzerlik araması)
        public static List<Siparis> SiparisAra(List<Siparis> siparisler, string aramaMetni)
        {
            if (string.IsNullOrWhiteSpace(aramaMetni))
                return siparisler;

            aramaMetni = aramaMetni.ToLower().Trim();

            return siparisler.Where(s =>
                s.SiparisNo.ToLower().Contains(aramaMetni) ||
                (s.Musteri?.Muhattap?.ToLower().Contains(aramaMetni) ?? false) ||
                (s.Musteri?.Telefon?.Contains(aramaMetni) ?? false) ||
                (s.Musteri?.Firma?.ToLower().Contains(aramaMetni) ?? false) ||
                s.Urunler.Any(u => u.Ebat.ToLower().Contains(aramaMetni))
            ).ToList();
        }

        public static List<CariHesap> CariAra(List<CariHesap> cariHesaplar, string aramaMetni)
        {
            if (string.IsNullOrWhiteSpace(aramaMetni))
                return cariHesaplar;

            aramaMetni = aramaMetni.ToLower().Trim();

            return cariHesaplar.Where(c =>
                c.Musteri.ToLower().Contains(aramaMetni) ||
                c.Telefon.Contains(aramaMetni)
            ).ToList();
        }

        // Levenshtein mesafesi (yazım hatası toleransı)
        public static int LevenshteinDistance(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1)) return s2?.Length ?? 0;
            if (string.IsNullOrEmpty(s2)) return s1.Length;

            int[,] d = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                d[i, 0] = i;
            for (int j = 0; j <= s2.Length; j++)
                d[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    int cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[s1.Length, s2.Length];
        }
    }
}