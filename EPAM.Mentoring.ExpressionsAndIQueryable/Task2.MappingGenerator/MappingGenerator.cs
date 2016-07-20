using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Task2.MappingGenerator
{
	public class MappingGenerator
	{
		public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
		{
			var builderFunc = GenerateBuilderFunc<TDestination>();
			var mappingAction = GenerateMappingAction<TSource, TDestination>();

			return new Mapper<TSource, TDestination>(builderFunc, mappingAction);
		}

		private Func<TDestination> GenerateBuilderFunc<TDestination>()
		{
			var builder =
				Expression.Lambda<Func<TDestination>>(
					Expression.New(typeof(TDestination)));

			return builder.Compile();
		}

		private Action<TSource, TDestination> GenerateMappingAction<TSource, TDestination>()
		{
			var sourceType = typeof(TSource);
			var destinationType = typeof(TDestination);
			var sourceParam = Expression.Parameter(sourceType);
			var destinationParam = Expression.Parameter(destinationType);
			var expressions = new List<Expression>();

			// adding of expressions for properties mapping
			foreach (var sourceProperty in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (!sourceProperty.CanRead)
				{
					continue;
				}

				var destinationProperty = destinationType.GetProperty(sourceProperty.Name, BindingFlags.Public | BindingFlags.Instance);
				if (destinationProperty != null && destinationProperty.CanWrite && destinationProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
				{
					expressions.Add(
						Expression.Assign(
							Expression.Property(destinationParam, destinationProperty),
							Expression.Convert(
								Expression.Property(sourceParam, sourceProperty), destinationProperty.PropertyType)));
				}
			}

			// adding of expressions for fields mapping
			foreach (var sourceField in sourceType.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				var destinationField = destinationType.GetField(sourceField.Name, BindingFlags.Public | BindingFlags.Instance);
				if (destinationField != null && destinationField.FieldType.IsAssignableFrom(sourceField.FieldType))
				{
					expressions.Add(
						Expression.Assign(
							Expression.Field(destinationParam, destinationField),
							Expression.Convert(
								Expression.Field(sourceParam, sourceField), destinationField.FieldType)));
				}
			}

			var mapper =
				Expression.Lambda<Action<TSource, TDestination>>(
					Expression.Block(expressions),
					sourceParam,
					destinationParam);

			return mapper.Compile();
		}
	}
}
