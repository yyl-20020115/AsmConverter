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
}
