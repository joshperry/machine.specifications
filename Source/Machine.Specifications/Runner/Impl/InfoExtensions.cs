using System.Reflection;
using Machine.Specifications.Model;

namespace Machine.Specifications.Runner.Impl
{
  public static class InfoExtensions
  {
    public static AssemblyInfo GetInfo(this Assembly assembly)
    {
      return new AssemblyInfo(GetAssemblyName(assembly), GetAssemblyLocation(assembly));
    }

    public static ContextInfo GetInfo(this Context context)
    {
      string concern = "";
      if (context.Subject != null)
      {
        concern = context.Subject.FullConcern;
      }

      return new ContextInfo(context.Name, concern, context.Type.FullName, context.Type.Namespace, GetAssemblyName(context.Type.Assembly));
    }

    public static SpecificationInfo GetInfo(this Specification specification)
    {
      return new SpecificationInfo(specification.Name, specification.FieldInfo.DeclaringType.FullName);
    }
      
    private static string GetAssemblyName(Assembly assembly)
    {
#if SILVERLIGHT
      return assembly.FullName.Split(',')[0];
#else
      return assembly.GetName().Name;
#endif
    }

    private static string GetAssemblyLocation(Assembly assembly)
    {
#if SILVERLIGHT
      return "Silverlight";
#else
      return assembly.Location;
#endif
    }
  }
}