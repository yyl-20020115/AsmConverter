using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsmConverter
{
    public partial class Program
    {
        public static void ConvertFromIntel_ToATT(FileInfo fi, FileInfo fo)
        {
            var lineno = 0;
            using var InputReader = new StreamReader(fi.FullName);
            using var OutputWriter = new StreamWriter(fo.FullName);
            var emptyline = false;
            while (InputReader.ReadLine() is string line)
            {
                line = line.Trim();
                emptyline = line.Length == 0;
                lineno++;

                //TODO:

            }
            if (!emptyline)
            {
                OutputWriter.WriteLine();//should be end with an empty line
            }
        }

    }

    public static class CIntel2Gas
    {
        public static int CTOKEN_ENDL = 0;

        public static int CTOKEN_ENDF = 1;

        public static int CTOKEN_IDENT = 2;

        public static int CTOKEN_KEYWORD = 3;

        public static int CTOKEN_STR = 4;

        public static int CTOKEN_OPERATOR = 5;

        public static int CTOKEN_INT = 6;

        public static int CTOKEN_FLOAT = 7;

        public static int CTOKEN_ERROR = 8;

        public static Dictionary<int, string> CTOKEN_NAME = new () {
        {
            0,
            "endl"},
        {
            1,
            "endf"},
        {
            2,
            "ident"},
        {
            3,
            "keyword"},
        {
            4,
            "str"},
        {
            5,
            "op"},
        {
            6,
            "int"},
        {
            7,
            "float"},
        {
            8,
            "error"}};
        public static Dictionary<string, int> REGSIZE = new();
        public static List<string> PREFIX = new() {
                "lock",
                "rep",
                "repne",
                "repnz",
                "repe",
                "repz"
            };
        public static readonly List<string> REGNAMES = new() {
                "AH",
                "AL",
                "BH",
                "BL",
                "CH",
                "CL",
                "DH",
                "DL",
                "AX",
                "BX",
                "CX",
                "DX",
                "EAX",
                "EBX",
                "ECX",
                "EDX",
                "RAX",
                "RBX",
                "RCX",
                "RDX",
                "CR0",
                "CR1",
                "CR2",
                "CR3",
                "DR0",
                "DR1",
                "DR2",
                "DR3",
                "DR4",
                "DR5",
                "DR6",
                "DR7",
                "SI",
                "DI",
                "SP",
                "BP",
                "ESI",
                "EDI",
                "ESP",
                "EBP",
                "RSI",
                "RDI",
                "RSP",
                "RBP",
                "TR6",
                "TR7",
                "ST0",
                "ST1",
                "ST2",
                "ST3",
                "ST4",
                "ST5",
                "ST6",
                "ST7",
                "MM0",
                "MM1",
                "MM2",
                "MM3",
                "MM4",
                "MM5",
                "MM6",
                "MM7",
                "MM8",
                "MM9",
                "MM10",
                "MM11",
                "MM12",
                "MM13",
                "MM14",
                "MM15",
                "XMM0",
                "XMM1",
                "XMM2",
                "XMM3",
                "XMM4",
                "XMM5",
                "XMM6",
                "XMM7",
                "XMM8",
                "XMM9",
                "XMM10",
                "XMM11",
                "XMM12",
                "XMM13",
                "XMM14",
                "XMM15",
                "R0",
                "R1",
                "R2",
                "R3",
                "R4",
                "R5",
                "R6",
                "R7",
                "R8",
                "R9",
                "R10",
                "R11",
                "R12",
                "R13",
                "R14",
                "R15"
            };
        public static int[] ValidScales = new int[] { 1, 2, 4, 8 };
        static CIntel2Gas()
        {
            foreach (var reg in REGNAMES)
            {
                REGSIZE[reg] = RegInfo(reg);
            }
        }
        public static Func<string, int> regsize = reg => REGSIZE[reg.ToUpper()];

        public static Func<string, bool> isreg = reg => REGSIZE.ContainsKey(reg.ToUpper());

        public static Dictionary<string, string> Instreplace = new(){
        {
            "cbw",
            "cbtw"},
        {
            "cdq",
            "cltd"},
        {
            "cmpsd",
            "cmpsl"},
        {
            "codeseg",
            ".text"},
        {
            "cwd",
            "cwtd"},
        {
            "cwde",
            "cwtl"},
        {
            "dataseg",
            ".data"},
        {
            "db",
            ".byte"},
        {
            "dd",
            ".int"},
        {
            "dw",
            ".short"},
        {
            "emit",
            ".byte"},
        {
            "_emit",
            ".byte"},
        {
            "insd",
            "insl"},
        {
            "lodsd",
            "lodsl"},
        {
            "movsd",
            "movsl"},
        {
            "movsx",
            "movs"},
        {
            "movzx",
            "movz"},
        {
            "outsd",
            "outsl"},
        {
            "public",
            ".globl"},
        {
            "scasd",
            "scasl"},
        {
            "stosd",
            "stosl"}};

        public static int O_REG = 0;

        public static int O_IMM = 1;

        public static int O_MEM = 2;

        public static int O_LABEL = 3;

        public static readonly int[] Spaces = new[] { (int)' ', '\r', '\n', '\t' };
        public static readonly int[] CommentChars = new int[] { (int)';', '#' };
        public static readonly int[] QuoteChars = new int[] { '\'', '\"' };
        public static readonly int[] StartChars = new int[] { '_','$','@' };
        public static readonly string[] HexHeaders = new string[] { "0x", "0X" };
        public static int RegInfo(string name)
        {
            name = name.ToLower();
            if (name[..2] == "mm")
            {
                return 8;
            }
            if (name[..3] == "xmm")
            {
                return 16;
            }
            if (name[..1] == "r" && char.IsDigit(name[1]))
            {
                return 8;
            }
            if (new string[] { "cr", "dr" }.Contains(name[..2]))
            {
                return 4;
            }
            if (new string[] { "st", "tr" }.Contains(name[..2]))
            {
                return 8;
            }
            if (name.Length == 2)
            {
                if (name[0] == 'r')
                {
                    return 8;
                }
                if (new char[] { 'h', 'l' }.Contains(name[1]))
                {
                    return 1;
                }
                if (new int[] { 'x', 'i', 'p' }.Contains(name[1]))
                {
                    return 2;
                }
                throw new Exception("unknow register " + name);
            }
            if (name.Length == 3)
            {
                if (new int[] { 'x', 'i', 'p' }.Contains(name[2]))
                {
                    if (name[0] == 'e')
                    {
                        return 4;
                    }
                    if (name[0] == 'r')
                    {
                        return 8;
                    }
                    throw new Exception("unknow register " + name);
                }
            }
            return 0;
        }

        public class CToken
        {
            public int mode;
            public int line;
            public int row;
            public int col;
            public string source;
            public string text;
            public string fd;
            public object value;

            public CToken(
                int mode = 0,
                object value = default,
                string text = "",
                int row = -1,
                int col = -1,
                string fd = "",
                string source = "")
            {
                this.mode = mode;
                this.value = value;
                this.text = text;
                this.row = row;
                this.col = col;
                this.fd = fd;
                this.source = source;
            }

            public virtual CToken Copy() => new
                    (this.mode, this.value, this.text, this.line, 1, this.fd, this.source);

            public virtual bool IsEndl => this.mode == CTOKEN_ENDL;

            public virtual bool IsEndf => this.mode == CTOKEN_ENDF;

            public virtual bool IsIdent => this.mode == CTOKEN_IDENT;

            public virtual bool IsKeyword => this.mode == CTOKEN_KEYWORD;

            public virtual bool IsStr => this.mode == CTOKEN_STR;

            public virtual bool IsOperator => this.mode == CTOKEN_OPERATOR;

            public virtual bool IsInt => this.mode == CTOKEN_INT;

            public virtual bool IsFloat => this.mode == CTOKEN_FLOAT;

            public virtual bool IsError => this.mode == CTOKEN_ERROR;

            public override string ToString() => string.Format("(%s, %s)", CTOKEN_NAME[this.mode], (this.value));
        }
        public class CTokenizer
        {
            public int ch = -1;
            public int un = -1;
            public int col = 1;
            public int row = 1;
            public int eof = 0;
            public int state = 0;
            public string text = "";
            public string error = "";
            public bool init = false;
            public int code = 0;
            public TextReader fp;
            public List<CToken> tokens = new();

            public CTokenizer(TextReader fp)
            {
                this.fp = fp;
                this.Reset();
            }
            public virtual void Reset()
            {
                this.ch = -1;
                this.un = -1;
                this.col = 0;
                this.row = 1;
                this.eof = 0;
                this.state = 0;
                this.text = "";
                this.init = false;
                this.error = "";
                this.code = 0;
                this.tokens = new List<CToken>();
            }
            public virtual int GetCh()
            {
                int ch;
                if (un!=-1)
                {
                    this.ch = this.un;
                    this.un = -1;
                    return this.ch;
                }
                try
                {
                    ch = this.fp.Read();
                }
                catch
                {
                    ch = '\0';
                }
                this.ch = (char)ch;
                if (ch == '\n')
                {
                    this.col = 1;
                    this.row += 1;
                }
                else
                {
                    this.col += 1;
                }
                return this.ch;
            }
            public virtual void Ungetch(int ch) => this.un = ch;
            public virtual bool IsSpace(char ch) => Spaces.Contains(ch);
            public virtual bool IsAlpha(char ch) => char.IsLetter(ch);
            public virtual bool IsNumber(char ch) => char.IsDigit(ch);
            public virtual int SkipSpaces()
            {
                var skip = 0;
                while (true)
                {
                    if (this.ch == -1)
                    {
                        return -1;
                    }
                    if (!Spaces.Contains(this.ch))
                    {
                        break;
                    }
                    if (this.ch == '\n')
                    {
                        break;
                    }
                    this.GetCh();
                    skip += 1;
                }
                return skip;
            }
            public virtual CToken Read() => new ();
            public virtual CToken Next()
            {
                if (!this.init)
                {
                    this.init = true;
                    this.GetCh();
                }
                var token = this.Read();
                if (token != null)
                {
                    this.tokens.Add(token);
                }
                return token;
            }
            public virtual List<CToken> GetTokens()
            {
                var result = new List<CToken>();
                while (true)
                {
                    var token = this.Next();
                    if (token == null)
                    {
                        if (this.code!=0)
                        {
                            var text = string.Format("%d: %s", this.row, this.error);
                            throw new Exception(text);
                        }
                        break;
                    }
                    result.Append(token);
                    if (token.mode == CTOKEN_ENDF)
                    {
                        break;
                    }
                }
                return result;
            }
            public override string ToString() => string.Join(",", this.GetTokens());
        }
        public class CScanner : CTokenizer
        {
            public bool casesensitive = false;
            public List<string> keywords = new();
            public Dictionary<int, string> comments = new();
            public CScanner(TextReader fp, List<string> keywords, bool casesensitive = false)
                :base(fp)
            {
                this.keywords = keywords ?? new List<string>();
                this.casesensitive = casesensitive;
                this.ch = -1;
            }

            public virtual string SkipComments()
            {
                var memo = new StringBuilder();
                while (true)
                {
                    var skip = 0;
                    this.SkipSpaces();
                    if (this.ch == -1)
                    {
                        break;
                    }
                    if (CommentChars.Contains(this.ch))
                    {
                        skip += 1;
                        while (this.ch != '\n' && this.ch != -1)
                        {
                            memo.Append(this.ch);
                            this.GetCh();
                            skip += 1;
                        }
                    }
                    else if (this.ch == '/')
                    {
                        this.GetCh();
                        if (this.ch == '/')
                        {
                            memo.Append(this.ch);
                            skip += 1;
                            while (this.ch != '\n' && this.ch != -1)
                            {
                                memo.Append(this.ch);
                                this.GetCh();
                                skip += 1;
                            }
                        }
                        else
                        {
                            this.Ungetch(this.ch);
                            this.ch = '/';
                        }
                    }
                    if (skip == 0)
                    {
                        break;
                    }
                }
                return memo.ToString();
            }

            public virtual CToken ReadString()
            {
                CToken token = null;
                this.error = "";
                if (!QuoteChars.Contains(this.ch))
                {
                    return null;
                }
                var mode = this.ch == '\'' ? 0 : 1;
                var text = "";
                var done = -1;
                while (true)
                {
                    var ch = this.GetCh();
                    if (ch == '\\')
                    {
                        this.GetCh();
                        text += "\\" + this.ch;
                    }
                    else if (mode == 0 && ch == '\'')
                    {
                        text += "\'";
                    }
                    else if (mode == 1 && ch == '\"')
                    {
                        text += '\"';
                    }
                    else if (mode == 0 && ch == '\"')
                    {
                        ch = this.GetCh();
                        if (ch == '\"')
                        {
                            text += "\"\"";
                        }
                        else
                        {
                            done = 1;
                            token = new CToken(CTOKEN_STR, text, text);
                            this.text = text;
                            break;
                        }
                    }
                    else if (mode == 1 && ch == '\"')
                    {
                        ch = this.GetCh();
                        if (ch == '\"')
                        {
                            text += "\'\'";
                        }
                        else
                        {
                            done = 1;
                            token = new CToken(CTOKEN_STR, text, text);
                            this.text = text;
                            break;
                        }
                    }
                    else if (ch == '\n')
                    {
                        this.error = "EOL while scanning string literal";
                        this.code = 1;
                        break;
                    }
                    else if (ch != -1)
                    {
                        text += ch;
                    }
                    else
                    {
                        this.error = "EOF while scanning string literal";
                        this.code = 2;
                        break;
                    }
                }
                if (token == null)
                {
                    return null;
                }
                token.row = this.row;
                token.col = this.col;
                return token;
            }

            public virtual CToken ReadNumber()
            {
                object value = 0;
                CToken token = null;
                var done = -1;
                if (this.ch <'0' || this.ch > '9')
                {
                    return null;
                }
                var text = "";
                while (this.IsNumber((char)this.ch) || this.ch =='.')
                {
                    text += this.ch;
                    this.GetCh();
                }
                var pos = text.Length;
                while (pos > 0)
                {
                    var ch = text[pos - 1];
                    if (char.IsDigit( ch) || ch =='.')
                    {
                        break;
                    }
                    if (ch >= 'A' && ch <= 'F')
                    {
                        break;
                    }
                    if (ch >= 'a' && ch <= 'f')
                    {
                        break;
                    }
                    pos -= 1;
                }
                if (text.Length - pos > 2)
                {
                    this.error = "number format error";
                    this.code = 1;
                    return null;
                }
                char ec1, ec2;
                if (text.Length - pos == 2)
                {
                     ec1 = text[pos - 2];
                     ec2 = text[pos - 1];
                }
                else if (text.Length - pos == 1)
                {
                    ec1 = text[pos - 1];
                    ec2 = '\0';
                }
                else
                {
                    ec1 = '\0';
                    ec2 = '\0';
                }
                text = text[..pos];
                if (HexHeaders.Contains(text[..2]))
                {
                    try
                    {
                        value = Convert.ToInt64(text, 16);
                    }
                    catch
                    {
                        //Debug
                        this.error = "bad hex number " + text;
                        this.code = 2;
                        return null;
                    }
                    if ((long)value >= -0x80000000 && (long)value <= 0x7fffffff)
                    {
                        value = Convert.ToInt64(value);
                    }
                    token = new CToken(CTOKEN_INT, value, text);
                }
                else if (ec1 == 'h' && ec2 == 0)
                {
                    try
                    {
                        value = Convert.ToInt64(text, 16);
                    }
                    catch
                    {
                        this.error = "bad hex number " + text;
                        this.code = 3;
                        return null;
                    }
                    if ((long)value >= -0x80000000 && (long)value <= 0x7fffffff)
                    {
                        value = Convert.ToInt64(value);
                    }
                    token = new CToken(CTOKEN_INT, value, text);
                }
                else if (ec1 == 'b' && ec2 == 0)
                {
                    try
                    {
                        value = Convert.ToInt64(text, 2);
                    }
                    catch
                    {
                        this.error = "bad binary number " + text;
                        this.code = 4;
                        return null;
                    }
                    if ((long)value >= -0x80000000 && (long)value <= 0x7fffffff)
                    {
                        value = Convert.ToInt64(value);
                    }
                    token = new CToken(CTOKEN_INT, value, text);
                }
                else if (ec1 == 'q' && ec2 == 0)
                {
                    try
                    {
                        value = Convert.ToInt64(text, 8);
                    }
                    catch
                    {
                        this.error = "bad octal number " + text;
                        this.code = 5;
                        return null;
                    }
                    if ((long)value >= -0x80000000 && (long)value <= 0x7fffffff)
                    {
                        value = Convert.ToInt64(value);
                    }
                    token = new CToken(CTOKEN_INT, value, text);
                }
                else
                {
                    var @decimal = !text.Contains(".") ? 1: 0;
                    if (@decimal!=0)
                    {
                        try
                        {
                            value = Convert.ToInt64(text, 10);
                        }
                        catch
                        {
                            this.error = "bad decimal number " + text;
                            this.code = 6;
                            return null;
                        }
                        if ((long)value >= -0x80000000 && (long)value <= 0x7fffffff)
                        {
                            value = Convert.ToInt64(value);
                        }
                        token = new CToken(CTOKEN_INT, value, text);
                    }
                    else
                    {
                        try
                        {
                            value = float.Parse(text);
                        }
                        catch
                        {
                            this.error = "bad float number " + text;
                            this.code = 7;
                            return null;
                        }
                        token = new CToken(CTOKEN_FLOAT, value, text);
                    }
                }
                token.row = this.row;
                token.col = this.col;
                return token;
            }

            public virtual CToken Read()
            {
                int col;
                int row;
                CToken token = null;
                var comment = this.SkipComments();
                if (this.ch == '\n')
                {
                    var lineno = this.row - 1;
                    token = new CToken(CTOKEN_ENDL);
                    token.row = lineno;
                    this.comments[lineno] = comment;
                    comment = "";
                    this.GetCh();
                    return token;
                }
                if (this.ch == -1)
                {
                    this.eof += 1;
                    if (this.eof > 1)
                    {
                        return null;
                    }
                    token = new CToken(CTOKEN_ENDF, 0);
                    token.row = this.row;
                    if (comment.Length>0)
                    {
                        this.comments[this.row] = comment;
                    }
                    return token;
                }
                // this is a string
                if (new int[] { '\n' , '\'' }.Contains(this.ch))
                {
                    row = this.row;
                    col = this.col;
                    this.code = 0;
                    this.error = "";
                    token = this.ReadString();
                    if (this.code!=0)
                    {
                        return null;
                    }
                    token.row = row;
                    token.col = col;
                    return token;
                }
                var issym2f =(char x) => this.IsAlpha(x) || StartChars.Contains(x);
                var issym2x = (char x) => this.IsNumber(x) || StartChars.Contains(x);
                // identity or keyword
                if (issym2f((char)this.ch))
                {
                    row = this.row;
                    col = this.col;
                    var text = "";
                    while (issym2x((char)this.ch))
                    {
                        text += (char)this.ch;
                        this.GetCh();
                    }
                    if (this.keywords.Count>0)
                    {
                        foreach (var i in Enumerable.Range(0, this.keywords.Count))
                        {
                            var same = false;
                            if (this.casesensitive)
                            {
                                same = text == this.keywords[i];
                            }
                            else
                            {
                                same = text.ToLower() == this.keywords[i].ToLower();
                            }
                            if (same)
                            {
                                token = new CToken(CTOKEN_KEYWORD, i, text);
                                token.row = row;
                                token.col = col;
                                return token;
                            }
                        }
                    }
                    token = new CToken(CTOKEN_IDENT, text, text);
                    token.row = row;
                    token.col = col;
                    return token;
                }
                // this is a number
                if (this.ch >= '0' && this.ch <= '9')
                {
                    row = this.row;
                    col = this.col;
                    this.code = 0;
                    this.error = "";
                    token = this.ReadNumber();
                    if (this.code!=0)
                    {
                        return null;
                    }
                    token.row = row;
                    token.col = col;
                    return token;
                }
                // this is an operator
                token = new CToken(CTOKEN_OPERATOR, this.ch, this.ch.ToString());
                token.row = this.row;
                token.col = this.col;
                this.GetCh();
                return token;
            }
        }
        public class COperand
        {

            public string _base;

            public long immediate;

            public string index;

            public string label;

            public int mode;

            public string name;

            public int offset;

            public string reg;

            public int scale;

            public string segment;

            public int size;

            public COperand(TextReader tokens)
            {
                this.mode = -1;
                this.reg = "";
                this._base = "";
                this.index = "";
                this.scale = 0;
                this.offset = 0;
                this.segment = "";
                this.immediate = 0;
                this.label = "";
                this.size = 0;
                this.parse(tokens);
                this.name = "operand";
            }

            public virtual void reset()
            {
                this.reg = "";
                this._base = "";
                this.index = "";
                this.scale = 0;
                this.offset = 0;
                this.immediate = 0;
                this.segment = "";
                this.label = "";
                this.size = 0;
            }

            public virtual List<CToken> parse(TextReader tokens_text)
            {
                var tokens = tokenize(tokens_text);
                while (tokens.Count > 0)
                {
                    if (!new int[] { CTOKEN_ENDF, CTOKEN_ENDL }.Contains(tokens.Last().mode))
                    {
                        break;
                    }
                    tokens.RemoveAt(0);
                }
                this.reset();
                if (tokens.Count >= 2)
                {
                    var t1 = tokens[0];
                    var t2 = tokens[1];
                    if (t2.mode == CTOKEN_IDENT && t2.value.ToString().ToLower() == "ptr")
                    {
                        if (t1.mode == CTOKEN_IDENT)
                        {
                            var size = t1.value.ToString().ToLower();
                            if (size == "byte")
                            {
                                this.size = 1;
                            }
                            else if (size == "word")
                            {
                                this.size = 2;
                            }
                            else if (size == "dword")
                            {
                                this.size = 4;
                            }
                            else if (size == "qword")
                            {
                                this.size = 8;
                            }
                            if (this.size != 0)
                            {
                                tokens = tokens.ToArray()[2..].ToList();
                            }
                        }
                    }
                }
                if (tokens.Count == 0)
                {
                    throw new Exception("expected operand token");
                }
                var head = tokens[0];
                var tail = tokens[-1];
                if (head.mode == CTOKEN_INT)
                {
                    // 如果是立即数
                    this.mode = O_IMM;
                    if (head.value is long im)
                    {
                        this.immediate = im;
                    }
                }
                else if (head.mode == CTOKEN_IDENT)
                {
                    // 寄存器或标识
                    if (isreg(head.value.ToString()))
                    {
                        // 如果是寄存器
                        this.mode = O_REG;
                        this.reg = head.value.ToString();
                        this.size = regsize(this.reg);
                    }
                    else
                    {
                        this.mode = O_LABEL;
                        this.label = head.value as string;
                    }
                }
                else if (head.mode == CTOKEN_OPERATOR)
                {
                    // 如果是符号
                    if (head.value as string == "[")
                    {
                        // 如果是内存
                        this.mode = O_MEM;
                        if (tail.mode != CTOKEN_OPERATOR || tail.value as string != "]")
                        {
                            throw new Exception("bad memory operand");
                        }
                        this.ParseMemory(tokens);
                    }
                    else
                    {
                        throw new Exception("bad operand descript " + (head.value));
                    }
                }
                else
                {
                    throw new Exception("bad operand desc");
                }
                return tokens;
            }

            public virtual int ParseMemory(List<CToken> tokens)
            {
                CToken t1;
                CToken token;
                tokens = tokens.ToArray()[1..-1].ToList();
                if (tokens.Count == 0)
                {
                    throw new Exception("memory operand error");
                }
                this.scale = 1;
                this.index = "";
                this.offset = 0;
                this._base = "";
                var segments = new List<string> {
                "cs",
                "ss",
                "ds",
                "es",
                "fs",
                "gs"
            };
                var pos = -1;
                foreach (var i in Enumerable.Range(0, tokens.Count))
                {
                    token = tokens[i];
                    if (token.mode == CTOKEN_OPERATOR && token.value == ":")
                    {
                        pos = i;
                        break;
                    }
                }
                if (pos >= 0 && pos < tokens.Count)
                {
                    // 如果覆盖段地址
                    if (pos == 0 || pos == tokens.Count - 1)
                    {
                        throw new Exception("memory operand segment error");
                    }
                    t1 = tokens[pos - 1];
                    tokens = tokens.ToArray()[..(pos - 1)].Append(tokens[pos + 1]).ToList();
                    if (t1.mode != CTOKEN_IDENT)
                    {
                        throw new Exception("memory operand segment bad");
                    }
                    var seg = t1.value.ToString().ToLower();
                    if (!segments.Contains(seg))
                    {
                        throw new Exception("memory operand segment unknow");
                    }
                    this.segment = seg;
                }
                pos = -1;
                foreach (var i in Enumerable.Range(0, tokens.Count))
                {
                    token = tokens[i];
                    if (token.mode == CTOKEN_OPERATOR && token.value == "*")
                    {
                        pos = i;
                        break;
                    }
                }
                if (pos >= 0 && pos < tokens.Count)
                {
                    // 如果有乘号
                    if (pos == 0 || pos == tokens.Count - 1)
                    {
                        throw new Exception("memory operand error (bad scale)");
                    }
                    t1 = tokens[pos - 1];
                    var t2 = tokens[pos + 1];
                    tokens = tokens.ToArray()[..(pos - 1)].Append(tokens[pos + 2]).ToList();
                    if (t1.mode == CTOKEN_IDENT && t2.mode == CTOKEN_INT)
                    {
                    }
                    else if (t1.mode == CTOKEN_INT && t2.mode == CTOKEN_IDENT)
                    {
                        t1 = t2;
                        t2 = t1;
                    }
                    else
                    {
                        throw new Exception("memory operand error (scale error)");
                    }
                    if (!isreg(t1.value.ToString()))
                    {
                        throw new Exception("memory operand error (no index register)");
                    }
                    this.index = t1.value.ToString();
                    this.scale = (int)(long)t2.value;
                    if (!ValidScales.Contains(this.scale))
                    {
                        throw new Exception("memory operand error (bad scale number)");
                    }
                }
                //for token in tokens: print token,
                //print ''
                foreach (var token_ in tokens)
                {
                    if (token_.mode == CTOKEN_IDENT && isreg(token_.value as string))
                    {
                        if (this._base == "")
                        {
                            this._base = token_.value as string;
                        }
                        else if (this.index == "")
                        {
                            this.index = token_.value as string;
                        }
                        else
                        {
                            Console.WriteLine(token_);
                            throw new Exception("memory operand error (too many regs)");
                        }
                    }
                    else if (token_.mode == CTOKEN_INT)
                    {
                        if (this.offset == 0)
                        {
                            this.offset = (int)(long)token_.value;
                        }
                        else
                        {
                            throw new Exception("memory operand error (too many offs)");
                        }
                    }
                    else if (token_.mode == CTOKEN_OPERATOR && token_.value == "+")
                    {
                    }
                    else
                    {
                        throw new Exception("operand token error " + (token_));
                    }
                }
                return 0;
            }

            public virtual string GetInfo()
            {
                if (this.mode == O_REG)
                {
                    return String.Format("reg:%s", this.reg);
                }
                else if (this.mode == O_IMM)
                {
                    return String.Format("imm:%d", this.immediate);
                }
                else if (this.mode == O_LABEL)
                {
                    return String.Format("label:%s", this.label);
                }
                var data = new List<string>();
                if (!string.IsNullOrEmpty(this._base))
                {
                    data.Append(this._base);
                }
                if (!string.IsNullOrEmpty(this.index))
                {
                    if (this.scale == 1)
                    {
                        data.Append(String.Format("%s", this.index));
                    }
                    else
                    {
                        data.Append(String.Format("%s * %d", this.index, this.scale));
                    }
                }
                if (this.offset != 0)
                {
                    data.Append(String.Format("0x%x", this.offset));
                }
                var size = "";
                if (this.size == 1)
                {
                    size = "8";
                }
                else if (this.size == 2)
                {
                    size = "16";
                }
                else if (this.size == 4)
                {
                    size = "32";
                }
                else if (this.size == 8)
                {
                    size = "64";
                }
                return String.Format("mem{0}:[+{1}]", size, string.Join(',', data));
            }

            public virtual string translate(int inline = 0)
            {
                var prefix = @"%";
                if (inline != 0)
                {
                    prefix = @"%%";
                }
                if (this.mode == O_REG)
                {
                    return prefix + this.reg;
                }
                if (this.mode == O_IMM)
                {
                    return "$" + string.Format("{0:X}", (this.immediate));
                }
                if (this.mode == O_LABEL)
                {
                    return this.label;
                }
                var text = "";
                var @base = String.IsNullOrEmpty(this._base) ? "" : prefix + this._base;
                var index = String.IsNullOrEmpty(this.index) ? "" : prefix + this.index;
                if (!string.IsNullOrEmpty(this.index))
                {
                    text = String.Format("(%s)", @base);
                }
                else
                {
                    text = String.Format("(%s,%s,%d)", @base, index, this.scale);
                }
                if (this.offset > 0)
                {
                    text = String.Format("0x%x%s", this.offset, text);
                }
                if (!string.IsNullOrEmpty(this.segment))
                {
                    text = String.Format("%s:%s", this.segment, text);
                }
                return text;
            }

            public override string ToString()
            {
                return this.GetInfo() + " -> " + this.translate();
            }
        }

        public class CEncoding
        {

            public bool empty;

            public string instruction;

            public string label;

            public string name;

            public List<COperand> operands;

            public string prefix;

            public int size;

            public List<CToken> tokens;

            public string inst;

            public CEncoding(TextReader tokens)
            {
                this.reset();
                this.parse(tokens);
                this.name = "cencoding";
            }

            public virtual int reset()
            {
                this.label = "";
                this.prefix = "";
                this.instruction = "";
                this.operands = new List<COperand>();
                this.tokens = null;
                this.empty = false;
                return 0;
            }

            public virtual object parse(TextReader tokens_)
            {
                var tokens = tokenize(tokens_);
                while (tokens.Count > 0)
                {
                    if (!new int[] { CTOKEN_ENDF, CTOKEN_ENDL }.Contains(tokens.Last().mode))
                    {
                        break;
                    }
                    tokens.RemoveAt(0);
                }
                if (tokens.Count == 0)
                {
                    this.empty = true;
                    return 0;
                }
                this.reset();
                this.tokens = tokens;
                this.ParseLabel();
                this.ParsePrefix();
                this.ParseInstruction();
                this.ParseOperands();
                this.Update();
                this.tokens = null;
                return 0;
            }

            public virtual int ParseLabel()
            {
                if (this.tokens.Count < 2)
                {
                    return 0;
                }
                var t1 = tokens[tokens.Count - 2];
                var t2 = tokens[tokens.Count - 1];
                if (t2.mode == CTOKEN_OPERATOR && t2.value as string == ":")
                {
                    if (t1.mode != CTOKEN_IDENT)
                    {
                        throw new Exception("error label type");
                    }
                    this.label = t1.value as string;
                    this.tokens = this.tokens.ToArray()[..2].ToList();
                }
                return 0;
            }

            public virtual object ParsePrefix()
            {
                var prefix = new List<string> {
                "lock",
                "rep",
                "repne",
                "repnz",
                "repe",
                "repz"
            };
                var segments = new List<string> {
                "cs",
                "ss",
                "ds",
                "es",
                "fs",
                "gs"
            };
                while (this.tokens.Count >= 1)
                {
                    var t1 = this.tokens[0];
                    if (t1.mode != CTOKEN_IDENT)
                    {
                        break;
                    }
                    var text = t1.value.ToString().ToLower();
                    if (!prefix.Contains(text) && !segments.Contains(text))
                    {
                        break;
                    }
                    this.prefix += " " + text;
                    this.prefix = this.prefix.Trim();
                    this.tokens = this.tokens.ToArray()[1..].ToList();
                }
                return 0;
            }

            public virtual int ParseInstruction()
            {
                if (this.tokens.Count < 1)
                {
                    return 0;
                }
                var t1 = this.tokens[0];
                this.tokens = this.tokens.ToArray()[1..].ToList();
                if (t1.mode != CTOKEN_IDENT)
                {
                    throw new Exception("instruction type error");
                }
                this.instruction = t1.value as string;
                return 0;
            }

            public virtual int ParseOperands()
            {
                var operands = new List<CToken>();
                while (this.tokens.Count > 0)
                {
                    var size = this.tokens.Count;
                    var pos = size;
                    foreach (var i in Enumerable.Range(0, size))
                    {
                        if (this.tokens[i].mode == CTOKEN_OPERATOR)
                        {
                            if (this.tokens[i].value == ",")
                            {
                                pos = i;
                                break;
                            }
                        }
                    }
                    operands.AddRange(this.tokens.ToArray()[..pos]);
                    this.tokens = this.tokens.ToArray()[(pos + 1)..].ToList();
                }
                foreach (var token in operands)
                {
                    //TODO:
                    var n = new COperand(null);
                    this.operands.Add(n);
                }
                operands = null;
                return 0;
            }

            public virtual int Update()
            {
                this.size = 0;
                foreach (var operand in this.operands)
                {
                    if (operand.size > this.size)
                    {
                        this.size = operand.size;
                    }
                }
                if (this.prefix == "" && this.instruction == "")
                {
                    if (this.operands.Count == 0)
                    {
                        this.empty = true;
                    }
                }
                return 0;
            }

            public virtual string translate_instruction()
            {
                var instruction = this.instruction.ToLower();
                if (instruction == "align")
                {
                    return ".align";
                }
                if (Instreplace.ContainsKey(instruction))
                {
                    instruction = Instreplace[instruction];
                }
                var postfix = false;
                if (this.operands.Count == 1)
                {
                    var o = this.operands[0];
                    if (o.mode == O_MEM)
                    {
                        postfix = true;
                    }
                    else if (o.mode == O_LABEL)
                    {
                        postfix = true;
                    }
                }
                else if (this.operands.Count == 2)
                {
                    var o1 = this.operands[0];
                    var o2 = this.operands[1];

                    if (o1.mode == O_IMM && o2.mode == O_MEM)
                    {
                        postfix = true;
                    }
                    if (o1.mode == O_MEM && o2.mode == O_IMM)
                    {
                        postfix = true;
                    }
                    if (o1.mode == O_IMM && o2.mode == O_LABEL)
                    {
                        postfix = true;
                    }
                    if (o1.mode == O_LABEL && o2.mode == O_IMM)
                    {
                        postfix = true;
                    }
                }
                if (postfix)
                {
                    if (this.size == 1)
                    {
                        instruction += "b";
                    }
                    else if (this.size == 2)
                    {
                        instruction += "w";
                    }
                    else if (this.size == 4)
                    {
                        instruction += "l";
                    }
                    else if (this.size == 8)
                    {
                        instruction += "q";
                    }
                }
                //if (!ToLower)
                {
                    instruction = instruction.ToUpper();
                }
                return instruction;
            }

            public virtual string translate_operand(int id, int inline = 0)
            {
                var desc = new List<object>();
                if (this.instruction.ToLower() == "align")
                {
                    var size = 4;
                    if (this.operands.Count > 0)
                    {
                        var op = this.operands[0];
                        if (op.mode == O_IMM)
                        {
                            size = (int)op.immediate;
                        }
                    }
                    if (id == 0)
                    {
                        return String.Format("%d, 0x90", size);
                    }
                    return "";
                }
                if (id < 0 || id >= this.operands.Count)
                {
                    throw new Exception("operand id out of range");
                }
                var text = this.operands[id].translate(inline);
                return text;
            }

            public override string ToString()
            {
                var text = "";
                if (!string.IsNullOrEmpty(this.label))
                {
                    text += String.Format("%s: ", this.label);
                }
                if (!string.IsNullOrEmpty(this.prefix))
                {
                    text += String.Format("%s ", this.prefix);
                }
                text += this.translate_instruction();
                text += " ";
                text += this.translate_operand(0);
                return text;
            }
        }

        public class CSynthesis
        {

            public bool amd64;

            public List<CEncoding> encoding;

            public string error;

            public int indent1;

            public int indent2;

            public Dictionary<string, (int,int)> labels;

            public List<List<CToken>> lines;

            public Dictionary<string, (int,bool)> maps;

            public Dictionary<int, string> memos;

            public string name;

            public Dictionary<string, List<(int,int)>> references;

            public Dictionary<object, object> registers;

            public int size;

            public string source;

            public List<object> table;

            public List<object> tokens;

            public List<(int,string,bool)> variables;

            public Dictionary<string, List<(string,int,int,int)>> vars;

            public CSynthesis(TextReader source)
            {
                this.reset();
                if (source != null)
                {
                    this.parse(source);
                }
                this.name = "csynthesis";
            }

            public virtual void reset()
            {
                this.source = "";
                this.tokens = new();
                this.encoding = new ();
                this.labels = new();
                this.references = new();
                this.lines = new ();
                this.memos = new();
                this.table = new();
                this.vars = new();
                this.maps = new();
                this.variables = new ();
                this.registers = new();
                this.amd64 = false;
                this.size = 0;
                this.error = "";
            }

            public virtual int parse(TextReader source)
            {
                this.reset();
                if (this.DoTokenize(source) != 0)
                {
                    return -1;
                }
                if (this.DoEncoding() != 0)
                {
                    return -2;
                }
                if (this.DoAnalyse() != 0)
                {
                    return -3;
                }
                return 0;
            }

            public virtual int DoTokenize(TextReader source = null)
            {
                var scanner = new CScanner(source, new());
                var tokens = new List<CToken>();
                while (true)
                {
                    var token = scanner.Next();
                    if (token == null)
                    {
                        var text = String.Format("%d: %s", scanner.row, scanner.error);
                        this.error = text;
                        return -1;
                    }
                    tokens.Append(token);
                    if (token.mode == CTOKEN_ENDF)
                    {
                        break;
                    }
                }
                while (tokens.Count > 0)
                {
                    var size = tokens.Count;
                    var pos = size - 1;
                    foreach (var i in Enumerable.Range(0, size))
                    {
                        if (tokens[i].mode == CTOKEN_ENDL)
                        {
                            pos = i;
                            break;
                        }
                    }
                    this.lines.Add(tokens.ToArray()[..(pos + 1)].ToList());
                    tokens = tokens.ToArray()[(pos + 1)..].ToList();
                }
                foreach (var i in Enumerable.Range(0, this.lines.Count))
                {
                    var lineno = i + 1;
                    if (scanner.comments.ContainsKey(lineno))
                    {
                        this.memos[i] = scanner.comments[lineno].Trim(new char[] {'\r','\n','\t',' '});
                    }
                    else
                    {
                        this.memos[i] = "";
                    }
                }
                scanner = null;
                return 0;
            }

            public virtual int DoEncoding()
            {
                var lineno = 1;
                foreach (var tokens in this.lines)
                {
                    try
                    {
                        //TODO: tokens
                        var encoding = new CEncoding(null);
                    }
                    catch (Exception e)
                    {
                        var text = String.Format("%d: %s", lineno, e);
                        this.error = text;
                        return -1;
                    }
                    this.encoding.AddRange(encoding);
                    lineno += 1;
                }
                if (this.lines.Count != this.encoding.Count)
                {
                    throw new Exception("core fault");
                }
                return 0;
            }

            public virtual int DoAnalyse()
            {
                CEncoding encoding;
                this.size = this.lines.Count;
                var index = 0;
                var amd64 = new List<string> { "rax", "rbx", "rcx", "rdx", "rdi", "rsi", "rbp", "rsp" };
                foreach (var i in Enumerable.Range(0, this.size))
                {
                    encoding = this.encoding[i];
                    if (encoding.label != null)
                    {
                        index += 1;
                        this.labels[encoding.label] = (i, index);
                    }
                }
                var varlist = new List<(string,int,int)>();
                foreach (var i in Enumerable.Range(0, this.size))
                {
                    encoding = this.encoding[i];
                    foreach (var j in Enumerable.Range(0, encoding.operands.Count))
                    {
                        var operand = encoding.operands[j];
                        if (operand.mode == O_LABEL)
                        {
                            if (!this.labels.ContainsKey(operand.label))
                            {
                                varlist.Add((operand.label, i, j));
                            }
                            else
                            {
                                if(!this.references.TryGetValue(operand.label, out var desc))
                                { 
                                    this.references[operand.label] = desc = new List<(int, int)>();
                                }

                                desc.Add((i, j));
                            }
                        }
                        else if (operand.mode == O_REG)
                        {
                            var reg = operand.reg.ToLower();
                            this.registers[reg] = 1;
                            if (amd64.Contains(reg))
                            {
                                this.amd64 = true;
                            }
                        }
                    }
                }
                var vartable = new List<(string,int,int)>();
                foreach (var (var, line, pos) in varlist)
                {
                    if (pos == 0)
                    {
                        vartable.Add((var, line, pos));
                    }
                }
                foreach (var (var, line, pos) in varlist)
                {
                    if (pos != 0)
                    {
                        vartable.Add((var, line, pos));
                    }
                }
                var names = new Dictionary<string, int>();
               
                foreach (var i in Enumerable.Range(0, vartable.Count))
                {
                    List<(string, int,int,int)> desc = null;

                    var (var, line, pos) = vartable[i];
                    if(!this.vars.TryGetValue(var, out desc))
                    {
                        desc = new ();
                    }

                    if (desc.Count == 0)
                    {
                        index = names.Count;
                        names[var] = index;
                        this.table.Append((var, line, pos, index));
                        var writable = pos == 0;
                        this.maps[var] = (index, writable);
                        this.variables.Add((index, var, writable));
                    }
                    else
                    {
                        index = names[var];
                    }
                    desc.Add((var, line, pos, index));
                    this.vars[var] = desc;
                }
                var indent1 = 0;
                var indent2 = 0;
                foreach (var i in Enumerable.Range(0, this.size))
                {
                    encoding = this.encoding[i];
                    encoding.inst = encoding.translate_instruction();
                    if (!string.IsNullOrEmpty(encoding.label) && !encoding.empty)
                    {
                        if (encoding.label.Length > indent1)
                        {
                            indent1 = encoding.label.Length;
                        }
                    }
                    if (encoding.inst.Length > indent2)
                    {
                        indent2 = encoding.inst.Length;
                    }
                }
                this.indent1 = indent1 + 2;
                this.indent2 = indent2;
                if (this.indent1 < 4)
                {
                    this.indent1 = 4;
                }
                if (this.indent2 < 4)
                {
                    this.indent2 = 4;
                }
                return 0;
            }

            public virtual string get_label(int lineno, int clabel = 0)
            {
                if (lineno < 0 || lineno >= this.size)
                {
                    throw new Exception("line number out of range");
                }
                var encoding = this.encoding[lineno];
                if (encoding.label == "")
                {
                    return "";
                }
                if (clabel == 0)
                {
                    return encoding.label + ":";
                }
                var (line, index) = this.labels[encoding.label];
                return String.Format("%d:", index);
            }

            public virtual string get_instruction(int lineno)
            {
                if (lineno < 0 || lineno >= this.size)
                {
                    throw new Exception("line number out of range");
                }
                var encoding = this.encoding[lineno];
                if (encoding.empty)
                {
                    return "";
                }
                var source = encoding.prefix + " " + encoding.inst;
                source = source.Trim();
                return source;
            }

            public virtual string get_operand(int lineno, int id, int clabel = 0, int inline = 0)
            {
                if (lineno < 0 || lineno >= this.size)
                {
                    throw new Exception("line number out of range");
                }
                var encoding = this.encoding[lineno];
                if (id < 0 || id >= encoding.operands.Count)
                {
                    throw new Exception("operand id out of range");
                }
                var operand = encoding.operands[id];
                if (new int[] { O_IMM, O_REG, O_MEM }.Contains(operand.mode))
                {
                    return operand.translate(inline);
                }
                var label = operand.label;
                if (this.labels.ContainsKey(label))
                {
                    // this is a jmp label
                    if (clabel == 0)
                    {
                        return label;
                    }
                    var (line, index) = this.labels[label];
                    if (line <= lineno)
                    {
                        return String.Format("%db", index);
                    }
                    return String.Format("%df", index);
                }
                if (this.vars.ContainsKey(label))
                {
                    // this is a variable
                    var (idx, writable) = this.maps[label];
                    return String.Format("%%%d", idx);
                }
                return "$" + label;
            }

            public virtual string synthesis(int lineno, int clabel = 0, int inline = 0, int align = 0)
            {
                COperand op;
                List<string> operands;
                if (lineno < 0 || lineno >= this.size)
                {
                    throw new Exception("line number out of range");
                }
                var encoding = this.encoding[lineno];
                var source = this.get_label(lineno, clabel);
                // 内容缩进
                var indent = this.indent1;
                if (clabel!=0)
                {
                    indent = this.labels.Count + 2;
                }
                indent = Convert.ToInt32((indent + 1) / 2 * 2);
                source = source.PadLeft(indent);
                // 没有指令
                if (encoding.empty)
                {
                    return source;
                }
                var instruction = this.get_instruction(lineno);
                if (align!=0)
                {
                    indent = Convert.ToInt32((this.indent2 + 3) / 4 * 4);
                    instruction = instruction.PadLeft(indent);
                }
                source += instruction + " ";
                if (encoding.instruction.ToLower() == "align")
                {
                    var size = 4;
                    if (encoding.operands.Count > 0)
                    {
                        var operand = encoding.operands[0];
                        if (operand.mode == O_IMM)
                        {
                            size =(int) operand.immediate;
                        }
                    }
                    source += String.Format("%d, 0x90", size);
                }
                else if (new string[] { ".byte", ".int", ".short" }.Contains(encoding.inst.ToLower()))
                {
                    operands = new List<string>();
                    foreach (var i in Enumerable.Range(0, encoding.operands.Count))
                    {
                        op = encoding.operands[i];
                        var text = string.Format("{0:X}",(op.immediate));
                        operands.Add(text);
                    }
                    source += string.Join(',',operands);
                }
                else
                {
                    operands = new List<string>();
                    foreach (var i in Enumerable.Range(0, encoding.operands.Count))
                    {
                        var opx = this.get_operand(lineno, i, clabel, inline);
                        operands.Add(opx);
                    }
                    operands.Reverse();
                    source += string.Join(',',operands);
                }
                source = source.TrimEnd(' ');
                return source;
            }

            public virtual string getvars(int mode = 0)
            {
                var vars = new List<object>();
                foreach (var (id, name, writable) in this.variables)
                {
                    if (mode == 0 && writable)
                    {
                        vars.Append(String.Format("\"=m\"(%s)", name));
                    }
                    else if (mode == 1 && !writable)
                    {
                        vars.Append(String.Format("\"m\"(%s)", name));
                    }
                }
                var text = string.Join(',',vars);
                return text;
            }

            public virtual string getregs()
            {
                if (this.amd64)
                {
                    return "\"rsi\", \"rdi\", \"rax\", \"rbx\", \"rcx\", \"rdx\"";
                }
                return "\"esi\", \"edi\", \"eax\", \"ebx\", \"ecx\", \"edx\"";
            }
        }


        public static List<CToken> tokenize(TextReader script)
        {
            var scanner = new CScanner(script, new());
            var result = scanner.GetTokens();
            scanner.Reset();
            return result;
        }
        
        public class CIntel2GAS
        {

            public Dictionary<string, string> config;

            public object error;

            public List<string> lines;

            public int maxsize;

            public Dictionary<int, string> memos;

            public List<string> output;

            public CSynthesis synthesis;

            public CIntel2GAS()
            {
                this.synthesis = new CSynthesis(null);
                this.config = new();
                this.config["align"] = "0";
                this.config["inline"] = "0";
                this.config["clabel"] = "0";
                this.config["memo"] = "0";
                this.error = "";
                this.lines = new List<string>();
                this.output = new List<string>();
                this.option();
            }

            public virtual int option(int align = 1, int inline = 1, int clabel = 1, int memo = 1)
            {
                this.config["align"] = align.ToString();
                this.config["clabel"] = clabel.ToString();
                this.config["inline"] = inline.ToString();
                this.config["memo"] = memo.ToString();
                return 0;
            }

            public virtual int Parse(TextReader source, int clabel, int inline, int align)
            {
                this.lines = new List<string>();
                this.output = new List<string>();
                this.memos = new();
                var retval = this.synthesis.parse(source);
                this.error = this.synthesis.error;
                if (retval != 0)
                {
                    return retval;
                }
                foreach (var i in Enumerable.Range(0, this.synthesis.size))
                {
                    var text = this.synthesis.synthesis(i, clabel, inline, align);
                    this.lines.Append(text);
                    var memo = this.synthesis.memos[i].Trim(new char[] {'\r','\n','\t',' '});
                    if (memo[..1] == ";")
                    {
                        memo = memo[1].ToString();
                    }
                    memo = memo.Trim(new char[] { '\r', '\n', '\t', ' ' });
                    if (memo[..2] != "//" && memo != "")
                    {
                        memo = "//" + memo;
                    }
                    this.memos[i] = memo;
                }
                this.maxsize = 0;
                foreach (var text in this.lines)
                {
                    if (text.Length > this.maxsize)
                    {
                        this.maxsize = text.Length;
                    }
                }
                this.maxsize = (this.maxsize + 6) / 2 * 2;
                return 0;
            }

            public virtual int intel2gas(TextReader source)
            {
                this.lines = new ();
                this.output = new ();
                var clabel = !string.IsNullOrEmpty(this.config["clabel"])?1:0;
                var inline = !string.IsNullOrEmpty(this.config["inline"])?1:0;
                var align = !string.IsNullOrEmpty(this.config["align"])?1:0;
                var memo = !string.IsNullOrEmpty(this.config["memo"])?1:0;
                var retval = this.Parse(source, clabel, inline, align);
                var prefix = "";
                if (retval != 0)
                {
                    return retval;
                }
                if (inline!=0)
                {
                    this.output.Add("__asm__ __volatile__ (");
                    prefix = "  ";
                }
                foreach (var i in Enumerable.Range(0, this.synthesis.size))
                {
                    var line = this.lines[i];
                    if (line.Trim(new[] {'\r','\t','\n',' '} ) == "")
                    {
                        if (this.memos[i] == "" || memo == 0)
                        {
                            if (inline!=0)
                            {
                                this.output.Add(prefix + "");
                            }
                            else
                            {
                                this.output.Add("");
                            }
                        }
                        else
                        {
                            this.output.Add(prefix + this.memos[i]);
                        }
                    }
                    else
                    {
                        if (inline!=0)
                        {
                            line = "\"" + line + "\\n\"";
                        }
                        if (!string.IsNullOrEmpty(this.memos[i]) && 0!=(memo))
                        {
                            line = line.PadLeft(Convert.ToInt32(this.maxsize)) + this.memos[i];
                        }
                        this.output.Append(prefix + line);
                    }
                }
                if (inline!=0)
                {
                    this.output.Append("  :" + this.synthesis.getvars(0));
                    this.output.Append("  :" + this.synthesis.getvars(1));
                    this.output.Append("  :\"memory\", " + this.synthesis.getregs());
                    this.output.Append(");");
                }
                return 0;
            }
        }

        public static int Main(string[] args)
        {
            var align = 0;
            var memo = 0;
            var clabel = 0;
            var inline = 0;
            foreach (var _argv in args)
            {
                var argv = _argv.ToLower();
                if (argv == "-i")
                {
                    inline = 1;
                }
                if (argv == "-a")
                {
                    align = 1;
                }
                if (argv == "-l")
                {
                    clabel = 1;
                }
                if (argv == "-m")
                {
                    memo = 1;
                }
            }
            var source = Console.In;
            var intel2gas = new CIntel2GAS();
            intel2gas.option(align, inline, clabel, memo);
            if (intel2gas.intel2gas(source) != 0)
            {
                Console.Error.WriteLine("error: " + intel2gas.error);
                return -1;
            }
            foreach (var line in intel2gas.output)
            {
                Console.WriteLine(line);
            }
            return 0;
        }

    }

}
