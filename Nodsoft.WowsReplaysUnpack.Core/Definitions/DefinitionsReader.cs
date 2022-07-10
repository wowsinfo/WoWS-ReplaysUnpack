using Nodsoft.WowsReplaysUnpack.Core.Extensions;
using System.Reflection;
using System.Xml;

namespace Nodsoft.WowsReplaysUnpack.Core.Definitions;

public class DefinitionDirectory
{
	public string Name { get; }
	public string Path { get; }

	public List<DefinitionFile> Files { get; } = new();
	public List<DefinitionDirectory> Directories { get; } = new();

	public DefinitionDirectory(string name, string path, string[] fileNames)
	{
		Name = name;
		Path = path;
		ParseChildrenRecursive(fileNames);
	}

	private void ParseChildrenRecursive(string[] fileNames)
	{
		fileNames = fileNames.Where(name => name.StartsWith(Path)).ToArray();
		foreach (var fileName in fileNames)
		{
			var actualFileName = fileName.GetStringAfterLength(Path + ".");
			if (actualFileName.Count(c => c == '.') == 1)
			{
				Files.Add(new DefinitionFile(actualFileName, fileName));
			}
			else
			{
				var directoryName = actualFileName.GetStringBeforeIndex('.');
				if (!Directories.Any(d => d.Name == directoryName))
					Directories.Add(new DefinitionDirectory(actualFileName.GetStringBeforeIndex('.'),
						fileName.GetStringBeforeIndex("." + actualFileName.GetStringAfterIndex('.')), fileNames));
			}
		}
	}
}

public class DefinitionFile
{
	public string Name { get; }
	public string Path { get; }
	public DefinitionFile(string name, string path)
	{
		Name = name;
		Path = path;
	}
}

public class DefinitionsReader
{
	private readonly Assembly _assembly;
	private readonly DefinitionDirectory _rootDirectory;

	public DefinitionsReader(Assembly assembly, Version clientVersion)
	{
		_assembly = assembly;
		var scriptsDirectory = JoinPath(_assembly.FullName!.GetStringBeforeIndex(','), "Versions", "_" + clientVersion.ToString().Replace('.', '_'), "scripts");
		var fileNames = assembly.GetManifestResourceNames()
			.Where(name => name.StartsWith(scriptsDirectory))
			.ToArray();

		_rootDirectory = new DefinitionDirectory("scripts", scriptsDirectory, fileNames);
	}

	public XmlDocument GetFileAsXml(string name, params string[] directoryNames)
	{
		var directory = FindDirectory(directoryNames);
		var file = directory.Files.SingleOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
		if (file is null)
			throw new Exception("File could not be found");

		var xmlDocument = new XmlDocument();
		xmlDocument.Load(_assembly.GetManifestResourceStream(file.Path) ?? throw new Exception("File not found"));
		return xmlDocument;
	}


	private DefinitionDirectory FindDirectory(string[] directoryNames)
	{
		DefinitionDirectory folder = _rootDirectory;
		foreach (var folderName in directoryNames)
		{
			var foundFolder = folder.Directories.SingleOrDefault(f => f.Name.Equals(folderName, StringComparison.InvariantCultureIgnoreCase));
			if (foundFolder is null)
				break;
			folder = foundFolder;
		}
		return folder;
	}

	public static string JoinPath(params string[] parts)
		=> string.Join(".", parts);
}
