using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Task3.QueryProvider
{
	public class ExpressionToFTSRequestTranslator : ExpressionVisitor
	{
		List<StringBuilder> _queries;

		public IEnumerable<string> Translate(Expression exp)
		{
			_queries = new List<StringBuilder>();
			Visit(exp);

			return _queries.Select(p => p.ToString());
		}

		private StringBuilder CurrentQuery
		{
			get
			{
				if (_queries.Count == 0)
				{
					_queries.Add(new StringBuilder());
				}

				return _queries.Last();
			}
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "Where")
			{
				var predicate = node.Arguments[1];
				Visit(predicate);

				return node;
			}

			if (node.Method.DeclaringType == typeof(string))
			{
				if (node.Method.Name == "StartsWith")
				{
					Visit(node.Object);
					CurrentQuery.Append("(");
					Visit(node.Arguments[0]);
					CurrentQuery.Append("*)");
				}

				if (node.Method.Name == "EndsWith")
				{
					Visit(node.Object);
					CurrentQuery.Append("(*");
					Visit(node.Arguments[0]);
					CurrentQuery.Append(")");
				}

				if (node.Method.Name == "Contains")
				{
					Visit(node.Object);
					CurrentQuery.Append("(*");
					Visit(node.Arguments[0]);
					CurrentQuery.Append("*)");
				}

				return node;
			}

			return base.VisitMethodCall(node);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			switch (node.NodeType)
			{
				case ExpressionType.Equal:
					// Check parameters
					if (!(node.Left.NodeType == ExpressionType.MemberAccess && node.Right.NodeType == ExpressionType.Constant) &&
							!(node.Left.NodeType == ExpressionType.Constant && node.Right.NodeType == ExpressionType.MemberAccess))
					{
						throw new NotSupportedException("Available parameters: <member> == <constant> or <constant> == <member>");
					}

					// Visit member
					Visit(node.Left.NodeType == ExpressionType.MemberAccess ? node.Left : node.Right);
					CurrentQuery.Append("(");
					// Visit constant
					Visit(node.Right.NodeType == ExpressionType.Constant ? node.Right : node.Left);
					CurrentQuery.Append(")");

					return node;
				case ExpressionType.AndAlso:
					Visit(node.Left);
					AddQuery();
					Visit(node.Right);

					return node;
				default:
					throw new NotSupportedException(string.Format("Operation {0} is not supported", node.NodeType));
			}
		}

		private void AddQuery()
		{
			_queries.Add(new StringBuilder());
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			CurrentQuery.Append(node.Member.Name).Append(":");

			return base.VisitMember(node);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			CurrentQuery.Append(node.Value);

			return node;
		}
	}
}
