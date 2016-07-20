using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Task2.MappingGenerator.SampleClasses;

namespace Task2.MappingGenerator
{
	[TestClass]
	public class MappingGeneratorTest
	{
		private MappingGenerator _mappingGenerator;

		[TestInitialize]
		public void Intialize()
		{
			_mappingGenerator = new MappingGenerator();
		}

		[TestMethod]
		public void PropertiesMappingTest()
		{
			var mapper = _mappingGenerator.Generate<Bar, Foo>();
			var bar = new Bar
			{
				StringProperty = "string",
				IntProperty = 3,
				IntProperty2 = 5,
				DoubleProperty = 10.5,
				List = new List<int> { 5, 6 },
			};

			var foo = mapper.Map(bar);

			Assert.AreEqual(foo.StringProperty, bar.StringProperty);
			Assert.AreEqual(foo.IntProperty, bar.IntProperty);
			Assert.AreEqual(foo.DoubleProperty, bar.DoubleProperty);
			CollectionAssert.AreEqual(foo.List, bar.List);
			Assert.AreEqual(foo.ByteProperty, 0);
		}

		[TestMethod]
		public void FieldsMappingTest()
		{
			var mapper = _mappingGenerator.Generate<Bar, Foo>();
			var bar = new Bar
			{
				StringField = "hello",
				DoubleField = double.MaxValue,
				Array = new[] { 10, 20 }
			};

			var foo = mapper.Map(bar);

			Assert.AreEqual(foo.StringField, bar.StringField);
			Assert.AreEqual(foo.DoubleField, bar.DoubleField);
			CollectionAssert.AreEqual(foo.Array, bar.Array);
			Assert.AreEqual(foo.BoolField, false);
		}
	}
}
