namespace System.Linq
{
	public static class DeferredCallEx
	{
		/// <summary>
		/// Make deferred call to a service
		/// </summary>
		/// <typeparam name="T">Type of observable</typeparam>
		/// <param name="observable">Observable</param>
		/// <param name="doAction">Call service action</param>
		/// <returns></returns>
		public static IObservable<T> Defer<T> (this IObservable<T> observable, Action doAction)
		{
			return Observable.Defer(() =>
			{
				try
				{
					doAction();
				}
				catch (Exception ex)
				{
					return Observable.Throw<T>(ex);
				}
				return observable;
			});
		}
	}
}