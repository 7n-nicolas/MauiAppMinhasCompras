using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    private Picker _pickerCategoria;
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
    public ListaProduto()
    {
        InitializeComponent();

        // obtem o elemento Picker criado no XAML e popula categorias (Todas + enum)
        _pickerCategoria = this.FindByName<Picker>("picker_categoria");
        var categorias = new List<string> { "Todas" };
        categorias.AddRange(Enum.GetNames(typeof(CategoriaProduto)));
        if (_pickerCategoria != null)
        {
            _pickerCategoria.ItemsSource = categorias;
            _pickerCategoria.SelectedIndex = 0;
        }

        lst_produtos.ItemsSource = lista;
    }
    protected async override void OnAppearing()
    {
        try
        {
            await LoadProdutos();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new Views.NovoProduto());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            await LoadProdutos(e.NewTextValue);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        } finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);

        string msg = $"O total é {soma:C}";

        DisplayAlert("Total", msg, "OK");
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem selecionado = sender as MenuItem;

            Produto p = selecionado.BindingContext as Produto;

            bool confirm = await DisplayAlert(
                "Tem certeza?", $"Remover produto {p.Descricao}?", "Sim", "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try 
        {
            Produto p = e.SelectedItem as Produto;

            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void ToolbarItem_Report_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new Views.RelatorioCategoriaPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {
            await LoadProdutos(txt_search.Text);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        } finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async Task LoadProdutos(string q = null)
    {
        lst_produtos.IsRefreshing = true;

        try
        {
            lista.Clear();

            List<Produto> tmp;

            if (!string.IsNullOrWhiteSpace(q))
            {
                tmp = await App.Db.Search(q);
            }
            else
            {
                tmp = await App.Db.GetAll();
            }

            // filtra por categoria selecionada
            string sel = _pickerCategoria?.SelectedItem?.ToString() ?? "Todas";
            if (sel != "Todas")
            {
                tmp = tmp.Where(i => i.Categoria.ToString() == sel).ToList();
            }

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async void picker_categoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        await LoadProdutos(txt_search.Text);
    }
}