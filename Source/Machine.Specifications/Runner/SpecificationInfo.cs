using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Machine.Specifications.Runner
{
  [DataContract]
  public class SpecificationInfo
  {
      [DataMember]
    public string Name { get; set; }
      [DataMember]
    public string ContainingType { get; set; }

    public SpecificationInfo(string name, string containingType)
    {
      Name = name;
      ContainingType = containingType;
    }
  }
}
