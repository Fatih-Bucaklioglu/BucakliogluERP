using System;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using BucakliogluERP.Models;

namespace BucakliogluERP.Services
{
    public class PdfExportService
    {
        public PdfExportService()
        {
            // QuestPDF Lisans (Community - Ücretsiz)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public void SiparisExport(Siparis siparis, string dosyaYolu)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .Height(100)
                        .Background(Colors.Blue.Lighten4)
                        .Padding(20)
                        .Column(column =>
                        {
                            column.Item().Text("BUCAKLIOĞLU SAÇ & PROFİL & DEMİR LTD. ŞTİ.")
                                .FontSize(18)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item().Text("Sipariş Formu")
                                .FontSize(14)
                                .SemiBold()
                                .FontColor(Colors.Grey.Darken1);
                        });

                    page.Content()
                        .PaddingVertical(10)
                        .Column(column =>
                        {
                            // Sipariş Bilgileri
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("MÜŞTERİ BİLGİLERİ").Bold().FontSize(12);
                                    col.Item().PaddingTop(5).Text($"Müşteri: {siparis.Musteri?.Muhattap ?? ""}");
                                    col.Item().Text($"Telefon: {siparis.Musteri?.Telefon ?? ""}");
                                    col.Item().Text($"Firma: {siparis.Musteri?.Firma ?? ""}");
                                });

                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("SİPARİŞ BİLGİLERİ").Bold().FontSize(12);
                                    col.Item().PaddingTop(5).Text($"Sipariş No: {siparis.SiparisNo}");
                                    col.Item().Text($"Tarih: {siparis.Tarih:dd.MM.yyyy}");
                                    if (!string.IsNullOrEmpty(siparis.IskontoOrani))
                                    {
                                        col.Item().Text($"İskonto: {siparis.IskontoOrani}");
                                    }
                                });
                            });

                            // Boşluk
                            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            // Ürün Tablosu
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(40);  // No
                                    columns.RelativeColumn(2);   // Kategori
                                    columns.RelativeColumn(3);   // Ürün
                                    columns.RelativeColumn(1.5f);  // Miktar
                                    columns.RelativeColumn(2);   // Birim Fiyat
                                    columns.RelativeColumn(2);   // Toplam
                                });

                                // Başlık
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("No").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Kategori").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Ürün").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Miktar").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Birim Fiyat").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten3).Padding(5).Text("Toplam").Bold();
                                });

                                // Satırlar
                                int sira = 1;
                                foreach (var urun in siparis.Urunler)
                                {
                                    var bgColor = sira % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                                    table.Cell().Background(bgColor).Padding(5).Text(sira.ToString());
                                    table.Cell().Background(bgColor).Padding(5).Text(urun.Kategori);
                                    table.Cell().Background(bgColor).Padding(5).Text(urun.Ebat);
                                    table.Cell().Background(bgColor).Padding(5).Text(urun.Miktar.ToString("N2"));
                                    table.Cell().Background(bgColor).Padding(5).Text($"{urun.BirimFiyat:N2} ₺");
                                    table.Cell().Background(bgColor).Padding(5).Text($"{urun.Tutar:N2} ₺").Bold();

                                    sira++;
                                }
                            });

                            // Toplam Bölümü
                            column.Item().PaddingTop(20).AlignRight().Column(col =>
                            {
                                col.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Ara Toplam:");
                                    row.ConstantItem(120).Text($"{siparis.ToplamTutar:N2} ₺").AlignRight();
                                });

                                col.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("KDV (%20):");
                                    row.ConstantItem(120).Text($"{siparis.KDV:N2} ₺").AlignRight();
                                });

                                col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                                col.Item().PaddingTop(5).Row(row =>
                                {
                                    row.RelativeItem().Text("GENEL TOPLAM:").Bold().FontSize(14);
                                    row.ConstantItem(120).Text($"{siparis.GenelToplam:N2} ₺")
                                        .Bold()
                                        .FontSize(14)
                                        .FontColor(Colors.Green.Darken2)
                                        .AlignRight();
                                });
                            });
                        });

                    page.Footer()
                        .Height(40)
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Oluşturma Tarihi: ");
                            text.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm")).Bold();
                            text.Span(" | Bucaklıoğlu ERP Sistemi v2.0");
                        })
                        .FontSize(9)
                        .FontColor(Colors.Grey.Darken1);
                });
            })
            .GeneratePdf(dosyaYolu);
        }

        public void CariHesapExport(CariHesap cariHesap, string dosyaYolu)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .Height(100)
                        .Background(Colors.Teal.Lighten4)
                        .Padding(20)
                        .Column(column =>
                        {
                            column.Item().Text("CARİ HESAP EKSTRESİ")
                                .FontSize(18)
                                .Bold()
                                .FontColor(Colors.Teal.Darken2);

                            column.Item().Text(cariHesap.Musteri)
                                .FontSize(14)
                                .SemiBold();
                        });

                    page.Content()
                        .PaddingVertical(10)
                        .Column(column =>
                        {
                            // Özet
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Toplam Borç:").FontSize(12);
                                    col.Item().Text($"{cariHesap.Borc:N2} ₺")
                                        .FontSize(18)
                                        .Bold()
                                        .FontColor(Colors.Red.Darken1);
                                });

                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Toplam Alacak:").FontSize(12);
                                    col.Item().Text($"{cariHesap.Alacak:N2} ₺")
                                        .FontSize(18)
                                        .Bold()
                                        .FontColor(Colors.Green.Darken1);
                                });

                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Bakiye:").FontSize(12);
                                    col.Item().Text($"{cariHesap.Bakiye:N2} ₺")
                                        .FontSize(18)
                                        .Bold()
                                        .FontColor(cariHesap.Bakiye < 0 ? Colors.Red.Darken2 : Colors.Green.Darken2);
                                });
                            });

                            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            // İşlem Geçmişi
                            column.Item().PaddingTop(10).Text("İşlem Geçmişi").Bold().FontSize(14);

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);   // Tarih
                                    columns.RelativeColumn(1);   // Tip
                                    columns.RelativeColumn(4);   // Açıklama
                                    columns.RelativeColumn(2);   // Tutar
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Teal.Lighten3).Padding(5).Text("Tarih").Bold();
                                    header.Cell().Background(Colors.Teal.Lighten3).Padding(5).Text("Tip").Bold();
                                    header.Cell().Background(Colors.Teal.Lighten3).Padding(5).Text("Açıklama").Bold();
                                    header.Cell().Background(Colors.Teal.Lighten3).Padding(5).Text("Tutar").Bold();
                                });

                                int sira = 1;
                                foreach (var islem in cariHesap.Islemler.OrderByDescending(i => i.Tarih))
                                {
                                    var bgColor = sira % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;
                                    var tutar = islem.Tip == "tahsilat" ? $"+{islem.Tutar:N2} ₺" : $"-{islem.Tutar:N2} ₺";
                                    var color = islem.Tip == "tahsilat" ? Colors.Green.Darken1 : Colors.Red.Darken1;

                                    table.Cell().Background(bgColor).Padding(5).Text(islem.Tarih.ToString("dd.MM.yyyy HH:mm"));
                                    table.Cell().Background(bgColor).Padding(5).Text(islem.Tip == "tahsilat" ? "Tahsilat" : "Borç");
                                    table.Cell().Background(bgColor).Padding(5).Text(islem.Aciklama);
                                    table.Cell().Background(bgColor).Padding(5).Text(tutar).FontColor(color).Bold();

                                    sira++;
                                }
                            });
                        });

                    page.Footer()
                        .Height(40)
                        .AlignCenter()
                        .Text($"Oluşturma Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm} | Bucaklıoğlu ERP Sistemi v2.0")
                        .FontSize(9)
                        .FontColor(Colors.Grey.Darken1);
                });
            })
            .GeneratePdf(dosyaYolu);
        }
    }
}