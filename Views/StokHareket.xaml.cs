using OfficeOpenXml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

< Page x: Class = "BucakliogluERP.Views.StokPage"
      xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns: x = "http://schemas.microsoft.com/winfx/2006/xaml"
      Title = "Stok Yönetimi"
      Background = "Transparent" >

    < Grid Margin = "20" >
        < Border Background = "White"
                CornerRadius = "10"
                Padding = "30" >
            < Border.Effect >
                < DropShadowEffect BlurRadius = "10" ShadowDepth = "2" Opacity = "0.1" />
            </ Border.Effect >


            < StackPanel >
                < TextBlock Text = "Stok Yönetimi"
                          FontSize = "28"
                          FontWeight = "Bold"
                          Foreground = "#2C3E50"
                          Margin = "0,0,0,30" />

                < TextBlock Text = "Stok takibi yakında eklenecek!"
                          FontSize = "16"
                          Foreground = "#6C757D"
                          HorizontalAlignment = "Center"
                          VerticalAlignment = "Center"
                          Margin = "0,100,0,0" />
            </ StackPanel >
        </ Border >
    </ Grid >
</ Page >