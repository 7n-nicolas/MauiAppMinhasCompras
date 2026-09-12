using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;

namespace MauiAppMinhasCompras.Views;

public partial class RelatorioCategoriaPage : ContentPage
{
    ObservableCollection<MauiAppMinhasCompras.Models.RelatorioCategoria> items = new ObservableCollection<MauiAppMinhasCompras.Models.RelatorioCategoria>();

    public RelatorioCategoriaPage()
    {
        InitializeComponent();

        lst_relatorio.ItemsSource = items;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        await CarregarRelatorio();
    }

    private async Task CarregarRelatorio()
    {
        try
        {
            items.Clear();

            var tmp = await App.Db.GetTotalPorCategoria();

            foreach (var r in tmp)
            {
                items.Add(r);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void ToolbarItem_Atualizar_Clicked(object sender, EventArgs e)
    {
        await CarregarRelatorio();
    }
}
