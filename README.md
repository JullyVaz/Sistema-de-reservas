# Sistema-de-reservas
É um sistema de hospedagem, que será usado para realizar uma reserva em um hotel. Esse sistema tem a classe Pessoa, que representa o hóspede, a classe Suíte e a classe Reserva, que permite um relacionamento entre ambos.

Este programa calcula o valor da diária, concedendo um desconto de 10% quando a reserva é feita para um período maior que 10 dias.

Esse sistema não permite realizar uma reserva de uma suíte com capacidade menor do que a quantidade de hóspedes. Exemplo: Se é uma suíte capaz de hospedar 2 pessoas, então ao passar 3 hóspedes retorna uma exception.

O método ObterQuantidadeHospedes da classe Reserva retorna a quantidade total de hóspedes, enquanto que o método CalcularValorDiaria calcula o valor da diária (Dias reservados x valor da diária).

Nesse sistema foi adicionado a interface com o banco de dados: Reserva com o MySQL.
