using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NCompiling {
	public enum WordType {
		BasicWord,
		Operator,
		Delimit,
		Number,
		Ident,
		Error
	}
	public enum WordClass {
		BeginSym, CallSym, ConstSym, DoSym, EndSym, IfSym, OddSym, ProcedureSym, ReadSym, ThenSym, VarSym, WhileSym, WriteSym,
		Plus, Minus, Times, Slash, Eql, Neq, Leq, Lss, Geg, Gtr, Becomes,
		Lparen, Rparen, Comma, Semicolon, Period,
		Number,
		Ident,
		End,
		Null,
		Error
	}

	public class Compiler {
		public class WordAndType {
			public string Word { get; set; }
			public WordType Type { get; set; }
			public WordClass Class { get; set; }
		}

		private readonly string[] basicWords = new string[] { "begin", "call", "const", "do", "end", "if", "odd", "procedure", "read", "then", "var", "while", "write" };
		private readonly WordClass[] basicWordsClass = new WordClass[12] {
			WordClass.BeginSym, WordClass.CallSym, WordClass.ConstSym, WordClass.DoSym,
			WordClass.EndSym, WordClass.IfSym, WordClass.OddSym, WordClass.ProcedureSym,
			WordClass.ReadSym, WordClass.ThenSym, WordClass.WhileSym, WordClass.WriteSym
		};
		private readonly string[] operators = new string[] { "+", "-", "*", "/", "=", "#", "<=", "<", ">=", ">", ":=" };
		private readonly WordClass[] operatorsClass = new WordClass[11] {
			WordClass.Plus, WordClass.Minus, WordClass.Times, WordClass.Slash,
			WordClass.Eql, WordClass.Neq, WordClass.Leq, WordClass.Lss,
			WordClass.Geg, WordClass.Gtr, WordClass.Becomes
		};
		private readonly string[] delimits = new string[] { "(", ")", ",", ";", "." };
		private readonly WordClass[] delimitsClass = new WordClass[5] {
			WordClass.Lparen, WordClass.Rparen, WordClass.Comma, WordClass.Semicolon, WordClass.Period
		};

		private FileStream fs;
		private List<WordAndType> wordList = new List<WordAndType>();

		private string preprocessStr(string str) {
			int i = 0;
			while(i < str.Length) {
				if(str[i] == ' ') {
					i++;
					continue;
				}

				for(int j = 0; j < operators.Length; j++) {
					int operatorLength = operators[j].Length;
					if(str.Length - i >= operatorLength && str.Substring(i, operatorLength) == operators[j]) {
						str = str.Insert(i, " ");
						str = str.Insert(i + 1 + operatorLength, " ");
						i += operatorLength;
						break;
					}
				}

				for(int j = 0; j < delimits.Length; j++) {
					int boundaryLength = delimits[j].Length;
					if(str.Length - i >= boundaryLength && str.Substring(i, boundaryLength) == delimits[j]) {
						str = str.Insert(i, " ");
						str = str.Insert(i + 1 + boundaryLength, " ");
						i += boundaryLength;
						break;
					}
				}

				i++;
			}
			return str;
		}
		private void splitStr(string str) {
			str = preprocessStr(str);
			str = str.ToLower();
			string[] words = str.Split(new char[] { ' ', '\n', '\t', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			foreach(string w in words) {
				int i;
				if((i = isBasicWord(w)) != -1) {
					wordList.Add(new WordAndType { Word = w, Type = WordType.BasicWord, Class = basicWordsClass[i] });
				}
				else if((i = isOperator(w)) != -1) {
					wordList.Add(new WordAndType { Word = w, Type = WordType.Operator, Class = operatorsClass[i] });
				}
				else if((i = isDelimit(w)) != -1) {
					wordList.Add(new WordAndType { Word = w, Type = WordType.Delimit, Class = delimitsClass[i] });
				}
				else if(isNumber(w)) {
					wordList.Add(new WordAndType { Word = w, Type = WordType.Number, Class = WordClass.Number });
				}
				else if(isIdent(w)) {
					wordList.Add(new WordAndType { Word = w, Type = WordType.Ident, Class = WordClass.Ident });
				}
				else {
					wordList.Add(new WordAndType { Word = w, Type = WordType.Error, Class = WordClass.Error });
					Console.WriteLine($"{w} is not a ident");
				}
			}
			wordList.Add(new WordAndType { Word = null, Type = WordType.Error, Class = WordClass.End });
		}

		private int isBasicWord(string word) {
			for(int j = 0; j < basicWords.Length; j++) {
				if(word == basicWords[j]) {
					return j;
				}
			}
			return -1;
		}
		private int isOperator(string word) {
			for(int j = 0; j < operators.Length; j++) {
				if(word == operators[j]) {
					return j;
				}
			}
			return -1;
		}
		private int isDelimit(string word) {
			for(int j = 0; j < delimits.Length; j++) {
				if(word == delimits[j]) {
					return j;
				}
			}
			return -1;
		}
		private bool isNumber(string word) {
			foreach(char w in word) {
				if(w < '0' || w > '9') {
					return false;
				}
			}
			return true;
		}
		private bool isIdent(string word) {
			char f = word[0];
			if((f >= 'a' && f <= 'z') || (f >= 'A' && f <= 'Z')) {

				for(int i = 1; i < word.Length; i++) {
					f = word[i];
					if((f < 'a' || f > 'z') && (f < 'A' || f > 'Z') && (f < '0' || f > '9')) {
						return false;
					}
				}

				return true;
			}
			return false;
		}

		public Compiler(string filePath) {
			try {
				fs = new FileStream(filePath, FileMode.Open);
			}
			catch(Exception e) {
				Console.WriteLine(e.Message);
			}
		}
		public void Run() {
			string buffer;
			StreamReader sr = new StreamReader(fs);
			while(!sr.EndOfStream) {
				buffer = sr.ReadLine();
				splitStr(buffer);
			}
			sr.Close();
		}
		public void DisPlayCount() {
			foreach(var p in wordList) {
				Console.WriteLine($"{p.Word,-10}: {p.Type,-10} {p.Class}");
			}
		}
		private int tempNum;
		private string tempIdent;
		private double result;
		private StringBuilder sb = new StringBuilder();
		public bool MathAnalyze() {
			Stack<BaseTerminal> stack = new Stack<BaseTerminal>();
			Stack<NonTermial[]> attrStack = new Stack<NonTermial[]>();

			stack.Push(new Terminal(WordClass.End));
			stack.Push(new NTA());

			attrStack.Push(new NonTermial[] { new NTA() });

			int wordListNum = 0;

			while(stack.Count > 0) {
				displayStack(stack, wordListNum);
				displayAttrStack(attrStack);
				BaseTerminal t = stack.Pop();
				if(t.Type == BaseTermialType.Terminal) {
					Terminal ter = t as Terminal;
					if(ter.word == WordClass.Number) {
						tempNum = Convert.ToInt32(wordList[wordListNum].Word);
					}
					else if(ter.word == WordClass.Ident) {
						tempNum = 1;
						tempIdent = wordList[wordListNum].Word;
					}
					if(wordList[wordListNum].Class == ter.word) {
						wordListNum++;
						if(attrStack.Peek().Length == 0) {
							attrStack.Pop();
						}

						Console.Write($"{"MATCH",20}");
					}
					else {
						Console.WriteLine("ERROR 2");
						return false;
					}
				}
				else if(t.Type == BaseTermialType.NonTermial) {
					NonTermial ter = t as NonTermial;
					BaseTerminal[] nextTers;
					try {
						BaseTermialsWithFunc nextTersWithNum = ter.GetNext(wordList[wordListNum].Class);
						nextTers = nextTersWithNum.BaseTermianals;
						stack.Push(new ActionNumTerminal(nextTersWithNum.Id));
						displayNext(nextTersWithNum);
					}
					catch {
						return false;
					}

					List<NonTermial> nextNonTers = new List<NonTermial>();
					for(int i = nextTers.Length - 1; i >= 0; i--) {
						if(nextTers[i].Type == BaseTermialType.NonTermial) {
							nextNonTers.Add((NonTermial)nextTers[i]);
						}
					}
					attrStack.Push(nextNonTers.ToArray());

					for(int i = nextTers.Length - 1; i >= 0; i--) {
						if(nextTers[i].Type == BaseTermialType.Terminal) {
							Terminal tt = nextTers[i] as Terminal;
							if(tt.word == WordClass.Null) {
								attrStack.Pop();
								continue;
							}

						}
						stack.Push(nextTers[i]);
					}
				}
				else {
					ActionNumTerminal ter = t as ActionNumTerminal;
					execute(attrStack, ter.Num);
					Console.Write($"{"exe" + ter.Num,20}");
				}

				Console.WriteLine();

			}
			Console.WriteLine(result);
			Console.WriteLine(sb.ToString());
			return true;
		}

		private void displayStack(Stack<BaseTerminal> stack, int wordNum) {
			StringBuilder sb = new StringBuilder();
			for(int i = stack.Count - 1; i >= 0; i--) {
				if(stack.ElementAt(i).Type == BaseTermialType.NonTermial) {
					string name = stack.ElementAt(i).GetType().Name;
					sb.Append(name.Substring(2, name.Length - 2));
				}
				else if(stack.ElementAt(i).Type == BaseTermialType.Terminal) {
					Terminal ter = (Terminal)stack.ElementAt(i);
					sb.Append($"[{ter.word}]");
				}
				else {
					ActionNumTerminal ter = (ActionNumTerminal)stack.ElementAt(i);
					sb.Append($"{{{ter.Num}}}");
				}
			}
			Console.Write($"{sb,-60} {wordList[wordNum].Class,-10}");
		}
		private void displayAttrStack(Stack<NonTermial[]> stack) {
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < stack.Count; i++) {
				NonTermial[] nonTers = stack.ElementAt(i);
				sb.Append("(");
				for(int j = 0; j < nonTers.Length; j++) {
					string name = nonTers[j].GetType().Name;
					sb.Append(name.Substring(2, name.Length - 2));
					if(nonTers[j].Attr.HasValue) {
						sb.Append($"[{nonTers[j].Attr.Value}]");
					}
					if(nonTers[j].Val.HasValue) {
						sb.Append($"{{{nonTers[j].Val.Value}}}");
					}
				}
				sb.Append(")");
			}
			Console.Write($"{sb,80}");
		}
		private void displayNext(BaseTermialsWithFunc nextTersWithNum) {
			BaseTerminal[] nextTers = nextTersWithNum.BaseTermianals;

			StringBuilder sb = new StringBuilder("-> ");
			for(int i = 0; i < nextTers.Length; i++) {
				if(nextTers.ElementAt(i).Type == BaseTermialType.NonTermial) {
					string name = nextTers.ElementAt(i).GetType().Name;
					sb.Append(name.Substring(2, name.Length - 2));
				}
				else {
					Terminal ter = (Terminal)nextTers.ElementAt(i);
					sb.Append($"[{ter.word}]");
				}

			}
			Console.Write($"{sb,20}  {nextTersWithNum.Id}");
		}
		bool isGood = true;
		private void execute(Stack<NonTermial[]> stack, int id) {
			NonTermial[] peekNonTers = stack.Peek();
			NonTermial[] secondTers = stack.ElementAt(1);
			bool isFull = true;
			foreach(var nonTer in peekNonTers) {
				if(!nonTer.Attr.HasValue && !nonTer.Val.HasValue) {
					isFull = false;
					break;
				}
			}
			if(isFull) {
				peekNonTers = stack.Pop();
			}
			if(id == 1) {
				NonTermial a = secondTers.FirstOrDefault(p => p.GetType().Name == "NTA");
				NonTermial g = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTG");
				NonTermial b = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTB");
				NonTermial h = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTH");
				a.place = getTempPlace();
				if(g.Attr == WordClass.Plus) {
					a.Val = result = b.Val.Value + h.Val.Value;
					if(h.place == "0" && isGood) {
						a.place = b.place;
						return;
					}
					sb.AppendLine($"+ {b.place} {h.place} {a.place}");
				}

				else {
					a.Val = result = -b.Val.Value + h.Val.Value;
					if(h.place == "0" && isGood) {
						string tempPlace1 = getTempPlace();
						sb.AppendLine($"- 0 {b.place} {tempPlace1}");
						a.place = tempPlace1;
						return;
					}
					string tempPlace = getTempPlace();
					sb.AppendLine($"- 0 {b.place} {tempPlace}");
					b.place = tempPlace;
					sb.AppendLine($"+ {b.place} {h.place} {a.place}");
				}
			}
			else if(id == 2) {
				NonTermial g = secondTers.FirstOrDefault(p => p.GetType().Name == "NTG");
				NonTermial c = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTC");
				g.Attr = c.Attr;
			}
			else if(id == 3) {
				NonTermial g = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTG");
				g.Attr = WordClass.Plus;
			}
			else if(id == 4) {
				NonTermial b = secondTers.FirstOrDefault(p => p.GetType().Name == "NTB");
				NonTermial e = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTE");
				NonTermial i = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTI");
				b.Val = e.Val * i.Val;
				if(i.place == "1" && isGood) {
					b.place = e.place;
					return;
				}
				b.place = getTempPlace();
				sb.AppendLine($"* {e.place} {i.place} {b.place}");
			}
			else if(id == 5) {
				NonTermial e = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTE");
				e.Val = tempNum;
				e.place = tempNum.ToString();
			}
			else if(id == 15) {
				NonTermial e = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTE");
				e.Val = tempNum;
				e.place = tempIdent;
			}
			else if(id == 6) {
				NonTermial a = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTA");
				NonTermial e = secondTers.FirstOrDefault(p => p.GetType().Name == "NTE");
				e.Val = a.Val;
				e.place = a.place;
			}
			else if(id == 7) {
				NonTermial c = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTC");
				c.Attr = WordClass.Plus;
			}
			else if(id == 8) {
				NonTermial c = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTC");
				c.Attr = WordClass.Minus;
			}
			else if(id == 9) {
				NonTermial d = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTD");
				d.Attr = WordClass.Times;
			}
			else if(id == 10) {
				NonTermial d = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTD");
				d.Attr = WordClass.Slash;
			}
			else if(id == 11) {
				NonTermial h = secondTers.FirstOrDefault(p => p.GetType().Name == "NTH");
				NonTermial c = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTC");
				NonTermial b = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTB");
				NonTermial h1 = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTH");
				h.place = getTempPlace();
				if(c.Attr.Value == WordClass.Plus) {
					h.Val = b.Val + h1.Val;
					if(h1.place == "0" && isGood) {
						h.place = b.place;
						return;
					}
					sb.AppendLine($"+ {b.place} {h1.place} {h.place}");
				}
				else {
					h.Val = -(b.Val + h1.Val);
					if(h1.place == "0" && isGood) {
						string tempPlace1 = getTempPlace();
						sb.AppendLine($"- 0 {b.place} {tempPlace1}");
						h.place = tempPlace1;
						return;
					}
					sb.AppendLine($"+ {b.place} {h1.place} {h.place}");
					string tempPlace = getTempPlace();
					sb.AppendLine($"- 0 {h.place} {tempPlace}");
					h.place = tempPlace;
				}
			}
			else if(id == 12) {
				NonTermial h = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTH");
				h.Val = 0;
				h.place = "0";
			}
			else if(id == 13) {
				NonTermial i = secondTers.FirstOrDefault(p => p.GetType().Name == "NTI");
				NonTermial d = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTD");
				NonTermial e = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTE");
				NonTermial i1 = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTI");
				i.place = getTempPlace();
				if(d.Attr == WordClass.Times) {
					i.Val = e.Val * i1.Val;
					if(i1.place == "1" && isGood) {
						i.place = e.place;
						return;
					}
					sb.AppendLine($"* {e.place} {i1.place} {i.place}");
				}
				else {
					i.Val = 1 / (e.Val * i1.Val);
					if(i1.place == "1" && isGood) {
						string tempPlace1 = getTempPlace();
						sb.AppendLine($"/ 1 {e.place} {tempPlace1}");
						i.place = tempPlace1;
						return;
					}
					sb.AppendLine($"* {e.place} {i1.place} {i.place}");
					string tempPlace = getTempPlace();
					sb.AppendLine($"/ 1 {i.place} {tempPlace}");
					i.place = tempPlace;
				}
			}
			else if(id == 14) {
				NonTermial i = peekNonTers.FirstOrDefault(p => p.GetType().Name == "NTI");
				i.Val = 1;
				i.place = "1";
			}
			
		}

		private int nextPlace = 1;
		private string getTempPlace() {
			string place = $"T{nextPlace}";
			nextPlace++;
			return place;
		}
	}
}
