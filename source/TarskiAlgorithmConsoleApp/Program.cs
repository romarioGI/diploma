using System;
using InputSubsystem;
using OutputSubsystem;
using ParserSubsystem;
using ProcessorsSubsystem;

namespace TarskiAlgorithmConsoleApp
{
    internal static class Program
    {
        private static readonly IInput<Symbol> Input = new ConsoleInput();
        private static readonly IParser<Symbol, SyntaxTree> Parser = new SyntaxTreeParser();
        private static readonly IProcessor<SyntaxTree> TarskiProcessor = new TarskiQuantifierEliminator();
        private static readonly IOutput<SyntaxTree> Output = new NativeOutput();

        private static void Main()
        {
            //TODO ловить ошибку, выводить сообщение её
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Console.WriteLine("Please, enter formula");

            var parsedInput = Parser.Parse(Input);
            Console.WriteLine($"Input formula:\t{Output.Print(parsedInput)}");

            var eliminatedFormula = TarskiProcessor.Do(parsedInput);

            Console.WriteLine($"Result formula:\t{Output.Print(eliminatedFormula)}");
            Console.WriteLine("Press any key to shut down the program");
            Console.ReadKey();
        }
    }
}