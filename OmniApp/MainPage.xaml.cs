using System;
using System.Numerics;
using System.Data;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace OmniApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnBerechnenGeklickt(object sender, EventArgs e)
    {
        // Wenn man vergisst, einen Modus auszuwählen
        if (ModusPicker.SelectedIndex == -1)
        {
            ErgebnisLabel.Text = "Fehler: Bitte wähle zuerst einen Modus aus!";
            ErgebnisLabel.TextColor = Colors.Red;
            return;
        }

        string eingabe = EingabeFeld.Text;
        if (string.IsNullOrWhiteSpace(eingabe)) return;

        ErgebnisLabel.TextColor = Colors.LimeGreen;

        try
        {
            // Modus 0: Binär zu Dezimal
            if (ModusPicker.SelectedIndex == 0)
            {
                BigInteger zb = BigInteger.Parse(eingabe);
                BigInteger dez = 0; int pot = 0;
                while (zb > 0) { dez += (zb % 10) * BigInteger.Pow(2, pot); zb /= 10; pot++; }
                ErgebnisLabel.Text = $"Ergebnis: {dez}";
            }
            // Modus 1: Dezimal zu Binär
            else if (ModusPicker.SelectedIndex == 1)
            {
                BigInteger zd = BigInteger.Parse(eingabe);
                string bin = "";
                while (zd > 0) { bin = (zd % 2).ToString() + bin; zd /= 2; }
                if (bin == "") bin = "0";
                ErgebnisLabel.Text = $"Ergebnis: {bin}";
            }
            // Modus 2: Der Einzeiler (DataTable)
            else if (ModusPicker.SelectedIndex == 2)
            {
                string gleichung = eingabe.Replace(',', '.');
                var result = new DataTable().Compute(gleichung, null);
                ErgebnisLabel.Text = $"{gleichung} = {Convert.ToDouble(result)}";
            }
            // Modus 3: Live-Währungen (Alle!)
            else if (ModusPicker.SelectedIndex == 3)
            {
                if (double.TryParse(eingabe, out double euro))
                {
                    ErgebnisLabel.Text = "Lade Kurse aus dem Internet...";
                    
                    using HttpClient client = new HttpClient();
                    string json = await client.GetStringAsync("https://open.er-api.com/v6/latest/EUR");
                    using JsonDocument doc = JsonDocument.Parse(json);
                    var rates = doc.RootElement.GetProperty("rates");

                    string ausgabe = $"--- {euro} € in alle Währungen ---\n";
                    foreach (JsonProperty rate in rates.EnumerateObject())
                    {
                        double umgerechnet = Math.Round(euro * rate.Value.GetDouble(), 2);
                        ausgabe += $"{umgerechnet} {rate.Name}\n";
                    }
                    ErgebnisLabel.Text = ausgabe;
                }
                else { ErgebnisLabel.Text = "Fehler: Ungültiger Euro-Betrag!"; ErgebnisLabel.TextColor = Colors.Red; }
            }
        }
        catch
        {
            ErgebnisLabel.Text = "Fehler: Ungültige Eingabe für diesen Modus!";
            ErgebnisLabel.TextColor = Colors.Red;
        }
    }
}