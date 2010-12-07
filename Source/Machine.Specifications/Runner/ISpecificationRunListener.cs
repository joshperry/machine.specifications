using System.Reflection;
using System.ServiceModel;

namespace Machine.Specifications.Runner
{
  [ServiceContract]
  public interface ISpecificationRunListener
  {
      [OperationContract]
    void OnAssemblyStart(AssemblyInfo assembly);
      [OperationContract]
    void OnAssemblyEnd(AssemblyInfo assembly);
      [OperationContract]
    void OnRunStart();
      [OperationContract]
    void OnRunEnd();
      [OperationContract]
    void OnContextStart(ContextInfo context);
      [OperationContract]
    void OnContextEnd(ContextInfo context);
      [OperationContract]
    void OnSpecificationStart(SpecificationInfo specification);
      [OperationContract]
    void OnSpecificationEnd(SpecificationInfo specification, Result result);
      [OperationContract]
    void OnFatalError(ExceptionResult exception);
  }

  public interface ISpecificationResultProvider
  {
    bool FailureOccured { get; }
  }
}