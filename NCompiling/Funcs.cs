using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCompiling {
	public class BaseTermialsWithFunc {
		public int Id { get; set; }
		public BaseTerminal[] BaseTermianals { get; set; }
	}

	public enum BaseTermialType {
		Terminal,
		NonTermial,
		Action
	}
	public abstract class BaseTerminal {
		public BaseTermialType Type { get; set; }
	}

	public class ActionNumTerminal : BaseTerminal {
		public ActionNumTerminal(int num) {
			Num = num;
			Type = BaseTermialType.Action;
		}

		public int Num { get; set; }
	}

	public class Terminal : BaseTerminal {
		public Terminal(WordClass w) {
			Type = BaseTermialType.Terminal;
			word = w;
		}
		public WordClass word { get; set; }
	}
	public abstract class NonTermial : BaseTerminal {
		public NonTermial() {
			Type = BaseTermialType.NonTermial;
		}
		public WordClass? Attr { get; set; } = null;
		public double? Val { get; set; } = null;
		public string place { get; set; } = null;
		public abstract BaseTermialsWithFunc GetNext(WordClass w);
	}

	public class NTA : NonTermial {
		public override BaseTermialsWithFunc GetNext(WordClass w) {
			if(w == WordClass.Plus || w == WordClass.Minus || w == WordClass.Ident || w == WordClass.Number || w == WordClass.Lparen)
				return new BaseTermialsWithFunc {
					Id = 1,
					BaseTermianals = new BaseTerminal[] { new NTG(), new NTB(), new NTH() }
				};
			throw new Exception("ERROR 1");
		}
	}

	public class NTG : NonTermial {
		public override BaseTermialsWithFunc GetNext(WordClass w) {
			if(w == WordClass.Plus || w == WordClass.Minus)
				return new BaseTermialsWithFunc {
					Id = 2,
					BaseTermianals = new BaseTerminal[] { new NTC() }
				};
			else if(w == WordClass.Ident || w == WordClass.Number || w == WordClass.Lparen)
				return new BaseTermialsWithFunc {
					Id = 3,
					BaseTermianals = new BaseTerminal[] { new Terminal(WordClass.Null) }
				};
			throw new Exception("ERROR 1");
		}
	}
	public class NTH : NonTermial {
		public override BaseTermialsWithFunc GetNext(WordClass w) {
			if(w == WordClass.Plus || w == WordClass.Minus)
				return new BaseTermialsWithFunc {
					Id = 11,
					BaseTermianals = new BaseTerminal[] { new NTC(), new NTB(), new NTH() }
				};
			else if(w == WordClass.Rparen || w == WordClass.End)
				return new BaseTermialsWithFunc {
					Id = 12,
					BaseTermianals = new BaseTerminal[] { new Terminal(WordClass.Null) }
				};
			throw new Exception("ERROR 1");
		}
	}

	public class NTB : NonTermial {
		public override BaseTermialsWithFunc GetNext(WordClass w) {
			if(w == WordClass.Ident || w == WordClass.Number || w == WordClass.Lparen)
				return new BaseTermialsWithFunc {
					Id = 4,
					BaseTermianals = new BaseTerminal[] { new NTE(), new NTI() }
				};
			throw new Exception("ERROR 1");
		}
	}
	public class NTI : NonTermial {
		public override BaseTermialsWithFunc GetNext(WordClass w) {
			if(w == WordClass.Times || w == WordClass.Slash)
				return new BaseTermialsWithFunc {
					Id = 13,
					BaseTermianals = new BaseTerminal[] { new NTD(), new NTE(), new NTI() }
				};
			else if(w == WordClass.Plus || w == WordClass.Minus || w == WordClass.Rparen || w == WordClass.End)
				return new BaseTermialsWithFunc {
					Id = 14,
					BaseTermianals = new BaseTerminal[] { new Terminal(WordClass.Null) }
				};
			throw new Exception("ERROR 1");
		}
	}

	public class NTE : NonTermial {
		public override BaseTermialsWithFunc GetNext(WordClass w) {
			if(w == WordClass.Ident)
				return new BaseTermialsWithFunc {
					Id = 15,
					BaseTermianals = new BaseTerminal[] { new Terminal(WordClass.Ident) }
				};
			else if(w == WordClass.Number)
				return new BaseTermialsWithFunc {
					Id = 5,
					BaseTermianals = new BaseTerminal[] { new Terminal(WordClass.Number) }
				};
			else if(w == WordClass.Lparen)
				return new BaseTermialsWithFunc {
					Id = 6,
					BaseTermianals = new BaseTerminal[] { new Terminal(WordClass.Lparen), new NTA(), new Terminal(WordClass.Rparen) }
				};
			throw new Exception("ERROR 1");
		}
	}
	public class NTC : NonTermial {
		public override BaseTermialsWithFunc GetNext(WordClass w) {
			if(w == WordClass.Plus)
				return new BaseTermialsWithFunc {
					Id = 7,
					BaseTermianals = new BaseTerminal[] { new Terminal(WordClass.Plus) }
				};
			else if(w == WordClass.Minus)
				return new BaseTermialsWithFunc {
					Id = 8,
					BaseTermianals = new BaseTerminal[] { new Terminal(WordClass.Minus) }
				};
			throw new Exception("ERROR 1");
		}
	}
	public class NTD : NonTermial {
		public override BaseTermialsWithFunc GetNext(WordClass w) {
			if(w == WordClass.Times)
				return new BaseTermialsWithFunc {
					Id = 9,
					BaseTermianals = new BaseTerminal[] { new Terminal(WordClass.Times) }
				};
			else if(w == WordClass.Slash)
				return new BaseTermialsWithFunc {
					Id = 10,
					BaseTermianals = new BaseTerminal[] { new Terminal(WordClass.Slash) }
				};
			throw new Exception("ERROR 1");
		}
	}
}
