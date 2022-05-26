using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsmConverter
{
    public partial class Program
    {
        public static void ConvertFromATT_ToIntel(FileInfo fi, FileInfo fo)
        {
            var lineno = 0;
            using var InputReader = new StreamReader(fi.FullName);
            using var OutputWriter = new StreamWriter(fo.FullName);
            var indef = false;
            var outdef = false;
            var lastpre = "";
            var align = 1;
            while (InputReader.ReadLine() is string line)
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
                var sp = line.IndexOfAny(new[] { '#', ';' });
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

                var pre = line.IndexOf(result) is int ep && ep >= 0 ? line[..ep] : "";
                if (result.Length == 0)
                {
                    OutputWriter.WriteLine(line + lineEnding);
                    continue;
                }
                else if (result.IndexOf(':') is int p && p >= 0)
                {
                    if (p == result.Length - 1)
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
                        else if (resultLower.StartsWith(".def") && char.IsWhiteSpace(resultLower, ".def".Length))
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
                                line = "global " + result[rp..].Trim();
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
                        else if (resultLower.StartsWith(".word"))
                        {
                            line = "dw " + result[(".word".Length + 1)..];
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
                        else if (resultLower.StartsWith(".section") && char.IsWhiteSpace(resultLower, ".section".Length))
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
                                if (int.TryParse(ts.Parts[0], out var _align))
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
                        var orginstr = instr;
                        var instrLower = instr.ToLower();
                        var operandSize = OperandSize.Unknown;

                        if (instrLower == "ljmp" || instrLower == "lcall" || instrLower == "lret")
                        {
                            instrLower = instrLower[1..];
                            instr = instr[1..];
                            far = true;
                            if (instrLower != "lret") star = true;
                        }
                        if (instrLower.StartsWith("j") || instrLower == "call")
                        {
                            if (operands.StartsWith('*'))
                            {
                                operands = operands[1..];
                            }
                        }
                        if (instrLower.EndsWith('b') || instrLower.EndsWith('w') || instrLower.EndsWith('l') || instrLower.EndsWith('q'))
                        {
                            if (Instructions.NasmInstructionNames.Contains(instrLower[..^1]))
                            {
                                operandSize = instrLower[^1] switch
                                {
                                    'b' => OperandSize.Byte,
                                    'w' => OperandSize.Word,
                                    'l' => OperandSize.Long,
                                    'q' => OperandSize.Quad,
                                    _ => OperandSize.Unknown
                                };
                                instrLower = instrLower[..^1];
                                instr = instr[..^1];
                            }
                        }

                        if (Instructions.NasmInstructionNames.Contains(instrLower))
                        {
                            var operandsParts = Split(operands, ',');
                            var operandsTransformed = new List<string>();
                            //opm = base, index, scale
                            var ptr = "";
                            char ec = ' ';
                            if (instrLower.Length == 5 &&
                                (instrLower.StartsWith("movz") || instrLower.StartsWith("movs"))
                                &&
                                (instrLower.EndsWith('b') || instrLower.EndsWith('w') || instrLower.EndsWith('l') || instrLower.EndsWith('q')))
                            {
                                ec = instrLower[^1];
                            }
                            else if (orginstr.Length == 4 &&
                                (instrLower.StartsWith("mul") || instrLower.StartsWith("div")))
                            {
                                //ok
                            }
                            if (ec == ' ' && operandSize != OperandSize.Unknown)
                            {
                                //this is a patch
                                switch (operandSize)
                                {
                                    case OperandSize.Byte:
                                        ec = 'b';
                                        break;
                                    case OperandSize.Word:
                                        ec = 'w';
                                        break;
                                    case OperandSize.Long:
                                        ec = 'l';
                                        break;
                                    case OperandSize.Quad:
                                        ec = 'q';
                                        break;
                                }
                            }

                            switch (ec)
                            {
                                case 'b':
                                    ptr = "byte ";
                                    break;
                                case 'w':
                                    ptr = "word ";
                                    break;
                                case 'l':
                                    ptr = "dword ";
                                    break;
                                case 'q':
                                    ptr = "qword ";
                                    break;
                            }
                            if (ptr.Length > 0)
                            {
                                if (instrLower.StartsWith("movz") || instrLower.StartsWith("movs"))
                                    instr = instr[..^1] + "x";
                            }

                            foreach (var operand in operandsParts.Parts)
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
                                    if (po >= 0 && operandTrimmeds.EndsWith(')'))
                                    {
                                        disp = operandTrimmeds[..po];
                                        operandTrimmeds = operandTrimmeds[po..];
                                    }

                                    var operandsList = new List<string>();
                                    var operandsSplitted = Split(operandTrimmeds, keep_bound: false);
                                    foreach (var operandPart in operandsSplitted.Parts)
                                    {
                                        var operandPartWorking = operandPart;
                                        if (operandPartWorking.StartsWith("%") &&
                                            operandPartWorking[1..] is string operandPartTrimmed
                                            && Instructions.NasmRegisters.Contains(operandPartTrimmed))
                                        {
                                            operandPartTrimmed = FixRegister(operandPartTrimmed);
                                            operandsList.Add(operandPartTrimmed);
                                        }
                                        else if (operandPartWorking.StartsWith("$") &&
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
                                        operandTrimmeds += disp.StartsWith("-") ? (" - " + disp[1..]) : (" + " + disp);
                                    }
                                    if (operandsSplitted.Enclosed
                                        || operandsSplitted.Parts.Count > 1)
                                    {
                                        operandTrimmed =
                                           ptr + "[" + sectionRegister + operandTrimmeds + "]";
                                        ptr = "";
                                    }
                                    else
                                    {
                                        operandTrimmed = operandTrimmeds;
                                    }

                                    operandsTransformed.Add(operandTrimmed);
                                }

                            }
                            instrLower = instr.ToLower();
                            if (instrLower == "movsx" || instrLower == "movzx")
                            {
                                if (operandsTransformed.Count == 0)
                                {
                                    instr = instr[..^1];
                                }
                            }
                            if (far)
                            {
                                if (operandsTransformed.Count == 1)
                                {
                                    line = instr + " far " + operandsTransformed[0];
                                }
                                else if (operandsTransformed.Count == 2)
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
                                    if ((instrLower == "shr" || instrLower == "shl") && operandsTransformed.Count == 1)
                                    {
                                        //fix the shr reg,1 issues
                                        operandsTransformed.Add("1");
                                    }
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

    }
}
