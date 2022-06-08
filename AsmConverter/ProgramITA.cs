using System.Text;

namespace AsmConverter
{
    public partial class Program
    {
        public static void ConvertFromIntel_ToATT(FileInfo fi, FileInfo fo)
        {
            using var InputReader = new StreamReader(fi.FullName);
            using var OutputWriter = new StreamWriter(fo.FullName);

            var ita = new CIntelToATT();
            ita.SetOptions(true, false, true, true);
            var (output, result) = ita.Convert(InputReader);
            if(result == 0)
            {
                foreach (var line in output)
                {
                    OutputWriter.WriteLine(line);
                }
            }
        }
    }

    public class CToken
    {
        public int Mode;
        public int Line;
        public int Row;
        public int Col;
        public string Source;
        public string Text;
        public string Fd;
        public object Value;
        public string TextValue => this.Value is string s ? s:string.Empty;
        public CToken(
            int mode = 0,
            object value = null,
            string text = "",
            int row = -1,
            int col = -1,
            string fd = "",
            string source = "")
        {
            this.Mode = mode;
            this.Value = value;
            this.Text = text;
            this.Row = row;
            this.Col = col;
            this.Fd = fd;
            this.Source = source;
        }

        public virtual CToken Copy() => new
                (this.Mode, this.Value, this.Text, this.Line, 1, this.Fd, this.Source);

        public virtual bool IsEndl => this.Mode == CIntelToATT.CTOKEN_ENDL;

        public virtual bool IsEndf => this.Mode == CIntelToATT.CTOKEN_ENDF;

        public virtual bool IsIdent => this.Mode == CIntelToATT.CTOKEN_IDENT;

        public virtual bool IsKeyword => this.Mode == CIntelToATT.CTOKEN_KEYWORD;

        public virtual bool IsStr => this.Mode == CIntelToATT.CTOKEN_STR;

        public virtual bool IsOperator => this.Mode == CIntelToATT.CTOKEN_OPERATOR;

        public virtual bool IsInt => this.Mode == CIntelToATT.CTOKEN_INT;

        public virtual bool IsFloat => this.Mode == CIntelToATT.CTOKEN_FLOAT;

        public virtual bool IsError => this.Mode == CIntelToATT.CTOKEN_ERROR;

