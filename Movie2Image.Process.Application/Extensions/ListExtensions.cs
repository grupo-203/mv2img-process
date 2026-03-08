namespace Movie2Image.Process;

public static class ListExtensions
{

	public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
	{
		return list == null || !list.Any();
	}

}
