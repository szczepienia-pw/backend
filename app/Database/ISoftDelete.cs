using System;

namespace backend.Database
{
	public interface ISoftDelete
	{
		bool IsDeleted { get; set; }

		void SoftDelete() => this.IsDeleted = true;
	}
}

