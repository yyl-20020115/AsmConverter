using System.Text;

namespace AsmConverter
{
    public partial class Program
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

        public static int Main2(string[] args)
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
