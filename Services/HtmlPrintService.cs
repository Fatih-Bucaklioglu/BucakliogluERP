using System;
using System.IO;
using System.Text;
using BucakliogluERP.Models;

namespace BucakliogluERP.Services
{
    public class HtmlPrintService
    {
        public enum PrintFormat
        {
            A4,
            A5
        }

        public void SaveToFile(string html, string filePath)
        {
            File.WriteAllText(filePath, html, Encoding.UTF8);
        }

        public string CreateA5Html(Siparis siparis)
        {
            string musteriAdi = siparis.Musteri?.Muhattap ?? "Belirtilmemiş";
            string telefon = siparis.Musteri?.Telefon ?? "-";
            string tarih = siparis.Tarih.ToString("dd.MM.yyyy");
            string genelToplam = siparis.GenelToplam.ToString("N2");

            StringBuilder sb = new StringBuilder();
            sb.Append("<!DOCTYPE html><html lang='tr'><head><meta charset='UTF-8'>");
            sb.Append("<title>Sipariş - A5</title><style>");
            sb.Append("body{font-family:Arial;background:#f0f0f0;display:flex;justify-content:center;padding:20px;font-size:12pt;}");
            sb.Append(".invoice-container{width:210mm;height:148mm;background:white;padding:8mm;box-shadow:0 0 15px rgba(0,0,0,0.1);}");
            sb.Append(".top-section{display:flex;justify-content:space-between;margin-bottom:15px;}");
            sb.Append(".header{display:flex;gap:15px;width:50%;}");
            sb.Append(".logo-placeholder{width:50px;height:50px;border:2px solid #333;border-radius:6px;display:flex;justify-content:center;align-items:center;font-size:28pt;font-weight:bold;}");
            sb.Append(".header-text h1{margin:0;font-size:1.5em;}");
            sb.Append(".header-text p{margin:2px 0 0;font-size:0.85em;color:#555;}");
            sb.Append(".info-section{width:45%;}");
            sb.Append(".info-box h2{font-size:1.1em;border-bottom:1px solid #f0f0f0;padding-bottom:4px;margin-bottom:6px;}");
            sb.Append(".info-box p{margin:4px 0;}");
            sb.Append("table{width:100%;border-collapse:collapse;margin-top:10px;}");
            sb.Append("th,td{border-bottom:1px solid #ddd;padding:5px 4px;}");
            sb.Append("thead th{background:#f9f9f9;text-align:left;font-weight:600;}");
            sb.Append("tbody tr{font-weight:bold;}");
            sb.Append(".totals-section{float:right;width:45%;margin-top:10px;}");
            sb.Append(".grand-total{font-size:1.2em;font-weight:bold;border-top:2px solid #333;padding-top:6px;display:flex;justify-content:space-between;}");
            sb.Append("@media print{@page{size:A5 landscape;margin:0;}body{padding:0;background:white;}.invoice-container{box-shadow:none;}}");
            sb.Append("</style></head><body><div class='invoice-container'>");

            sb.Append("<div class='top-section'><div class='header'>");
            sb.Append("<div class='logo-placeholder'>B</div>");
            sb.Append("<div class='header-text'><h1>Bucaklıoğlu</h1>");
            sb.Append("<p>Saç &amp; Profil &amp; Demir LTD. ŞTİ.</p>");
            sb.Append("<p>ERP SİSTEMİ V2.0</p></div></div>");

            sb.Append("<div class='info-section'><div class='info-box'>");
            sb.Append("<h2>SİPARİŞ BİLGİLERİ</h2>");
            sb.Append($"<p>Müşteri Adı: {musteriAdi}</p>");
            sb.Append($"<p>Telefon: {telefon}</p>");
            sb.Append($"<p>Tarih: {tarih}</p>");
            sb.Append("</div></div></div>");

            sb.Append("<div><h2>SİPARİŞ DETAYLARI</h2>");
            sb.Append("<table><thead><tr><th colspan='2'>ADI</th><th>ADET</th><th>FİYAT</th><th>TOPLAM</th></tr></thead><tbody>");

            foreach (var urun in siparis.Urunler)
            {
                sb.Append($"<tr><td>{urun.Kategori}</td><td>{urun.Ebat}</td>");
                sb.Append($"<td>{urun.Miktar:N0}</td>");
                sb.Append($"<td>{urun.BirimFiyat:N2}</td>");
                sb.Append($"<td>{urun.Tutar:N2} ₺</td></tr>");
            }

            sb.Append("</tbody></table></div>");
            sb.Append($"<div class='totals-section'><div class='grand-total'><span>GENEL TOPLAM:</span><span>{genelToplam} ₺</span></div></div>");
            sb.Append("</div></body></html>");

            return sb.ToString();
        }

