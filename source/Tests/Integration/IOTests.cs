using System.IO;
using InputSubsystem;
using OutputSubsystem;
using ParserSubsystem;
using Xunit;

namespace Tests.Integration
{
    public class IOTests
    {
        [Theory]
        [MemberData(nameof(ParseInput))]
        public void ParseAndWrite(string inputString, string expectedOutputString)
        {
            var input = new TextReaderInput(new StringReader(inputString));
            var parser = new SyntaxTreeParser();
            var parsedInput = parser.Parse(input);
            var output = new NativeOutput();
            var actualOutputString = output.Print(parsedInput);
            
            Assert.Equal(expectedOutputString, actualOutputString);
        }
        
        public static TheoryData<string, string> ParseInput()
        {
            return new()
            {
                { "1=1", "1=1" },
                { "(2^2)^2=2^(2^2)", "(2^2)^2=2^2^2" },
                { "(2+2)+2=2+(2+2)", "2+2+2=2+(2+2)" },
                { "(\\exists x)(\\forall a)a*x^2^2+b*x+c=0 \\land c\\over 2>0", "(∃x)(∀a)a*x^2^2+b*x+c=0&c/2>0" },
                {"(\\exists x_1)(\\exists x_2)\\lnot x_1=x_2 \\land a*x^2+b*x+c=0", "(∃x_1)(∃x_2)¬x_1=x_2&a*x^2+b*x+c=0"}
            };
        }
    }
}