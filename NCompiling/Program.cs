using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCompiling {
	class Program {
		static void Main(string[] args) {
			Console.WindowWidth = 200;
			Console.WindowHeight = 40;
			Compiler compiler = new Compiler("C:\\Users\\Fisher\\Documents\\Visual Studio 2015\\Projects\\Compiling\\NCompiling\\test.txt");
			compiler.Run();
			compiler.DisPlayCount();
			if(compiler.MathAnalyze()) {
				Console.WriteLine("\nSUCCEEDED");
			}
			else {
				Console.WriteLine("\nFAILED");
			}

		}
	}
}
