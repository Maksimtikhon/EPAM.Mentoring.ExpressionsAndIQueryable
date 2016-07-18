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
				ParameterExpression leftParam = null, rightParam = null;
				ConstantExpression leftConstant = null, rightConstant = null;

				if (node.Left.NodeType == ExpressionType.Parameter)
				{
					leftParam = (ParameterExpression)node.Left;
				}
				else if (node.Left.NodeType == ExpressionType.Constant)
				{
					leftConstant = (ConstantExpression)node.Left;
				}

				if (node.Right.NodeType == ExpressionType.Parameter)
				{
					rightParam = (ParameterExpression)node.Right;
				}
				else if (node.Right.NodeType == ExpressionType.Constant)
				{
					rightConstant = (ConstantExpression)node.Right;
				}

				// (x - 1) to Decrement(x)
				if (node.NodeType == ExpressionType.Subtract && leftParam != null && rightConstant != null
					&& rightConstant.Type == typeof(int) && (int)rightConstant.Value == 1)
				{
					return Expression.Decrement(leftParam);
				}

				var param = leftParam ?? rightParam;
				var constant = leftConstant ?? rightConstant;
				if (node.NodeType == ExpressionType.Add && param != null && constant != null && constant.Type == typeof(int))
				{
					// (x + 1) or (1 + x) to Increment(x)
					if ((int)constant.Value == 1)
					{
						return Expression.Increment(param);
					}
					// (-1 + x) to Decrement(x)
					else if ((int)constant.Value == -1)
					{
						return Expression.Decrement(param);
					}
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
