using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MySql.Data.MySqlClient;

namespace ProjetoHospedagem
{
    public class Pessoa
    {
        public string NomeCompleto { get; set; }

        public Pessoa(string nome)
        {
            NomeCompleto = nome;
        }
    }

    public class Suite
    {
        public string TipoSuite { get; set; }
        public int Capacidade { get; set; }
        public decimal ValorDiaria { get; set; }

        public Suite(string tipoSuite, int capacidade, decimal valorDiaria)
        {
            TipoSuite = tipoSuite;
            Capacidade = capacidade;
            ValorDiaria = valorDiaria;
        }
    }

    public class Reserva
    {
        public int DiasReservados { get; set; }
        public Suite Suite { get; set; }
        public List<Pessoa> Hospedes { get; set; }

        public Reserva(int diasReservados)
        {
            DiasReservados = diasReservados;
            Hospedes = new List<Pessoa>();
        }

        public void CadastrarSuite(Suite suite)
        {
            Suite = suite;
        }

        public void CadastrarHospedes(List<Pessoa> hospedes)
        {
            Hospedes = hospedes;
        }

        public int ObterQuantidadeHospedes()
        {
            return Hospedes.Count;
        }

        public decimal CalcularValorDiaria()
        {
            return Suite.ValorDiaria * DiasReservados;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            List<Pessoa> hospedes = new List<Pessoa>();

            Console.WriteLine("Digite o nome do hóspede:");
            string nomeHospede = Console.ReadLine();

            Pessoa p1 = new Pessoa(nome: nomeHospede);
            hospedes.Add(p1);

            Console.WriteLine("Escolha o tipo de suíte:");
            Console.WriteLine("1. Standard");
            Console.WriteLine("2. Master");
            Console.WriteLine("3. Deluxe ou Master Superior");

            int tipoSuiteOpcao = 0;
            bool isTipoSuiteValid = false;

            while (!isTipoSuiteValid)
            {
                Console.Write("Opção: ");
                string tipoSuiteInput = Console.ReadLine();

                if (int.TryParse(tipoSuiteInput, out tipoSuiteOpcao) && tipoSuiteOpcao >= 1 && tipoSuiteOpcao <= 3)
                {
                    isTipoSuiteValid = true;
                }
                else
                {
                    Console.WriteLine("Opção inválida. Por favor, escolha uma opção válida.");
                }
            }

            string tipoSuite;

            switch (tipoSuiteOpcao)
            {
                case 1:
                    tipoSuite = "Standard";
                    break;
                case 2:
                    tipoSuite = "Master";
                    break;
                case 3:
                    tipoSuite = "Deluxe ou Master Superior";
                    break;
                default:
                    tipoSuite = "Standard";
                    break;
            }

            Console.WriteLine("Digite a capacidade da suíte:");
            int capacidadeSuite = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Digite o valor da diária:");
            decimal valorDiaria = Convert.ToDecimal(Console.ReadLine());

            Suite suite = new Suite(tipoSuite: tipoSuite, capacidade: capacidadeSuite, valorDiaria: valorDiaria);

            Console.WriteLine("Digite a quantidade de dias reservados:");
            int diasReservados = Convert.ToInt32(Console.ReadLine());

            Reserva reserva = new Reserva(diasReservados);

            reserva.CadastrarSuite(suite);

            if (suite.Capacidade > 1)
            {
                int numeroHospedesAdicionais = suite.Capacidade - 1;
                int contador = 0;

                while (contador < numeroHospedesAdicionais)
                {
                    Console.WriteLine("Deseja adicionar mais hóspedes? (S/N)");
                    string opcao = Console.ReadLine();

                    if (opcao.ToUpper() == "S")
                    {
                        if (hospedes.Count < suite.Capacidade)
                        {
                            Console.WriteLine("Digite o nome do hóspede:");
                            string nomeHospedeAdicional = Console.ReadLine();

                            Pessoa hospedeAdicional = new Pessoa(nome: nomeHospedeAdicional);
                            hospedes.Add(hospedeAdicional);

                            contador++;
                        }
                        else
                        {
                            Console.WriteLine("A capacidade da suíte foi atingida. Não é possível adicionar mais hóspedes.");
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            reserva.CadastrarHospedes(hospedes);

            int reservaId = InsertReservationIntoDatabase(reserva);
            RetrieveAndDisplayReservation(reservaId, reserva);
        }

        public static int InsertReservationIntoDatabase(Reserva reserva)
        {
            string server = "localhost";
            string database = "reserva";
            string username = "root";
            string password = "Ros#195402";
            string connectionString = $"Server={server};Database={database};Uid={username};Pwd={password};";

            int reservaId = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO Reservations (Hospedes, NomeCompletoHospedes, TipoSuite, Capacidade, ValorDiaria, DiasReservados, ValorTotal) " +
                                     "VALUES (@hospedes, @nomeCompletoHospedes, @tipoSuite, @capacidade, @valorDiaria, @diasReservados, @valorTotal)";

                MySqlCommand command = new MySqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@hospedes", reserva.ObterQuantidadeHospedes());
                command.Parameters.AddWithValue("@nomeCompletoHospedes", GetNomeCompletoHospedes(reserva.Hospedes));
                command.Parameters.AddWithValue("@tipoSuite", reserva.Suite.TipoSuite);
                command.Parameters.AddWithValue("@capacidade", reserva.Suite.Capacidade);
                command.Parameters.AddWithValue("@valorDiaria", reserva.Suite.ValorDiaria);
                command.Parameters.AddWithValue("@diasReservados", reserva.DiasReservados);
                command.Parameters.AddWithValue("@valorTotal", reserva.CalcularValorDiaria());

                command.ExecuteNonQuery();

                reservaId = (int)command.LastInsertedId;
            }

            return reservaId;
        }

        public static void RetrieveAndDisplayReservation(int reservaId, Reserva reserva)
        {
            string server = "localhost";
            string database = "reserva";
            string username = "root";
            string password = "Ros#195402";
            string connectionString = $"Server={server};Database={database};Uid={username};Pwd={password};";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT * FROM Reservations WHERE ID = @reservaId";
                MySqlCommand command = new MySqlCommand(selectQuery, connection);
                command.Parameters.AddWithValue("@reservaId", reservaId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine($"Hóspedes: {reader["Hospedes"]}");
                        Console.WriteLine($"Nome Completo dos Hóspedes: {reader["NomeCompletoHospedes"]}");
                        Console.WriteLine($"Tipo de Suíte: {reader["TipoSuite"]}");
                        Console.WriteLine($"Capacidade: {reader["Capacidade"]}");
                        Console.WriteLine($"Valor Diária: {reader["ValorDiaria"]}");
                        Console.WriteLine($"Dias Reservados: {reader["DiasReservados"]}");
                        Console.WriteLine($"Valor Total da Estadia: {reader.GetDecimal("ValorTotal")}");
                    }
                }
            }
        }

        public static string GetNomeCompletoHospedes(List<Pessoa> hospedes)
        {
            StringBuilder nomeCompleto = new StringBuilder();

            foreach (var hospede in hospedes)
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                string nomeFormatado = textInfo.ToTitleCase(hospede.NomeCompleto.ToLower());
                
                if (nomeCompleto.Length > 0)
                {
                    nomeCompleto.Append(", ");
                }

                nomeCompleto.Append(nomeFormatado);
            }

            return nomeCompleto.ToString();
        }
    }
}
