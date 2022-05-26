using System.Text;

namespace AsmConverter
{
    public class Program
    {
        public enum OperandSize : uint
        {
            Unknown = 0,
            Byte = 1,
            Word = 2,
            Long = 3,
            Quad = 4,
        }

        public enum Bits : uint
        {
            Unknown = 0,
            Bits16 = 1,
            Bits32 = 2,
            Bits64 = 3,
        }
        public static string FixRegister(string s)
        {
            if(s == "sil")
            {
                return "si";
            }else if(s == "dil")
            {
                return "di";
            }
            return s;
        }
        public enum ConvertDirection : uint
        {
            Undefined = 0,
            ATT_To_Intel = 1,
            Intel_To_ATT = 2,
        }
        public static Bits DefaultBits = Bits.Bits64;
        public static int IndexOfWhitespace(string s)
        {
            int ri = -1;
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsWhiteSpace(s[i]))
                {
                    ri = i;
                    break;
                }
            }
            return ri;
        }
        public static (List<string> Parts,bool Enclosed) Split(string s, char c = ',', char open = '(', char close=')', bool keep_bound =true)
        {
            var enclosed = false;
            var rets = new List<string>();
            var st = new Stack<int>();
            var sb = new StringBuilder();
            if (enclosed = s.StartsWith(open) && s.EndsWith(close)) s = s[1..^1];
            for (int i= 0; i < s.Length; i++)
            {
                if (s[i] == open)
                {
                    enclosed = true;
                    st.Push(i);
                    if (keep_bound)
                        sb.Append(s[i]);
                }else if (s[i]==close && st.Count > 0)
                {
                    st.Pop();
                    if (keep_bound)
                        sb.Append(s[i]);
                }
                else if(st.Count == 0 && s[i] == c)
                {
                    rets.Add(sb.ToString().Trim());
                    sb.Clear();
                }
                else
                {
                    sb.Append(s[i]);
                }
            }
            var sm = sb.ToString().Trim();
            if (sm.Length > 0)
            {
                rets.Add(sm);
            }

            return (rets,enclosed);
        }
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
        public static void ConvertFromATT_ToIntel(FileInfo fi, FileInfo fo)
        {
            var lineno = 0;
            using var InputReader = new StreamReader(fi.FullName);  
            using var OutputWriter = new StreamWriter(fo.FullName);
            var indef = false;
            var outdef = false;
            var lastpre = "";
            var align = 1;
            while(InputReader.ReadLine() is string line)
            {
            do_line:
                lineno++;
                if (lineno == 1)
                {
                    switch (DefaultBits)
                    {
                        case Bits.Bits16:
                            OutputWriter.WriteLine("[Bits 16]");
                            continue;
                        case Bits.Bits32:
                            OutputWriter.WriteLine("[Bits 32]");
                            continue;
                        case Bits.Bits64:
                            OutputWriter.WriteLine("[Bits 64]");
                            continue;
                    }
                }
                var lineEnding = "";
                var sp = line.IndexOfAny(new [] { '#', ';' });
                if (sp >= 0)
                {
                    lineEnding = line[sp..];
                    line = line[..sp];
                    if (lineEnding.StartsWith('#'))
                    {
                        lineEnding = ';' + lineEnding;
                    }
                }
                var result = line.Trim();
                
                var pre = line.IndexOf(result) is int ep && ep >=0 ? line[..ep] : "";
                if (result.Length == 0)
                {
                    OutputWriter.WriteLine(line + lineEnding);
                    continue;
                }
                else if(result.IndexOf(':') is int p && p>=0)
                {
                    if(p == result.Length - 1)
                    {
                        //skip all
                    }
                    else
                    {
                        OutputWriter.WriteLine(result[..(p + 1)] + lineEnding);
                        line = result[(p + 1)..];
                        //if(pre.Length == 0 && lastpre.Length > 0)
                        //{
                        //    line = lastpre + line;
                        //}
                        goto do_line;
                    }
                }
                else
                {
                    if (result.StartsWith('.'))
                    {
                        var resultLower = result.ToLower();

                        if (!resultLower.StartsWith(".endef") && indef)
                        {
                            line = result.Trim();
                        }
                        else if (resultLower.StartsWith(".def")&& char.IsWhiteSpace(resultLower,".def".Length))
                        {
                            indef = true;
                            outdef = false;
                            line = result.Trim();
                        }
                        else if (resultLower.StartsWith(".endef"))
                        {
                            outdef = true;
                            line = result.Trim();
                        }
                        else if (resultLower.StartsWith(".global") 
                            || resultLower.StartsWith(".globl"))
                        {
                            int rp = IndexOfWhitespace(result);
                            if (rp >= 0)
                            {
                                line =  "global " + result[rp..].Trim();
                            }
                        }
                        else if (resultLower.StartsWith(".extern"))
                        {
                            int rp = IndexOfWhitespace(result);
                            if (rp >= 0)
                            {
                                line = "extern " + result[rp..].Trim();
                            }
                        }
                        else if (resultLower.StartsWith(".byte"))
                        {
                            line = "db " + result[(".byte".Length + 1)..];
                        }
                        else if (resultLower.StartsWith(".long"))
                        {
                            line = "dd " + result[(".long".Length + 1)..];
                        }
                        else if (resultLower.StartsWith(".value"))
                        {
                            switch (align)
                            {
                                case 1:
                                    line = "db " + result[(".value".Length + 1)..];
                                    break;
                                case 2:
                                    line = "dw " + result[(".value".Length + 1)..];
                                    break;
                                case 4:
                                    line = "dd " + result[(".value".Length + 1)..];
                                    break;
                                case 8:
                                    line = "dq " + result[(".value".Length + 1)..];
                                    break;
                            }
                        }

                        else if (resultLower == ".text")
                        {
                            line = "section .text";
                        }
                        else if(resultLower.StartsWith(".section") && char.IsWhiteSpace(resultLower,".section".Length))
                        {
                            var ps = resultLower[(".section".Length + 1)..].Trim();
                            var ts = Split(ps);
                            if (ts.Parts.Count >= 1)
                            {
                                line = "section " + ts.Parts[0];
                            }
                        }
                        //align
                        else if (resultLower.StartsWith(".align") && char.IsWhiteSpace(resultLower, ".align".Length))
                        {
                            var ps = resultLower[(".align".Length + 1)..].Trim();
                            var ts = Split(ps);
                            if (ts.Parts.Count >= 1)
                            {
                                if (ts.Parts.Count >= 2)
                                {
                                    line = "align " + ts.Parts[0] + $", db {ts.Parts[1]}";
                                }
                                else
                                {
                                    line = "align " + ts.Parts[0];
                                }
                                if (int.TryParse(ts.Parts[0],out var _align))
                                {
                                    align = _align;
                                }
                            }
                        }
                        //psedo
                        //copy for now
                    }
                    else //try instructions
                    {
                        var far = false;
                        var star = false;
                        var instr = "";
                        var operands = "";
                        var i = IndexOfWhitespace(result);
                        if (i < 0)
                        {
                            instr = result; //whole instruction
                        }
                        else
                        {
                            instr = result[..i]; //partial
                            operands = result[(i + 1)..];
                        }
                        var instrLower = instr.ToLower();
                        var operandSize = OperandSize.Byte;

                        if (instrLower == "ljmp"|| instrLower == "lcall" || instrLower=="lret")
                        {
                            instrLower = instrLower[1..];
                            instr = instr[1..];
                            far = true;
                            if (instrLower != "lret") star = true;
                        }
                        if(instrLower.StartsWith("j") || instrLower == "call")
                        {
                            if (operands.StartsWith('*'))
                            {
                                operands = operands[1..];
                            }
                        }

                        if (instrLower.EndsWith('b')|| instrLower.EndsWith('w')|| instrLower.EndsWith('l')|| instrLower.EndsWith('q'))
                        {
                            if (Instructions.NasmInstructionNames.Contains(instrLower[..^1]))
                            {
                                instrLower = instrLower[..^1];
                                operandSize = instrLower[^1] switch
                                {
                                    'b' => OperandSize.Byte,
                                    'w' => OperandSize.Word,
                                    'l' => OperandSize.Long,
                                    'q' => OperandSize.Quad,
                                    _ =>OperandSize.Unknown
                                };
                                instr = instr[..^1];
                            }
                        }    

                        if(Instructions.NasmInstructionNames.Contains(instrLower))
                        {
                            var operandsParts = Split(operands, ',');
                            var operandsTransformed = new List<string>();

                            foreach(var operand in operandsParts.Parts)
                            {
                                var operandTrimmed = operand.Trim();
                                if (operandTrimmed.StartsWith('$'))
                                {                                    
                                    operandsTransformed.Add(operandTrimmed[1..].Trim());
                                }
                                else if (operandTrimmed.StartsWith("%") && 
                                    (operandTrimmed[1..].Trim().ToLower() is string rgt) 
                                    && Instructions.NasmRegisters.Contains(rgt))
                                {
                                    operandsTransformed.Add(rgt);
                                }
                                else //not register
                                {
                                    var sectionRegister = "";
                                    var operandTrimmeds = operandTrimmed;
                                    int colonIndex = operandTrimmed.IndexOf(':');
                                    if (colonIndex >= 0)
                                    {
                                        var sr = operandTrimmed[..colonIndex];
                                        if (sr.StartsWith("%") &&
                                            (sr[1..] is String srt)
                                            && Instructions.NasmRegisters.Contains(srt))
                                        {
                                            srt = FixRegister(srt);
                                            sectionRegister = srt + ":";
                                        }
                                        operandTrimmeds = operandTrimmed[(colonIndex + 1)..];
                                    }
                                    var disp = "";
                                    int po = operandTrimmeds.IndexOf('(');
                                    if(po>=0 && operandTrimmeds.EndsWith(')'))
                                    {
                                        disp = operandTrimmeds[..po];
                                        operandTrimmeds = operandTrimmeds[po..];
                                    }
                                    //opm = base, index, scale
                                    var operandsList = new List<string>();
                                    var operandsSplitted = Split(operandTrimmeds, keep_bound: false);
                                    foreach(var operandPart in operandsSplitted.Parts)
                                    {
                                        var operandPartWorking = operandPart;
                                        if(operandPartWorking.StartsWith("%") && 
                                            operandPartWorking[1..] is string operandPartTrimmed
                                            && Instructions.NasmRegisters.Contains(operandPartTrimmed))
                                        {
                                            operandPartTrimmed = FixRegister(operandPartTrimmed);
                                            operandsList.Add(operandPartTrimmed);
                                        }else if(operandPartWorking.StartsWith("$")&&
                                            (operandPartWorking[1..] is string immediateNumberTrimmed))
                                        {
                                            operandsList.Add(immediateNumberTrimmed);
                                        }
                                        else
                                        {
                                            operandsList.Add(operandPart);
                                        }
                                    }
                                    //section:disp(base, index, scale) 
                                    //[section:base + index*scale + disp] 

                                    if (operandsList.Count == 1)
                                    {
                                        operandTrimmeds = operandsList[0];
                                    }
                                    else if (operandsList.Count == 2)
                                    {
                                        operandTrimmeds = operandsList[0];
                                        operandTrimmeds += operandTrimmeds.Length == 0 ? String.Empty : " + ";
                                        operandTrimmeds += operandsList[1];
                                    }
                                    else if (operandsList.Count == 3)
                                    {
                                        operandTrimmeds = operandsList[0];
                                        operandTrimmeds += operandTrimmeds.Length == 0 ? String.Empty : " + ";
                                        operandTrimmeds += operandsList[1] + " * " + operandsList[2];
                                    }
                                    if (disp.Length > 0)
                                    {
                                        operandTrimmeds += disp.StartsWith("-") ? (" - "+ disp[1..]) : (" + " + disp);
                                    }
                                    if (operandsSplitted.Enclosed 
                                        || operandsSplitted.Parts.Count>1)
                                    {
                                        operandTrimmed =
                                           "[" + sectionRegister + operandTrimmeds + "]";
                                    }
                                    else
                                    {
                                        operandTrimmed = operandTrimmeds;
                                    }

                                    operandsTransformed.Add(operandTrimmed);
                                }
                            }

                            if (far)
                            {
                                if(operandsTransformed.Count == 1)
                                {
                                    line = instr + " far " + operandsTransformed[0];
                                }
                                else if(operandsTransformed.Count == 2)
                                {
                                    line = instr + " far " + operandsTransformed[0] + ":" + operandsTransformed[1];
                                }
                            }
                            else
                            {
                                operandsTransformed.Reverse();
                                line = instr;
                                if (operandsTransformed.Count > 0)
                                {
                                    line += "\t" + operandsTransformed.Aggregate((a, b) => a + ", " + b);
                                }
                            }
                        }
                        else //this is bad instruction!
                        {
                            line = ";BAD " + line;
                        }
                    }
                }

                OutputWriter.WriteLine((indef ? pre + ";" : pre) + line + lineEnding);
                if (outdef) outdef = indef = false;
                lastpre = pre;
            }
        }

        public static int Main(string[] args)
        {
            var cd = ConvertDirection.Undefined;
            var target = ".";
            var inputs = new List<FileInfo>();
            var outext = "";
            var dirmode = false;
            if (args.Length == 0)
            {
                Console.Write("AsmConverter [-t ati/ita] [Source] [Target]");
            }
            else if(args.Length>2 && args[0]=="-t")
            {
                switch (args[1])
                {
                    case "ita":
                        cd = ConvertDirection.Intel_To_ATT;
                        outext = ".s";
                        break;
                    case "ati":
                        cd = ConvertDirection.ATT_To_Intel;
                        outext = ".asm";
                        break;
                    default:
                        cd = ConvertDirection.Undefined;
                        break;
                }
                if (cd != ConvertDirection.Undefined)
                {
                    if (dirmode = Directory.Exists(args[2]))
                    {
                        var pattern = cd == ConvertDirection.ATT_To_Intel ? "*.s" : "*.asm";

                        var fs = (new DirectoryInfo(args[2]).EnumerateFiles(
                            pattern,
                            SearchOption.AllDirectories));
                        inputs.AddRange(fs);
                    }
                    else if (File.Exists(args[2]))
                    {
                        inputs.Add(new FileInfo(args[2]));
                    }
                    if(args.Length >= 3)
                    {
                        if (Directory.Exists(args[2]) || dirmode)
                        {
                            target = new DirectoryInfo(args[2]).FullName; //this is dir
                        }
                        else //dirmode
                        {
                            target = new FileInfo(args[2]).FullName;
                        }
                    }
                }
            }
            if(cd!= ConvertDirection.Undefined)
            {
                if (dirmode)
                {
                    foreach (var input in inputs)
                    {
                        if(cd == ConvertDirection.ATT_To_Intel)
                        {
                            ConvertFromATT_ToIntel(input, new(Path.Combine(target, input.Name + outext)));
                        }
                        else
                        {
                            ConvertFromIntel_ToATT(input, new(Path.Combine(target, input.Name + outext)));
                        }
                    }
                }
                else if(inputs.Count>=1)
                {
                    if (cd == ConvertDirection.ATT_To_Intel)
                    {
                        ConvertFromATT_ToIntel(inputs[0], new(Path.Combine(target, inputs[0].Name + outext)));
                    }
                    else
                    {
                        ConvertFromIntel_ToATT(inputs[0], new(Path.Combine(target, inputs[0].Name + outext)));
                    }
                }
            }

            return 0;
        }
    }
}
