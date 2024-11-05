namespace App.Common.Models
{
	public class GetResourcesRequest : GetListBaseRequest
	{
	}

	public class GetResourcesResponse : GetListBaseResponse<ResourceViewModel>
	{
	}

	public class ResourceViewModel
	{
		/// <summary>
		/// 資源 ID
		/// </summary>
		public string Id { get; set; } = null!;
		/// <summary>
		/// 資源名稱
		/// </summary>
		public string Name { get; set; } = null!;
	}
}
