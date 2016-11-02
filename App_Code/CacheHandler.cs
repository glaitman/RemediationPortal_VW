using System;
using System.Collections.Generic;
using System.Web.Caching;
using System.Web;

/// <summary>
/// This class reads/writes to ASP .NET server cache. Due to out of memory  exceptions this class has been
/// created as an alternative storing large objects in session.
/// </summary>
public class CacheHandler
{
	
	public static bool Write(string cacheID, object data)
	{
		if (HttpContext.Current == null)
			return false;

		if (cacheID == null || cacheID.Equals(""))
			return false;

		HttpRuntime.Cache.Insert(
				cacheID, data, null, Cache.NoAbsoluteExpiration, 
				Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null
				);
		return true;
	}

	public static object Read(string cacheID)
	{
		if (HttpContext.Current == null)
			return null;

		return HttpRuntime.Cache.Get(cacheID);
	}

	public static void Remove(string cacheID)
	{
		if (HttpContext.Current == null )
			return;

		if (cacheID == null || cacheID.Equals(""))
			return;

		HttpRuntime.Cache.Remove(cacheID);
	}
}
