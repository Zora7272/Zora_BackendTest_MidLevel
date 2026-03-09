using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApplication1.Models;

namespace WebApplication1.Swagger
{
    public class SampleSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(MyOfficeAcpd))
            {
                schema.Example = new OpenApiObject
                {
                    ["ACPD_Cname"] = new OpenApiString("王小明"),
                    ["ACPD_Ename"] = new OpenApiString("Xiaoming Wang"),
                    ["ACPD_Sname"] = new OpenApiString("小明"),
                    ["ACPD_Email"] = new OpenApiString("xiaoming@example.com"),
                    ["ACPD_Status"] = new OpenApiInteger(1),
                    ["ACPD_Stop"] = new OpenApiBoolean(false),
                    ["ACPD_StopMemo"] = new OpenApiString(null),
                    ["ACPD_LoginID"] = new OpenApiString("xiaoming"),
                    ["ACPD_LoginPWD"] = new OpenApiString("P@ssw0rd"),
                    ["ACPD_Memo"] = new OpenApiString("測試用帳戶"),
                    ["ACPD_NowDateTime"] = new OpenApiString(System.DateTime.Now.ToString("o")),
                    ["ACPD_NowID"] = new OpenApiString("system"),
                    ["ACPD_UPDDateTime"] = new OpenApiString(System.DateTime.Now.ToString("o")),
                    ["ACPD_UPDID"] = new OpenApiString("system")
                };
            }
        }
    }
}
