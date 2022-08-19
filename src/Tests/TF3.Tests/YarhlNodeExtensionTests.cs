namespace TF3.Tests
{
    using System.Collections.Generic;
    using System.Text.Json;
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
            const string parameter = "{\"Id\":\"TestParameterId\",\"TypeName\":\"System.String\",\"Value\":\"Prueba\"}";

            var converter = new ConverterInfo() { TypeName = "TF3.Core.Converters.FormatToSingleNode", ParameterId = "TestParameterId", };
            ParameterInfo parameterInfo = JsonSerializer.Deserialize<ParameterInfo>(parameter);

            using Node n = NodeFactory.FromMemory("test");
            Assert.IsFalse(n.IsContainer);
            n.Transform(new List<ConverterInfo>() { converter }, new List<ParameterInfo>() { parameterInfo });
            Assert.IsTrue(n.IsContainer);
            Assert.AreEqual("Prueba", n.Children[0].Name);
        }

        [Test]
        public void MultipleConverters()
        {
            var converter1 = new ConverterInfo() { TypeName = "TF3.Core.Converters.FormatToSingleNode", ParameterId = string.Empty, };
            var converter2 = new ConverterInfo() { TypeName = "TF3.Core.Converters.SingleNodeToFormat", ParameterId = string.Empty, };
            using Node n = NodeFactory.FromMemory("test");
            Assert.IsFalse(n.IsContainer);
            n.Transform(new List<ConverterInfo>() { converter1, converter2 }, new List<ParameterInfo>());
            Assert.IsFalse(n.IsContainer);
            Assert.IsTrue(n.Stream != null);
        }

        [Test]
        public void UnknownConverterThrowsException()
        {
            var converter = new ConverterInfo() { TypeName = "TF3.Core.Converters.FormatToSingleNode1", ParameterId = string.Empty, };
            using Node n = NodeFactory.FromMemory("test");
            Assert.Throws<UnknownConverterException>(() => n.Transform(new List<ConverterInfo>() { converter }, new List<ParameterInfo>()));
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
            const string translator = "TF3.Core.Converters.FormatReplace";
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
            const string translator = "TF3.Core.Converters.FormatReplace1";
            using Node n = NodeFactory.FromMemory("test");
            using Node translation = NodeFactory.FromMemory("test2");
            Assert.Throws<UnknownConverterException>(() => n.Translate(translation, translator));
        }
    }
}
