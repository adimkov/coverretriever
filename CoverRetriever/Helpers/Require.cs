using System;

public static class Require
{
	/// <summary>
	/// Throws <see cref="ArgumentNullException"/> in case target is null
	/// </summary>
	/// <param name="target"></param>
	/// <param name="paramName"></param>
	/// <param name="message"></param>
	public static void NotNull(object target, string paramName)
	{
		if (target == null)
		{
			throw new ArgumentNullException(paramName, "{0} is null".FormatString(paramName));
		}
	}

	
}
