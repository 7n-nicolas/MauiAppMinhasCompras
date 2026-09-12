using MauiAppMinhasCompras.Models;
using System;
using System.Linq;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiAppMinhasCompras.Views;

public partial class EditarProduto : ContentPage
{
	public EditarProduto()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        picker_categoria.ItemsSource = Enum.GetNames(typeof(CategoriaProduto));

        if (BindingContext is Produto p)
        {
            picker_categoria.SelectedItem = p.Categoria.ToString();
            // popula campos de texto com valores atuais formatados
            txt_descricao.Text = p.Descricao;
            txt_quantidade.Text = p.Quantidade.ToString("G", CultureInfo.CurrentCulture);
            txt_preco.Text = p.Preco.ToString("N2", CultureInfo.CurrentCulture);
        }
    }

    private void txt_preco_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if (entry == null) return;

        var newText = new string(entry.Text?.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
        if (newText != entry.Text)
        {
            entry.Text = newText;
        }
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Produto produto_anexado = BindingContext as Produto;

            // Normaliza e parseia valores numéricos aceitando diferentes formatos (ex: "26,90", "26.90", "1.234,56")
            double quantidade;
            double preco;

            string NormalizeNumberString(string input)
            {
                if (string.IsNullOrWhiteSpace(input)) return input;
                var s = input.Trim();
                s = s.Replace(" ", "");

                var nf = CultureInfo.CurrentCulture.NumberFormat;
                var dec = nf.NumberDecimalSeparator;
                var grp = nf.NumberGroupSeparator;

                if (s.Contains(".") && s.Contains(","))
                {
                    int lastDot = s.LastIndexOf('.');
                    int lastComma = s.LastIndexOf(',');
                    char actualDec = lastComma > lastDot ? ',' : '.';
                    char other = actualDec == ',' ? '.' : ',';
                    s = s.Replace(other.ToString(), "");
                    s = s.Replace(actualDec.ToString(), ".");
                }
                else
                {
                    if (!string.IsNullOrEmpty(grp))
                        s = s.Replace(grp, "");

                    if (!string.IsNullOrEmpty(dec))
                        s = s.Replace(dec, ".");
                }

                return s;
            }

            var qStr = NormalizeNumberString(txt_quantidade.Text);
            var pStr = NormalizeNumberString(txt_preco.Text);

            if (!double.TryParse(qStr, NumberStyles.Number, CultureInfo.InvariantCulture, out quantidade))
                throw new Exception("Quantidade inválida.");

            if (!double.TryParse(pStr, NumberStyles.Number, CultureInfo.InvariantCulture, out preco))
                throw new Exception("Preço inválido.");

            Produto p = new Produto
            {
                Id = produto_anexado.Id,
                Descricao = txt_descricao.Text,
                Quantidade = quantidade,
                Preco = preco,
                Categoria = Enum.TryParse<CategoriaProduto>(picker_categoria.SelectedItem?.ToString(), out var cat) ? cat : CategoriaProduto.Outros
            };

            await App.Db.Update(p);
            await DisplayAlert("Sucesso", "Registro Atualizado", "OK");
            await Navigation.PopAsync();

        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}