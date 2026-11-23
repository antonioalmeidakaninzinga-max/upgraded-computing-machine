using System;
using System.Collections.Generic;
using System.Linq;

var clientes = new List<Cliente>();
int proximoId = 1;

while (true)
{
    Console.WriteLine("\n=== LOJA DE PRODUTOS ALIMENTARES ===");
    Console.WriteLine("1 - Registar cliente");
    Console.WriteLine("2 - Consultar factura");
    Console.WriteLine("3 - Ver fila de atendimento");
    Console.WriteLine("4 - Atender próximo cliente");
    Console.WriteLine("0 - Sair");
    Console.Write("Opção: ");

    switch (Console.ReadLine())
    {
        case "1": Registar(); break;
        case "2": ConsultarFactura(); break;
        case "3": MostrarFila(); break;
        case "4": AtenderProximo(); break;
        case "0": return;
        default: Console.WriteLine("Opção inválida!"); break;
    }
}

// ---------- FUNÇÕES ----------

void Registar()
{
    var c = new Cliente { Id = proximoId++ };

    Console.Write("Nome: ");
    c.Nome = Console.ReadLine() ?? "";

    Console.Write("Género (M/F): ");
    c.Genero = (Console.ReadLine() ?? "").Trim().ToUpper() == "F" ? "Feminino" : "Masculino";

    c.Total = LerValor("Valor total das compras (Kz): ");

    // Só mulheres podem ser grávidas
    if (c.Genero == "Feminino")
        c.Gravida = LerSimNao("É grávida? (S/N): ");

    c.Idoso = LerSimNao("É pessoa idosa? (S/N): ");
    c.Deficiente = LerSimNao("Tem deficiência? (S/N): ");

    clientes.Add(c);
    Console.WriteLine($"\nCliente registado com o nº {c.Id}.");
}

void ConsultarFactura()
{
    Console.Write("Nº do cliente: ");
    int.TryParse(Console.ReadLine(), out int id);
    var c = clientes.FirstOrDefault(x => x.Id == id);

    if (c == null) { Console.WriteLine("Cliente não encontrado."); return; }

    Console.WriteLine("\n------------- FACTURA -------------");
    Console.WriteLine($"Cliente nº:     {c.Id}");
    Console.WriteLine($"Nome:           {c.Nome}");
    Console.WriteLine($"Género:         {c.Genero}");
    Console.WriteLine($"Total compras:  {c.Total:N2} Kz");
    Console.WriteLine($"Bónus (10%):    {c.Bonus:N2} Kz");
    Console.WriteLine($"TOTAL A PAGAR:  {c.TotalAPagar:N2} Kz");
    Console.WriteLine($"Atendimento:    {c.TipoAtendimento}");
    Console.WriteLine("-----------------------------------");
}

void MostrarFila()
{
    var fila = ObterFila();
    if (!fila.Any()) { Console.WriteLine("Fila vazia."); return; }

    Console.WriteLine("\nFila de atendimento:");
    int pos = 1;
    foreach (var c in fila)
        Console.WriteLine($"{pos++}º - {c.Nome} (nº {c.Id}) - {c.TipoAtendimento}");
}

void AtenderProximo()
{
    var c = ObterFila().FirstOrDefault();
    if (c == null) { Console.WriteLine("Não há clientes à espera."); return; }

    c.Atendido = true;
    Console.WriteLine($"A atender: {c.Nome} (nº {c.Id}) - {c.TipoAtendimento}");
}

// Prioritários primeiro; dentro de cada grupo, por ordem de chegada
List<Cliente> ObterFila() =>
    clientes.Where(c => !c.Atendido)
            .OrderByDescending(c => c.Prioritario)
            .ThenBy(c => c.Id)
            .ToList();

decimal LerValor(string texto)
{
    while (true)
    {
        Console.Write(texto);
        if (decimal.TryParse(Console.ReadLine(), out decimal v) && v >= 0) return v;
        Console.WriteLine("Valor inválido, tente novamente.");
    }
}

bool LerSimNao(string texto)
{
    Console.Write(texto);
    return (Console.ReadLine() ?? "").Trim().ToUpper() == "S";
}

// ---------- CLASSE ----------

class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Genero { get; set; } = "";
    public decimal Total { get; set; }
    public bool Gravida { get; set; }
    public bool Idoso { get; set; }
    public bool Deficiente { get; set; }
    public bool Atendido { get; set; }

    // Bónus: só se o total for SUPERIOR a 100 000 Kz
    public decimal Bonus => Total > 100000 ? Total * 0.10m : 0;
    public decimal TotalAPagar => Total - Bonus;

    // Prioridade no atendimento (independente do bónus)
    public bool Prioritario => Gravida || Idoso || Deficiente;
    public string TipoAtendimento => Prioritario ? "PRIORITÁRIO" : "Normal";
}
