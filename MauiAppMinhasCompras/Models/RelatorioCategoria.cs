using System;
namespace MauiAppMinhasCompras.Models
{
    public class RelatorioCategoria
    {
        // SQLite-net irá mapear o valor inteiro da coluna Categoria para este enum
        public CategoriaProduto Categoria { get; set; }

        // Total gasto (Quantidade * Preco)
        public double Total { get; set; }
    }
}
