// Copyright (c) 2022 Kaplas
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

namespace TF3.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using TF3.Core.Exceptions;
    using TF3.Core.Helpers;
    using TF3.Core.Models;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    public class YarhlNodeExtensionTests
    {
        [Test]
        public void SingleConverter()
        {
            byte[] values = { 0x01, 0x02 };
            using DataStream stream = DataStreamFactory.FromArray(values);
            var converter = new ConverterInfo() { TypeName = "TF3.Core.Converters.SingleNodeToFormat", ParameterId = string.Empty, };

            using Node ncf = NodeFactory.CreateContainer("root");
            using Node n = NodeFactory.FromSubstream("test", stream, 0, stream.Length);
            ncf.Add(n);

            Assert.IsTrue(ncf.IsContainer);
            ncf.Transform(new List<ConverterInfo>() { converter }, new List<ParameterInfo>());
            Assert.IsFalse(ncf.IsContainer);
            Assert.IsTrue(ncf.Stream.Compare(stream));
        }

        [Test]
        public void MultipleConverters()
        {
            byte[] values = { 0x01, 0x02 };
            using DataStream stream = DataStreamFactory.FromArray(values);

            var converter1 = new ConverterInfo() { TypeName = "TF3.Core.Converters.SingleNodeToFormat", ParameterId = string.Empty, };
            var converter2 = new ConverterInfo() { TypeName = "TF3.Core.Converters.SingleNodeToFormat", ParameterId = string.Empty, };
            using Node ncf = NodeFactory.CreateContainer("root");
            using Node n1 = NodeFactory.CreateContainer("test1");
            using Node n2 = NodeFactory.FromSubstream("test", stream, 0, stream.Length);
            n1.Add(n2);
            ncf.Add(n1);

            Assert.IsTrue(ncf.IsContainer);
            ncf.Transform(new List<ConverterInfo>() { converter1, converter2 }, new List<ParameterInfo>());
            Assert.IsFalse(ncf.IsContainer);
            Assert.IsTrue(ncf.Stream.Compare(stream));
        }

        [Test]
        public void UnknownConverterThrowsException()
        {
            var converter = new ConverterInfo() { TypeName = "TF3.Core.Converters.SingleNodeToFormat1", ParameterId = string.Empty, };
            using Node n = NodeFactory.FromMemory("test");
            _ = _ = Assert.Throws<UnknownConverterException>(() => n.Transform(new List<ConverterInfo>() { converter }, new List<ParameterInfo>()));
        }

        [Test]
        public void EmptyConverterListDoesNotChangeFormat()
        {
            using Node n = NodeFactory.FromMemory("test");
            IBinary format = n.GetFormatAs<IBinary>();
            n.Transform(new List<ConverterInfo>(), new List<ParameterInfo>());
            IBinary format2 = n.GetFormatAs<IBinary>();
            Assert.AreSame(format, format2);
        }

        [Test]
        public void TranslateNode()
        {
            const string translator = "TF3.YarhlPlugin.Common.Converters.FormatReplace";
            using Node n = NodeFactory.FromMemory("test");
            using Node translation = NodeFactory.FromMemory("test2");

            IBinary format1 = n.GetFormatAs<IBinary>();
            IBinary format2 = translation.GetFormatAs<IBinary>();
            Assert.AreNotSame(format1, format2);

            n.Translate(translation, translator);

            IBinary result = n.GetFormatAs<IBinary>();
            Assert.AreSame(result, format2);
        }

        [Test]
        public void EmptyTranslatorReplacesFormat()
        {
            const string translator = "";
            using Node n = NodeFactory.FromMemory("test");
            using Node translation = NodeFactory.FromMemory("test2");

            IBinary format1 = n.GetFormatAs<IBinary>();
            IBinary format2 = translation.GetFormatAs<IBinary>();
            Assert.AreNotSame(format1, format2);

            n.Translate(translation, translator);

            IBinary result = n.GetFormatAs<IBinary>();
            Assert.AreSame(result, format2);
        }

        [Test]
        public void UnknownTranslatorThrowsException()
        {
            const string translator = "TF3.YarhlPlugin.Common.Converters.FormatReplace1";
            using Node n = NodeFactory.FromMemory("test");
            using Node translation = NodeFactory.FromMemory("test2");
            _ = _ = Assert.Throws<UnknownConverterException>(() => n.Translate(translation, translator));
        }
    }
}