        public override string ToString() => string.Format("({0}, {1})", CIntelToATT.CTOKEN_NAME[this.Mode], (this.Value));
    }
    public class CTokenizer
    {
        public int CH = -1;
        public int UN = -1;
        public int Col = 1;
        public int Row = 1;
        public int Eofs = 0;
        public int State = 0;
        public string Text = "";
        public string Error { 
            get => this.err; 
            set => this.err = value; 
        }
        protected string err = "";
        public bool Inited = false;
        public int Code = 0;
        public TextReader Reader = null;
        public List<CToken> Tokens = new();
        public CTokenizer(TextReader Reader)
        {
            this.Reader = Reader;
            this.Reset();
        }
        public virtual void Reset()
        {
            this.CH = -1;
            this.UN = -1;
            this.Col = 0;
            this.Row = 1;
            this.Eofs = 0;
            this.State = 0;
            this.Text = "";
            this.Inited = false;
            this.Error = "";
            this.Code = 0;
            this.Tokens = new ();
        }
        public virtual int GetCh()
        {
            int ch = -1;
            if (UN != -1)
            {
                this.CH = this.UN;
                this.UN = -1;
                return this.CH;
            }
            try
            {
                ch = this.Reader.Read();
            }
            catch
            {
                ch = '\0';
            }
            this.CH = ch;
            if (ch == '\n')
            {
                this.Col = 1;
                this.Row += 1;
            }
            else
            {
                this.Col += 1;
            }
            return this.CH;
        }
        public virtual void Ungetch(int ch) => this.UN = ch;
        public virtual bool IsSpace(char ch) => CIntelToATT.Spaces.Contains(ch);
        public virtual bool IsLetterOrDigit(char ch) => char.IsLetterOrDigit(ch);
        public virtual bool IsDigit(char ch) => char.IsDigit(ch);
        public virtual int SkipSpaces()
        {
            var skip = 0;
            while (true)
            {
                if (this.CH == -1)
                { 
                    return -1;
                }
                if (!CIntelToATT.Spaces.Contains(this.CH))
                {
                    break;
                }
                if (this.CH == '\n')
                {
                    break;
                }
                this.GetCh();
                skip += 1;
            }
            return skip;
        }
        public virtual CToken Read() => null;
        public virtual CToken Next()
        {
            if (!this.Inited)
            {
                this.Inited = true;
                this.GetCh();
            }
            var token = this.Read();
            if (token != null)
            {
                this.Tokens.Add(token);
            }
            else
            {

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
                    if (this.Code != 0)
                    {
                        var text = string.Format("{0}: {1}", this.Row, this.Error);
                        throw new Exception(text);
                    }
                    break;
                }
                result.Add(token);
                if (token.Mode == CIntelToATT.CTOKEN_ENDF) break;
            }
            return result;
        }
        public override string ToString() => string.Join(",", this.GetTokens());
    }
    public class CScanner : CTokenizer
    {
        public bool CaseSensitive = false;
        public List<string> Keywords = new();
        public Dictionary<int, string> Comments = new();
        public CScanner(TextReader fp, List<string> keywords, bool casesensitive = false)
            : base(fp)
        {
            this.Keywords = keywords ?? new List<string>();
            this.CaseSensitive = casesensitive;
            this.CH = -1;
        }
        public virtual string SkipComments()
        {
            var memo = new StringBuilder();
            while (true)
            {
                var skip = this.SkipSpaces();
                if (this.CH == -1)
                {
                    break;
                }
                if (CIntelToATT.CommentChars.Contains(this.CH))
                {
                    skip += 1;
                    while (this.CH != '\n' && this.CH != -1)
                    {
                        memo.Append((char)this.CH);
                        this.GetCh();
                        skip += 1;
                    }
                }
                else if (this.CH == '/')
                {
                    this.GetCh();
                    if (this.CH == '/')
                    {
                        memo.Append((char)this.CH);
                        skip += 1;
                        while (this.CH != '\n' && this.CH != -1)
                        {
                            memo.Append((char)this.CH);
                            this.GetCh();
                            skip += 1;
                        }
                    }
                    else
                    {
                        this.Ungetch(this.CH);
                        this.CH = '/';
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
            this.Error = "";
            if (!CIntelToATT.QuoteChars.Contains(this.CH)) return null;

            CToken token = null;
            var mode = this.CH == '\'' ? 0 : 1;
            var text = "";
            while (true)
            {
                var ch = this.GetCh();
                if (ch == '\\')
                {
                    this.GetCh();
                    text += "\\" + this.CH;
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
                        token = new(CIntelToATT.CTOKEN_STR, text, text);
                        this.Text = text;
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
                        token = new(CIntelToATT.CTOKEN_STR, text, text);
                        this.Text = text;
                        break;
                    }
                }
                else if (ch == '\n')
                {
                    this.Error = "EOL while scanning string literal";
                    this.Code = 1;
                    break;
                }
                else if (ch != -1)
                {
                    text += ch;
                }
                else
                {
                    this.Error = "EOF while scanning string literal";
                    this.Code = 2;
                    break;
                }
            }
            if (token == null) return null;
            token.Row = this.Row;
            token.Col = this.Col;
            return token;
        }
        public virtual CToken ReadNumber()
        {
            object value = 0;
            if (this.CH < '0' || this.CH > '9') return null;
            CToken token = null;
            var text = "";
            while (this.IsDigit((char)this.CH) || this.CH == '.')
            {
                text += this.CH;
                this.GetCh();
            }
            var pos = text.Length;
            while (pos > 0)
            {
                var ch = text[pos - 1];
                if (char.IsDigit(ch) || ch == '.')
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
                this.Error = "number format error";
                this.Code = 1;
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
            if (CIntelToATT.HexHeaders.Contains(text[..2]))
            {
                try
                {
                    value = Convert.ToInt64(text, 16);
                }
                catch
                {
                    //Debug
                    this.Error = "bad hex number " + text;
                    this.Code = 2;
                    return null;
                }
                if ((long)value >= -0x80000000 && (long)value <= 0x7fffffff)
                {
                    value = Convert.ToInt64(value);
                }
                token = new(CIntelToATT.CTOKEN_INT, value, text);
            }
            else if (ec1 == 'h' && ec2 == 0)
            {
                try
                {
                    value = Convert.ToInt64(text, 16);
                }
                catch
                {
                    this.Error = "bad hex number " + text;
                    this.Code = 3;
                    return null;
                }
                if ((long)value >= -0x80000000 && (long)value <= 0x7fffffff)
                {
                    value = Convert.ToInt64(value);
                }
                token = new(CIntelToATT.CTOKEN_INT, value, text);
            }
            else if (ec1 == 'b' && ec2 == 0)
            {
                try
                {
                    value = Convert.ToInt64(text, 2);
                }
                catch
                {
                    this.Error = "bad binary number " + text;
                    this.Code = 4;
                    return null;
                }
                if ((long)value >= -0x80000000 && (long)value <= 0x7fffffff)
                {
                    value = Convert.ToInt64(value);
                }
                token = new(CIntelToATT.CTOKEN_INT, value, text);
            }
            else if (ec1 == 'q' && ec2 == 0)
            {
                try
                {
                    value = Convert.ToInt64(text, 8);
                }
                catch
                {
                    this.Error = "bad octal number " + text;
                    this.Code = 5;
                    return null;
                }
                if ((long)value >= -0x80000000 && (long)value <= 0x7fffffff)
                {
                    value = Convert.ToInt64(value);
                }
                token = new(CIntelToATT.CTOKEN_INT, value, text);
            }
            else
            {
                var @decimal = !text.Contains(".") ? 1 : 0;
                if (@decimal != 0)
                {
                    try
                    {
                        value = Convert.ToInt64(text, 10);
                    }
                    catch
                    {
                        this.Error = "bad decimal number " + text;
                        this.Code = 6;
                        return null;
                    }
                    if ((long)value >= -0x80000000 && (long)value <= 0x7fffffff)
                    {
                        value = Convert.ToInt64(value);
                    }
                    token = new(CIntelToATT.CTOKEN_INT, value, text);
                }
                else
                {
                    try
                    {
                        value = float.Parse(text);
                    }
                    catch
                    {
                        this.Error = "bad float number " + text;
                        this.Code = 7;
                        return null;
                    }
                    token = new(CIntelToATT.CTOKEN_FLOAT, value, text);
                }
            }
            token.Row = this.Row;
            token.Col = this.Col;
            return token;
        }
        public override CToken Read()
        {
            int col = 0;
            int row = 0;
            CToken token = null;
            var comment = this.SkipComments();
            if (this.CH == '\n')
            {
                var lineno = this.Row - 1;
                token = new (CIntelToATT.CTOKEN_ENDL);
                token.Row = lineno;
                this.Comments[lineno] = comment;
                comment = "";
                this.GetCh();
                return token;
            }
            if (this.CH == -1)
            {
                this.Eofs += 1;
                if (this.Eofs > 1) return token;
                token = new (CIntelToATT.CTOKEN_ENDF, 0);
                token.Row = this.Row;
                if (comment.Length > 0)
                {
                    this.Comments[this.Row] = comment;
                }
                return token;
            }
            // this is a string
            if (CIntelToATT.StringChars.Contains(this.CH))
            {
                row = this.Row;
                col = this.Col;
                this.Code = 0;
                this.Error = "";
                token = this.ReadString();
                if (this.Code != 0) return null;
                token.Row = row;
                token.Col = col;
                return token;
            }
            // identity or keyword
            if (this.IsLetterOrDigit((char)this.CH)|| CIntelToATT.StartChars.Contains((char)this.CH))
            {
                row = this.Row;
                col = this.Col;
                var text = "";
                while (this.IsLetterOrDigit((char)this.CH) || CIntelToATT.StartChars.Contains((char)this.CH))
                {
                    text += (char)this.CH;
                    this.GetCh();
                }
                if (this.Keywords.Count > 0)
                {
                    for(int i = 0;i< this.Keywords.Count;i++)
                    {
                        var same = false;
                        if (this.CaseSensitive)
                        {
                            same = text == this.Keywords[i];
                        }
                        else
                        {
                            same = text.ToLower() == this.Keywords[i].ToLower();
                        }
                        if (same)
                        {
                            token = new(CIntelToATT.CTOKEN_KEYWORD, i, text);
                            token.Row = row;
                            token.Col = col;
                            return token;
                        }
                    }
                }
                token = new(CIntelToATT.CTOKEN_IDENT, text, text);
                token.Row = row;
                token.Col = col;
                return token;
            }
            // this is a number
            if (this.CH >= '0' && this.CH <= '9')
            {
                row = this.Row;
                col = this.Col;
                this.Code = 0;
                this.Error = "";
                token = this.ReadNumber();
                if (this.Code != 0) return null;
                token.Row = row;
                token.Col = col;
                return token;
            }
            // this is an operator
            token = new(CIntelToATT.CTOKEN_OPERATOR, this.CH, ((char)this.CH).ToString());
            token.Row = this.Row;
            token.Col = this.Col;
            this.GetCh();
            return token;
        }
    }
    public class COperand
    {
        public string _Base = "";
        public long Immediate = 0;
        public string Index = "";
        public string Label = "";
        public string Name = "";
        public string Reg = "";
        public string Segment = "";
        public int Mode = 0;
        public int Offset = 0;
        public int Scale = 0;
        public int Size = 0;
        public static readonly List<string> segments = new List<string> {
                "cs",
                "ss",
                "ds",
                "es",
                "fs",
                "gs"
            };

        public COperand(List<CToken> tokens)
        {
            this.Mode = -1;
            this.Reg = "";
            this._Base = "";
            this.Index = "";
            this.Scale = 0;
            this.Offset = 0;
            this.Segment = "";
            this.Immediate = 0;
            this.Label = "";
            this.Size = 0;
            this.Parse(tokens);
            this.Name = "operand";
        }

        public virtual void Reset()
        {
            this.Reg = "";
            this._Base = "";
            this.Index = "";
            this.Scale = 0;
            this.Offset = 0;
            this.Immediate = 0;
            this.Segment = "";
            this.Label = "";
            this.Size = 0;
        }

        public virtual List<CToken> Parse(List<CToken> tokens)
        {
            while (tokens.Count > 0)
            {
                if (!CIntelToATT.EndTokens.Contains(tokens.Last().Mode))
                {
                    break;
                }
                tokens.RemoveAt(0);
            }
            this.Reset();
            if (tokens.Count >= 2)
            {
                var t1 = tokens[0];
                var t2 = tokens[1];
                if (t2.Mode == CIntelToATT.CTOKEN_IDENT && (t2.TextValue).ToLower() == "ptr")
                {
                    if (t1.Mode == CIntelToATT.CTOKEN_IDENT)
                    {
                        var size = t1.TextValue.ToLower();
                        if (size == "byte")
                        {
                            this.Size = 1;
                        }
                        else if (size == "word")
                        {
                            this.Size = 2;
                        }
                        else if (size == "dword")
                        {
                            this.Size = 4;
                        }
                        else if (size == "qword")
                        {
                            this.Size = 8;
                        }
                        if (this.Size != 0)
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
            var tail = tokens[^1];
            if (head.Mode == CIntelToATT.CTOKEN_INT)
            {
                // 如果是立即数
                this.Mode = CIntelToATT.O_IMM;
                if (head.Value is long im)
                {
                    this.Immediate = im;
                }
            }
            else if (head.Mode == CIntelToATT.CTOKEN_IDENT)
            {
                // 寄存器或标识
                if (CIntelToATT.IsReg(head.TextValue))
                {
                    // 如果是寄存器
                    this.Mode = CIntelToATT.O_REG;
                    this.Reg = head.Value.ToString();
                    this.Size = CIntelToATT.RegSize(this.Reg);
                }
                else
                {
                    this.Mode = CIntelToATT.O_LABEL;
                    this.Label = head.TextValue;
                }
            }
            else if (head.Mode == CIntelToATT.CTOKEN_OPERATOR)
            {
                // 如果是符号
                if (head.TextValue == "[")
                {
                    // 如果是内存
                    this.Mode = CIntelToATT.O_MEM;
                    if (tail.Mode != CIntelToATT.CTOKEN_OPERATOR || tail.TextValue != "]")
                    {
                        throw new Exception("bad memory operand");
                    }
                    this.ParseMemory(tokens);
                }
                else
                {
                    throw new Exception("bad operand descript " + (head.Value));
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
            this.Scale = 1;
            this.Index = "";
            this.Offset = 0;
            this._Base = "";
            var pos = -1;
            foreach (var i in Enumerable.Range(0, tokens.Count))
            {
                token = tokens[i];
                if (token.Mode == CIntelToATT.CTOKEN_OPERATOR && token.Value == ":")
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
                if (t1.Mode != CIntelToATT.CTOKEN_IDENT)
                {
                    throw new Exception("memory operand segment bad");
                }
                var seg = t1.Value.ToString().ToLower();
                if (!segments.Contains(seg))
                {
                    throw new Exception("memory operand segment unknow");
                }
                this.Segment = seg;
            }
            pos = -1;
            for(int i = 0;i<tokens.Count;i++)
            {
                token = tokens[i];
                if (token.Mode == CIntelToATT.CTOKEN_OPERATOR && token.TextValue == "*")
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
                if (t1.Mode == CIntelToATT.CTOKEN_IDENT && t2.Mode == CIntelToATT.CTOKEN_INT)
                {
                }
                else if (t1.Mode == CIntelToATT.CTOKEN_INT && t2.Mode == CIntelToATT.CTOKEN_IDENT)
                {
                    t1 = t2;
                    t2 = t1;
                }
                else
                {
                    throw new Exception("memory operand error (scale error)");
                }
                if (!CIntelToATT.IsReg(t1.TextValue))
                {
                    throw new Exception("memory operand error (no index register)");
                }
                this.Index = t1.TextValue;
                this.Scale = (int)(long)t2.Value;
                if (!CIntelToATT.ValidScales.Contains(this.Scale))
                {
                    throw new Exception("memory operand error (bad scale number)");
                }
            }
            //for token in tokens: print token,
            //print ''
            foreach (var token_ in tokens)
            {
                if (token_.Mode == CIntelToATT.CTOKEN_IDENT && CIntelToATT.IsReg(token_.TextValue))
                {
                    if (this._Base == "")
                    {
                        this._Base = token_.TextValue;
                    }
                    else if (this.Index == "")
                    {
                        this.Index = token_.TextValue;
                    }
                    else
                    {
                        Console.WriteLine(token_);
                        throw new Exception("memory operand error (too many regs)");
                    }
                }
                else if (token_.Mode == CIntelToATT.CTOKEN_INT)
                {
                    if (this.Offset == 0)
                    {
                        this.Offset = (int)(long)token_.Value;
                    }
                    else
                    {
                        throw new Exception("memory operand error (too many offs)");
                    }
                }
                else if (token_.Mode == CIntelToATT.CTOKEN_OPERATOR && token_.TextValue == "+")
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
            if (this.Mode == CIntelToATT.O_REG)
            {
                return string.Format("reg:{0}", this.Reg);
            }
            else if (this.Mode == CIntelToATT.O_IMM)
            {
                return string.Format("imm:{0}", this.Immediate);
            }
            else if (this.Mode == CIntelToATT.O_LABEL)
            {
                return string.Format("label:{0}", this.Label);
            }
            var data = new List<string>();
            if (!string.IsNullOrEmpty(this._Base))
            {
                data.Add(this._Base);
            }
            if (!string.IsNullOrEmpty(this.Index))
            {
                if (this.Scale == 1)
                {
                    data.Add(string.Format("{0}", this.Index));
                }
                else
                {
                    data.Add(string.Format("{0} * {1}", this.Index, this.Scale));
                }
            }
            if (this.Offset != 0)
            {
                data.Add(string.Format("0x{0:X}", this.Offset));
            }
            var size = "";
            if (this.Size == 1)
            {
                size = "8";
            }
            else if (this.Size == 2)
            {
                size = "16";
            }
            else if (this.Size == 4)
            {
                size = "32";
            }
            else if (this.Size == 8)
            {
                size = "64";
            }
            return string.Format("mem{0}:[+{1}]", size, string.Join(',', data));
        }

        public virtual string Translate(bool inline = false)
        {
            var prefix = @"%";
            if (inline)
            {
                prefix = @"%%";
            }
            if (this.Mode == CIntelToATT.O_REG)
            {
                return prefix + this.Reg;
            }
            if (this.Mode == CIntelToATT.O_IMM)
            {
                return "$" + string.Format("{0:X}", (this.Immediate));
            }
            if (this.Mode == CIntelToATT.O_LABEL)
            {
                return this.Label;
            }
            var text = "";
            var @base = String.IsNullOrEmpty(this._Base) ? "" : prefix + this._Base;
            var index = String.IsNullOrEmpty(this.Index) ? "" : prefix + this.Index;
            if (!string.IsNullOrEmpty(this.Index))
            {
                text = string.Format("({0})", @base);
            }
            else
            {
                text = string.Format("({0},{1},{2})", @base, index, this.Scale);
            }
            if (this.Offset > 0)
            {
                text = string.Format("0x{0:X}{1}", this.Offset, text);
            }
            if (!string.IsNullOrEmpty(this.Segment))
            {
                text = string.Format("{0}:{1}", this.Segment, text);
            }
            return text;
        }

        public override string ToString() => this.GetInfo() + " -> " + this.Translate();
    }
    public class CEncoding
    {
        public bool Empty;
        public string Instruction;
        public string Label;
        public string Name;
        public List<COperand> Operands;
        public string Prefix;
        public int Size;
        public List<CToken> Tokens;
        public string Inst;

        public CEncoding(List<CToken> tokens)
        {
            this.Reset();
            this.Parse(tokens);
            this.Name = "cencoding";
        }

        public virtual int Reset()
        {
            this.Label = "";
            this.Prefix = "";
            this.Instruction = "";
            this.Operands = new List<COperand>();
            this.Tokens = null;
            this.Empty = false;
            return 0;
        }

        public virtual int Parse(List<CToken> tokens)
        {
            while (tokens.Count > 0)
            {
                if (!CIntelToATT.EndTokens.Contains(tokens.Last().Mode))
                {
                    break;
                }
                tokens.RemoveAt(0);
            }
            if (tokens.Count == 0)
            {
                this.Empty = true;
                return 0;
            }
            this.Reset();
            this.Tokens = tokens;
            this.ParseLabel();
            this.ParsePrefix();
            this.ParseInstruction();
            this.ParseOperands();
            this.Update();
            this.Tokens = new();
            return 0;
        }

        public virtual int ParseLabel()
        {
            if (this.Tokens.Count < 2) return 0;
            var t1 = Tokens[Tokens.Count - 2];
            var t2 = Tokens[Tokens.Count - 1];
            if (t2.Mode == CIntelToATT.CTOKEN_OPERATOR && t2.TextValue == ":")
            {
                if (t1.Mode != CIntelToATT.CTOKEN_IDENT)
                {
                    throw new Exception("error label type");
                }
                this.Label = t1.TextValue;
                this.Tokens = this.Tokens.ToArray()[..2].ToList();
            }
            return 0;
        }
        protected List<string> prefix = new () {
                "lock",
                "rep",
                "repne",
                "repnz",
                "repe",
                "repz"
            };
        protected List<string> segments = new () {
                "cs",
                "ss",
                "ds",
                "es",
                "fs",
                "gs"
            };

        public virtual int ParsePrefix()
        {
            while (this.Tokens.Count >= 1)
            {
                var t1 = this.Tokens[0];
                if (t1.Mode != CIntelToATT.CTOKEN_IDENT)
                {
                    break;
                }
                var text = t1.TextValue.ToLower();
                if (!prefix.Contains(text) && !segments.Contains(text))
                {
                    break;
                }
                this.Prefix += " " + text;
                this.Prefix = this.Prefix.Trim();
                this.Tokens = this.Tokens.ToArray()[1..].ToList();
            }
            return 0;
        }

        public virtual int ParseInstruction()
        {
            if (this.Tokens.Count < 1) return 0;
            var t1 = this.Tokens[0];
            this.Tokens = this.Tokens.ToArray()[1..].ToList();
            if (t1.Mode != CIntelToATT.CTOKEN_IDENT)
            {
                throw new Exception("instruction type error");
            }
            this.Instruction = t1.TextValue;
            return 0;
        }

        public virtual int ParseOperands()
        {
            var operands = new List<List<CToken>>();
            while (this.Tokens.Count > 0)
            {
                var size = this.Tokens.Count;
                var pos = size;
                for(int i = 0; i < size; i++)
                {
                    if (this.Tokens[i].Mode == CIntelToATT.CTOKEN_OPERATOR)
                    {
                        if (this.Tokens[i].TextValue == ",")
                        {
                            pos = i;
                            break;
                        }
                    }
                }
                operands.Add(this.Tokens.ToArray()[..pos].ToList());
                this.Tokens = this.Tokens.ToArray()[(pos + 1)..].ToList();
            }
            foreach (var token in operands)
            {
                var n = new COperand(token);
                this.Operands.Add(n);
            }
            operands = null;
            return 0;
        }

        public virtual int Update()
        {
            this.Size = 0;
            foreach (var operand in this.Operands)
            {
                if (operand.Size > this.Size)
                {
                    this.Size = operand.Size;
                }
            }
            if (this.Prefix == "" && this.Instruction == "")
            {
                if (this.Operands.Count == 0)
                {
                    this.Empty = true;
                }
            }
            return 0;
        }

        public virtual string TranslateInstruction()
        {
            var instruction = this.Instruction.ToLower();
            if (instruction == "align")
            {
                return ".align";
            }
            if (CIntelToATT.Instreplace.ContainsKey(instruction))
            {
                instruction = CIntelToATT.Instreplace[instruction];
            }
            var postfix = false;
            if (this.Operands.Count == 1)
            {
                var o = this.Operands[0];
                if (o.Mode == CIntelToATT.O_MEM)
                {
                    postfix = true;
                }
                else if (o.Mode == CIntelToATT.O_LABEL)
                {
                    postfix = true;
                }
            }
            else if (this.Operands.Count == 2)
            {
                var o1 = this.Operands[0];
                var o2 = this.Operands[1];

                if (o1.Mode == CIntelToATT.O_IMM && o2.Mode == CIntelToATT.O_MEM)
                {
                    postfix = true;
                }
                if (o1.Mode == CIntelToATT.O_MEM && o2.Mode == CIntelToATT.O_IMM)
                {
                    postfix = true;
                }
                if (o1.Mode == CIntelToATT.O_IMM && o2.Mode == CIntelToATT.O_LABEL)
                {
                    postfix = true;
                }
                if (o1.Mode == CIntelToATT.O_LABEL && o2.Mode == CIntelToATT.O_IMM)
                {
                    postfix = true;
                }
            }
            if (postfix)
            {
                if (this.Size == 1)
                {
                    instruction += "b";
                }
                else if (this.Size == 2)
                {
                    instruction += "w";
                }
                else if (this.Size == 4)
                {
                    instruction += "l";
                }
                else if (this.Size == 8)
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

        public virtual string TranslateOperand(int id, bool inline = false)
        {
            if (this.Instruction.ToLower() == "align")
            {
                var size = 4;
                if (this.Operands.Count > 0)
                {
                    var op = this.Operands[0];
                    if (op.Mode == CIntelToATT.O_IMM)
                    {
                        size = (int)op.Immediate;
                    }
                }
                if (id == 0)
                {
                    return string.Format("{0}, 0x90", size);
                }
                return "";
            }
            if (id < 0 || id >= this.Operands.Count)
            {
                throw new Exception("operand id out of range");
            }
            var text = this.Operands[id].Translate(inline);
            return text;
        }

        public override string ToString()
        {
            var text = "";
            if (!string.IsNullOrEmpty(this.Label))
            {
                text += string.Format("{0}: ", this.Label);
            }
            if (!string.IsNullOrEmpty(this.Prefix))
            {
                text += string.Format("{0} ", this.Prefix);
            }
            text += this.TranslateInstruction();
            text += " ";
            text += this.TranslateOperand(0);
            return text;
        }
    }
    public class CSynthesis
    {

        public bool IsAMd64;

        public List<CEncoding> Encodings;

        public string Error;

        public int Indent1;

        public int Indent2;

        public Dictionary<string, (int, int)> Labels;

        public List<List<CToken>> Lines;

        public Dictionary<string, (int, bool)> Maps;

        public Dictionary<int, string> Memos;

        public string Name;

        public Dictionary<string, List<(int, int)>> References;

        public Dictionary<string, bool> Registers;

        public int Size;

        public string Source;

        public List<(string,int,int,int)> Table;

        public List<(int, string, bool)> Variables;

        public Dictionary<string, List<(string, int, int, int)>> Vars;

        public CSynthesis()
        {
            this.Reset();
            this.Name = "csynthesis";
        }

        public virtual void Reset()
        {
            this.Source = "";
            this.Encodings = new();
            this.Labels = new();
            this.References = new();
            this.Lines = new();
            this.Memos = new();
            this.Table = new();
            this.Vars = new();
            this.Maps = new();
            this.Variables = new();
            this.Registers = new();
            this.IsAMd64 = false;
            this.Size = 0;
            this.Error = "";
        }

        public virtual int Parse(TextReader source)
        {
            this.Reset();
            if (this.DoTokenize(source) != 0)
                return -1;
            if (this.DoEncoding() != 0)
                return -2;
            if (this.DoAnalyse() != 0)
                return -3;
            return 0;
        }

        public virtual int DoTokenize(TextReader source)
        {
            var scanner = new CScanner(source, new());
            var tokens = new List<CToken>();
            while (true)
            {
                var token = scanner.Next();
                if (token == null)
                {
                    var text = string.Format("{0}: {1}", scanner.Row, scanner.Error);
                    this.Error = text;
                    return -1;
                }
                tokens.Add(token);
                if (token.Mode == CIntelToATT.CTOKEN_ENDF)
                {
                    break;
                }
            }
            while (tokens.Count > 0)
            {
                var size = tokens.Count;
                var pos = size - 1;
                for(int i = 0; i < size; i++)
                {
                    if (tokens[i].Mode == CIntelToATT.CTOKEN_ENDL)
                    {
                        pos = i;
                        break;
                    }
                }
                this.Lines.Add(tokens.ToArray()[..(pos + 1)].ToList());
                tokens = tokens.ToArray()[(pos + 1)..].ToList();
            }
            for(int i = 0; i < Lines.Count; i++)
            {
                var lineno = i + 1;
                if (scanner.Comments.ContainsKey(lineno))
                {
                    this.Memos[i] = scanner.Comments[lineno].Trim(CIntelToATT.SpaceChars);
                }
                else
                {
                    this.Memos[i] = string.Empty;
                }
            }
            scanner = null;
            return 0;
        }

        public virtual int DoEncoding()
        {
            var lineno = 1;
            foreach (var tokens in this.Lines)
            {
                try
                {
                    this.Encodings.Add(new (tokens));
                }
                catch (Exception e)
                {
                    var text = string.Format("{0}: {1}", lineno, e);
                    this.Error = text;
                    return -1;
                }
                lineno += 1;
            }
            if (this.Lines.Count != this.Encodings.Count)
            {
                throw new Exception("core fault");
            }
            return 0;
        }

        protected static readonly List<string> amd64 = new () 
                { "rax", "rbx", "rcx", "rdx", "rdi", "rsi", "rbp", "rsp" };

        public virtual int DoAnalyse()
        {
            CEncoding encoding;
            this.Size = this.Lines.Count;
            var index = 0;
            for(int i = 0;i<this.Size;i++)
            {
                encoding = this.Encodings[i];
                if (encoding.Label != null)
                {
                    index += 1;
                    this.Labels[encoding.Label] = (i, index);
                }
            }
            var varlist = new List<(string, int, int)>();
            for (int i = 0; i < this.Size; i++)
            {
                encoding = this.Encodings[i];
                for(int j = 0;j< encoding.Operands.Count;j++)
                {
                    var operand = encoding.Operands[j];
                    if (operand.Mode == CIntelToATT.O_LABEL)
                    {
                        if (!this.Labels.ContainsKey(operand.Label))
                        {
                            varlist.Add((operand.Label, i, j));
                        }
                        else
                        {
                            if (!this.References.TryGetValue(operand.Label, out var desc))
                            {
                                this.References[operand.Label] = desc = new List<(int, int)>();
                            }

                            desc.Add((i, j));
                        }
                    }
                    else if (operand.Mode == CIntelToATT.O_REG)
                    {
                        var reg = operand.Reg.ToLower();
                        this.Registers[reg] = true;
                        if (amd64.Contains(reg))
                        {
                            this.IsAMd64 = true;
                        }
                    }
                }
            }
            var vartable = new List<(string, int, int)>();
            foreach (var (var, line, pos) in varlist)
            {
                if (pos == 0)
                    vartable.Add((var, line, pos));
            }
            foreach (var (var, line, pos) in varlist)
            {
                if (pos != 0)
                    vartable.Add((var, line, pos));
            }
            var names = new Dictionary<string, int>();

            for(int i = 0;i<vartable.Count;i++)
            {
                var (var, line, pos) = vartable[i];
                if (!this.Vars.TryGetValue(var, out var desc))
                {
                    this.Vars[var]= desc = new();                
                }

                if (desc.Count == 0)
                {
                    index = names.Count;
                    names[var] = index;
                    this.Table.Add((var, line, pos, index));
                    var writable = pos == 0;
                    this.Maps[var] = (index, writable);
                    this.Variables.Add((index, var, writable));
                }
                else
                {
                    index = names[var];
                }
                desc.Add((var, line, pos, index));
                this.Vars[var] = desc;
            }
            var indent1 = 0;
            var indent2 = 0;
            for (int i = 0; i < this.Size; i++)
            {
                encoding = this.Encodings[i];
                encoding.Inst = encoding.TranslateInstruction();
                if (!string.IsNullOrEmpty(encoding.Label) && !encoding.Empty)
                {
                    if (encoding.Label.Length > indent1)
                    {
                        indent1 = encoding.Label.Length;
                    }
                }
                if (encoding.Inst.Length > indent2)
                {
                    indent2 = encoding.Inst.Length;
                }
            }
            this.Indent1 = indent1 + 2;
            this.Indent2 = indent2;
            if (this.Indent1 < 4)
            {
                this.Indent1 = 4;
            }
            if (this.Indent2 < 4)
            {
                this.Indent2 = 4;
            }
            return 0;
        }

        public virtual string GetLabel(int lineno, bool clabel = false)
        {
            if (lineno < 0 || lineno >= this.Size)
            {
                throw new Exception("line number out of range");
            }
            var encoding = this.Encodings[lineno];
            if (encoding.Label == "")
            {
                return "";
            }
            if (!clabel)
            {
                return encoding.Label + ":";
            }
            var (line, index) = this.Labels[encoding.Label];
            return string.Format("{0}:", index);
        }

        public virtual string GetInstruction(int lineno)
        {
            if (lineno < 0 || lineno >= this.Size)
            {
                throw new Exception("line number out of range");
            }
            var encoding = this.Encodings[lineno];
            if (encoding.Empty)
            {
                return string.Empty;
            }
            var source = encoding.Prefix + " " + encoding.Inst;
            source = source.Trim();
            return source;
        }

        public virtual string GetOperand(int lineno, int id, bool clabel = false, bool inline = false)
        {
            if (lineno < 0 || lineno >= this.Size)
            {
                throw new Exception("line number out of range");
            }
            var encoding = this.Encodings[lineno];
            if (id < 0 || id >= encoding.Operands.Count)
            {
                throw new Exception("operand id out of range");
            }
            var operand = encoding.Operands[id];
            if (new int[] { CIntelToATT.O_IMM, CIntelToATT.O_REG, CIntelToATT.O_MEM }.Contains(operand.Mode))
            {
                return operand.Translate(inline);
            }
            var label = operand.Label;
            if (this.Labels.ContainsKey(label))
            {
                // this is a jmp label
                if (!clabel) return label;
                var (line, index) = this.Labels[label];
                if (line <= lineno)
                {
                    return string.Format("{0}b", index);
                }
                return string.Format("{0}f", index);
            }
            if (this.Vars.ContainsKey(label))
            {
                // this is a variable
                var (idx, writable) = this.Maps[label];
                return string.Format("%%{0}", idx);
            }
            return "$" + label;
        }

        public virtual string Synth(int lineno, bool clabel = true, bool inline = true, bool align = true)
        {
            COperand op;
            List<string> operands;
            if (lineno < 0 || lineno >= this.Size)
            {
                throw new Exception("line number out of range");
            }
            var encoding = this.Encodings[lineno];
            var source = this.GetLabel(lineno, clabel);
            // 内容缩进
            var indent = this.Indent1;
            if (clabel)
            {
                indent = this.Labels.Count + 2;
            }
            indent = Convert.ToInt32((indent + 1) / 2 * 2);
            source = source.PadLeft(indent);
            // 没有指令
            if (encoding.Empty)
                return source;
            var instruction = this.GetInstruction(lineno);
            if (align)
            {
                indent = Convert.ToInt32((this.Indent2 + 3) / 4 * 4);
                instruction = instruction.PadLeft(indent);
            }
            source += instruction + " ";
            if (encoding.Instruction.ToLower() == "align")
            {
                var size = 4;
                if (encoding.Operands.Count > 0)
                {
                    var operand = encoding.Operands[0];
                    if (operand.Mode == CIntelToATT.O_IMM)
                        size = (int)operand.Immediate;
                }
                source += string.Format("{0}, 0x90", size);
            }
            else if (new string[] { ".byte", ".int", ".short" }.Contains(encoding.Inst.ToLower()))
            {
                operands = new List<string>();
                for(int i = 0;i< encoding.Operands.Count;i++)
                {
                    op = encoding.Operands[i];
                    var text = string.Format("0x{0:X}", (op.Immediate));
                    operands.Add(text);
                }
                source += string.Join(',', operands);
            }
            else
            {
                operands = new List<string>();
                for(int i= 0;i<operands.Count;i++)
                {
                    var opx = this.GetOperand(lineno, i, clabel, inline);
                    operands.Add(opx);
                }
                operands.Reverse();
                source += string.Join(',', operands);
            }
            source = source.TrimEnd(' ');
            return source;
        }

        public virtual string GetVars(int mode = 0)
        {
            var vars = new List<string>();
            foreach (var (id, name, writable) in this.Variables)
            {
                if (mode == 0 && writable)
                {
                    vars.Add(string.Format("\"=m\"({0})", name));
                }
                else if (mode == 1 && !writable)
                {
                    vars.Add(string.Format("\"m\"({0})", name));
                }
            }
            var text = string.Join(',', vars);
            return text;
        }

        public virtual string GetRegs() 
            => this.IsAMd64 
            ? "\"rsi\", \"rdi\", \"rax\", \"rbx\", \"rcx\", \"rdx\"" 
            : "\"esi\", \"edi\", \"eax\", \"ebx\", \"ecx\", \"edx\""
            ;
    }
    public class CIntelToATT
    {
        public const int CTOKEN_ENDL = 0;
        public const int CTOKEN_ENDF = 1;
        public const int CTOKEN_IDENT = 2;
        public const int CTOKEN_KEYWORD = 3;
        public const int CTOKEN_STR = 4;
        public const int CTOKEN_OPERATOR = 5;
        public const int CTOKEN_INT = 6;
        public const int CTOKEN_FLOAT = 7;
        public const int CTOKEN_ERROR = 8;

        public static Dictionary<int, string> CTOKEN_NAME = new() {
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
        static CIntelToATT()
        {
            foreach (var reg in REGNAMES)
                REGSIZE[reg] = RegInfo(reg);
        }
        public static Func<string, int> RegSize = reg => REGSIZE[reg.ToUpper()];

        public static Func<string, bool> IsReg = reg => REGSIZE.ContainsKey(reg.ToUpper());

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
        public static readonly char[] SpaceChars = new[] { ' ', '\r', '\n', '\t' };
        public static readonly int[] CommentChars = new int[] { (int)';', '#' };
        public static readonly int[] QuoteChars = new int[] { '\'', '\"' };
        public static readonly int[] StartChars = new int[] { '_', '$', '@' };
        public static readonly int[] StringChars = new int[] { '\n', '\'' };
        public static readonly int[] EndTokens = new int[] { CIntelToATT.CTOKEN_ENDF, CIntelToATT.CTOKEN_ENDL };
        public static readonly string[] HexHeaders = new string[] { "0x", "0X" };
        public static int RegInfo(string name)
        {
            name = name.ToLower();
            if (name.Length>=2 && name[..2] == "mm")
            {
                return 8;
            }
            if (name.Length >= 3 && name[..3] == "xmm")
            {
                return 16;
            }
            if (name.Length >= 1 && name[..1] == "r" && char.IsDigit(name[1]))
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

        public static List<CToken> Tokenize(TextReader script)
        {
            var scanner = new CScanner(script, new());
            var result = scanner.GetTokens();
            scanner.Reset();
            return result;
        }

        public Dictionary<string, bool> Config;
        public string Error;
        public List<string> Lines;
        public int Maxsize;
        public Dictionary<int, string> Memos;
        public List<string> Output;
        public CSynthesis Synthesis;
        public CIntelToATT()
        {
            this.Synthesis = new();
            this.Config = new();
            this.Config["align"] = false;
            this.Config["inline"] = false;
            this.Config["clabel"] = false;
            this.Config["memo"] = false;
            this.Error = "";
            this.Lines = new();
            this.Output = new();
            this.Memos = new();
            this.SetOptions();
        }

        public virtual int SetOptions(bool align = true, bool inline = true, bool clabel = true, bool memo = true)
        {
            this.Config["align"] = align;
            this.Config["clabel"] = clabel;
            this.Config["inline"] = inline;
            this.Config["memo"] = memo;
            return 0;
        }

        public virtual int Parse(TextReader source, bool clabel =true, bool inline=false, bool align =true)
        {
            this.Lines = new();
            this.Output = new();
            this.Memos = new();
            var retval = this.Synthesis.Parse(source);
            this.Error = this.Synthesis.Error;
            if (retval != 0)
            {
                return retval;
            }
            for(int i = 0;i<this.Synthesis.Size;i++)
            {
                var text = this.Synthesis.Synth(i, clabel, inline, align);
                this.Lines.Add(text);
                var memo = this.Synthesis.Memos[i].Trim(CIntelToATT.SpaceChars);
                if (memo[..1] == ";")
                {
                    memo = memo[1].ToString();
                }
                memo = memo.Trim();
                if (memo[..2] != "//" && memo != "")
                {
                    memo = "//" + memo;
                }
                this.Memos[i] = memo;
            }
            this.Maxsize = 0;
            foreach (var text in this.Lines)
            {
                if (text.Length > this.Maxsize)
                {
                    this.Maxsize = text.Length;
                }
            }
            this.Maxsize = (this.Maxsize + 6) / 2 * 2;
            return 0;
        }

        public virtual (List<string>, int) Convert(TextReader source)
        {
            this.Lines = new();
            this.Output = new();
            var clabel = this.Config["clabel"];
            var inline = this.Config["inline"];
            var align = this.Config["align"];
            var memo = this.Config["memo"];

            var retval = this.Parse(source, clabel, inline, align);
            var prefix = "";
            if (retval != 0)
            {
                return (this.Output,retval);
            }
            if (inline)
            {
                this.Output.Add("__asm__ __volatile__ (");
                prefix = "  ";
            }
            for(int i = 0; i < this.Synthesis.Size; i++)
            {
                var line = this.Lines[i];
                if (line.Trim(new[] { '\r', '\t', '\n', ' ' }) == "")
                {
                    if (this.Memos[i] == "" || !memo )
                    {
                        if (inline)
                        {
                            this.Output.Add(prefix + "");
                        }
                        else
                        {
                            this.Output.Add("");
                        }
                    }
                    else
                    {
                        this.Output.Add(prefix + this.Memos[i]);
                    }
                }
                else
                {
                    if (inline)
                    {
                        line = "\"" + line + "\\n\"";
                    }
                    if (!string.IsNullOrEmpty(this.Memos[i]) && (memo))
                    {
                        line = line.PadLeft(this.Maxsize)+ this.Memos[i];
                    }
                    this.Output.Add(prefix + line);
                }
            }
            if (inline)
            {
                this.Output.Add("  :" + this.Synthesis.GetVars(0));
                this.Output.Add("  :" + this.Synthesis.GetVars(1));
                this.Output.Add("  :\"memory\", " + this.Synthesis.GetRegs());
                this.Output.Add(");");
            }
            return (this.Output, 0);
        }
    }
}
