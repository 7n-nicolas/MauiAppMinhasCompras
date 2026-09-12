using MauiAppMinhasCompras.Models;
using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using System;
using System.Linq;

namespace MauiAppMinhasCompras.Views;

public partial class NovoProduto : ContentPage
{
	public NovoProduto()
	{
		InitializeComponent();

		picker_categoria.ItemsSource = Enum.GetNames(typeof(CategoriaProduto));
	}

	private async void ToolbarItem_Clicked(object sender, EventArgs e)
	{
		try
		{
			// Parse seguro de valores numéricos aceitando ',' ou '.' conforme cultura
			double quantidade;
			double preco;

			// Normaliza e parseia valores numéricos aceitando diferentes formatos (ex: "26,90", "26.90", "1.234,56")
			string NormalizeNumberString(string input)
			{
				if (string.IsNullOrWhiteSpace(input)) return input;
				var s = input.Trim();
				s = s.Replace(" ", "");

				var nf = CultureInfo.CurrentCulture.NumberFormat;
				var dec = nf.NumberDecimalSeparator; // geralmente "," em pt-BR
				var grp = nf.NumberGroupSeparator; // geralmente "." em pt-BR

				if (s.Contains(".") && s.Contains(","))
				{
					// ambos presentes: o último símbolo é o separador decimal presumido
					int lastDot = s.LastIndexOf('.');
					int lastComma = s.LastIndexOf(',');
					char actualDec = lastComma > lastDot ? ',' : '.';
					char other = actualDec == ',' ? '.' : ',';
					s = s.Replace(other.ToString(), ""); // remove separador de milhar
					s = s.Replace(actualDec.ToString(), "."); // padroniza decimal como ponto
				}
				else
				{
					// remove separador de milhar do culture atual, se presente
					if (!string.IsNullOrEmpty(grp))
						s = s.Replace(grp, "");

					// substitui separador decimal da cultura atual por ponto
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
				Descricao = txt_descricao.Text,
				Quantidade = quantidade,
				Preco = preco,
				Categoria = Enum.TryParse<CategoriaProduto>(picker_categoria.SelectedItem?.ToString(), out var cat) ? cat : CategoriaProduto.Outros
			};

			await App.Db.Insert(p);
			await DisplayAlert("Sucesso", "Registro Inserido", "OK");
            await Navigation.PopAsync();

        }
		catch (Exception ex)
		{
			await DisplayAlert("Ops", ex.Message, "OK");
		}
	}

	private void txt_preco_TextChanged(object sender, TextChangedEventArgs e)
	{
		// Permitir apenas dígitos, vírgula e ponto; não bloquear a edição do usuário
		// Aqui apenas evita caracteres inválidos
		var entry = sender as Entry;
		if (entry == null) return;

		var newText = new string(entry.Text?.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
		if (newText != entry.Text)
		{
			entry.Text = newText;
		}
	}
}