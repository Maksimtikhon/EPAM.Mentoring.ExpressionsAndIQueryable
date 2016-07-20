using System;

namespace Task2.MappingGenerator
{
	public class Mapper<TSource, TDestination>
	{
		private readonly Func<TDestination> _builderFunc;
		private readonly Action<TSource, TDestination> _mappingAction;

		internal Mapper(Func<TDestination> builderFunc, Action<TSource, TDestination> mappingAction)
		{
			_builderFunc = builderFunc;
			_mappingAction = mappingAction;
		}

		public TDestination Map(TSource source)
		{
			var destination = _builderFunc();
			_mappingAction(source, destination);

			return destination;
		}
	}
}
