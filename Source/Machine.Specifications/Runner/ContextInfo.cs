using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Machine.Specifications.Runner
{
  [DataContract]
  public class ContextInfo
  {
      [DataMember]
    public string Name { get; private set; }
      [DataMember]
    public string Concern { get; private set; }
      [DataMember]
    public string TypeName { get; private set; }
      [DataMember]
    public string Namespace { get; private set; }
      [DataMember]
    public string AssemblyName { get; private set; }
    public string FullName
    {
      get
      {
        string line = "";

        if (!String.IsNullOrEmpty(Concern))
        {
          line += Concern + ", ";
        }

        return line + Name;
      }
    }

    public ContextInfo(string name, string concern, string typeName, string @namespace, string assemblyName)
    {
      Concern = concern;
      Name = name;
      TypeName = typeName;
      AssemblyName = assemblyName;
      Namespace = @namespace;
    }
  }
}
