// Copyright (c) 2021 Kaplas
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace TF3.CommandLine {
    using System;
    using System.IO;
    using TF3.Plugin.YakuzaKiwami2.Converters.Armp;
    using TF3.Plugin.YakuzaKiwami2.Formats;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    /// <summary>
    /// Main program class.
    /// </summary>
    public static class Program {
        /// <summary>
        /// Main entry-point.
        /// </summary>
        /// <param name="args">Application arguments.</param>
        public static void Main(string[] args)
        {
            // Method intentionally left empty.
            string path = @"H:\tmp\Yakuza Kiwami 2\data\db.par.unpack\en";
            
            foreach (string file in Directory.EnumerateFiles(path, "*.bin")) {
                Node node = NodeFactory.FromFile(file);
                _ = node.TransformWith<Reader>();
                ArmpTable expected = node.GetFormatAs<ArmpTable>();

                _ = node.TransformWith<Writer>();
                BinaryFormat converted = node.GetFormatAs<BinaryFormat>();
                converted.Stream.WriteTo(string.Concat(file, ".test"));

                _ = node.TransformWith<Reader>();
                ArmpTable result = node.GetFormatAs<ArmpTable>();

                if (!result.Equals(expected)) {
                    Console.WriteLine(file);
                } else {
                    File.Delete(string.Concat(file, ".test"));
                }
            }
            

            /*
            string fileName = "character_generate_property_template.bin";
            string file1 = Path.Combine(path, fileName);
            Node node = NodeFactory.FromFile(file1);
            _ = node.TransformWith<Reader>();
            ArmpTable expected = node.GetFormatAs<ArmpTable>();

            _ = node.TransformWith<Writer>();
            //node.Stream.WriteTo(string.Concat(file1, ".test"));
            
            _ = node.TransformWith<Reader>();
            ArmpTable result = node.GetFormatAs<ArmpTable>();
            _ = result.Equals(expected);
            // _ = node.TransformWith<XlsxWriter>();
            // node.Stream.WriteTo(string.Concat(file1, ".xlsx"));

            /*
            ArmpTable expected = node.GetFormatAs<ArmpTable>();
            _ = node.TransformWith<Writer>();
            node.Stream.WriteTo(string.Concat(file1, ".test"));

            string file2 = Path.Combine(path, string.Concat(fileName, ".test"));
            node = NodeFactory.FromFile(file2);
            _ = node.TransformWith<Reader>();
            ArmpTable result = node.GetFormatAs<ArmpTable>();

            _ = result.Equals(expected);
            */
        }
    }
}
