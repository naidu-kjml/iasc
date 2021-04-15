using System.IO;
using NUnit.Framework;
using Its.Iasc.Workflows.Utils;

namespace Its.Iasc.Workflows
{
    public class WorkflowGenericTest
    {
        private string yaml1 = @"
config:
  defaultChartId: helm-terraform-gcp

charts:
  helm-terraform-gcp:
    chartUrl: https://its-software-services-devops.github.io/helm-terraform-gcp/
    version: 1.1.5-rc8

infraIasc:
  - valuesFile: iasc-its-global.yaml
    chartId: helm-terraform-gcp
    version: 1.1.5-rc8
    alias: iasc-its-global
    chartUrl: https://its-software-services-devops.github.io/helm-terraform-gcp/

  - valuesFile: iasc-its-gce-manager.yaml
  - valuesFile: iasc-its-gce-rke.yaml
";
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void YamlParseNormalTest()
        {
            var wf = new WorkflowGeneric();
            var ctx = wf.GetContext();

            var result = wf.Load(yaml1);
            var m = wf.GetManifest();

            Assert.AreEqual(0, result);
            Assert.NotNull(m);
            Assert.NotNull(ctx);

            Assert.AreEqual("helm-terraform-gcp", m.Config.DefaultChartId);
            Assert.AreEqual(1, m.Charts.Count);
            Assert.AreEqual(3, m.InfraIasc.Length);

            var chart = m.Charts["helm-terraform-gcp"];

            Assert.AreEqual("https://its-software-services-devops.github.io/helm-terraform-gcp/", chart.ChartUrl);
            Assert.AreEqual("1.1.5-rc8", chart.Version);

            var iasc = m.InfraIasc[0];
            Assert.AreEqual("iasc-its-global.yaml", iasc.ValuesFile);
            Assert.AreEqual("helm-terraform-gcp", iasc.ChartId);
            Assert.AreEqual("1.1.5-rc8", iasc.Version);
            Assert.AreEqual("iasc-its-global", iasc.Alias);

            var nrmIasc = m.InfraIasc[2];
            Assert.AreEqual("iasc-its-gce-rke.yaml", nrmIasc.ValuesFile);
            Assert.AreEqual("helm-terraform-gcp", nrmIasc.ChartId);
            Assert.AreEqual("1.1.5-rc8", nrmIasc.Version);
            Assert.AreEqual("default-3", nrmIasc.Alias);
            Assert.AreEqual("https://its-software-services-devops.github.io/helm-terraform-gcp/", nrmIasc.ChartUrl);    
        }

        [Test]
        public void YamlTransformTest()
        {
          var wf = new WorkflowGeneric();            
          var result = wf.Load(yaml1);

          UtilsHelm.SetCmd("echo");
          wf.Transform();
          UtilsHelm.ResetHelmCmd();
        }

        [Test]
        public void YamlLoadFileTest()
        {
          var path = "dummy.yaml";
          File.WriteAllText(path, yaml1);

          var wf = new WorkflowGeneric();            
          var result = wf.LoadFile(path);

          UtilsHelm.SetCmd("echo");
          wf.Transform();
          UtilsHelm.ResetHelmCmd();
        }
    }
}