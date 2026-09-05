using System;
using SQLite;

namespace MauiAppMinhasCompras.Models
{
    public class Produto
    {
        string _descricao;
        double _quantidade;
        double _preco;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Descricao
        {
            get => _descricao;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("A descrição do produto não pode ser vazia.");
                }
                _descricao = value;
            }
        }
        public double Quantidade
        {
            get => _quantidade;
            set
            {
                if (value <= 0)
                {
                    throw new Exception("A quantidade do produto deve ser um valor positivo.");
                }
                _quantidade = value;
            }
        }
        public double Preco
        {
            get => _preco;
            set
            {
                if (value <= 0)
                {
                    throw new Exception("O preço do produto deve ser um valor positivo.");
                }
                _preco = value;
            }
        }

        public double Total { get => Quantidade * Preco; }
    }
}
