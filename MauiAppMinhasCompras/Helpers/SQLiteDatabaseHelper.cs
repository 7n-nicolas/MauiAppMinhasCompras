using MauiAppMinhasCompras.Models;
using SQLite;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;
        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
            _conn.CreateTableAsync<Produto>().Wait();
        }

        public Task<int> Insert(Produto p) 
        {
            return _conn.InsertAsync(p);        
        }
        public Task<int> Update(Produto p)
        {
            // Usa API do SQLite-net para atualizar o registro (inclui todas as propriedades mapeadas)
            return _conn.UpdateAsync(p);
        }
        public Task<int> Delete(int id) 
        {
            return _conn.Table<Produto>().DeleteAsync(i => i.Id == id);
        }
        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>().ToListAsync();
        }
        public Task<List<Produto>> Search(string q)
        {
            string sql = "SELECT * FROM Produto WHERE Descricao LIKE '%" + q +"%'";

            return _conn.QueryAsync<Produto>(sql);
        }

        public Task<List<RelatorioCategoria>> GetTotalPorCategoria()
        {
            // Agrupa por Categoria e soma Quantidade * Preco
            string sql = "SELECT Categoria, SUM(Quantidade * Preco) as Total FROM Produto GROUP BY Categoria";

            return _conn.QueryAsync<RelatorioCategoria>(sql);
        }


    }
}
