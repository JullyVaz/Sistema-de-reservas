using ProjetoHospedagem.Models;

namespace ProjetoHospedagem.Models
{
    public class Reserva
    {
        public List<Pessoa> Hospedes { get; set; }
        public Suite Suite { get; set; }
        public int DiasReservados { get; set; }
        public int valorTotal {get; set; }

        public Reserva() { }

        public Reserva(int diasReservados)
        {
            DiasReservados = diasReservados;
        }

        public void CadastrarHospedes(List<Pessoa> hospedes)
        {
           if (Suite.Capacidade >= hospedes.Count) 
           {
                Hospedes = hospedes;
           }
            else
            {
                 throw new Exception("Capacidade menor do que o número de hóspedes");
            }
        }

        public void CadastrarSuite(Suite suite)
        {
            Suite = suite;
        }

        public int ObterQuantidadeHospedes()
        {
            // Implementado!
             return Hospedes.Count;
        }

        public decimal CalcularValorDiaria()
        {
            
            decimal valorTotal = DiasReservados * Suite.ValorDiaria;
            
            // Regra: Caso os dias reservados forem maior ou igual a 10, conceder um desconto de 10%
            if (DiasReservados >= 10) {
                valorTotal -= Decimal.Divide(Decimal.Multiply(valorTotal, 10), 100);
            }

            return valorTotal;
        }
    }
}