using System;

namespace HoaHoeHoaSoi.UpLoadFile
{
	public interface EmptyInterface
	{
		Task<string> UpLoadFileAsync(IFormFile);
	}
}

