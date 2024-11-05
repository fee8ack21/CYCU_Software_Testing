using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;

namespace App.Common.Filters
{
	/// <summary>
	/// 隱藏 Model 欄位不顯示在 Swagger 上 (JsonIgnore 不適用 FromQuery 情境，因此另做補充)
	/// </summary>
	public class JsonIgnoreQueryOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			context.ApiDescription.ParameterDescriptions
				.Where(d => d.Source.Id == "Query").ToList()
				.ForEach(desc =>
				{
					var toIgnore = ((DefaultModelMetadata)desc.ModelMetadata).Attributes.PropertyAttributes?.Any(x => x is JsonIgnoreAttribute);
					var toRemove = operation.Parameters.SingleOrDefault(p => p.Name == desc.Name);
					if (toIgnore ?? false) operation.Parameters.Remove(toRemove);
				});
		}
	}
}
