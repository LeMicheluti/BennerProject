using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Benner.Services
{
    public class DataService<T>
    {
        private readonly string _filePath;

        public DataService(string fileName)
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", fileName);
            if (!Directory.Exists(Path.GetDirectoryName(_filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        public List<T> Carregar()
        {
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }

        public void Salvar(List<T> lista)
        {
            var json = JsonConvert.SerializeObject(lista, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public void Editar(T objeto)
        {
            if (objeto == null)
                throw new ArgumentNullException(nameof(objeto));

            var lista = Carregar();

            var idProp = typeof(T).GetProperty("Id") ?? throw new InvalidOperationException("A classe deve ter uma propriedade Id.");
            var idValor = idProp.GetValue(objeto);

            var itemExistente = lista.FirstOrDefault(x => idProp.GetValue(x)?.Equals(idValor) == true);

            if (itemExistente == null)
                throw new InvalidOperationException("Item não encontrado para edição.");

            var props = typeof(T).GetProperties().Where(p => p.Name != "Id");
            foreach (var prop in props)
            {
                var novoValor = prop.GetValue(objeto);
                prop.SetValue(itemExistente, novoValor);
            }

            Salvar(lista);
        }
    }
}
