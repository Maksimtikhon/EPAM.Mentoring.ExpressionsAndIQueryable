using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Task1.ExpressionTransformator
{
	public class ExpressionTransformator : ExpressionVisitor
	{
		public Dictionary<string, int> _paramDictionary = new Dictionary<string, int>();

		public T ReplaceAddAndSubstract<T>(T expression) where T : Expression
		{
			return VisitAndConvert(expression, "");
		}

		public Expression ReplaceParameters(Expression expression, Dictionary<string, int> paramDictionary)
		{
			if (paramDictionary != null)
			{
				_paramDictionary = paramDictionary;
			}

			return Visit(expression);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (node.NodeType == ExpressionType.Add || node.NodeType == ExpressionType.Subtract)
			{
				var parameter = (node.Left.NodeType == ExpressionType.Parameter) ? (ParameterExpression)node.Left : null;
				var constant = (node.Right.NodeType == ExpressionType.Constant) ? (ConstantExpression)node.Right : null;

				if (parameter != null && constant != null && constant.Type == typeof(int) && (int)constant.Value == 1)
				{
					return node.NodeType == ExpressionType.Add ? Expression.Increment(parameter) : Expression.Decrement(parameter);
				}
			}

			return base.VisitBinary(node);
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			int value;
			if (_paramDictionary.TryGetValue(node.Name, out value))
			{
				return Expression.Constant(value);
			}

			return base.VisitParameter(node);
		}

		protected override Expression VisitLambda<T>(Expression<T> node)
		{
			var parameters = node.Parameters.Where(p => _paramDictionary.Keys.Contains(p.Name) == false);

			return Expression.Lambda(Visit(node.Body), parameters);
		}
	}
}
