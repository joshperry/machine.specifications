using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Machine.Specifications.ConsoleRunner.Properties;
using Machine.Specifications.Reporting.Generation.Spark;
using Machine.Specifications.Reporting.Generation.Xml;
using Machine.Specifications.Reporting.Integration;
using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;
using System.ServiceModel;

namespace Machine.Specifications.ConsoleRunner
{
  public class Program
  {

    [STAThread]
    [LoaderOptimization(LoaderOptimization.MultiDomain)]
    public static void Main(string[] args)
    {
      var program = new Program(new DefaultConsole());
      ExitCode exitCode = program.Run(args);

      Environment.Exit((int)exitCode);
    }

    readonly IConsole _console;

    public Program(IConsole console)
    {
      _console = console;
    }

    public ExitCode Run(string[] arguments)
    {
      ExceptionReporter reporter = new ExceptionReporter(_console);

      Options options = new Options();
      if (!options.ParseArguments(arguments))
      {
        _console.WriteLine(Resources.UsageStatement);
        return ExitCode.Failure;
      }

          ISpecificationRunListener mainListener = null;
      do
      {
          List<ISpecificationRunListener> listeners = new List<ISpecificationRunListener>();

          var timingListener = new TimingRunListener();
          listeners.Add(timingListener);

          if (options.TeamCityIntegration)
          {
              mainListener = new TeamCityReporter(_console.WriteLine, timingListener);
          }
          else
          {
              mainListener = new RunListener(_console, options.Silent, timingListener);
          }

          try
          {

              if (!String.IsNullOrEmpty(options.HtmlPath))
              {
                  if (IsHtmlPathValid(options.HtmlPath))
                  {
                      listeners.Add(GetHtmlReportListener(options));
                  }
                  else
                  {
                      _console.WriteLine("Invalid html path:" + options.HtmlPath);
                      _console.WriteLine(Resources.UsageStatement);
                      return ExitCode.Failure;
                  }

              }

              if (!String.IsNullOrEmpty(options.XmlPath))
              {
                  if (IsHtmlPathValid(options.XmlPath))
                  {
                      listeners.Add(GetXmlReportListener(options));
                  }
                  else
                  {
                      _console.WriteLine("Invalid xml path:" + options.XmlPath);
                      _console.WriteLine(Resources.UsageStatement);
                      return ExitCode.Failure;
                  }
              }

              listeners.Add(mainListener);

              if (options.AssemblyFiles.Count == 0)
              {
                  _console.WriteLine(Resources.UsageStatement);
                  return ExitCode.Failure;
              }

              _console.WriteLine("Files Count: {0} Name: {1}", options.AssemblyFiles.Count, options.AssemblyFiles.Count > 0?options.AssemblyFiles[options.AssemblyFiles.Count-1]:"none");
              bool runXap = options.AssemblyFiles.Count > 0 && options.AssemblyFiles[options.AssemblyFiles.Count-1].EndsWith(".xap", StringComparison.OrdinalIgnoreCase);

              if (!options.WcfListen && !runXap)
              {
                  listeners.Add(new AssemblyLocationAwareListener());
                  var listener = new AggregateRunListener(listeners);

                  ISpecificationRunner specificationRunner = new AppDomainRunner(listener, options.GetRunOptions());
                  List<Assembly> assemblies = new List<Assembly>();
                  foreach (string assemblyName in options.AssemblyFiles)
                  {
                      if (!File.Exists(assemblyName))
                      {
                          throw NewException.MissingAssembly(assemblyName);
                      }

                      Assembly assembly = Assembly.LoadFrom(assemblyName);
                      assemblies.Add(assembly);
                  }

                  specificationRunner.RunAssemblies(assemblies);
              }
              else
              {
                  var completionListener = new CompletionListener();
                  listeners.Add(completionListener);

                  var listener = new AggregateRunListener(listeners);

                  var proxy = new WcfRunnerProxy(listener);
                  ServiceHost host = null;
                  try
                  {
                      host = new ServiceHost(proxy);

                      host.AddServiceEndpoint(typeof(ISpecificationRunListener), new BasicHttpBinding(), new Uri("http://localhost:5931/MSpecListener"));

                      ((System.ServiceModel.Description.ServiceDebugBehavior)host.Description.Behaviors[typeof(System.ServiceModel.Description.ServiceDebugBehavior)]).IncludeExceptionDetailInFaults = true;

                      var smb = new System.ServiceModel.Description.ServiceMetadataBehavior();
                      smb.MetadataExporter.PolicyVersion = System.ServiceModel.Description.PolicyVersion.Policy15;
                      host.Description.Behaviors.Add(smb);

                      host.AddServiceEndpoint(typeof(System.ServiceModel.Description.IMetadataExchange),
                          System.ServiceModel.Description.MetadataExchangeBindings.CreateMexHttpBinding(), "http://localhost:5931/MSpecListener/MEX");

                      host.Open();

                      _console.WriteLine("=========================================================================");
                      _console.WriteLine("Waiting for test results via WCF at http://localhost:5931/MSpecListener");

                      if (runXap)
                      {
                          var xap = options.AssemblyFiles[options.AssemblyFiles.Count-1];
                          if (!File.Exists(xap))
                          {
                              throw NewException.MissingAssembly(xap);
                          }

                          var runner = new Wp7DeviceRunner(true);
                          runner.RunXap(xap);
                      }

                      completionListener.WaitForRunCompletion();
                      System.Threading.Thread.Sleep(1000);
                  }
                  finally
                  {
                      if (host != null && host.State != CommunicationState.Faulted)
                          host.Close();
                  }
              }
          }
          catch (Exception ex)
          {
              if (System.Diagnostics.Debugger.IsAttached)
                  System.Diagnostics.Debugger.Break();

              reporter.ReportException(ex);
              return ExitCode.Error;
          }
      } while (options.Loop);

      if (mainListener != null && mainListener is ISpecificationResultProvider)
      {
        var errorProvider = (ISpecificationResultProvider)mainListener;
        if (errorProvider.FailureOccured)
        {
          Console.WriteLine("Generic failure occurred, no idea what this is");
          return ExitCode.Failure;
        }
      }
      return ExitCode.Success;
    }

    private static ISpecificationRunListener GetXmlReportListener(Options options)
    {
      var listener = new GenerateXmlReportListener(options.XmlPath, options.ShowTimeInformation);

      return listener;
    }

    static bool IsHtmlPathValid(string path)
    {
      return Directory.Exists(Path.GetDirectoryName(path));
    }

    public ISpecificationRunListener GetHtmlReportListener(Options options)
    {
      return new GenerateSparkHtmlReportRunListener(options.HtmlPath, options.ShowTimeInformation);
    }

    class CompletionListener : ISpecificationRunListener
    {
        System.Threading.AutoResetEvent _waiter = new System.Threading.AutoResetEvent(false);

        public void WaitForRunCompletion()
        {
            _waiter.WaitOne();
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
        }

        public void OnRunStart()
        {
        }

        public void OnRunEnd()
        {
            _waiter.Set();
        }

        public void OnContextStart(ContextInfo context)
        {
        }

        public void OnContextEnd(ContextInfo context)
        {
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
        }

        public void OnFatalError(ExceptionResult exception)
        {
        }
    }

  }
}