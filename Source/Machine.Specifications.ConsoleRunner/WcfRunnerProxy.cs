using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Machine.Specifications.Runner;

namespace Machine.Specifications.ConsoleRunner
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class WcfRunnerProxy : ISpecificationRunListener
    {
        private ISpecificationRunListener _listener;

        public WcfRunnerProxy(ISpecificationRunListener listener)
        {
            _listener = listener;
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            _listener.OnAssemblyStart(assembly);
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            _listener.OnAssemblyEnd(assembly);
        }

        public void OnRunStart()
        {
            _listener.OnRunStart();
        }

        public void OnRunEnd()
        {
            _listener.OnRunEnd();
        }

        public void OnContextStart(ContextInfo context)
        {
            _listener.OnContextStart(context);
        }

        public void OnContextEnd(ContextInfo context)
        {
            _listener.OnContextEnd(context);
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            _listener.OnSpecificationStart(specification);
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            _listener.OnSpecificationEnd(specification, result);
        }

        public void OnFatalError(ExceptionResult exception)
        {
            _listener.OnFatalError(exception);
        }
    }
}
