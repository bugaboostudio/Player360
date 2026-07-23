using System.IO;
using NUnit.Framework;
using UnityEditor;

namespace Player360.Tests.EditMode
{
    /// <summary>
    /// Testes de fumaça do projeto: garantem que a configuração mínima para
    /// gerar um build válido do player está íntegra. Rodam no CI (GameCI) e
    /// também localmente via Window > General > Test Runner.
    /// </summary>
    public class ProjectSmokeTests
    {
        [Test]
        public void BuildSettings_TemPeloMenosUmaCenaHabilitada()
        {
            var cenasHabilitadas = System.Array.FindAll(
                EditorBuildSettings.scenes, scene => scene.enabled);

            Assert.IsNotEmpty(cenasHabilitadas,
                "Nenhuma cena habilitada em File > Build Settings. O build sairia vazio.");

            foreach (var cena in cenasHabilitadas)
            {
                Assert.IsTrue(File.Exists(cena.path),
                    $"A cena '{cena.path}' está no Build Settings mas o arquivo não existe.");
            }
        }

        [Test]
        public void PlayerSettings_IdentificadorAndroidConfigurado()
        {
            var identifier = PlayerSettings.GetApplicationIdentifier(
                UnityEditor.Build.NamedBuildTarget.Android);

            Assert.IsNotEmpty(identifier,
                "O Application Identifier do Android não está configurado.");
            Assert.IsFalse(identifier.Contains("com.DefaultCompany"),
                "O Application Identifier do Android ainda é o padrão da Unity.");
        }

        [Test]
        public void PlayerSettings_ProductNameConfigurado()
        {
            Assert.IsNotEmpty(PlayerSettings.productName,
                "O Product Name do projeto não está configurado.");
        }
    }
}
