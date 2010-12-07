using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Machine.Specifications.Runner
{
  [DataContract]
  public class AssemblyInfo
  {
      [DataMember]
    public string Name { get; private set; }

      [DataMember]
    public string Location { get; private set; }

    public AssemblyInfo(string name, string location)
    {
      this.Name = name;
      this.Location = location;
    }
  }
}