        public string CreateA4Html(Siparis siparis)
        {
            string musteriAdi = siparis.Musteri?.Muhattap ?? "Belirtilmemiş";
            string telefon = siparis.Musteri?.Telefon ?? "-";
            string firma = siparis.Musteri?.Firma ?? "-";
            string siparisNo = siparis.SiparisNo;
            string tarih = siparis.Tarih.ToString("dd.MM.yyyy");
            string iskonto = siparis.IskontoOrani;

            StringBuilder sb = new StringBuilder();
            sb.Append("<!DOCTYPE html><html lang='tr'><head><meta charset='UTF-8'>");
            sb.Append("<title>Sipariş - A4</title><style>");
            sb.Append("body{font-family:Arial;background:#f5f5f5;padding:20px;font-size:11pt;}");
            sb.Append(".invoice-container{width:210mm;background:white;padding:20mm;margin:0 auto;box-shadow:0 0 20px rgba(0,0,0,0.1);}");
            sb.Append(".header{display:flex;justify-content:space-between;border-bottom:3px solid #0066cc;padding-bottom:20px;margin-bottom:30px;}");
            sb.Append(".company-logo{width:80px;height:80px;background:linear-gradient(135deg,#0066cc,#004999);border-radius:12px;display:flex;align-items:center;justify-content:center;color:white;font-size:40pt;font-weight:bold;margin-bottom:15px;}");
            sb.Append(".company-info h1{margin:0;font-size:24pt;color:#0066cc;}");
            sb.Append(".company-info p{margin:5px 0;color:#666;font-size:10pt;}");
            sb.Append(".invoice-info{text-align:right;background:#f8f9fa;padding:20px;border-radius:8px;}");
            sb.Append(".invoice-info h2{margin:0 0 15px 0;font-size:20pt;}");
            sb.Append(".invoice-number{font-size:14pt;font-weight:bold;color:#0066cc;}");
            sb.Append(".customer-section{margin-bottom:30px;background:#f8f9fa;padding:20px;border-radius:8px;}");
            sb.Append(".customer-section h3{margin:0 0 15px 0;font-size:14pt;color:#0066cc;border-bottom:2px solid #0066cc;padding-bottom:8px;}");
            sb.Append("table{width:100%;border-collapse:collapse;margin:20px 0;}");
            sb.Append("thead{background:linear-gradient(135deg,#0066cc,#004999);color:white;}");
            sb.Append("th{padding:12px;text-align:left;font-weight:600;}");
            sb.Append("td{padding:12px;border-bottom:1px solid #e0e0e0;}");
            sb.Append("tbody tr:hover{background-color:#f8f9fa;}");
            sb.Append(".text-right{text-align:right;}");
            sb.Append(".text-center{text-align:center;}");
            sb.Append(".totals-section{margin-top:30px;float:right;width:350px;}");
            sb.Append(".total-row{display:flex;justify-content:space-between;padding:10px 15px;border-bottom:1px solid #e0e0e0;}");
            sb.Append(".total-row.grand-total{background:linear-gradient(135deg,#0066cc,#004999);color:white;font-size:16pt;font-weight:bold;border:none;margin-top:10px;border-radius:8px;}");
            sb.Append("@media print{@page{size:A4;margin:15mm;}body{background:white;padding:0;}.invoice-container{box-shadow:none;padding:0;}}");
            sb.Append("</style></head><body><div class='invoice-container'>");

            sb.Append("<div class='header'><div class='company-info'>");
            sb.Append("<div class='company-logo'>B</div>");
            sb.Append("<h1>BUCAKLIOĞLU</h1>");
            sb.Append("<p>Saç &amp; Profil &amp; Demir LTD. ŞTİ.</p>");
            sb.Append("</div><div class='invoice-info'>");
            sb.Append("<h2>SİPARİŞ FORMU</h2>");
            sb.Append($"<p><strong>Sipariş No:</strong></p><p class='invoice-number'>{siparisNo}</p>");
            sb.Append($"<p><strong>Tarih:</strong> {tarih}</p>");
            sb.Append($"<p><strong>İskonto:</strong> {iskonto}</p>");
            sb.Append("</div></div>");

            sb.Append("<div class='customer-section'><h3>MÜŞTERİ BİLGİLERİ</h3>");
            sb.Append($"<p><strong>Müşteri Adı:</strong> {musteriAdi}</p>");
            sb.Append($"<p><strong>Telefon:</strong> {telefon}</p>");
            sb.Append($"<p><strong>Firma:</strong> {firma}</p>");
            sb.Append("</div>");

            sb.Append("<table><thead><tr>");
            sb.Append("<th style='width:50px;'>No</th>");
            sb.Append("<th style='width:150px;'>Kategori</th>");
            sb.Append("<th>Ürün Adı</th>");
            sb.Append("<th class='text-center' style='width:100px;'>Miktar</th>");
            sb.Append("<th class='text-right' style='width:120px;'>Birim Fiyat</th>");
            sb.Append("<th class='text-right' style='width:140px;'>Toplam</th>");
            sb.Append("</tr></thead><tbody>");

            int sira = 1;
            foreach (var urun in siparis.Urunler)
            {
                sb.Append($"<tr><td class='text-center'>{sira}</td>");
                sb.Append($"<td>{urun.Kategori}</td>");
                sb.Append($"<td>{urun.Ebat}</td>");
                sb.Append($"<td class='text-center'>{urun.Miktar:N2}</td>");
                sb.Append($"<td class='text-right'>{urun.BirimFiyat:N2} ₺</td>");
                sb.Append($"<td class='text-right'><strong>{urun.Tutar:N2} ₺</strong></td></tr>");
                sira++;
            }

            sb.Append("</tbody></table>");
            sb.Append("<div class='totals-section'>");
            sb.Append($"<div class='total-row'><span>Ara Toplam:</span><span><strong>{siparis.ToplamTutar:N2} ₺</strong></span></div>");
            sb.Append($"<div class='total-row'><span>KDV (%20):</span><span><strong>{siparis.KDV:N2} ₺</strong></span></div>");
            sb.Append($"<div class='total-row grand-total'><span>GENEL TOPLAM:</span><span>{siparis.GenelToplam:N2} ₺</span></div>");
            sb.Append("</div>");
            sb.Append("</div></body></html>");

            return sb.ToString();
        }
    }
}