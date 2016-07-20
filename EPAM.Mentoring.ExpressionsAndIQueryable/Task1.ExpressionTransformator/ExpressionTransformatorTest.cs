using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Task1.ExpressionTransformator
{
	[TestClass]
	public class ExpressionTransformatorTest
	{
		[TestMethod]
		public void AddToIncrementAndSubstractToDecrementTransformTest()
		{
			Expression<Func<int, int>> sourceExpression = x => (x - 5) * (x - 1) + (x + 1) * (x + 10) + (1 + x) * (-1 + x);
			var resultExpression = new ExpressionTransformator().ReplaceAddAndSubstract(sourceExpression);

			var result = sourceExpression.Compile().Invoke(2);
		  var resultAfterTransformation = resultExpression.Compile().Invoke(2);

			Console.WriteLine("Source expression: {0} Result: {1}", sourceExpression, result);
			Console.WriteLine("Result expression: {0} Result: {1}", resultExpression, resultAfterTransformation);

			Assert.AreEqual(result, resultAfterTransformation);
		}

		[TestMethod]
		public void ParameterToConstantTransformTest()
		{
			var dictionary = new Dictionary<string, int>()
			{
				{ "a", -4 },
				{ "b", 0 },
				{ "c", 2 }
			};

			Expression<Func<int, int, int, int, int>> sourceExpression = (a, b, c, d) => a + b + c + d + 7;
			var resultExpression = (Expression <Func<int, int>>)new ExpressionTransformator().ReplaceParameters(sourceExpression, dictionary);

			var result = sourceExpression.Compile().Invoke(-4, 0, 2, 1);
			var resultAfterTransformation = resultExpression.Compile().Invoke(1);

			Console.WriteLine("Source expression: {0} Result: {1}", sourceExpression, result);
			Console.WriteLine("Result expression: {0} Result: {1}", resultExpression, resultAfterTransformation);

		  Assert.AreEqual(result, resultAfterTransformation);
		}
	}
}
