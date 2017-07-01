using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapControl.WPF.StaticAnalysis
{
	class Program
	{
		static void Main(string[] args)
		{
			Solution solution = MSBuildWorkspace.Create()
				.OpenSolutionAsync("../../../MapControl.sln")
				.Result;

			Project mapControlWpfProject = solution.Projects.Single(p => p.Name == "MapControl.WPF");

			Compilation compilation = mapControlWpfProject.GetCompilationAsync().Result;

			HashSet<string> Edges = new HashSet<string>();

			foreach (var tree in compilation.SyntaxTrees)
			{
				SemanticModel model = compilation.GetSemanticModel(tree);

				var classNodes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

				foreach (ClassDeclarationSyntax classNode in classNodes)
				{
					var typesForCurrentClass = classNode.DescendantNodes()
														.Select(node => model.GetTypeInfo(node).Type)
														.Where(type => type != null);

					HashSet<ITypeSymbol> typeSymbols = new HashSet<ITypeSymbol>();

					foreach (var type in typesForCurrentClass)
						if (type?.ContainingAssembly?.Name == mapControlWpfProject.AssemblyName)
							typeSymbols.Add(type);

					foreach (var symbol in typeSymbols.Where(s => s != null && s.BaseType != null))
						Edges.Add(symbol.Name + " -> " + symbol.BaseType.Name);
				}
			}

			foreach (var edge in Edges)
			{
				Console.WriteLine(edge);
			}

			Console.ReadKey();
		}
	}
}
