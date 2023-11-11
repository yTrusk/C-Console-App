namespace Intro;

using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
      public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<PessoaCompras> PessoaCompras { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(""); // Use um banco de dados postgresql
    }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<PessoaCompras>()
        .HasKey(pc => pc.Id);

    modelBuilder.Entity<PessoaCompras>()
        .Property(pc => pc.Id)
        .ValueGeneratedNever();

    modelBuilder.Entity<Pessoa>()
        .HasMany(p => p.Compras)
        .WithOne(c => c.Pessoa);
}

}
public class PessoaCompras{
    public int Id { get; set; }
   public int PessoaId { get; set; }
    public int QuantidadeGasta { get; set; }
    public string ItemComprado { get; set; }
    public Pessoa Pessoa { get; set; }
}
public class Pessoa
{
    public int PessoaId { get; set; }
    public string PessoaUserName { get; set; }
    public string PessoaPassword { get; set; }
    public int PessoaIdade { get; set; }
    public bool Admin { get; set; }
    public ICollection<PessoaCompras> Compras { get; set; }
    
}


class Program
{
    static bool login(string name, string pass, string admin) {
        using (var context = new ApplicationDbContext()) {
            var usuario = context.Pessoas.FirstOrDefault(p => p.PessoaUserName == name);
            if (admin == "yes") {
                if (usuario.PessoaUserName == name && usuario.PessoaPassword == pass && usuario.Admin == true) {
                    return true;
                } else {
                    return false;
                }
            } else {
                if (usuario.PessoaUserName == name && usuario.PessoaPassword == pass) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }
    static bool ProcurarId(int id) {
        using (var context = new ApplicationDbContext())
        {

            var usuario = context.Pessoas.Find(id);

            if (usuario != null) {
                return true;
            } else {
                return false;
            }
        }
    }

      static bool ProcurarIdCompras(int id) {
        using (var context = new ApplicationDbContext())
        {

            var usuario = context.PessoaCompras.Find(id);

            if (usuario != null) {
                return true;
            } else {
                return false;
            }
        }
    }
    static bool ProcurarNome(string name) {
        using (var context = new ApplicationDbContext())
        {

            var usuario = context.Pessoas.FirstOrDefault(p => p.PessoaUserName == name);

            if (usuario != null) {
                return true;
            } else {
                return false;
            }
        }
    }
    static int GerarIdAleatorio()
    {
        Random random = new Random();
        return random.Next();
    }
    static int GerarIdNaoExistente() {
        int id;
        using (var context = new ApplicationDbContext())
        {
            do
            {
                id = GerarIdAleatorio();
            } while (ProcurarId(id) == true);
        }
        return id;
    }
     static int GerarIdProduto() {
        int id;
        using (var context = new ApplicationDbContext())
        {
            do
            {
                id = GerarIdAleatorio();
            } while (ProcurarIdCompras(id) == true);
        }
        return id;
    }
    static bool CriarUser() {
        Console.WriteLine("Digite o nome do seu usuário");
        string name = Console.ReadLine();
        if (ProcurarNome(name) == true) {
            Console.WriteLine("This name exists on our db, try again.");
            return false;
        }
        Console.WriteLine("Digite sua senha");
        string pass = Console.ReadLine();
        Console.WriteLine("Digite sua idade");
        int years = int.Parse(Console.ReadLine());

        var novapessoa = new Pessoa
        {
            PessoaId = GerarIdNaoExistente(),
            PessoaUserName = name,
            PessoaPassword = pass,
            PessoaIdade = years,
            Admin = true
        };

        using (var context = new ApplicationDbContext())
        {
            context.Pessoas.Add(novapessoa);
            context.SaveChanges();
            Console.WriteLine("Usuário criado com sucesso!");
            Console.WriteLine(novapessoa.PessoaId);
            Console.WriteLine(novapessoa.PessoaUserName);
        }
        return true;
    }
    static void AdminPage() {
        Console.Clear();
        Console.WriteLine("You has logged sucessfully");
        Console.WriteLine("1 - Cadastre usuários");
        Console.WriteLine("2 - Veja a lista de usuários");
        Console.WriteLine("3 - Remova um usuário");
        Console.WriteLine("4 - Torne um usuário Admin/Remova um usuário");
        int selected = int.Parse(Console.ReadLine());
        AdminFunctions(selected);
        return;
    }

    static void userlist() {
        using (var context = new ApplicationDbContext())
        {
            var usuarios = context.Pessoas.ToList();

            Console.WriteLine("Usuários cadastrados:");
            foreach (var usuario in usuarios)
            {
                Console.WriteLine($"ID: {usuario.PessoaId}, Nome: {usuario.PessoaUserName}, Idade: {usuario.PessoaIdade}, Admin? {usuario.Admin}");
            }
        }
        return;
    }
    static bool AdminFunctions(int selected = 0) {
        bool b = true;
        if (selected == 0) {
            AdminPage();
        }

        if (selected == 1) {
            bool c = CriarUser();
            if (c == true) {
                AdminPage();
            } else {
                AdminPage();
            }
        } else if (selected == 2) {
            userlist();
            Console.WriteLine("Voltar para a dashboard admin? (yes/no)");
            string s = Console.ReadLine();
            if (s == "yes") {
                AdminPage();
            } else {
                b = false;
                return b;
            }
        } else if (selected == 3) {
            Console.WriteLine("Digite o id do usuário.");
            int a = int.Parse(Console.ReadLine());
            bool x = RemoveUsers(a);
            if (x == true) {
                Console.WriteLine("Pessoa removida com sucesso.");
                Console.WriteLine("Voltar para a dashboard admin? (yes/no)");
                string s = Console.ReadLine();
                if (s == "yes") {
                    AdminPage();
                } else {
                    b = false;
                    return b;
                }
            } else {
                Console.WriteLine("Pessoa não encontrada.");
                Console.WriteLine("Voltar para a dashboard admin? (yes/no)");
                string s = Console.ReadLine();
                if (s == "yes") {
                    AdminPage();
                } else {
                    b = false;
                    return b;
                }
            }
        } else if (selected == 4) {
            Console.WriteLine("1 - Adicionar Admin");
            Console.WriteLine("2 - Remover um Admin");
            int p = int.Parse(Console.ReadLine());
            if (p == 1) {
                Console.WriteLine("Digite o id do usuário.");
                int a = int.Parse(Console.ReadLine());
                using (var context = new ApplicationDbContext()) {
                    var pessoaparaadmin = context.Pessoas.Find(a);
                    if (pessoaparaadmin != null) {
                        pessoaparaadmin.Admin = true;
                        context.SaveChanges();
                        Console.WriteLine("Usuário adicionado a admin com sucesso!");
                        Console.WriteLine("Voltar para a dashboard admin? (yes/no)");
                        string s = Console.ReadLine();
                        if (s == "yes") {
                            AdminPage();
                        } else {
                            b = false;
                            return b;
                        }
                    } else {
                        Console.WriteLine("Pessoa não encontrada.");
                        Console.WriteLine("Voltar para a dashboard admin? (yes/no)");
                        string s = Console.ReadLine();
                        if (s == "yes") {
                            AdminPage();
                        } else {
                            b = false;
                            return b;
                        }
                    }
                }
            } else {
                Console.WriteLine("Digite o id do usuário.");
                int a = int.Parse(Console.ReadLine());
                using (var context = new ApplicationDbContext()) {
                    var pessoaparaadmin = context.Pessoas.Find(a);
                    if (pessoaparaadmin != null) {
                        pessoaparaadmin.Admin = false;
                        context.SaveChanges();
                        Console.WriteLine("Usuário removido de admin com sucesso!");
                        Console.WriteLine("Voltar para a dashboard admin? (yes/no)");
                        string s = Console.ReadLine();
                        if (s == "yes") {
                            AdminPage();
                        } else {
                            b = false;
                            return b;
                        }
                    } else {
                        Console.WriteLine("Pessoa não encontrada.");
                        Console.WriteLine("Voltar para a dashboard admin? (yes/no)");
                        string s = Console.ReadLine();
                        if (s == "yes") {
                            AdminPage();
                        } else {
                            b = false;
                            return b;
                        }
                    }
                }
            }
        }
        return b;
    }
    static bool RemoveUsers(int userid)
    {

        using (var context = new ApplicationDbContext())
        {
            var pessoaParaRemover = context.Pessoas.Find(userid);

            if (pessoaParaRemover != null)
            {
                context.Pessoas.Remove(pessoaParaRemover);
                context.SaveChanges();
                return true;
            }
            else
            {
                Console.WriteLine("Pessoa não encontrada.");
                return false;
            }
        }
    }

    static bool AdicionarCompra(int value, string name, int pessoaId){
        using (var context = new ApplicationDbContext()){
            var pessoa = context.Pessoas.Find(pessoaId);
            if(pessoa != null){
                var compra = new PessoaCompras { ItemComprado = name, QuantidadeGasta = value, Pessoa = pessoa, Id = GerarIdProduto() };
                context.PessoaCompras.Add(compra);
                context.SaveChanges();
                Console.WriteLine(compra);
                return true;
            }else{
                return false;
            }
        }
    }
    static void Main()
    {
        Console.WriteLine("1 - User Login Page");
        Console.WriteLine("2 - Admin Login Page");
        Console.WriteLine("3 - Cadastrar-se");

        string l = Console.ReadLine();
        if(l == "1"){
            Console.WriteLine("Type your name");
            string n = Console.ReadLine();
            if(ProcurarNome(n) == true){
                Console.WriteLine("Type Your PassWord");
                string pass = Console.ReadLine();
                if(login(n, pass, "no") ==  true){
                    Console.Clear();
                    Console.WriteLine("You has logged sucessfully");
                    Console.WriteLine("Digite o nome do produto.");
                    string productname = Console.ReadLine();
                    Console.WriteLine("Digite o preço do produto");
                    int productprice = int.Parse(Console.ReadLine());

    using (var context = new ApplicationDbContext()){
    var p = context.Pessoas.FirstOrDefault(ps => ps.PessoaUserName == n);

    if (p != null && p.PessoaPassword != null && p.PessoaUserName == n && p.PessoaPassword == pass)
    {
        bool o = AdicionarCompra(productprice, productname, p.PessoaId);

        if (o)
        {
            Console.WriteLine("Seu produto foi cadastrado com sucesso na lista de compras.");
            Console.WriteLine("Deseja ver suas compras? (yes/no)");
            string i = Console.ReadLine();

            if (i == "yes")
            {
                var pessoatolist = context.PessoaCompras
                    .Where(pc => pc.PessoaId == p.PessoaId)
                    .ToList();

                if (pessoatolist.Any())
                {
                    Console.WriteLine("Itens comprados:");

                    foreach (var compra in pessoatolist)
                    {
                        Console.WriteLine($"Item: {compra.ItemComprado}, Quantidade: {compra.QuantidadeGasta}");
                    }
                }
                else
                {
                    Console.WriteLine("A lista de itens comprados está vazia.");
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            Console.WriteLine("Não foi possível cadastrar seu produto.");
        }
    }
}

                }else{
                    Console.WriteLine("Incorrect Password");
                }
            }else{
                Console.WriteLine("This name doesn't exists on our db, try again.");
            }
        }else if(l == "2"){
            Console.WriteLine("Type your name");
            string n = Console.ReadLine();
            if(ProcurarNome(n) == true){
                Console.WriteLine("Type Your PassWord");
                string pass = Console.ReadLine();
                if(login(n, pass, "yes") ==  true){
                    AdminFunctions();
                }else{
                    Console.WriteLine("Incorrect Password or you aren't a admin.");
                }
            }else{
                Console.WriteLine("This name doesn't exists on our db, try again.");
            }
        }else if (l == "3") {
             bool c = CriarUser();
                    if(c ==  true){
                        return;
                    }else{
                            return;
                    }
        }else{
            return;
        }
    }   
}
