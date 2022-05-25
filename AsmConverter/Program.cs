// See https://aka.ms/new-console-template for more information

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
        public enum ConvertDirection : uint
        {
            Undefined = 0,
            ATT_To_Intel = 1,
            Intel_To_ATT = 2,
        }
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
            //TODO:
        }
        public static void ConvertFromATT_ToIntel(FileInfo fi, FileInfo fo)
        {
            var lineno = 1;
            using var InputReader = new StreamReader(fi.FullName);  
            using var OutputWriter = new StreamWriter(fo.FullName);
            while(InputReader.ReadLine() is string line)
            {
            do_line:
                lineno++;
                var el = "";
                var sp = line.IndexOfAny(new [] { '#', ';' });
                if (sp >= 0)
                {
                    el = line[sp..];
                    line = line[..sp];
                    if (el.StartsWith('#'))
                    {
                        el = ';' + el;
                    }
                }
                var result = line.Trim();
                
                var pre = line.IndexOf(result) is int ep && ep >=0 ? line[..ep] : "";
                if (result.Length == 0)
                {
                    OutputWriter.WriteLine(line + el);
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
                        OutputWriter.WriteLine(result[..(p + 1)] + el);
                        line = result[(p + 1)..];
                        goto do_line;
                    }
                }
                else
                {
                    if (result.StartsWith('.'))
                    {
                        //psedo
                        //copy for now
                    }
                    else //try instructions
                    {
                        var far = false;
                        var star = false;
                        var ins = "";
                        var ops = "";
                        var i = IndexOfWhitespace(result);
                        if (i < 0)
                        {
                            ins = result; //whole instruction
                        }
                        else
                        {
                            ins = result[..i]; //partial
                            ops = result[(i + 1)..];
                        }
                        var inslower = ins.ToLower();
                        var oprs = OperandSize.Byte;

                        if (inslower == "ljmp"|| inslower == "lcall" || inslower=="lret")
                        {
                            inslower = inslower[1..];
                            ins = ins[1..];
                            far = true;
                            if (inslower != "lret") star = true;
                        }

                        if (inslower.EndsWith('b')|| inslower.EndsWith('w')|| inslower.EndsWith('l')|| inslower.EndsWith('q'))
                        {
                            if (Instructions.NasmInstructionNames.Contains(inslower[..^1]))
                            {
                                inslower = inslower[..^1];
                                oprs = inslower[^1] switch
                                {
                                    'b' => OperandSize.Byte,
                                    'w' => OperandSize.Word,
                                    'l' => OperandSize.Long,
                                    'q' => OperandSize.Quad,
                                    _ =>OperandSize.Unknown
                                };
                                ins = ins[..^1];
                            }
                        }    

                        //try opcode
                        if(Instructions.NasmInstructionNames.Contains(inslower))
                        {
                            var pts = Split(ops, ',');
                            var rts = new List<string>();

                            foreach(var pt in pts.Parts)
                            {
                                var rpt = pt.Trim();
                                if (rpt.StartsWith('$'))
                                {                                    
                                    rts.Add(rpt[1..].Trim());
                                }
                                else if (rpt.StartsWith("%") && 
                                    (rpt[1..].Trim().ToLower() is string rgt) 
                                    && Instructions.NasmRegisters.Contains(rgt))
                                {
                                    rts.Add(rgt);
                                }
                                else //not register
                                {
                                    var seg = "";
                                    var opm = rpt;
                                    int pi = rpt.IndexOf(':');
                                    if (pi >= 0)
                                    {
                                        seg = rpt[..pi];
                                        if (seg.StartsWith("%") &&
                                            (seg[1..] is String stg)
                                            && Instructions.NasmRegisters.Contains(stg))
                                        {
                                            seg = stg;
                                        }
                                        opm = rpt[(pi + 1)..];
                                    }
                                    var disp = "";
                                    int po = opm.IndexOf('(');
                                    if(po>=0 && opm.EndsWith(')'))
                                    {
                                        disp = opm[..po];
                                        opm = opm[po..];
                                    }
                                    //opm = base, index, scale
                                    var opp = new List<string>();
                                    var opn = Split(opm, keep_bound: false);
                                    foreach(var opu in opn.Parts)
                                    {
                                        var opw = opu;
                                        if(opw.StartsWith("%") && 
                                            opw.Substring(1) is string opa
                                            && Instructions.NasmRegisters.Contains(opa))
                                        {
                                            opp.Add(opa);
                                        }else if(opw.StartsWith("$")&&
                                            (opw.Substring(1) is string opb))
                                        {
                                            opp.Add(opb);
                                        }
                                        else
                                        {
                                            opp.Add(opu);
                                        }
                                    }
                                    //section:disp(base, index, scale) 
                                    //section:[base + index*scale + disp] 

                                    if (opp.Count == 1)
                                    {
                                        opm = opp[0];
                                    }
                                    else if (opp.Count == 3)
                                    {
                                        opm = opp[0];
                                        opm += opm.Length > 0 ? " + " : "";
                                        opm += opp[1] +" * " + opp[2];
                                    }
                                    if (disp.Length > 0)
                                    {
                                        opm += disp.StartsWith("-") ? (" - "+ disp[1..]) : (" + " + disp);
                                    }
                                    if (opn.Enclosed || opn.Parts.Count>1)
                                    {
                                        opm = "[" + opm + "]";
                                        if (seg.Length > 0)
                                        {
                                            rpt = seg + ":" + opm;
                                        }
                                        else
                                        {
                                            rpt = opm;
                                        }
                                    }
                                    else
                                    {
                                        rpt = opm;
                                    }

                                    rts.Add(rpt);
                                }
                            }

                            if (far)
                            {
                                if(rts.Count == 1)
                                {
                                    line = pre + ins + " far " + rts[0];
                                }
                                else if(rts.Count == 2)
                                {
                                    line = pre + ins + " far " + rts[0] + ":" + rts[1];
                                }
                            }
                            else
                            {
                                rts.Reverse();
                                line = pre + ins;
                                if (rts.Count > 0)
                                {
                                    line += "\t" + rts.Aggregate((a, b) => a + ", " + b);
                                }
                            }
                        }
                        else //this is bad instruction!
                        {
                            line = ";BAD " + line;
                        }

                    }
                }

                OutputWriter.WriteLine(line + el);
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
