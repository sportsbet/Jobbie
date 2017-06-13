using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

namespace Jobbie.Domain.Tests.Unit
{
    [TestFixture]
    internal sealed class ReferenceTest
    {
        private static readonly string[] _acceptable =
        {
            "mscorlib",
            "System",
            "System.Core"
        };

        [TestCaseSource(nameof(ReferencedAssemblies))]
        public void The_domain_project_should_only_reference_acceptable_assemblies(AssemblyName referenced) =>
            _acceptable
                .Should()
                .Contain(
                    referenced.Name,
                    "the Domain layer is restricted to reference only core .NET libraries.");

        internal static IEnumerable<AssemblyName> ReferencedAssemblies() =>
            Assembly.Load("Jobbie.Domain").GetReferencedAssemblies();
    }
}