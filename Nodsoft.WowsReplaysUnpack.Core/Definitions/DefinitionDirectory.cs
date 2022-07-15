using Nodsoft.WowsReplaysUnpack.Core.Extensions;

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
		foreach (string fileName in fileNames)
		{
			string actualFileName = fileName.GetStringAfterLength(Path + ".");
			if (actualFileName.Count(c => c == '.') == 1)
			{
				Files.Add(new DefinitionFile(actualFileName, fileName));
			}
			else
			{
				string directoryName = actualFileName.GetStringBeforeIndex('.');
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
