using System.Reflection;

namespace E_Biznes.Application.Shared.Helpers;

public static class PermissionHelper
{
    public static Dictionary<string, List<string>> GetAllPermissions()
    {
        var result = new Dictionary<string, List<string>>();

        var nestedTypes = typeof(Permission).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

        foreach (var moduleType in nestedTypes)
        {
            var allfield = moduleType.GetField("All", BindingFlags.Public | BindingFlags.Static);
            if (allfield != null)
            {
                var permissions = allfield.GetValue(null) as List<string>;
                if (permissions != null)
                {
                    result.Add(moduleType.Name, permissions);
                }
            }
        }

        return result;
    }

    public static List<string> GetPermissionList()
    {
        return GetAllPermissions().SelectMany(x => x.Value).ToList();
    }


}